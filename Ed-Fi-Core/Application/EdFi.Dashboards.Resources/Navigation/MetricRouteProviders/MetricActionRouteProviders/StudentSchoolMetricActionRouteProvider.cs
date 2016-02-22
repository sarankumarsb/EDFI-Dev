using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.Requests;

namespace EdFi.Dashboards.Resources.Navigation.MetricRouteProviders
{
    public class StudentSchoolMetricActionRouteProvider : ChainOfResponsibilityBase<IMetricActionRouteProvider, MetricActionRouteProviderChainRequest, string>, IMetricActionRouteProvider
    {
        private readonly IRootMetricNodeResolver rootMetricNodeResolver;
        private readonly IStudentSchoolAreaLinks studentSchoolLinks;

        public StudentSchoolMetricActionRouteProvider(IMetricActionRouteProvider next, IRootMetricNodeResolver rootMetricNodeResolver, IStudentSchoolAreaLinks studentSchoolLinks) 
            : base(next)
        {
            this.rootMetricNodeResolver = rootMetricNodeResolver;
            this.studentSchoolLinks = studentSchoolLinks;
        }

        public string GetRoute(MetricInstanceSetRequestBase metricInstanceSetRequest, MetricAction action)
        {
            return base.ProcessRequest(MetricActionRouteProviderChainRequest.Create(metricInstanceSetRequest, action));
        }

        protected override bool CanHandleRequest(MetricActionRouteProviderChainRequest request)
        {
            return request.MetricInstanceSetRequest is StudentSchoolMetricInstanceSetRequest;
        }

        protected override string HandleRequest(MetricActionRouteProviderChainRequest request)
        {
            var metricInstanceSetRequest = request.MetricInstanceSetRequest as StudentSchoolMetricInstanceSetRequest;
            MetricAction action = request.Action;
            string resourceName = LegacyRouteUtility.GetRouteResourceName(action.Url);

            string url = studentSchoolLinks.MetricsDrilldown(metricInstanceSetRequest.SchoolId, metricInstanceSetRequest.StudentUSI, action.MetricVariantId, resourceName);

            return url;
        }
    }
}
