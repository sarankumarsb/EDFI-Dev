using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Models.School.Detail;

namespace EdFi.Dashboards.Warehouse.Resource.Models.School.Detail
{
    [Serializable]
    public class StaffPriorYearMetricListModel
    {
        public StaffPriorYearMetricListModel()
        {
            StaffMetrics = new List<StaffMetricListModel.StaffMetric>();
            MetricFootnotes = new List<MetricFootnote>();
        }

        public List<StaffMetricListModel.StaffMetric> StaffMetrics { get; set; }
        public List<MetricFootnote> MetricFootnotes { get; set; }
        public int SchoolId { get; set; }
        public string UniqueListId { get; set; }
        public string MetricValueLabel { get; set; }
    }
}
