// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Security;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Routing;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Presentation.Core.Models.Shared;
using EdFi.Dashboards.Presentation.Web.Areas.Search.Models;
using EdFi.Dashboards.Presentation.Web.Utilities;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using BriefRequest = EdFi.Dashboards.Resources.School.BriefRequest;
using IBriefService = EdFi.Dashboards.Resources.School.IBriefService;
using Microsoft.IdentityModel.Web;
using EdFi.Dashboards.Common.Utilities;

namespace EdFi.Dashboards.Presentation.Core.Controllers
{
    public class MasterPageController : Controller
    {
        protected readonly ISystemMessageProvider systemMessageProvider;
        protected readonly ICurrentUrlProvider currentUrlProvider;
        protected readonly IConfigValueProvider configValueProvider;
        protected readonly IDirectory directory;
        protected readonly IBriefService briefService;
        protected readonly ILevelCrumbService levelCrumbService;
        protected readonly IEntryService entryService;
        protected readonly ISessionStateProvider sessionStateProvider;
        protected readonly IHttpRequestProvider httpRequestProvider;
        protected readonly ICurrentUserClaimInterrogator currentUserClaimInterrogator;
        protected readonly ICacheProvider cacheProvider;
        protected readonly IHomeService service;
        protected readonly ICacheKeyGenerator cacheKeyGenerator;

        public MasterPageController(ISystemMessageProvider systemMessageProvider, 
            ICurrentUrlProvider currentUrlProvider, 
            IConfigValueProvider configValueProvider,
            IDirectory directory,
            IBriefService briefService,
            ILevelCrumbService levelCrumbService,
            IEntryService entryService,
            ISessionStateProvider sessionStateProvider,
            IHttpRequestProvider httpRequestProvider,
            ICacheProvider cacheProvider,
            ICurrentUserClaimInterrogator currentUserClaimInterrogator,
            IHomeService service,
            ICacheKeyGenerator cacheKeyGenerator
            )
        {
            this.systemMessageProvider = systemMessageProvider;
            this.currentUrlProvider = currentUrlProvider;
            this.configValueProvider = configValueProvider;
            this.directory = directory;
            this.briefService = briefService;
            this.levelCrumbService = levelCrumbService;
            this.entryService = entryService;
            this.sessionStateProvider = sessionStateProvider;
            this.httpRequestProvider = httpRequestProvider;
            this.currentUserClaimInterrogator = currentUserClaimInterrogator;
            this.cacheProvider = cacheProvider;
            this.service = service;
            this.cacheKeyGenerator = cacheKeyGenerator;
        }

        [ChildActionOnly]
        public ActionResult HelpLinkUrl()
        {
            int? localEducationAgencyId = EdFiDashboardContext.Current.LocalEducationAgencyId;

            if (localEducationAgencyId == null)
                return PartialView((object) null);

            var trainingAndPlanningHref = GetCacheKeyValue(GetCacheKey((MethodInfo)MethodBase.GetCurrentMethod(), localEducationAgencyId),
                                    () => (service.Get(HomeRequest.Create(localEducationAgencyId.Value))).TrainingAndPlanningHref,
                                    30);

            return PartialView((object)trainingAndPlanningHref);
        }

        [ChildActionOnly]
        public ActionResult SystemMessage()
        {
            int? localEducationAgencyId = EdFiDashboardContext.Current.LocalEducationAgencyId;

            if (localEducationAgencyId == null)
                return PartialView((object) null);

            string cacheKey;

            var cachedSystemMessageProvider = systemMessageProvider as IHasCachedResult;

            if (cachedSystemMessageProvider != null)
                cacheKey = cachedSystemMessageProvider.GetCacheKey(localEducationAgencyId.Value);
            else
                cacheKey = GetCacheKey((MethodInfo) MethodBase.GetCurrentMethod(), localEducationAgencyId);

            var systemMessage = GetCacheKeyValue(cacheKey,
                                    () => systemMessageProvider.GetSystemMessage(localEducationAgencyId.Value),
                                    5);

            return PartialView((object) systemMessage);
        }

