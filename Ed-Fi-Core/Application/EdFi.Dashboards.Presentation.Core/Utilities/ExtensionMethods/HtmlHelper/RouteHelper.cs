namespace EdFi.Dashboards.Presentation.Core.Utilities.ExtensionMethods.HtmlHelper
{
    public static class RouteHelper
    {
        private const string MetricVariantId = "metricVariantId";

        public static string GetMetricIdOrDefault(this System.Web.Mvc.HtmlHelper html)
        {
            var data = html.RouteCollection.GetRouteData(html.ViewContext.HttpContext);
            if (data != null && data.Values.ContainsKey(MetricVariantId))
            {
                return data.Values[MetricVariantId].ToString();
            }
            return "";
        }
    }
}
