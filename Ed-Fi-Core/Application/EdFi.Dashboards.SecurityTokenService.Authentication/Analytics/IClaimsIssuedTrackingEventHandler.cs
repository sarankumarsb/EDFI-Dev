using System.Collections.Generic;
using Microsoft.IdentityModel.Claims;

namespace EdFi.Dashboards.SecurityTokenService.Authentication.Analytics
{
    public interface IClaimsIssuedTrackingEventHandler
    {
        void Track(string username, long staffUSI, bool isImpersonating, IEnumerable<Claim> claims);
    }
}
