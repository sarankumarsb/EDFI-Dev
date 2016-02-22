using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Resources.LocalEducationAgency;

namespace EdFi.Dashboards.Presentation.Core.Providers.Context
{
    public class DashboardContextLeaCodeProvider : LeaCodeProviderChainOfResponsibilityBase
    {
        public DashboardContextLeaCodeProvider(IIdCodeService idCodeService, IHttpRequestProvider httpRequestProvider, ILocalEducationAgencyContextProvider next)
            : base(idCodeService, httpRequestProvider, next) { }

        public override string GetLeaCode(LeaCodeRequest request)
        {
            if (EdFiDashboardContext.Current == null)
                return null;
            return EdFiDashboardContext.Current.LocalEducationAgency;
        }
    }
}
