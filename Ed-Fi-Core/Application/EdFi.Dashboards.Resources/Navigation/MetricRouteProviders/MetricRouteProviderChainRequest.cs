// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Metric.Requests;

namespace EdFi.Dashboards.Resources.Navigation.MetricRouteProviders
{
    public class MetricRouteProviderChainRequest
    {
        public MetricInstanceSetRequestBase MetricInstanceSetRequest { get; set; }
        public MetricMetadataNode MetadataNode { get; set; }

        public static MetricRouteProviderChainRequest Create(MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metadataNode)
        {
            return new MetricRouteProviderChainRequest { MetricInstanceSetRequest = metricInstanceSetRequest, MetadataNode = metadataNode };
        }
    }
}