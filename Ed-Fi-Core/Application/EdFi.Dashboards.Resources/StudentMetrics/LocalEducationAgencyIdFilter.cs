using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Resources.Common;

namespace EdFi.Dashboards.Resources.StudentMetrics
{
    public class LocalEducationAgencyIdFilter : IStudentMetricFilter
    {
        public IQueryable<EnhancedStudentInformation> ApplyFilter(IQueryable<EnhancedStudentInformation> query, StudentMetricsProviderQueryOptions providerQueryOptions)
        {
            if (!providerQueryOptions.LocalEducationAgencyId.HasUsableValue())
                return query;

            return query.Where(student => student.LocalEducationAgencyId == providerQueryOptions.LocalEducationAgencyId.Value);
        }
    }
}