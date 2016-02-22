using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using EdFi.Dashboards.Presentation.Architecture;
using EdFi.Dashboards.Presentation.Architecture.Mvc.AreaRegistration;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Navigation;

namespace EdFi.Dashboards.Presentation.Core.Utilities.CastleWindsor
{
    /// <summary>
    /// Installs the standard route value providers implementations (see <see cref="IRouteValueProvider"/>),
    /// and installs the <see cref="NullAreaRouteMappingPreparer"/> which leaves the route mappings unmodified
    /// during MVC area registration at application startup.
    /// </summary>
    public class DefaultRouteMappingComponentsInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // Register all the default route value providers (to provide values to all route keys representing names)
            container.Register(
                Classes.FromAssembly(Assembly.GetAssembly(typeof(Marker_EdFi_Dashboards_Resources)))
                        .BasedOn<IRouteValueProvider>()
                        .WithService.FirstInterface());

            // Register the "Null" mapping preparer, which makes no modifications to the route definitions
            container.Register(Component
                .For<IAreaRouteMappingPreparer>()
                .ImplementedBy<NullAreaRouteMappingPreparer>());
        }
    }
}
