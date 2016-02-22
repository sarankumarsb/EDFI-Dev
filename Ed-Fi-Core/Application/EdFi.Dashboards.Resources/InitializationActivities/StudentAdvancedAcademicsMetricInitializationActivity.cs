// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Metric.Resources.InitializationActivity;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.Requests;

namespace EdFi.Dashboards.Resources.InitializationActivities
{
    public class StudentAdvancedAcademicsMetricInitializationActivity : MetricInitializationActivityBase<IGranularMetric>
    {
        private const string taking = "Taking";
        private const string mastery = "Mastery";

        private readonly IDomainSpecificMetricNodeResolver domainSpecificMetricNodeResolver;

        public StudentAdvancedAcademicsMetricInitializationActivity(IDomainSpecificMetricNodeResolver domainSpecificMetricNodeResolver)
        {
            this.domainSpecificMetricNodeResolver = domainSpecificMetricNodeResolver;
        }

        public override void InitializeMetric(IGranularMetric granularMetric, MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metricMetadataNode, 
                                              MetricData metricData, Dictionary<string, object> metricInitializationActivityData)
        {
            //only used for Student granular metrics
            if (!metricMetadataNode.DomainEntityType.EqualTo(MetricInstanceSetType.StudentSchool))
                return;

            //Quick check before we go to more intensive work.
            if (granularMetric.Values[taking] == null && granularMetric.Values[mastery] == null)
                return;

            //Only where the parent metricIds are equal to:
            var advancedCoursePotentialNode = domainSpecificMetricNodeResolver.GetAdvancedCoursePotentialMetricNode();
            var advancedCourseEnrollmentNode = domainSpecificMetricNodeResolver.GetAdvancedCourseEnrollmentMetricNode();

            if (advancedCoursePotentialNode!=null && granularMetric.Parent.MetricId == advancedCoursePotentialNode.MetricId)
            {
                //set the state display text to the value from the extended properties
                granularMetric.State.DisplayStateText = granularMetric.Values[taking] != null
                                                            ? granularMetric.Values[taking].ToString()
                                                            : string.Empty;
                //change the state so that it will show the text
                if (granularMetric.State.StateType == MetricStateType.None)
                    granularMetric.State.StateType = MetricStateType.Neutral;
                if (granularMetric.State.StateType == MetricStateType.Low)
                    granularMetric.IsFlagged = true;
            }

            if(advancedCourseEnrollmentNode!=null && granularMetric.Parent.MetricId == advancedCourseEnrollmentNode.MetricId)
            {
                //set the state display text to the value from the extended properties
                granularMetric.State.DisplayStateText = granularMetric.Values[mastery] != null
                                                            ? granularMetric.Values[mastery].ToString()
                                                            : string.Empty;
                //set the state display text to the value from the extended properties
                if (granularMetric.State.StateType == MetricStateType.None)
                    granularMetric.State.StateType = MetricStateType.Neutral;
                if (granularMetric.State.StateType == MetricStateType.Low)
                    granularMetric.IsFlagged = true;

            }


        }
    }
}
