// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services;

namespace EdFi.Dashboards.Metric.Resources.InitializationActivity
{
    /// <summary>
    /// Filters general metric initialization activities to those for granular metrics, and provides
    /// an abstract convenience method to be overridden by implementers that supplies the metric instance
    /// as an <see cref="IGranularMetric"/>.
    /// </summary>
    public abstract class MetricInitializationActivityBase<T> : IMetricInitializationActivity
        where T : class
    {
        #region Explicit IMetricInitializationActivity implementation

        /// <summary>
        /// Processes initialization activities for granular metrics, ignoring other metric types.
        /// </summary>
        /// <param name="metric">The metric instance to be initialized.</param>
        /// <param name="metricMetadataNode">The metadata for the metric instance.</param>
        /// <param name="metricData">The data for the metric instance.</param>
        /// <param name="metricInitializationActivityData">A dictionary containing all the metric initialization activity data, accessible by key.</param>
        /// <param name="metricInstanceSetRequest">The request originally passed to the metric service, containing the domain-specific context.</param>
        void IMetricInitializationActivity.InitializeMetric(MetricBase metric, MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metricMetadataNode, 
                                                            MetricData metricData, Dictionary<string, object> metricInitializationActivityData)
        {
            // Invoke initialization activity on metrics of the specified type only
            var castMetric = metric as T;
            if (castMetric != null)
                InitializeMetric(castMetric, metricInstanceSetRequest, metricMetadataNode, metricData, metricInitializationActivityData);
        }

        #endregion

        /// <summary>
        /// Performs initialization activities on a specific metric type, filtering out calls to non-matching metric types.
        /// </summary>
        /// <param name="metric">The metric instance to be initialized.</param>
        /// <param name="metricMetadataNode">The metadata for the metric instance.</param>
        /// <param name="metricData">The data for the metric instance.</param>
        /// <param name="metricInitializationActivityData">A dictionary containing all the metric initialization activity data, accessible by key.</param>
        /// <param name="metricInstanceSetRequest">The request originally passed to the metric service, containing the domain-specific context.</param>
        public abstract void InitializeMetric(T metric, MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metricMetadataNode, 
                                              MetricData metricData, Dictionary<string, object> metricInitializationActivityData);
    }
}