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
    public class SchoolMetricActionRouteProvider : ChainOfResponsibilityBase<IMetricActionRouteProvider, MetricActionRouteProviderChainRequest, string>, IMetricActionRouteProvider
    {
        private readonly IRootMetricNodeResolver rootMetricNodeResolver;
        private readonly ISchoolAreaLinks schoolLinks;
        private readonly IDomainSpecificMetricNodeResolver domainSpecificMetricNodeResolver;

        public SchoolMetricActionRouteProvider(IMetricActionRouteProvider next, IRootMetricNodeResolver rootMetricNodeResolver, ISchoolAreaLinks schoolLinks,
            IDomainSpecificMetricNodeResolver domainSpecificMetricNodeResolver) 
            : base(next)
        {
            this.rootMetricNodeResolver = rootMetricNodeResolver;
            this.schoolLinks = schoolLinks;
            this.domainSpecificMetricNodeResolver = domainSpecificMetricNodeResolver;
        }

        public string GetRoute(MetricInstanceSetRequestBase metricInstanceSetRequest, MetricAction action)
        {
            return base.ProcessRequest(MetricActionRouteProviderChainRequest.Create(metricInstanceSetRequest, action));
        }

        protected override bool CanHandleRequest(MetricActionRouteProviderChainRequest request)
        {
            return request.MetricInstanceSetRequest is SchoolMetricInstanceSetRequest;
        }

        protected override string HandleRequest(MetricActionRouteProviderChainRequest request)
        {
            var metricInstanceSetRequest = request.MetricInstanceSetRequest as SchoolMetricInstanceSetRequest;
            MetricAction action = request.Action;
            string resourceName = LegacyRouteUtility.GetRouteResourceName(action.Url);

            string url = schoolLinks.MetricsDrilldown(metricInstanceSetRequest.SchoolId, action.MetricVariantId, resourceName);

            return url;
        }
    }
}
