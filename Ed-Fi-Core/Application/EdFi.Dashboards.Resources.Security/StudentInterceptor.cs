// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Linq;
using Castle.DynamicProxy;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security
{
    public class StudentInterceptor : IInterceptorStage
    {
        private readonly ICurrentUserAccessibleStudentsProvider currentUserAccessibleStudentsProvider;

        public StudentInterceptor(ICurrentUserAccessibleStudentsProvider currentUserAccessibleStudentsProvider)
        {
            this.currentUserAccessibleStudentsProvider = currentUserAccessibleStudentsProvider;
        }

        public StageResult BeforeExecution(IInvocation invocation, bool topLevelIntercept)
        {
            return new StageResult {Proceed = true};
        }

        public void AfterExecution(IInvocation invocation, bool topLevelIntercept, StageResult state)
        {
            if (!topLevelIntercept)
                return;

            if (AuthenticationIgnoreAttribute.MarkedAuthenticationIgnore(invocation))
                return;

            var authorizationAttribute = GetCanBeAuthorizedByAttribute(invocation);
            if (authorizationAttribute == null || (authorizationAttribute.AuthorizingClaims == null && authorizationAttribute.AuthorizingTypes == null))
                return;
           
            var isSearch = authorizationAttribute.AuthorizingTypes != null &&
                            authorizationAttribute.AuthorizingTypes.Contains(AuthorizationDelegate.Search);

            if ((authorizationAttribute.AuthorizingClaims == null ||
                 (!authorizationAttribute.AuthorizingClaims.Contains(EdFiClaimTypes.ViewAllStudents) &&
                  !authorizationAttribute.AuthorizingClaims.Contains(EdFiClaimTypes.ViewMyStudents))) && !isSearch)
                return;

            object value;
            int? educationOrganization = null;
                
            if (TryGetParameterValue(invocation, "SchoolId", out value))
            {
                if (value != null)
                {
                    educationOrganization = Convert.ToInt32(value);
                    if (educationOrganization == 0)
                    {
                        educationOrganization = null;
                        //throw new Exception("Unable to retrieve school id from parameter value.");
                    }
                }
            }

            if (!educationOrganization.HasValue && TryGetParameterValue(invocation,"LocalEducationAgencyId", out value))
            {
                if (value != null)
                {
                    educationOrganization = Convert.ToInt32(value);
                    if (educationOrganization == 0)
                    {
							throw new InvalidOperationException("Unable to retrieve school id from parameter value.");
                    }
                }
            }

            //Education organization may not be set here, this should primarily only be if it is a statewide call.
            var accessibleStudents = currentUserAccessibleStudentsProvider.GetAccessibleStudents(educationOrganization, isSearch);

            var filter = new StudentFilter(accessibleStudents);
            invocation.ReturnValue = filter.ExecuteFilter(invocation.ReturnValue);
        }

        private static bool TryGetParameterValue(IInvocation invocation, string parameterName, out object value)
        {
            var request = invocation.Arguments[0];

            foreach (var parameterInfo in request.GetType().GetProperties()
                        .Where(parameterInfo => !AuthenticationIgnoreAttribute.MarkedAuthenticationIgnore(parameterInfo) &&
                            parameterInfo.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase)))
            {
                value = parameterInfo.GetValue(request, null);
                return true;
            }

            value = null;
            return false;
        }

        private static CanBeAuthorizedByAttribute GetCanBeAuthorizedByAttribute(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget;
            var attr = method.GetCustomAttributes(typeof(CanBeAuthorizedByAttribute), true).FirstOrDefault() ??
                       ApplyViewPermissionsByClaimInterceptor.GetCanBeAuthorizedByAttributeFromParameter(method);
            return attr as CanBeAuthorizedByAttribute;
        }
        
    }
}
