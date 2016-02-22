// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Text;
using System.Web.Routing;
using EdFi.Dashboards.Resources.Navigation;

namespace EdFi.Dashboards.Resources.Tests.Navigation.Fakes
{
    public class SiteAreaFakeBase : SiteAreaBase
    {
        protected override string CreateRouteCore(string routeName, System.Web.Routing.RouteValueDictionary routeValueDictionary, object additionalValues)
        {
            var sb = new StringBuilder();

            // Add route name
            sb.Append("/" + routeName);

            // Add route value dictionary values
            foreach (var kvp in routeValueDictionary)
                sb.AppendFormat("/{0}={1}", kvp.Key, kvp.Value);

            // Add additional values
            if (additionalValues != null)
            {
                var rvd = new RouteValueDictionary(additionalValues);

                int i = 0;

                foreach (var kvp in rvd)
                {
                    sb.AppendFormat(i++ == 0 ? "?" : "&");
                    sb.AppendFormat("{0}={1}", kvp.Key, kvp.Value);
                }
            }

            return sb.ToString();
        }
    }
}