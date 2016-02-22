// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Metric.Resources.Providers
{
    public interface IMetricFlagProvider
    {
        bool GetMetricFlag(MetricBase metricBase, MetricMetadataNode metadataNode, MetricData metricData);
    }
}
