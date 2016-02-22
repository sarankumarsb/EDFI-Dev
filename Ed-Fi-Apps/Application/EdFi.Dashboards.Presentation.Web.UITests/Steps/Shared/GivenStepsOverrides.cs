using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BoDi;
using EdFi.Dashboards.Presentation.Core.UITests.Pages;
using EdFi.Dashboards.Presentation.Core.UITests.Support.Coypu;
using EdFi.Dashboards.Presentation.Core.UITests.Support.SpecFlow;
using TechTalk.SpecFlow;

namespace EdFi.Dashboards.Presentation.Web.UITests.Steps.Shared
{
    [Binding]
    public class GivenStepsOverrides
    {
        private IObjectContainer container;

        public GivenStepsOverrides(IObjectContainer container)
        {
            this.container = container;
        }

        public void GivenIAmLoggedOnAs(string userProfileName)
        {
            // Save the user profile for the current scenario context
            ScenarioContext.Current.SetUserProfileName(userProfileName);

            var browser = ScenarioContext.Current.GetBrowser();
            var currentUserProfile = ScenarioContext.Current.GetUserProfile();

            var actualUserFullName = browser.GetLoggedInUserFullName();

            // Did we find a user name, and does it match the current user profile?
            if (!string.IsNullOrWhiteSpace(actualUserFullName)
                && actualUserFullName == currentUserProfile.FullName)
            {
                // We're already logged in as the user
                return;
            }

            // Showing visual of customized behavior
            browser.Visit("http://www.ed-fi.org/");
            Thread.Sleep(2000);

            // Go log the user in
            var page = container.Resolve<LoginPage>();
            page.Login(currentUserProfile.Username, currentUserProfile.Password);
        }
    }
}
