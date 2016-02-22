// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.AreaRegistration
{
    /// <summary>
    /// Defines a method for enabling the customization of area route mappings.
    /// </summary>
    public interface IAreaRouteMappingPreparer
    {
        /// <summary>
        /// Customizes provided route mappings just prior to being registered for the specified area.
        /// </summary>
        /// <param name="areaName">The name of the area for which routes are about to be registered.</param>
        /// <param name="routeMappings">The route mappings to be subsequently registered.</param>
        void CustomizeRouteMappings(string areaName, List<RouteMapping> routeMappings);
    }
}