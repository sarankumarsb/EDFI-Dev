using System.Web;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
    /// <summary>
    /// Use this class when you are authenticating off of EdFi.Dashboards.SecurityTokenService.Web
    /// </summary>
    public class SecurityTokenServiceSignInRequestMessageProvider : ISignInRequestMessageProvider
    {
        protected ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks;

        public SecurityTokenServiceSignInRequestMessageProvider(ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks)
        {
            this.localEducationAgencyAreaLinks = localEducationAgencyAreaLinks;
        }

        public void Adorn(Microsoft.IdentityModel.Protocols.WSFederation.SignInRequestMessage signInRequestMessage, ISignInRequestAdornModel signInRequestAdornModel)
        {
            signInRequestMessage.Parameters.Add("lea", signInRequestAdornModel.LocalEducationAgencyCode);
            signInRequestMessage.Parameters.Add("leaName", signInRequestAdornModel.LocalEducationAgencyName);
            signInRequestMessage.Parameters.Add("home", this.localEducationAgencyAreaLinks.Home(signInRequestAdornModel.LocalEducationAgencyCode));

            if( !string.IsNullOrEmpty(signInRequestAdornModel.Wimp))
                signInRequestMessage.Parameters.Add("wimp", signInRequestAdornModel.Wimp);

            // VINLOGINOUT
            if (!string.IsNullOrEmpty(signInRequestAdornModel.idofuser))
                signInRequestMessage.Parameters.Add("idofuser", signInRequestAdornModel.idofuser);

            // VINLOGINOUT
            if (!string.IsNullOrEmpty(signInRequestAdornModel.idofuser))
                signInRequestMessage.Parameters.Add("idoftoken", signInRequestAdornModel.idoftoken);

            //Add this to the wctx to gaurantee it will come back to us when the token is posted back to us.  
            signInRequestMessage.Context = signInRequestMessage.Context + HttpUtility.UrlEncode("?" + signInRequestAdornModel.ToUrlQuery());
        }
    }
}
