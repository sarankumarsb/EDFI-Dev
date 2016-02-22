// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;

namespace EdFi.Dashboards.Metric.Resources.Models
{
    [Serializable]
    public class Trend
    {
        public TrendInterpretation Interpretation { get; set; }
        public TrendDirection Direction { get; set; }
        public TrendEvaluation Evaluation { get; set; }
        public TrendEvaluation RenderingDisposition { get; set; }
        public string Tooltip { get; set; }
    }
}
