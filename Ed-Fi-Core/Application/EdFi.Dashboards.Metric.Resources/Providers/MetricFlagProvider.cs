// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using EdFi.Dashboards.Metric.Resources.Models;
using System;
using System.Linq;
using EdFi.Dashboards.Common;

namespace EdFi.Dashboards.Metric.Resources.Providers
{
    public class MetricFlagProvider : IMetricFlagProvider
    {

        public bool GetMetricFlag(MetricBase metricBase, MetricMetadataNode metadataNode, MetricData metricData)
        {
            var metricInstance = metricData.MetricInstancesByMetricId.GetValueOrDefault(metricBase.MetricId);

            if (metricInstance == null)
                return false;

            return metricInstance.Flag.GetValueOrDefault();
        }
    }
}
