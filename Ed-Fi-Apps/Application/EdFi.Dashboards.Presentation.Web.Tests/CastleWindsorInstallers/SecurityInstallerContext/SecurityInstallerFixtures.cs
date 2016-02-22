// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Diagnostics;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using NUnit.Framework;
using EdFi.Dashboards.Data.CastleWindsorInstallers;
using EdFi.Dashboards.Resources.CastleWindsorInstallers;
using EdFi.Dashboards.Resources.Security;
using EdFi.Dashboards.Testing;

namespace EdFi.Dashboards.Presentation.Web.Tests.CastleWindsorInstallers.ServiceInstallerContext
{
	// NOTE: This class is marked to NOT be compiled because it is intended for interactive
	// debugging of IoC configuration code, as necessary.
	public class ServiceInstallerFixtures : TestFixtureBase
	{
		protected override void ExecuteTest()
		{
		    var securityInstaller = new SecurityInstaller();
			
			var container = new WindsorContainer();

			// Add the array resolver for resolving arrays of services automatically
			container.Kernel.Resolver.AddSubResolver(
				new ArrayResolver(container.Kernel));

			container.Install(
                securityInstaller,
				new PresenterInstaller(),
				new InfrastructureServiceInstaller(),
                new ServiceInstaller(),
				new RepositoryInstaller(),
				new UIAspNetInstaller());

			try
			{
				var result1 = container.Resolve<IRoleAuthorization>("IRoleAuthorization.PrincipalRoleAuthorization");
				//var result2 = container.Resolve<IRoleAuthorization>("IRoleAuthorization.SuperintendentRoleAuthorization");
				//var result = container.Kernel.ResolveAll(typeof(IRoleAuthorization), (IDictionary)null);

			}
			catch (Exception ex)
			{
				throw;
			}

			Debug.WriteLine("Hello.");
			Debug.WriteLine("Hello.");
		}

		[Test]
		public void RunIt()
		{
			
		}
	}
}
