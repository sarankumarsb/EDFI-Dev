using System;
using System.Text.RegularExpressions;
using System.Web;

namespace EdFi.Dashboards.Presentation.Architecture.Providers
{
    public interface IRequestUrlBaseProvider
    {
        string GetRequestUrlBase(HttpRequestBase httpRequestBase);
    }

    public class RequestUrlBaseProvider : IRequestUrlBaseProvider
    {
        public string GetRequestUrlBase(HttpRequestBase httpRequestBase)
        {
            string requestUrl = httpRequestBase.Url.ToString(); // i.e. "https://localhost/MvcDashboardDev/AllenISD/Overview"

            string applicationPath = httpRequestBase.ApplicationPath; // i.e. "/MvcDashboardDev" or ""

            if (!string.IsNullOrEmpty(applicationPath) && !applicationPath.Equals("/"))
            {
                // Extract the leading portion of target URL
                var applicationPattern = @"^(?<RouteBase>.*?" + applicationPath + "[/]?)";
                var match = Regex.Match(requestUrl, applicationPattern);

                if (!match.Success)
                    throw new Exception(string.Format("Unable to extract base of URL using pattern '{0}' with application path '{1}'.", applicationPattern, applicationPath));

                return match.Groups["RouteBase"].Value;
            }
            else
            {
                string host = httpRequestBase.Url.Host;

                string routeBasePattern = @"^(?<RouteBase>.*/" + host + @"(\:[0-9]+)?[/]?)";
                var match = Regex.Match(requestUrl, routeBasePattern);

                if (!match.Success)
                    throw new Exception(string.Format("Unable to extract base of URL using pattern '{0}' with host name '{1}'.", routeBasePattern, host));

                return match.Groups["RouteBase"].Value;
            }
        }
    }
}
