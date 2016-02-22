// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resource.Models.Common;

namespace EdFi.Dashboards.Resources.Models.LocalEducationAgency.Detail
{
    [Serializable]
    public class GoalPlanningSchoolMetricModel : ResourceModelBase
    {
        public IEnumerable<GoalPlanningSchoolMetric> GoalPlanningSchoolMetrics { get; set; }
        public IEnumerable<SchoolMetric> SchoolMetrics { get; set; }
    }

    [Serializable]
    public class SchoolMetric
    {
        public int SchoolId { get; set; }
        public int MetricId { get; set; }
    }

    [Serializable]
    public class GoalPlanningSchoolMetric
    {
        public GoalPlanningSchoolMetric()
        {
            MetricState = new MetricState();
            GoalMetricIds = new List<int>();
        }
        public int SchoolId { get; set; }
        public string Name { get; set; }
        public string Principal { get; set; }
        public string SchoolCategory { get; set; }
        public string ValueType { get; set; }
        public string DisplayValue { get; set; }
        public double? Value { get; set; }
        public double Goal { get; set; }
        public double? GoalDifference { get; set; }
        public MetricState MetricState { get; set; }
        public Link Href { get; set; }
        public Link MetricContextLink { get; set; }
        public IList<int> GoalMetricIds { get; private set; }
        public bool StandardGoalInterpretation { get; set; }
    }
}
