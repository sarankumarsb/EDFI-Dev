// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services;

namespace EdFi.Dashboards.Resources.Navigation.RouteValueProviders
{
    public class MetricNameRouteValueProvider : IRouteValueProvider
    {
        private readonly IMetricMetadataTreeService metricMetadataTreeService;

        public MetricNameRouteValueProvider(IMetricMetadataTreeService metricMetadataTreeService)
        {
            this.metricMetadataTreeService = metricMetadataTreeService;
        }

        public bool CanProvideRouteValue(string key, Func<string, object> getValue)
        {
            if (key.Equals("metricName", StringComparison.OrdinalIgnoreCase) && GetMetricVariantId(getValue) != 0)
            {
                return true;
            }

            return false;
        }

        public void ProvideRouteValue(string key, Func<string, object> getValue, Action<string, object> setValue)
        {
            int metricVariantId = GetMetricVariantId(getValue);
            var metricName = metricNamesByMetricVariantId.GetOrAdd(metricVariantId, GetMetricName);
            setValue("metricName", metricName);
        }

        private readonly ConcurrentDictionary<int, string> metricNamesByMetricVariantId = new ConcurrentDictionary<int, string>();

        private readonly Regex routeValueTextRegex = new Regex(@"[^\w]", RegexOptions.Compiled);

        private string GetMetricName(int metricVariantId)
        {
            var metricNode = MetricMetadataTree.AllNodesByMetricVariantId.ValuesByKey(metricVariantId).FirstOrDefault();

            if (metricNode == null)
				throw new InvalidOperationException(
                    string.Format("Metric variant Id '{0}' could not be found.", metricVariantId));

            var metricName = routeValueTextRegex.Replace(metricNode.Name, "-");

            return metricName;
        }

        private MetricMetadataTree metadataTree;

        private MetricMetadataTree MetricMetadataTree
        {
            get { return metadataTree ?? (metadataTree = metricMetadataTreeService.Get(MetricMetadataTreeRequest.Create())); }
        }

        private static int GetMetricVariantId(Func<string, object> getValue)
        {
            return Convert.ToInt32(getValue("metricVariantId"));
        }
    }
}
