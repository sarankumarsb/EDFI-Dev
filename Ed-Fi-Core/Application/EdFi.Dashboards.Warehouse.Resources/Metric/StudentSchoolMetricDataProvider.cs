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
    public class StudentSchoolMetricDataProvider : IMetricDataProvider<StudentSchoolMetricInstanceSetRequest>
    {
        private readonly IStudentSchoolMetricDataService studentSchoolMetricDataService;
        private readonly IBriefService schoolBriefService;
        private readonly IMetricInstanceSetKeyResolver<StudentSchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        private readonly IWarehouseAvailabilityProvider warehouseAvailabilityProvider;
        private readonly IMaxPriorYearProvider maxPriorYearProvider;

        public StudentSchoolMetricDataProvider(IStudentSchoolMetricDataService studentSchoolMetricDataService,
                                              IBriefService schoolBriefService,
                                              IMetricInstanceSetKeyResolver<StudentSchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver,
                                              IWarehouseAvailabilityProvider warehouseAvailabilityProvider,
                                              IMaxPriorYearProvider maxPriorYearProvider)
        {
            this.studentSchoolMetricDataService = studentSchoolMetricDataService;
            this.schoolBriefService = schoolBriefService;
            this.metricInstanceSetKeyResolver = metricInstanceSetKeyResolver;
            this.warehouseAvailabilityProvider = warehouseAvailabilityProvider;
            this.maxPriorYearProvider = maxPriorYearProvider;
        }

        [NoCache]
        public bool CanProvideData(StudentSchoolMetricInstanceSetRequest request)
        {
            return request != null;
        }

        [CacheBehavior(copyOnSet: false, copyOnGet: false)]
        public MetricData Get(StudentSchoolMetricInstanceSetRequest request)
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

            return studentSchoolMetricDataService.Get(StudentSchoolMetricDataRequest.Create(request.StudentUSI, request.SchoolId, localEducationAgencyId, metricInstanceSetKey, year));
        }
    }
}
