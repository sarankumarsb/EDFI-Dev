// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Support;

namespace EdFi.Dashboards.Presentation.Web.Utilities
{
    public static class Legacy
    {
        public static string ShowState(MetricStateType stateType)
        {
            if (stateType == MetricStateType.None)
                return "display:none;";
            return String.Empty;
        }

        public static string EvaluateState(MetricStateType stateType)
        {
            switch (stateType)
            {
                case MetricStateType.NoData:
                    return "No data.";
                case MetricStateType.Neutral:
                    return string.Empty;
                case MetricStateType.Good:
                    return "Goal has been met.";
                case MetricStateType.Acceptable:
                    return "Goal has not been met but it is acceptable.";
                case MetricStateType.Low:
                    return "Goal has not been met.";
                case MetricStateType.Na:
                    break;
                case MetricStateType.None:
                    break;
                case MetricStateType.VeryGood:
                    return "Goal has been exceeded.";
                case MetricStateType.VeryLow:
                    return "Goal has not been met and is way below.";
                default:
                    throw new ArgumentOutOfRangeException("stateType in the LegacyControlFunctions for the Status Label partial in metrics.");
            }

            //We should never get here.
            return string.Empty;
        }

        /// <summary>
        /// The value to display in the image color.
        /// </summary>
        public static string StateClass(MetricStateType stateType)
        {
            switch (stateType)
            {
                case MetricStateType.VeryGood:
                    return "status-goal-met-exceeded";
                case MetricStateType.Good:
                    return "status-goal-met";
                case MetricStateType.Acceptable:
                    return "status-goal-not-met-acceptable";
                case MetricStateType.Low:
                    return "status-goal-not-met";
                case MetricStateType.VeryLow:
                    return "status-goal-not-met-way-below";
                case MetricStateType.Na:
                    return "status-goal-na";
                case MetricStateType.Neutral:
                case MetricStateType.None:
                    return "status-goal-none";
            }

            return "status-goal-none";
        }

        public static string StatusClass(MetricStateType stateType)
        {
            switch (stateType)
            {
                case MetricStateType.VeryGood:
                    return "aggregate-status-very-good";
                case MetricStateType.Good:
                    return "aggregate-status-good";
                case MetricStateType.Acceptable:
                    return "aggregate-status-acceptable";
                case MetricStateType.Low:
                    return "aggregate-status-low";
                case MetricStateType.VeryLow:
                    return "aggregate-status-very-low";
                case MetricStateType.Na:
                    return "aggregate-status-na";
                case MetricStateType.Neutral:
                case MetricStateType.None:
                    return "aggregate-status-none";
            }

            return "aggregate-status-none";
        }

        public static string DifferenceFromGoalClass(object goalDifference)
        {
            if (goalDifference == null)
                return String.Empty;
            var str = goalDifference as string;
            if (str != null && String.IsNullOrEmpty(str))
                return String.Empty;
            var d = Convert.ToDecimal(goalDifference);
            if (d >= 0)
                return String.Empty;
            return "goal-not-met";
        }

        private const string upArrowIcon = "icon-up-dir";
        private const string downArrowIcon = "icon-down-dir";
        private const string noChangeIcon = "icon-no-change";

        public static string GetArrowDirection(Trend trend)
        {
            switch (trend.RenderingDisposition)
            {
                case TrendEvaluation.UpGood:
                case TrendEvaluation.UpBad:
                case TrendEvaluation.UpNoOpinion:
                    return upArrowIcon;
                case TrendEvaluation.DownBad:
                case TrendEvaluation.DownGood:
                case TrendEvaluation.DownNoOpinion:
                    return downArrowIcon;
                case TrendEvaluation.NoChangeNoOpinion:
                    return noChangeIcon;
            }

            return string.Empty;
        }

        private const string redArrowColor = "arrow-red";
        private const string greenArrowColor = "arrow-green";
        private const string greyArrowColor = "arrow-grey";

        public static string GetArrowColor(Trend trend)
        {
            switch (trend.RenderingDisposition)
            {
                case TrendEvaluation.UpGood:
                case TrendEvaluation.DownGood:
                    return greenArrowColor; // good is green
                case TrendEvaluation.UpNoOpinion:
                case TrendEvaluation.DownNoOpinion:
                case TrendEvaluation.NoChangeNoOpinion:
                    return greyArrowColor; // no opinion is gray
                case TrendEvaluation.DownBad:
                case TrendEvaluation.UpBad:
                    return redArrowColor; // bad is red
            }

            return string.Empty;
        }

        private const string upTooltip = "Getting better from prior period.";
        private const string noChangeTooltip = "No change from the prior period.";
        private const string downTooltip = "Getting worse from prior period.";

        public static string Tooltip(Trend trend)
        {
            switch (trend.Evaluation)
            {
                case TrendEvaluation.UpGood:
                case TrendEvaluation.DownGood:
                case TrendEvaluation.UpNoOpinion:
                    return upTooltip;
                case TrendEvaluation.NoChangeNoOpinion:
                    return noChangeTooltip;
                case TrendEvaluation.DownBad:
                case TrendEvaluation.UpBad:
                case TrendEvaluation.DownNoOpinion:
                    return downTooltip;
            }

            return string.Empty;
        }

        public static bool IsVisible(Trend trend)
        {
            return trend.Evaluation != TrendEvaluation.None;
        }

