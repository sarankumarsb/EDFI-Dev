using EdFi.Dashboards.Resources.Security;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.IdentityModel.Claims;
using EdFi.Dashboards.Infrastructure;

namespace EdFi.Dashboards.SecurityTokenService.Web.Providers
{
    /// <summary>
    /// Use this version if you want the STS to only authenticate.  Some claims are added to the token, but the Application Claims
    /// will be added in the web site.  Be sure to turn on DashboardClaimsAuthenticationManagerProvider for IClaimsAuthenticationManagerProvider
    /// to use this version.
    /// </summary>
    public class IdentityClaimsGetOutputClaimsIdentityProvider : IGetOutputClaimsIdentityProvider
    {
        private IStaffInformationProvider staffInformationProvider;
        private IAuthenticationProvider authenticationProvider;
        private IDashboardUserClaimsInformationProvider<EdFiUserSecurityDetails> dashboardUserClaimsInformationProvider;
        private IHttpRequestProvider httpRequestProvider;

        public const string NoUserInformationErrorMessage = "We were able to verify your password, but were unable to locate your user information which is needed to authorize access to the website.  If you feel this is incorrect, please contact an administrator.";

        public IdentityClaimsGetOutputClaimsIdentityProvider(IStaffInformationProvider staffInformationProvider, IAuthenticationProvider authenticationProvider,
            IDashboardUserClaimsInformationProvider<EdFiUserSecurityDetails> dashboardUserClaimsInformationProvider, IHttpRequestProvider httpRequestProvider)
        {
            this.staffInformationProvider = staffInformationProvider;
            this.authenticationProvider = authenticationProvider;
            this.dashboardUserClaimsInformationProvider = dashboardUserClaimsInformationProvider;
            this.httpRequestProvider = httpRequestProvider;
        }

        public Microsoft.IdentityModel.Claims.IClaimsIdentity GetOutputClaimsIdentity(Microsoft.IdentityModel.Claims.IClaimsPrincipal principal, Microsoft.IdentityModel.Protocols.WSTrust.RequestSecurityToken request, Microsoft.IdentityModel.SecurityTokenService.Scope scope)
        {
            if (principal == null)
                throw new ArgumentNullException("principal");

            string username = principal.Identity.Name;
            var staffUSI = staffInformationProvider.ResolveStaffUSI(authenticationProvider, username);

            // Get information necessary to create the app-specific claims
            var userClaimsInfo = dashboardUserClaimsInformationProvider.GetClaimsInformation(username, staffUSI);

            // No user information found?
            if (userClaimsInfo == null)
            {
                throw new StaffSchoolClassAssociationException(NoUserInformationErrorMessage) { Name = username, StaffUSI = staffUSI };
            }

            var claims = new List<Claim>();

            // Initialize "Name" claims
            claims.Add(new Claim(EdFiClaimTypes.FullName, userClaimsInfo.FullName));
            claims.Add(new Claim(ClaimTypes.GivenName, userClaimsInfo.FirstName));
            claims.Add(new Claim(ClaimTypes.Surname, userClaimsInfo.LastName));
            claims.Add(new Claim(CustomDashboardClaimType.LocalEducationAgencyCode, this.httpRequestProvider.GetValue("lea")));

            if (!string.IsNullOrEmpty(userClaimsInfo.Email))
                claims.Add(new Claim(ClaimTypes.Email, userClaimsInfo.Email));
            else
                throw new InvalidOperationException(string.Format("Missing required claim: {0}", ClaimTypes.Email));

            // Add the StaffUSI claim
            claims.Add(new Claim(EdFiClaimTypes.StaffUSI, staffUSI.ToString()));
            // Add the UserType claim - EDFIDB-139	            
            claims.Add(new Claim(EdFiClaimTypes.UserType, userClaimsInfo.UserType.ToString()));

            string serviceType = string.IsNullOrEmpty(Convert.ToString(this.httpRequestProvider.GetValue("idofuser"))) ? "Normal" : "Moodel"; // Convert.ToString(System.Web.HttpContext.Current.Session["ServiceType"]); // VINLOGINTYP
            claims.Add(new Claim(EdFiClaimTypes.ServiceType, (serviceType != null) ? serviceType : ""));

            // VIN05112015                        
            claims.Add(new Claim(EdFiClaimTypes.UserId, string.IsNullOrEmpty(this.httpRequestProvider.GetValue("idofuser")) ? "" : this.httpRequestProvider.GetValue("idofuser")));
            claims.Add(new Claim(EdFiClaimTypes.UserToken, string.IsNullOrEmpty(this.httpRequestProvider.GetValue("idoftoken")) ? "" : this.httpRequestProvider.GetValue("idoftoken")));

            var claimsIdentity = new ClaimsIdentity();

            // Issue custom claims
            claimsIdentity.Claims.Add(new Claim(ClaimTypes.Name, username));
            claimsIdentity.Claims.AddRange(claims);
            return claimsIdentity;

        }
    }
}