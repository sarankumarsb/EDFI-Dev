using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Resources.Common;

namespace EdFi.Dashboards.Resources.StudentMetrics
{
    public class SchoolIdFilter : IStudentMetricFilter
    {
        public IQueryable<EnhancedStudentInformation> ApplyFilter(IQueryable<EnhancedStudentInformation> query, StudentMetricsProviderQueryOptions providerQueryOptions)
        {
            if (!providerQueryOptions.SchoolId.HasUsableValue())
                return query;

            return query.Where(student => student.SchoolId == providerQueryOptions.SchoolId.GetValueOrDefault());
        }
    }
}