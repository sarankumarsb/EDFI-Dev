using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdFi.Dashboards.Common.Utilities.CastleWindsorInstallers;
using EdFi.Dashboards.Data.Providers;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Resources.Security.Implementations;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.SecurityTokenService.Authentication.Analytics;
using EdFi.Dashboards.SecurityTokenService.Authentication.Implementations;

namespace EdFi.Dashboards.SecurityTokenService.Core.Utilities.CastleWindsor
{
    public abstract class ConfigurationSpecificInstallerBase : RegistrationMethodsInstallerBase
    {
        protected virtual void RegisterISubsonicDataProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<ISubsonicDataProviderProvider>()
                .ImplementedBy<SubsonicDataProviderProvider>().IsDefault());
        }
        
        protected virtual void RegisterIStaffInformationLookupKeyProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IStaffInformationLookupKeyProvider>()
                .ImplementedBy<StaffInformationLookupKeyProvider>());
        }

        protected virtual void RegisterIClaimsIssuedTrackingProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IClaimsIssuedTrackingEventProvider>()
                .ImplementedBy<ClaimsIssuedTrackingEventProvider>());
        }

        protected virtual void RegisterIHttpRequestProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IHttpRequestProvider>()
                .ImplementedBy<HttpRequestProvider>());
        }

        protected virtual void RegisterIWimpProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IWimpProvider>()
                .ImplementedBy<Base64WctxWimpProvider>());
        }

        protected virtual void RegisterIGetImpersonatedClaimsDataProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IGetImpersonatedClaimsDataProvider>()
                .ImplementedBy<GetImpersonatedClaimsDataProvider>());
        }
    }
}
