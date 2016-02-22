// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;

namespace EdFi.Dashboards.Metric.Resources.Models
{
    public enum MetricActionType
    {
        None = 0,
        DynamicContent = 1,//A 
        Link = 2, //A URL to a page in the same window
        AlwaysDisplayedDynamicContent = 3
    }

    [Serializable]
    public class MetricAction
    {
        public int MetricVariantId { get; set; }
        public string Title { get; set; }
        public string Tooltip { get; set; }
        
        public string GetTitleSafeForHtmlId()
        {
            return Title.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("/", "").Replace("\\", "").Replace(",","");
        }

        /// <summary>
        /// A string that represents the URL, javascript or action that is to be executed.
        /// </summary>
        public string Url { get; set; }

        public MetricActionType ActionType { get; set; }

        public string DrilldownHeader { get; set; }

        public string DrilldownFooter { get; set; }

        public string Icon { get; set; }
    }
}
