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
    public interface IHttpServerProvider
    {
        string MapPath(string path);
    }

    public class HttpServerProvider : IHttpServerProvider
    {
        public string MapPath(string path)
        {
            return HttpContext.Current.Server.MapPath(path);
        }
    }
}
