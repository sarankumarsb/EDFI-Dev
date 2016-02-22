// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Resources.Common
{
    public interface IMetricTreeToIEnumerableOfKeyValuePairProvider
    {
        IEnumerable<KeyValuePair<string, object>> FlattenMetricTree(ContainerMetric metricTreeRootNode);
    }

    public class MetricTreeToIEnumerableOfKeyValuePairProvider : IMetricTreeToIEnumerableOfKeyValuePairProvider
    {
        public IEnumerable<KeyValuePair<string, object>> FlattenMetricTree(ContainerMetric metricTreeRootNode)
        {
            var model = new List<KeyValuePair<string, object>>();

            
            //We only export metrics that can contain data.
            var granularMetrics = metricTreeRootNode.DescendantsOrSelf.Where(x => x.MetricType == MetricType.GranularMetric);
            foreach (dynamic metric in granularMetrics)
            {
                var propertyName = Utilities.Metrics.GetMetricName(metric);
                model.Add(new KeyValuePair<string, object>(propertyName,metric.Value));
            }
 
            return model;
        }
    }
}