        public static IEnumerable<MetricAction> GetModelActions(MetricBase metric)
        {
            var granular = metric as IGranularMetric;
            if (granular == null || granular.Value != null)
                return metric.Actions;

            return metric.Actions.Where(x => x.ActionType == MetricActionType.AlwaysDisplayedDynamicContent);
        }

        /// <summary>
        /// Returns a string that represents the MetricAction suitable for use in a HTML id.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static string GetDynamicContentNameFromAction(MetricAction action)
        {
            return action.MetricVariantId + action.GetTitleSafeForHtmlId();
        }

        public static string GetMenuActionScript(MetricBase model, MetricAction action, string moreActionDivId,
                                                 string contextValues)
        {
            const string dynamicFormat = "closeMoreMenu(); showDynamicContent('#{0}','{1}{2}','#{3}', '{4}', '{5}');";
            const string linkFormat = "closeMoreMenu(); NavigateToPage('{1}');";

            if (action == null)
                throw new ArgumentNullException("action", "Action cannot be null.");

            if (action.Url == null)
                throw new ApplicationException("Action does not have it's Url property set.  Metric Variant Id: "  + action.MetricVariantId + "  Title: " + action.Title);

            if (action == null)
                throw new ArgumentNullException("action", "Action cannot be null.");

            if (action.Url == null)
                throw new ApplicationException("Action does not have it's Url property set.  Metric Variant Id: "  + action.MetricVariantId + "  Title: " + action.Title);

            if (action == null)
                throw new ArgumentNullException("action", "Action cannot be null.");

            if (action.Url == null)
                throw new ApplicationException("Action does not have it's Url property set.  Metric Variant Id: "  + action.MetricVariantId + "  Title: " + action.Title);

            string parameterValues;
            if (!action.Url.Contains(".aspx"))//converted resources
            {
                parameterValues = action.Url.Resolve() + "?" + DateTime.Now.Ticks;
                string subjectArea = GetSubjectArea(model.MetricVariantId);
                if (!string.IsNullOrEmpty(subjectArea))
                {
                    parameterValues += ("&subjectArea=" + subjectArea.Replace(" ", "%20"));
                }

                if (action.ActionType == MetricActionType.DynamicContent ||
                    action.ActionType == MetricActionType.AlwaysDisplayedDynamicContent)
                {
                    string divId = "DynamicContentDiv" + GetDynamicContentNameFromAction(action);
                    return String.Format(dynamicFormat,
                                         divId,
                                         parameterValues + "&amp;Title=" + action.GetTitleSafeForHtmlId(), "",
                                         moreActionDivId,
                                         action.GetTitleSafeForHtmlId() + action.MetricVariantId,
                                         model.Parent == null ? model.MetricVariantId : model.Parent.MetricVariantId);
                }
                else
                {
                    return String.Format(linkFormat, moreActionDivId, parameterValues);
                }
            }
            else
            {
                parameterValues = ReplaceParametersWithValues(action.Url, contextValues);
                if (action.Url.Contains("{schoolId}") && !action.Url.Contains("{localEducationAgencyId}"))
                    parameterValues =
                        parameterValues.AppendParameters("localEducationAgencyId=" +
                                                         EdFiDashboardContext.Current.LocalEducationAgencyId);

                if (action.ActionType == MetricActionType.DynamicContent ||
                    action.ActionType == MetricActionType.AlwaysDisplayedDynamicContent)
                {
                    string divId = "DynamicContentDiv" + action.MetricVariantId + action.GetTitleSafeForHtmlId();
                    string additionalQstrings = "&" + DateTime.Now.Ticks + "&divId=" + divId + "&moreImageId=" + moreActionDivId +
                                                "&metricActionTitle=" + action.Title.Replace(" ", "%20") +
                                                "&blueHeaderSpan=blueHeaderSpan" + model.MetricVariantId +
                                                action.GetTitleSafeForHtmlId();
                    return String.Format(dynamicFormat, divId, parameterValues, additionalQstrings, moreActionDivId,
                                              action.GetTitleSafeForHtmlId() + action.MetricVariantId,
                                              model.Parent == null ? model.MetricVariantId : model.Parent.MetricVariantId);
                }
                else
                {
                    return String.Format(linkFormat, moreActionDivId, parameterValues);
                }
            }
        }

        private static string ReplaceParametersWithValues(string url, string contextValues)
        {
            MatchCollection matches = Regex.Matches(url, @"\{(?<Name>[\w]+)\}");
            foreach (Match match in matches)
                url = url.Replace("{" + match.Groups["Name"].Value + "}",
                                  GetParameter(match.Groups["Name"].Value, contextValues).Replace(" ", "%20"));

            return url;
        }

        private static string GetParameter(string paramName, string contextValues)
        {
            string[] temp = contextValues.Replace("{", "").Replace("}", "").Split(',');

            foreach (var s in temp)
                if (s.Contains(paramName))
                    return s.Split(':')[1];

            return null;
        }

        // this should be done some other way
        public static string GetSubjectArea(int metricVariantId)
        {
            switch (metricVariantId)
            {
                case (int)StudentMetricEnum.AdvancedCourseEnrollmentELA:
                    return "ELA";
                case (int)StudentMetricEnum.AdvancedCourseEnrollmentMath:
                    return "Mathematics";
                case (int)StudentMetricEnum.AdvancedCourseEnrollmentScience:
                    return "Science";
                case (int)StudentMetricEnum.AdvancedCourseEnrollmentSocialStudies:
                    return "Social Studies";
                case (int)StudentMetricEnum.CourseGradeAlgebraI:
                    return "Mathematics";
            }
            return String.Empty;
        }
    }
}
