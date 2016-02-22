using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Presentation.Core.Areas.School.Models.StudentDemographicList;
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
    public class StudentDemographicListController : EdFiGridBaseController
    {
        protected readonly IStudentDemographicMenuService MenuService;

        public StudentDemographicListController(
            IService<EdFiGridMetaRequest, EdFiGridModel> gridMetaService,
            IService<EdFiGridRequest, EdFiGridModel> gridService,
            IMetricsBasedWatchListDataProvider metricsBasedWatchListDataProvider,
            IStudentDemographicMenuService menuService,
            IPreviousNextSessionProvider previousNextSessionProvider,
            ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks)
            : base(gridMetaService, gridService, previousNextSessionProvider, metricsBasedWatchListDataProvider, localEducationAgencyAreaLinks)
        {
            MenuService = menuService;
        }

        public virtual ActionResult Get(EdFiDashboardContext context, string demographic)
        {
            var menuModel = MenuService.Get(StudentDemographicMenuRequest.Create(context.SchoolId.GetValueOrDefault()));
            var model = new StudentDemographicListModel { MenuModel = menuModel };

            if (string.IsNullOrEmpty(demographic))
            {
                model.PageTitle = "Select demographic";
                model.Demographic = "Select demographic";
            }
            else
            {
                var schoolId = context.SchoolId.GetValueOrDefault();
                var sectionOrCohortId = context.SectionOrCohortId.GetValueOrDefault();
                var studentListType = context.StudentListType;

                demographic = FixDemographicNomenclature(demographic);
                var results = GetGridData(StudentDemographicListMetaRequest.Create(schoolId, sectionOrCohortId, studentListType, demographic));
                model.PageTitle = demographic + " Students";
                model.Demographic = demographic;
                model.PreviousNextSessionPage = results.PreviousNextSessionPage;
                model.ExportGridDataUrl = results.ExportGridDataUrl;
                model.ListType = ListType.StudentDemographic;

                model.GridData = new GridTable
                {
                    Columns = results.ListMetadata.GenerateHeader(),
                    SchoolId = schoolId,
                    WatchList = results.GridTable.WatchList
                };
            }
            return View(model);
        }

        protected virtual string FixDemographicNomenclature(string demographic)
        {
            return demographic
                    .Replace('-', ' ')
                    .Replace("   ", " - ")
                    .Replace("Hispanic Latino", "Hispanic/Latino")
                    .Replace("Gifted Talented", "Gifted/Talented");
        }
    }
}
