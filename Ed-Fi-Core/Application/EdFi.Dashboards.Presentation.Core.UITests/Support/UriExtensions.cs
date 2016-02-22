using System;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Presentation.Core.UITests.Support.SpecFlow;
using RestSharp;
using TechTalk.SpecFlow;

namespace EdFi.Dashboards.Presentation.Core.UITests.Support
{
    public static class UriExtensions
    {
        /// <summary>
        /// Prepares a <see cref="Uri"/> (generally based on the browser's current location) for a call to the <see cref="RestClient"/>
        /// to get the deserialized view model for a web page.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> representing the browser's current location (or a 
        /// full <see cref="Uri"/> to a dashboard website location).</param>
        /// <returns>A path relative to the dashboard website root that can be used for a request with the <see cref="RestClient"/>.</returns>
        public static string ToRelativeDashboardPath(this Uri uri)
        {
            return uri.AbsoluteUri.ToRelativeDashboardPath();
        }

        public static string ToRelativeDashboardPath(this string url)
        {
            return url
                    .Replace(ScenarioContext.Current.GetRestClient().BaseUrl, string.Empty, StringComparison.InvariantCultureIgnoreCase)
                    .TrimStart('/');
        }
    }
}
