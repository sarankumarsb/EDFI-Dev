using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Presentation.Web.Utilities;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.LocalEducationAgency.Detail;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency.Detail;

namespace EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers.Detail
{
    public class SchoolMetricTableController : Controller
    {
        private readonly ISchoolMetricTableService service;

        public SchoolMetricTableController(ISchoolMetricTableService service)
        {
            this.service = service;
        }

        public ActionResult Get(EdFiDashboardContext context)
        {
            //IList<SchoolMetricModel> schoolMetrics = service.Get(new SchoolMetricTableRequest() { MetricVariantId = context.MetricVariantId.GetValueOrDefault(), LocalEducationAgencyId = context.LocalEducationAgencyId.GetValueOrDefault() });
            SchoolMetricTableModel schoolMetrics = service.Get(new SchoolMetricTableRequest() { MetricVariantId = context.MetricVariantId.GetValueOrDefault(), LocalEducationAgencyId = context.LocalEducationAgencyId.GetValueOrDefault() });


            var model = new GridTable
             {
                 MetricVariantId = context.MetricVariantId.GetValueOrDefault(),
                 Columns = schoolMetrics.ListMetadata.GenerateHeader(),
                 Rows = schoolMetrics.ListMetadata.GenerateRows(schoolMetrics.SchoolMetrics)
             };
            return View(model);
        }

        private static Type CreateGenericType(Type generic, Type innerType)
        {
            return generic.MakeGenericType(new[] { innerType });
        }

    }
}
