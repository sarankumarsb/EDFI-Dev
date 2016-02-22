// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Common;

namespace EdFi.Dashboards.Resources.Metric
{
    public interface IDomainMetricService<in TRequest>
        where TRequest : MetricInstanceSetRequestBase, new()
    {
        [ApplyMetricSecurity]
        MetricTree Get(TRequest request);
    }

    public class DomainMetricService<TRequest> : IDomainMetricService<TRequest>
        where TRequest : MetricInstanceSetRequestBase, new()
    {
        private readonly IMetricService<TRequest> metricService;

        public DomainMetricService(IMetricService<TRequest> metricService)
        {
            this.metricService = metricService;
        }

        public MetricTree Get(TRequest request)
        {
            return new MetricTree(metricService.Get(request));
        }
    }
}
