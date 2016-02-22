// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace EdFi.Dashboards.Infrastructure.Implementations
{
    public class HttpContextUserProviderProvider : ICurrentUserProvider
    {
        public IPrincipal User
        {
            get { return HttpContext.Current.User; }
            set { HttpContext.Current.User = value; }
        }
    }
}
