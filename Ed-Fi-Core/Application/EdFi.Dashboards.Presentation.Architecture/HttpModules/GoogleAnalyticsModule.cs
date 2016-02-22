// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Configuration;
using System.Web;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Presentation.Architecture.HttpModules
{
    public class GoogleAnalyticsModule : IHttpModule
    {
        public void Dispose()
        {
            //clean-up code here.
        }

        public void Init(HttpApplication context)
        {
            context.ReleaseRequestState += InstallResponseFilter;
        }

        private void InstallResponseFilter(object sender, EventArgs e)
        {
            HttpResponse response = HttpContext.Current.Response;

            if (response.ContentType != "text/html")
                return;

            string userTracking = String.Empty;

            var userInfo = UserInformation.Current;
            var dashboardContext = EdFiDashboardContext.Current;
            if (userInfo != null && dashboardContext != null && !userInfo.HasIdentityClaim(EdFiClaimTypes.Impersonating))
                userTracking = ("rizixere" + userInfo.StaffUSI + dashboardContext.LocalEducationAgencyId).GetHashCode().ToString();

            string orgCode = String.Empty;
            try
            {
                orgCode = LocalEducationAgencyContextProvider.GetCurrentLocalEducationAgencyCode();
            }
            catch (InvalidOperationException)
            {
                // this can happen on the default landing page.
            }
            

            response.Filter = new GoogleAnalyticsFilter(response.Filter)
                                    {
                                        AnalyticsId = ConfigurationManager.AppSettings["GoogleAnalyticsId"],
                                        LocalEducationAgency = orgCode,
                                        UserTracking = userTracking,
                                        TrackingUrl = IoC.Resolve<ICurrentUrlProvider>().Url.PathAndQuery.ToLower()
                                    };
        }

        private ILocalEducationAgencyContextProvider localEducationAgencyContextProvider;
        private ILocalEducationAgencyContextProvider LocalEducationAgencyContextProvider
        {
            get
            {
                if (localEducationAgencyContextProvider == null)
                    localEducationAgencyContextProvider = IoC.Resolve<ILocalEducationAgencyContextProvider>();
                return localEducationAgencyContextProvider;
            }
        }
    }
}
