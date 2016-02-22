using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace EdFi.Dashboards.Presentation.Core.Plugins.Utilities.CastleWindsor
{
    public abstract class ResourceDefaultConventionInstaller<TMarker> : IWindsorInstaller
    {        
        public virtual void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var serviceInstaller = new EdFi.Dashboards.Resources.CastleWindsorInstallers.GenericServiceInstaller<TMarker>();

            container.Install(new IWindsorInstaller[] { serviceInstaller });
        }
    }
}
