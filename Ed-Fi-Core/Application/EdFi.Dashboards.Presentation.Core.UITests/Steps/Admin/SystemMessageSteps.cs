using System;
using EdFi.Dashboards.Presentation.Core.UITests.Pages.Admin;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace EdFi.Dashboards.Presentation.Core.UITests.Steps.Admin
{
    [Binding]
    public class SystemMessageSteps
    {
        private const string SystemMessageTextContextKey = "systemMessageText";

        private readonly ConfigurationPage configurationPage;

        public SystemMessageSteps(ConfigurationPage configurationPage)
        {
            this.configurationPage = configurationPage;
        }

        [Given(@"I enter a system message")]
        public void GivenIEnterASystemMessage()
        {
            // Create a unique message to be displayed
            string messageText = string.Format("System message for UI testing set ({0}).", DateTime.Now.Ticks);

            // Save the text to the scenario context
            ScenarioContext.Current[SystemMessageTextContextKey] = messageText;

            // Set the system message
            configurationPage.SetSystemMessage(messageText);
        }

        [Given(@"I clear the system message")]
        public void GivenIClearTheSystemMessage()
        {
            // Set the system message
            configurationPage.SetSystemMessage(string.Empty);
        }

        [Then(@"the previously set system message should be displayed")]
        public void ThenThePreviouslySetSystemMessageShouldBeDisplayed()
        {
            // Get the system message text from the scenario context
            string expectedMessage = ScenarioContext.Current.Get<string>(SystemMessageTextContextKey);
            string actualMessage = configurationPage.GetSystemMessageText();

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
        }

        [Then(@"the system message should not be displayed")]
        public void ThenNoSystemMessageShouldBeDisplayed()
        {
            // Get the system message text from the scenario context
            string actualMessage = configurationPage.GetSystemMessageText();

            
            Assert.That(string.IsNullOrEmpty(actualMessage));
        }
    }
}
