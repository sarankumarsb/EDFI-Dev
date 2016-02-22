using System.Linq;
using EdFi.Dashboards.Data.Entities;

namespace EdFi.Dashboards.Resources.StudentMetrics
{
    public interface IStudentMetricFilter
    {
        IQueryable<EnhancedStudentInformation> ApplyFilter(IQueryable<EnhancedStudentInformation> query,
            StudentMetricsProviderQueryOptions providerQueryOptions);
    }
}