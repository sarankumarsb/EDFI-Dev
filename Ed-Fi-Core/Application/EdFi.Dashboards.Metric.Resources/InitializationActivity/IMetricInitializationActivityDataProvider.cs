// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using EdFi.Dashboards.Metric.Resources.Services;

namespace EdFi.Dashboards.Metric.Resources.InitializationActivity
{
    /// <summary>
    /// Provides a key/value pair for maintaining the metric initialization data.
    /// </summary>
    public class MetricInitializationActivityData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetricInitializationActivityData"/> class.
        /// </summary>
        /// <param name="key">The key used to uniquely identify the data for the targeted metric initialization activity.</param>
        /// <param name="data">The data for the metric initialization activity.</param>
        public MetricInitializationActivityData(string key, object data)
        {
            Key = key;
            Data = data;
        }

        /// <summary>
        /// The key used to uniquely identify the data for the targeted metric initialization activity.
        /// </summary>
        public string Key { get; private set; }
        
        /// <summary>
        /// The data for the metric initialization activity.
        /// </summary>
        public object Data { get; private set; }
    }

    /// <summary>
    /// Provides a method for obtaining additional data needed for a custom metric initialization activity.
    /// </summary>
    public interface IMetricInitializationActivityDataProvider
    {
        /// <summary>
        /// Gets additional data needed for a custom metric initialization activity.
        /// </summary>
        /// <param name="metricInstanceSetRequest">The domain-specific metric instance set request.</param>
        /// <returns>A key-value pair containing the named data needed by the corresponding metric initialization activity; or null if no data is available.</returns>
        /// <remarks>If the request does not apply to the activity, th</remarks>
        MetricInitializationActivityData GetInitializationActivityData(MetricInstanceSetRequestBase metricInstanceSetRequest);
    }
}
