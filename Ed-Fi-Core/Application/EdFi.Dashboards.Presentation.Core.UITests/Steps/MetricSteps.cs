using EdFi.Dashboards.Presentation.Core.UITests.Pages;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace EdFi.Dashboards.Presentation.Core.UITests.Steps
{
    /// <summary>
    /// Contains steps for metrics pages.
    /// </summary>
    [Binding]
    public class MetricsSteps
    {
        private readonly MetricsPage metricsPage;

        public MetricsSteps(MetricsPage metricsPage)
        {
            this.metricsPage = metricsPage;
        }

        [When(@"I show all the drilldowns on the page")]
        public void WhenIShowAllTheDrilldownsOnThePage()
        {
            metricsPage.ShowAllDrilldowns();
        }

        [When(@"I show the (.*) for the (.*)-->(.*) metric")]
        public void WhenIShowTheDrilldownForTheMetric(string actionLabel, string metricContainerName, string metricName)
        {
            metricsPage.ShowDrilldownForMetric(metricContainerName, metricName, actionLabel);
        }

        [Then(@"I should (see|not see) the ""More"" menu")]
        public void IShouldSeeOrNotSeeTheMoreMenu(string seeOrNotSee)
        {
            bool expectedVisibility = (seeOrNotSee == "see");

            Assert.That(metricsPage.AnyMoreMenusVisible, Is.EqualTo(expectedVisibility));
        }

        [Then(@"I should see the correct menu items for each metric action")]
        public void ThenIShouldSeeTheCorrectMenuItemsForEachMetricAction()
        {
            // TODO: Implement for reference
        }
    }
}