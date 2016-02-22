using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Resources;

namespace EdFi.Dashboards.Metric.Resources.Services.Data
{
    public interface IMetricComponentsService : IService<MetricDataRequest, IEnumerable<MetricComponent>> { }

    public class MetricComponentsService : IMetricComponentsService
    {
        private readonly IRepository<MetricComponent> metricComponentRepository;

        public MetricComponentsService(IRepository<MetricComponent> metricComponentRepository)
        {
            this.metricComponentRepository = metricComponentRepository;
        }

        //[CacheNoCopy(cacheOriginalInstanceOnly: true)]
        [CacheBehavior(copyOnSet: false, copyOnGet: false)]
        public IEnumerable<MetricComponent> Get(MetricDataRequest request)
        {
            var metricInstanceSetKey = request.MetricInstanceSetKey;

            return
                (from m in metricComponentRepository.GetAll()
                 where m.MetricInstanceSetKey == metricInstanceSetKey
                 select m)
                 .ToList();
        }
    }
}
