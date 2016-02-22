// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Configuration;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Infrastructure.CastleWindsorInstallers;
using EdFi.Dashboards.Resources.Security.Implementations;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.SecurityTokenService.Web.Providers;

namespace EdFi.Dashboards.SecurityTokenService.Web.Utilities.CastleWindsor.Demo
{
	public class ConfigurationSpecificInstaller : ConfigurationSpecificInstallerBase
	{
		protected override void RegisterIAuthenticationProvider(IWindsorContainer container)
		{
			var config = GetConfigValueProvider(container);

			container.Register(Component
				.For<IAuthenticationProvider>()
				.ImplementedBy<TextFileAuthenticationProvider>()
				.DependsOn(Property.ForKey("credentialsFilePath").Eq(config.GetValue("credentialsFilePath"))));
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

		private static IConfigValueProvider GetConfigValueProvider(IWindsorContainer container)
		{
			IConfigValueProvider config;

			try
			{
				config = container.Resolve<IConfigValueProvider>();
			}
			catch (Exception ex)
			{
				throw new ConfigurationErrorsException("Unable to resolve the IConfigValueProvider while registering Demo user role provider.  The configuration value provider is used to read the 'credentialsFilePath' setting from the web.config appSettings section to supply to the service.  Make sure the IConfigValueProvider is being registered with the container before the IUserRolesProvider.", ex);
			}
			return config;
		}

	}
}