using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.Requests;

namespace EdFi.Dashboards.Resources.Navigation.MetricRouteProviders
{
    public class LocalEducationAgencyMetricActionRouteProvider : ChainOfResponsibilityBase<IMetricActionRouteProvider, MetricActionRouteProviderChainRequest, string>, IMetricActionRouteProvider
    {
        private readonly ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks;

        public LocalEducationAgencyMetricActionRouteProvider(IMetricActionRouteProvider next, ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks)
            : base(next)
        {
            this.localEducationAgencyAreaLinks = localEducationAgencyAreaLinks;
        }

        public string GetRoute(MetricInstanceSetRequestBase metricInstanceSetRequest, MetricAction action)
        {
            return base.ProcessRequest(MetricActionRouteProviderChainRequest.Create(metricInstanceSetRequest, action));
        }

        protected override bool CanHandleRequest(MetricActionRouteProviderChainRequest request)
        {
            return request.MetricInstanceSetRequest is LocalEducationAgencyMetricInstanceSetRequest;
        }
        protected override string HandleRequest(MetricActionRouteProviderChainRequest request)
        {
            var metricInstanceSetRequest = request.MetricInstanceSetRequest as LocalEducationAgencyMetricInstanceSetRequest;
            MetricAction action = request.Action;
            string resourceName = LegacyRouteUtility.GetRouteResourceName(action.Url);

            string url = localEducationAgencyAreaLinks.MetricsDrilldown(metricInstanceSetRequest.LocalEducationAgencyId, action.MetricVariantId, resourceName);

            return url;
        }
    }
}
