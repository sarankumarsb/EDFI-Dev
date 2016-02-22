using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Presentation.Web.Utilities;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.LocalEducationAgency.Detail;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Warehouse.Resource.Models.LocalEducationAgency.Detail;
using EdFi.Dashboards.Warehouse.Resources.LocalEducationAgency.Detail;

namespace EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers.Detail
{
    public class SchoolPriorYearMetricTableController : Controller
    {
        private readonly ISchoolPriorYearMetricTableService service;
        private readonly IMetadataListIdResolver metadataListIdResolver;
        private readonly IListMetadataProvider listMetadataProvider;

        public SchoolPriorYearMetricTableController(ISchoolPriorYearMetricTableService service, IMetadataListIdResolver metadataListIdResolver, IListMetadataProvider listMetadataProvider)
        {
            this.service = service;
            this.metadataListIdResolver = metadataListIdResolver;
            this.listMetadataProvider = listMetadataProvider;
        }

        public ActionResult Get(EdFiDashboardContext context)
        {
            IList<SchoolPriorYearMetricModel> schoolMetrics = service.Get(new SchoolPriorYearMetricTableRequest() { MetricVariantId = context.MetricVariantId.GetValueOrDefault(), LocalEducationAgencyId = context.LocalEducationAgencyId.GetValueOrDefault() });


            var resolvedListId = metadataListIdResolver.GetListId(ListType.PriorYearSchoolMetricTable, SchoolCategory.None);
            var columnGroups = listMetadataProvider.GetListMetadata(resolvedListId);

            var model = new GridTable
            {
                MetricVariantId = context.MetricVariantId.GetValueOrDefault(),
                Columns = columnGroups.GenerateHeader(),
                Rows = columnGroups.GenerateRows((List<SchoolPriorYearMetricModel>)schoolMetrics)
            };

            
            return View(model);
        }
    }
}
