// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using Microsoft.IdentityModel.Claims;
using System;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
	internal struct DashboardUserClaimsProviderConstants
	{
		internal const string NoUserInformationErrorMessage = "We were able to verify your password, but were unable to locate your user information which is needed to authorize access to the website.  If you feel this is incorrect, please contact an administrator.";
		internal const string NoClaimSetsErrorMessage = "We were able to verify your password, but you do not have authorization to access the website.  If you feel this is incorrect, please contact an administrator.";
	}

    public class DashboardUserClaimsProvider<TClaimsSet, TUserSecurityDetails> : IUserClaimsProvider where TUserSecurityDetails : IErrorLogOutput
    {
        private readonly IDashboardUserClaimsInformationProvider<TUserSecurityDetails> dashboardUserClaimsInformationProvider;
		protected IDashboardUserClaimsInformationProvider<TUserSecurityDetails> DashboardUserClaimsInformationProvider
		{
			get { return dashboardUserClaimsInformationProvider; }
		}

        private readonly IUserClaimSetsProvider<TClaimsSet, TUserSecurityDetails> userClaimSetsProvider;
		protected IUserClaimSetsProvider<TClaimsSet, TUserSecurityDetails> UserClaimSetsProvider
		{
			get { return userClaimSetsProvider; }
		}

        private readonly IClaimSetBasedClaimsProvider<TClaimsSet> claimSetBasedClaimsProvider;
		protected IClaimSetBasedClaimsProvider<TClaimsSet> ClaimSetBasedClaimsProvider
		{
			get { return claimSetBasedClaimsProvider; }
		}

        private readonly IAuthenticationProvider authenticationProvider;
		protected IAuthenticationProvider AuthenticationProvider
		{
			get { return authenticationProvider; }
		}
		
        public DashboardUserClaimsProvider(
            IDashboardUserClaimsInformationProvider<TUserSecurityDetails> dashboardUserClaimsInformationProvider,
            IUserClaimSetsProvider<TClaimsSet, TUserSecurityDetails> userClaimSetsProvider, 
            IClaimSetBasedClaimsProvider<TClaimsSet> claimSetBasedClaimsProvider,
            IAuthenticationProvider authenticationProvider)
        {
            this.dashboardUserClaimsInformationProvider = dashboardUserClaimsInformationProvider;
            this.userClaimSetsProvider = userClaimSetsProvider;
            this.claimSetBasedClaimsProvider = claimSetBasedClaimsProvider;
            this.authenticationProvider = authenticationProvider;
        }

        public virtual IEnumerable<Claim> GetApplicationSpecificClaims(string username, long staffUSI)
        {
            // Get information necessary to create the app-specific claims
            var userClaimsInfo = dashboardUserClaimsInformationProvider.GetClaimsInformation(username, staffUSI);

            // No user information found?
            if (userClaimsInfo == null)
            {
				throw new StaffSchoolClassAssociationException(DashboardUserClaimsProviderConstants.NoUserInformationErrorMessage) { Name = username, StaffUSI = staffUSI };
            }

            var claims = new List<Claim>();

            // Initialize "Name" claims
            claims.Add(new Claim(EdFiClaimTypes.FullName, userClaimsInfo.FullName));
            claims.Add(new Claim(ClaimTypes.GivenName, userClaimsInfo.FirstName));
            claims.Add(new Claim(ClaimTypes.Surname, userClaimsInfo.LastName));

            if (!string.IsNullOrEmpty(userClaimsInfo.Email))
                claims.Add(new Claim(ClaimTypes.Email, userClaimsInfo.Email));
            else
                throw new InvalidOperationException(string.Format("Missing required claim: {0}", ClaimTypes.Email));

            // Add the StaffUSI claim
            claims.Add(new Claim(EdFiClaimTypes.StaffUSI, staffUSI.ToString()));

            // Add the UserType claim - EDFIDB-139	            
            claims.Add(new Claim(EdFiClaimTypes.UserType, userClaimsInfo.UserType.ToString()));

            // Add the Site Login Type claim
            if (userClaimsInfo.ServiceType != null)
                claims.Add(new Claim(EdFiClaimTypes.ServiceType, System.Convert.ToString(userClaimsInfo.UserType))); // VINLOGINTYP

            var foundUserClaims = false;
            // Iterate through user's organizations
            foreach (var org in userClaimsInfo.AssociatedOrganizations)
            {
                // Resolve the user's relationship with the organization to an application role
                var claimsSets = userClaimSetsProvider.GetUserClaimSets(org.SecurityDetails);

                // Skip this organization if no claim sets can be determined
                if (!claimsSets.Any())
                    continue;

                foundUserClaims = true;

                // Add the claims for this org/claim sets
                foreach (var claimSet in claimsSets)
                {
                    // Claims derived from the role
                    var claimSetBasedClaims = claimSetBasedClaimsProvider.GetClaims(claimSet, org.Ids);
                    claims.AddRange(claimSetBasedClaims);
                }
            }

            if (!foundUserClaims)
            {
				throw new StaffPositionTitleAssociationException<TUserSecurityDetails>(DashboardUserClaimsProviderConstants.NoClaimSetsErrorMessage)
                            {
                                Name = username,
                                StaffUSI = staffUSI,
                                ClaimsInformation = userClaimsInfo
                            };
            }

            return claims;
        }
    }
}