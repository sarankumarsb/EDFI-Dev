using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Resources.StudentMetrics;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.LocalEducationAgency
{
    public class StudentSchoolCategoryListRequest
    {
        public int LocalEducationAgencyId { get; set; }
        [AuthenticationIgnore("Could be added later, but user probably should have to see ALL students for this school")]
        public string SchoolCategory { get; set; }
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

        public static StudentSchoolCategoryListRequest Create(int localEducationAgencyId, string schoolCategory, int pageNumber, int pageSize, int? sortColumn, string sortDirection, string visibleColumns, string uniqueListId)
        {
            return new StudentSchoolCategoryListRequest
                       {
                           LocalEducationAgencyId = localEducationAgencyId,
                           SchoolCategory = schoolCategory,
                           PageNumber = pageNumber,
                           PageSize = pageSize,
                           SortColumn = sortColumn,
                           SortDirection = sortDirection,
                           VisibleColumns = visibleColumns,
                           UniqueListId = uniqueListId
                       };
        }
    }

    public interface IStudentSchoolCategoryListService : IService<StudentSchoolCategoryListRequest, StudentSchoolCategoryListModel> { }

    public class StudentSchoolCategoryListService : IStudentSchoolCategoryListService
    {
        private readonly IMetadataListIdResolver metadataListIdResolver;
        private readonly IListMetadataProvider listMetadataProvider;
        private readonly IClassroomMetricsProvider classroomMetricsProvider;
        private readonly IStudentSchoolAreaLinks studentSchoolLinks;
        private readonly IStudentMetricsProvider studentListByCategoryProvider;
        private readonly ISchoolCategoryProvider schoolCategoryProvider;
        private readonly IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;

        public StudentSchoolCategoryListService(IMetadataListIdResolver metadataListIdResolver, 
                                             IListMetadataProvider listMetadataProvider, 
                                             IClassroomMetricsProvider classroomMetricsProvider, 
                                             IStudentSchoolAreaLinks studentSchoolLinks,
                                             IStudentMetricsProvider studentListByCategoryProvider,
                                             ISchoolCategoryProvider schoolCategoryProvider,
                                             IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider)
        {
            this.metadataListIdResolver = metadataListIdResolver;
            this.listMetadataProvider = listMetadataProvider;
            this.classroomMetricsProvider = classroomMetricsProvider;
            this.studentSchoolLinks = studentSchoolLinks;
            this.studentListByCategoryProvider = studentListByCategoryProvider;
            this.schoolCategoryProvider = schoolCategoryProvider;
            this.gradeLevelUtilitiesProvider = gradeLevelUtilitiesProvider;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyMetrics)]
        public StudentSchoolCategoryListModel Get(StudentSchoolCategoryListRequest request)
        {
            var model = new StudentSchoolCategoryListModel();

            var selectedSchoolCategory = schoolCategoryProvider.GetSchoolCategoryType(request.SchoolCategory);
            //Setting the metadata.
            var resolvedListId = metadataListIdResolver.GetListId(ListType.StudentSchoolCategory, selectedSchoolCategory);
            model.ListMetadata = listMetadataProvider.GetListMetadata(resolvedListId);

            var byCategoryOptions = new StudentMetricsProviderQueryOptions
                    {
                        LocalEducationAgencyId = request.LocalEducationAgencyId, SchoolCategory = request.SchoolCategory,
                        //StudentIds = request.StudentIdList
                    };

            IEnumerable<EnhancedStudentInformation> students;

            var sortColumn = model.ListMetadata.GetSortColumn(request.SortColumn);
            students = studentListByCategoryProvider.GetOrderedStudentList(byCategoryOptions, sortColumn, request.SortDirection).ToList();

            model.EntityIds = students.Select(x => new StudentSchoolIdentifier { StudentUSI = x.StudentUSI, SchoolId = x.SchoolId }).ToList();

            if (model.EntityIds.Count == 0)
                return model;

            // If Page Size > 100 then the user requested all records
            students = request.PageSize > 100 ? students : students.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);

            //Set the StudentIds for the GetStudentsWithMetrics call.  You have to pass in StudentIds
            byCategoryOptions.StudentIds = students.Select(student => student.StudentUSI).ToArray();

            var metrics = studentListByCategoryProvider.GetStudentsWithMetrics(byCategoryOptions).ToList();

            //Student Unique Field Newly Added : Saravanan
            foreach (var student in students)
            {
                var studentMetrics = metrics.Where(x => x.StudentUSI == student.StudentUSI).ToList();
                model.Students.Add(new StudentWithMetrics
                    {
                        StudentUSI = student.StudentUSI,
                        SchoolId = student.SchoolId,
                        SchoolName = student.SchoolName,
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
                        Metrics = classroomMetricsProvider.GetAdditionalMetrics(studentMetrics, model.ListMetadata),
                        StudentUniqueID = student.StudentUniqueID
                    });
            }

            return model;
        }
    }
}
