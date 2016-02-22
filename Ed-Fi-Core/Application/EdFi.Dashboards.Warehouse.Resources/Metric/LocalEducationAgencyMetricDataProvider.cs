using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Warehouse.Resource.Models.Metric;
using EdFi.Dashboards.Warehouse.Resources.Application;

namespace EdFi.Dashboards.Warehouse.Resources.Metric
{
    public class LocalEducationAgencyMetricDataProvider : IMetricDataProvider<LocalEducationAgencyMetricInstanceSetRequest>
    {
        private readonly ILocalEducationAgencyMetricDataService localEducationAgencyMetricDataService;
        private readonly IMetricInstanceSetKeyResolver<LocalEducationAgencyMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        private readonly IWarehouseAvailabilityProvider warehouseAvailabilityProvider;
        private readonly IMaxPriorYearProvider maxPriorYearProvider;

        public LocalEducationAgencyMetricDataProvider(ILocalEducationAgencyMetricDataService localEducationAgencyMetricDataService,
                                                     IMetricInstanceSetKeyResolver<LocalEducationAgencyMetricInstanceSetRequest> metricInstanceSetKeyResolver,
                                                     IWarehouseAvailabilityProvider warehouseAvailabilityProvider,
                                                     IMaxPriorYearProvider maxPriorYearProvider)
        {
            this.localEducationAgencyMetricDataService = localEducationAgencyMetricDataService;
            this.metricInstanceSetKeyResolver = metricInstanceSetKeyResolver;
            this.warehouseAvailabilityProvider = warehouseAvailabilityProvider;
            this.maxPriorYearProvider = maxPriorYearProvider;
        }

        [NoCache]
        public bool CanProvideData(LocalEducationAgencyMetricInstanceSetRequest request)
        {
            return request != null;
        }

        [CacheBehavior(copyOnSet: false, copyOnGet: false)]
        public MetricData Get(LocalEducationAgencyMetricInstanceSetRequest request)
        {
            var result = new PriorYearMetricData();
            if (!warehouseAvailabilityProvider.Get())
            {
                result.InitializeEmptyCollections();
                return result;
            }

            var metricInstanceSetKey = metricInstanceSetKeyResolver.GetMetricInstanceSetKey(request);
            var year = maxPriorYearProvider.Get(request.LocalEducationAgencyId);

            return localEducationAgencyMetricDataService.Get(LocalEducationAgencyMetricDataRequest.Create(request.LocalEducationAgencyId, metricInstanceSetKey, year));
        }
    }
}
