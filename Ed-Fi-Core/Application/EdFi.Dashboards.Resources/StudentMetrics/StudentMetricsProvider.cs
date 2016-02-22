using System.Collections.Generic;
using System.Linq;
using Castle.Core.Resource;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.CustomGrid;

namespace EdFi.Dashboards.Resources.StudentMetrics
{
    public class StudentMetricsProviderQueryOptions
    {
        public long[] TeacherSectionIds { get; set; }

        public long[] StaffCohortIds { get; set; }

        /// <summary>
        /// Filters to just the specified Staff member.  Logically this should be a parameter in the URL, not a pulled through the currently logged in user.
        /// Set to zero to select all students.
        /// </summary>
        public long StaffUSI { get; set; }

        /// <summary>
        /// Filters to just the specified school.
        /// </summary>
        public int? SchoolId { get; set; }
        
        /// <summary>
        /// Filters to just the specified LocalEducationAgency.
        /// </summary>
        public int? LocalEducationAgencyId { get; set; }
        
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
        /// If this is false, then it will only return the metrics specified in MetricVariantIds.  If this is set to true, it will return all Metric Variants
        /// for the specified students, ignoring MetricVariantIds
        /// </summary>
        public bool GetAllMetrics { get; set; }

        /// <summary>
        /// Optional.  Set to the School Category you want to filter, null to select all students.
        /// </summary>
        public string SchoolCategory { get; set; }

        /// <summary>
        /// Optional.  Used for doing drill-downs from school metrics to students.  Checks to ensure the studentSchool exists in
        /// SchoolMetricStudentList.  Set to null to get all students.  This should be the MetricId of the School level metric.
        /// </summary>
        public int? SchoolMetricStudentListMetricId { get; set; }

        /// <summary>
        /// Optional.  Filters to the specified grade levels.  Set to null or empty array to get all students.
        /// </summary>
        public string[] GradeLevel { get; set; }

        /// <summary>
        /// Filters by one or more demographic option groups.  Set to null or empty array to get all students, which is the default.
        /// If there are multiple option groups they will be anded together.
        /// </summary>
        public SelectionOptionGroup[] DemographicOptionGroups { get; set; }

        /// <summary>
        /// Gets or sets the metric option groups.
        /// </summary>
        /// <value>
        /// The metric option groups.
        /// </value>
        public MetricFilterOptionGroup[] MetricOptionGroups { get; set; }

        /// <summary>
        /// Gets or sets the assessment checkbox option groups.
        /// </summary>
        /// <value>
        /// The assessment checkbox option groups.
        /// </value>
        public MetricFilterOptionGroup[] AssessmentOptionGroups { get; set; }
    }

    /// <summary>
    /// A group of selected demographics that will be or'd together.
    /// </summary>
    public class SelectionOptionGroup
    {
        /// <summary>
        /// Gets or sets the name of the selection option.
        /// </summary>
        /// <value>
        /// The name of the selection option.
        /// </value>
        public string SelectionOptionName { get; set; }

        /// <summary>
        /// Gets or sets the selected options.
        /// </summary>
        /// <value>
        /// The selected options.
        /// </value>
        public string[] SelectedOptions { get; set; }
    }

    /// <summary>
    /// Contains a list of MetricFilterOption objects; this is used to create
    /// logical separations of options groups so they can be applied separately.
    /// </summary>
    public class MetricFilterOptionGroup
    {
        /// <summary>
        /// Gets or sets the metric filter options.
        /// </summary>
        /// <value>
        /// The metric filter options.
        /// </value>
        public MetricFilterOption[] MetricFilterOptions { get; set; }
    }

    /// <summary>
    /// Represents a filter to be applied to the data.
    /// </summary>
    public class MetricFilterOption
    {
        /// <summary>
        /// Gets or sets the name of the metric option.
        /// </summary>
        /// <value>
        /// The name of the metric option.
        /// </value>
        public int MetricId { get; set; }

        /// <summary>
        /// Gets or sets the minimum inclusive metric instance extended property.
        /// </summary>
        /// <value>
        /// The minimum inclusive metric instance extended property.
        /// </value>
        public string MinInclusiveMetricInstanceExtendedProperty { get; set; }

        /// <summary>
        /// Gets or sets the maximum exclusive metric instance extended property.
        /// </summary>
        /// <value>
        /// The maximum exclusive metric instance extended property.
        /// </value>
        public string MaxExclusiveMetricInstanceExtendedProperty { get; set; }

