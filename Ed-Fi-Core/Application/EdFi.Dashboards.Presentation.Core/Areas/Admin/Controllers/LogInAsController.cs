// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Text;
using System.Web.Mvc;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Admin;
using EdFi.Dashboards.Resources.Models.Admin;
using EdFi.Dashboards.Resources.Navigation;
using Microsoft.IdentityModel.Web;
using EdFi.Dashboards.Resources.Security;

namespace EdFi.Dashboards.Presentation.Core.Areas.Admin.Controllers
{
    public class LogInAsController : Controller
    {
        private readonly IService<CanLogInAsUserRequest, CanLogInAsUserModel> service;
        private readonly ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks;
        private readonly ISignInRequestMessageProvider signInRequestMessageProvider;

        public LogInAsController(IService<CanLogInAsUserRequest, CanLogInAsUserModel> service, ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks,
			ISignInRequestMessageProvider signInRequestMessageProvider)
        {
            this.service = service;
            this.localEducationAgencyAreaLinks = localEducationAgencyAreaLinks;
            this.signInRequestMessageProvider = signInRequestMessageProvider;
        }

        public ActionResult Get(int localEducationAgencyId,  long staffUSI)
        {
            var canLoginAsUser = service.Get(new CanLogInAsUserRequest { StaffUSI = staffUSI });
            if (!canLoginAsUser.CanLogIn)
            {
                return View();
            }

            var localEducationAgencyContextProvider = IoC.Resolve<ILocalEducationAgencyContextProvider>();
            string localEducationAgencyCode = localEducationAgencyContextProvider.GetCurrentLocalEducationAgencyCode();

            WSFederationAuthenticationModule authModule = FederatedAuthentication.WSFederationAuthenticationModule;
            var signInRequest = authModule.CreateSignInRequest("passive", localEducationAgencyAreaLinks.Entry(localEducationAgencyCode), false);
            
            // Base64 encode the staff usi just to make it less obvious what we're doing
            byte[] bytes = Encoding.UTF8.GetBytes(staffUSI.ToString());
            string base64StaffUSI = Convert.ToBase64String(bytes);

            // Redirect to the STS with a sign in request, augmented with an email address to impersonate
            FederatedAuthentication.SessionAuthenticationModule.SignOut();

            this.signInRequestMessageProvider.Adorn(signInRequest, new SignInRequestAdornModel()
            {
                LocalEducationAgencyId = localEducationAgencyId,
                LocalEducationAgencyCode = localEducationAgencyCode,
                Wimp = base64StaffUSI
            });

            return Redirect(signInRequest.RequestUrl);
        }
    }
}
