using EdFi.Dashboards.Infrastructure;

namespace EdFi.Dashboards.SecurityTokenService.Authentication.Implementations
{
    public class HttpRequestWimpProvider : IWimpProvider
    {
        protected IHttpRequestProvider httpRequestProvider;

        public HttpRequestWimpProvider(IHttpRequestProvider httpRequestProvider)
        {
            this.httpRequestProvider = httpRequestProvider;
        }

        public virtual string GetWimp()
        {
            return httpRequestProvider.GetValue("wimp");
        }
    }
}