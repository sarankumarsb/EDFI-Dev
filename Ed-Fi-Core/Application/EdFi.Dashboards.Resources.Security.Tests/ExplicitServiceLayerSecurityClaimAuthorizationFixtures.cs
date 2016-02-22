// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using EdFi.Dashboards.Application.Data;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Data;
using EdFi.Dashboards.Data.CastleWindsorInstallers;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Resources.CastleWindsorInstallers;
using EdFi.Dashboards.Resources.Security.CastleWindsorInstallers;
using EdFi.Dashboards.Resources.Security.ClaimValidators;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Tests;
using EdFi.Dashboards.Resources.Tests.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Testing;
using EdFi.Dashboards.Warehouse.Data;
using EdFi.Dashboards.Warehouse.Resources;
using NUnit.Framework;

namespace EdFi.Dashboards.Resources.Security.Tests
{
    public abstract class ExplicitServiceLayerMethodClaimCoverageFixtureBase : TestFixtureBase
    {
        public const string ErrorMessage =
            @"Errors were encountered while invoking all service layer methods to ensure complete and explicit security coverage.  To rectify the situation, take one of the following actions on each of the items below: 
   1) For UnhandledSignature exceptions:
        a) Implement an appropriate ClaimParameterValidatorXxxxxYyyyy class (where x and y represent the parameter names, excluding the Id suffix if appropriate) to handle the scenario to limit the user's access appropriately based on data ownership.
        b) Add [AuthIgnore] attribute to the parameter of the method in the concrete implementation of the class, or a property of the Request object,  if its inclusion in the method is not going to expose any private data.
        c) Add [AuthIgnore] attribute to the method in the concrete implementation of the class.  Do this only if the method is not going to expose any private data.
        d) Add a [CanBeAuthorizedBy] attribute on the request object associated with the specific service method.
