// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using EdFi.Dashboards.Resources.Models.Charting;

namespace EdFi.Dashboards.Resources.Models.Student.Detail
{
    [Serializable]
    public class BenchmarkModel : IStudent
    {
        public BenchmarkModel()
        {
            ChartData = new ChartData();        
        }

        public BenchmarkModel(long studentUSI) : this()
        {
            StudentUSI = studentUSI;
        }

        public long StudentUSI { get; set; }
        public string DrillDownTitle { get; set; }
        public ChartData ChartData { get; set; }
    }
}
