using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Application;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Support;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.StudentMetrics;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Staff
{
    public class GeneralOverviewRequest
    {
        public int SchoolId { get; set; }
        public long StaffUSI { get; set; }

        [AuthenticationIgnore("LocalEducationAgencyId is implied by SchoolId, and does not need to be independently authorized.  Furthermore, it does not appear to be used currently, and may only be present to force ASP.NET MVC to populate the local education agency Id property on the EdFiDashboardContext.Current instance.")]
        public int LocalEducationAgencyId { get; set; }
        public string StudentListType { get; set; }
        public long SectionOrCohortId { get; set; }
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
        [AuthenticationIgnore("Server side student watchlist metadata")]
        public List<NameValuesType> StudentWatchListData { get; set; }
    }

    public class GeneralOverviewService : StaffServiceBase, IService<GeneralOverviewRequest, GeneralOverviewModel>
    {
        public IStudentWatchListManager WatchListManager { get; set; }
        public ISchoolCategoryProvider SchoolCategoryProvider { get; set; }
        public IClassroomMetricsProvider ClassroomMetricsProvider { get; set; }
        public IListMetadataProvider ListMetadataProvider { get; set; }
        public IMetadataListIdResolver MetadataListIdResolver { get; set; }
        public IGradeLevelUtilitiesProvider GradeLevelUtilitiesProvider { get; set; }
        public IMetricsBasedWatchListDataProvider WatchListDataProvider { get; set; }
        
        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public GeneralOverviewModel Get(GeneralOverviewRequest request)
        {
            var staffUSI = request.StaffUSI;
            var schoolId = request.SchoolId;
            var studentListType = request.StudentListType;
            var sectionOrCohortId = request.SectionOrCohortId;

            var studentListIdentity = GetStudentListIdentity(staffUSI, schoolId, studentListType, sectionOrCohortId);

            // If we're not asking for all students, but we have no list Id, return empty results now
            if (studentListIdentity.StudentListType != StudentListType.All && studentListIdentity.Id == 0)
                return new GeneralOverviewModel();

            var model = new GeneralOverviewModel();

            if (request.UniqueListId == null)
            {
                request.UniqueListId = sectionOrCohortId.ToString(CultureInfo.InvariantCulture);
            }

            var schoolCategory = SchoolCategoryProvider.GetSchoolCategoryType(schoolId);
            switch (schoolCategory)
            {
                case SchoolCategory.Elementary:
                case SchoolCategory.MiddleSchool:
                    break;
                default:
                    schoolCategory = SchoolCategory.HighSchool;
                    break;
            }

            //Get the metadata.
            var resolvedListId = MetadataListIdResolver.GetListId(ListType.ClassroomGeneralOverview, schoolCategory);
            model.ListMetadata = ListMetadataProvider.GetListMetadata(resolvedListId);

            var customStudentIdList = GetCustomListStudentIds(staffUSI, schoolId, studentListIdentity);

            if (studentListIdentity.StudentListType == StudentListType.MetricsBasedWatchList && request.StudentWatchListData.Count <= 0)
            {
                request.StudentWatchListData = WatchListDataProvider.GetWatchListSelectionData(sectionOrCohortId);
            }

            var queryOptions = WatchListManager.CreateStudentMetricsProviderQueryOptions(request.StudentWatchListData,
                model.ListMetadata.GetMetricVariantIds(),
                schoolId,
                0,
                staffUSI,
                customStudentIdList,
                sectionOrCohortId,
                studentListIdentity.StudentListType);

            var students = GetStudentList(request, queryOptions, model).ToList();
            queryOptions.StudentIds = students.Select(s=>s.StudentUSI);
            var metrics = StudentListWithMetricsProvider.GetStudentsWithMetrics(queryOptions).ToList();

            //StudentUniqueID Add : Saravanan
            foreach (var student in students)
            {
                var studentMetrics = metrics.Where(x => x.StudentUSI == student.StudentUSI).ToList();
                model.Students.Add(
                    new StudentWithMetricsAndAccommodations(student.StudentUSI)
                        {
                            SchoolId = student.SchoolId,
                            Name = Utilities.FormatPersonNameByLastName(student.FirstName, student.MiddleName, student.LastSurname),
                            ThumbNail = StudentSchoolAreaLinks.Image(student.SchoolId, student.StudentUSI, student.Gender, student.FullName).Resolve(),
                            Href = new Link {Rel = StudentLink, Href = StudentSchoolAreaLinks.Overview(student.SchoolId, student.StudentUSI, student.FullName).AppendParameters("listContext=" + request.UniqueListId).Resolve()},
                            GradeLevel = GradeLevelUtilitiesProvider.FormatGradeLevelForSorting(student.GradeLevel),
                            GradeLevelDisplayValue = GradeLevelUtilitiesProvider.FormatGradeLevelForDisplay(student.GradeLevel),
                            Metrics = ClassroomMetricsProvider.GetAdditionalMetrics(studentMetrics, model.ListMetadata),
                            StudentUniqueID = student.StudentUniqueID
                        });
            }

            OverlayStudentAccommodation(model.Students, schoolId);

            return model;
        }

        private IEnumerable<EnhancedStudentInformation> GetStudentList(GeneralOverviewRequest request, StudentMetricsProviderQueryOptions providerQueryOptions, GeneralOverviewModel model)
        {
            var sortColumn = model.ListMetadata.GetSortColumn(request.SortColumn);
            var students = StudentListWithMetricsProvider.GetOrderedStudentList(providerQueryOptions, sortColumn, request.SortDirection).ToList();
            model.EntityIds = students.Select(x => new StudentSchoolIdentifier { StudentUSI = x.StudentUSI, SchoolId = x.SchoolId }).ToList();

            if (!students.Any())
                return new List<EnhancedStudentInformation>();
            // If Page Size > 100 then the user requested all records
            return request.PageSize > 100 
                ? students
                : students.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
        }
    }
}
