using System;
using System.Linq;
using System.Threading;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers;
using EdFi.Dashboards.Presentation.Core.UITests.Attributes;
using EdFi.Dashboards.Presentation.Core.UITests.Support;
using EdFi.Dashboards.Presentation.Core.UITests.Support.Coypu;
using EdFi.Dashboards.Presentation.Core.UITests.Support.Dashboards;

namespace EdFi.Dashboards.Presentation.Core.UITests.Pages
{
    [AssociatedController(typeof(DomainMetricController), typeof(Areas.School.Controllers.DomainMetricController), typeof(Areas.StudentSchool.Controllers.DomainMetricController))]
    public class MetricsPage : PageBase<ContainerMetric>
    {
        /// <summary>
        /// Gets the CSS selector the context menus.
        /// </summary>
        protected virtual string ContextMenuButtonCssSelector
        {
            get { return ".contextMenu-Button"; }
        }

        /// <summary>
        /// Gets the format specifier for "More" menu actions, containing a parameter marker for the metric Id.
        /// </summary>
        protected virtual string MoreMenuIdFormat
        {
            get { return "mainmoreActions-{0}"; }
        }

        /// <summary>
        /// Gets the format specifier for "More" menu actions, with parameter markers for the metric Id and the action title.
        /// </summary>
        protected virtual string MoreMenuActionIdFormat
        {
            get { return "moreActions-{0}{1}"; }
        }

        public override void Visit(bool forceNavigation = false)
        {
            throw new NotSupportedException(
                "MetricsPage provides generic metric-related behavior, but cannot be visited directly.");
        }

        public virtual bool AnyMoreMenusVisible
        {
            get { return Browser.FindCss(ContextMenuButtonCssSelector).Exists(Make_It.Wait_1_Second); }
        }

        public virtual void ShowAllDrilldowns()
        {
            var allDrilldowns =
                from m in Model.Descendants.OfType<IGranularMetric>()
                from a in m.Actions
                select new
                    {
                        MetricId = m.MetricId, 
                        ActionTitle = a.Title,
                    };

            foreach (var drilldown in allDrilldowns)
            {
                ClickDrilldown(drilldown.MetricId, drilldown.ActionTitle);
                Thread.Sleep(50); // Slight delay to prevent script "freeze" in Firefox
            }
        }

        public virtual void ShowDrilldownForMetric(string containerName, string metricName, string drilldownTitle)
        {
            // Trim the incoming labels
            TrimStrings(ref containerName, ref metricName, ref drilldownTitle);

            var targetMetric = Model.FindMetricByContainerAndName(containerName, metricName);

            if (targetMetric == null)
                throw new Exception(string.Format("Unable to locate metric {0} / {1}.", containerName, metricName));

            // If metric was found, expand the More menu.
            ClickDrilldown(targetMetric.MetricId, drilldownTitle);
        }

        protected virtual void ClickDrilldown(int metricId, string drilldownTitle)
        {
            drilldownTitle = drilldownTitle.RemoveWhitespace();

            // First show the context menu
            var moreElement = Browser.FindId(string.Format(MoreMenuIdFormat, metricId));
            moreElement.Click();

            // Click the item by known ID
            var listElement = Browser.FindId(string.Format(MoreMenuActionIdFormat, metricId, drilldownTitle));
            listElement.Click();
        }
    }
}