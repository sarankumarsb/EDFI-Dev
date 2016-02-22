using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.AreaRegistration
{
    public class StudentNameRemovalRouteValuePreparer : IAreaRouteMappingPreparer
    {
        public void CustomizeRouteMappings(string areaName, List<RouteMapping> routeMappings)
        {
            foreach (var mapping in routeMappings)
            {
                mapping.Url = Regex.Replace(mapping.Url, @"\{student\}\-?", "");
            }   
        }
    }
}