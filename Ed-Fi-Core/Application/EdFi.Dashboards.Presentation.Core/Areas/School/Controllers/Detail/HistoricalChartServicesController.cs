using System.Web.Mvc;
using EdFi.Dashboards.Warehouse.Resources.School.Detail;

namespace EdFi.Dashboards.Presentation.Core.Areas.School.Controllers.Detail
{
    public class HistoricalChartServicesController : Controller
    {
        private readonly IHistoricalChartService service;

        public HistoricalChartServicesController(IHistoricalChartService service)
        {
            this.service = service;
        }

        [HttpPost]
        public JsonResult Get(int schoolId, int metricVariantId, int? periodId, string title)
        {
            var result = Json(
                service.Get(new HistoricalChartRequest() { MetricVariantId = metricVariantId, SchoolId = schoolId, PeriodId = periodId, Title = title}));

            return result;
        }

    }
}
