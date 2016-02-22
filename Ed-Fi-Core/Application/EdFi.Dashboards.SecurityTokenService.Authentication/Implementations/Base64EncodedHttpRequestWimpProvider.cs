using System;
using System.Text;
using EdFi.Dashboards.Infrastructure;

namespace EdFi.Dashboards.SecurityTokenService.Authentication.Implementations
{
    public class Base64EncodedHttpRequestWimpProvider : HttpRequestWimpProvider
    {
        public Base64EncodedHttpRequestWimpProvider(IHttpRequestProvider httpRequestProvider)
            : base(httpRequestProvider)
        {

        }

        public override string GetWimp()
        {
            var rawWimp = base.GetWimp();
            if (string.IsNullOrWhiteSpace(rawWimp))
                return null;

            // Decode the email address from request, for impersonation
            byte[] bytes = Convert.FromBase64String(rawWimp);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}