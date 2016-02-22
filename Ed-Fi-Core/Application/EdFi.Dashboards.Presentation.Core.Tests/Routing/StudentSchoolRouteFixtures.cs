using System.Collections.Generic;
using EdFi.Dashboards.Presentation.Architecture.Mvc.Controllers;
using EdFi.Dashboards.Presentation.Core.Areas.StudentSchool.Controllers;
using EdFi.Dashboards.Presentation.Core.Areas.StudentSchool.Controllers.Detail;
using EdFi.Dashboards.Presentation.Core.Tests.Routing.Support;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Models.Charting;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.Student.AcademicProfile;
using EdFi.Dashboards.Resources.Models.Student.Detail;
using EdFi.Dashboards.Resources.Models.Student.Detail.AssessmentHistory;
using EdFi.Dashboards.Resources.Models.Student.Detail.CurrentCourses;
using EdFi.Dashboards.Resources.Models.Student.Information;
using EdFi.Dashboards.Resources.Models.Student.Overview;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Mvc.Areas;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.Resources.StudentSchool.Detail;
using NUnit.Framework;

namespace EdFi.Dashboards.Presentation.Core.Tests.Routing
{
    public partial class When_resolving_routes_to_controllers
    {
        protected StudentSchool Student;

        protected virtual void InitializeStudentSchoolLinkGenerator(RouteValuesPreparer routeValuesPreparer, HttpRequestProviderFake httpRequestProviderFake)
        {
            Student = new StudentSchool(null);
            Student.RouteValuesPreparer = routeValuesPreparer;
            Student.HttpRequestProvider = httpRequestProviderFake;
        }

        [Test]
        public virtual void Should_go_to_student_school_Default()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Students/StudentNameFirst-M--Last-123456"
            
            // Special handling required because LocalEducationAgencyId property was not included in factory method
            var overviewRequest = OverviewRequest.Create(TestId.School, TestId.Student);
            overviewRequest.LocalEducationAgencyId = TestId.LocalEducationAgency;

