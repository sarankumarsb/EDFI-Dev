using System;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Support;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Resources.StudentMetrics;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EdFi.Dashboards.Resources.Common
{
    /// <summary>
    /// This is the post request the EdFi Grid uses to perform functions like
    /// paging and limiting displayed data.
    /// </summary>
    public class EdFiGridRequest
    {
        /// <summary>
        /// Gets or sets the school identifier.
        /// </summary>
        /// <value>
        /// The school identifier.
        /// </value>
        public int? SchoolId { get; set; }

        /// <summary>
        /// Gets or sets the staff usi.
        /// </summary>
        /// <value>
        /// The staff usi.
        /// </value>
        public long? StaffUSI { get; set; }

        /// <summary>
        /// Gets or sets the section or cohort identifier.
        /// </summary>
        /// <value>
        /// The section or cohort identifier.
        /// </value>
        public long? SectionOrCohortId { get; set; }

        /// <summary>
        /// Gets or sets the type of the student list.
        /// </summary>
        /// <value>
        /// The type of the student list.
        /// </value>
        public string StudentListType { get; set; }

        /// <summary>
        /// Gets or sets the local education agency identifier.
        /// </summary>
        /// <value>
        /// The local education agency identifier.
        /// </value>
        public int LocalEducationAgencyId { get; set; }

        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        /// <value>
        /// The page number.
        /// </value>
        [AuthenticationIgnore("Server side paging metadata")]
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        [AuthenticationIgnore("Server side paging metadata")]
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the sort column.
        /// </summary>
        /// <value>
        /// The sort column.
        /// </value>
        [AuthenticationIgnore("Server side paging metadata")]
        public int? SortColumn { get; set; }

        /// <summary>
        /// Gets or sets the sort direction.
        /// </summary>
        /// <value>
        /// The sort direction.
        /// </value>
        [AuthenticationIgnore("Server side paging metadata")]
        public string SortDirection { get; set; }

        /// <summary>
        /// Gets or sets the visible columns.
        /// </summary>
        /// <value>
        /// The visible columns.
        /// </value>
        [AuthenticationIgnore("Server side paging metadata")]
        public string VisibleColumns { get; set; }

        /// <summary>
        /// Gets or sets the student identifier list.
        /// </summary>
        /// <value>
        /// The student identifier list.
        /// </value>
        [AuthenticationIgnore("Server side paging metadata")]
        public List<long> StudentIdList { get; set; }

        /// <summary>
        /// Gets or sets the unique list identifier.
        /// </summary>
        /// <value>
        /// The unique list identifier.
        /// </value>
        [AuthenticationIgnore("Server side paging metadata")]
        public string UniqueListId { get; set; }

        /// <summary>
        /// Gets or sets the previous next session page.
        /// </summary>
        /// <value>
        /// The previous next session page.
        /// </value>
        [AuthenticationIgnore("Used to get previous/next controller session data")]
        public string PreviousNextSessionPage { get; set; }

        /// <summary>
        /// Gets or sets the student watch list data.
        /// </summary>
        /// <value>
        /// The student watch list data.
        /// </value>
        [AuthenticationIgnore("Server side student watchlist metadata")]
        public List<NameValuesType> StudentWatchListData { get; set; }

        /// <summary>
        /// Gets or sets the current list that is using the EdFi Grid.
        /// </summary>
        /// <value>
        /// The type of the list.
        /// </value>
        [AuthenticationIgnore("Defines the list that is being viewed; this will be converted into a ListType object manually")]
        public string ListType { get; set; }

        /// <summary>
        /// Gets or sets the demographic option groups.
        /// </summary>
        /// <value>
        /// The demographic option groups.
        /// </value>
        [AuthenticationIgnore("Loads any pre-defined demographics")]
        public string SelectedDemographicOption { get; set; }

        /// <summary>
        /// Gets or sets the selected level option.
        /// </summary>
        /// <value>
        /// The selected level option.
        /// </value>
        [AuthenticationIgnore("Loads any pre-defined levels")]
        public string SelectedSchoolCategoryOption { get; set; }

        /// <summary>
        /// Gets or sets the selected grade option.
        /// </summary>
        /// <value>
        /// The selected grade option.
        /// </value>
        [AuthenticationIgnore("Loads any pre-defined grades")]
        public string SelectedGradeOption { get; set; }
    }

    public interface IEdFiGridService : IService<EdFiGridRequest, EdFiGridModel> { }

    public class EdFiGridService : EdFiGridServiceBase, IEdFiGridService
    {
        public IStudentWatchListManager WatchListManager { get; set; }
        public ISchoolCategoryProvider SchoolCategoryProvider { get; set; }
        public IClassroomMetricsProvider ClassroomMetricsProvider { get; set; }
        public IListMetadataProvider ListMetadataProvider { get; set; }
        public IMetadataListIdResolver MetadataListIdResolver { get; set; }
        public IGradeLevelUtilitiesProvider GradeLevelUtilitiesProvider { get; set; }
        public IMetricsBasedWatchListDataProvider WatchListDataProvider { get; set; }

        [NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public EdFiGridModel Get(EdFiGridRequest request)
        {
            var staffUSI = request.StaffUSI;
            var schoolId = request.SchoolId;
            var localEducationAgencyId = request.LocalEducationAgencyId;
            var studentListType = request.StudentListType ?? "All";
            var sectionOrCohortId = request.SectionOrCohortId;
            var listType = (ListType)Enum.Parse(typeof (ListType), request.ListType);

            var studentListIdentity = GetStudentListIdentity(
                staffUSI.GetValueOrDefault(),
                schoolId > 0 ? schoolId.GetValueOrDefault() : localEducationAgencyId,
                studentListType,
                sectionOrCohortId.GetValueOrDefault());

            var model = new EdFiGridModel();

            if (request.UniqueListId == null)
            {
                request.UniqueListId = sectionOrCohortId.GetValueOrDefault().ToString(CultureInfo.InvariantCulture);
            }

            var schoolCategory = request.SchoolId.GetValueOrDefault() > 0
                ? SchoolCategoryProvider.GetSchoolCategoryType(request.SchoolId.GetValueOrDefault())
                : SchoolCategory.HighSchool;
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
            //var resolvedListId = MetadataListIdResolver.GetListId(ListType.ClassroomGeneralOverview, schoolCategory);
            var resolvedListId = MetadataListIdResolver.GetListId(listType, schoolCategory);
            model.ListMetadata = ListMetadataProvider.GetListMetadata(resolvedListId);

            var customStudentIdList = GetCustomListStudentIds(
                staffUSI.GetValueOrDefault(),
                schoolId > 0 ? schoolId.GetValueOrDefault() : localEducationAgencyId,
                studentListIdentity);

            bool hasWatchListData = request.StudentWatchListData == null || request.StudentWatchListData.Count <= 0;

            if (studentListIdentity.StudentListType == StudentListType.MetricsBasedWatchList && hasWatchListData)
            {
                request.StudentWatchListData = WatchListDataProvider.GetWatchListSelectionData(sectionOrCohortId);
            }

            SelectionOptionGroup[] demographicOptionsGroup = null;
            if (!string.IsNullOrWhiteSpace(request.SelectedDemographicOption))
                demographicOptionsGroup = new[]
                {
                    new SelectionOptionGroup
                    {
                        SelectionOptionName = GetOptionGroupName(request.SelectedDemographicOption),
                        SelectedOptions = new[] { request.SelectedDemographicOption }
                    }
                };

            //Most of the pages should only return items that is applicable to the staff being viewed, but when showing 
            //  CustomStudentLists, we want to not restrict by the StaffUSI, because it could be a school Admin
            //  viewing a student that's was added from a completely different screen.
            if (studentListIdentity.StudentListType == StudentListType.CustomStudentList)
                staffUSI = 0;

            var queryOptions = WatchListManager.CreateStudentMetricsProviderQueryOptions(request.StudentWatchListData,
                model.ListMetadata.GetMetricVariantIds(),
                schoolId.GetValueOrDefault(),
                localEducationAgencyId,
                staffUSI,
                customStudentIdList,
                sectionOrCohortId,
                studentListIdentity.StudentListType,
                demographicOptionsGroup,
                request.SelectedSchoolCategoryOption,
                request.SelectedGradeOption);

            var students = GetStudentList(request, queryOptions, model).ToList();
            queryOptions.StudentIds = students.Select(s => s.StudentUSI);
            var metrics = StudentListWithMetricsProvider.GetStudentsWithMetrics(queryOptions).ToList();

            //Student Unique Field Newly Added : Saravanan
            foreach (var student in students)
            {
                var studentMetrics = metrics.Where(x => x.StudentUSI == student.StudentUSI).ToList();
                model.Students.Add(
                    new StudentWithMetricsAndAccommodations(student.StudentUSI)
                    {
                        SchoolId = student.SchoolId,
                        SchoolName = student.SchoolName,
                        Name = Utilities.FormatPersonNameByLastName(student.FirstName, student.MiddleName, student.LastSurname),
                        ThumbNail = StudentSchoolAreaLinks.Image(student.SchoolId, student.StudentUSI, student.Gender, student.FullName).Resolve(),
                        Href = new Link { Rel = StudentLink, Href = StudentSchoolAreaLinks.Overview(student.SchoolId, student.StudentUSI, student.FullName).AppendParameters("listContext=" + request.UniqueListId).Resolve() },
                        GradeLevel = GradeLevelUtilitiesProvider.FormatGradeLevelForSorting(student.GradeLevel),
                        GradeLevelDisplayValue = GradeLevelUtilitiesProvider.FormatGradeLevelForDisplay(student.GradeLevel),
                        Metrics = ClassroomMetricsProvider.GetAdditionalMetrics(studentMetrics, model.ListMetadata),
                        StudentUniqueID  =student.StudentUniqueID
                    });
            }

            if (schoolId > 0)
                OverlayStudentAccommodation(model.Students, schoolId.GetValueOrDefault());

            return model;
        }

        private IEnumerable<EnhancedStudentInformation> GetStudentList(EdFiGridRequest request, StudentMetricsProviderQueryOptions providerQueryOptions, EdFiGridModel model)
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

        private static string GetOptionGroupName(string optionValue)
        {
            var optionGroupName = string.Empty;

            //TODO: This should really be pulled from WatchListDataProvider.GetEdFiGridWatchListModel, instead of hard coding this here.

            switch (optionValue)
            {
                case "White":
                case "Asian":
                case "Native Hawaiian - Pacific Islander":
                case "Hispanic/Latino":
                case "Black - African American":
                case "American Indian - Alaskan Native":
                    optionGroupName = "demographic-demographics";
                    break;
                case "504 Designation":
                case "Bilingual Program":
                case "Career and Technical Education":
                case "Gifted/Talented":
                case "Special Education":
                case "Title I Participation":
                    optionGroupName = "demographic-program-status";
                    break;
                case "Targeted Achievement Gap Group (TAGG)":
                case "Highly Mobile":
                case "Homeless":
                case "Immigrant":
                case "Limited English Proficiency":
                case "Migrant":
                case "Over Age":
                case "Retained":
                case "Alternative Learning Environment":
                    optionGroupName = "demographic-other-information";
                    break;
            }

            return optionGroupName;
        }
    }
}
