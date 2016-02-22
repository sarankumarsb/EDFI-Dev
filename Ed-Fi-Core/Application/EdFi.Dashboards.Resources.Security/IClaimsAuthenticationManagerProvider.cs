using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.IdentityModel.Claims;

namespace EdFi.Dashboards.Resources.Security
{
    public interface IClaimsAuthenticationManagerProvider
    {
        IClaimsPrincipal Get(string resourceName, IClaimsPrincipal incomingPrincipal);
    }
}
