// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace EdFi.Dashboards.Common.Utilities.CastleWindsorInstallers
{
	/// <summary>
	/// Registers the Presenter classes (for the Model-View-Presenter implementation) with the Castle Windsor Inversion of Control container.
	/// </summary>
	public class PresenterInstaller<TMarker> : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(
				Classes
                    .FromAssemblyContaining<TMarker>()
					.Where(t => t.Name.EndsWith("Presenter"))
                    .LifestyleTransient()
				);
		}
	}
}
