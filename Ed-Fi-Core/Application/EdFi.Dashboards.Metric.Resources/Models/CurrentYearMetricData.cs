using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Metric.Resources.Models
{
    public class CurrentYearMetricData : MetricData
    {
        public override bool CanSupplyMetricData(MetricMetadataNode metricMetadataNode)
        {
            return metricMetadataNode.MetricVariantType == MetricVariantType.CurrentYear;
        }
    }
}
