using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Presentation.Core.Areas.School.Models.StudentGradeList;
using EdFi.Dashboards.Presentation.Core.Controllers;
using EdFi.Dashboards.Presentation.Core.Providers.Session;
using EdFi.Dashboards.Presentation.Web.Utilities;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.School;
using System.Web.Mvc;

namespace EdFi.Dashboards.Presentation.Core.Areas.School.Controllers
{
    public class StudentGradeListController : EdFiGridBaseController
    {
        protected readonly IStudentGradeMenuService MenuService;

        public StudentGradeListController(
            IService<EdFiGridMetaRequest, EdFiGridModel> gridMetaService,
            IService<EdFiGridRequest, EdFiGridModel> gridService,
            IMetricsBasedWatchListDataProvider metricsBasedWatchListDataProvider,
            IStudentGradeMenuService menuService,
            IPreviousNextSessionProvider previousNextSessionProvider,
            ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks)
            : base(gridMetaService, gridService, previousNextSessionProvider, metricsBasedWatchListDataProvider, localEducationAgencyAreaLinks)
        {
            MenuService = menuService;
        }

        public ActionResult Get(StudentGradeListRequest request)
        {
            var menuModel = MenuService.Get(StudentGradeMenuRequest.Create(request.SchoolId));
            var model = new StudentGradeListModel { MenuModel = menuModel };

            request.Grade = FixGradeNomenclature(request.Grade);


            var results = GetGridData(request);
            model.Grade = request.Grade;
            model.ListType = ListType.StudentGrade;
            model.PreviousNextSessionPage = results.PreviousNextSessionPage;
            model.ExportGridDataUrl = results.ExportGridDataUrl;

            model.GridData = new GridTable
            {
                Columns = results.ListMetadata.GenerateHeader(),
                SchoolId = request.SchoolId,
                WatchList = results.GridTable.WatchList
            };

            return View(model);
        }

        private static string FixGradeNomenclature(string grade)
        {
            if (string.IsNullOrEmpty(grade))
                return string.Empty;

            return grade
                    .Replace('-', ' ')
                    .Replace("   ", " - ")
                    .Replace("Infant toddler", "Infant/toddler")
                    .Replace("Preschool Prekindergarten", "Preschool/Prekindergarten")
                    .Replace("All", string.Empty);
        }
    }
}
