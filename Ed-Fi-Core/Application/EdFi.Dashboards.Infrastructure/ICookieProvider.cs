// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace EdFi.Dashboards.Infrastructure
{
    public interface ICookieProvider
    {
        void Add(HttpCookie cookie);
        HttpCookie Retrieve(string cookieName);
    }
}