            Student.Default(TestId.School, TestId.Student, TestName.Student).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<OverviewRequest, OverviewModel>>
                (c => c.Get(overviewRequest, TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_student_school_Overview()
        {
            Student.Overview(TestId.School, TestId.Student, TestName.Student).ToVirtual()
            //"~/Districts/DistrictName/Schools/School-Name/Students/StudentNameFirst-M--Last-123456/Overview"
                .ShouldMapTo<ServicePassthroughController<OverviewRequest, OverviewModel>>(c => c.Get(OverviewRequest.Create(TestId.LocalEducationAgency, TestId.School, TestId.Student), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_student_school_Information()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Students/StudentNameFirst-M--Last-123456/Information"
            Student.Information(TestId.School, TestId.Student, TestName.Student).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<InformationRequest, InformationModel>>(c => c.Get(InformationRequest.Create(TestId.LocalEducationAgency, TestId.School, TestId.Student), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_student_school_Metrics()
        {
            Student.Metrics(TestId.School, TestId.Student, TestId.MetricVariant, TestName.Student).ToVirtual()
            //"~/Districts/DistrictName/Schools/School-Name/Students/StudentNameFirst-M--Last-123456/Metrics/Metric-Name-1234"
                .ShouldMapTo<DomainMetricController>(c => c.Get(StudentSchoolMetricInstanceSetRequest.Create(TestId.School, TestId.Student, TestId.MetricVariant), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_student_school_Academic_Profile()
        {
            Student.AcademicProfile(TestId.School, TestId.Student, TestName.Student).ToVirtual()
            //"~/Districts/DistrictName/Schools/School-Name/Students/StudentNameFirst-M--Last-123456/AcademicProfile"
                .ShouldMapTo<ServicePassthroughController<AcademicProfileRequest, AcademicProfileModel>>(c => c.Get(AcademicProfileRequest.Create(TestId.Student, TestId.School), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_student_school_Export_All_Metrics()
        {
            Student.ExportAllMetrics(TestId.School, TestId.Student, TestName.Student).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<ExportAllMetricsRequest, ExportAllModel>>
                (c => c.Get(ExportAllMetricsRequest.Create(TestId.School, TestId.Student), TestId.LocalEducationAgency));
        }

        // ---------------------
        //   Detail Drilldowns 
        // ---------------------

        [Test]
        public virtual void Should_go_to_student_school_Metric_Detail_Historical_Chart()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Students/StudentNameFirst-M--Last-123456/Metrics/Metric-Name-1234/HistoricalChart"
            Student.MetricsDrilldown(TestId.School, TestId.Student, TestId.MetricVariant, "HistoricalChart", TestName.Student).ToVirtual()
                .ShouldMapTo<HistoricalChartController>(); 
        }

        [Test]
        public virtual void Should_go_to_student_school_Metric_Detail_Daily_Attendance_Chart()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Students/StudentNameFirst-M--Last-123456/Metrics/Metric-Name-1234/DailyAttendanceChart"
            Student.MetricsDrilldown(TestId.School, TestId.Student, TestId.MetricVariant, "DailyAttendanceChart", TestName.Student).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<DailyAttendanceChartRequest, ChartData>>();
        }

        [Test]
        public virtual void Should_go_to_student_school_Metric_Detail_Days_Absent_List()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Students/StudentNameFirst-M--Last-123456/Metrics/Metric-Name-1234/DaysAbsentList"
            Student.MetricsDrilldown(TestId.School, TestId.Student, TestId.MetricVariant, "DaysAbsentList", TestName.Student).ToVirtual()    
                .ShouldMapTo<ServicePassthroughController<DaysAbsentListRequest, IList<DaysAbsentModel>>>();
        }

        [Test]
        public virtual void Should_go_to_student_school_Metric_Detail_Class_Absence_Chart()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Students/StudentNameFirst-M--Last-123456/Metrics/Metric-Name-1234/ClassAbsencesChart"
            Student.MetricsDrilldown(TestId.School, TestId.Student, TestId.MetricVariant, "ClassAbsencesChart", TestName.Student).ToVirtual()
                .ShouldMapTo<ClassAbsencesChartController>();
        }

        [Test]
        public virtual void Should_go_to_student_school_Metric_Detail_Discipline_Referral_List()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Students/StudentNameFirst-M--Last-123456/Metrics/Metric-Name-1234/DisciplineReferralList"
            Student.MetricsDrilldown(TestId.School, TestId.Student, TestId.MetricVariant, "DisciplineReferralList", TestName.Student).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<DisciplineReferralListRequest, IList<DisciplineReferralModel>>>();
        }

        [Test]
        public virtual void Should_go_to_student_school_Metric_Detail_Assessment_History()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Students/StudentNameFirst-M--Last-123456/AssessmentHistory"
            Student.MetricsDrilldown(TestId.School, TestId.Student, TestId.MetricVariant, "AssessmentHistory", TestName.Student).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<AssessmentHistoryRequest, AssessmentHistoryModel>>();
        }

        [Test]
        public virtual void Should_go_to_student_school_Metric_Detail_Benchmark_Historical_Chart()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Students/StudentNameFirst-M--Last-123456/BenchmarkHistoricalChart"
            Student.MetricsDrilldown(TestId.School, TestId.Student, TestId.MetricVariant, "BenchmarkHistoricalChart", TestName.Student).ToVirtual()
                .ShouldMapTo<BenchmarkHistoricalChartController>();
        }

        [Test]
        public virtual void Should_go_to_student_school_Metric_Detail_Metric_Objectives_List()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Students/StudentNameFirst-M--Last-123456/MetricObjectivesList"
            Student.MetricsDrilldown(TestId.School, TestId.Student, TestId.MetricVariant, "MetricObjectivesList", TestName.Student).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<MetricObjectivesListRequest, IList<MetricObjectiveModel>>>();
        }

        [Test]
        public virtual void Should_go_to_student_school_Metric_Detail_Learning_Standards_Table()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Students/StudentNameFirst-M--Last-123456/LearningStandardsTable"
            Student.MetricsDrilldown(TestId.School, TestId.Student, TestId.MetricVariant, "LearningStandardsTable", TestName.Student).ToVirtual()
                .ShouldMapTo<LearningStandardsTableController>();
        }

        [Test]
        public virtual void Should_go_to_student_school_Metric_Detail_Historical_Learning_Objective()
        {
           // "~/Districts/DistrictName/Schools/School-Name/Students/StudentNameFirst-M--Last-123456/HistoricalLearningObjectivesChart"
            Student.MetricsDrilldown(TestId.School, TestId.Student, TestId.MetricVariant, "HistoricalLearningObjectivesChart", TestName.Student).ToVirtual()
                .ShouldMapTo<HistoricalLearningObjectivesChartController>();
        }

        [Test]
        public virtual void Should_go_to_student_school_Metric_Detail_Learning_Objective()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Students/StudentNameFirst-M--Last-123456/LearningObjective"
            Student.MetricsDrilldown(TestId.School, TestId.Student, TestId.MetricVariant, "LearningObjective", TestName.Student).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<LearningObjectiveRequest, LearningObjectiveModel>>();
        }

        [Test]
        public virtual void Should_go_to_student_school_Metric_Detail_Current_Courses_List()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Students/StudentNameFirst-M--Last-123456/CurrentCoursesList"
            Student.MetricsDrilldown(TestId.School, TestId.Student, TestId.MetricVariant, "CurrentCoursesList", TestName.Student).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<CurrentCoursesListRequest, CurrentCoursesModel>>();
        }

        [Test]
        public virtual void Should_go_to_student_school_Metric_Detail_Subject_Course_History_List()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Students/StudentNameFirst-M--Last-123456/SubjectCourseHistoryList"
            Student.MetricsDrilldown(TestId.School, TestId.Student, TestId.MetricVariant, "SubjectCourseHistoryList", TestName.Student).ToVirtual()
                .ShouldMapTo<SubjectCourseHistoryListController>();
        }

        [Test]
        public virtual void Should_go_to_student_school_Metric_Detail_Credit_Accumulation_Chart()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Students/StudentNameFirst-M--Last-123456/CreditAccumulationChart"
            Student.MetricsDrilldown(TestId.School, TestId.Student, TestId.MetricVariant, "CreditAccumulationChart", TestName.Student).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<CreditAccumulationChartRequest, ChartData>>();
        }

        [Test]
        public virtual void Should_go_to_student_school_Metric_Detail_Academic_Challenge_Historical_List()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Students/StudentNameFirst-M--Last-123456/AcademicChallengeHistoricalList"
            Student.MetricsDrilldown(TestId.School, TestId.Student, TestId.MetricVariant, "AcademicChallengeHistoricalList", TestName.Student).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<AcademicChallengeHistoricalListRequest, List<Assessment>>>();
        }

        [Test]
        public virtual void Should_go_to_student_school_Metric_Detail_College_Readiness_Assessment_List()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Students/StudentNameFirst-M--Last-123456/CollegeReadinessAssessmentList"
            Student.MetricsDrilldown(TestId.School, TestId.Student, TestId.MetricVariant, "CollegeReadinessAssessmentList", TestName.Student).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<CollegeReadinessAssessmentListRequest, IEnumerable<CollegeCareerReadinessAssessmentModel>>>();
        }
    }
}