        /// <summary>
        /// Gets or sets the value the metric instance is equal to.
        /// </summary>
        /// <value>
        /// The metric instance equal to.
        /// </value>
        public string MetricInstanceEqualTo { get; set; }

        /// <summary>
        /// Gets or sets the value less than.
        /// </summary>
        /// <value>
        /// The value less than.
        /// </value>
        public double? ValueLessThan { get; set; }

        /// <summary>
        /// Gets or sets the value less than equal to.
        /// </summary>
        /// <value>
        /// The value less than or equal to.
        /// </value>
        public double? ValueLessThanEqualTo { get; set; }

        /// <summary>
        /// Gets or sets the value greater than.
        /// </summary>
        /// <value>
        /// The value greater than.
        /// </value>
        public double? ValueGreaterThan { get; set; }

        /// <summary>
        /// Gets or sets the value greater than or equal to.
        /// </summary>
        /// <value>
        /// The value greater than equal to.
        /// </value>
        public double? ValueGreaterThanEqualTo { get; set; }

        /// <summary>
        /// Gets or sets the metric state equal to.
        /// </summary>
        /// <value>
        /// The metric state equal to.
        /// </value>
        public int? MetricStateEqualTo { get; set; }

        /// <summary>
        /// Gets or sets the metric state not equal to.
        /// </summary>
        /// <value>
        /// The metric state not equal to.
        /// </value>
        public int? MetricStateNotEqualTo { get; set; }
    }

    public interface IStudentMetricsProvider
    {
        IQueryable<EnhancedStudentInformation> GetOrderedStudentList(
            StudentMetricsProviderQueryOptions providerQueryOptions, MetadataColumn sortColumn = null,
            string sortDirection = "", int? schoolMetricStudentListMetricId = null);

        IQueryable<StudentMetric> GetStudentsWithMetrics(StudentMetricsProviderQueryOptions providerQueryOptions);
    }

    public static class SpecialMetricVariantSortingIds
    {
        public const int Student = -3;
        public const int Designations = -4;
        public const int GradeLevel = -5;
        public const int School = -6;
        public const int SchoolMetricStudentList = -7;
    }

    public class StudentMetricsProvider : IStudentMetricsProvider
    {
        protected readonly IRepository<StudentAccommodationCountAndMaxValue> StudentAccommodationCountAndMaxValueRepository;
        protected readonly IRepository<EnhancedStudentInformation> EnhancedStudentInformationRepository;
        protected readonly IRepository<StudentMetric> StudentMetricRepository;
        protected readonly IRepository<SchoolMetricStudentList> SchoolMetricStudentListRepository;
        protected readonly IStateAssessmentMetricIdGroupingProvider StateAssessmentMetricIdGroupingProvider;
        protected readonly IStudentMetricFilter[] StudentMetricFilter;

        public StudentMetricsProvider(
            IRepository<StudentAccommodationCountAndMaxValue> studentAccommodationCountAndMaxValueRepository,
            IRepository<EnhancedStudentInformation> enhancedStudentInformationRepository,
            IRepository<StudentMetric> studentMetricRepository,
            IRepository<SchoolMetricStudentList> schoolMetricStudentListRepository,
            IStateAssessmentMetricIdGroupingProvider stateAssessmentMetricIdGroupingProvider,
            IStudentMetricFilter[] studentMetricFilter)
        {
            StudentAccommodationCountAndMaxValueRepository = studentAccommodationCountAndMaxValueRepository;
            EnhancedStudentInformationRepository = enhancedStudentInformationRepository;
            StudentMetricRepository = studentMetricRepository;
            SchoolMetricStudentListRepository = schoolMetricStudentListRepository;
            StateAssessmentMetricIdGroupingProvider = stateAssessmentMetricIdGroupingProvider;
            StudentMetricFilter = studentMetricFilter;
        }

        public IQueryable<EnhancedStudentInformation> GetOrderedStudentList(
            StudentMetricsProviderQueryOptions providerQueryOptions, MetadataColumn sortColumn = null,
            string sortDirection = "", int? schoolMetricStudentListMetricId = null)
        {
            var query = ApplyQueryOptionsAndSecurity(EnhancedStudentInformationRepository.GetAll(), providerQueryOptions);
            return SortStudents(query, sortColumn, sortDirection, schoolMetricStudentListMetricId);
        }

