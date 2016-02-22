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
    public class WoopraAnalyticsModule : IHttpModule
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
            var domainTracking = ConfigurationManager.AppSettings["WoopraDomainTracking"];
            if (String.IsNullOrWhiteSpace(domainTracking))
                return;

            HttpResponse response = HttpContext.Current.Response;

            if (response.ContentType != "text/html")
                return;

            var userInfo = UserInformation.Current;
            if (userInfo == null)
                return;

            
            string orgCode = String.Empty;
            try
            {
                orgCode = LocalEducationAgencyContextProvider.GetCurrentLocalEducationAgencyCode();
            }
            catch (InvalidOperationException)
            {
                // this can happen on the default landing page.
            }

            if (String.IsNullOrWhiteSpace(orgCode))
                return;

            // don't log if impersonating
            if (userInfo.HasIdentityClaim(EdFiClaimTypes.Impersonating))
                return;

            // don't log if non-district user
            if (userInfo.StaffUSI < 0)
                return;

            // since the default staff page reloads, if we don't have a section let's not log
            var dashboardContext = EdFiDashboardContext.Current;
            if (dashboardContext == null || (dashboardContext.StaffUSI.HasValue && String.IsNullOrWhiteSpace(dashboardContext.StudentListType)))
                return;

            response.Filter = new WoopraAnalyticsFilter(response.Filter)
                                    {
                                        Domain = ConfigurationManager.AppSettings["WoopraDomainTracking"],
                                        UserFullName = userInfo.FullName,
                                        UserUSI = userInfo.StaffUSI,
                                        LocalEducationAgencyCode = orgCode,
                                        Email = userInfo.EmailAddress,

                                        LocalEducationAgencyId = dashboardContext.LocalEducationAgencyId,
                                        SchoolId = dashboardContext.SchoolId,
                                        StaffUSI = dashboardContext.StaffUSI,
                                        SectionOrCohortId = dashboardContext.SectionOrCohortId,
                                        StudentListType = dashboardContext.StudentListType,
                                        StudentUSI = dashboardContext.StudentUSI,
                                        TrackingUrl = IoC.Resolve<ICurrentUrlProvider>().Url.AbsolutePath.ToLower()
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
