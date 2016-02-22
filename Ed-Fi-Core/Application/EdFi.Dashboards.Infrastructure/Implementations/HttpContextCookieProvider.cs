// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Web;

namespace EdFi.Dashboards.Infrastructure.Implementations
{
    public class HttpContextCookieProvider : ICookieProvider
    {
        public void Add(HttpCookie cookie)
        {
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public HttpCookie Retrieve(string cookieName)
        {
            return HttpContext.Current.Request.Cookies[cookieName];
        }
    }
}
