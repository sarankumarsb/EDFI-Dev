using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services.Data;

namespace EdFi.Dashboards.Metric.Resources.Services
{
    public class MetricDataRequest
    {
        public Guid MetricInstanceSetKey { get; set; }

        public static MetricDataRequest Create(Guid metricInstanceSetKey)
        {
            return new MetricDataRequest {MetricInstanceSetKey = metricInstanceSetKey};
        }
    }

    public class CurrentYearMetricDataProvider<T> : IMetricDataProvider<T>
        where T : MetricInstanceSetRequestBase
    {
        private readonly IMetricInstanceSetKeyResolver<T> metricInstanceSetKeyResolver;
        private readonly IMetricInstancesService metricInstancesService;
        private readonly IMetricInstanceExtendedPropertiesService metricInstanceExtendedPropertiesService;
        private readonly IMetricComponentsService metricComponentsService;
        private readonly IMetricGoalsService metricGoalsService;
        private readonly IMetricIndicatorsService metricIndicatorsService;
        private readonly IMetricInstanceFootnotesService metricInstanceFootnotesService;
        private readonly IMetricFootnoteDescriptionTypesService metricFootnoteDescriptionTypesService;

        public CurrentYearMetricDataProvider(IMetricInstanceSetKeyResolver<T> metricInstanceSetKeyResolver, 
                                                IMetricInstancesService metricInstancesService, 
                                                IMetricInstanceExtendedPropertiesService metricInstanceExtendedPropertiesService,
                                                IMetricComponentsService metricComponentsService, 
                                                IMetricGoalsService metricGoalsService, 
                                                IMetricIndicatorsService metricIndicatorsService,
                                                IMetricInstanceFootnotesService metricInstanceFootnotesService, 
                                                IMetricFootnoteDescriptionTypesService metricFootnoteDescriptionTypesService)
        {
            this.metricInstanceSetKeyResolver = metricInstanceSetKeyResolver;
            this.metricInstancesService = metricInstancesService;
            this.metricInstanceExtendedPropertiesService = metricInstanceExtendedPropertiesService;
            this.metricComponentsService = metricComponentsService;
            this.metricGoalsService = metricGoalsService;
            this.metricIndicatorsService = metricIndicatorsService;
            this.metricInstanceFootnotesService = metricInstanceFootnotesService;
            this.metricFootnoteDescriptionTypesService = metricFootnoteDescriptionTypesService;
        }

        [NoCache]
        public bool CanProvideData(T request)
        {
            return true;
        }

        [NoCache] // All the sub-data is cached and expired individually.
        public MetricData Get(T request)
        {
            Guid metricInstanceSetKey = metricInstanceSetKeyResolver.GetMetricInstanceSetKey(request);
            var metricDataRequest = MetricDataRequest.Create(metricInstanceSetKey);

            var returnMetricData = new CurrentYearMetricData
            {
				// Single value per metric
                MetricInstances = metricInstancesService.Get(metricDataRequest),
                MetricGoals = metricGoalsService.Get(metricDataRequest),
                MetricIndicators = metricIndicatorsService.Get(metricDataRequest),

				// Multiple values per metric
                MetricInstanceExtendedProperties = metricInstanceExtendedPropertiesService.Get(metricDataRequest),
                MetricComponents = metricComponentsService.Get(metricDataRequest),
                MetricInstanceFootnotes = metricInstanceFootnotesService.Get(metricDataRequest),

				// General types (lookup)
                MetricFootnoteDescriptionTypes = metricFootnoteDescriptionTypesService.Get(MetricFootnoteDescriptionTypesRequest.Create()),
            };

            return returnMetricData;
        }
    }
}
