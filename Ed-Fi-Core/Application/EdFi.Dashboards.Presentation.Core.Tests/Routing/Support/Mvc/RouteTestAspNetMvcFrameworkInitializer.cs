using System;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resources.Navigation;

namespace EdFi.Dashboards.Presentation.Core.Tests.Routing.Support.Mvc
{
    public class RouteTestAspNetMvcFrameworkInitializer : TestAspNetMvcFrameworkInitializer
    {
        public RouteTestAspNetMvcFrameworkInitializer(Type[] markerInterfacesForWebAssemblies, string baseUrl) 
            : base(markerInterfacesForWebAssemblies, baseUrl) { }

        protected override void InitializeRouteGenerationForTesting(string baseUrl)
        {
            RouteTestingExtensions.BaseUrl = baseUrl;

            var baseUri = new Uri(baseUrl);

            // Initialize link generators
            var routeValuesPreparer = new RouteValuesPreparer(new TestRouteValueProvider().ToArray());
            var httpRequestProviderFake = new HttpRequestProviderFake(baseUri.Scheme, baseUri.Host, baseUri.LocalPath);

            Website.InitializeLinkGenerators(routeValuesPreparer, httpRequestProviderFake);
            //InitializeLinkGenerators(routeValuesPreparer, httpRequestProviderFake);
        }

    }
}