using System;
using System.Collections.Generic;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using Microsoft.IdentityModel.Claims;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
    public class DashboardDatabaseWithStateAgencySupportUserClaimsProvider : DashboardUserClaimsProvider<ClaimsSet, EdFiUserSecurityDetails>
    {
        public DashboardDatabaseWithStateAgencySupportUserClaimsProvider(IDashboardUserClaimsInformationProvider<EdFiUserSecurityDetails> dashboardUserClaimsInformationProvider, IUserClaimSetsProvider<ClaimsSet, EdFiUserSecurityDetails> userClaimSetsProvider, IClaimSetBasedClaimsProvider<ClaimsSet> claimSetBasedClaimsProvider, IAuthenticationProvider authenticationProvider) 
            : base(dashboardUserClaimsInformationProvider, userClaimSetsProvider, claimSetBasedClaimsProvider, authenticationProvider)
        {
        }

        public override IEnumerable<Claim> GetApplicationSpecificClaims(string username, long staffUSI)
        {
            if (string.IsNullOrWhiteSpace(username) || !username.Equals("AceOfSpades", StringComparison.OrdinalIgnoreCase))
                return base.GetApplicationSpecificClaims(username, staffUSI);

            // Initialize "Name" claims
            var claims = new List<Claim>
                             {
                                 new Claim(EdFiClaimTypes.FullName, "Ace Of Spades"),
                                 new Claim(ClaimTypes.GivenName, "Ace"),
                                 new Claim(ClaimTypes.Surname, "Spades"),
                                 new Claim(EdFiClaimTypes.StaffUSI, staffUSI.ToString())
                                 //new Claim(ClaimTypes.Email, email)
                             };

            var edOrgId = new EducationOrganizationIdentifier
                          {
                              StateAgencyId = int.MaxValue
                          };

            // Claims derived from the role
            var claimSetBasedClaims = new List<Claim>
            {
                ClaimHelper.CreateClaim(EdFiClaimTypes.AccessOrganization, edOrgId),
                ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllMetrics, edOrgId),
                ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllStudents, edOrgId),
                ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllTeachers, edOrgId),
                ClaimHelper.CreateClaim(EdFiClaimTypes.ViewOperationalDashboard, edOrgId),
                ClaimHelper.CreateClaim(EdFiClaimTypes.ManageGoals, edOrgId),
                ClaimHelper.CreateClaim(EdFiClaimTypes.AdministerDashboard, edOrgId)
            };
            
            claims.AddRange(claimSetBasedClaims);

            return claims;
        }
    }
}