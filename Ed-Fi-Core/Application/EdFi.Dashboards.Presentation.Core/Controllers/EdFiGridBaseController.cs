using EdFi.Dashboards.Common;
using EdFi.Dashboards.Presentation.Core.Providers.Session;
using EdFi.Dashboards.Presentation.Web.Utilities;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EdFi.Dashboards.Presentation.Core.Controllers
{
    /// <summary>
    /// The base grid controller containing the core grid features.
    /// </summary>
    public class EdFiGridBaseController : Controller
    {
        protected readonly IService<EdFiGridMetaRequest, EdFiGridModel> EdFiGridMetaService;
        protected readonly IService<EdFiGridRequest, EdFiGridModel> EdFiGridService;
        protected readonly IPreviousNextSessionProvider PreviousNextSessionProvider;
        protected readonly IMetricsBasedWatchListDataProvider MetricsBasedWatchListDataProvider;
        protected readonly ILocalEducationAgencyAreaLinks LocalEducationAgencyAreaLinks;

        public EdFiGridBaseController(
            IService<EdFiGridMetaRequest, EdFiGridModel> edFiGridMetaService,
            IService<EdFiGridRequest, EdFiGridModel> edFiGridService,
            IPreviousNextSessionProvider previousNextSessionProvider,
            IMetricsBasedWatchListDataProvider metricsBasedWatchListDataProvider,
            ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks)
        {
            EdFiGridMetaService = edFiGridMetaService;
            EdFiGridService = edFiGridService;
            PreviousNextSessionProvider = previousNextSessionProvider;
            MetricsBasedWatchListDataProvider = metricsBasedWatchListDataProvider;
            LocalEducationAgencyAreaLinks = localEducationAgencyAreaLinks;
        }

        /// <summary>
        /// The default grid get method; this will take a grid request object
        /// and return the base grid data.
        /// </summary>
        /// <param name="convertableRequest">The convertable request.</param>
        /// <returns></returns>
        public EdFiGridModel GetGridData(IEdFiGridMetaRequestConvertable convertableRequest)
        {
            // convert what ever request is being sent into an EdFiGridMetaRequest
            var request = convertableRequest.ConvertToGridMetaRequest();

            if (!request.SchoolId.HasValue && request.LocalEducationAgencyId <= 0)
                return new EdFiGridModel();

            var results = EdFiGridMetaService.Get(request);

            results.ExportGridDataUrl = LocalEducationAgencyAreaLinks.Resource(request.LocalEducationAgencyId, "EdFiGridExport"); // , new { format = "csv" }

            if (results.ListMetadata.Any())
                results.GridTable.Columns = results.ListMetadata.GenerateHeader();

            // pass this back from the get so it can be passed in the post
            results.PreviousNextSessionPage = GetPreviousNextPageNameByListType(request.GridListType);

            var previousNextModel = PreviousNextSessionProvider.GetPreviousNextModel(Request.UrlReferrer, results.PreviousNextSessionPage, request.SchoolId ?? request.LocalEducationAgencyId);
            var watchListSelectedValues = PreviousNextSessionProvider.GetWatchListSelectedValues(previousNextModel);

            var studentUSIs = new List<long>();
            if (previousNextModel != null && previousNextModel.EntityIdArray != null && watchListSelectedValues.Any() &&
                Request.UrlReferrer != null && !Request.UrlReferrer.AbsoluteUri.Contains("MetricsBasedWatchList"))
            {
                var parameterIndex = Array.FindIndex(previousNextModel.ParameterNames,
                    data => data.ToLower() == "studentusi");
                studentUSIs = previousNextModel.EntityIdArray.Select(data => data[parameterIndex]).ToList();
            }
            else
            {
                watchListSelectedValues = new List<NameValuesType>();
            }

            var staffUSI = request.StaffUSI;// > 0 ? request.StaffUSI : UserInformation.Current.StaffUSI;
            var schoolId = request.SchoolId;
            var localEducationAgencyId = request.LocalEducationAgencyId;
            results.GridTable.WatchList = MetricsBasedWatchListDataProvider.GetEdFiGridWatchListModel(staffUSI.GetValueOrDefault(),
                schoolId,
                localEducationAgencyId,
                request.StudentListType == StudentListType.Section.ToString() ||
                request.StudentListType == StudentListType.MetricsBasedWatchList.ToString()
                    ? request.SectionOrCohortId : null,
                watchListSelectedValues.Any() ? watchListSelectedValues : null);

            return results;
        }

        /// <summary>
        /// The post request the EdFi Grid uses to perform data retrieval,
        /// paging and metrics based watch list features.
        /// </summary>
        /// <param name="request">The request containing the data telling what actions need to be performed.</param>
        /// <returns>A data model to be consumed by the EdFi Grid.</returns>
        [HttpPost]
        public ActionResult Get(EdFiGridRequest request)
        {
            var previousNextModel = PreviousNextSessionProvider.GetPreviousNextModel(Request.UrlReferrer, request.PreviousNextSessionPage, request.SchoolId ?? request.LocalEducationAgencyId);

            // make sure the StaffUSI is set
            //request.StaffUSI = UserInformation.Current.StaffUSI;

            // If the StudentListType is None then the list will not be known until the service call
            if (request.StudentListType == StudentListType.None.ToString())
            {
                previousNextModel.EntityIdArray = null;
                previousNextModel.ListPersistenceUniqueId = null;
                previousNextModel.ListUrl = Request.UrlReferrer != null ? Request.UrlReferrer.OriginalString : null;
            }
            else
            {
                request.UniqueListId = previousNextModel.ListPersistenceUniqueId;
            }

            // this will persist the watch list selections to the next/previous
            // controller
            if (request.StudentWatchListData != null && request.StudentWatchListData.Any())
            {
                previousNextModel.StudentWatchListData = request.StudentWatchListData;
            }

            var totalRows = 0;
            if (request.StudentIdList != null && !request.StudentIdList.Any())
            {
                totalRows = previousNextModel.EntityIdArray != null ? previousNextModel.EntityIdArray.Count() : 0;
                request.StudentIdList = totalRows == 0 ? new List<long>() : PreviousNextSessionProvider.GetStudentIdList(previousNextModel, request.PageNumber, request.PageSize, request.SortColumn, request.SortDirection);
            }

            var resourceModel = EdFiGridService.Get(request);

            // If ListPersistenceUniqueId is Null then the Id should have been set in the service based on list id if determined
            if (previousNextModel.ListPersistenceUniqueId == null)
            {
                previousNextModel.ListPersistenceUniqueId = request.UniqueListId;
            }

            var rows = resourceModel.ListMetadata.GenerateRows(resourceModel.Students.ToList<StudentWithMetrics>(), previousNextModel.ListPersistenceUniqueId);
            var model = new GridTable
            {
                Rows = rows,
                TotalRows = resourceModel.EntityIds.Any() ? resourceModel.EntityIds.Count : totalRows
            };

            // Do not store previous/next if the Id could not be determined
            if (previousNextModel.ListPersistenceUniqueId != null)
            {
                PreviousNextSessionProvider.SetPreviousNextDataModel(previousNextModel, request.SortColumn, request.SortDirection, resourceModel.EntityIds);
            }

            return Json(model);
        }

        /// <summary>
        /// Gets the previous next page name by the list type.
        /// </summary>
        /// <param name="listType">Type of the list.</param>
        /// <returns></returns>
        private static string GetPreviousNextPageNameByListType(ListType listType)
        {
            var pageName = string.Empty;

            switch (listType)
            {
                case ListType.ClassroomGeneralOverview:
                    pageName = "GeneralOverview";
                    break;
                case ListType.StudentDemographic:
                    pageName = "StudentDemographicList";
                    break;
                case ListType.StudentSchoolCategory:
                    pageName = "StudentSchoolCategoryList";
                    break;
                case ListType.StudentGrade:
                    pageName = "StudentGradeList";
                    break;
            }

            return pageName;
        }
    }
}
