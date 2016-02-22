using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace EdFi.Dashboards.SecurityTokenService.Authentication.Implementations
{
    public class GetImpersonatedClaimsDataProvider : IGetImpersonatedClaimsDataProvider
    {
        private IWimpProvider wimpProvider;
        private IStaffInformationProvider staffInformationProvider;
        private IAuthenticationProvider authenticationProvider;
        private IUserClaimsProvider userClaimsProvider;

        public GetImpersonatedClaimsDataProvider(IWimpProvider wimpProvider, IStaffInformationProvider staffInformationProvider,
            IAuthenticationProvider authenticationProvider, IUserClaimsProvider userClaimsProvider)
        {
            this.wimpProvider = wimpProvider;
            this.staffInformationProvider = staffInformationProvider;
            this.authenticationProvider = authenticationProvider;
            this.userClaimsProvider = userClaimsProvider;
        }

        public UserClaimsData GetImpersonatedClaimsData(IEnumerable<Claim> impersonatorClaims)
        {
            if (impersonatorClaims == null)
                throw new ArgumentNullException();

            //var emailAddressToImpersonate = GetEmailAddressToImpersonate();
            var staffUSIToImpersonate = GetStaffUSIToImpersonate();

            if( staffUSIToImpersonate == null)
                throw new InvalidOperationException("Unable to determine staff USI while attempting to impersonate.");

            // Make sure the user has the ability to impersonate
            var administerDashboardClaims =
                from c in impersonatorClaims
                where c.ClaimType == EdFiClaimTypes.AdministerDashboard
                select c;

            if (!administerDashboardClaims.Any())
				throw new InvalidOperationException("You don't have permissions to impersonate.  This action has been logged.");

            string targetedUsername = staffInformationProvider.ResolveUsername(authenticationProvider, staffUSIToImpersonate, 0); // VIN22012016

            if (targetedUsername == null)
				throw new InvalidOperationException(String.Format("Impersonation request could not be satisfied because the user associated with staff USI '{0}' could not be found.", staffUSIToImpersonate));

            // Get the target user's claims

            IEnumerable<Claim> targetedUsersClaims;
            try
            {
                targetedUsersClaims = userClaimsProvider.GetApplicationSpecificClaims(targetedUsername, Convert.ToInt32(staffUSIToImpersonate));
            }
            catch (DashboardsAuthenticationException dae)
            {
                dae.IsImpersonating = true;
                throw dae;
            }

            // Add the impersonation claim
            var impersonationClaim = new Claim(EdFiClaimTypes.Impersonating, staffUSIToImpersonate);
            targetedUsersClaims = targetedUsersClaims.Concat(new[] { impersonationClaim });

            var adminsterStateId = from c in administerDashboardClaims
                                   let xmlNode = JsonConvert.DeserializeXmlNode(c.Value, "root")
                                   let stateNode = xmlNode.SelectSingleNode("//" + EdFiClaimProperties.StateAgencyId)
                                   where stateNode != null && !string.IsNullOrWhiteSpace(stateNode.InnerText)
                                   select Convert.ToInt32(stateNode.InnerText);

            // Get the LEA ids on which the user has the AdministerDashboard claims
            var administerLEAIds =
                from c in administerDashboardClaims
                let xmlNode = JsonConvert.DeserializeXmlNode(c.Value, "root")
                let leaNode = xmlNode.SelectSingleNode("//" + EdFiClaimProperties.LocalEducationAgencyId)
                where leaNode != null && !string.IsNullOrWhiteSpace(leaNode.InnerText)
                select Convert.ToInt32(leaNode.InnerText);

            // Get the LEA ids the impersonation target is associated with
            var targetedUserLEAIds =
                from c in targetedUsersClaims
                where c.ClaimType.StartsWith(EdFiClaimTypes._OrgClaimNamespace)
                let xmlNode = JsonConvert.DeserializeXmlNode(c.Value, "root")
                let leaNode = xmlNode.SelectSingleNode("//" + EdFiClaimProperties.LocalEducationAgencyId)
                where leaNode != null
                select Convert.ToInt32(leaNode.InnerText);

            // Is there any overlap between the LEA Ids?  There must be in order to proceed. Or they must be a state user.
            if (adminsterStateId.Any() || administerLEAIds.Intersect(targetedUserLEAIds).Any())
            {
                return new UserClaimsData
                {
                    Username = targetedUsername,
                    //Email = emailAddressToImpersonate,
                    StaffUSI = Convert.ToInt32(staffUSIToImpersonate),
                    Claims = targetedUsersClaims,
                };
            }

			throw new InvalidOperationException(String.Format("You do not have permissions to impersonate this user (user name: '{0}' staff USI: '{1}') because they are associated with a different local education agency.", targetedUsername, staffUSIToImpersonate));
        }

        public bool IsImpersonating()
        {
            //return GetEmailAddressToImpersonate() != null;
            return GetStaffUSIToImpersonate() != null;
        }

        private string GetStaffUSIToImpersonate()
        {
            return wimpProvider.GetWimp();
        }
    }
}