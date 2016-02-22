using System.Web.Mvc;
using EdFi.Dashboards.Resources.StudentSchool.Detail;

namespace EdFi.Dashboards.Presentation.Core.Areas.StudentSchool.Controllers.Detail
{
    public class BenchmarkHistoricalChartServiceController : Controller
    {
        private readonly IBenchmarkHistoricalChartService service;

        public BenchmarkHistoricalChartServiceController(IBenchmarkHistoricalChartService service)
        {
            this.service = service;
        }

        [HttpPost]
        public JsonResult Get(long studentUSI, int schoolId, int metricVariantId, string title)
        {
            var result = Json(
                service.Get(new BenchmarkHistoricalChartRequest()
                {
                    StudentUSI = studentUSI,
                    SchoolId = schoolId,
                    MetricVariantId = metricVariantId,
                    Title = title
                }));

            return result;
        }

    }
}
