// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdFi.Dashboards.Infrastructure.CastleWindsorInstallers;
using EdFi.Dashboards.Resources.Security.Implementations;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.SecurityTokenService.Authentication.Analytics;
using EdFi.Dashboards.SecurityTokenService.Web.Providers;

namespace EdFi.Dashboards.SecurityTokenService.Web.Utilities.CastleWindsor.Integration
{
	public class ConfigurationSpecificInstaller : ConfigurationSpecificInstallerBase
	{
		protected override void RegisterIAuthenticationProvider(IWindsorContainer container)
		{
			container.Register(Component
				.For<IAuthenticationProvider>()
				.ImplementedBy<AlwaysValidAuthenticationProvider>()
				.DynamicParameters((k, d) => d["emailDomain"] = "edfi.org"));
		}

		protected override void RegisterIUserRolesProvider(IWindsorContainer container)
		{
			var registrar = new ChainOfResponsibilityRegistrar(container);

			var types = new[]
		                    {
		                        typeof (PositionTitleUserClaimSetsProvider),
                                typeof (StaffCategoryUserClaimSetsProvider)
		                    };

            registrar.RegisterChainOf<IUserClaimSetsProvider<ClaimsSet, EdFiUserSecurityDetails>, NullUserClaimSetsProvider<ClaimsSet, EdFiUserSecurityDetails>>(types);
		}

        protected override void RegisterIUserClaimsProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IUserClaimsProvider>()
                .ImplementedBy<DashboardDatabaseWithStateAgencySupportUserClaimsProvider>());
        }

        protected override void RegisterIClaimsIssuedTracking(IWindsorContainer container)
        {
            container.Register(Component
                .For<IClaimsIssuedTrackingEventHandler>()
                .ImplementedBy<WoopraClaimsIssuedTrackingEventHandler>());
        }
	}
}