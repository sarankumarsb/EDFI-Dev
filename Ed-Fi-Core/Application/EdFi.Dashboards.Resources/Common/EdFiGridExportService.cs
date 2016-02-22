using EdFi.Dashboards.Common;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Common
{
    public class EdFiGridExportRequest
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
        public int? LocalEducationAgencyId { get; set; }

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
        /// Gets or sets the student watch list JSON.
        /// </summary>
        /// <value>
        /// The student watch list data.
        /// </value>
        [AuthenticationIgnore("Server side student watchlist metadata")]
        public string StudentWatchListJson { get; set; }

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

    public interface IEdFiGridExportService : IService<EdFiGridExportRequest, ExportDataModel> { }

    public class EdFiGridExportService : IEdFiGridExportService
    {
        protected readonly IService<EdFiGridMetaRequest, EdFiGridModel> EdFiGridMetaService;
        protected readonly IService<EdFiGridRequest, EdFiGridModel> EdFiGridService;

        public EdFiGridExportService(
            IService<EdFiGridMetaRequest, EdFiGridModel> edFiGridMetaService,
            IService<EdFiGridRequest, EdFiGridModel> edFiGridService)
        {
            EdFiGridMetaService = edFiGridMetaService;
            EdFiGridService = edFiGridService;
        }

        [NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public ExportDataModel Get(EdFiGridExportRequest request)
        {
            ListType gridListType;
            Enum.TryParse(request.ListType, true, out gridListType);

            var metaRequest = new EdFiGridMetaRequest
            {
                StaffUSI = request.StaffUSI,
                SchoolId = request.SchoolId,
                SectionOrCohortId = request.SectionOrCohortId.GetValueOrDefault(),
                LocalEducationAgencyId = request.LocalEducationAgencyId.GetValueOrDefault(),
                StudentListType = request.StudentListType,
                Demographic = request.SelectedDemographicOption,
                Level = request.SelectedGradeOption,
                Grade = request.SelectedGradeOption,
                GridListType = gridListType
            };

            var metaResult = EdFiGridMetaService.Get(metaRequest);

            var gridRequest = new EdFiGridRequest
            {
                LocalEducationAgencyId = request.LocalEducationAgencyId.GetValueOrDefault(),
                SchoolId = request.SchoolId.GetValueOrDefault(),
                StaffUSI = request.StaffUSI.GetValueOrDefault(),
                PageSize = 101,
                StudentWatchListData = request.StudentWatchListData,
                SelectedDemographicOption = request.SelectedDemographicOption,
                SelectedSchoolCategoryOption = request.SelectedSchoolCategoryOption,
                SelectedGradeOption = request.SelectedGradeOption,
                ListType = request.ListType
            };

            var gridResult = EdFiGridService.Get(gridRequest);

            var exportModel = new ExportDataModel
            {
                ExportColumns = metaResult.ListMetadata,
                ExportRows = gridResult.Students
            };

            return exportModel;
        }
    }
}
