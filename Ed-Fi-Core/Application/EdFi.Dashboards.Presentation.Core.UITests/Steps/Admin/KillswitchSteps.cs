using EdFi.Dashboards.Presentation.Core.UITests.Pages;
using EdFi.Dashboards.Presentation.Core.UITests.Pages.Admin;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace EdFi.Dashboards.Presentation.Core.UITests.Steps.Admin
{
    [Binding]
    public class KillswitchSteps
    {
        private readonly ConfigurationPage configurationPage;
        private readonly HomePage homePage;

        public KillswitchSteps(ConfigurationPage configurationPage, HomePage homePage)
        {
            this.configurationPage = configurationPage;
            this.homePage = homePage;
        }

        [Given(@"I have ((?:de)?activated) the kill switch")]
        public void GivenIHaveActivatedTheKillSwitch(string activatedOrDeactivated)
        {
            if (activatedOrDeactivated == "activated")
                configurationPage.ActivateKillSwitch();
            else
                configurationPage.DeactivateKillSwitch();
        }

        [Then(@"access to the website data should be prevented")]
        public void ThenAccessToTheSystemShouldBePrevented()
        {
            // Current website behavior is to redirect to the site home page
            Assert.That(homePage.IsCurrent());
        }

        [Then(@"access to the website data should be allowed")]
        public void ThenAccessToTheSystemShouldBeAllowed()
        {
            // Current website behavior is to redirect to the site home page
            Assert.That(homePage.IsCurrent(), Is.Not.True);
        }
    }
}
