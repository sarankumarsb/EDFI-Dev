// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;

namespace EdFi.Dashboards.Warehouse.Resource.Models.School.Detail
{
    [Serializable]
    public class StudentPriorYearMetricListModel
    {
        public StudentPriorYearMetricListModel()
        {
            Students = new List<StudentWithMetricsAndPrimaryMetric>();
            MetricFootnotes = new List<MetricFootnote>();
        }

        public List<MetadataColumnGroup> ListMetadata { get; set; }
        public List<StudentWithMetricsAndPrimaryMetric> Students { get; set; }
        public List<MetricFootnote> MetricFootnotes { get; set; }
        public SchoolCategory SchoolCategory { get; set; }
        public string UniqueListId { get; set; }
    }
}
