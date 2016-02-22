// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using EdFi.Dashboards.Resources.Models.Charting;

namespace EdFi.Dashboards.Resources.Models.Student.Detail
{
    [Serializable]
    public class StudentMetricAssessmentHistoricalModel : IStudent
    {
        public StudentMetricAssessmentHistoricalModel()
        {
            ChartData = new ChartData();        
        }

        public long StudentUSI { get; set; }

        // This was put in because it was required for the model of anything using HistoricalChart.cshtml.  I don't think we really need it.
        // This ends up getting serialized into JSON, and the HistoricalChart.cshtml expects this to be here.
        public string DrillDownTitle { get; set; }
        public ChartData ChartData { get; set; }
    }
}
