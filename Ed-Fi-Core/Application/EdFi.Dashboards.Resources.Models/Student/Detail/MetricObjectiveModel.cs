// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Resources.Models.Student.Detail
{
    [Serializable]
    public class MetricObjectiveModel : IStudent
    {
        public MetricObjectiveModel() {}

        public MetricObjectiveModel(long studentUSI)
        {
            StudentUSI = studentUSI;
        }

        public long StudentUSI { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public MetricState State { get; set; }
        public bool IsFlagged { get; set; }
    }
}
