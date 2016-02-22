using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;

namespace EdFi.Dashboards.Resources.StudentMetrics
{
    public class MetricIdFilter : IStudentMetricFilter
    {
        protected readonly IRepository<SchoolMetricStudentList> SchoolMetricStudentListRepository;

        public MetricIdFilter(IRepository<SchoolMetricStudentList> schoolMetricStudentListRepository)
        {
            SchoolMetricStudentListRepository = schoolMetricStudentListRepository;
        }

        public IQueryable<EnhancedStudentInformation> ApplyFilter(IQueryable<EnhancedStudentInformation> query, StudentMetricsProviderQueryOptions providerQueryOptions)
        {
            if (!providerQueryOptions.SchoolMetricStudentListMetricId.HasValue)
                return query;

            var schoolMetricStudentList = SchoolMetricStudentListRepository.GetAll();
            return query.Where(student => schoolMetricStudentList.Any(smsl =>
                smsl.StudentUSI == student.StudentUSI
                && smsl.SchoolId == student.SchoolId
                && smsl.MetricId == providerQueryOptions.SchoolMetricStudentListMetricId.Value));
        }
    }
}