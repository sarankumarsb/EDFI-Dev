using System.Text;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.SecurityTokenService.Authentication.Analytics;
using Microsoft.IdentityModel.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace EdFi.Dashboards.SecurityTokenService.Web.Providers
{
    /// <summary>
    /// Use this version if you want legacy mode where the STS not only authenticates but also does the database lookup
    /// and creates the application claims.  Remember if you turn this version on, you need to turn on 
    /// PassThroughClaimsAuthenticationManagerProvider for the IClaimsAuthenticationManagerProvider.
    /// </summary>
    public class DashboardClaimsGetOutputClaimsIdentityProvider : IGetOutputClaimsIdentityProvider
    {
        private IStaffInformationProvider staffInformationProvider;
        private IAuthenticationProvider authenticationProvider;
        private IUserClaimsProvider userClaimsProvider;
        private IClaimsIssuedTrackingEventProvider ClaimsIssuedTrackingEventProvider;
        private IGetImpersonatedClaimsDataProvider getImpersonatedClaimsDataProvider;

        public DashboardClaimsGetOutputClaimsIdentityProvider(IStaffInformationProvider staffInformationProvider, IAuthenticationProvider authenticationProvider,
            IUserClaimsProvider userClaimsProvider, IClaimsIssuedTrackingEventProvider claimsIssuedTrackingEventProvider, IGetImpersonatedClaimsDataProvider getImpersonatedClaimsDataProvider)
        {
            this.staffInformationProvider = staffInformationProvider;
            this.authenticationProvider = authenticationProvider;
            this.userClaimsProvider = userClaimsProvider;
            this.ClaimsIssuedTrackingEventProvider = claimsIssuedTrackingEventProvider;
            this.getImpersonatedClaimsDataProvider = getImpersonatedClaimsDataProvider;
        }

        public Microsoft.IdentityModel.Claims.IClaimsIdentity GetOutputClaimsIdentity(Microsoft.IdentityModel.Claims.IClaimsPrincipal principal, Microsoft.IdentityModel.Protocols.WSTrust.RequestSecurityToken request, Microsoft.IdentityModel.SecurityTokenService.Scope scope)
        {
            if (principal == null)
                throw new ArgumentNullException("principal");

            string username = principal.Identity.Name;

            var staffUsi = staffInformationProvider.ResolveStaffUSI(authenticationProvider, username);
            var appSpecificClaims = userClaimsProvider.GetApplicationSpecificClaims(username, staffUsi);

            // Determine if we have a "wimp" (WIF-impersonation) parameter
            var isImpersonating = getImpersonatedClaimsDataProvider.IsImpersonating();
            var userClaimsData = isImpersonating
                                    ? getImpersonatedClaimsDataProvider.GetImpersonatedClaimsData(appSpecificClaims) // Create claims data for impersonated user
                                    : new UserClaimsData { Username = username, Claims = appSpecificClaims }; // Create claims data for current user

            // Create the identity, and add all the claims
            var claimsIdentity = CreateClaimsIdentity(userClaimsData);
            this.ClaimsIssuedTrackingEventProvider.Track(username, staffUsi, isImpersonating, appSpecificClaims);
            return claimsIdentity;
        }

        private ClaimsIdentity CreateClaimsIdentity(UserClaimsData userClaimsData)
        {
            var claimsIdentity = new ClaimsIdentity();

            // Issue custom claims
            claimsIdentity.Claims.Add(new Claim(ClaimTypes.Name, userClaimsData.Username));
            claimsIdentity.Claims.AddRange(userClaimsData.Claims);

            return claimsIdentity;
        }
    }
}