using System.Web.Mvc;
using EdFi.Dashboards.Warehouse.Resources.StudentSchool.Detail;

namespace EdFi.Dashboards.Presentation.Core.Areas.StudentSchool.Controllers.Detail
{
    public class HistoricalChartServicesController : Controller
    {
        private readonly IHistoricalChartService service;

        public HistoricalChartServicesController(IHistoricalChartService service)
        {
            this.service = service;
        }

        [HttpPost]
        public JsonResult Get(long studentUSI, int schoolId, int metricVariantId, int? periodId, string title)
        {
            var result = Json(
                service.Get(new HistoricalChartRequest()
                {
                    StudentUSI = studentUSI,
                    SchoolId = schoolId,
                    MetricVariantId = metricVariantId,
                    PeriodId = periodId,
                    Title = title
                }));

            return result;
        }

    }
}
