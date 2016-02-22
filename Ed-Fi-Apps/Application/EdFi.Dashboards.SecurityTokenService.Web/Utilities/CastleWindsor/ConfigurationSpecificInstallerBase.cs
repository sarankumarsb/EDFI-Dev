// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Linq;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Common.Utilities.CastleWindsorInstallers;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Infrastructure.Data;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources.Application;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Security.Implementations;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.SecurityTokenService.Authentication.Analytics;
using EdFi.Dashboards.SecurityTokenService.Web.Providers;
using EdFi.Dashboards.SecurityTokenService.Authentication.Implementations;

namespace EdFi.Dashboards.SecurityTokenService.Web.Utilities.CastleWindsor
{
    public abstract class ConfigurationSpecificInstallerBase : EdFi.Dashboards.SecurityTokenService.Core.Utilities.CastleWindsor.ConfigurationSpecificInstallerBase
    {
        private const string defaultDatabaseSelector = "Default Database Selector";

        [Preregister]
        protected virtual void RegisterIConfigSectionProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IConfigSectionProvider>()
                .ImplementedBy<AppConfigSectionProvider>());
        }

        [Preregister]
        protected virtual void RegisterIConfigValueProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IConfigValueProvider>()
                .ImplementedBy<AppConfigValueProvider>());
        }

        protected virtual void RegisterIFile(IWindsorContainer container)
        {
            container.Register(Component
                .For<IFile>()
                .ImplementedBy<FileWrapper>());
        }

        protected virtual void RegisterISessionStateProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<ISessionStateProvider>()
                .ImplementedBy<AspNetSessionStateProvider>());
        }

        protected virtual void RegisterICurrentUrlProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<ICurrentUrlProvider>()
                .ImplementedBy<AspNetCurrentUrlProvider>());
        }

        protected virtual void RegisterIUserClaimsProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IUserClaimsProvider>()
                .ImplementedBy<DashboardUserClaimsProvider<ClaimsSet, EdFiUserSecurityDetails>>());
        }

        protected virtual void RegisterIDashboardUserClaimsInformationProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IDashboardUserClaimsInformationProvider<EdFiUserSecurityDetails>>()
                .ImplementedBy<QEduDashboardUserClaimsInformationProvider>());
                //.ImplementedBy<DashboardUserClaimsInformationProvider>());
        }

        protected virtual void RegisterIRoleBasedClaimsProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IClaimSetBasedClaimsProvider<ClaimsSet>>()
                .ImplementedBy<ClaimSetBasedClaimsProvider>());
        }

        protected virtual void RegisterICacheProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<ICacheProvider>()
                .ImplementedBy<AspNetCacheProvider>());
        }

        protected virtual void RegisterIAuthorizationInformationProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IAuthorizationInformationProvider>()
                .ImplementedBy<AuthorizationInformationProvider>());
        }

        protected virtual void RegisterIHttpServerProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IHttpServerProvider>()
                .ImplementedBy<HttpServerProvider>());
        }

        protected virtual void RegisterILocalEducationAgencyContextProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<ILocalEducationAgencyContextProvider>()
                .ImplementedBy<SignInRequestLocalEducationAgencyContextProvider>());
        }

        protected virtual void RegisterIDbConnectionStringSelector(IWindsorContainer container)
        {
            //container.Register(Component
            //    .For<IDbConnectionStringSelector>()
            //    .Instance(new NamedDbConnectionStringSelector("MultiLEA")));
            container.Register(Component
                .For<IDbConnectionStringSelector>()
                .ImplementedBy<LocalEducationAgencyContextConnectionStringSelector>());

            container.Register(Component
                .For<IDbConnectionStringSelector>()
                .ImplementedBy<DefaultDbConnectionStringSelector>().Named(defaultDatabaseSelector));
        }

        protected virtual void RegisterICurrentUserClaimInterrogator(IWindsorContainer container)
        {
            container.Register(Component
                                   .For<ICurrentUserClaimInterrogator>()
                                   .Configuration(Attrib.ForName("inspectionBehavior").Eq("none"))
                                   .ImplementedBy<CurrentUserClaimInterrogator>());
        }

        protected virtual void RegisterICurrentUserAccessibleStudentsProvider(IWindsorContainer container)
        {
            container.Register(Component
                                   .For<ICurrentUserAccessibleStudentsProvider>()
                                   .ImplementedBy<CurrentUserAccessibleStudentsProvider>());
        }

        protected virtual void RegisterIErrorLoggingProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IErrorLoggingService>()
                .ImplementedBy<ErrorLoggingService>());
        }

        protected virtual void RegisterIFeedbackProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IFeedbackService>()
                .ImplementedBy<FeedbackService>());
        }

        protected virtual void RegisterIErrorAreaLinks(IWindsorContainer container)
        {
            container.Register(Component
                .For<IErrorAreaLinks>()
                .ImplementedBy<EdFi.Dashboards.Resources.Navigation.WebForms.Areas.Error>());
        }

        protected virtual void RegisterICodeIdProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<ICodeIdProvider>()
                .ImplementedBy<CodeIdProvider>());
        }

        protected virtual void RegisterIStaffInformationProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IStaffInformationProvider>()
                .ImplementedBy<QEduLoginInformationProvider>());
                //.ImplementedBy<StaffInformationProvider>());
        }

        protected virtual void RegisterIGetOutputClaimsIdentityProvider(IWindsorContainer container)
        {
            //Client side claims enrichment - when DashboardClaimsAuthenticationManagerProvider for IClaimsAuthenticationManagerProvider
            container.Register(Component
                .For<IGetOutputClaimsIdentityProvider>()
                .ImplementedBy<IdentityClaimsGetOutputClaimsIdentityProvider>());

            //STS claims enrichment - when PassThroughClaimsAuthenticationManagerProvider for IClaimsAuthenticationManagerProvider
            //container.Register(Component
            //    .For<IGetOutputClaimsIdentityProvider>()
            //    .ImplementedBy<DashboardClaimsGetOutputClaimsIdentityProvider>());
        }

        protected virtual void RegisterIClaimsIssuedTracking(IWindsorContainer container){}

        protected abstract void RegisterIAuthenticationProvider(IWindsorContainer container);
        protected abstract void RegisterIUserRolesProvider(IWindsorContainer container);
    }
}
