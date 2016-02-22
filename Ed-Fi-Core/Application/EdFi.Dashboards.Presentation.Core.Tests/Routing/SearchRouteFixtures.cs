using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Presentation.Core.Areas.Search.Controllers;
using EdFi.Dashboards.Presentation.Core.Tests.Routing.Support;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Mvc.Areas;
using NUnit.Framework;

namespace EdFi.Dashboards.Presentation.Core.Tests.Routing
{
    public partial class When_resolving_routes_to_controllers
    {
        protected Search Search;

        protected virtual void InitializeSearchLinkGenerator(RouteValuesPreparer routeValuesPreparer, HttpRequestProviderFake httpRequestProviderFake)
        {
            Search = new Search();
            Search.RouteValuesPreparer = routeValuesPreparer;
            Search.HttpRequestProvider = httpRequestProviderFake;
        }

        [Test]
        public virtual void Should_go_to_LEA_Quick_Search()
        {
            //"~/Districts/DistrictName/Search/QuickSearch"
            Search.QuickSearchWebService(TestId.LocalEducationAgency, TestId.School, new { textToFind = "Bob", rowCountToReturn = 25, matchContains = true }).ToVirtual()
                  .ShouldMapTo<QuickSearchController>(c => c.Get("Bob", 25, true, TestId.LocalEducationAgency));
        }
        [Test]
        public virtual void Should_go_to_LEA_Quick_Search_School_Id_null()
        {
            //"~/Districts/DistrictName/Search/QuickSearch"
            Search.QuickSearchWebService(TestId.LocalEducationAgency, null, new { textToFind = "Bob", rowCountToReturn = 25, matchContains = true }).ToVirtual()
                  .ShouldMapTo<QuickSearchController>(c => c.Get("Bob", 25, true, TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_LEA_Search_Results()
        {
            //"~/Districts/DistrictName/Search/Results"
            Search.Results(TestId.LocalEducationAgency).ToVirtual()
                  .ShouldMapTo<ResultsController>(c => c.Get(new EdFiDashboardContext()), "GET", true);
        }

        [Test]
        public virtual void Should_go_to_LEA_Search_Log_User_Action()
        {
            //"~/Districts/DistrictName/Search/LogUserAction"
            Search.LogUserActionWebService(TestId.LocalEducationAgency, TestId.School, new { url = "url", controlNameWhoIsCalling = "controlName" }).ToVirtual()
                .ShouldMapTo<LogUserActionController>(c => c.Get("url", "controlName"));
        }

        [Test]
        public virtual void Should_go_to_LEA_Search_Log_User_Action_School_Id_null()
        {
            Search.LogUserActionWebService(TestId.LocalEducationAgency, null, new { url = "url", controlNameWhoIsCalling = "controlName" }).ToVirtual()
                .ShouldMapTo<LogUserActionController>(c => c.Get("url", "controlName"));
        }

        [Test]
        public virtual void Should_go_to_LEA_Search_Log_User_Sorting_Action()
        {
            //"~/Districts/DistrictName/Search/LogUserSortingAction"
            Search.LogUserSortingActionWebService(TestId.LocalEducationAgency, TestId.School).ToVirtual()
                .ShouldMapTo<LogUserSortingActionController>();
        }

        [Test]
        public virtual void Should_go_to_LEA_Search_Log_User_Sorting_Action_School_Id_null()
        {
            Search.LogUserSortingActionWebService(TestId.LocalEducationAgency, null).ToVirtual()
                .ShouldMapTo<LogUserSortingActionController>();
        }

        [Test]
        public virtual void Should_go_to_Search()
        {
            Search.SearchWebService(TestId.LocalEducationAgency, null).ToVirtual()
                .ShouldMapTo<SearchController>(); //TODO: (C -> c.get?)
        }

        [Test]
        public virtual void Should_go_to_Search_School_Id_not_null()
        {
            Search.SearchWebService(TestId.LocalEducationAgency, TestId.School, new
            {
                textToFind = "text", rowCountToReturn = 10, matchContains = true, filter = "filter"
            }).ToVirtual()
                .ShouldMapTo<SearchController>(c => c.Get("text", 10, true, "filter", TestId.LocalEducationAgency), "GET", true);
        }

        //[Test]
        //public virtual void Should_go_to_Transcript_Search()
        //{
        //    Search.TranscriptSearchWebService(TestId.LocalEducationAgency, null).ToVirtual()
        //        .ShouldMapTo<TranscriptSearchController>();
        //}
        ////}
        //[Test]
        //public virtual void Should_go_to_Transcript_Results()
        //{
        //    Search.TranscriptResults(TestId.LocalEducationAgency, null).ToVirtual()
        //        .ShouldMapTo<TranscriptResultsController>();
        //}
    }
}
