using System.Collections.Generic;
using Microsoft.IdentityModel.Claims;

namespace EdFi.Dashboards.SecurityTokenService.Authentication
{
    public interface IGetImpersonatedClaimsDataProvider
    {
        UserClaimsData GetImpersonatedClaimsData(IEnumerable<Claim> impersonatorClaims);
        bool IsImpersonating();
    }
}