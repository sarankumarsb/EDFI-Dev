using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Warehouse.Resource.Models.Metric
{
    public class PriorYearMetricData : MetricData
    {
        public override bool CanSupplyMetricData(MetricMetadataNode metricMetadataNode)
        {
            return metricMetadataNode.MetricVariantType == MetricVariantType.PriorYear;
        }
    }
}
