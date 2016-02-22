using EdFi.Dashboards.Core;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Resources.LocalEducationAgency;

namespace EdFi.Dashboards.Presentation.Core.Providers.Context
{
    public class HttpContextItemsLeaCodeProvider : LeaCodeProviderChainOfResponsibilityBase
    {
        private readonly IHttpContextItemsProvider httpContextItemsProvider;
        private readonly string httpContextKey = "lea";

        public HttpContextItemsLeaCodeProvider(IIdCodeService idCodeService, IHttpRequestProvider httpRequestProvider,
            IHttpContextItemsProvider httpContextItemsProvider, ILocalEducationAgencyContextProvider next)
            : base(idCodeService, httpRequestProvider, next)
        {
            this.httpContextItemsProvider = httpContextItemsProvider;
        }

        public override string GetLeaCode(LeaCodeRequest request)
        {
            if (!this.httpContextItemsProvider.Contains(httpContextKey))
                return null;

            return this.httpContextItemsProvider.GetValue(httpContextKey) as string;
        }
    }
}
