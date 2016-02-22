// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Resources.Models.Charting
{
    [Serializable]
    public class AssessmentRateChartModel
    {
        public AssessmentRateChartModel()
        {
            ChartData = new ChartData();
            MetricFootnotes = new List<MetricFootnote>();
        }

        public ChartData ChartData { get; set; }

        public List<MetricFootnote> MetricFootnotes { get; set; }
    }
}
