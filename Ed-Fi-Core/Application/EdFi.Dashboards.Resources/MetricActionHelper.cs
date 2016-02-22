using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Resources
{
    public static class MetricActionHelper
    {
        /// <summary>
        /// Method that gets the Title to be used on the Chart.
        /// </summary>
        /// <param name="actions">The actions associated with the current metric.</param>
        /// <param name="title">The title that is part of the key. (usually we have already removed spaces)</param>
        /// <returns></returns>
        public static string GetChartTitle(this IEnumerable<MetricAction> actions, string title)
        {
            if (string.IsNullOrEmpty(title))
                return string.Empty;

            var singleOrDefault = actions.SingleOrDefault(x => x.GetTitleSafeForHtmlId() == title);
            return singleOrDefault != null ? singleOrDefault.DrilldownHeader : string.Empty;
        }
    }
}
