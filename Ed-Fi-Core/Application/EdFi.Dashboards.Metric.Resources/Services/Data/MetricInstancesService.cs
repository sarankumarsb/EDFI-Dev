using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Resources;

namespace EdFi.Dashboards.Metric.Resources.Services.Data
{
    public interface IMetricInstancesService : IService<MetricDataRequest, IEnumerable<MetricInstance>> { }

    public class MetricInstancesService : IMetricInstancesService
    {
        private readonly IRepository<MetricInstance> metricInstanceRepository;

        public MetricInstancesService(IRepository<MetricInstance> metricInstanceRepository)
        {
            this.metricInstanceRepository = metricInstanceRepository;
        }

        [CacheBehavior(copyOnSet: false, copyOnGet: false)]
        public IEnumerable<MetricInstance> Get(MetricDataRequest request)
        {
            var metricInstanceSetKey = request.MetricInstanceSetKey;

            return
                (from m in metricInstanceRepository.GetAll()
                 where m.MetricInstanceSetKey == metricInstanceSetKey
                 select m)
                    .ToList();
        }
    }
}
