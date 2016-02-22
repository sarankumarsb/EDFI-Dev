// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Castle.Core.Internal;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Infrastructure;
using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Navigation.Support
{
    public static class NavigationSupportExtensions
    {
        private static readonly Regex urlRegex = new Regex(@"^(?<url>[^?#]*)(?:\?(?<parameters>.*))?(?<anchor>[#].*)?$");

		public static string AppendParameters(this string url, params KeyValuePair<string, string>[] additionalParameters)
	    {
			var parameters = new List<string>();
			additionalParameters.ForEach(kvp => parameters.Add(kvp.Key + "=" + kvp.Value));
			return AppendParameters(url, parameters.ToArray());
	    }

	    public static string AppendParameters(this string url, params string[] additionalParameters)
        {
            string additionalParametersText =
                string.Join("&",
                            (from p in additionalParameters.Where(x => !string.IsNullOrEmpty(x) && !x.EndsWith("=")) // Filter out empty parameters
                             select p.TrimStart('&'))
                    .ToArray());

            // No parameters, nothing to do
            if (string.IsNullOrEmpty(additionalParametersText))
                return url;

            if (string.IsNullOrEmpty(url))
                return "?" + additionalParametersText;

            var match = urlRegex.Match(url);

            string newParms = match.Groups["parameters"].Value + "&" + additionalParametersText;

            if (newParms[0] == '&')
                return match.Groups["url"].Value + "?" + newParms.Substring(1) + match.Groups["anchor"].Value;

            return match.Groups["url"].Value + "?" + newParms + match.Groups["anchor"].Value;
        }
    }
}

namespace EdFi.Dashboards.Resources.Navigation
{
    public static class NavigationExtensions
    {
        public static string Anchor(this string resolvedUrl, string anchorName)
        {
            if (resolvedUrl.Contains("#"))
                throw new InvalidOperationException("Cannot have more than one '#' in a URL.");

            return resolvedUrl + "#" + anchorName;
        }

        public static string MetricAnchor(this string resolvedUrl, int? metricVariantId)
        {
            if (metricVariantId == null)
                return resolvedUrl;

            return resolvedUrl.Anchor("m" + metricVariantId);
        }

        public static string Resolve(this string virtualPath)
        {
            return ResolveServerUrl(virtualPath);
        }

        /// <summary>
        /// Returns a site relative HTTP path from a partial path starting out with a ~.
        /// Same syntax that ASP.Net internally supports but this method can be used
        /// outside of the Page framework.
        /// 
        /// Works like Control.ResolveUrl including support for ~ syntax
        /// but returns an absolute URL.
        /// </summary>
        /// <param name="originalUrl">Any Url including those starting with ~</param>
        /// <returns>relative url</returns>
        private static string ResolveUrl(string originalUrl)
        {
            if (originalUrl == null)
                return null;

            // *** Fix up image path for ~ root app dir directory
            if (originalUrl.IndexOf("://") == -1 && // *** not absolute path
                originalUrl.StartsWith("~") && 
                HttpContext.Current != null)
                return (HttpContext.Current.Request.ApplicationPath + originalUrl.Substring(1)).Replace("//", "/");

            // if running in test mode, HttpContext is null. So we just return the original url.
            return originalUrl;
        }

        /// <summary>
        /// This method returns a fully qualified absolute server Url which includes
        /// the protocol, server, port in addition to the server relative Url.
        /// 
        /// Works like Control.ResolveUrl including support for ~ syntax
        /// but returns an absolute URL.
        /// </summary>
        /// <param name="serverUrl">Any Url, either App relative or fully qualified</param>
        /// <param name="forceHttps">if true forces the url to use https</param>
        /// <returns></returns>
        private static string ResolveServerUrl(string serverUrl, bool forceHttps)
        {
            if (serverUrl == null)
                return null;

            // *** Is it already an absolute Url?
            if (serverUrl.IndexOf("://") > -1 || HttpContext.Current == null)
                return serverUrl;

            // *** Start by fixing up the Url an Application relative Url
            string newUrl = ResolveUrl(serverUrl);

            Uri originalUri = IoC.Resolve<ICurrentUrlProvider>().Url;

            newUrl = (forceHttps ? "https" : originalUri.Scheme) +
                     "://" + originalUri.Authority + newUrl;

            return newUrl;
        }

        /// <summary>
        /// This method returns a fully qualified absolute server Url which includes
        /// the protocol, server, port in addition to the server relative Url.
        /// 
        /// It work like Page.ResolveUrl, but adds these to the beginning.
        /// This method is useful for generating Urls for AJAX methods
        /// </summary>
        /// <param name="serverUrl">Any Url, either App relative or fully qualified</param>
        /// <returns></returns>
        private static string ResolveServerUrl(string serverUrl)
        {
            return ResolveServerUrl(serverUrl, true);
        }
    }
}
