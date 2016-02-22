using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Metric.Requests;

namespace EdFi.Dashboards.Resources.Navigation.MetricRouteProviders
{
    public class MetricActionRouteProviderChainRequest
    {
        public MetricInstanceSetRequestBase MetricInstanceSetRequest { get; set; }
        public MetricAction Action { get; set; }

        public static MetricActionRouteProviderChainRequest Create(MetricInstanceSetRequestBase metricInstanceSetRequest, MetricAction action)
        {
            return new MetricActionRouteProviderChainRequest { MetricInstanceSetRequest = metricInstanceSetRequest, Action = action };
        }
    }
}