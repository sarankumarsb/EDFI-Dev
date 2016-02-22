// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace EdFi.Dashboards.Infrastructure
{
    public interface ICurrentUserProvider
    {
        IPrincipal User { get; set; }
    }
}
