// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;

namespace EdFi.Dashboards.Metric.Resources.Models
{
    public enum MetricFootnoteType
    {
        None = 0,
        MetricFootnote = 1,
        DrillDownFootnote = 2, 
    }

    [Serializable]
    public class MetricFootnote
    {
        public int FootnoteNumber { get; set; }
        public MetricFootnoteType FootnoteTypeId { get; set; }
        public string FootnoteText { get; set; }
    }
}
