// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;

namespace EdFi.Dashboards.Metric.Resources.Models
{
    [Serializable]
    public class MetricComponent
    {
        public MetricStateType MetricStateType { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string ValueTypeName { get; set; }
        public string Format { get; set; }
        public TrendDirection TrendDirection { get; set; }
    }
}
