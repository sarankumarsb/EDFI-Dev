// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Security;
using System.Text;
using Castle.DynamicProxy;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Security.ClaimValidators;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using Microsoft.IdentityModel.Claims;
using log4net;


namespace EdFi.Dashboards.Resources.Security
{

    public class ApplyViewPermissionsByClaimWithNoProceedInterceptor : ApplyViewPermissionsByClaimInterceptor
    {
        public ApplyViewPermissionsByClaimWithNoProceedInterceptor(IClaimAuthorization[] claimAuthorizations, IAuthorizationDelegate[] authorizationDelegates, ISupportedClaimNamesProvider supportedClaimNamesProvider)
            : base(claimAuthorizations, authorizationDelegates, supportedClaimNamesProvider) { }

        protected override bool CanProceed(IInvocation invocation)
        {
            // Do nothing, we don't want the service method called.
            throw new NotSupportedException("Call was authorized.");
        }
    }

    public class ApplyViewPermissionsByClaimInterceptor : IInterceptorStage
    {
        private readonly ISupportedClaimNamesProvider supportedClaimNamesProvider;
        private static readonly ILog logger = LogManager.GetLogger(typeof(ApplyViewPermissionsByClaimInterceptor));

        private readonly Dictionary<string, IClaimAuthorization> claimAuthorizationsByName = new Dictionary<string, IClaimAuthorization>();
        private readonly Dictionary<string, IAuthorizationDelegate> authorizationDelegatesByTypeName = new Dictionary<string, IAuthorizationDelegate>();

        private static readonly string[] predefinedSafeParameters = new[] { "metricvariantid" };

        public ApplyViewPermissionsByClaimInterceptor(IClaimAuthorization[] claimAuthorizations, IAuthorizationDelegate[] authorizationDelegates, ISupportedClaimNamesProvider supportedClaimNamesProvider)
        {
            this.supportedClaimNamesProvider = supportedClaimNamesProvider;
            InitClaimAuthorizations(claimAuthorizations);
            InitAuthorizationDelegates(authorizationDelegates);
        }

        private void InitClaimAuthorizations(IEnumerable<IClaimAuthorization> claimAuthorizations)
        {
            // Validate the claim authorizations to make sure that they're all provided
            ValidateClaimAuthorizationImplementations(claimAuthorizations);

            foreach (var ca in claimAuthorizations)
            {
                claimAuthorizationsByName.Add(ca.GetType().Name.Replace("ClaimAuthorization", String.Empty).ToLower(), ca);
            }
        }

        private void InitAuthorizationDelegates(IEnumerable<IAuthorizationDelegate> authorizationDelegates)
        {
            foreach (var authorizationDelegate in authorizationDelegates)
            {
                authorizationDelegatesByTypeName.Add(authorizationDelegate.GetType().Name.ToLower(), authorizationDelegate);
            }
        }

        private void ValidateClaimAuthorizationImplementations(IEnumerable<IClaimAuthorization> claimAuthorizationsImplementations)
        {
            var claimNames = supportedClaimNamesProvider.GetSupportedClaimNames();

            var targetedClaimNames =
                from a in claimAuthorizationsImplementations
                select a.GetType().Name.Replace("ClaimAuthorization", "");

            var sb = new StringBuilder();

            foreach (var targetedClaimName in targetedClaimNames)
            {
                if (!claimNames.Contains(targetedClaimName))
                    sb.AppendLine(string.Format("Claim Authorization targeting non-existing claim '{0}' was found.", targetedClaimName));
            }

            foreach (var claimName in claimNames)
            {
                if (!targetedClaimNames.Contains(claimName))
                    sb.AppendLine(string.Format("No Claim Authorization implementation was provided for claim '{0}'.", claimName));
            }

            if (sb.Length > 0)
                throw new SecurityConfigurationException(sb.ToString());
        }

        private static Type GetRequestObjectTypeFirstParameter(MethodInfo method)
        {
            var parameters = method.GetParameters();
            if (parameters.Count() != 1) return null;

            var parameter = parameters.ElementAt(0);
            if (parameter == null) return null;

            var type = parameter.ParameterType;
            if ((!type.IsClass) ||
                (type == typeof(string))) return null;

            return type;
        }

        /// <summary>
        /// Get the CanBeAuthorizedByAttribute, from a Request Object.
        /// 
        /// A RequestObject is defined as a method with a single non null parameter,
        /// that is a type, and is not a string.
        /// 
        /// Returns null if the parameters is not a RequestObject, or if the attribute is not present.
        /// 
        /// This is exposed as a static method such that the unit tests may execute the same accessor.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static CanBeAuthorizedByAttribute GetCanBeAuthorizedByAttributeFromParameter(MethodInfo method)
        {
            var type = GetRequestObjectTypeFirstParameter(method);
            if (type == null) return null;

            var attr = (CanBeAuthorizedByAttribute)type.GetCustomAttributes(typeof(CanBeAuthorizedByAttribute), true).FirstOrDefault();

            return attr;
        }

