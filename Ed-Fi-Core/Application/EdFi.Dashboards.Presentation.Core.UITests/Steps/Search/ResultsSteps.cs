using BoDi;
using EdFi.Dashboards.Presentation.Core.UITests.Pages;
using EdFi.Dashboards.Presentation.Core.UITests.Pages.Search;
using EdFi.Dashboards.Presentation.Core.UITests.Support.SpecFlow;
using TechTalk.SpecFlow;

namespace EdFi.Dashboards.Presentation.Core.UITests.Steps.Search
{
    [Binding]
    public class ResultsSteps
    {
        private readonly ResultsPage resultsPage;

        public ResultsSteps(ResultsPage resultsPage)
        {
            this.resultsPage = resultsPage;
        }
        
        [When(@"I search for and select a student")]
        public void WhenISearchForAndSelectAStudent()
        {
            PageBase.SearchStudents(ScenarioContext.Current.GetBrowser(), "e");
            var names = resultsPage.SelectFirstSearchResult();
            ScenarioContext.Current.Set(names, PageBase.StudentNameContextKey);
        }

    }
}
