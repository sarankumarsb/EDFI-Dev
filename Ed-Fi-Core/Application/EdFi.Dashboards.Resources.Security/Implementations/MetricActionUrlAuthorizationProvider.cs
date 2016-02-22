// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Routing;
using EdFi.Dashboards.Common.Configuration;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
	public class MetricActionUrlAuthorizationProvider : IMetricActionUrlAuthorizationProvider
	{
        private readonly LocationAuthorizationConfiguration locationAuthorizationSection;
	    private readonly ICurrentUserClaimInterrogator currentUserClaimInterrogator;
        public MetricActionUrlAuthorizationProvider(IConfigSectionProvider configSectionProvider, ICurrentUserClaimInterrogator currentUserClaimInterrogator)
        {
            this.currentUserClaimInterrogator = currentUserClaimInterrogator;
            locationAuthorizationSection = configSectionProvider.GetSection(LocationAuthorizationConfiguration.SectionName) as LocationAuthorizationConfiguration;
            if (locationAuthorizationSection == null)
                throw new ConfigurationErrorsException("Missing Configuration Section '" + LocationAuthorizationConfiguration.SectionName + "'.");
        }

	    /// <summary>
	    /// This is used to filter out links that the user does not have access to based on claims.
	    /// If there are no claims required for a specific path then the path will NOT be filtered out.
	    /// If there are claims required for a specific path then the current user must have at least
	    /// one of the required claims.
	    /// </summary>
	    /// <param name="virtualPath"></param>
	    /// <param name="educationalOrganization">The educational organization context to consider for the claims</param>
	    /// <returns></returns>
	    public bool CurrentUserHasAccessToPath(string virtualPath, int educationalOrganization)
		{
            if (virtualPath == null) return true;

            var path = GetVirtualPathWithoutQueryString(virtualPath);
            var routeInfo = new RouteInfo(new Uri(path), HttpContext.Current.Request.ApplicationPath);
            var area = routeInfo.RouteData.DataTokens["area"];
            var controller = routeInfo.RouteData.Values["controller"];

		    LocationAuthorization auth = null;
            foreach (LocationAuthorization locationAuthorization in locationAuthorizationSection.LocationAuthorizations)
            {
                if (locationAuthorization.Area.Equals(area) && locationAuthorization.Controller.Equals(controller))
                {
                    auth = locationAuthorization;
                    break;
                }
            }
            if (auth == null) return true;

            var names = auth.AuthorizedBy;
            var claims = ClaimHelper.GetClaimValuesByNames(names);
            var result = claims.Any(c => (currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(c, educationalOrganization)));

            return result;
		}

        private static string GetVirtualPathWithoutQueryString(string url)
        {
            if (url.Contains("?"))
            {
                return url.Substring(0, url.IndexOf("?"));
            }

            return url;
        }
	}
}
