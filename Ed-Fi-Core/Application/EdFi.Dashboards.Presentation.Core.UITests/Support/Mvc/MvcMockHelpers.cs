using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;

namespace EdFi.Dashboards.Presentation.Core.UITests.Support.Mvc
{
    public static class MvcMockHelpers
    {
        public static HttpContextBase FakeHttpContext()
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();

            // UrlHelper invokes this method to support cookieless sessions
            response.Setup(resp => resp.ApplyAppPathModifier(It.IsAny<string>()))
                .Returns((string s) => s);

            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);
            context.SetupProperty(ctx => ctx.User);

            return context.Object;
        }

        public static HttpContextBase FakeHttpContext(string url)
        {
            var context = FakeHttpContext();
            context.Request.SetupRequestUrl(url);
            return context;
        }

        public static HttpContextBase FakeHttpContext(string url, string applicationPath)
        {
            var context = FakeHttpContext(url);
            context.Request.SetupRequestApplicationPath(applicationPath);
            return context;
        }

        public static void SetFakeControllerContext(this Controller controller, string requestUrl = "~/Nowhere/")
        {
            var httpContext = FakeHttpContext(requestUrl);
            var requestContext = new RequestContext(httpContext, new RouteData());
            var context = new ControllerContext(requestContext, controller);
            controller.ControllerContext = context;
            controller.Url = new UrlHelper(requestContext);
        }

        private static string GetUrlFileName(string url)
        {
            return url.Contains("?") ? url.Substring(0, url.IndexOf("?")) : url;
        }

        private static NameValueCollection GetQueryStringParameters(string url)
        {
            if (url.Contains("?"))
            {
                var parameters = new NameValueCollection();

                var parts = url.Split("?".ToCharArray());
                var keys = parts[1].Split("&".ToCharArray());

                foreach (var key in keys)
                {
                    var part = key.Split("=".ToCharArray());
                    parameters.Add(part[0], part[1]);
                }

                return parameters;
            }
            return null;
        }

        public static void SetHttpMethodResult(this HttpRequestBase request, string httpMethod)
        {
            Mock.Get(request)
                .Setup(req => req.HttpMethod)
                .Returns(httpMethod);
        }

        public static void SetupRequestApplicationPath(this HttpRequestBase request, string applicationPath)
        {
            var mock = Mock.Get(request);

            mock.Setup(req => req.ApplicationPath)
                .Returns(applicationPath);
        }

        public static void SetupRequestUrl(this HttpRequestBase request, string url)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            if (!url.StartsWith("~/"))
                throw new ArgumentException("Sorry, we expect a virtual url starting with \"~/\".");

            var mock = Mock.Get(request);

            mock.Setup(req => req.QueryString)
                .Returns(GetQueryStringParameters(url));
            mock.Setup(req => req.AppRelativeCurrentExecutionFilePath)
                .Returns(GetUrlFileName(url));
            mock.Setup(req => req.PathInfo)
                .Returns(string.Empty);
            mock.SetupGet(x => x.Url).Returns(new Uri(url.Replace("~/", "https://localhost/EdFiDashboardDev/")));
        }
    }
}
