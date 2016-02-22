// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.AreaRegistration
{
    /// <summary>
    /// Provides an implementation of an <see cref="IAreaRouteMappingPreparer"/> that does nothing.
    /// </summary>
    public class NullAreaRouteMappingPreparer : IAreaRouteMappingPreparer
    {
        /// <summary>
        /// Provides an implementation of the method, but makes no modifications.
        /// </summary>
        /// <param name="areaName">The name of the area for which routes are about to be registerd.</param>
        /// <param name="routeMappings">The default route mappings for the area.</param>
        public void CustomizeRouteMappings(string areaName, List<RouteMapping> routeMappings)
        {
            // By default, no customizations are performed
        }
    }
}