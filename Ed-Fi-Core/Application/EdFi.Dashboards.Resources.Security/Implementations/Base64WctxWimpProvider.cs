using System;
using System.Text;
using EdFi.Dashboards.Infrastructure;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
    public class Base64WctxWimpProvider : WctxWimpProvider
    {
        public Base64WctxWimpProvider(IHttpRequestProvider httpRequestProvider)
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