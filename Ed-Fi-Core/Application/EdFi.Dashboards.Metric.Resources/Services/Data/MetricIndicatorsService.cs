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
    public interface IMetricIndicatorsService : IService<MetricDataRequest, IEnumerable<MetricIndicator>> { }

    public class MetricIndicatorsService : IMetricIndicatorsService
    {
        private readonly IRepository<MetricIndicator> metricIndicatorRepository;

        public MetricIndicatorsService(IRepository<MetricIndicator> metricIndicatorRepository)
        {
            this.metricIndicatorRepository = metricIndicatorRepository;
        }

        //[CacheNoCopy(cacheOriginalInstanceOnly: true)]
        [CacheBehavior(copyOnSet: false, copyOnGet: false)]
        public IEnumerable<MetricIndicator> Get(MetricDataRequest request)
        {
            var metricInstanceSetKey = request.MetricInstanceSetKey;

            return
                (from m in metricIndicatorRepository.GetAll()
                 where m.MetricInstanceSetKey == metricInstanceSetKey
                 select m)
                 .ToList();
        }
    }
}
