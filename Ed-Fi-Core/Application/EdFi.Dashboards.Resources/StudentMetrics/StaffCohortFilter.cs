using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;

namespace EdFi.Dashboards.Resources.StudentMetrics
{
    public class StaffCohortFilter : IStudentMetricFilter
    {
        protected readonly IRepository<StaffStudentCohort> StaffStudentCohortRepository;

        public StaffCohortFilter(IRepository<StaffStudentCohort> staffStudentCohortRepository)
        {
            StaffStudentCohortRepository = staffStudentCohortRepository;
        }

        public IQueryable<EnhancedStudentInformation> ApplyFilter(IQueryable<EnhancedStudentInformation> query, StudentMetricsProviderQueryOptions providerQueryOptions)
        {
            if (providerQueryOptions.StaffCohortIds == null || !providerQueryOptions.StaffCohortIds.Any())
                return query;

            return from studentItemWithMetrics in query
                from staffStudentCohort in
                    StaffStudentCohortRepository.GetAll()
                        .Where(cohort => cohort.StudentUSI == studentItemWithMetrics.StudentUSI)
                where providerQueryOptions.StaffCohortIds.Contains(staffStudentCohort.StaffCohortId)
                select studentItemWithMetrics;
        }
    }
}