        public IQueryable<StudentMetric> GetStudentsWithMetrics(StudentMetricsProviderQueryOptions providerQueryOptions)
        {
            var studentThatWillBeShown = ApplyQueryOptionsAndSecurity(EnhancedStudentInformationRepository.GetAll(),
                providerQueryOptions);
            var query = from studentMetrics in StudentMetricRepository.GetAll()
                join esi in studentThatWillBeShown on new {studentMetrics.StudentUSI, studentMetrics.SchoolId} equals
                    new {esi.StudentUSI, esi.SchoolId}
                select studentMetrics;

            return FilterByMetrics(query, providerQueryOptions);
        }

        /// <summary>
        /// Applies the query options and security.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="providerQueryOptions">The provider query options.</param>
        /// <returns></returns>
        protected virtual IQueryable<EnhancedStudentInformation> ApplyQueryOptionsAndSecurity(
            IQueryable<EnhancedStudentInformation> query, StudentMetricsProviderQueryOptions providerQueryOptions)
        {
            foreach (var filter in StudentMetricFilter)
            {
                query = filter.ApplyFilter(query, providerQueryOptions);
            }
            
            return query;
        }
        
        protected virtual IQueryable<StudentMetric> FilterByMetrics(IQueryable<StudentMetric> query, StudentMetricsProviderQueryOptions providerQueryOptions)
        {
            // in cases where there have been no students specified, should return no metrics unless GetAllMetrics option is used
            //   This is used to prevent accidentally selecting the whole database.  There is not currently a use case where you need to
            //   select all metrics, you only need to select the metrics for the current page of users.
            if (!providerQueryOptions.GetAllMetrics &&
                (providerQueryOptions.StudentIds == null || !providerQueryOptions.StudentIds.Any()))
                query = query.Where(metrics => false);

            if (providerQueryOptions.MetricVariantIds != null)
            {
                var enumerableMetricVariantIds = providerQueryOptions.MetricVariantIds as int[] ??
                                                 providerQueryOptions.MetricVariantIds.ToArray();

                var groupingIds = StateAssessmentMetricIdGroupingProvider.GetMetricVariantGroupIds();
                enumerableMetricVariantIds = enumerableMetricVariantIds.Concat(groupingIds).ToArray();

                if (enumerableMetricVariantIds.Any())
                    query = query.Where(metrics => enumerableMetricVariantIds.Contains(metrics.MetricVariantId));
            }
            return query;
        }

        //TODO: All of this sorting code should be in it's own set of classes.  There should be some kind of factory that
        //  selects the correct sorting system.  This was not done due to time constraints.
        protected virtual IQueryable<EnhancedStudentInformation> SortStudents(
            IQueryable<EnhancedStudentInformation> query, MetadataColumn sortColumn, string sortDirection, int? schoolMetricStudentListMetricId)
        {
            if (sortColumn == null)
                return DefaultOrder(query, sortDirection);
            switch (sortColumn.MetricVariantId)
            {
                case SpecialMetricVariantSortingIds.Designations:
                    return DesignationOrder(query, sortDirection);
                case SpecialMetricVariantSortingIds.GradeLevel:
                    return GradeLevelOrder(query, sortDirection);
                case SpecialMetricVariantSortingIds.SchoolMetricStudentList:
                    if (schoolMetricStudentListMetricId == null) break;
                    return SchoolMetricStudentOrder(query, sortDirection, schoolMetricStudentListMetricId.Value);
                default:
                    if (sortColumn.MetricVariantId > 0)
                    {
                        return MetricValueOrder(query, sortColumn.MetricVariantId, sortDirection);
                    }
                    break;
            }
            return DefaultOrder(query, sortDirection);
        }

        protected virtual IQueryable<EnhancedStudentInformation> DesignationOrder(
            IQueryable<EnhancedStudentInformation> query,
            string sortDirection)
        {
            var sortQuery = from enhancedStudentInformation in query
                from accomodationCountAndMaxValue in
                    StudentAccommodationCountAndMaxValueRepository.GetAll()
                        .Where(
                            value =>
                                value.StudentUSI == enhancedStudentInformation.StudentUSI &&
                                value.SchoolId == enhancedStudentInformation.SchoolId)
                        .DefaultIfEmpty()
                select new {metrics = enhancedStudentInformation, accomodationCountAndMaxValue};
            sortQuery = sortQuery.Distinct();
            sortQuery = sortDirection == "desc"
                ? sortQuery.OrderByDescending(entity => entity.accomodationCountAndMaxValue.AccomodationCount)
                    .ThenByDescending(entity => entity.accomodationCountAndMaxValue.MaxAccomodationValue)
                    .ThenByDescending(entity => entity.metrics.LastSurname)
                    .ThenByDescending(entity => entity.metrics.FirstName)
                    .ThenByDescending(entity => entity.metrics.MiddleName)
                : sortQuery.OrderBy(entity => entity.accomodationCountAndMaxValue.AccomodationCount)
                    .ThenBy(entity => entity.accomodationCountAndMaxValue.MaxAccomodationValue)
                    .ThenBy(entity => entity.metrics.LastSurname)
                    .ThenBy(entity => entity.metrics.FirstName)
                    .ThenBy(entity => entity.metrics.MiddleName);
            return sortQuery.Select(data => data.metrics);
        }

