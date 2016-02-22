// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Linq;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.AreaRegistration
{
    /// <summary>
    /// Adds convenience methods for working with the typed list of route mappings.
    /// </summary>
    public static class RouteMappingListExtensions
    {
        /// <summary>
        /// Gets the RouteMapping from the list by name.
        /// </summary>
        /// <param name="routeMappings">The list of route mappings to search.</param>
        /// <param name="name">The name of the route mapping to find.</param>
        /// <returns>The matching route, or null if not found.</returns>
        public static RouteMapping GetByName(this List<RouteMapping> routeMappings, string name)
        {
            return routeMappings.SingleOrDefault(x => x.Name == name);
        }
    }
}