// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EdFi.Dashboards.Common.Utilities;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.AreaRegistration
{
    /// <summary>
    /// Provides an abstract base class for Ed-Fi area registrations, adding a call to the 
    /// registered <see cref="IAreaRouteMappingPreparer"/> implementation allowing for customization
    /// of the core routes.
    /// </summary>
    public abstract class EdFiAreaRegistrationBase : System.Web.Mvc.AreaRegistration
    {
        /// <summary>
        /// Registers all routes for the area.
        /// </summary>
        /// <param name="context">The context to be used to register the routes.</param>
        public override void RegisterArea(AreaRegistrationContext context)
        {
            // Get the default Ed-Fi route mappings for the area
            var routeMappings = GetDefaultRouteMappings();

            if (routeMappings == null)
                throw new Exception(string.Format("Route mappings for area '{0}' was null.", this.AreaName));

            // Give route registration extensibility implementation a chance to customize the routes being registered
            var areaRouteMappingPreparer = IoC.ResolveAll<IAreaRouteMappingPreparer>();

            foreach (var routeMappingPreparer in areaRouteMappingPreparer)
            {
                routeMappingPreparer.CustomizeRouteMappings(this.AreaName, routeMappings);    
            }
            

            // Register each route mapping
            foreach (var routeMapping in routeMappings)
            {
                context.MapRoute(
                    routeMapping.Name, routeMapping.Url, routeMapping.Defaults,
                    routeMapping.Constraints, routeMapping.Namespaces);
            }
        }

        /// <summary>
        /// Gets a list of the default route registrations for the area.
        /// </summary>
        /// <returns>A list containing the default routes to be registered.</returns>
        protected abstract List<RouteMapping> GetDefaultRouteMappings();
    }
}
