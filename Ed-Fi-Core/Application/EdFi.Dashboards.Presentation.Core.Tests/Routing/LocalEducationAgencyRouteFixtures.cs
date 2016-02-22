using EdFi.Dashboards.Application.Resources.LocalEducationAgency;
using EdFi.Dashboards.Application.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Presentation.Architecture.Mvc.Controllers;
using EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers;
using EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers.Detail;
using EdFi.Dashboards.Presentation.Core.Controllers;
using EdFi.Dashboards.Presentation.Core.Tests.Routing.Support;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Images.Navigation;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency.Information;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency.Overview;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Mvc.Areas;
using NUnit.Framework;
using HomeController = EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers.HomeController;

namespace EdFi.Dashboards.Presentation.Core.Tests.Routing
{
    public partial class When_resolving_routes_to_controllers
    {
        protected LocalEducationAgency LocalEducationAgency;

        protected virtual void InitializeLocalEducationAgencyLinkGenerator(RouteValuesPreparer routeValuesPreparer, HttpRequestProviderFake httpRequestProviderFake)
        {
            LocalEducationAgency = new LocalEducationAgency(new LocalEducationAgencyRandomImageLinkProvider(new NullImageLinkProvider()));
            LocalEducationAgency.RouteValuesPreparer = routeValuesPreparer;
            LocalEducationAgency.HttpRequestProvider = httpRequestProviderFake;
        }

        [Test]
        public virtual void Should_go_to_local_education_agency_Entry()
        {
            //"~/Districts/DistrictName/Entry"
            LocalEducationAgency.Entry(TestName.LocalEducationAgency).ToVirtual()
                .ShouldMapTo<EntryController>
                (c => c.Get(EntryRequest.Create(TestId.LocalEducationAgency, null)));
        }

