// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Text.RegularExpressions;
using System.Web;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Presentation.Architecture.Mvc.ValueProviders;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Presentation.Architecture.HttpModules
{
    public class SiteAvailableModule : IHttpModule
    {
        private ISiteAvailableProvider service;

        public void Init(HttpApplication context)
        {
            context.PostAuthenticateRequest += IsSiteAvailable;
        }

        public void Dispose()
        {
        }

        private void IsSiteAvailable(object sender, EventArgs e)
        {
            var currentUser = UserInformation.Current;
            if (currentUser != null && !((HttpApplication)sender).Request.Url.LocalPath.ToLower().Contains("error"))
            {
                try
                {
                    //Lets get the LEAId that is in context.
                    int localEducationAgencyId;

                    if (!TryGetLocalEducationAgencyId(out localEducationAgencyId))
                        return;

                    var killSwitch = SiteAvailableProvider.IsKillSwitchActivatedForCurrentUser(localEducationAgencyId);
                    if (killSwitch && !((HttpApplication)sender).Request.Url.LocalPath.ToLower().Contains("logout"))
                    {
                        ((HttpApplication)sender).Response.Redirect(EdFiDashboardsSite.Common.Logout().Resolve(), true);
                    }
                }
                ////This applied when we were pulling the LEA off of the user and not the context.
                //catch (InvalidOperationException)
                //{
                //    // this is thrown if the LEA can't be determined.
                    
                //    ((HttpApplication)sender).Response.Redirect(EdFiDashboardsSite.Common.Logout().Resolve(), true);
                //}
                catch(LocalEducationAgencyNotFoundException)
                {
                    //Note:Based on the premises that the KillSwitch is not a security artifact and is only used to turn the site off in the case of wrong data.
                    //Its Ok to do this...

                    //This catches any issue related with not finding the lea with the LocalEducationAgencyIdValueProvider.
                    //So do nothing...
                }
            }
        }

        private bool TryGetLocalEducationAgencyId(out int localEducationAgencyId)
        {
            localEducationAgencyId = 0;

            var leaUrlValue = GetLocalEducationAgencyCodeFromUrl();
            var routeValueResolutionService = IoC.Resolve<IRouteValueResolutionService>();
            var leaValueProvider = new LocalEducationAgencyIdValueProvider(leaUrlValue, routeValueResolutionService);

            var leaValueProviderResult = leaValueProvider.GetValue("LocalEducationAgencyId");

            // If value doesn't match an actual LEA, quit processing now
            if (leaValueProviderResult == null)
                return false;

            localEducationAgencyId = (int)leaValueProviderResult.RawValue;

            return localEducationAgencyId != 0;
        }

        private static Regex _localEducationAgencyRegex;

        private static Regex LocalEducationAgencyRegex
        {
            get
            {
                if (_localEducationAgencyRegex == null)
                {
                    string applicationPath = HttpContext.Current.Request.ApplicationPath; // i.e. "/MvcDashboardDev" or "/"

                    if (applicationPath.Length > 1)
                        applicationPath = applicationPath + "/";

                    // Extract the local education agency code out of the target URL
                    _localEducationAgencyRegex = new Regex(@"^(" + applicationPath + "Districts)/(?<lea>.*?)/", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                }

                return _localEducationAgencyRegex;
            }
        }

        private string GetLocalEducationAgencyCodeFromUrl()
        {
            string requestUrl = HttpContext.Current.Request.Path; // i.e. "/MvcDashboardDev/AllenISD/Overview" or "/AllenISD/Overview"

            var match = LocalEducationAgencyRegex.Match(requestUrl);

            if (!match.Success)
                throw new LocalEducationAgencyNotFoundException(string.Format("Unable to extract LEA. Regex: '{0}'  Request path: '{1}'", _localEducationAgencyRegex, HttpContext.Current.Request.Path));

            return match.Groups["lea"].Value;
        }

        private ISiteAvailableProvider SiteAvailableProvider
        {
            get
            {
                if (service == null)
                    service = IoC.Resolve<ISiteAvailableProvider>();

                return service;
            }
        }
    }
}