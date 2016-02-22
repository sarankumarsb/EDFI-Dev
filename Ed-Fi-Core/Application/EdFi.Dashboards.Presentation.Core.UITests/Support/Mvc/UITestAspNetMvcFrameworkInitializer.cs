using System;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Presentation.Core.Tests.Routing.Support;
using EdFi.Dashboards.Presentation.Core.Tests.Routing.Support.Mvc;
using EdFi.Dashboards.Resources.Navigation;

namespace EdFi.Dashboards.Presentation.Core.UITests.Support.Mvc
{
    public class UITestAspNetMvcFrameworkInitializer : TestAspNetMvcFrameworkInitializer
    {
        public UITestAspNetMvcFrameworkInitializer(Type[] markerInterfacesForWebAssemblies, string baseUrl)
            : base(markerInterfacesForWebAssemblies, baseUrl) { }

        protected override void InitializeRouteGenerationForTesting(string baseUrl)
        {
            RouteTestingExtensions.BaseUrl = baseUrl;

            var baseUri = new Uri(baseUrl);

            // Initialize link generators
            var routeValuesPreparer = new RouteValuesPreparer(new UITestRouteValueProvider().ToArray());
            var httpRequestProviderFake = new HttpRequestProviderFake(baseUri.Scheme, baseUri.Host, baseUri.LocalPath);

            Website.InitializeLinkGenerators(routeValuesPreparer, httpRequestProviderFake);
            //InitializeLinkGenerators(routeValuesPreparer, httpRequestProviderFake);
        }

    }
}
