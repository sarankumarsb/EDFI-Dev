using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Resources.StudentMetrics;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.School
{
    public class StudentDemographicListRequest
    {
        public int SchoolId { get; set; }
        [AuthenticationIgnore("Could be added later, but user probably should have to see ALL students for this school")]
        public string Demographic { get; set; }
        [AuthenticationIgnore("Server side paging metadata")]
        public int PageNumber { get; set; }
        [AuthenticationIgnore("Server side paging metadata")]
        public int PageSize { get; set; }
        [AuthenticationIgnore("Server side paging metadata")]
        public int? SortColumn { get; set; }
        [AuthenticationIgnore("Server side paging metadata")]
        public string SortDirection { get; set; }
        [AuthenticationIgnore("Server side paging metadata")]
        public string VisibleColumns { get; set; }
        [AuthenticationIgnore("Server side paging metadata")]
        public string UniqueListId { get; set; }

        public static StudentDemographicListRequest Create(int schoolId, string demographic, int pageNumber, int pageSize, int? sortColumn, string sortDirection, string visibleColumns, string uniqueListId)
        {
            return new StudentDemographicListRequest
            {
                SchoolId = schoolId,
                Demographic = demographic,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                VisibleColumns = visibleColumns,
                UniqueListId = uniqueListId
            };
        }
    }

    public interface IStudentDemographicListService : IService<StudentDemographicListRequest, StudentDemographicListModel> { }

    public class StudentDemographicListService : IStudentDemographicListService
    {
        private readonly IStudentSchoolAreaLinks studentSchoolLinks;
        private readonly ISchoolCategoryProvider schoolCategoryProvider;
        private readonly IMetadataListIdResolver metadataListIdResolver;
        private readonly IListMetadataProvider listMetadataProvider;
        private readonly IClassroomMetricsProvider classroomMetricsProvider;
        private readonly IStudentMetricsProvider studentListPagingProvider;
        private readonly IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;

        public StudentDemographicListService(IStudentSchoolAreaLinks studentSchoolLinks, 
                                             ISchoolCategoryProvider schoolCategoryProvider, 
                                             IMetadataListIdResolver metadataListIdResolver, 
                                             IListMetadataProvider listMetadataProvider, 
                                             IClassroomMetricsProvider classroomMetricsProvider,
                                             IStudentMetricsProvider studentListPagingProvider,
                                             IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider)
        {
            this.studentSchoolLinks = studentSchoolLinks;
            this.schoolCategoryProvider = schoolCategoryProvider;
            this.metadataListIdResolver = metadataListIdResolver;
            this.listMetadataProvider = listMetadataProvider;
            this.classroomMetricsProvider = classroomMetricsProvider;
            this.studentListPagingProvider = studentListPagingProvider;
            this.gradeLevelUtilitiesProvider = gradeLevelUtilitiesProvider;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public StudentDemographicListModel Get(StudentDemographicListRequest request)
        {
            var model = new StudentDemographicListModel();
            var schoolCategory = schoolCategoryProvider.GetSchoolCategoryType(request.SchoolId);

            switch (schoolCategory)
            {
                case SchoolCategory.Elementary:
                case SchoolCategory.MiddleSchool:
                    break;
                default:
                    schoolCategory = SchoolCategory.HighSchool;
                    break;
            }

            //Setting the metadata.
            var resolvedListId = metadataListIdResolver.GetListId(ListType.StudentDemographic, schoolCategory);
            model.ListMetadata = listMetadataProvider.GetListMetadata(resolvedListId);

            SelectionOptionGroup[] demographicOptionGroups = null;

            if (!string.IsNullOrWhiteSpace(request.Demographic))
            {
                demographicOptionGroups = new []
                {
                    new SelectionOptionGroup
                    {
                        SelectedOptions = new []
                        {
                            request.Demographic
                        }
                    }
                };
            }

            var queryOptions = new StudentMetricsProviderQueryOptions
            {
                SchoolId = request.SchoolId,
                DemographicOptionGroups = demographicOptionGroups
            };

            IEnumerable<EnhancedStudentInformation> students;

            var sortColumn = model.ListMetadata.GetSortColumn(request.SortColumn);
            students = studentListPagingProvider.GetOrderedStudentList(queryOptions, sortColumn, request.SortDirection).ToList();

            model.EntityIds = students.Select(x => new StudentSchoolIdentifier { StudentUSI = x.StudentUSI, SchoolId = x.SchoolId }).ToList();

            if (model.EntityIds.Count == 0)
                return model;

            // If Page Size > 100 then the user requested all records
            students = request.PageSize > 100 ? students : students.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);

            queryOptions.StudentIds = students.Select(s => s.StudentUSI);

            var metrics = studentListPagingProvider.GetStudentsWithMetrics(queryOptions);
            
            //Student Unique Field Newly Added : Saravanan
            //foreach (var student in studentListPagingProvider.GetPageData(StudentListDemographicPageRequest.Create(request.LocalEducationAgencyId, null, request.Demographic, studentIdFilter, model.ListMetadata, request.SortColumn, request.SortDirection, request.VisibleColumns)))
            foreach (var student in students)
            {
                var studentMetrics = metrics.Where(m => m.StudentUSI == student.StudentUSI).ToList();

                var studentModel = new StudentWithMetricsAndPrimaryMetric(student.StudentUSI)
                {
                    SchoolId = student.SchoolId,
                    Name = Utilities.FormatPersonNameByLastName(student.FirstName,
                                                                student.MiddleName,
                                                                student.LastSurname),
                    GradeLevel = gradeLevelUtilitiesProvider.FormatGradeLevelForSorting(student.GradeLevel),
                    GradeLevelDisplayValue = gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay(student.GradeLevel),
                    Href = new Link
                    {
                        Rel = "student",
                        Href = studentSchoolLinks.Overview(student.SchoolId,
                                                            student.StudentUSI,
                                                            student.FullName,
                                                            new { listContext = request.UniqueListId })
                    },
                    PrimaryMetricValue = request.Demographic,
                    PrimaryMetricDisplayValue = request.Demographic,
                    Metrics = classroomMetricsProvider.GetAdditionalMetrics(studentMetrics, model.ListMetadata),
                    StudentUniqueID = student.StudentUniqueID
                };

                model.Students.Add(studentModel);
            }

            return model;
        }
    }
}