		private static CanBeAuthorizedByAttribute GetCanBeAuthorizedByAttribute(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget;
            var attr = method.GetCustomAttributes(typeof (CanBeAuthorizedByAttribute), true).FirstOrDefault() ??
                       GetCanBeAuthorizedByAttributeFromParameter(method);

            return (CanBeAuthorizedByAttribute) (attr ?? new CanBeAuthorizedByAttribute(new string[0]));
        }

        protected bool AuthorizeByDelegate(IEnumerable<ParameterInstance> parameters, IEnumerable<AuthorizationDelegate> delegateTypes)
        {
            Exception firstAuthorizationException = null;

            if ((null == delegateTypes) || (delegateTypes.Count() <= 0)) 
                return false;

            // Get the suffix by trimming off the leading I of the interface name
            string delegateSuffix = typeof(IAuthorizationDelegate).Name.Substring(1);

            foreach (var delegateTypeName in delegateTypes.Select(dt => dt.ToString() + delegateSuffix))
            {
                try
                {
                    string key = delegateTypeName.ToLower();

                    if (!authorizationDelegatesByTypeName.ContainsKey(key))
                    {
                        throw new ArgumentException(
                            string.Format("Unable to find the '{0}' authorization delegate type.", delegateTypeName));
                    }
                    
                    var authorizationDelegate = authorizationDelegatesByTypeName[key];
                    authorizationDelegate.AuthorizeRequest(parameters);
                    return true;
                }
                catch (Exception exception)
                {
                    if (firstAuthorizationException == null)
                        firstAuthorizationException = exception;
                }
            }

            // If all delegates failed authorization, throw the first exception
            if (firstAuthorizationException != null) 
                throw firstAuthorizationException;
            
            throw new SecurityAccessDeniedException("Could not authorized by delegate.");
        }

        public bool AuthorizeByClaim(IEnumerable<ParameterInstance> parameters, IEnumerable<string> claims)
        {
            Exception firstAuthorizationException = null;
            var allExceptions = new List<Exception>();

            if ((null == claims) || (!claims.Any())) return false;

            var request = new ClaimValidatorRequest {Parameters = parameters};

            foreach (var claim in claims)
            {
                try
                {
                    var authorization = GetClaimAuthorization(claim);
                    authorization.AuthorizeRequest(request);
                    return true;
                }
                catch (InvalidCastException)
                {
                    // Several claim validators have been throwing invalid cast exceptions which have been hidden by the catch block below
                    throw;
                }
                catch (Exception ex)
                {
                    if (firstAuthorizationException == null)
                        firstAuthorizationException = ex;
                    allExceptions.Add(ex);
                }
            }

            // If all claims failed authorization, throw the first exception.
            // However the first exception is not necessarily the most useful message. So,
            // we'll log all the information we have here to allow better debugging
            // of failures. Also look at the exception thrown in the BeforeExecution method for the 
            // service name and method that it was trying to authorize.
            if (firstAuthorizationException != null)
            {
                var sb = new StringBuilder();
                sb.AppendFormat("Authorization Failure: Parameters: [{0}]  Claims: [{1}]", string.Join(", ", parameters), string.Join(", ", claims));
                sb.AppendLine();

                var claimsPrincipal = UserInformation.Current.ToClaimsPrincipal();

                foreach (var identity in claimsPrincipal.Identities)
                    foreach (var claim in identity.Claims)
                        sb.AppendLine(claim.ClaimType + ": " + claim.Value);

                foreach (var ex in allExceptions)
                    sb.AppendLine(ex.ToString());

                logger.Warn(sb);
                throw firstAuthorizationException;
            }

            throw new SecurityAccessDeniedException("Could not authorize by claim.");
        }

        private bool AuthorizeByAttribute(IInvocation invocation)
        {

            var parms = invocation.Method.GetParameters();
            var args = invocation.Arguments;
            var parameters = GetParameterInstancesForValidation(parms, args).ToList();

            if (parameters.Count == 0) return true;

            var attr = GetCanBeAuthorizedByAttribute(invocation);
            if (null == attr)
            {
                throw new SecurityAccessDeniedException("Could not get CanBeAuthorizedByAttribute");
            }

            var result = ((AuthorizeByDelegate(parameters, attr.AuthorizingTypes)) ||
                          (AuthorizeByClaim(parameters, attr.AuthorizingClaims)));

            return result;
        }