";
        protected StringBuilder errorLog;
        protected readonly List<ClaimAuthorizationResult> authorizationResults = new List<ClaimAuthorizationResult>();

        public readonly Assembly securityAssembly = Assembly.GetAssembly(typeof (Marker_EdFi_Dashboards_Resources_Security));
        public readonly Assembly serviceAssembly = Assembly.GetAssembly(typeof(Marker_EdFi_Dashboards_Resources));

        protected IMetricActionUrlAuthorizationProvider metricActionUrlAuthService;

        public virtual IEnumerable<Type> GetServiceTypes()
        {
            var result = CastleWindsorHelper.GetServiceTypesFromIOC(serviceAssembly);

            return result;
        }

        public virtual IEnumerable<Type> GetAllValidators()
        {
            var result = GetAllValidatorsFromAssembly(securityAssembly);

            return result;
        }

        protected IEnumerable<Type> GetAllValidatorsFromAssembly(Assembly assembly)
        {
            var allTypes = assembly.GetTypes();
            var validatorType = typeof(ClaimValidatorBase);

            var claimTypes =
                (from t in allTypes
                 where validatorType.IsAssignableFrom(t) &&
                       t.IsAbstract == false
                 orderby t.Name
                 select t);

            return claimTypes;
        }

        protected void WriteClaimSignatureSet( HashSet<string> claimSignatures)
        {
            Debug.WriteLine("Claim Signatures: ");
            foreach (var claimSignature in claimSignatures.OrderBy(n => n))
            {
                Debug.WriteLine("\t" + claimSignature);
            }
        }

        protected HashSet<string> BuildClaimSignatureSet()
        {
            try
            {
                var result = new HashSet<string>();

                var objs = new[] { new SafeMethodOnlySecurityAssertionProvider(), null };

                var claimTypes = GetAllValidators();

                foreach (var claimType in claimTypes)
                {
                    var instance = (ClaimValidatorBase)Activator.CreateInstance(claimType, objs);
                    var key = instance.GetValidatorKey();

                    if (false == result.Add(key)) throw new ArgumentException("Duplicate Claim Signature: " + key);
                }

                return result;
            }
            catch (Exception exception)
            {
                Debug.WriteLine("BuildClaimSignatureSet: " + exception.Message);
                throw;
            }
        }

        protected virtual void AddRegistration( WindsorContainer container)
        {
            container.Register(Component.For<ISecurityAssertionProvider>().Instance(new SafeMethodOnlySecurityAssertionProvider()));
            // Use the version of the interceptor that doesn't allow any calls to go through
            // container.Register(Component.For<IInterceptor, ApplyViewPermissionsWithNoProceedInterceptor>());

            //container.Install(new InfrastructureServiceInstaller());

            container.Register(
                Component
                    .For(typeof(IMetricActionUrlAuthorizationProvider))
                    .Instance(metricActionUrlAuthService));

            // Initialize the service locator with the container 
            // (enabling installers to access container through IoC during registration process)
            IoC.Initialize(container);
            container.Install(new TestConfigurationSpecificInstaller());
            container.Install(new SecurityComponentsInstaller(typeof(ApplyViewPermissionsByClaimWithNoProceedInterceptor)));
            container.Install(new MetricServiceInstaller());
            container.Install(new GenericServiceInstaller<Marker_EdFi_Dashboards_Warehouse_Resources>());
            container.Install(new GenericServiceInstaller<Marker_EdFi_Dashboards_Resources>());
            container.Install(new RepositoryInstaller<Marker_EdFi_Dashboards_Data>());
            container.Install(new RepositoryInstaller<Marker_EdFi_Dashboards_Metric_Data_Entities>());
            container.Install(new PersistedRepositoryInstaller<Marker_EdFi_Dashboards_Application_Data>());
            container.Install(new QueryInstaller<Marker_EdFi_Dashboards_Data>());
            container.Install(new RepositoryInstaller<Marker_EdFi_Dashboards_Warehouse_Data>());

            container.Register(
                Classes.FromAssembly(securityAssembly)
                .BasedOn<IAuthorizationDelegate>()
                .WithService.FirstInterface(),
                Component.For<AuthorizationDelegateMap>());

            // Register the wrapped service locator singleton instance
            container.Register(
                Component.For<IServiceLocator>()
                    .Instance(IoC.WrappedServiceLocator));
        }

        protected override void EstablishContext()
        {

            metricActionUrlAuthService = mocks.StrictMock<IMetricActionUrlAuthorizationProvider>();

            var container = new WindsorContainer();

            // Add the array resolver for resolving arrays of services automatically
            container.Kernel.Resolver.AddSubResolver(new ArrayResolver(container.Kernel));

            AddRegistration(container);

            Thread.CurrentPrincipal = new UserInformation().ToClaimsPrincipal();
        }

        protected IEnumerable<MethodInfo> GetServiceMethods(Type serviceType)
        {
            try
            {
                var implementation = IoC.Resolve(serviceType);
                var impType = implementation.GetType().GetField("__target").GetValue(implementation).GetType();

                // Get public methods on service
                var methods = impType.GetMethods();
                var serviceMethods = serviceType.GetMethods().Concat(serviceType.GetInterfaces().SelectMany(i => i.GetMethods()));


                // Find public methods on class which are also defined on the interface (by name only, not entire signature for simplicity)
                var methodsToInvoke =
                    from m in methods
                    where serviceMethods.Any(sm => sm.Name == m.Name && sm.IsPublic)
                    select m;

                return methodsToInvoke;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                throw;
            }
        }

        protected MethodInfo GetInterfaceMethod(Type interfaceType, MethodInfo classMethod)
        {
            var reflectedType = classMethod.ReflectedType;
            var it = new Type[] {interfaceType};
            var interfaces = it.Concat(interfaceType.GetInterfaces());

            foreach (var type in interfaces)
            {
                var interfaceMap = reflectedType.GetInterfaceMap(type);

                var index = Array.IndexOf(interfaceMap.TargetMethods, classMethod);
                if (index != -1)
                    return interfaceMap.InterfaceMethods[index];
            }

            return null;
        }

        protected void WriteDelegates()
        {
            var authorizationDelegateDictionary = IoC.Resolve<AuthorizationDelegateMap>().AuthorizationDelegateDictionary;

            Debug.WriteLine("\n\nAuthorizationDelegates: ");
            foreach (var authorizationDelegate in authorizationDelegateDictionary.Keys.OrderBy( k => k.Name))
            {
                Debug.WriteLine("\t" + authorizationDelegate.Name);
            }
            Debug.WriteLine("\n");
        }

        protected bool AuthorizeByDelegate( MemberInfo method)
        {
            var attr = (CanBeAuthorizedByAttribute)method.GetCustomAttributes(typeof(CanBeAuthorizedByAttribute), true).FirstOrDefault();
            if (null == attr) return false;

            var authorizingTypes = attr.AuthorizingTypes;
            return (null != authorizingTypes) && (0 != authorizingTypes.Count());
        }

        protected bool AuthorizeIgnore(MemberInfo method)
        {
            var attr = method.GetCustomAttributes(typeof(AuthenticationIgnoreAttribute), true).FirstOrDefault();

            return (null != attr);
        }

        protected string[] GetCanBeAuthorizedByClaims(Type serviceType, MethodInfo method, string parametersText)
        {
            var methodName = method.Name;

            var attr = (CanBeAuthorizedByAttribute)method.GetCustomAttributes(typeof(CanBeAuthorizedByAttribute), true).FirstOrDefault() ??
                       ApplyViewPermissionsByClaimInterceptor.GetCanBeAuthorizedByAttributeFromParameter(method);

            // log that there is no authorizing claims.
            if (null == attr)
            {
                authorizationResults.Add(
                    new ClaimAuthorizationResult
                    {
                        Claim = "None:",
                        InterfaceName = method.ReflectedType.FullName,
                        MethodName = methodName,
                        Parameters = parametersText,
                        ExceptionName = "NoAuthorizingClaims",
                        ExceptionMessage = "The Authorizing Claims Attribute does not exist for this method."
                    });
                return null;
            }

            var claimTypes = attr.AuthorizingClaims;
            if (0 != claimTypes.Count()) return claimTypes;

            authorizationResults.Add(
                new ClaimAuthorizationResult
                {
                    Claim = "None:",
                    InterfaceName = serviceType.FullName,
                    MethodName = methodName,
                    Parameters = parametersText,
                    ExceptionName = "NoAuthorizingClaims",
                    ExceptionMessage = "The Authorizing Claims Attribute exists, but is empty."
                });

            return null;
        }

        protected override void ExecuteTest()
        {
            errorLog = new StringBuilder();

            var claimSignatureSet = BuildClaimSignatureSet();

            //WriteClaimSignatureSet(claimSignatureSet);
            //WriteDelegates();

            var usedKeys = new HashSet<string>();

            var serviceTypes = GetServiceTypes();
            Debug.WriteLine("ServiceType.Method Claims.");
            foreach (var serviceType in serviceTypes.OrderBy(t => t.Name))
            {
                Debug.WriteLine( "\nServiceType: " + serviceType);
                var methods = GetServiceMethods(serviceType);
                foreach (var method in methods.OrderBy(m => m.Name))
                {
                    // Since the AuthIgnore attribute is on the interface, get the
                    // method on the interface.
                    //
                    var interfaceMethod = GetInterfaceMethod(serviceType, method);
                    var parameters = interfaceMethod.GetParameters();

                    var sig = ApplyViewPermissionsByClaimInterceptor.BuildSignatureKey(parameters);
                    if (String.IsNullOrEmpty(sig)) continue;

                    var parametersText =
                        string.Join(", ",
                                    (from p in parameters
                                     orderby p.Name
                                     select p.Name).ToArray());

                    if ((AuthorizeIgnore(method)) || (AuthorizeByDelegate(method))) continue;

                    var claimTypes = GetCanBeAuthorizedByClaims(serviceType, method, parametersText);

                    if (null == claimTypes) continue;

                    var parameterSignature = ApplyViewPermissionsByClaimInterceptor.BuildSignatureKey(parameters);

                    Debug.WriteLine( "\t" + serviceType + "." + method.Name + "(" + parameterSignature + ")");
                    foreach (var claimType in claimTypes.OrderBy( ct => ct.ToString()))
                    {
                        var claimName = ClaimHelper.GetClaimShortName(claimType);
                        Debug.WriteLine("\t\t" + claimName);
                        var requiredKey = string.Format("{0}:{1}", claimName, parameterSignature).ToLower();

                        usedKeys.Add(requiredKey);
                        if (claimSignatureSet.Contains(requiredKey)) continue;


                        authorizationResults.Add(
                            new ClaimAuthorizationResult
                                {
                                    Claim = requiredKey,
                                    InterfaceName = serviceType.FullName,
                                    MethodName = method.Name,
                                    Parameters = parametersText + "):(" + requiredKey,
                                    ExceptionName = "AuthorizingClaimNotSupported",
                                    ExceptionMessage = "The Authorizing Claim is not supported."
                                });
                    }
                }
            }

            Debug.WriteLine("\n\nUnused Claim Signatures: ");
            foreach (var key in claimSignatureSet)
            {
                if (!usedKeys.Contains(key))
                {
                    Console.WriteLine( "\t\t" + key);
                }
            }
        }

        protected void ReportResults()
        {
            var groupedResults =
                from r in authorizationResults
                orderby r.InterfaceName, r.MethodName
                group r by r.Claim into g
                select g;

            int maxMethodNameLength =
                (from r in authorizationResults
                 select (r.InterfaceName.Length + 1 + r.MethodName.Length + r.Parameters.Length + 2))
                    .Max();

            foreach (var grouping in groupedResults)
            {
                errorLog.AppendLine(grouping.Key + ":");

                foreach (var result in grouping)
                {
                    errorLog.AppendFormat("\t{0}.{1}({2}):{5}({3}) {4}\n",
                                          result.InterfaceName, result.MethodName, result.Parameters, result.ExceptionName, result.ExceptionMessage,
                                          new string(' ', maxMethodNameLength + 3 - (result.InterfaceName.Length + result.MethodName.Length + result.Parameters.Length + 2)));
                }

                errorLog.AppendLine("------------------------------------------------------------------------");
            }
        }


        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            CallContext.SetData("StageInterceptor", null);
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            CallContext.SetData("StageInterceptor", null);
        }
    }

    
    public class ExplicitServiceLayerMethodClaimCoverageFixture : ExplicitServiceLayerMethodClaimCoverageFixtureBase
    {
        [Test]
        public void Should_have_no_errors()
        {
            // If we have no errors, quit now.
            if (authorizationResults.Count == 0)
                return;

            ReportResults();

            Assert.That(errorLog.Length, Is.EqualTo(0), ErrorMessage + errorLog);
        }
    }

    public class ClaimAuthorizationResult
    {
        public ClaimAuthorizationResult()
        {
            Claim = string.Empty;
            InterfaceName = string.Empty;
            MethodName = string.Empty;
            Parameters = string.Empty;
            ExceptionName = string.Empty;
            ExceptionMessage = string.Empty;
        }

        public string Claim { get; set; }
        public string InterfaceName { get; set; }
        public string MethodName { get; set; }
        public string Parameters { get; set; }
        public string ExceptionName { get; set; }
        public string ExceptionMessage { get; set; }
    }

    public static class ClaimExceptionExtensions
    {
        public static IEnumerable<Exception> ClaimExceptions(this Exception ex)
        {
            var temp = ex;

            yield return temp;

            while (temp.InnerException != null)
            {
                temp = temp.InnerException;
                yield return temp;
            }
        }

        public static Exception InnermostException(this Exception ex)
        {
            var temp = ex;

            while (temp.InnerException != null)
            {
                temp = temp.InnerException;
            }

            return temp;
        }
    }
}

