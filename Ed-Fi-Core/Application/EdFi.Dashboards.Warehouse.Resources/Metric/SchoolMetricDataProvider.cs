using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Warehouse.Resource.Models.Metric;
using EdFi.Dashboards.Warehouse.Resources.Application;

namespace EdFi.Dashboards.Warehouse.Resources.Metric
{
    public class SchoolMetricDataProvider : IMetricDataProvider<SchoolMetricInstanceSetRequest>
    {
        private readonly ISchoolMetricDataService schoolMetricDataService;
        private readonly IBriefService schoolBriefService;
        private readonly IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        private readonly IWarehouseAvailabilityProvider warehouseAvailabilityProvider;
        private readonly IMaxPriorYearProvider maxPriorYearProvider;

        public SchoolMetricDataProvider(ISchoolMetricDataService schoolMetricDataService,
                                       IBriefService schoolBriefService,
                                       IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver,
                                       IWarehouseAvailabilityProvider warehouseAvailabilityProvider,
                                       IMaxPriorYearProvider maxPriorYearProvider)
        {
            this.schoolMetricDataService = schoolMetricDataService;
            this.schoolBriefService = schoolBriefService;
            this.metricInstanceSetKeyResolver = metricInstanceSetKeyResolver;
            this.warehouseAvailabilityProvider = warehouseAvailabilityProvider;
            this.maxPriorYearProvider = maxPriorYearProvider;
        }

        [NoCache]
        public bool CanProvideData(SchoolMetricInstanceSetRequest request)
        {
            return request != null;
        }

        [CacheBehavior(copyOnSet: false, copyOnGet: false)]
        public MetricData Get(SchoolMetricInstanceSetRequest request)
        {
            var result = new PriorYearMetricData();

            if (!warehouseAvailabilityProvider.Get())
            {
                result.InitializeEmptyCollections();
                return result;
            }

            var metricInstanceSetKey = metricInstanceSetKeyResolver.GetMetricInstanceSetKey(request);

            var localEducationAgencyId = schoolBriefService.Get(BriefRequest.Create(request.SchoolId)).LocalEducationAgencyId;

            var year = maxPriorYearProvider.Get(localEducationAgencyId);

            return schoolMetricDataService.Get(SchoolMetricDataRequest.Create(request.SchoolId, metricInstanceSetKey, year));
        }
    }
}
