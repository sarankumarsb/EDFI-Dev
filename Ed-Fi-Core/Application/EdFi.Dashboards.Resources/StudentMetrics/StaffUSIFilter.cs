using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Common;

namespace EdFi.Dashboards.Resources.StudentMetrics
{
    public class StaffUSIFilter : IStudentMetricFilter
    {
        protected readonly IRepository<StaffStudentCohort> StaffStudentCohortRepository;
        protected readonly IRepository<StaffCohort> StaffCohortRepository;
        protected readonly IRepository<StaffStudentAssociation> StaffStudentAssociationRepository;

        public StaffUSIFilter(
            IRepository<StaffStudentCohort> staffStudentCohortRepository, 
            IRepository<StaffCohort> staffCohortRepository, 
            IRepository<StaffStudentAssociation> staffStudentAssociationRepository)
        {
            StaffStudentCohortRepository = staffStudentCohortRepository;
            StaffCohortRepository = staffCohortRepository;
            StaffStudentAssociationRepository = staffStudentAssociationRepository;
        }

        public IQueryable<EnhancedStudentInformation> ApplyFilter(IQueryable<EnhancedStudentInformation> query, StudentMetricsProviderQueryOptions providerQueryOptions)
        {
            //If the Staff USI Isn't set, then no-op.
            if (providerQueryOptions.StaffUSI <= 0)
                return query;

            //The only way a Staff member can be associated with a student at LEA is via the Staff Cohort tables.  Only check that one table.
            if (providerQueryOptions.LocalEducationAgencyId.HasUsableValue() && !providerQueryOptions.SchoolId.HasUsableValue())
            {
                var staffStudentCohorts = StaffStudentCohortRepository.GetAll();
                IQueryable<StaffCohort> staffCohort = StaffCohortRepository.GetAll().Where(sc => sc.StaffUSI == providerQueryOptions.StaffUSI);

                return query.Where(si => staffStudentCohorts
                    .SelectMany(ssc => staffCohort.Where(sc => ssc.StaffCohortId == sc.StaffCohortId).Select(sc => ssc))
                    .Any(ssc => ssc.StudentUSI == si.StudentUSI));
            }

            //At the school level, a staff member can be associated by Cohort or by Section.  This view collapses the two.
            var ssa = StaffStudentAssociationRepository.GetAll();
            return query.Where(student => ssa.Any(ms => ms.StudentUSI == student.StudentUSI
                                                        && ms.SchoolId == student.SchoolId
                //This filters on school id, because if a teacher is a teacher at two schools,
                //We need to filter on the Staff Section records only for the current school.
                                                        && ms.SchoolId == providerQueryOptions.SchoolId.GetValueOrDefault()
                                                        && ms.StaffUSI == providerQueryOptions.StaffUSI));
        }
    }
}