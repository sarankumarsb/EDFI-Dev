using EdFi.Dashboards.Presentation.Core.Providers.Session;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.School;

namespace EdFi.Dashboards.Presentation.Web.Areas.School.Controllers
{
    public class StudentDemographicListController : EdFi.Dashboards.Presentation.Core.Areas.School.Controllers.StudentDemographicListController
    {
        public StudentDemographicListController(
            IService<EdFiGridMetaRequest, EdFiGridModel> gridMetaService,
            IService<EdFiGridRequest, EdFiGridModel> gridService,
            IMetricsBasedWatchListDataProvider metricsBasedWatchListDataProvider,
            IStudentDemographicMenuService menuService,
            IPreviousNextSessionProvider previousNextSessionProvider,
            IGeneralLinks generalLinks): base(gridMetaService, gridService, metricsBasedWatchListDataProvider, menuService, previousNextSessionProvider, generalLinks)
        {
            
        }

        protected override string FixDemographicNomenclature(string demographic)
        {
            return base.FixDemographicNomenclature(demographic.Replace("--TAGG-", " (TAGG)"));
        }
    }
}
