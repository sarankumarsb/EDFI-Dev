using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EdFi.Dashboards.Application.Resources.LocalEducationAgency.Detail;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Presentation.Web.Areas.LocalEducationAgency.Models.Detail;
using EdFi.Dashboards.Presentation.Web.Utilities;
using EdFi.Dashboards.Resources.LocalEducationAgency.Detail;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency.Detail;
using EdFi.Dashboards.Resources.Staff;

namespace EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers.Detail
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class GoalPlanningSchoolMetricTableController : Controller
    {
        private readonly IGoalPlanningSchoolMetricTableService service;
        private readonly IGoalPlanningSchoolListService goalPlanningService;
        private readonly IMetadataListIdResolver metadataListIdResolver;
        private readonly IListMetadataProvider listMetadataProvider;

        public GoalPlanningSchoolMetricTableController(IGoalPlanningSchoolMetricTableService service, IGoalPlanningSchoolListService goalPlanningService, IMetadataListIdResolver metadataListIdResolver, IListMetadataProvider listMetadataProvider)
        {
            this.service = service;
            this.goalPlanningService = goalPlanningService;
            this.metadataListIdResolver = metadataListIdResolver;
            this.listMetadataProvider = listMetadataProvider;
        }

        public ActionResult Get(EdFiDashboardContext context)
        {
            var serviceResult = service.Get(new GoalPlanningSchoolMetricTableRequest { MetricVariantId = context.MetricVariantId.GetValueOrDefault(), LocalEducationAgencyId = context.LocalEducationAgencyId.GetValueOrDefault() });

            var model = new GoalPlanningSchoolMetricTableModel();
            var request = GoalPlanningSchoolListGetRequest.Create(context.LocalEducationAgencyId.GetValueOrDefault(), serviceResult.SchoolMetrics.Select(sm => new GoalPlanningSchoolListGetRequest.SchoolMetric { SchoolId = sm.SchoolId, MetricId = sm.MetricId }));
            model.GoalPlanning = goalPlanningService.Get(request);
            model.SchoolMetrics = request.SchoolMetrics.ToArray();
            var resolvedListId = metadataListIdResolver.GetListId(ListType.GoalPlanningSchoolMetricTable, SchoolCategory.None);

            var columnGroups = listMetadataProvider.GetListMetadata(resolvedListId);
            model.GridTable = new GridTable()
            {
                MetricVariantId = context.MetricVariantId.GetValueOrDefault(),
                Columns = columnGroups.GenerateHeader(),
                Rows = columnGroups.GenerateRows((List<GoalPlanningSchoolMetric>) serviceResult.GoalPlanningSchoolMetrics)
            };
            
            return View(model);
        }
    }
}
