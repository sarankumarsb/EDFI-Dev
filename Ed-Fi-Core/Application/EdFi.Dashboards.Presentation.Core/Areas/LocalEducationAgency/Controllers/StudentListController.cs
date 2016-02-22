using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Presentation.Core.Providers.Session;
using EdFi.Dashboards.Presentation.Web.Utilities;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers
{
    public class StudentListController : Controller
    {
        protected readonly IStudentListService Service;
        protected readonly IStudentListMenuService MenuService;
        protected readonly IService<MetricsBasedWatchListMenuRequest, List<StudentListMenuModel>> MetricsBasedWatchListMenuService;
        protected readonly IPreviousNextSessionProvider PreviousNextSessionProvider;
        protected readonly IListMetadataProvider ListMetadataProvider;
        protected readonly IMetadataListIdResolver MetadataListIdResolver;


        public StudentListController(
            IStudentListService service,
            IStudentListMenuService menuService,
            IService<MetricsBasedWatchListMenuRequest, List<StudentListMenuModel>> metricsBasedWatchListMenuService,
            IPreviousNextSessionProvider previousNextSessionProvider,
            IListMetadataProvider listMetadataProvider,
            IMetadataListIdResolver metadataListIdResolver)
        {
            Service = service;
            MenuService = menuService;
            MetricsBasedWatchListMenuService = metricsBasedWatchListMenuService;
            PreviousNextSessionProvider = previousNextSessionProvider;
            ListMetadataProvider = listMetadataProvider;
            MetadataListIdResolver = metadataListIdResolver;
        }

        public ActionResult Get(long staffUSI, string studentListType, long? sectionOrCohortId, int localEducationAgencyId)
        {
            var menuModel = MenuService.Get(StudentListMenuRequest.Create(localEducationAgencyId, staffUSI, sectionOrCohortId ?? 0, studentListType));

            var resolvedListId = MetadataListIdResolver.GetListId(ListType.StudentDemographic, SchoolCategory.HighSchool);
            var listMetadata = ListMetadataProvider.GetListMetadata(resolvedListId);

            var watchListRequest = new MetricsBasedWatchListMenuRequest
            {
                LocalEducationAgencyId = localEducationAgencyId,
                StaffUSI = staffUSI
            };

            var watchListModel = MetricsBasedWatchListMenuService.Get(watchListRequest);

            //Constructing the Grid Data.
            var model = new EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Models.StudentList.StudentListModel();
            model.MenuModel = menuModel;
            model.WatchListMenuModel = watchListModel;
            model.Cohort = menuModel.First(x => x.Selected).Description;
            model.GridTable = new GridTable();

            if (listMetadata.Any())
            {
                //Grouping headers and underlying columns.
                model.GridTable.Columns = listMetadata.GenerateHeader();
            }
            //if this is the current users own page
            model.IsCurrentUserListOwner = UserInformation.Current.StaffUSI == staffUSI;
            model.IsCustomStudentList = studentListType == StudentListType.CustomStudentList.ToString();
            model.ListId = sectionOrCohortId.GetValueOrDefault();
            model.LegendViewNames = new List<string> { "Default" };

            return View(model);
        }

        [HttpPost]
        public ActionResult Get(long staffUSI, string studentListType, long? sectionOrCohortId, int localEducationAgencyId, int pageNumber, int pageSize, int? sortColumn, string sortDirection)
        {
            var previousNextModel = PreviousNextSessionProvider.GetPreviousNextModel(Request.UrlReferrer, "StudentList");
            var request = new StudentListRequest
                              {
                                  LocalEducationAgencyId = localEducationAgencyId,
                                  StaffUSI = staffUSI,
                                  SectionOrCohortId = sectionOrCohortId ?? 0,
                                  StudentListType = studentListType,
                                  PageNumber = pageNumber,
                                  PageSize = pageSize,
                                  SortColumn = sortColumn,
                                  SortDirection = sortDirection,
                              };
            if (request.SectionOrCohortId == 0)
                request.StudentListType = StudentListType.All.ToString();

            var resourceModel = Service.Get(request);

            var data = resourceModel.ListMetadata.GenerateRows(resourceModel.Students.ToList(), previousNextModel.ListPersistenceUniqueId, pageNumber, pageSize, sortColumn, sortDirection);

            var model = new GridDataWithFootnotes
            {
                Rows = data.Item2,
                TotalRows = resourceModel.Students.Count
            };

            PreviousNextSessionProvider.SetPreviousNextDataModel(previousNextModel, sortColumn, sortDirection, data.Item1.Select(x => new StudentSchoolIdentifier { StudentUSI = x[0], SchoolId = (int)x[1] }).ToList());

            return Json(model);
        }
    }
}
