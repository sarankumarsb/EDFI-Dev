using System.Linq;
using EdFi.Dashboards.Data.Entities;

namespace EdFi.Dashboards.Resources.StudentMetrics
{
    public class SchoolCategoryFilter : IStudentMetricFilter
    {
        public IQueryable<EnhancedStudentInformation> ApplyFilter(IQueryable<EnhancedStudentInformation> query,
            StudentMetricsProviderQueryOptions providerQueryOptions)
        {
            if (providerQueryOptions.SchoolCategory == null)
                return query;

            return query.Where(student => student.SchoolCategory.Contains(providerQueryOptions.SchoolCategory));
        }
    }
}