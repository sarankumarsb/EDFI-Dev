using EdFi.Dashboards.Application.Resources.Models.School;
using EdFi.Dashboards.Application.Resources.School;
using EdFi.Dashboards.Presentation.Architecture.Mvc.Controllers;
using EdFi.Dashboards.Presentation.Core.Areas.School.Controllers;
using EdFi.Dashboards.Presentation.Core.Areas.School.Controllers.Detail;
using EdFi.Dashboards.Presentation.Core.Controllers;
using EdFi.Dashboards.Presentation.Core.Tests.Routing.Support;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Models.Charting;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.School.Information;
using EdFi.Dashboards.Resources.Models.School.Overview;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Mvc.Areas;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.School.Detail;
using NUnit.Framework;

namespace EdFi.Dashboards.Presentation.Core.Tests.Routing
{
    public partial class When_resolving_routes_to_controllers
    {
        protected School School;

        protected virtual void InitializeSchoolLinkGenerator(RouteValuesPreparer routeValuesPreparer, HttpRequestProviderFake httpRequestProviderFake)
        {
            School = new School(null);
            School.RouteValuesPreparer = routeValuesPreparer;
            School.HttpRequestProvider = httpRequestProviderFake;
        }

        [Test]
        public virtual void Should_go_to_school_Default()
        {
            //"~/Districts/DistrictName/Schools/School-Name"
            School.Default(TestId.School, "SchoolName").ToVirtual()
                .ShouldMapTo<ServicePassthroughController<OverviewRequest, OverviewModel>>
                (c => c.Get(OverviewRequest.Create(TestId.School), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_school_Entry()
        {
            //"~/Districts/DistrictName/Schools/School-Name"
            School.Entry(TestName.LocalEducationAgency, TestId.School, TestName.School).ToVirtual()
                .ShouldMapTo<EntryController>
                (c => c.Get(EntryRequest.Create(TestId.LocalEducationAgency, TestId.School)));
        }

        [Test]
        public virtual void Should_go_to_school_Watch_List_Search_Controller()
        {
            var request = new MetricsBasedWatchListSearchRequest
            {
                SchoolId = TestId.School
            };

            School.Resource(TestId.School, "MetricsBasedWatchListSearch").ToVirtual()
                .ShouldMapTo<MetricsBasedWatchListSearchController>(c => c.Get(request));
        }

        [Test]
        public virtual void Should_go_to_school_Overview()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Overview"
            School.Overview(TestId.School, TestName.School).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<OverviewRequest, OverviewModel>>
                (c => c.Get(OverviewRequest.Create(TestId.School), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_school_Information()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Information"
            School.Information(TestId.School, TestName.School).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<InformationRequest, InformationModel>>
                (c => c.Get(InformationRequest.Create(TestId.School), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_student_demographic()
        {
            School.StudentDemographicList(TestId.School, TestName.Demographic).ToVirtual()
                  .ShouldMapTo<StudentDemographicListController>();
        }

        [Test]
        public virtual void Should_got_to_student_demographic()
        {
            School.StudentsByGrade(TestId.School, TestName.School).ToVirtual()
                  .ShouldMapTo<StudentsByGradeController>(c => c.Get(StudentsByGradeRequest.Create(TestId.School), TestId.LocalEducationAgency));

        }
        [Test]
        public virtual void Should_go_to_school_Metrics()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Metrics/Metric-Name-1234"
            School.Metrics(TestId.School, TestId.MetricVariant, TestName.School).ToVirtual()
                .ShouldMapTo<DomainMetricController>
                (c => c.Get(SchoolMetricInstanceSetRequest.Create(TestId.School, TestId.MetricVariant), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_school_Operational_Dashboard()
        {
            //"~/Districts/DistrictName/Schools/School-Name/OperationalDashboard"
            School.OperationalDashboard(TestId.School, TestName.School).ToVirtual()
                .ShouldMapTo<DomainMetricController>
                (c => c.Get(SchoolMetricInstanceSetRequest.Create(TestId.School, TestId.MetricVariant), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_school_Goal_Planning()
        {
            //"~/Districts/DistrictName/Schools/School-Name/GoalPlanning/Metric-Name-1234"
            School.GoalPlanning(TestId.School, TestId.MetricVariant, TestName.School).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<GoalPlanningGetRequest, GoalPlanningModel>>
                (c => c.Get(GoalPlanningGetRequest.Create(TestId.School, TestId.MetricVariant), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_school_Export_All_Metrics()
        {
            School.ExportAllMetrics(TestId.School).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<ExportAllMetricsRequest, ExportAllModel>>
                (c => c.Get(ExportAllMetricsRequest.Create(TestId.School), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_school_Export_Metric_List()
        {
            School.ExportMetricList(TestId.School, 1234).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<ExportMetricListRequest, StudentExportAllModel>>
                (c => c.Get(ExportMetricListRequest.Create(TestId.School, 1234), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_school_Export_Student_Demographic_List()
        {
            School.ExportStudentDemographicList(TestId.School, "gender").ToVirtual()
                .ShouldMapTo<ServicePassthroughController<ExportStudentDemographicListRequest, StudentExportAllModel>>
                (c => c.Get(ExportStudentDemographicListRequest.Create(TestId.School, "gender"), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_school_Export_Student_Grade_List()
        {
            School.ExportStudentGradeList(TestId.School, "12th").ToVirtual()
                .ShouldMapTo<ServicePassthroughController<ExportStudentGradeListRequest, StudentExportAllModel>>
                (c => c.Get(ExportStudentGradeListRequest.Create(TestId.School, "12th"), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_school_Staff_List()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Staff"
            School.Staff(TestId.School, TestName.School).ToVirtual()
                .ShouldMapTo<StaffController>
                (c => c.Get(TestId.School, TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_school_Teachers()
        {
            // "~/Districts/DistrictName/Schools/School-Name/Teachers"
            School.Teachers(TestId.School, TestName.School).ToVirtual()
                .ShouldMapTo<TeachersController>
                (c => c.Get(TestId.School, TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_school_Students_by_Grade()
        {
            //"~/Districts/DistrictName/Schools/School-Name/StudentsByGrade"
            School.StudentsByGrade(TestId.School, TestName.School).ToVirtual()
                .ShouldMapTo<StudentsByGradeController>
                (c => c.Get(StudentsByGradeRequest.Create(TestId.School), TestId.LocalEducationAgency));
        }

        // ---------------------
        //   Detail Drilldowns 
        // ---------------------

        [Test]
        public virtual void Should_go_to_school_Metric_detail_Historical()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Metrics/Metric-Name-1234/HistoricalChart"
            School.MetricsDrilldown(TestId.School, TestId.MetricVariant, "HistoricalChart", TestName.School).ToVirtual()
                .ShouldMapTo<HistoricalChartController>(); // TODO: Create appropriate request model for this resource.  EdFiDashboardContext is not intended for this purpose.
        }

        [Test]
        public virtual void Should_go_to_school_Metric_detail_Grade_Level_Chart()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Metrics/Metric-Name-1234/GradeLevelChart"
            School.MetricsDrilldown(TestId.School, TestId.MetricVariant, "GradeLevelChart", TestName.School, new { title = "The Chart Title." }).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<GradeLevelChartRequest, ChartData>>
                (c => c.Get(GradeLevelChartRequest.Create(TestId.School, TestId.MetricVariant, "The%20Chart%20Title."), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_school_Metric_detail_Student_List()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Metrics/Metric-Name-1234/StudentMetricTable"
            School.MetricsDrilldown(TestId.School, TestId.MetricVariant, "StudentMetricTable", TestName.School).ToVirtual()
                .ShouldMapTo<StudentMetricTableController>
                (c => c.Get(StudentMetricListMetaRequest.Create(TestId.School, TestId.MetricVariant)));
        }

        [Test]
        public virtual void Should_go_to_school_Metric_detail_Assessment_Rate_Chart()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Metrics/Metric-Name-1234/AssessmentRateChart"
            School.MetricsDrilldown(TestId.School, TestId.MetricVariant, "AssessmentRateChart", TestName.School).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<AssessmentRateChartRequest, AssessmentRateChartModel>>
                (c => c.Get(AssessmentRateChartRequest.Create(TestId.School, TestId.MetricVariant), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_school_Operational_Dashboard_detail_Staff_Metric_List()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Metrics/Metric-Name-1234/StaffMetricTable"
            School.MetricsDrilldown(TestId.School, TestId.MetricVariant, "StaffMetricTable", TestName.School).ToVirtual()
                .ShouldMapTo<StaffMetricTableController>(); // TODO: EdFiDashboardContext should be replaced with proper request model class.
        }

        [Test]
        public virtual void Should_go_to_school_Metric_Detail_Historical_Learning_Objective_Chart()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Metrics/Metric-Name-1234/HistoricalLearningObjectivesChart"
            School.MetricsDrilldown(TestId.School, TestId.MetricVariant, "HistoricalLearningObjectivesChart", TestName.School).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<HistoricalLearningObjectivesChartRequest, ChartData>>
                (c => c.Get(HistoricalLearningObjectivesChartRequest.Create(TestId.School, TestId.MetricVariant), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_school_Grades()
        {
            School.StudentGradeList(TestId.School, TestName.School).ToVirtual()
                  .ShouldMapTo<StudentGradeListController>();
        }

        [Test]
        public virtual void Should_go_to_school_Custom_Metrics_Based_Watch_List_Grade()
        {
            School.CustomMetricsBasedWatchListGrade(TestId.School, "MetricsBasedWatchListSearch", "grade", 7, "All").ToVirtual()
                .ShouldMapTo<MetricsBasedWatchListSearchController>
                (c => c.Get(new MetricsBasedWatchListSearchRequest()), "GET", true);
        }

        [Test]
        public virtual void Should_go_to_school_Custom_Metrics_Based_Watch_List_Demographic()
        {
            School.CustomMetricsBasedWatchListDemographic(TestId.School, "MetricsBasedWatchListSearch", "grade", 7, "All").ToVirtual()
                .ShouldMapTo<MetricsBasedWatchListSearchController>
                (c => c.Get(new MetricsBasedWatchListSearchRequest()), "GET", true);
        }
    }
}
