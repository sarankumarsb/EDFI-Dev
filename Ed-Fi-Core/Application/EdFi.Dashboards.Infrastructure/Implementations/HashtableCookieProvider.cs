// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace EdFi.Dashboards.Infrastructure.Implementations
{
    public class HashtableCookieProvider : ICookieProvider
    {
        private readonly HttpCookieCollection cookies = new HttpCookieCollection();

        public void Add(HttpCookie cookie)
        {
            cookies.Add(cookie);
        }

        public HttpCookie Retrieve(string cookieName)
        {
            return cookies[cookieName];
        }
    }
}
