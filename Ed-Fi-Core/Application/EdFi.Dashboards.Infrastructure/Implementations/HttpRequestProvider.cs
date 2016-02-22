// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Web;
using System.Web.Routing;

namespace EdFi.Dashboards.Infrastructure.Implementations
{
    public class HttpRequestProvider : IHttpRequestProvider
    {
        public string GetValue(string name)
        {
            HttpRequest request = GetRequest();
            if (request == null)
                return null;
            return request[name];
        }

        public string this[string name]
        {
            get { return GetValue(name); }
        }

        public System.Uri Url
        {
            get
            {
                HttpRequest request = GetRequest();
                if (request == null)
                    return null;
                return request.Url;
            }
        }

        public System.Uri UrlReferrer
        {
            get
            {
                HttpRequest request = GetRequest();
                if (request == null)
                    return null;
                return request.UrlReferrer;
            }
        }

        public RequestContext RequestContext
        {
            get 
            { 
                var request = GetRequest();

                if (request == null)
                    return null;

                return request.RequestContext;
            }
        }

        private static HttpRequest GetRequest()
        {
            if (HttpContext.Current == null)
                return null;
            return HttpContext.Current.Request;
        }
    }
}