        private string GetCacheKeyValue(string cacheKey, Func<string> getValue, int durationMinutes)
        {
            string cacheValue;
            object value;

            if (cacheProvider.TryGetCachedObject(cacheKey, out value))
            {
                // Use the cached message
                cacheValue = (string)value;
            }
            else
            {
                // Get the source value
                cacheValue = getValue.Invoke();

                // Expire the cached message in durationMinutes
                cacheProvider.Insert(cacheKey, cacheValue, null, DateTime.Now.AddMinutes(durationMinutes), Cache.NoSlidingExpiration);
            }

            return cacheValue;
        }

        private string GetCacheKey(MethodInfo methodInfo, int? localEducationAgencyId)
        {
            string cacheKey = cacheKeyGenerator.GenerateCacheKey(methodInfo, new object[] {localEducationAgencyId});
            return cacheKey;
        }

        [ChildActionOnly]
        public ActionResult JQueryGlobalSiteBasePath()
        {
            string siteBasePath = currentUrlProvider.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath;

            return PartialView((object) siteBasePath);
        }

        [ChildActionOnly]
        public ActionResult TopRightCorner()
        {
            string topRightLegend = configValueProvider.GetValue("TopRightCornerLegend").Trim();

            return PartialView((object) topRightLegend);
        }

        [ChildActionOnly]
        public virtual ActionResult LevelCrumb()
        {
            var edfiContext = EdFiDashboardContext.Current;
            var request = new LevelCrumbRequest
                              {
                                  LocalEducationAgencyId = edfiContext.LocalEducationAgencyId,
                                  SchoolId = edfiContext.SchoolId,
                                  StaffUSI = edfiContext.StaffUSI
                              };

            //The level crumb service expects that if we have a school the LEA Id has to be passed in ass well. This due to security.
            if (request.SchoolId.HasValue)
                if (!request.LocalEducationAgencyId.HasValue)
                {
                    var schoolBriefRequest = new BriefRequest { SchoolId = request.SchoolId.Value };
                    request.LocalEducationAgencyId = briefService.Get(schoolBriefRequest).LocalEducationAgencyId;
                }

            #region Staff specific logic
            var staffUSI = edfiContext.StaffUSI;
            if (request.StaffUSI != null && request.SchoolId != null)
            {
                sessionStateProvider[EdFiApp.Session.LevelCrumbSchoolStaff] = new SchoolStaff(request.SchoolId, request.StaffUSI);
            }
            else
            {
                var schoolStaff = sessionStateProvider.GetValue(EdFiApp.Session.LevelCrumbSchoolStaff) as SchoolStaff;
                if (schoolStaff != null)
                {
                    // inject the staffUSI only if the cached schoolId matches
                    if (request.SchoolId == schoolStaff.SchoolId)
                    {
                        staffUSI = schoolStaff.StaffUSI;
                    }
                    else
                    {
                        // clear the cached value if the schoolId does not match.
                        sessionStateProvider.SetValue(EdFiApp.Session.LevelCrumbSchoolStaff, null);
                    }
                }
            }

            InitializeTeachersLastUrl();
            request.StaffUSI = staffUSI;
            #endregion

            EdFi.Dashboards.Resources.Models.Common.LevelCrumbModel model;
            //Graceful degradation if you don't have access or you are missing a claim "AccessOrganization"
            try
            {
                model = levelCrumbService.Get(request);
            }
            catch (SecurityAccessDeniedException)
            {
                model = new EdFi.Dashboards.Resources.Models.Common.LevelCrumbModel();
            }
            
            // Populate the Home URL using the current user's session value
            model.HomeHref = (string)sessionStateProvider.GetValue(EdFiApp.Session.LandingPageUrl);

            if (String.IsNullOrEmpty(model.HomeHref))
            {
                // Extract parameterized rule from user's claims
                string landingPageUrl = entryService.Get(EntryRequest.Create(edfiContext.LocalEducationAgencyId, edfiContext.SchoolId));

                // Store their "home" page for later
                sessionStateProvider[EdFiApp.Session.LandingPageUrl] = landingPageUrl;
                model.HomeHref = landingPageUrl;
            }

            //More staff specific logic...
            if (model.BriefModel != null)
                if (string.IsNullOrEmpty(TeachersLastUrl))
                    model.BriefModel = null;
                else
                    model.BriefModel.Url = TeachersLastUrl;

            var viewModel = new EdFi.Dashboards.Presentation.Core.Models.Shared.LevelCrumbModel
                                {
                                    HomeIconHref = model.HomeIconHref,
                                    HomeHref = model.HomeHref,
                                    LocalEducationAgencyName = model.LocalEducationAgencyBriefModel != null ? model.LocalEducationAgencyBriefModel.Name : null,
                                    SchoolName = model.SchoolBriefModel != null ? model.SchoolBriefModel.Name : null,
                                    StaffName = model.BriefModel != null ? model.BriefModel.FullName : null,
                                    StaffHref = model.BriefModel != null ? model.BriefModel.Url : null
                                };

            if (model.LocalEducationAgencyBriefModel != null && UserInformation.Current.HasAnyLocalEducationAgencyLevelClaims(model.LocalEducationAgencyBriefModel.LocalEducationAgencyId))
                viewModel.LocalEducationAgencyHref = model.LocalEducationAgencyBriefModel.Url;

            if (model.SchoolBriefModel != null && (currentUserClaimInterrogator.HasClaimOnEducationOrganization(EdFiClaimTypes.AccessOrganization, model.SchoolBriefModel.SchoolId) 
                                                   || currentUserClaimInterrogator.HasClaimOnEducationOrganization(EdFiClaimTypes.ViewAllMetrics, model.LocalEducationAgencyBriefModel.LocalEducationAgencyId)))
                viewModel.SchoolHref = model.SchoolBriefModel.Url;

            return PartialView(viewModel);
        }

