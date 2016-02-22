// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;

namespace EdFi.Dashboards.Metric.Resources.Models
{
    /// <summary>
    /// This is an int value that is passed to be used in the call to the javascript function getMetricIndicator(indicator) in the EdFiGridHTMLTemplateHelper.js file
    /// </summary>
    public enum MetricIndicatorType
    {
        None = 0,
        Accommodation = 1,
        SpanishAccommodation = 2,
        LanguageAccommodation = 3,
        StateAssessmentM = 4,
        StateAssessmentAlt = 5,
        StateAssessmentCommended = 6
    }

    [Serializable]
    public class MetricIndicator
    {
        public int IndicatorTypeId { get; set; }
        public MetricIndicatorType Type { get { return (MetricIndicatorType)IndicatorTypeId; } }
        public string Tooltip { get { return Type.ToString(); } }
    }
}