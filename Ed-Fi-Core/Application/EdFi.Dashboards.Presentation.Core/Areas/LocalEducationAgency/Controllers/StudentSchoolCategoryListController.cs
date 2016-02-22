using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Models.StudentSchoolCategoryList;
using EdFi.Dashboards.Presentation.Core.Controllers;
using EdFi.Dashboards.Presentation.Core.Providers.Session;
using EdFi.Dashboards.Presentation.Web.Utilities;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Navigation;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers
{
    public class StudentSchoolCategoryListController : EdFiGridBaseController
    {
        protected readonly IStudentSchoolCategoryMenuService MenuService;

        public StudentSchoolCategoryListController(
            IService<EdFiGridMetaRequest, EdFiGridModel> gridMetaService,
            IService<EdFiGridRequest, EdFiGridModel> gridService,
            IMetricsBasedWatchListDataProvider metricsBasedWatchListDataProvider,
            IStudentSchoolCategoryMenuService menuService,
            IPreviousNextSessionProvider previousNextSessionProvider,
            ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks)
            : base(gridMetaService, gridService, previousNextSessionProvider, metricsBasedWatchListDataProvider, localEducationAgencyAreaLinks)
        {
            MenuService = menuService;
        }

        public ActionResult Get(StudentSchoolCategoryListMetaRequest request)
        {
            var menuModel = MenuService.Get(StudentSchoolCategoryMenuRequest.Create(request.LocalEducationAgencyId, EdFiDashboardContext.Current.StaffUSI.GetValueOrDefault()));
            var model = new StudentSchoolCategoryListModel { MenuModel = menuModel };
            AttributeItemWithSelected<string> selectedMenuOption = null;
            if (request.SchoolCategory != null)
            {
                var schoolCategoryRegEx = request.SchoolCategory.Replace("-", ".");
                var regexExp = new Regex(schoolCategoryRegEx);
                selectedMenuOption = menuModel.SchoolCategories.FirstOrDefault(schoolCategory => regexExp.IsMatch(schoolCategory.Value));
            }

            var isWatchList = false;
            if (request.StudentListType != null &&
                request.StudentListType == StudentListType.MetricsBasedWatchList.ToString())
            {
                selectedMenuOption = menuModel.DynamicWatchLists.FirstOrDefault(sc => sc.Url.Contains(EdFiDashboardContext.Current.RoutedUrl));
                isWatchList = true;
            }

            if (selectedMenuOption != null)
            {
                if (!isWatchList)
                {
                    model.MenuModel.SchoolCategories.First(
                        sc =>
                            sc.Attribute == selectedMenuOption.Attribute && sc.Value == selectedMenuOption.Value &&
                            sc.Url == selectedMenuOption.Url).Selected = true;
                }
                else
                {
                    model.MenuModel.DynamicWatchLists.First(
                        sc =>
                            sc.Attribute == selectedMenuOption.Attribute && sc.Value == selectedMenuOption.Value &&
                            sc.Url == selectedMenuOption.Url).Selected = true;
                }
            }

            request.SchoolCategory = FixSchoolCategoryNomenclature(request.SchoolCategory, request.LocalEducationAgencyId);

            if (selectedMenuOption == null && !request.SectionOrCohortId.HasUsableValue())
            {
                model.Title = "Select Level";
            }
            else
            {
                request.SectionOrCohortId = EdFiDashboardContext.Current.SectionOrCohortId;
                request.StudentListType = EdFiDashboardContext.Current.StudentListType;

                var result = GetGridData(request);

                model.Title = selectedMenuOption.Attribute;
                model.Level = request.SchoolCategory;
                model.ListType = ListType.StudentSchoolCategory;
                model.PreviousNextSessionPage = result.PreviousNextSessionPage;
                model.ExportGridDataUrl = result.ExportGridDataUrl;
                model.GridData = new GridTable
                {
                    Columns = result.ListMetadata.GenerateHeader(),
                    SchoolId = request.LocalEducationAgencyId,
                    WatchList = result.GridTable.WatchList
                };
            }
            return View(model);
        }

        private string FixSchoolCategoryNomenclature(string schoolCategory, int localEducationAgencyId)
        {
            if (string.IsNullOrEmpty(schoolCategory))
                return string.Empty;

            var schoolCategories = MenuService.GetUniqueSchoolCategories(localEducationAgencyId);
            var replacementRegex = new Regex(@"[^\w]");

            var schoolCategoryDictionary =
                schoolCategories.ToDictionary(category => replacementRegex.Replace(category, "-"), category => category);

            return schoolCategoryDictionary[schoolCategory];
        }
    }
}