        [ChildActionOnly]
        public ActionResult Search()
        {
            bool canSearch;
            int? educationOrganization = EdFiDashboardContext.Current.SchoolId ?? EdFiDashboardContext.Current.LocalEducationAgencyId;
            if (educationOrganization.HasValue)
            {
                canSearch =
                    (currentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(EdFiClaimTypes.AdministerDashboard, (int) educationOrganization) || // search on staff
                    currentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllMetrics, (int) educationOrganization));
            }
            else
            {
                canSearch = (currentUserClaimInterrogator.HasClaimForStateAgency(EdFiClaimTypes.AdministerDashboard) ||
                             currentUserClaimInterrogator.HasClaimForStateAgency(EdFiClaimTypes.ViewAllMetrics));
            }
            if(!canSearch)
            {
                // search on schools
                canSearch = (currentUserClaimInterrogator.HasClaimForSearch(EdFiClaimTypes.ViewAllTeachers) || // search on teachers
                currentUserClaimInterrogator.HasClaimForSearch(EdFiClaimTypes.ViewAllStudents) || // search on students
                 currentUserClaimInterrogator.HasClaimForSearch(EdFiClaimTypes.ViewMyStudents));
            }
        
            var model = new QuickSearchModel
                            {
                                CanSearch = canSearch,
                                LocalEducationAgencyId = EdFiDashboardContext.Current.LocalEducationAgencyId
                            };
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult Feedback(string supportLinkControlId, bool allowNameEdit=false)
        {
            var model = new FeedbackModel
                            {
                                SupportLinkControlId = supportLinkControlId,
                                AllowNameEdit = allowNameEdit,
                            };

            var identity = UserInformation.Current;

            if (identity != null)
            {
                model.UserName = identity.FullName;
                model.Email = !string.IsNullOrEmpty(identity.EmailAddress) ? identity.EmailAddress : string.Empty;

                if (!model.AllowNameEdit)
                    model.DisableFeedbackName = "readonly='readonly'"; // Readonly is more accessible by screen readers

                if (!model.AllowNameEdit && !string.IsNullOrEmpty(identity.EmailAddress))
                    model.DisableFeedbackEmail = "readonly='readonly'"; // Readonly is more accessible by screen readers
            }

            return PartialView(model);
        }