        [Test]
        public virtual void Should_go_to_local_education_agency_Home()
        {
            //"~/Districts/DistrictName"
            LocalEducationAgency.Home(TestName.LocalEducationAgency).ToVirtual()
                .ShouldMapTo<HomeController>
                (c => c.Get(TestName.LocalEducationAgency, TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_local_education_agency_Information()
        {
            //"~/Districts/DistrictName/Information"
            LocalEducationAgency.Information(TestId.LocalEducationAgency).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<InformationRequest, InformationModel>>
                (c => c.Get(InformationRequest.Create(TestId.LocalEducationAgency), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_local_education_agency_School_Category_List()
        {
            //"~/Districts/DistrictName/SchoolCategoryList"
            LocalEducationAgency.SchoolCategoryList(TestId.LocalEducationAgency).ToVirtual()
                .ShouldMapTo<SchoolCategoryListController>
                (c => c.Get(SchoolCategoryListRequest.Create(TestId.LocalEducationAgency), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_local_education_agency_Overview()
        {
            //"~/Districts/DistrictName/Overview"
            LocalEducationAgency.Overview(TestId.LocalEducationAgency).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<OverviewRequest, OverviewModel>>
                (c => c.Get(OverviewRequest.Create(TestId.LocalEducationAgency), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_local_education_agency_Metric()
        {
            //"~/Districts/DistrictName/Metrics/Metric-Name-1234"
            LocalEducationAgency.Metrics(TestId.LocalEducationAgency, 1234).ToVirtual()
                .ShouldMapTo<DomainMetricController>
                (c => c.Get(LocalEducationAgencyMetricInstanceSetRequest.Create(TestId.LocalEducationAgency, 1234), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_local_education_agency_School_Metric_Table()
        {
            //"~/Districts/DistrictName/Metrics/Metric-Name-1234/SchoolMetricTable"
            LocalEducationAgency.MetricsDrilldown(TestId.LocalEducationAgency, 1234, "SchoolMetricTable").ToVirtual()
                .ShouldMapTo<SchoolMetricTableController>();
            // TODO: Why is this controller's Get action accepting the EdFiDashboardContext as a model? I think it was a misguided attempt to force MVC to model bind desired values and should be removed.
        }

        [Test]
        public virtual void Should_go_to_local_education_agency_Goal_Planning()
        {
            //"~/Districts/DistrictName/GoalPlanning/Metric-Name-1234"
            LocalEducationAgency.GoalPlanning(TestId.LocalEducationAgency, 1234).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<GoalPlanningGetRequest, GoalPlanningModel>>
                (c => c.Get(GoalPlanningGetRequest.Create(TestId.LocalEducationAgency, 1234), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_local_education_agency_Export_All_Metrics()
        {
            LocalEducationAgency.ExportAllMetrics(TestId.LocalEducationAgency).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<ExportAllMetricsRequest, ExportAllModel>>
                (c => c.Get(ExportAllMetricsRequest.Create(TestId.LocalEducationAgency), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_local_education_agency_Export_Metric_List()
        {
            LocalEducationAgency.ExportMetricList(TestId.LocalEducationAgency, 1234).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<ExportMetricListRequest, ExportAllModel>>
                (c => c.Get(ExportMetricListRequest.Create(TestId.LocalEducationAgency, 1234), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_local_education_agency_Export_Student_Demographic_List()
        {
            LocalEducationAgency.ExportStudentDemographicList(TestId.LocalEducationAgency, "gender").ToVirtual()
                .ShouldMapTo<ServicePassthroughController<ExportStudentDemographicListRequest, StudentExportAllModel>>
                (c => c.Get(ExportStudentDemographicListRequest.Create(TestId.LocalEducationAgency, "gender"), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_local_education_agancy_Export_Student_List()
        {
            LocalEducationAgency.ExportStudentList(TestId.LocalEducationAgency, 1234, null, 7, "student").ToVirtual()
                .ShouldMapTo<ServicePassthroughController<ExportStudentListRequest, StudentExportAllModel>>
                (c => c.Get(ExportStudentListRequest.Create(TestId.LocalEducationAgency, 1234, 7, "student"), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_local_education_agency_Export_Student_School_Category_List()
        {
            LocalEducationAgency.ExportStudentSchoolCategoryList(TestId.LocalEducationAgency, "category").ToVirtual()
                .ShouldMapTo<ServicePassthroughController<ExportStudentSchoolCategoryListRequest, StudentExportAllModel>>
                (c => c.Get(ExportStudentSchoolCategoryListRequest.Create(TestId.LocalEducationAgency, "category"), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_local_education_agency_Operational_Dashboard()
        {
            //"~/Districts/DistrictName/OperationalDashboard"
            LocalEducationAgency.OperationalDashboard(TestId.LocalEducationAgency).ToVirtual()
                .ShouldMapTo<DomainMetricController>
                (c => c.Get(LocalEducationAgencyMetricInstanceSetRequest.Create(TestId.LocalEducationAgency, TestId.MetricVariant), TestId.LocalEducationAgency));
            // Note: Mapping of operationalDashboardSubtype is handled in FakeIdValueProvider constructor that effectively lets it get treated as a metricVariantId from the TestId class
        }

        [Test]
        public virtual void Should_go_to_local_education_agency_entry()
        {
            LocalEducationAgency.Entry(TestName.LocalEducationAgency).ToVirtual()
                                .ShouldMapTo<EntryController>(
                                    c => c.Get(EntryRequest.Create(TestId.LocalEducationAgency, null)));
        }

        [Test]
        public virtual void Should_go_to_local_education_agency_Watch_List_Search_Controller()
        {
            var request = new MetricsBasedWatchListSearchRequest
            {
                LocalEducationAgencyId = TestId.LocalEducationAgency
            };

            LocalEducationAgency.Resource(TestId.LocalEducationAgency, "MetricsBasedWatchListSearch").ToVirtual()
                .ShouldMapTo<MetricsBasedWatchListSearchController>(c => c.Get(request));
        }

        [Test]
        public virtual void Should_go_to_local_education_agency_Student_demographics_list()
        {
            LocalEducationAgency.StudentDemographicList(TestId.LocalEducationAgency, TestName.Demographic).ToVirtual()
                .ShouldMapTo<StudentDemographicListController>(); // TODO: Eliminate use of the EdFiDashboardContext as a request model
        }

        [Test]
        public virtual void Should_go_to_local_education_agency_Student_School_Category_List()
        {
            LocalEducationAgency.StudentSchoolCategoryList(TestId.LocalEducationAgency, "category", new { sectionOrCohortId = 7, StudentListType = "list" }).ToVirtual()
                .ShouldMapTo<StudentSchoolCategoryListController>(
                c => c.Get(StudentSchoolCategoryListMetaRequest.Create(TestId.LocalEducationAgency, 7, "list", "category")));
        }

        [Test]
        public virtual void Should_go_to_School_category_list()
        {
            LocalEducationAgency.SchoolCategoryList(TestId.LocalEducationAgency).ToVirtual()
                .ShouldMapTo<SchoolCategoryListController>
                (c => c.Get(SchoolCategoryListRequest.Create(TestId.LocalEducationAgency), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_Student_List_Controller()
        {
            LocalEducationAgency.StudentList(TestId.LocalEducationAgency, TestId.Staff, TestName.Staff, TestId.SectionOrCohort, StudentListType.CustomStudentList.ToString(), new { pageNumber = 1, pageSize = 10, sortColumn = 100, sortDirection = "desc" }).ToVirtual()
                                .ShouldMapTo<StudentListController>(c => c.Get(TestId.Staff, StudentListType.CustomStudentList.ToString(), TestId.SectionOrCohort, TestId.LocalEducationAgency, 1, 10, 100, "desc"));
        }

#if DEBUG
        [Test]
        public virtual void Should_go_to_Api()
        {
            LocalEducationAgency.ApiResource(TestId.LocalEducationAgency, TestId.MetricVariant, typeof(PlannedGoalsService).GetResourceName()).ToVirtual()
                                .ShouldMapTo<ServicePassthroughController<PlannedGoalsItemRequest, PlannedGoalModel>>(c => c.Get(PlannedGoalsItemRequest.Create(TestId.LocalEducationAgency, TestId.MetricVariant), TestId.LocalEducationAgency));
        }
#endif
    }
}
