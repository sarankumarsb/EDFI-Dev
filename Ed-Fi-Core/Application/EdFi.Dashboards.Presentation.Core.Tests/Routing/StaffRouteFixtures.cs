using System.Collections.Generic;
using EdFi.Dashboards.Application.Resources.Models.Staff;
using EdFi.Dashboards.Application.Resources.Staff;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Presentation.Architecture.Mvc.Controllers;
using EdFi.Dashboards.Presentation.Core.Areas.Staff.Controllers;
using EdFi.Dashboards.Presentation.Core.Tests.Routing.Support;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Mvc.Areas;
using EdFi.Dashboards.Resources.Staff;
using NUnit.Framework;
using ExportAllMetricsRequest = EdFi.Dashboards.Resources.Staff.ExportAllMetricsRequest;

namespace EdFi.Dashboards.Presentation.Core.Tests.Routing
{
    public partial class When_resolving_routes_to_controllers
    {
        protected Staff Staff;
        private readonly FakeClassroomViewProvider fakeClassroomViewProvider = new FakeClassroomViewProvider();

        protected virtual void InitializeStaffLinkGenerator(RouteValuesPreparer routeValuesPreparer, HttpRequestProviderFake httpRequestProviderFake)
        {
            Staff = new Staff(fakeClassroomViewProvider, new StubCodeIdProvider(), new StubLocalEducationAgencyContextProvider(), null)
                {
                    RouteValuesPreparer = routeValuesPreparer,
                    HttpRequestProvider = httpRequestProviderFake
                };
        }

        class FakeClassroomViewProvider : IClassroomViewProvider
        {
            public string ViewTypeToSupply { get; set; }

            public string GetDefaultClassroomView(int leaId)
            {
                return ViewTypeToSupply;
            }
        }

        class StubLocalEducationAgencyContextProvider : ILocalEducationAgencyContextProvider
        {
            public string GetCurrentLocalEducationAgencyCode() { return null; }
        }

        class StubCodeIdProvider : ICodeIdProvider
        {
            public int Get(string code) { return TestId.LocalEducationAgency; }
        }

        [Test]
        public virtual void Default_should_go_to_AssessmentDetailsController_when_assessment_details_is_the_default_view_for_the_LEA()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Staff/StaffFirst-LastName-1234"

            // Define the value to be returned as the default view on the next call
            fakeClassroomViewProvider.ViewTypeToSupply = StaffModel.ViewType.AssessmentDetails.ToString();

            Staff.Default(TestId.School, TestId.Staff, TestName.Staff, 1234, "TestListType").ToVirtual()
                .ShouldMapTo<AssessmentDetailsController>(c => c.Get(TestId.Staff, TestId.School, "TestListType", 1234, TestId.LocalEducationAgency, null, null),
                    allowNulls : true);
        }

        [Test]
        public virtual void Default_should_go_to_PriorYearController_when_prior_year_is_the_default_view_for_the_LEA()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Staff/StaffFirst-LastName-1234"

            // Define the value to be returned as the default view on the next call
            fakeClassroomViewProvider.ViewTypeToSupply = StaffModel.ViewType.PriorYear.ToString();

            Staff.Default(TestId.School, TestId.Staff, TestName.Staff, 1234, "TestListType").ToVirtual()
                .ShouldMapTo<PriorYearController>(c => c.Get(TestId.Staff, TestId.School, "TestListType", 1234, TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Default_should_go_to_the_GeneralOverviewController_for_other_view_types()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Staff/StaffFirst-LastName-1234"

            // Define the value to be returned as the default view on the next call
            fakeClassroomViewProvider.ViewTypeToSupply = "abcd";

            Staff.Default(TestId.School, TestId.Staff, TestName.Staff, 1234, "TestListType").ToVirtual()
                .ShouldMapTo<GeneralOverviewController>(c => c.Get(GeneralOverviewMetaRequest.Create(TestId.LocalEducationAgency, TestId.School, TestId.Staff, "TestListType", 1234)));
        }

        [Test]
        public virtual void Should_go_to_staff_General_Overview()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Staff/StaffFirst-LastName-1234/GeneralOverview/StateStandardized/Section/1234"
            
            Staff.GeneralOverview(TestId.School, TestId.Staff, null, 1234, "TestListType").ToVirtual()
                .ShouldMapTo<GeneralOverviewController>(c => c.Get(GeneralOverviewMetaRequest.Create(TestId.LocalEducationAgency, TestId.School, TestId.Staff, "TestListType", 1234 )));
        }

        [Test]
        public virtual void Should_go_to_staff_Subject_Specific()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Staff/StaffFirst-LastName-1234/SubjectSpecificOverview/StateStandardized/Section/1234"
            
            Staff.SubjectSpecificOverview(TestId.School, TestId.Staff, null, 1234, "TestListType").ToVirtual()
                .ShouldMapTo<SubjectSpecificOverviewController>(c => c.Get(SubjectSpecificOverviewRequest.Create(TestId.School, TestId.Staff, "TestListType", 1234)));
        }

        [Test]
        public virtual void Should_go_to_staff_Benchmark_Assessment_Detail()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Staff/StaffFirst-LastName-1234/AssessmentDetails/Benchmark/Section/1234"

            Staff.AssessmentDetails(TestId.School, TestId.Staff, null, 1234, "TestListType", "Math").ToVirtual()
                .ShouldMapTo<AssessmentDetailsController>(c => c.Get(TestId.Staff, TestId.School, "TestListType", 1234, TestId.LocalEducationAgency, "Math", "General"));
        }

        [Test]
        public virtual void Should_go_to_staff_Export_All_Metrics()
        {
            Staff.ExportAllMetrics(TestId.School, TestId.Staff, null, 7, "All").ToVirtual()
                .ShouldMapTo<ServicePassthroughController<ExportAllMetricsRequest, StudentExportAllModel>>
                (c => c.Get(ExportAllMetricsRequest.Create(TestId.Staff, TestId.School, "All", 7, null, null), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_staff_Custom_Student_List()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Staff/StaffFirst-LastName-1234/CustomStudentList"

            Staff.CustomStudentList(TestId.School, TestId.Staff, TestName.Staff).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<CustomStudentListGetRequest, IEnumerable<CustomStudentListModel>>>
                (c => c.Get(CustomStudentListGetRequest.Create(TestId.LocalEducationAgency, TestId.School, TestId.Staff), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_staff_Assessment_Sub_Type()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Staff/StaffFirst-LastName-1234/CustomStudentList"

            //Staff.LocalEducationAgencyCustomStudentList(TestId.LocalEducationAgency, TestId.Staff, TestName.Staff, TestId.SectionOrCohort, "test", "maths", "Benchmark").ToVirtual()
            Staff.LocalEducationAgencyCustomStudentList(TestId.LocalEducationAgency, TestId.Staff, TestName.Staff).ToVirtual()
                 .ShouldMapTo<ServicePassthroughController<CustomStudentListGetRequest, IEnumerable<CustomStudentListModel>>>
                    (c => c.Get(CustomStudentListGetRequest.Create(TestId.LocalEducationAgency, null, TestId.Staff), TestId.LocalEducationAgency));
        }
    }
}
