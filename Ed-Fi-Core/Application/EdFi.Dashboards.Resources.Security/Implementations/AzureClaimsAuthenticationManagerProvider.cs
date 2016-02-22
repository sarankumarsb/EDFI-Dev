using System;
using System.Linq;
using System.Security.Authentication;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.SecurityTokenService.Authentication.Analytics;
using Microsoft.IdentityModel.Claims;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
    /// <summary>
    /// Use this implementation when the STS is not our EdFi.Dashboards.SecurityTokenService.Web.  
    /// Hosted STS like Azure Access Control or Azure AD do not know about our application specific claims, 
    /// so this hook will look up the claims based on the email and staffusi on the web side.  
    /// </summary>
    /// <remarks>
    /// You must specify CustomClaimsAuthenticationManager to be used in the web.config by specifying <claimsAuthenticationManager type="CustomClaimsAuthenticationManager" />
    /// See class EdFi.Dashboards.Resources.Security.CustomClaimsAuthenticationManager for instructions on configuring.
    /// </remarks>
    public class AzureClaimsAuthenticationManagerProvider : IClaimsAuthenticationManagerProvider
    {
        private IStaffInformationFromEmailProvider staffInformationFromEmailProvider;
        private IClaimsIssuedTrackingEventProvider claimsIssuedTrackingEventProvider;
        private IUserClaimsProvider userClaimsProvider;
        private IGetImpersonatedClaimsDataProvider getImpersonatedClaimsDataProvider;

        public AzureClaimsAuthenticationManagerProvider(IStaffInformationFromEmailProvider staffInformationFromEmailProvider,
            IClaimsIssuedTrackingEventProvider claimsIssuedTrackingEventProvider, IUserClaimsProvider userClaimsProvider, IGetImpersonatedClaimsDataProvider getImpersonatedClaimsDataProvider)
        {
            this.staffInformationFromEmailProvider = staffInformationFromEmailProvider;
            this.claimsIssuedTrackingEventProvider = claimsIssuedTrackingEventProvider;
            this.userClaimsProvider = userClaimsProvider;
            this.getImpersonatedClaimsDataProvider = getImpersonatedClaimsDataProvider;
        }

        public Microsoft.IdentityModel.Claims.IClaimsPrincipal Get(string resourceName, Microsoft.IdentityModel.Claims.IClaimsPrincipal incomingPrincipal)
        {
            if (incomingPrincipal == null)
                throw new ArgumentNullException("incomingPrincipal");

            if( !incomingPrincipal.Identity.IsAuthenticated )
                throw new AuthenticationException();

            //Issue Claims
            string username = incomingPrincipal.Identity.Name;
            var staffUsi = this.staffInformationFromEmailProvider.ResolveStaffUSI(username);
            var appSpecificClaims = this.userClaimsProvider.GetApplicationSpecificClaims(username, staffUsi);
            var identity = incomingPrincipal.Identities.First();

            // Determine if we have a "wimp" (WIF-impersonation) parameter
            var isImpersonating = getImpersonatedClaimsDataProvider.IsImpersonating();

            var userClaimsData = isImpersonating
                                    ? getImpersonatedClaimsDataProvider.GetImpersonatedClaimsData(appSpecificClaims) // Create claims data for impersonated user
                                    : new UserClaimsData { Username = username, Claims = appSpecificClaims }; // Create claims data for current user

            identity.Claims.Add((new Claim(ClaimTypes.Name, username)));
            identity.Claims.AddRange(userClaimsData.Claims);
            this.claimsIssuedTrackingEventProvider.Track(username, staffUsi, false, userClaimsData.Claims);
            return incomingPrincipal;
        }
    }
}
