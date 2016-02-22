// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resource.Models.Common;

namespace EdFi.Dashboards.Resources.Models.LocalEducationAgency.Detail
{
    [Serializable]
    public class SchoolMetricModel
    {
        public SchoolMetricModel()
        {
            MetricState = new MetricState();
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
        public Link MetricLink { get; set; }
    }
}
