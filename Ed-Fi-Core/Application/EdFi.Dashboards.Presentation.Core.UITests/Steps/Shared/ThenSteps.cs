using BoDi;
using EdFi.Dashboards.Presentation.Core.UITests.Pages;
using EdFi.Dashboards.Presentation.Core.UITests.Pages.Student;
using EdFi.Dashboards.Presentation.Core.UITests.Support;
using EdFi.Dashboards.Presentation.Core.UITests.Support.SpecFlow;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace EdFi.Dashboards.Presentation.Core.UITests.Steps.Shared
{
    [Binding]
    public class ThenSteps
    {
        private readonly IObjectContainer container;

        public ThenSteps(IObjectContainer container)
        {
            this.container = container;
        }

        [Then(@"I should see the text ""(.*)""")]
        public void ThenIShouldSeeTheText(string text)
        {
            Assert.That(
                ScenarioContext.Current.GetBrowser()
                    .HasContent(text));
        }

        [Then(@"I should be on the (Local Education Agency|School|Student) (.*) page")]
        public void ThenIShouldBeOnACertainPage(AcademicDashboardType dashboardType, string pageName)
        {
            var page = container.ResolvePageObject(dashboardType, pageName);

            Assert.That(page.IsCurrent());
        }

        [Then(@"I should be on that student's page")]
        public void ThenIShouldBeOnThatStudentsPage()
        {
            var studentOverviewPage = container.Resolve<OverviewPage>();

            //var studentClicked = ScenarioContext.Current.Get<string>(PageBase.StudentNameContextKey);

            Assert.That(studentOverviewPage.IsCurrent(), "Browser was not on expected student's page");

            //var parsedname = PageBase.QuickSearchAndSelectFirstStudent(ScenarioContext.Current.GetBrowser(), studentClicked);
            var parsedname = ScenarioContext.Current.Get<PageBase.StudentFirstAndLastNames>(PageBase.StudentNameContextKey);
            string studentNameFromStudentPage = studentOverviewPage.GetStudentNameOnStudentPage(ScenarioContext.Current.GetBrowser());
            
            Assert.That(studentNameFromStudentPage.Contains(parsedname.FirstName));
            Assert.That(studentNameFromStudentPage.Contains(parsedname.LastName));
        }
    }
}
