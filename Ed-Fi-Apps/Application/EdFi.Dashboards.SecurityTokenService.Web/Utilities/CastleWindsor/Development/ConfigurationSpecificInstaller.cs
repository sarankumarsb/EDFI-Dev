// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdFi.Dashboards.Resources.Security.Implementations;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.SecurityTokenService.Web.Providers;
using IntegrationConfigurationSpecificInstaller = EdFi.Dashboards.SecurityTokenService.Web.Utilities.CastleWindsor.Integration.ConfigurationSpecificInstaller;

namespace EdFi.Dashboards.SecurityTokenService.Web.Utilities.CastleWindsor.Development
{
    public class ConfigurationSpecificInstaller : IntegrationConfigurationSpecificInstaller
    {
        protected override void RegisterIUserClaimsProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IUserClaimsProvider>()
                .ImplementedBy<DashboardDatabaseWithStateAgencySupportUserClaimsProvider>());
        }
    }
}