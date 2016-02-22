// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Metric.Requests;

namespace EdFi.Dashboards.Resources.Metric.MetricInstanceSetKeyResolvers
{
    public class LocalEducationAgencyLookupMetricInstanceSetKeyResolver
        : IMetricInstanceSetKeyResolver<LocalEducationAgencyMetricInstanceSetRequest>
    {
        private readonly IRepository<LocalEducationAgencyMetricInstanceSet> localEducationAgencyRepository;

        public LocalEducationAgencyLookupMetricInstanceSetKeyResolver(IRepository<LocalEducationAgencyMetricInstanceSet> localEducationAgencyRepository)
        {
            this.localEducationAgencyRepository = localEducationAgencyRepository;
        }

        public Guid GetMetricInstanceSetKey(LocalEducationAgencyMetricInstanceSetRequest metricInstanceSetRequestBase)
        {
            var localEducationAgencyData = localEducationAgencyRepository.GetAll().SingleOrDefault(x => x.LocalEducationAgencyId == metricInstanceSetRequestBase.LocalEducationAgencyId);

            if (localEducationAgencyData == null)
                throw new KeyNotFoundException("Local Education Agency Key not found for local Education Agency Id:" + metricInstanceSetRequestBase.LocalEducationAgencyId);

            return localEducationAgencyData.MetricInstanceSetKey;
        }
    }
}
