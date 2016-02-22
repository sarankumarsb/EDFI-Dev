using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Warehouse.Data.Entities;

namespace EdFi.Dashboards.Warehouse.Resources.School.Detail
{
    public class PriorYearStudentMetricsProviderQueryOptions
    {
        /// <summary>
        /// Filters to just the specified school.  This will get ignored if the LocalEducationAgencyId is set.
        /// </summary>
        public int SchoolId { get; set; }

        /// <summary>
        /// Filters to just the specified LocalEducationAgency.  If it's null, SchoolId will be set, and will be used for filter.  If it's not null,
        /// SchoolId will be ignored, and only this will be used.
        /// </summary>
        public int? LocalEducationAgencyId { get; set; }

        /// <summary>
        /// Filters to just the specified year. 
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Filters down the results to only the specified studentIds.  Optional for GetOrderedStudentList, required for GetStudentsWithMetrics.
        /// </summary>
        public IEnumerable<long> StudentIds { get; set; }

        /// <summary>
        /// When calling GetStudentsWithMetrics, this filters the metrics down to the specified list.  If this list is not included, GetStudentsWithMetrics will
        /// return 0 results, unless you set GetAllMetrics to true, and then this will return all metrics for the specified students.
        /// </summary>
        public IEnumerable<int> MetricVariantIds { get; set; }

        /// <summary>
        /// Optional.  Used for doing drilldowns from school metrics to students.  Checks to ensure the studentschool exists in
        /// SchoolMetricStudentList.  Set to null to get all students.  This should be the MetricId of the School level metric.
        /// </summary>
        public int? SchoolMetricStudentListMetricId { get; set; }
    }

    public interface IPriorYearStudentMetricsProvider
    {
        IQueryable<SchoolMetricInstanceStudentList> GetStudentList(PriorYearStudentMetricsProviderQueryOptions providerQueryOptions, MetadataColumn sortColumn = null, string sortDirection = "");
        IQueryable<StudentSchoolMetricInstance> GetStudentsWithMetrics(PriorYearStudentMetricsProviderQueryOptions providerQueryOptions);
    }

    public class PriorYearStudentMetricsProvider : IPriorYearStudentMetricsProvider
    {
        protected readonly IRepository<StudentSchoolMetricInstance> studentSchoolMetricInstanceRepository;
        protected readonly IStateAssessmentMetricIdGroupingProvider stateAssessmentMetricIdGroupingProvider;
        protected readonly ICurrentUserClaimInterrogator currentUserClaimInterrogator;
        protected readonly IRepository<SchoolMetricInstanceStudentList> schoolMetricStudentListRepository;

        public PriorYearStudentMetricsProvider(
            IRepository<StudentSchoolMetricInstance> studentSchoolMetricInstanceRepository,
            IStateAssessmentMetricIdGroupingProvider stateAssessmentMetricIdGroupingProvider,
            ICurrentUserClaimInterrogator currentUserClaimInterrogator,
            IRepository<SchoolMetricInstanceStudentList> schoolMetricStudentListRepository)
        {
            this.studentSchoolMetricInstanceRepository = studentSchoolMetricInstanceRepository;
            this.stateAssessmentMetricIdGroupingProvider = stateAssessmentMetricIdGroupingProvider;
            this.currentUserClaimInterrogator = currentUserClaimInterrogator;
            this.schoolMetricStudentListRepository = schoolMetricStudentListRepository;
        }

        public IQueryable<SchoolMetricInstanceStudentList> GetStudentList(PriorYearStudentMetricsProviderQueryOptions providerQueryOptions, MetadataColumn sortColumn = null, string sortDirection = "")
        {
            var query = ApplyQueryOptionsAndSecurity(schoolMetricStudentListRepository.GetAll(), providerQueryOptions);
            return query;
        }

        public IQueryable<StudentSchoolMetricInstance> GetStudentsWithMetrics(PriorYearStudentMetricsProviderQueryOptions providerQueryOptions)
        {
            var studentThatWillBeShown = ApplyQueryOptionsAndSecurity(schoolMetricStudentListRepository.GetAll(), providerQueryOptions);
            var query = from studentMetrics in studentSchoolMetricInstanceRepository.GetAll()
                        join esi in studentThatWillBeShown on new { studentMetrics.StudentUSI, studentMetrics.SchoolId, studentMetrics.SchoolYear } equals new { esi.StudentUSI, esi.SchoolId, esi.SchoolYear }
                        select studentMetrics;

            return FilterByMetrics(query, providerQueryOptions);
        }

        protected virtual IQueryable<SchoolMetricInstanceStudentList> ApplyQueryOptionsAndSecurity(IQueryable<SchoolMetricInstanceStudentList> query, PriorYearStudentMetricsProviderQueryOptions providerQueryOptions)
        {
            query = ApplySecurityFilter(query);
            query = ApplyLocalEducationAgencyIdAndSchoolIdFilter(query, providerQueryOptions);
            query = ApplySchoolMetricStudentListMetricIdFilter(query, providerQueryOptions);
            query = ApplyStudentIdsFilter(query, providerQueryOptions);

            return query;
        }

