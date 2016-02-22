using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Resources.Navigation.MetricRouteProviders
{
    public static class LegacyRouteUtility
    {
        //TODO:Remove this when we have the resource name in the Spiner metadata for the metric ACTIONS/Drilldowns.
        public static string GetRouteResourceName(string legacyUrl)
        {
            try
            {
                return legacyUrl.Substring(legacyUrl.LastIndexOf("/") + 1,
                                    legacyUrl.IndexOf(".aspx") - (legacyUrl.LastIndexOf("/") + 1));
            }
            catch (Exception)
            {
                return legacyUrl;
            }

        }
    }
}
