// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Infrastructure.CastleWindsorInstallers;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Security.ClaimValidators;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Security.Implementations;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security.CastleWindsorInstallers
{
    public class SecurityComponentsInstaller : IWindsorInstaller
    {
        private readonly Type explicitInterceptorType;

        public SecurityComponentsInstaller() { }

        /// <summary>
        /// Constructor intended to be used for testing coverage of security model, so that methods are not actually invoked.
        /// </summary>
        /// <param name="applyViewPermissionInterceptorType">The interceptor to use for service method invocation testing.</param>
        public SecurityComponentsInstaller(Type applyViewPermissionInterceptorType)
        {
            explicitInterceptorType = applyViewPermissionInterceptorType;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var assemblyForSecurity = Assembly.GetAssembly(typeof(Marker_EdFi_Dashboards_Resources_Security));
            var allTypesForSecurity = assemblyForSecurity.GetTypes();

            RegisterClaims(container);
            RegisterAuthorizationDelegates(container, assemblyForSecurity);
            RegisterInterceptors(container, assemblyForSecurity);
            RegisterProviders(container, allTypesForSecurity);
        }

        private static Dictionary<string, string> GetClaimValidatorKeysByClaimName(IWindsorContainer container, IEnumerable<Type> securityTypes)
        {
            var claimNames = GetSupportedClaimNames();
            var claimValidatorKeysByClaim = new Dictionary<string, string>();

            // Register Decorator chains for each claim, based on conventions.
            //
            claimNames.Each(claimName =>
                                {
                                    // Get all the role parameter validators for this role, by convention
                                    var decoratorTypes =
                                        (from t in securityTypes
                                         //Assembly.GetExecutingAssembly().GetTypes()
                                         where ((t.Name.StartsWith(claimName + "ClaimValidator")) &&
                                                (false == t.IsAbstract))
                                         select t)
                                            .ToList();

                                    // DJWhite, 5 Dec 2011 There are no explicitly unhandled signatures at this time.
                                    //
                                    var decoratorRegistrar = new ChainOfResponsibilityRegistrar(container);
                                    var key = decoratorRegistrar.RegisterChainOf<IClaimValidator, NullClaimValidator>(decoratorTypes.ToArray(), claimName);

                                    claimValidatorKeysByClaim[claimName] = key;
                                });
            return claimValidatorKeysByClaim;
        }

        public static IEnumerable<string> GetSupportedClaimNames()
        {
            var type = typeof(EdFiClaimTypes);
            var fields = type.GetFields();
            var result = (from field in fields
                          let fieldValue = field.GetValue(null)
                          where ((fieldValue.ToString().StartsWith(EdFiClaimTypes._OrgClaimNamespace)) &&
                                 (!fieldValue.Equals(EdFiClaimTypes._OrgClaimNamespace)))
                          select field.Name).OrderBy(s => s);

            return result;
        }

        public static void RegisterClaims(IWindsorContainer container)
        {
            var assemblyForClaims = Assembly.GetAssembly(typeof(IClaimValidator));
            var allTypesForClaims = assemblyForClaims.GetTypes();

            var parmValidatorKeysByClaimName = GetClaimValidatorKeysByClaimName(container, allTypesForClaims);
            RegisterClaimAuthorizations(container, parmValidatorKeysByClaimName, assemblyForClaims);
        }

        private static void RegisterAuthorizationDelegates(IWindsorContainer container, Assembly securityAssembly)
        {
            container.Register(
                Classes.FromAssembly(securityAssembly)
                    .BasedOn<IAuthorizationDelegate>()
                    .WithService.FirstInterface());
        }

        private static void RegisterClaimAuthorizations(IWindsorContainer container, Dictionary<string, string> parmValidatorKeysByClaimName, Assembly securityAssembly)
        {
            var claimAuthorizationRegex = new Regex("^(?<ClaimName>[A-Za-z]+)ClaimAuthorization$");

            container.Register(
                Classes.FromAssembly(securityAssembly)
                    .BasedOn<IClaimAuthorization>()
                    .WithService.FirstInterface()
                    .Configure(c =>
                                   {
                                       var match = claimAuthorizationRegex.Match(c.Implementation.Name);

                                       // Make sure implementation class name follows conventions
                                       if (!match.Success)
                                           throw new ConventionsException(
                                               String.Format(
                                                   "IClaimAuthorization implementation '{0}' does not follow expected convention of I{{ClaimName}}ClaimAuthorization.",
                                                   c.Implementation.Name));

                                       var claimName = match.Groups["ClaimName"].Value;
                                       var key = parmValidatorKeysByClaimName[claimName];

                                       c.DependsOn(
                                           Dependency.OnComponent("claimValidator", key));

                                   })
                );
        }

        private void RegisterInterceptors(IWindsorContainer container, Assembly securityAssembly)
        {
            // Register all the IInterceptorStage implementations
            container.Register(
                Classes.FromAssembly(securityAssembly).BasedOn<IInterceptorStage>());

            // Register the domain service stage interceptor
            container.Register(
                Component
                    .For<IInterceptor>()
                    //.For(typeof(StageInterceptor))
                    .Named(SecurityGlobals.DomainServiceStageInterceptorKey)
                    .UsingFactoryMethod(CreateStageInterceptor));

            // Register the cache-only stage interceptor
            container.Register(
                Component
                    .For(typeof(StageInterceptor))
                    .Named(SecurityGlobals.CacheOnlyStageInterceptorKey)
                    .UsingFactoryMethod(CreateCachingStageInterceptor));
        }

        private StageInterceptor CreateStageInterceptor()
        {
            var stages = new Lazy<IInterceptorStage>[5];

            stages[0] = explicitInterceptorType != null ? new Lazy<IInterceptorStage>(() => (IInterceptorStage) IoC.Resolve(explicitInterceptorType)) : new Lazy<IInterceptorStage>(IoC.Resolve<ApplyViewPermissionsByClaimInterceptor>);
            stages[1] = new Lazy<IInterceptorStage>(IoC.Resolve<UserContextInterceptor>);
            stages[2] = new Lazy<IInterceptorStage>(IoC.Resolve<StudentInterceptor>);
            stages[3] = new Lazy<IInterceptorStage>(IoC.Resolve<MetricInterceptor>);
            stages[4] = new Lazy<IInterceptorStage>(IoC.Resolve<CacheInterceptor>);

            return new StageInterceptor(stages);
        }

        private static void RegisterProviders(IWindsorContainer container, Type[] allTypes)
        {
            var providersToRegister =
                from serviceType in allTypes
                    .Where(t => t.Name.StartsWith("I")
                                && t.Name.EndsWith("Provider"))
                from concreteType in allTypes
                    .Where(t => t.Name == serviceType.Name.Substring(1))
                select new
                {
                    ServiceType = serviceType,
                    ConcreteType = concreteType,
                    Cache = concreteType.GetCustomAttributes(typeof(CacheAllMethods), true).Any(),
                };

            foreach (var pair in providersToRegister.Where(pair => !container.Kernel.HasComponent(pair.ServiceType)))
            {
                if (pair.Cache)
                {
                    // Register cached providers
                    container.Register(
                        Component
                            .For(pair.ServiceType)
                            .ImplementedBy(pair.ConcreteType)
                            .Interceptors(SecurityGlobals.CacheOnlyStageInterceptorKey));
                }
                else
                {
                    // Register non-cached providers
                    container.Register(
                        Component
                            .For(pair.ServiceType)
                            .ImplementedBy(pair.ConcreteType));
                }
            }
        }

        private static StageInterceptor CreateCachingStageInterceptor()
        {
            var stages = new Lazy<IInterceptorStage>[1];
            stages[0] = new Lazy<IInterceptorStage>(IoC.Resolve<CacheInterceptor>);
            return new StageInterceptor(stages);
        }
    }
}