        protected virtual IQueryable<SchoolMetricInstanceStudentList> ApplyLocalEducationAgencyIdAndSchoolIdFilter(IQueryable<SchoolMetricInstanceStudentList> query,
                                                                        PriorYearStudentMetricsProviderQueryOptions providerQueryOptions)
        {
            if (providerQueryOptions.LocalEducationAgencyId.HasValue)
            {
                return query.Where(student => student.SchoolId.ToString().StartsWith(providerQueryOptions.LocalEducationAgencyId.Value.ToString()));
            }
            return query.Where(student => student.SchoolId == providerQueryOptions.SchoolId);
        }

        protected virtual IQueryable<SchoolMetricInstanceStudentList> ApplySchoolMetricStudentListMetricIdFilter(IQueryable<SchoolMetricInstanceStudentList> query,
                                                                                PriorYearStudentMetricsProviderQueryOptions
                                                                                    providerQueryOptions)
        {
            if (!providerQueryOptions.SchoolMetricStudentListMetricId.HasValue)
                return query;

            return query.Where(student => student.SchoolYear == providerQueryOptions.Year &&
                                            student.MetricId == providerQueryOptions.SchoolMetricStudentListMetricId);
        }

        protected virtual IQueryable<SchoolMetricInstanceStudentList> ApplyStudentIdsFilter(IQueryable<SchoolMetricInstanceStudentList> query,
                                                        PriorYearStudentMetricsProviderQueryOptions providerQueryOptions)
        {
            if (providerQueryOptions.StudentIds == null)
                return query;

            var enumerableStudentIds = providerQueryOptions.StudentIds as long[] ??
                                       providerQueryOptions.StudentIds.ToArray();

            if (!enumerableStudentIds.Any())
                return query;

            return query.Where(metrics => enumerableStudentIds.Contains(metrics.StudentUSI));
        }

        /// <summary>
        /// This security logic is here because we need to handle security correctly.  if the interceptors filter out students later on in
        /// the process, it will mess up paging.  So this makes sure all the students are visible to the user.
        /// </summary>
        /// <returns></returns>
        protected virtual IQueryable<SchoolMetricInstanceStudentList> ApplySecurityFilter(IQueryable<SchoolMetricInstanceStudentList> query)
        {
            //The only user that can see everyone are state employees...
            if (currentUserClaimInterrogator.HasClaimForStateAgency(EdFiClaimTypes.ViewAllStudents))
                return query;

            //Start off with a lit of empty students we're allowed to see...
            var studentsTheUserCanSee = ExpressionExtensions.False<SchoolMetricInstanceStudentList>();

            if (UserInformation.Current == null)
                return query.Where(studentsTheUserCanSee);

            //Walk through all their associated ed-orgs...
            foreach (var associatedOrganization in UserInformation.Current.AssociatedOrganizations)
            {
                //Find what they have permissions to, and let them see just those students.
                foreach (var claimType in associatedOrganization.ClaimTypes)
                {
                    int educationOrganizationId = associatedOrganization.EducationOrganizationId;

                    switch (claimType)
                    {

                        //People with ViewAllStudents can see any user associated with their ed-org
                        case EdFiClaimTypes.ViewAllStudents:
                            studentsTheUserCanSee = studentsTheUserCanSee.Or(
                                    student => student.SchoolId.ToString().StartsWith(educationOrganizationId.ToString()));
                            break;
                        //People with ViewMyStudents can see their students, or students that are part of their cohort.
                        case EdFiClaimTypes.ViewMyStudents:
                            //Logically, the goal of this code is to do studentsTheUserCanSee.Or(student => myStudents.Contains(student.StudentUSI)), but we have
                            //   to hack around subsonic, so this is phrased oddly.
                            // TODO: Link to myStudents

                            //var myStudents =
                            //    staffStudentAssociationRepository.GetAll().Where(ssa => ssa.StaffUSI == staffUsi && ssa.SchoolId == educationOrganizationId);
                            //studentsTheUserCanSee = studentsTheUserCanSee.Or(student => myStudents.Any(ms => ms.StudentUSI == student.StudentUSI));

                            break;
                    }
                }
            }

            return query.Where(studentsTheUserCanSee);
        }

        protected virtual IQueryable<StudentSchoolMetricInstance> FilterByMetrics(IQueryable<StudentSchoolMetricInstance> query, PriorYearStudentMetricsProviderQueryOptions providerQueryOptions)
        {
            if (providerQueryOptions.MetricVariantIds != null)
            {
                var enumerableMetricVariantIds = providerQueryOptions.MetricVariantIds as int[] ??
                                                 providerQueryOptions.MetricVariantIds.ToArray();

                var groupingIds = stateAssessmentMetricIdGroupingProvider.GetMetricVariantGroupIds();
                enumerableMetricVariantIds = enumerableMetricVariantIds.Concat(groupingIds).ToArray();

                if (enumerableMetricVariantIds.Any())
                    query = query.Where(metrics => enumerableMetricVariantIds.Contains(metrics.MetricId));
            }
            return query;
        }

        protected virtual IList<StudentWithMetrics> GradeLevelOrder(IList<StudentWithMetrics> query,
                                                             string sortDirection)
        {
            query = query.Distinct().ToList();

            query = sortDirection == "desc"
                ? query.OrderByDescending(entity => entity.GradeLevel)
                    .ThenByDescending(entity => entity.Name).ToList()
                : query.OrderBy(entity => entity.GradeLevel)
                    .ThenBy(entity => entity.Name).ToList();

            return query;
        }
    }
}