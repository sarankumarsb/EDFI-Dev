using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Presentation.Architecture.Mvc.ActionResults;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Warehouse.Resource.Models.LocalEducationAgency.Detail;
using EdFi.Dashboards.Warehouse.Resources.LocalEducationAgency.Detail;
using IronRuby.Builtins;

namespace EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers.Detail
{
    public class SchoolPriorYearMetricExportController : Controller
    {
        private readonly ISchoolPriorYearMetricTableService _service;

        public SchoolPriorYearMetricExportController(ISchoolPriorYearMetricTableService service)
        {
            _service = service;
        }

        public CsvResult Get(EdFiDashboardContext context)
        {
            var schoolMetrics = _service.Get(new SchoolPriorYearMetricTableRequest() { MetricVariantId = context.MetricVariantId.GetValueOrDefault(), LocalEducationAgencyId = context.LocalEducationAgencyId.GetValueOrDefault() });
            var export = new ExportAllModel();
            var rows = new List<ExportAllModel.Row>();
            
            rows.AddRange(schoolMetrics.OrderBy(x => x.Name)
                .Select(schoolMetric => new ExportAllModel.Row()
                    {
                        Cells = new List<KeyValuePair<string, object>>
                        {
                            new KeyValuePair<string, object>("School", schoolMetric.Name),
                            new KeyValuePair<string, object>("Type", schoolMetric.SchoolCategory),
                            new KeyValuePair<string, object>("School Metric Value", schoolMetric.Value),
                            new KeyValuePair<string, object>("School Goal", schoolMetric.Goal),
                            new KeyValuePair<string, object>("Difference from Goal", schoolMetric.GoalDifference)
                        }
                    }));

            export.Rows = rows;

            return new CsvResult(export);
        }
    }
}
