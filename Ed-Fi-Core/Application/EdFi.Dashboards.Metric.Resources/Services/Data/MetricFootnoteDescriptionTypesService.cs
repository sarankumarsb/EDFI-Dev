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
    public class MetricFootnoteDescriptionTypesRequest
    {
        public static MetricFootnoteDescriptionTypesRequest Create()
        {
            return new MetricFootnoteDescriptionTypesRequest();
        }
    }

    public interface IMetricFootnoteDescriptionTypesService : IService<MetricFootnoteDescriptionTypesRequest, IEnumerable<MetricFootnoteDescriptionType>> { }

    public class MetricFootnoteDescriptionTypesService : IMetricFootnoteDescriptionTypesService
    {
        private readonly IRepository<MetricFootnoteDescriptionType> metricFootnoteDescriptionTypeRepository;

        public MetricFootnoteDescriptionTypesService(IRepository<MetricFootnoteDescriptionType> metricFootnoteDescriptionTypeRepository)
        {
            this.metricFootnoteDescriptionTypeRepository = metricFootnoteDescriptionTypeRepository;
        }

        //[CacheNoCopy(cacheOriginalInstanceOnly: true)]
        [CacheBehavior(copyOnSet: false, copyOnGet: false)]
        public IEnumerable<MetricFootnoteDescriptionType> Get(MetricFootnoteDescriptionTypesRequest request)
        {
            return metricFootnoteDescriptionTypeRepository.GetAll().ToList();
        }
    }
}
