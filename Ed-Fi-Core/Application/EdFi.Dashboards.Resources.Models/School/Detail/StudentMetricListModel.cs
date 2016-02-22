// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;

namespace EdFi.Dashboards.Resources.Models.School.Detail
{
    [Serializable]
    public class StudentMetricListModel
    {
        public StudentMetricListModel()
        {
            EntityIds = new List<long[]>();
            Students = new List<StudentWithMetricsAndPrimaryMetric>();
            MetricFootnotes = new List<MetricFootnote>();
        }

        public List<MetadataColumnGroup> ListMetadata { get; set; }
        public List<long[]> EntityIds { get; set; }
        public List<StudentWithMetricsAndPrimaryMetric> Students { get; set; }
        public List<MetricFootnote> MetricFootnotes { get; set; }
    }
}
