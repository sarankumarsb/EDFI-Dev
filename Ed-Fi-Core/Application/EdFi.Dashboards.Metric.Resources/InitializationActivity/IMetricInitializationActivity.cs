// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services;

namespace EdFi.Dashboards.Metric.Resources.InitializationActivity
{
    /// <summary>
    /// Provides an extension point for performing custom initialization on metric instances.
    /// </summary>
    public interface IMetricInitializationActivity
    {
        /// <summary>
        /// Performs custom initialization activity on the metric instance.
        /// </summary>
        /// <param name="metric">The metric instance to be initialized.</param>
        /// <param name="metricMetadataNode">The metadata for the metric instance.</param>
        /// <param name="metricData">The data for the metric instance.</param>
        /// <param name="metricInitializationActivityData"> </param>
        /// <param name="metricInstanceSetRequest">The context of the current metric instance set request.</param>
        void InitializeMetric(MetricBase metric, MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metricMetadataNode, 
                              MetricData metricData, Dictionary<string, object> metricInitializationActivityData);
    }
}
