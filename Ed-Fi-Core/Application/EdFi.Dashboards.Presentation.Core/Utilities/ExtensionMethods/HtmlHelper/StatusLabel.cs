using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Presentation.Web.Architecture.HtmlHelperExtensions
{
    public static partial class Html
    {
        public static IHtmlString StatusLabel(this HtmlHelper html,
                                              MetricStateType metricState, string stateText)
        {
            var val = new StringBuilder();
            val.Append("<table cellpadding=\"0\" cellspacing=\"0\" style=\"margin:0px auto;");
            val.Append(GetShowState(metricState));
            val.Append("\" class=\"");
            val.Append(GetStateClass(metricState)).Append("\">");
            val.Append("<tr><td>");
            val.Append(stateText);
            val.Append("</td></tr></table>");

            return new MvcHtmlString(val.ToString());
        }

        private static string GetStateClass(MetricStateType metricState)
        {
            switch (metricState)
            {
                case MetricStateType.VeryGood:
                    return "StateGreen";
                case MetricStateType.Good:
                    return "StateGreen";
                case MetricStateType.Acceptable:
                    return "StateYellow";
                case MetricStateType.Low:
                    return "StateRed";
                case MetricStateType.VeryLow:
                    return "StateRed";
                case MetricStateType.Na:
                    return "StateNA";
                case MetricStateType.Neutral:
                    return "StateNeutral";
                case MetricStateType.None:
                    return String.Empty;
            }
            return "StateGray";
        }
        private static string GetShowState(MetricStateType metricState)
        {
            if (MetricStateType.None == metricState)
                return "display:none;";
            return String.Empty;
        }
    }
}