        protected virtual IQueryable<EnhancedStudentInformation> MetricValueOrder(
            IQueryable<EnhancedStudentInformation> query,
            int metricVariantId, string sortDirection)
        {
            var sortQuery = (from student in query
                from metric in
                    StudentMetricRepository.GetAll()
                        .Where(
                            m =>
                                m.StudentUSI == student.StudentUSI && m.SchoolId == student.SchoolId &&
                                m.MetricVariantId == metricVariantId)
                        .DefaultIfEmpty()
                select new
                {
                    student,
                    metric.ValueSortOrder
                }).Distinct();

            sortQuery = sortDirection == "desc"
                ? sortQuery.OrderByDescending(entity => entity.ValueSortOrder)
                    .ThenByDescending(entity => entity.student.LastSurname)
                    .ThenByDescending(entity => entity.student.FirstName)
                    .ThenByDescending(entity => entity.student.MiddleName)
                : sortQuery.OrderBy(entity => entity.ValueSortOrder)
                    .ThenBy(entity => entity.student.LastSurname)
                    .ThenBy(entity => entity.student.FirstName)
                    .ThenBy(entity => entity.student.MiddleName);

            return sortQuery.Select(data => data.student);
        }

        protected virtual IQueryable<EnhancedStudentInformation> GradeLevelOrder(
            IQueryable<EnhancedStudentInformation> query,
            string sortDirection)
        {
            query = query.Distinct();

            query = sortDirection == "desc"
                ? query.OrderByDescending(entity => entity.GradeLevelSortOrder)
                    .ThenByDescending(entity => entity.LastSurname)
                    .ThenByDescending(entity => entity.FirstName)
                    .ThenByDescending(entity => entity.MiddleName)
                : query.OrderBy(entity => entity.GradeLevelSortOrder)
                    .ThenBy(entity => entity.LastSurname)
                    .ThenBy(entity => entity.FirstName)
                    .ThenBy(entity => entity.MiddleName);

            return query;
        }

        protected virtual IQueryable<EnhancedStudentInformation> SchoolMetricStudentOrder(
            IQueryable<EnhancedStudentInformation> query,
            string sortDirection, int schoolMetricStudentListMetricId)
        {
            var sortQuery = from enhancedStudentInformation in query
                            from schoolMetricStudentListValue in
                                SchoolMetricStudentListRepository.GetAll()
                                    .Where(
                                        value =>
                                            value.StudentUSI == enhancedStudentInformation.StudentUSI &&
                                            value.SchoolId == enhancedStudentInformation.SchoolId &&
                                            value.MetricId == schoolMetricStudentListMetricId)
                                    .DefaultIfEmpty()
                            select new { metrics = enhancedStudentInformation, schoolMetricStudentListValue };
            sortQuery = sortQuery.Distinct();
            sortQuery = sortDirection == "desc"
                ? sortQuery.OrderByDescending(entity => entity.schoolMetricStudentListValue.Value)
                    .ThenByDescending(entity => entity.metrics.LastSurname)
                    .ThenByDescending(entity => entity.metrics.FirstName)
                    .ThenByDescending(entity => entity.metrics.MiddleName)
                : sortQuery.OrderBy(entity => entity.schoolMetricStudentListValue.Value)
                    .ThenBy(entity => entity.metrics.LastSurname)
                    .ThenBy(entity => entity.metrics.FirstName)
                    .ThenBy(entity => entity.metrics.MiddleName);
            return sortQuery.Select(data => data.metrics);
        }

        protected virtual IQueryable<EnhancedStudentInformation> DefaultOrder(
            IQueryable<EnhancedStudentInformation> query, string sortDirection)
        {
            query = query.Distinct();

            query = sortDirection == "desc"
                ? query.OrderByDescending(entity => entity.LastSurname)
                    .ThenByDescending(entity => entity.FirstName)
                    .ThenByDescending(entity => entity.MiddleName)
                : query.OrderBy(entity => entity.LastSurname)
                    .ThenBy(entity => entity.FirstName)
                    .ThenBy(entity => entity.MiddleName);

            return query;
        }

    }
}
