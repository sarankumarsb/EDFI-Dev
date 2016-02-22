using System.Linq;
using EdFi.Dashboards.Presentation.Core.UITests.Pages.LocalEducationAgency;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace EdFi.Dashboards.Presentation.Core.UITests.Steps.LocalEducationAgency
{
    [Binding]
    public class OverviewSteps
    {
        private readonly OverviewPage overviewPage;

        public OverviewSteps(OverviewPage overviewPage)
        {
            this.overviewPage = overviewPage;
        }

        [Then(@"the accountability rating label should be correct")]
        public void ThenTheAccountabilityRatingLabelShouldBeCorrect()
        {
            string expectedRatingLabel = overviewPage.Model.AccountabilityRatings.First().Attribute;
            Assert.That(overviewPage.AccountabilityRatingLabel.TrimEnd(':'), Is.EqualTo(expectedRatingLabel));
        }

        [Then(@"the accountability rating should be correct")]
        public void ThenTheAccountabilityRatingShouldBeCorrect()
        {
            string expectedRating = overviewPage.Model.AccountabilityRatings.First().Value;
            Assert.That(overviewPage.AccountabilityRating, Is.EqualTo(expectedRating));
        }
    }
}
