// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Models.CustomGrid;

namespace EdFi.Dashboards.Resources.Models.School.Detail
{
    [Serializable]
    public class StudentMetricListMetaModel
    {
        public StudentMetricListMetaModel()
        {
            MetricFootnotes = new List<MetricFootnote>();
        }

        public List<MetadataColumnGroup> ListMetadata { get; set; }
        public List<MetricFootnote> MetricFootnotes { get; set; }
    }
}