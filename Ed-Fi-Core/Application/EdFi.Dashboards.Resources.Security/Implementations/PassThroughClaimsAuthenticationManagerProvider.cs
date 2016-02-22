using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
    /// <summary>
    /// Use this implementation when using EdFi.Dashboards.SecurityTokenService.Web as the STS.
    /// Since our STS attaches the claims to the token it sends, we really just need to pass the incoming principal back to the caller.
    /// Be sure to turn on DashboardClaimsGetOutputClaimsIdentityProvider for IGetOutputClaimsIdentityProvider
    /// </summary>
    /// <remarks>
    /// You must remove CustomClaimsAuthenticationManager from the web.config when using DashboardClaimsGetOutputClaimsIdentityProvider and PassThroughClaimsAuthenticationManagerProvider
    /// </remarks>
    public class PassThroughClaimsAuthenticationManagerProvider : IClaimsAuthenticationManagerProvider
    {
        public Microsoft.IdentityModel.Claims.IClaimsPrincipal Get(string resourceName, Microsoft.IdentityModel.Claims.IClaimsPrincipal incomingPrincipal)
        {
            return incomingPrincipal;
        }
    }
}
