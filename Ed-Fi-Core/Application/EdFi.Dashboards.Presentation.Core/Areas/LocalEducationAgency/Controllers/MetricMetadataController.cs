using System.Web.Mvc;
using EdFi.Dashboards.Metric.Resources.Services;

namespace EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers
{
    /// <summary>
    /// Provides access to the metric metadata.
    /// </summary>
    public class MetricMetadataController : Controller
    {
        private readonly IMetricMetadataTreeService metricMetadataTreeService;

        public MetricMetadataController(IMetricMetadataTreeService metricMetadataTreeService)
        {
            this.metricMetadataTreeService = metricMetadataTreeService;
        }

        public ActionResult Get()
        {
            return View(metricMetadataTreeService.Get(new MetricMetadataTreeRequest()));
        }
    }
}