        [ChildActionOnly]
        public ActionResult DashboardStatus()
        {
            if (UserInformation.Current.ServiceType != "Normal" && 1 == 2)
            {
                var authenticationProvider = new EdFi.Dashboards.Resources.Security.Implementations.QuadraLMSTokenAuthenticationProvider();
                if (!authenticationProvider.ValidateUser(UserInformation.Current.UserId, UserInformation.Current.UserToken))
                {
                    //return RedirectToAction("Get", "LogOut");
                    var userName = String.Empty;
                    var currentUser = System.Threading.Thread.CurrentPrincipal;
                    if (currentUser != null)
                        userName = currentUser.Identity.Name;

                    //siteUseLog.Info(String.Format("{0} logged out.", userName));

                    var session = IoC.Resolve<ISessionStateProvider>();
                    session.Clear();
                    FederatedAuthentication.SessionAuthenticationModule.SignOut();

                    var redirect = ("~/").Resolve();

                    if (currentUser != null && currentUser.Identity.IsAuthenticated)
                    {
                        // FederatedSignOut will send the redirect headers and start the Response
                        WSFederationAuthenticationModule.FederatedSignOut(null, new Uri(redirect));
                    }

                    return Response.IsRequestBeingRedirected ? null : Redirect(redirect);
                }
            }
            return View();
        }

        #region Staff Specific logic
        protected string TeachersLastUrl
        {
            get
            {
                return (sessionStateProvider[EdFiApp.Session.TeachersLastUrl] != null) ? sessionStateProvider[EdFiApp.Session.TeachersLastUrl].ToString() : "";
            }
            set
            {
                sessionStateProvider[EdFiApp.Session.TeachersLastUrl] = value;
            }
        }

        /// <summary>
        /// Sets the TeachersLastUrl to a virtual path with the query string on it "~/xxxx/xxx.aspx?vvv=###&rrr=###
        /// </summary>
        protected void InitializeTeachersLastUrl()
        {
            //Get current Area
            var currentRoute = this.ControllerContext.RouteData.Route as Route;
            string currentArea = currentRoute.GetArea();
            
            //Get the referrer Area
            Route referrerRoute = null;
            string refererArea = string.Empty;
            if (httpRequestProvider.UrlReferrer != null)
            {
                var request = new HttpRequest(null, GetCleanUrl(httpRequestProvider.UrlReferrer), null);
                var response = new HttpResponse(new StringWriter());
                var httpContext = new HttpContext(request, response);
                var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
                
                if (routeData != null)
                {
                    referrerRoute = routeData.Route as Route;
                    refererArea = referrerRoute.GetArea();
                }
            }

            //If we are at a staff area then we set the TeachersLastURL
            if (currentArea == "Staff")
                TeachersLastUrl = Request.Url.ToString();

            //If we are at a student path level then we see if we just moved there from a teacher level page.
            if ((currentArea == "StudentSchool") && (refererArea == "Staff"))
                TeachersLastUrl = Request.UrlReferrer.ToString();

            //If we are any where but we came from the Teacher we save it.
            if (string.IsNullOrEmpty(TeachersLastUrl))
                if (refererArea == "Staff")
                    TeachersLastUrl = Request.UrlReferrer.ToString();

            //Reset the teacher URL if we come up to a school level.
            if ( currentArea=="School" || currentArea=="LocalEducationAgency" || currentArea=="SearchResults")
                TeachersLastUrl = "";
        }

        private string GetCleanUrl(Uri uri)
        {
            if (uri.ToString().Contains("?"))
                return uri.ToString().Replace(uri.Query, "");

            return uri.ToString();
        }

        protected class SchoolStaff
        {
            public int? SchoolId;
            public long? StaffUSI;

            public SchoolStaff(int? schoolId, long? staffId)
            {
                SchoolId = schoolId;
                StaffUSI = staffId;
            }
        }
        #endregion

    }
}
