using BoDi;
using EdFi.Dashboards.Presentation.Core.Tests.Routing.Support;
using EdFi.Dashboards.Presentation.Core.UITests.Pages;
using EdFi.Dashboards.Presentation.Core.UITests.Support;
using EdFi.Dashboards.Presentation.Core.UITests.Support.Coypu;
using EdFi.Dashboards.Presentation.Core.UITests.Support.SpecFlow;
using TechTalk.SpecFlow;

namespace EdFi.Dashboards.Presentation.Core.UITests.Steps.Shared
{
    [Binding]
    public class GivenSteps
    {
        

        private readonly IObjectContainer container;

        public GivenSteps(IObjectContainer container)
        {
            this.container = container;
        }

        [Given(@"I attempt to log on as the (.*)")]
        public void GivenIAttemptToLogOnAs(string userProfileName)
        {
            // Save the user profile for the current scenario context
            ScenarioContext.Current.SetUserProfileName(userProfileName);

            var browser = ScenarioContext.Current.GetBrowser();

            // First, make sure user is logged out
            browser.Visit(Website.General.Logout());

            // Now go log in
            GivenIAmLoggedOnAs(userProfileName);
        }

        [Given(@"I am logged on as the (.*)")]
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

            // Go log the user in
            var page = container.Resolve<LoginPage>();
            page.Login(currentUserProfile.Username, currentUserProfile.Password);
        }

        [Given(@"I am on the (Local Education Agency|Student) (.*) page")]
        public void GivenIAmOnTheSomethingPage(AcademicDashboardType academicDashboardType, string pageName)
        {
            var page = container.ResolvePageObject(academicDashboardType, pageName);
            page.Visit();
        }

        [Given(@"I am on the (High School|Middle School|Elementary School) (.*) page")]
        public void GivenIAmOnTheSchoolSomethingPage(SchoolType schoolType, string pageName)
        {
            ScenarioContext.Current.SetSchoolType(schoolType);

            var page = container.ResolvePageObject(AcademicDashboardType.School, pageName, schoolType);
            page.Visit();
        }

        [Given(@"I am on the teacher's (.*) page")]
        public void GivenIAmOnTheTeachersPage(string pageName)
        {
            // TODO: Do we need to establish school context for the teacher?
            // ScenarioContext.Current.SetSchoolType(schoolType);

            var page = container.ResolveStaffPageObject(pageName);
            page.Visit();
        }

        [Given(@"I go to the teacher's (.*) page")]
        public void GivenIGoToTheTeachersPage(string pageName)
        {
            // TODO: Do we need to establish school context for the teacher?
            // ScenarioContext.Current.SetSchoolType(schoolType);

            var page = container.ResolveStaffPageObject(pageName);
            page.Visit(forceNavigation: true);
        }

        [Given(@"I am on the (.*) page of the (Local Education Agency|Student) Academic Dashboard")]
        public void GivenIAmOnThePageOfTheAcademicDashboard(string subPageName, AcademicDashboardType academicDashboardType)
        {
            // Perform direct navigation to the Overview page, then use links to get to sub-types
            // TODO: Reconsider location of this method (something other than PageBase?)
            PageBase.VisitAcademicDashboardSection(academicDashboardType, subPageName);
        }

        [Given(@"I go to the (.*) page of the (Local Education Agency|Student) Academic Dashboard")]
        public void GivenIGoToThePageOfTheAcademicDashboard(string subPageName, AcademicDashboardType academicDashboardType)
        {
            // Perform direct navigation to the Overview page, then use links to get to sub-types
            // TODO: Reconsider location of this method (something other than PageBase?)
            PageBase.VisitAcademicDashboardSection(academicDashboardType, subPageName, forceNavigation: true);
        }

        [Given(@"I am on the (.*) page of the (High School|Middle School|Elementary School) Academic Dashboard")]
        public void GivenIAmOnThePageOfTheAcademicDashboard(string subPageName, SchoolType schoolType)
        {
            // Perform direct navigation to the Overview page, then use links to get to sub-types
            PageBase.VisitAcademicDashboardSection(AcademicDashboardType.School, subPageName, schoolType);
        }

        [Given(@"I go to the (.*) page of the (High School|Middle School|Elementary School) Academic Dashboard")]
        public void GivenIGoToThePageOfTheAcademicDashboard(string subPageName, SchoolType schoolType)
        {
            // Perform direct navigation to the Overview page, then use links to get to sub-types
            PageBase.VisitAcademicDashboardSection(AcademicDashboardType.School, subPageName, schoolType, 
                forceNavigation: true);
        }

        [Given("I refresh the current web page")]
        public void GivenIRefreshTheCurrentWebPage()
        {
            var browser = ScenarioContext.Current.GetBrowser();
            browser.Visit(browser.Location.ToString());
        }
    }
}
