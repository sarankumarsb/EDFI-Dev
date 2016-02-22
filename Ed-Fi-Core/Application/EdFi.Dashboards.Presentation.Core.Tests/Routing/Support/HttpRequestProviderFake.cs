using System;
using System.Web;
using System.Web.Routing;
using EdFi.Dashboards.Infrastructure;

namespace EdFi.Dashboards.Presentation.Core.Tests.Routing.Support
{
    public class HttpRequestProviderFake : IHttpRequestProvider
    {
        private HttpContextBase httpContext;

        public HttpRequestProviderFake()
        {
            //httpContext = MvcMockHelpers.FakeHttpContext("https://localhost/EdFiDashboardDev/");
            httpContext = MvcMockHelpers.FakeHttpContext("~/", "/EdFiDashboardDev/");
        }

        public HttpRequestProviderFake(string protocol, string serverAddress, string appPath)
        {
            httpContext = MvcMockHelpers.FakeHttpContext("~/", appPath, serverAddress, protocol);
        }

        public string GetValue(string name)
        {
            throw new NotImplementedException();
        }

        public string this[string name]
        {
            get { throw new NotImplementedException(); }
        }

        public Uri Url
        {
            get { return httpContext.Request.Url; }
        }

        public Uri UrlReferrer
        {
            get { throw new NotImplementedException(); }
        }

        public RequestContext RequestContext
        {
            get
            {
                var requestContext = new RequestContext(httpContext, new RouteData());

                object x;
                bool result = requestContext.RouteData.Values.TryGetValue("abcd", out x);

                return requestContext;
            }
        }
    }
}
