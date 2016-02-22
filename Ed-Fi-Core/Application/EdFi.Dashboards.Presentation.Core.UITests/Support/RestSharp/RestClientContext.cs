using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Presentation.Core.Tests.Routing.Support;
using RestSharp;

namespace EdFi.Dashboards.Presentation.Core.UITests.Support.RestSharp
{
    public static class RestClientContext
    {
        static RestClientContext()
        {
            // Ignore SSL errors
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
        }

        private static IDictionary<string, RestClient> _restClientsByUserProfile
            = new Dictionary<string, RestClient>(StringComparer.OrdinalIgnoreCase);

        public static RestClient For(string userProfileName)
        {
            RestClient restClient;

            if (_restClientsByUserProfile.TryGetValue(userProfileName, out restClient))
                return restClient;

            var newClient = CreateNewRestClient(userProfileName);
            _restClientsByUserProfile[userProfileName] = newClient;

            return newClient;
        }

        private static RestClient CreateNewRestClient(string userProfileName)
        {
            string baseUrl = TestSessionContext.Current.Configuration.BaseUrl;
            string localEducationAgency = TestSessionContext.Current.Configuration.LocalEducationAgency;
            var userProfile = TestSessionContext.Current.UserProfiles[userProfileName];

            var restClient = InitializeRestClient(baseUrl, localEducationAgency, userProfile.Username, userProfile.Password);

            return restClient;
        }

        private static RestClient InitializeRestClient(string baseUrl, string localEducationAgency, string username, string password)
        {
            CookieContainer cookieContainer = new CookieContainer();

            RestClient client = new RestClient(baseUrl);
            client.AddHandler("application/json", new JsonDotNetSerializer());
            client.FollowRedirects = true;
            client.CookieContainer = cookieContainer;

            string entryUrl = Website.LocalEducationAgency.Entry(localEducationAgency)
                .Replace(baseUrl, string.Empty, StringComparison.InvariantCultureIgnoreCase);

            RestRequest request = new RestRequest(entryUrl);
            request.AddHeader("Accept", "text/html");

            var response = client.Execute(request);
            string content = response.Content;

            string relativePath = GetContextPath(response.ResponseUri);

            var loginResponse = ProcessAndSubmitForm(relativePath, content,
                cookieContainer, s =>
                    {
                        string fieldName = s.ToLower();

                        if (fieldName.Contains("username"))
                            return username;
                        
                        if (fieldName.Contains("password"))
                            return password;
                        
                        return null;
                    });

            var streamReader = new StreamReader(loginResponse.GetResponseStream());
            string html = streamReader.ReadToEnd();

            if (!html.Contains(@"name=""hiddenform"""))
                throw new Exception("Login attempt failed.");

            relativePath = GetContextPath(loginResponse.ResponseUri);

            CookieContainer siteCookieContainer = new CookieContainer();

            var mainSiteSubmissionResponse = ProcessAndSubmitForm(relativePath, html, siteCookieContainer, s => null, true);
            streamReader = new StreamReader(mainSiteSubmissionResponse.GetResponseStream());
            string html2 = streamReader.ReadToEnd();

            client.CookieContainer = siteCookieContainer;

            return client;
        }

        private static string GetContextPath(Uri absoluteUri)
        {
            int localPathPos = absoluteUri.AbsoluteUri.IndexOf(absoluteUri.LocalPath);

            string pagePath = absoluteUri.AbsoluteUri.Substring(0, localPathPos + absoluteUri.LocalPath.Length);

            int lastSlash = pagePath.LastIndexOf("/");

            string basePath = pagePath.Substring(0, lastSlash + 1);

            return basePath;
        }

