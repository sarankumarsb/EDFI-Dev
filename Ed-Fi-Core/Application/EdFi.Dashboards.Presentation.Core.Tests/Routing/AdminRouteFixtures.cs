using EdFi.Dashboards.Application.Resources.Admin;
using EdFi.Dashboards.Application.Resources.Models.Admin;
using EdFi.Dashboards.Presentation.Architecture.Mvc.Controllers;
using EdFi.Dashboards.Presentation.Core.Areas.Admin.Controllers;
using EdFi.Dashboards.Presentation.Core.Tests.Routing.Support;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Mvc.Areas;
using NUnit.Framework;

namespace EdFi.Dashboards.Presentation.Core.Tests.Routing
{
    public partial class When_resolving_routes_to_controllers
    {
        protected Admin Admin;

        protected virtual void InitializeAdminLinkGenerator(RouteValuesPreparer routeValuesPreparer, HttpRequestProviderFake httpRequestProviderFake)
        {
            Admin = new Admin();
            Admin.RouteValuesPreparer = routeValuesPreparer;
            Admin.HttpRequestProvider = httpRequestProviderFake;
        }

        [Test]
        public virtual void Should_go_to_LEA_Admin_Default()
        {
            //"~/Districts/DistrictName/Admin/Configuration"
            Admin.Default(TestId.LocalEducationAgency).ToVirtual()
                .ShouldMapTo<ConfigurationController>
                (c => c.Get(TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_LEA_Admin_Login_As()
        {
            //"~/Districts/DistrictName/Admin/LogInAs?staffUSI=1234"
            Admin.LogInAs(TestId.LocalEducationAgency, new { staffUSI = 1234}).ToVirtual()
                .ShouldMapTo<LogInAsController>
                (c => c.Get(TestId.LocalEducationAgency, 1234));
        }

        [Test]
        public virtual void Should_go_to_LEA_Admin_Title_Claim_Set()
        {
            Admin.TitleClaimSet(TestId.LocalEducationAgency).ToVirtual()
                .ShouldMapTo<TitleClaimSetController>(c => c.Get(TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_LEA_Admin_Export_Title_Claim_Set()
        {
            Admin.ExportTitleClaimSet(TestId.LocalEducationAgency).ToVirtual()
                .ShouldMapTo<ServicePassthroughController<ExportTitleClaimSetRequest, ExportTitleClaimSetModel>>(
                c => c.Get(ExportTitleClaimSetRequest.Create(TestId.LocalEducationAgency), TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_LEA_Admin_Metric_Threshold()
        {
            Admin.MetricThreshold(TestId.LocalEducationAgency).ToVirtual()
                .ShouldMapTo<MetricThresholdController>(
                c => c.Get(TestId.LocalEducationAgency, string.Empty, null), "GET", true);
        }

        [Test]
        public virtual void Should_go_to_LEA_Admin_Photo_Management()
        {
            Admin.PhotoManagement(TestId.LocalEducationAgency).ToVirtual()
                .ShouldMapTo<PhotoManagementController>(
                    c => c.Get(TestId.LocalEducationAgency));
        }

        [Test]
        public virtual void Should_go_to_LEA_Admin_Show_Claims()
        {
            "~/Districts/DistrictName/Admin/ShowClaims"
                .ShouldMapTo<ShowClaimsController>
                (c => c.Get());
        }

        [Test]
        public virtual void Should_go_to_Admin_Show_Claims()
        {
            "~/Admin/ShowClaims"
                .ShouldMapTo<ShowClaimsController>
                (c => c.Get());
        }

        [Test]
        public virtual void Should_go_to_LEA_Admin_Metrics_threshold()
        {
            "~/Districts/DistrictName/Admin/MetricThreshold/Delete"
                .ShouldMapTo<MetricThresholdController>("POST");
            // Can't test Delete action method due to method's return value of "bool", rather than "ActionResult"
        }
    }
}
