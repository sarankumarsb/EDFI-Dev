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
    public interface IMetricInstanceExtendedPropertiesService : IService<MetricDataRequest, IEnumerable<MetricInstanceExtendedProperty>> { }

    public class MetricInstanceExtendedPropertiesService : IMetricInstanceExtendedPropertiesService
    {
        private readonly IRepository<MetricInstanceExtendedProperty> metricInstanceExtendedPropertyRepository;

        public MetricInstanceExtendedPropertiesService(IRepository<MetricInstanceExtendedProperty> metricInstanceExtendedPropertyRepository)
        {
            this.metricInstanceExtendedPropertyRepository = metricInstanceExtendedPropertyRepository;
        }

        //[CacheNoCopy(cacheOriginalInstanceOnly: true)]
        [CacheBehavior(copyOnSet: false, copyOnGet: false)]
        public IEnumerable<MetricInstanceExtendedProperty> Get(MetricDataRequest request)
        {
            var metricInstanceSetKey = request.MetricInstanceSetKey;

            return
                (from m in metricInstanceExtendedPropertyRepository.GetAll()
                 where m.MetricInstanceSetKey == metricInstanceSetKey
                 select m)
                 .ToList();
        }
    }
}

