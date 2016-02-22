using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using EdFi.Dashboards.Presentation.Architecture.Mvc.AreaRegistration;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.RouteValueProviders;

namespace EdFi.Dashboards.Presentation.Core.Utilities.CastleWindsor
{
    /// <summary>
    /// Installs the standard route value providers implementations (see <see cref="IRouteValueProvider"/>)
    /// except the ones responsible for supplying metric, staff or student names, and installs the
    /// route mapping preparers (see <see cref="IAreaRouteMappingPreparer"/>) for removing the "name"
    /// tokens from the route mapping patterns for metrics, staff and students during MVC area registration
    /// at application startup.
    /// </summary>
    public class RemoveMetricStaffAndStudentNamesRouteMappingComponentsInstaller : IWindsorInstaller
    {
        // Define a list of route provider types to explicitly exclude
        private static readonly List<Type> ExcludedRouteValueProviders = new List<Type>
            {
                typeof(MetricNameRouteValueProvider),
                typeof(StaffLocalEducationAgencyRouteValueProvider),
                typeof(StaffSchoolRouteValueProvider),
                typeof(StudentRouteValueProvider),
            };

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // Register all the standard route value providers, excluding those that provide metric, staff, or student names
            container.Register(
                Classes.FromAssembly(Assembly.GetAssembly(typeof(Marker_EdFi_Dashboards_Resources)))
                        .Where(t => typeof(IRouteValueProvider).IsAssignableFrom(t)
                            && !ExcludedRouteValueProviders.Contains(t))
                        .WithService.FirstInterface());

            // Removes "metric" key from the route pattern
            container.Register(Component
                                   .For<IAreaRouteMappingPreparer>()
                                   .ImplementedBy<MetricNameRemovalRouteValuePreparer>());

            // Removes "staff" key from the route pattern
            container.Register(Component
                                   .For<IAreaRouteMappingPreparer>()
                                   .ImplementedBy<StaffNameRemovalRouteValuePreparer>());

            // Removes "student" key from the route pattern
            container.Register(Component
                                   .For<IAreaRouteMappingPreparer>()
                                   .ImplementedBy<StudentNameRemovalRouteValuePreparer>());
        }
    }
}