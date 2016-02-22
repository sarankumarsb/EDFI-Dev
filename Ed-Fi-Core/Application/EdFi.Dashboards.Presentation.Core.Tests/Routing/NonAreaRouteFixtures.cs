using EdFi.Dashboards.Presentation.Core.Controllers;
using EdFi.Dashboards.Presentation.Core.Tests.Routing.Support;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Mvc;
using NUnit.Framework;

namespace EdFi.Dashboards.Presentation.Core.Tests.Routing
{
    public partial class When_resolving_routes_to_controllers
    {
        protected General General;

        protected virtual void InitializeGeneralLinkGenerator(RouteValuesPreparer routeValuesPreparer, HttpRequestProviderFake httpRequestProviderFake)
        {
            General = new General
            {
                RouteValuesPreparer = routeValuesPreparer,
                HttpRequestProvider = httpRequestProviderFake
            };
        }

        [Test]
        public virtual void Should_logout()
        {
            General.Logout().ToVirtual()
                .ShouldMapTo<LogoutController>
                (c => c.Get());
        }

        [Test]
        public virtual void Should_go_to_home()
        {
            "~/".ToVirtual()
                .ShouldMapTo<HomeController>
                (c => c.Get());
        }

        // TODO: Review this later
        //[Test]
        //public virtual void Should_go_to_Watch_List_Description_Controller()
        //{
        //    var request = new MetricsBasedWatchListDescriptionRequest
        //    {
        //        StaffUSI = TestId.Staff
        //    };

        //    General.MetricsBasedWatchList("MetricsBasedWatchListDescription").ToVirtual()
        //        .ShouldMapTo<MetricsBasedWatchListDescriptionController>(c => c.Get(request));
        //}

        //[Test]
        //public virtual void Should_go_to_Watch_List_Unshare_Controller()
        //{
        //    General.MetricsBasedWatchList("MetricsBasedWatchListUnshare").ToVirtual()
        //        .ShouldMapTo<MetricsBasedWatchListUnshareController>(c => c.Post(new MetricsBasedWatchListUnshareRequest
        //        {
        //            LocalEducationAgencyId = TestId.LocalEducationAgency,
        //            StaffUSI = TestId.Staff,
        //            MetricBasedWatchListId = 1
        //        }));
        //}

        [Test]
        public virtual void Should_go_to_staff_Custom_Metrics_Based_Watch_List()
        {
            //"~/Districts/DistrictName/Schools/School-Name/Staff/StaffFirst-LastName-1234/MetricsBasedWatchList"
            var request = new MetricsBasedWatchListGetRequest
            {
                Id = 1
            };

            General.MetricsBasedWatchList("MetricsBasedWatchList", 1).ToVirtual()
                .ShouldMapTo<MetricsBasedWatchListController>(c => c.Get(request));
        }
    }
}
