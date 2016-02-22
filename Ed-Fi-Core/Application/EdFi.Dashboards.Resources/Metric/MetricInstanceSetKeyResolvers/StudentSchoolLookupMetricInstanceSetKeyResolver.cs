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
    public class StudentSchoolLookupMetricInstanceSetKeyResolver
        : IMetricInstanceSetKeyResolver<StudentSchoolMetricInstanceSetRequest>
    {
        private readonly IRepository<StudentSchoolMetricInstanceSet> studentSchoolMetricInstanceSetRepository;

        public StudentSchoolLookupMetricInstanceSetKeyResolver(IRepository<StudentSchoolMetricInstanceSet> studentSchoolMetricInstanceSetRepository)
        {
            this.studentSchoolMetricInstanceSetRepository = studentSchoolMetricInstanceSetRepository;
        }

        public Guid GetMetricInstanceSetKey(StudentSchoolMetricInstanceSetRequest metricInstanceSetRequestBase)
        {
            var studentSchoolMetricInstanceSetData = studentSchoolMetricInstanceSetRepository.GetAll().SingleOrDefault(x => x.StudentUSI == metricInstanceSetRequestBase.StudentUSI && x.SchoolId == metricInstanceSetRequestBase.SchoolId);

            if (studentSchoolMetricInstanceSetData == null)
                throw new KeyNotFoundException("Student Key not found for student USI:" + metricInstanceSetRequestBase.StudentUSI + " school Id:" + metricInstanceSetRequestBase.SchoolId);

            return studentSchoolMetricInstanceSetData.MetricInstanceSetKey;
        }
    }
}