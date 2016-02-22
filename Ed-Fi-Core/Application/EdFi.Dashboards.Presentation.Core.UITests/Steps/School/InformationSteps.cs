using System;
using EdFi.Dashboards.Presentation.Core.UITests.Pages.School;
using EdFi.Dashboards.Presentation.Core.UITests.Support.SpecFlow;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace EdFi.Dashboards.Presentation.Core.UITests.Steps.School
{
    [Binding]
    public class InformationSteps
    {
        private readonly InformationPage informationPage;

        public InformationSteps(InformationPage informationPage)
        {
            this.informationPage = informationPage;
        }

        [Then(@"I should see the correct student gender demographics")]
        public void ThenIShouldSeeTheCorrectStudentGenderDemographics()
        {
            // Set the school type for the page
            informationPage.For(ScenarioContext.Current.GetSchoolType()); 
            
            // Get expected values from the model (NOTE: proper null handling not implemented)
            var expectedFemaleValue = Math.Round(informationPage.Model.StudentDemographics.Female.Value ?? 0, 3);
            var expectedMaleValue = Math.Round(informationPage.Model.StudentDemographics.Male.Value ?? 0, 3);

            // Get actual values from the page
            var actualFemaleValue = (informationPage.PercentFemales ?? 0) / 100M;
            var actualMaleValue = (informationPage.PercentMales ?? 0) / 100M;

            // Make sure they match
            Assert.That(actualFemaleValue, Is.EqualTo(expectedFemaleValue));
            Assert.That(actualMaleValue, Is.EqualTo(expectedMaleValue));
        }
    }
}
