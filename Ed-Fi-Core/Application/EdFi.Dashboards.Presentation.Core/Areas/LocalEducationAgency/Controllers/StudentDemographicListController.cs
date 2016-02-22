using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Models.StudentDemographicList;
using EdFi.Dashboards.Presentation.Core.Controllers;
using EdFi.Dashboards.Presentation.Core.Providers.Session;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Navigation;
using System.Web.Mvc;

namespace EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers
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
            var menuModel = MenuService.Get(StudentDemographicMenuRequest.Create(context.LocalEducationAgencyId.GetValueOrDefault(), context.StaffUSI.GetValueOrDefault()));
            var model = new StudentDemographicListModel { MenuModel = menuModel };

            if (string.IsNullOrEmpty(demographic) && !context.SectionOrCohortId.HasUsableValue())
            {
                model.Demographic = "Select demographic";
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(demographic))
                    demographic = FixDemographicNomenclature(demographic);

                var localEducationAgencyId = context.LocalEducationAgencyId.GetValueOrDefault();
                var sectionOrCohortId = context.SectionOrCohortId.GetValueOrDefault();
                var studentListType = context.StudentListType;

                if (studentListType == StudentListType.MetricsBasedWatchList.ToString())
                {
                    var selectedMenuOption = menuModel.WatchLists.FirstOrDefault(sc => sc.Url.Contains(EdFiDashboardContext.Current.RoutedUrl));
                    if (selectedMenuOption != null)
                    {
                        model.MenuModel.WatchLists.First(
                            sc =>
                                sc.Attribute == selectedMenuOption.Attribute && sc.Value == selectedMenuOption.Value &&
                                sc.Url == selectedMenuOption.Url).Selected = true;
                    }
                }
                else
                {
                    if (model.MenuModel.Ethnicity.FirstOrDefault(d => d.Attribute == demographic) != null)
                        model.MenuModel.Ethnicity.First(d => d.Attribute == demographic).Selected = true;

                    if (model.MenuModel.Gender.FirstOrDefault(d => d.Attribute == demographic) != null)
                        model.MenuModel.Gender.First(d => d.Attribute == demographic).Selected = true;

                    if (model.MenuModel.Indicator.FirstOrDefault(d => d.Attribute == demographic) != null)
                        model.MenuModel.Indicator.First(d => d.Attribute == demographic).Selected = true;

                    if (model.MenuModel.Program.FirstOrDefault(d => d.Attribute == demographic) != null)
                        model.MenuModel.Program.First(d => d.Attribute == demographic).Selected = true;

                    if (model.MenuModel.Race.FirstOrDefault(d => d.Attribute == demographic) != null)
                        model.MenuModel.Race.First(d => d.Attribute == demographic).Selected = true;
                }

                var results = GetGridData(StudentDemographicListMetaRequest.Create(localEducationAgencyId, sectionOrCohortId, studentListType, demographic));

                model.Demographic = demographic;
                model.GridData = results.GridTable;
                model.PreviousNextSessionPage = results.PreviousNextSessionPage;
                model.ExportGridDataUrl = results.ExportGridDataUrl;
                model.ListType = ListType.StudentDemographic;
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
