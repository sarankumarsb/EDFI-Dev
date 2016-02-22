using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using System.Web;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
    public class WctxWimpProvider : IWimpProvider
    {
        protected IHttpRequestProvider httpRequestProvider;

        public WctxWimpProvider(IHttpRequestProvider httpRequestProvider)
        {
            this.httpRequestProvider = httpRequestProvider;
        }

        public virtual string GetWimp()
        {
            var wctx = httpRequestProvider.GetValue("wctx");
            var values = HttpUtility.ParseQueryString(wctx);
            var ru = values["ru"];
            if (ru == null)
                return null;
            var model = SignInRequestAdornUtility.FromUrlQuery(ru);
            return model.Wimp;
        }
    }
}