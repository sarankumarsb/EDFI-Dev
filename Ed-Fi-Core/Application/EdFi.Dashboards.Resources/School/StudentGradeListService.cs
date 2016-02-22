using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Support;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Resources.StudentMetrics;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.School
{
    public class StudentGradeListRequest : IEdFiGridMetaRequestConvertable
    {
        public int SchoolId { get; set; }
		[AuthenticationIgnore("Could be added later, but user probably should have to see ALL students for this school")]
        public string Grade { get; set; }
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

        public static StudentGradeListRequest Create(int schoolId, string grade, int pageNumber, int pageSize, int? sortColumn, string sortDirection, string visibleColumns, string uniqueListId)
        {
            return new StudentGradeListRequest
            {
                SchoolId = schoolId,
                Grade = grade,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                VisibleColumns = visibleColumns,
                UniqueListId = uniqueListId
            };
        }

        public EdFiGridMetaRequest ConvertToGridMetaRequest()
        {
            return new EdFiGridMetaRequest
            {
                SchoolId = SchoolId,
                StaffUSI = null,
                SectionOrCohortId = null,
                StudentListType = null,
                Grade = Grade,
                GridListType = ListType.StudentGrade
            };
        }
    }

    public interface IStudentGradeListService : IService<StudentGradeListRequest, StudentGradeListModel> { }

    public class StudentGradeListService : IStudentGradeListService
    {
        private readonly IStudentSchoolAreaLinks studentSchoolLinks;
        private readonly ISchoolCategoryProvider schoolCategoryProvider;
        private readonly IMetadataListIdResolver metadataListIdResolver;
        private readonly IListMetadataProvider listMetadataProvider;
        private readonly IClassroomMetricsProvider classroomMetricsProvider;
        private readonly IStudentMetricsProvider studentGradeListPagingProvider;
        private readonly IAccommodationProvider accommodationProvider;
        private readonly IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;

        public StudentGradeListService(IStudentSchoolAreaLinks studentSchoolLinks, 
                                             ISchoolCategoryProvider schoolCategoryProvider, 
                                             IMetadataListIdResolver metadataListIdResolver, 
                                             IListMetadataProvider listMetadataProvider, 
                                             IClassroomMetricsProvider classroomMetricsProvider,
                                             IStudentMetricsProvider studentGradeListPagingProvider,
                                             IAccommodationProvider accommodationProvider,
                                             IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider)
        {
            this.studentSchoolLinks = studentSchoolLinks;
            this.schoolCategoryProvider = schoolCategoryProvider;
            this.metadataListIdResolver = metadataListIdResolver;
            this.listMetadataProvider = listMetadataProvider;
            this.classroomMetricsProvider = classroomMetricsProvider;
            this.studentGradeListPagingProvider = studentGradeListPagingProvider;
            this.accommodationProvider = accommodationProvider;
            this.gradeLevelUtilitiesProvider = gradeLevelUtilitiesProvider;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public StudentGradeListModel Get(StudentGradeListRequest request)
        {
            var model = new StudentGradeListModel();
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
            var resolvedListId = metadataListIdResolver.GetListId(ListType.StudentGrade, schoolCategory);
            model.ListMetadata = listMetadataProvider.GetListMetadata(resolvedListId);

            string[] gradeLevel = null;
            if (!string.IsNullOrWhiteSpace(request.Grade))
            {
                gradeLevel = new[]
                {
                    request.Grade
                };
            }

            var queryOptions = new StudentMetricsProviderQueryOptions
            {
                LocalEducationAgencyId = null, 
                MetricVariantIds = GetMetricVariantIds(model.ListMetadata),
                SchoolId = request.SchoolId,
                GradeLevel = gradeLevel,
            };
            var students = GetStudentList(request, queryOptions, model).ToList();
            queryOptions.StudentIds = students.Select(s => s.StudentUSI).ToList();
            var metrics = studentGradeListPagingProvider.GetStudentsWithMetrics(queryOptions).ToList();

            //Student Unique Field Newly Added : Saravanan
            foreach (var student in students)
            {
                var studentMetrics = metrics.Where(x => x.StudentUSI == student.StudentUSI).ToList();
                model.Students.Add(
                    new StudentWithMetricsAndAccommodations(student.StudentUSI)
                    {
                        SchoolId = student.SchoolId,
                        Name = Utilities.FormatPersonNameByLastName(student.FirstName, student.MiddleName, student.LastSurname),
                        ThumbNail = studentSchoolLinks.Image(student.SchoolId, student.StudentUSI, student.Gender, student.FullName).Resolve(),
                        Href = new Link { Rel = "student", Href = studentSchoolLinks.Overview(student.SchoolId, student.StudentUSI, student.FullName).AppendParameters("listContext=" + request.UniqueListId).Resolve() },
                    GradeLevel = gradeLevelUtilitiesProvider.FormatGradeLevelForSorting(student.GradeLevel),
                    GradeLevelDisplayValue = gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay(student.GradeLevel),
                        Metrics = classroomMetricsProvider.GetAdditionalMetrics(studentMetrics, model.ListMetadata),
                        StudentUniqueID  = student.StudentUniqueID
                    });
            }

            OverlayStudentAccommodation(model.Students, request.SchoolId);

            return model;
        }

        private static IEnumerable<int> GetMetricVariantIds(IEnumerable<MetadataColumnGroup> listMetadata)
        {
            var metricVariantIds = new List<int>();
            foreach (var @group in listMetadata.Where(@group => @group.GroupType != GroupType.EntityInformation))
            {
                metricVariantIds.AddRange(@group.Columns.Select(column => column.MetricVariantId));
            }
            return metricVariantIds;
        }

        private IEnumerable<EnhancedStudentInformation> GetStudentList(StudentGradeListRequest request, StudentMetricsProviderQueryOptions providerQueryOptions, StudentGradeListModel model)
        {
            var sortColumn = model.ListMetadata.GetSortColumn(request.SortColumn);
            var students = studentGradeListPagingProvider.GetOrderedStudentList(providerQueryOptions, sortColumn, request.SortDirection).ToList();

            model.EntityIds = students.Select(entity =>
                                new StudentSchoolIdentifier
                                {
                                    SchoolId = entity.SchoolId,
                                    StudentUSI = entity.StudentUSI
                                }).ToList();

            if (!model.EntityIds.Any())
                return new List<EnhancedStudentInformation>();
            // If Page Size > 100 then the user requested all records
            return request.PageSize > 100
                ? students
                : students.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
        }

        protected void OverlayStudentAccommodation(List<StudentWithMetricsAndAccommodations> students, int schoolId)
        {
            var studentIds = students.Select(x => x.StudentUSI).ToArray();
            if (studentIds.Any())
            {
                var allAccommodationsForStudents = accommodationProvider.GetAccommodations(studentIds, schoolId);
                if (allAccommodationsForStudents != null)
                    foreach (var sa in allAccommodationsForStudents)
                    {
                        students.Where(x => x.StudentUSI == sa.StudentUSI).ToList().ForEach(y => y.Accommodations = sa.AccommodationsList);
                    }
            }
        }
    }
}
