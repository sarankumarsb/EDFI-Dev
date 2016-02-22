using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Metric.Resources.InitializationActivity;
using EdFi.Dashboards.Metric.Resources.Services;

namespace EdFi.Dashboards.Resources.InitializationActivities
{
    public class DoNothingMetricInitializationActivityDataProvider : IMetricInitializationActivityDataProvider
    {
        public MetricInitializationActivityData GetInitializationActivityData(MetricInstanceSetRequestBase metricInstanceSetRequest)
        {
            return null;
        }
    }
}
