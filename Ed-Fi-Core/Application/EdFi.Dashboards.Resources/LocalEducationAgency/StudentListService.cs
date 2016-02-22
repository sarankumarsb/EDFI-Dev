using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.StudentMetrics;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.LocalEducationAgency
{
    public class StudentListRequest
    {
        public int LocalEducationAgencyId { get; set; }
        public long StaffUSI { get; set; }
        public long SectionOrCohortId { get; set; }
        public string StudentListType { get; set; }

        [AuthenticationIgnore("Server side paging metadata")]
        public int PageNumber { get; set; }
        [AuthenticationIgnore("Server side paging metadata")]
        public int PageSize { get; set; }
        [AuthenticationIgnore("Server side paging metadata")]
        public int? SortColumn { get; set; }
        [AuthenticationIgnore("Server side paging metadata")]
        public string SortDirection { get; set; }

        public static StudentListRequest Create(int localEducationAgencyId, long staffUSI, long sectionOrCohortId, string studentListType, int pageNumber, int pageSize, int? sortColumn, string sortDirection)
        {
            return new StudentListRequest { LocalEducationAgencyId = localEducationAgencyId, StaffUSI = staffUSI, SectionOrCohortId = sectionOrCohortId, StudentListType = studentListType, PageNumber = pageNumber, PageSize = pageSize, SortColumn = sortColumn, SortDirection = sortDirection};
        }
    }

    public interface IStudentListService : IService<StudentListRequest, StudentListModel> { }

    public class StudentListService : IStudentListService
    {
        private readonly IRepository<StaffCohort> staffCohortRepository;
        private readonly IRepository<StaffCustomStudentListStudent> staffCustomStudentListStudentRepository;
        private readonly IRepository<StaffCustomStudentList> staffCustomStudentListRepository;
        private readonly IUniqueListIdProvider uniqueListProvider;
        private readonly IMetadataListIdResolver metadataListIdResolver;
        private readonly IListMetadataProvider listMetadataProvider;
        private readonly IClassroomMetricsProvider classroomMetricsProvider;
        private readonly IStudentSchoolAreaLinks studentSchoolLinks;
        private readonly IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;
        private readonly IStudentMetricsProvider _studentListWithMetricsProvider;

        public StudentListService(IRepository<StaffCohort> staffCohortRepository,
                                    IRepository<StaffCustomStudentList> staffCustomStudentListRepository,
                                    IRepository<StaffCustomStudentListStudent> staffCustomStudentListStudentRepository,
                                    IUniqueListIdProvider uniqueListProvider,
                                    IMetadataListIdResolver metadataListIdResolver,
                                    IListMetadataProvider listMetadataProvider,
                                    IClassroomMetricsProvider classroomMetricsProvider,
                                    IStudentSchoolAreaLinks studentSchoolLinks,
                                    IStudentMetricsProvider studentListWithMetricsProvider,
                                    IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider)
        {
            this.staffCohortRepository = staffCohortRepository;
            this.staffCustomStudentListStudentRepository = staffCustomStudentListStudentRepository;
            this.staffCustomStudentListRepository = staffCustomStudentListRepository;
            this.uniqueListProvider = uniqueListProvider;
            this.metadataListIdResolver = metadataListIdResolver;
            this.listMetadataProvider = listMetadataProvider;
            this.classroomMetricsProvider = classroomMetricsProvider;
            this.studentSchoolLinks = studentSchoolLinks;
            this.gradeLevelUtilitiesProvider = gradeLevelUtilitiesProvider;
            _studentListWithMetricsProvider = studentListWithMetricsProvider;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public StudentListModel Get(StudentListRequest request)
        {
            long staffUSI = request.StaffUSI;
            int localEducationAgencyId = request.LocalEducationAgencyId;
            string studentListType = request.StudentListType;
            long sectionOrCohortId = request.SectionOrCohortId;

            var model = new StudentListModel();
            model.UniqueListId = uniqueListProvider.GetUniqueId();
            model.SchoolCategory = SchoolCategory.HighSchool; //default to SchoolCategory.HighSchool
            model.LocalEducationAgencyId = request.LocalEducationAgencyId;

            var slt = GetSection(staffUSI, localEducationAgencyId, studentListType, ref sectionOrCohortId);
            if (slt != StudentListType.All && sectionOrCohortId == 0)
                return model;

            var studentUSIs = new long[0];

            //We do not pass the StaffUSI in here because this should return all students in the LEA, letting you use cohort and student
            //  list filtering, not just the students that are associated with a specific teacher.
            var options = new StudentMetricsProviderQueryOptions()
            {
                LocalEducationAgencyId = request.LocalEducationAgencyId,
                StudentIds = studentUSIs
            };

            switch (slt)
            {
                case StudentListType.Section:
                    options.TeacherSectionIds = new[]
                    {
                        sectionOrCohortId
                    };

                    break;
                case StudentListType.Cohort:
                    options.StaffCohortIds = new[]
                    {
                        sectionOrCohortId
                    };

                    break;
            }

            if (slt == StudentListType.CustomStudentList)
                options.StudentIds = Enumerable.ToArray(staffCustomStudentListStudentRepository.GetAll().Where(x => x.StaffCustomStudentListId == sectionOrCohortId).Select(x => x.StudentUSI));

            var sortColumn = model.ListMetadata.GetSortColumn(request.SortColumn);

            var studentListEntities = _studentListWithMetricsProvider.GetOrderedStudentList(options, sortColumn, request.SortDirection).ToList();

            // If Page Size > 100 then the user requested all records
            studentUSIs = (request.PageSize > 100 ? studentListEntities : studentListEntities.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)).Select(esi => esi.StudentUSI).ToArray();

            if (!studentUSIs.Any())
                return model;

            options.StudentIds = studentUSIs;
            
            //Setting the metadata.
            var resolvedListId = metadataListIdResolver.GetListId(ListType.StudentDemographic, model.SchoolCategory);
            model.ListMetadata = listMetadataProvider.GetListMetadata(resolvedListId);

            var metrics = _studentListWithMetricsProvider.GetStudentsWithMetrics(options).ToList();

            //Student Unique Field Newly Added : Saravanan
            foreach (var student in studentListEntities)
            {
                var studentInformation = student;
                var studentMetrics = metrics.Where(m => m.StudentUSI == studentInformation.StudentUSI);
                var studentModel = new StudentWithMetrics(student.StudentUSI)
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
                                                                                   new { listContext = model.UniqueListId })
                                           },
                                           Metrics = classroomMetricsProvider.GetAdditionalMetrics(studentMetrics, model.ListMetadata),
                                           StudentUniqueID  = student.StudentUniqueID
                                       };

                model.Students.Add(studentModel);
            }

            return model;
        }


        protected StudentListType GetSection(long staffUSI, int localEducationAgencyId, string studentListType, ref long sectionOrCohortId)
        {
            if (String.IsNullOrEmpty(studentListType))
                studentListType = StudentListType.None.ToString();

            var slt = (StudentListType)Enum.Parse(typeof(StudentListType), studentListType, true);

            if (slt == StudentListType.None)
            {
                var firstCohort = (from data in staffCohortRepository.GetAll()
                                    where data.StaffUSI == staffUSI && data.EducationOrganizationId == localEducationAgencyId
                                    orderby data.CohortDescription, data.StaffCohortId
                                    select data).FirstOrDefault();
                if (firstCohort != null)
                {
                    sectionOrCohortId = firstCohort.StaffCohortId;
                    return StudentListType.Cohort;
                }

                var firstCustomStudentList = (from data in staffCustomStudentListRepository.GetAll()
                                              where data.StaffUSI == staffUSI && data.EducationOrganizationId == localEducationAgencyId
                                              orderby data.CustomStudentListIdentifier , data.StaffCustomStudentListId
                                              select data).FirstOrDefault();
                if (firstCustomStudentList != null)
                {
                    sectionOrCohortId = firstCustomStudentList.StaffCustomStudentListId;
                    return StudentListType.CustomStudentList;
                }
            }

            return slt;
        }
    }
}
