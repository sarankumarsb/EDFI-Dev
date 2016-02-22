using System;
using System.Text.RegularExpressions;
using System.Web;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Presentation.Architecture.Providers;
using EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency;
using EdFi.Dashboards.Presentation.Web.Utilities;
using EdFi.Dashboards.Resources.LocalEducationAgency;

namespace EdFi.Dashboards.Presentation.Core.Providers.Context
{
    public class HttpRequestUrlLeaCodeProvider : LeaCodeProviderChainOfResponsibilityBase
    {
        private readonly IRequestUrlBaseProvider requestUrlBaseProvider;

        public HttpRequestUrlLeaCodeProvider(IIdCodeService idCodeService, IHttpRequestProvider httpRequestProvider, ILocalEducationAgencyContextProvider next, IRequestUrlBaseProvider requestUrlBaseProvider)
            : base(idCodeService, httpRequestProvider, next)
        {
            this.requestUrlBaseProvider = requestUrlBaseProvider;
        }

        public override string GetLeaCode(LeaCodeRequest request)
        {
            if (this.httpRequestProvider.Url == null)
                return null;
            var requestUrl = this.httpRequestProvider.Url.ToString();
            string requestBase = requestUrlBaseProvider.GetRequestUrlBase(HttpContext.Current.Request.RequestContext.HttpContext.Request);
            string requestRelativeUrl = requestUrl.Replace(requestBase, string.Empty);
            // Try to extract the LEA name from the root of the virtual path
            string code;
            if (TryGetCodeFromUrl(requestRelativeUrl, out code))
                return code;
            return null;
        }

        private static bool TryGetCodeFromUrl(string requestRelativeUrl, out string code)
        {
            code = null;

            var regex = String.Format("^{1}/(?<Code>{0}?)/", RoutingPatterns.LocalEducationAgency, LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix);
            var match = Regex.Match(requestRelativeUrl + "/", regex, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                code = match.Groups["Code"].Value;
                return true;
            }

            // Check for the WIF sign-in
            if (requestRelativeUrl.Contains("wa=wsignin1.0"))
            {
                var returnUrlMatch = Regex.Match(requestRelativeUrl, @"&ru=(?<ReturnUrl>[^&]*)&");

                if (returnUrlMatch.Success)
                {
                    string returnUrl = returnUrlMatch.Groups["ReturnUrl"].Value;
                    regex = String.Format("%252f(?<Code>{0})(%252f|&)", RoutingPatterns.LocalEducationAgency);
                    var codeMatch = Regex.Match(returnUrl, regex, RegexOptions.IgnoreCase);

                    if (codeMatch.Success)
                    {
                        code = codeMatch.Groups["Code"].Value;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
