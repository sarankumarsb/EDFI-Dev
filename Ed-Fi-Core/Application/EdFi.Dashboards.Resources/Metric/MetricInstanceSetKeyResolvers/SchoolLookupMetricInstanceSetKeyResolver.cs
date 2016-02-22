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
    public class SchoolLookupMetricInstanceSetKeyResolver
        : IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest>
    {
        private readonly IRepository<SchoolMetricInstanceSet> schoolMetricInstanceSetRepository;

        public SchoolLookupMetricInstanceSetKeyResolver(IRepository<SchoolMetricInstanceSet> schoolMetricInstanceSetRepository)
        {
            this.schoolMetricInstanceSetRepository = schoolMetricInstanceSetRepository;
        }

        public Guid GetMetricInstanceSetKey(SchoolMetricInstanceSetRequest metricInstanceSetRequestBase)
        {
            var schoolMetricInstanceSetData = schoolMetricInstanceSetRepository.GetAll().SingleOrDefault(x => x.SchoolId == metricInstanceSetRequestBase.SchoolId);

            if (schoolMetricInstanceSetData == null)
                throw new KeyNotFoundException("School Key not found for school Id:" + metricInstanceSetRequestBase.SchoolId);

            return schoolMetricInstanceSetData.MetricInstanceSetKey;
        }
    }
}