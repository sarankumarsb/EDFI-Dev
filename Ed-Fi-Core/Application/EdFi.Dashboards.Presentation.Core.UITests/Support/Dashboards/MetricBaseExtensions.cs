using System.Linq;
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Presentation.Core.UITests.Support.Dashboards
{
    public static class MetricBaseExtensions
    {
        /// <summary>
        /// Searches the descendants of the provided metric for a matching metric inside a matching container metric.
        /// </summary>
        /// <param name="metricBase">The metric whose descendants are to be searched.</param>
        /// <param name="containerName">The name of container metric to find.</param>
        /// <param name="metricName">The name of the metric within the container metric to find.</param>
        /// <returns></returns>
        public static MetricBase FindMetricByContainerAndName(this ContainerMetric metricBase, string containerName,
                                                              string metricName)
        {
            MetricBase targetMetric = null;

            // Find container metric by name
            var containerMetric = metricBase.Descendants
                                    .OfType<ContainerMetric>()
                                    .FirstOrDefault(m => m.Name.EqualsIgnoreCase(containerName));

            if (containerMetric != null)
            {
                // Find target metric by name, within the container
                targetMetric = containerMetric.Descendants
                                    .FirstOrDefault(m => m.Name.EqualsIgnoreCase(metricName));
            }

            return targetMetric;
        }
    }
}
