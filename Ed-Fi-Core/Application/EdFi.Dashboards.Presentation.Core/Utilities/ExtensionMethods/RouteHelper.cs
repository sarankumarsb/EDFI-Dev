// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Web.Routing;
using System.Linq;

namespace EdFi.Dashboards.Presentation.Web.Utilities
{
    public static class RouteHelper
    {
        public static string GetArea(this Route route)
        {
            // Make sure route and DataTokens are actually available
            if (route == null || route.DataTokens == null)
                return null;

            var area = route.DataTokens.SingleOrDefault(x => x.Key == "area");

            if (area.Value != null)
                return area.Value.ToString();

            return null;
        }
    }
}