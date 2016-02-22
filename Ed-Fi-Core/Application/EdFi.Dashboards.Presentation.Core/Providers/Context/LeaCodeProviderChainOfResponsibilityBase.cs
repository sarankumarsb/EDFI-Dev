using System;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Presentation.Architecture.Mvc.ValueProviders;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Infrastructure;

namespace EdFi.Dashboards.Presentation.Core.Providers.Context
{
    public abstract class LeaCodeProviderChainOfResponsibilityBase :
        ChainOfResponsibilityBase<ILocalEducationAgencyContextProvider, LeaCodeRequest, string>,
        ILocalEducationAgencyContextProvider
    {
        protected IIdCodeService idCodeService;
        protected IHttpRequestProvider httpRequestProvider;

        protected LeaCodeProviderChainOfResponsibilityBase(IIdCodeService idCodeService, IHttpRequestProvider httpRequestProvider, ILocalEducationAgencyContextProvider next)
            : base(next)
        {
            this.idCodeService = idCodeService;
            this.httpRequestProvider = httpRequestProvider;
        }

        protected override bool CanHandleRequest(LeaCodeRequest request)
        {
            return !string.IsNullOrEmpty(this.GetLeaCode(request));
        }

        protected override string HandleRequest(LeaCodeRequest request)
        {
            var code = this.GetLeaCode(request);
            var result = this.idCodeService.Get(IdCodeRequest.Create(code: code));
            if (result != null)
                return code;
            throw new LocalEducationAgencyNotFoundException("The local education agency " + code + " could not be found for request to " + this.httpRequestProvider.Url);
        }

        public string GetCurrentLocalEducationAgencyCode()
        {
            return this.ProcessRequest(new LeaCodeRequest());
        }

        public abstract string GetLeaCode(LeaCodeRequest request);
    }

    #region LeaCodeRequest
    public class LeaCodeRequest { }
    #endregion LeaCodeRequest

    #region NullLeaCodeProvider
    public class NullLeaCodeProvider : ILocalEducationAgencyContextProvider
    {
        public string GetCurrentLocalEducationAgencyCode()
        {
            throw new LocalEducationAgencyNotFoundException("The local education agency could not be found.");
        }
    }
    #endregion //NullLeaCodeProvider
}
