// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services.Data;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources;

namespace EdFi.Dashboards.Metric.Resources.Services
{
    public interface IMetricDataProvider<TRequest>
        where TRequest : MetricInstanceSetRequestBase
    {
        bool CanProvideData(TRequest request);
        MetricData Get(TRequest request);
    }


    public interface IMetricDataService<TRequest> : IService<TRequest, MetricDataContainer>
        where TRequest : MetricInstanceSetRequestBase { }

    public class MetricDataService<TRequest> : IMetricDataService<TRequest>
        where TRequest : MetricInstanceSetRequestBase
    {
        private readonly IMetricDataProvider<TRequest>[] metricDataProviders;

        public MetricDataService(IMetricDataProvider<TRequest>[] metricDataProviders)
        {
            this.metricDataProviders = metricDataProviders;
        }

        [NoCache] // All the sub-data is cached and expired individually.
        public MetricDataContainer Get(TRequest request)
        {
            var metricData = metricDataProviders.Where(x => x.CanProvideData(request)).Select(x => x.Get(request)).ToList();
            return new MetricDataContainer(metricData);
        }
    }
}
