using System;
using System.Web;
using System.Web.Routing;

namespace EdFi.Dashboards.Common.Utilities
{
    public class RouteInfo
    {
        public RouteInfo(RouteData data)
        {
            RouteData = data;
        }

        public RouteInfo(Uri uri, string applicationPath)
        {
            RouteData = RouteTable.Routes.GetRouteData(new InternalHttpContext(uri, applicationPath));
        }

        public RouteData RouteData { get; private set; }

        private class InternalHttpContext : HttpContextBase
        {
            private readonly HttpRequestBase request;

            public InternalHttpContext(Uri uri, string applicationPath)
            {
                request = new InternalRequestContext(uri, applicationPath);
            }

            public override HttpRequestBase Request { get { return request; } }
        }

        private class InternalRequestContext : HttpRequestBase
        {
            private readonly string appRelativePath;
            private readonly string pathInfo;

            public InternalRequestContext(Uri uri, string applicationPath)
            {
                pathInfo = uri.Query;

                if (!String.IsNullOrEmpty(applicationPath) && !applicationPath.Equals("/") && uri.AbsolutePath.StartsWith(applicationPath, StringComparison.OrdinalIgnoreCase))
                {
                    appRelativePath = uri.AbsolutePath.Substring(applicationPath.Length);
                }
                else
                {
                    appRelativePath = uri.AbsolutePath;
                }
            }

            public override string AppRelativeCurrentExecutionFilePath { get { return String.Concat("~", appRelativePath); } }
            public override string PathInfo { get { return pathInfo; } }
        }
    }
}
