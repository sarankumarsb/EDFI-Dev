// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Metric.Resources.InitializationActivity;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Metric.Requests;
using MetricStateType = EdFi.Dashboards.Metric.Resources.Models.MetricStateType;

namespace EdFi.Dashboards.Resources.InitializationActivities
{
    [InitializationActivityDependency(typeof(GoalMetricInitializationActivity))]
    public class DisplayStateTextMetricInitializationActivity : MetricInitializationActivityBase<IGranularMetric>
    {
        public override void InitializeMetric(IGranularMetric granularMetric, MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metricMetadataNode, 
                                              MetricData metricData, Dictionary<string, object> metricInitializationActivityData)
        {
            //only used for School or LocalEducationAgency granular metrics
            if (!(metricMetadataNode.DomainEntityType.EqualTo(MetricInstanceSetType.School) || metricMetadataNode.DomainEntityType.EqualTo(MetricInstanceSetType.LocalEducationAgency)))
                return;

            //The Value is placed instead of the State text.
            granularMetric.State.DisplayStateText = granularMetric.DisplayValue;

            //School's and LEA's have a State override based on the Goal.
            if (granularMetric.Goal.Value == null)
                return;

            if(granularMetric.Value==null)
                return;
            
            //If we cant cast the value to double then do nothing.
            decimal metricValue;
            if (!Decimal.TryParse(granularMetric.Value.ToString(), out metricValue))
                return;

            if (granularMetric.Goal.Interpretation == TrendInterpretation.Standard)
            {
                granularMetric.State.StateType = (metricValue >= granularMetric.Goal.Value) ? MetricStateType.Good : MetricStateType.Low;
                return;
            }

            if (granularMetric.Goal.Interpretation == TrendInterpretation.Inverse)
                granularMetric.State.StateType = (metricValue <= granularMetric.Goal.Value) ? MetricStateType.Good : MetricStateType.Low;
        }
    }
}