        private static WebResponse ProcessAndSubmitForm(string relativeUrl, string content, CookieContainer cookieContainer, Func<string, string> getOverriddenValue, bool lookForFedAuthCookie = false)
        {
            var formFields = Regex.Matches(content, @"<form(\s+(?<attr_name>\w+)=""(?<attr_value>[^""]*)"")+");
            var formAttributesByName = GetAttributeValuesByName(formFields[0]);
            string postUrl = formAttributesByName["action"];

            string absolutePostUrl;

            if ((new Uri(postUrl, UriKind.RelativeOrAbsolute)).IsAbsoluteUri)
                absolutePostUrl = postUrl;
            else
                absolutePostUrl = relativeUrl.TrimEnd('/') + "/" + postUrl;

            var loginRequest = HttpWebRequest.Create(absolutePostUrl) as HttpWebRequest;
            loginRequest.ContentType = "application/x-www-form-urlencoded";
            loginRequest.Method = "POST";
            loginRequest.CookieContainer = cookieContainer;

            var loginRequestWriter = new StreamWriter(loginRequest.GetRequestStream());

            var inputFields = Regex.Matches(content, @"<input(\s+(?<attr_name>\w+)=""(?<attr_value>[^""]*)"")+");

            StringBuilder sb = new StringBuilder();

            bool firstParm = true;

            foreach (Match inputField in inputFields)
            {
                var attributeValuesByName = GetAttributeValuesByName(inputField);

                if (!attributeValuesByName.ContainsKey("value")
                    && attributeValuesByName["type"] != "text"
                    && attributeValuesByName["type"] != "password"
                    && attributeValuesByName["type"] != "hidden")
                    continue;

                if (!attributeValuesByName.ContainsKey("name"))
                    continue;

                if (!firstParm)
                    sb.Append("&");

                string name = attributeValuesByName["name"];
                string value = null;

                if (attributeValuesByName.ContainsKey("value"))
                {
                    value = HttpUtility.HtmlDecode(attributeValuesByName["value"]);
                }

                string newValue = getOverriddenValue(name);

                if (newValue != null)
                    value = newValue;

                sb.Append(name);
                sb.Append("=");
                sb.Append(HttpUtility.UrlEncode(value));

                firstParm = false;
            }

            loginRequestWriter.Write(sb.ToString());
            loginRequestWriter.Flush();

            if (lookForFedAuthCookie)
            {
                // Don't follow redirects automatically because remote HttpOnly, secure cookies require special handling
                loginRequest.AllowAutoRedirect = false;

                var loginResponse = loginRequest.GetResponse() as HttpWebResponse;

                // Find the FedAuth cookie header (if it's present)
                string fedAuthCookie =
                    (from h in loginResponse.Headers.GetValues("Set-Cookie")
                     where h.StartsWith("FedAuth")
                     select h)
                        .SingleOrDefault();

                // Process the fedAuthCookie
                if (fedAuthCookie != null)
                    ProcessFedAuthCookie(cookieContainer, absolutePostUrl, fedAuthCookie);

                return loginResponse;
            }
            else
            {
                var loginResponse = loginRequest.GetResponse();
                
                return loginResponse;
            }
        }

        private static void ProcessFedAuthCookie(CookieContainer cookieContainer, string absolutePostUrl,
                                                            string fedAuthCookie)
        {
            // Look for existing FedAuth cookie
            var storedCookies = cookieContainer.GetCookies(new Uri(absolutePostUrl));

            // Don't do anything if we already have a FedAuth cookie processed
            if (storedCookies["FedAuth"] != null)
                return;

            string[] cookieParts = fedAuthCookie.Split(';');

            string fedAuthCookieValue = null;
            string path = null;

            foreach (var cookiePart in cookieParts)
            {
                string[] nameValuePairs = cookiePart.Split('=');

                switch (nameValuePairs[0].ToLower().Trim())
                {
                    case "fedauth":
                        fedAuthCookieValue = nameValuePairs[1].Trim();
                        break;
                    case "path":
                        path = nameValuePairs[1].Trim();
                        break;
                }
            }

            string domainName = new Uri(absolutePostUrl).Host;

            cookieContainer.Add(new Cookie("FedAuth", fedAuthCookieValue, path, domainName));
        }

        private static Dictionary<string, string> GetAttributeValuesByName(Match inputField)
        {
            var names = inputField.Groups["attr_name"].Captures;
            var values = inputField.Groups["attr_value"].Captures;

            var valuesByName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < names.Count; i++)
                valuesByName[names[i].Value] = values[i].Value;

            return valuesByName;
        }
    }
}