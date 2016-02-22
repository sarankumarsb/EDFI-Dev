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
    public interface IMetricInstanceFootnotesService : IService<MetricDataRequest, IEnumerable<MetricInstanceFootnote>> { }

    public class MetricInstanceFootnotesService : IMetricInstanceFootnotesService
    {
        private readonly IRepository<MetricInstanceFootnote> metricInstanceFootnoteRepository;

        public MetricInstanceFootnotesService(IRepository<MetricInstanceFootnote> metricInstanceFootnoteRepository)
        {
            this.metricInstanceFootnoteRepository = metricInstanceFootnoteRepository;
        }

        //[CacheNoCopy(cacheOriginalInstanceOnly: true)]
        [CacheBehavior(copyOnSet: false, copyOnGet: false)]
        public IEnumerable<MetricInstanceFootnote> Get(MetricDataRequest request)
        {
            var metricInstanceSetKey = request.MetricInstanceSetKey;

            return
                (from m in metricInstanceFootnoteRepository.GetAll()
                 where m.MetricInstanceSetKey == metricInstanceSetKey
                 select m)
                 .ToList();
        }
    }
}
