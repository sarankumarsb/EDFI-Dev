using EdFi.Dashboards.Presentation.Core.Areas.Common.Controllers;
using EdFi.Dashboards.Presentation.Core.Tests.Routing.Support;
using EdFi.Dashboards.Resources.Navigation;
using NUnit.Framework;
using CommonLinks = EdFi.Dashboards.Resources.Navigation.Mvc.Areas.Common;

namespace EdFi.Dashboards.Presentation.Core.Tests.Routing
{
    public partial class When_resolving_routes_to_controllers
    {
        protected CommonLinks Common;

        protected virtual void InitializeCommonLinkGenerator(RouteValuesPreparer routeValuesPreparer, HttpRequestProviderFake httpRequestProviderFake)
        {
            Common = new CommonLinks();
            //Common.RouteValuesPreparer = routeValuesPreparer;
            //Common.HttpRequestProvider = httpRequestProviderFake;
        }

        [Test]
        public virtual void Should_go_to_Common_ListSort_Context()
        {
            // TODO: There is no method for this route... is it being hardcoded in the application?
            "~/Common/ListSortContext"
                .ShouldMapTo<ListSortContextController>(); // Not validating action arguments since it's a JSON string
        }
    }
}
