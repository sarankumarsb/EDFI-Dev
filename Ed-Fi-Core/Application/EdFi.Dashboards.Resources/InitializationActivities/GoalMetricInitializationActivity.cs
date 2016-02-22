using System.Collections.Generic;
using EdFi.Dashboards.Metric.Resources.InitializationActivity;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Metric.Requests;

namespace EdFi.Dashboards.Resources.InitializationActivities
{
    public class GoalMetricInitializationActivity : MetricInitializationActivityBase<IGranularMetric>
    {
        private readonly IMetricGoalProvider metricGoalProvider;

        public GoalMetricInitializationActivity(IMetricGoalProvider metricGoalProvider)
        {
            this.metricGoalProvider = metricGoalProvider;
        }

        public override void InitializeMetric(IGranularMetric granularMetric, MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metricMetadataNode, 
                                              MetricData metricData, Dictionary<string, object> metricInitializationActivityData)
        {
            granularMetric.Goal = metricGoalProvider.GetMetricGoal(metricMetadataNode, metricData);
        }
    }
}