        public StageResult BeforeExecution(IInvocation invocation, bool topLevelIntercept)
        {
            if (!topLevelIntercept)
                return new StageResult { Proceed = CanProceed(invocation) };

            Exception proceedException = null;

            try
            {
                if ((AuthenticationIgnoreAttribute.MarkedAuthenticationIgnore(invocation)) ||
                    (AuthorizeByAttribute(invocation)))
                {
                    try
                    {
                        // If we're still here, proceed!
                        return new StageResult {Proceed = CanProceed(invocation)};
                    }
                    catch (Exception ex)
                    {
                        proceedException = ex;
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                // Cascade proceed exception all the way out
                if (proceedException != null)
                    throw;

                var attr = GetCanBeAuthorizedByAttribute(invocation);

                // Wrap authorization exception with contextual information
                throw new SecurityAccessDeniedException(
                    string.Format("Access denied while authorizing call to '{0}.{1}', which can be authorized using the following claim(s):\n\n\t{2}\n\nAre you missing a claim?",
                                  invocation.InvocationTarget.GetType().Name,
                                  invocation.Method.Name,
                                  string.Join("\n\t",attr.AuthorizingClaims)),
                    ex);
            }

            throw new UserAccessDeniedException("You do not have permission to view the requested data.");
        }

        public void AfterExecution(IInvocation invocation, bool topLevelIntercept, StageResult state)
        {
        }

        protected virtual bool CanProceed(IInvocation invocation)
        {
            return true;
        }

        public static IEnumerable<ParameterInstance> GetParameterInstancesFromRequestObject(ParameterInfo parameterInfo, object requestObjectInstance)
        {
            var type = (null == requestObjectInstance) ? parameterInfo.ParameterType : requestObjectInstance.GetType();

            var props = type.GetProperties();
            return from prop in props
                   where !DoNotValidateProperty(prop)
                   let value = (null == requestObjectInstance) ? prop.PropertyType.GetDefault(): prop.GetValue(requestObjectInstance, null)
                   select new ParameterInstance
                              {
                                  Name = prop.Name,
                                  ParameterInfo = parameterInfo,
                                  Value = value,
                              };
        }

        public static IEnumerable<ParameterInstance> GetParameterInstancesForValidation(IEnumerable<ParameterInfo> parameterInfos, IEnumerable<object> arguments)
        {
            int i = -1;

            foreach (var parmInfo in parameterInfos)
            {
                i++;
                var value = (arguments == null) ? null : arguments.ElementAt(i);

                if (DoNotValidateParameter(parmInfo)) continue;

                // If the parameter is an object then lets get its internal properties and values.
                if (parmInfo.ParameterType.IsClass && !(parmInfo.ParameterType == typeof(string)))
                {
                    var parms = GetParameterInstancesFromRequestObject(parmInfo, value);

                    foreach (var parm in parms)
                    {
                        yield return parm;
                    }
                }
                else
                {
                    yield return
                        new ParameterInstance
                        {
                            Name = parmInfo.Name,
                            ParameterInfo = parmInfo,
                            Value = value,
                        };
                }
            }
        }

        private static bool DoNotValidateProperty(PropertyInfo propertyInfo)
        {
            var result = PredefinedSafe(propertyInfo.Name) || AuthenticationIgnoreAttribute.MarkedAuthenticationIgnore(propertyInfo);
            return result;
        }

        private static bool DoNotValidateParameter(ParameterInfo parmInfo)
        {
            var result = PredefinedSafe(parmInfo.Name) || AuthenticationIgnoreAttribute.MarkedAuthenticationIgnore(parmInfo);
            return result;
        }

        private static bool PredefinedSafe(string name)
        {
            var result = predefinedSafeParameters.Any(p => p.ToLower() == name.ToLower());
            return result;
        }

        public static string BuildSignatureKey(IEnumerable<ParameterInfo> rawParameters)
        {
            // Use the same code for building the list of parameters for validation to
            // build the SignatureKey.  This avoids the implementations getting 'out of sync'.
            var parameterInstances = GetParameterInstancesForValidation(rawParameters, null);

            if ((parameterInstances == null) || (false == parameterInstances.GetEnumerator().MoveNext())) return "";

            var key = (from p in parameterInstances
                       let n = p.Name.ToLower()
                       orderby n
                       select n).Aggregate((total, next) => total + "|" + next);

            return key;
        }

        protected IClaimAuthorization GetClaimAuthorization(string claimName)
        {
            var key = ClaimHelper.GetClaimShortName(claimName);
            if (null == key) return null;

            if (!claimAuthorizationsByName.ContainsKey(key))
                throw new ArgumentException(string.Format("Could not find authorization implementation for claim '{0}'.", key));

            return claimAuthorizationsByName[key];
        }
    }
}
