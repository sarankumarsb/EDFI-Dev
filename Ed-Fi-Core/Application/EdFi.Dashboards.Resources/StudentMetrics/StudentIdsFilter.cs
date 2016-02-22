using System.Linq;
using EdFi.Dashboards.Data.Entities;

namespace EdFi.Dashboards.Resources.StudentMetrics
{
    public class StudentIdsFilter : IStudentMetricFilter
    {
        public IQueryable<EnhancedStudentInformation> ApplyFilter(IQueryable<EnhancedStudentInformation> query, StudentMetricsProviderQueryOptions providerQueryOptions)
        {
            if (providerQueryOptions.StudentIds == null)
                return query;

            var enumerableStudentIds = providerQueryOptions.StudentIds as long[] ??
                                       providerQueryOptions.StudentIds.ToArray();

            if (!enumerableStudentIds.Any())
                return query;

            return query.Where(metrics => enumerableStudentIds.Contains(metrics.StudentUSI));
        }
    }
}