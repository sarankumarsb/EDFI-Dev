// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Resources.Models.CustomGrid
{
    [Serializable]
    public class GridDataWithFootnotes : GridTable
    {
        public GridDataWithFootnotes()
        {
            MetricFootnotes = new List<MetricFootnote>();
        }

        public List<MetricFootnote> MetricFootnotes { get; set; }
    }
}
