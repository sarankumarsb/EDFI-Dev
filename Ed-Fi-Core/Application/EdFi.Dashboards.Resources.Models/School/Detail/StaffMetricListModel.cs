// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Resources.Models.School.Detail
{
    [Serializable]
    public class StaffMetricListModel
    {
        public StaffMetricListModel()
        {
            StaffMetrics = new List<StaffMetric>();
            MetricFootnotes = new List<MetricFootnote>();
        }

        public int SchoolId { get; set; }
        public string UniqueListId { get; set; }
        public string MetricValueLabel { get; set; }
        public List<StaffMetric> StaffMetrics { get; set; }
        public List<MetricFootnote> MetricFootnotes { get; set; }

        [Serializable]
        public class StaffMetric
        {
            public long StaffUSI { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public dynamic Value { get; set; }
            public string DisplayValue { get; set; }
            public int Experience { get; set; }
            public string Education { get; set; }
            public string Href { get; set; }
        }
    }
}
