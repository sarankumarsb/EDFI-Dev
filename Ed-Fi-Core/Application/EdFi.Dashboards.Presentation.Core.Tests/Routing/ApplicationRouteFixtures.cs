using EdFi.Dashboards.Presentation.Architecture.Mvc.Controllers;
using EdFi.Dashboards.Presentation.Core.Tests.Routing.Support;
using EdFi.Dashboards.Resources.Application;
using EdFi.Dashboards.Resources.Models.Application;
using EdFi.Dashboards.Resources.Navigation;
using NUnit.Framework;
using ApplicationLinks = EdFi.Dashboards.Resources.Navigation.Mvc.Areas.Application;

namespace EdFi.Dashboards.Presentation.Core.Tests.Routing
{
    public partial class When_resolving_routes_to_controllers
    {
        protected ApplicationLinks Application;

        protected virtual void InitializeApplicationLinkGenerator(RouteValuesPreparer routeValuesPreparer, HttpRequestProviderFake httpRequestProviderFake)
        {
            Application = new ApplicationLinks();
            Application.RouteValuesPreparer = routeValuesPreparer;
            Application.HttpRequestProvider = httpRequestProviderFake;
        }

        [Test]
        public virtual void Should_go_to_feedback()
        {
            //"~/Districts/DistrictName/Application/Feedback"
            Application.Feedback(TestId.LocalEducationAgency).ToVirtual()
                .ShouldMapTo<PostHandlerPassthroughController<FeedbackRequest, FeedbackModel>>("POST");
        }
    }
}
