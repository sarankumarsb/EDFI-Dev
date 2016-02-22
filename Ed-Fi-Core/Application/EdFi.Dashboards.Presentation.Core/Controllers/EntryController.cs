// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Web.Mvc;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Presentation.Core.Controllers
{
    public class EntryController : Controller
    {
        private readonly ISessionStateProvider sessionStateProvider;
        private readonly IEntryService entryService;

        public EntryController(ISessionStateProvider sessionStateProvider, IEntryService entryService)
        {
            this.sessionStateProvider = sessionStateProvider;
            this.entryService = entryService;
        }

        public ActionResult Get(EntryRequest request)
        {
            // Check to see if the incoming user's claims match the targeted local education agency
            if (!UserInformation.Current.IsAffiliatedWithLocalEducationAgency(request.LocalEducationAgencyId))
                throw new UserAccessDeniedException(
                    string.Format("User is not affiliated with the local education agency at '{0}'.", ControllerContext.HttpContext.Request.Url));

            // Extract parameterized rule from user's claims
            string landingPageUrl = entryService.Get(request);

            // Store their "home" page for later
            sessionStateProvider[EdFiApp.Session.LandingPageUrl] = landingPageUrl;

            // this allows this page to handle this processing when the web app's session has expired
            // but the authentication did not expire. it then returns the user to the page they requested.
            var returnUrl = Request["returnUrl"];
            if (!String.IsNullOrEmpty(returnUrl))
            {
                return new RedirectResult(returnUrl);
            }

            return new RedirectResult(landingPageUrl);
        }
    }
}
