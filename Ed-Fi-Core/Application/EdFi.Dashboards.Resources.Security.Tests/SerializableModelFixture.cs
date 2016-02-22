using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Resources.CastleWindsorInstallers;
using EdFi.Dashboards.Resources.Security.CastleWindsorInstallers;
using EdFi.Dashboards.Resources.Tests;
using EdFi.Dashboards.Resources.Tests.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Testing;
using EdFi.Dashboards.Warehouse.Data;
using EdFi.Dashboards.Warehouse.Resources;
using NUnit.Framework;

namespace EdFi.Dashboards.Resources.Security.Tests
{
    public class All_services_should_return_serializable_models : TestFixtureBase
    {
        private string ErrorMessage = "All returned models should be serializable";
        protected StringBuilder errorLog;
        public readonly Assembly securityAssembly = Assembly.GetAssembly(typeof(Marker_EdFi_Dashboards_Resources_Security));
        public readonly Assembly serviceAssembly = Assembly.GetAssembly(typeof(Marker_EdFi_Dashboards_Resources));

        protected IMetricActionUrlAuthorizationProvider metricActionUrlAuthService;

        public virtual IEnumerable<Type> GetServiceTypes()
        {
            var result = CastleWindsorHelper.GetServiceTypesFromIOC(serviceAssembly);

            return result;
        }

        protected virtual void AddRegistration(WindsorContainer container)
        {
            container.Register(Component.For<ISecurityAssertionProvider>().Instance(new SafeMethodOnlySecurityAssertionProvider()));
            // Use the version of the interceptor that doesn't allow any calls to go through
            // container.Register(Component.For<IInterceptor, ApplyViewPermissionsWithNoProceedInterceptor>());

            IoC.Initialize(container);
            container.Register(
                Component
                    .For(typeof(IMetricActionUrlAuthorizationProvider))
                    .Instance(metricActionUrlAuthService));

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
            var it = new Type[] { interfaceType };
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



        protected override void ExecuteTest()
        {
            errorLog = new StringBuilder();

            var serviceTypes = GetServiceTypes();
            foreach (var serviceType in serviceTypes.OrderBy(t => t.Name))
            {
                var methods = GetServiceMethods(serviceType);
                foreach (var method in methods.OrderBy(m => m.Name))
                {
                    if (method.GetAttribute<NoCacheAttribute>() != null)
                        continue;

                    var interfaceMethod = GetInterfaceMethod(serviceType, method);
                    var returnType = interfaceMethod.ReturnType;
                    if (!returnType.IsSerializable)
                    {
                        if (returnType.IsGenericType)
                        {
                            var genericArguments = returnType.GetGenericArguments();
                            foreach (var arg in genericArguments)
                            {
                                if (!arg.IsSerializable)
                                {
                                    errorLog.AppendLine();
                                    errorLog.AppendFormat("Service: {0}  Method: {1} Return Type: {2}", serviceType.FullName, method.Name, returnType.FullName);
                                }
                            }
                            continue;
                        }
                        errorLog.AppendLine();
                        errorLog.AppendFormat("Service: {0}  Method: {1} Return Type: {2}", serviceType.FullName, method.Name, returnType.FullName);
                    }
                }
            }
        }

        [Test]
        public void Should_have_no_errors()
        {
            // If we have no errors, quit now.
            if (errorLog.Length == 0)
                return;

            Assert.That(errorLog.Length, Is.EqualTo(0), ErrorMessage + errorLog);
        }
    }
}
