// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.InitializationActivity;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Metric.Requests;

namespace EdFi.Dashboards.Resources.InitializationActivities
{
    [InitializationActivityDependencyAttribute(typeof(GoalMetricInitializationActivity))]
    public class GoalDifferenceMetricInitializationActivity : MetricInitializationActivityBase<IGranularMetric>
    {
        public override void InitializeMetric(IGranularMetric granularMetric, MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metricMetadataNode, 
                                              MetricData metricData, Dictionary<string, object> metricInitializationActivityData)
        {
            //If the value is null then we have to add the keys because templates might expect it.
            if (granularMetric.Value == null)
            {
                //add values to the values(ExtendedProperties) dictionary in the granular metric
                granularMetric.Values.Add("GoalDifference", string.Empty);
                granularMetric.Values.Add("DisplayGoalDifference", string.Empty);
                return;
            }

            //If there is no goal then no difference can be calculated and if the metric is of type string then the goal difference cannot be calculated
            if (granularMetric.Goal == null || !granularMetric.Goal.Value.HasValue || granularMetric.Value.GetType() == Type.GetType("System.String"))
                return;

            //If we got to here but we have no Format on the metric then we need a clear error message.
            if (string.IsNullOrEmpty(metricMetadataNode.Format))
                throw new ArgumentOutOfRangeException("metricMetadataNode", string.Format("The format value for metric Id ({0}) is null.",metricMetadataNode.MetricId));

            if (granularMetric.Goal != null && granularMetric.Goal.Value.HasValue)
            {
                granularMetric.Values.Add("DisplayGoal", string.Format(metricMetadataNode.Format, granularMetric.Goal.Value));
                granularMetric.Values.Add("GoalFormat", metricMetadataNode.Format);
            }

            //convert metric value to decimal to match goal value type
            decimal dataMetricValue = Convert.ToDecimal(granularMetric.Value);

            //initialize goal difference
            decimal goalDifference = 0;

            //Note: Cy look at the original code and look at the history of this file.
            //calculate goal difference based on trend direction
            if (granularMetric.Goal.Interpretation == TrendInterpretation.Standard)
                goalDifference = dataMetricValue - granularMetric.Goal.Value.Value;
            else if (granularMetric.Goal.Interpretation == TrendInterpretation.Inverse)
                goalDifference = granularMetric.Goal.Value.Value - dataMetricValue;

            //add values to the values(ExtendedProperties) dictionary in the granular metric
            granularMetric.Values.Add("GoalDifference", goalDifference);
            granularMetric.Values.Add("DisplayGoalDifference", String.Format(metricMetadataNode.Format, goalDifference));
        }
    }
}
