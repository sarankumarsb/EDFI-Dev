using EdFi.Dashboards.Presentation.Core.Areas.Error.Controllers;
using EdFi.Dashboards.Presentation.Core.Tests.Routing.Support;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Mvc.Areas;
using NUnit.Framework;

namespace EdFi.Dashboards.Presentation.Core.Tests.Routing
{
    public partial class When_resolving_routes_to_controllers
    {
        protected Error Error;

        protected virtual void InitializeErrorLinkGenerator(RouteValuesPreparer routeValuesPreparer, HttpRequestProviderFake httpRequestProviderFake)
        {
            Error = new Error();
            Error.RouteValuesPreparer = routeValuesPreparer;
            Error.HttpRequestProvider = httpRequestProviderFake;
        }

        [Test]
        public virtual void Should_go_to_Error()
        {
            //"~/Error/Error"
            Error.ErrorPage(null).ToVirtual()
                .ShouldMapTo<ErrorController>
                (c => c.Get());
        }

        [Test]
        public virtual void Should_go_to_LEA_Error()
        {
            //"~/Districts/DistrictName/Error/Error"
            Error.ErrorPage(TestName.LocalEducationAgency).ToVirtual()
                .ShouldMapTo<ErrorController>
                (c => c.Get());
        }

        [Test]
        public virtual void Should_go_to_Error_Not_Found()
        {
            //"~/Districts/DistrictName/Error/NotFound/"
            Error.NotFound(TestName.LocalEducationAgency).ToVirtual()
                .ShouldMapTo<NotFoundController>
                (c => c.Get(TestId.LocalEducationAgency));
        }
    }
}
