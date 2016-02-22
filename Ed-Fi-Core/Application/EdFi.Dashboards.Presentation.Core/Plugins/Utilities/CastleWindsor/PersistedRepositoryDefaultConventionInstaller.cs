using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace EdFi.Dashboards.Presentation.Core.Plugins.Utilities.CastleWindsor
{
    public abstract class PersistedRepositoryDefaultConventionInstaller<TMarker> : IWindsorInstaller
    {        
        public virtual void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var controllerInstaller = new EdFi.Dashboards.Resources.CastleWindsorInstallers.PersistedRepositoryInstaller<TMarker>();

            container.Install(new IWindsorInstaller[] { controllerInstaller });
        }
    }
}
