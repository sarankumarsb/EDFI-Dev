using EdFi.Dashboards.Presentation.Core.UITests.Pages;
using EdFi.Dashboards.Presentation.Core.UITests.Support;
using EdFi.Dashboards.Presentation.Core.UITests.Support.Coypu;
using EdFi.Dashboards.Presentation.Core.UITests.Support.SpecFlow;
using EdFi.Dashboards.Resources.Security.Implementations;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace EdFi.Dashboards.Presentation.Core.UITests.Steps
{
    [Binding]
    public class LoginSteps
    {
        private readonly LoginPage loginPage;

        public LoginSteps(LoginPage loginPage)
        {
            this.loginPage = loginPage;
        }

        [Given(@"I am at the login page")]
        public void GivenIAmAtTheLoginPage()
        {
            loginPage.Visit();
        }

        [Given(@"I have entered an incorrect password")]
        public void GivenIHaveEnteredAnIncorrectPassword()
        {
            loginPage.Username = "some-user";
            loginPage.Password = AlwaysValidAuthenticationProvider.IncorrectPassword;
        }

        [Given(@"I have entered the correct password for the (.*)")]
        public void GivenIHaveEnteredTheCorrectPasswordFor(string userProfileName)
        {
            // For testing login feature, just use the superintendent
            var user = TestSessionContext.Current.UserProfiles[userProfileName];

            // Save the full name of the current user to the scenario context
            ScenarioContext.Current.Set(user.FullName, "userFullName");

            loginPage.Username = user.Username;
            loginPage.Password = user.Password;
        }

        [When(@"I click the Login button")]
        public void WhenIClickTheLoginButton()
        {
            loginPage.ClickLoginButton();
        }

        [When(@"I hit the ENTER key")]
        public void WhenIHitTheEnterKey()
        {
            loginPage.HitEnter();
        }

        [Then(@"I should see an error message containing the text ""(.*)""")]
        public void ThenIShouldSeeAnErrorMessageContainingTheText(string message)
        {
            Assert.That(loginPage.ErrorMessage.Contains(message));
        }

        [Then(@"I should be logged in successfully")]
        public void ThenIShouldBeLoggedOnSuccessfully()
        {
            string actualUserFullName = ScenarioContext.Current.GetBrowser().GetLoggedInUserFullName(Make_It.Wait_10_Seconds);
            string expectedUserFullName = ScenarioContext.Current.Get<string>("userFullName");

            Assert.That(actualUserFullName, Is.EqualTo(expectedUserFullName));
        }
    }
}
