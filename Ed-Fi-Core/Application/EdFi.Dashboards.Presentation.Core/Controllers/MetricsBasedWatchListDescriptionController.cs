using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Common;

namespace EdFi.Dashboards.Presentation.Core.Controllers
{
    public class MetricsBasedWatchListDescriptionController : Controller
    {
        protected readonly IService<MetricsBasedWatchListDescriptionRequest, MetricsBasedWatchListDescriptionModel> MetricsBasedWatchListDescriptionService;

        public MetricsBasedWatchListDescriptionController(IService<MetricsBasedWatchListDescriptionRequest, MetricsBasedWatchListDescriptionModel> metricsBasedWatchListDescriptionService)
        {
            MetricsBasedWatchListDescriptionService = metricsBasedWatchListDescriptionService;
        }

        [HttpPost]
        public ActionResult Get(MetricsBasedWatchListDescriptionRequest request)
        {
            var result = MetricsBasedWatchListDescriptionService.Get(request);

            return Json(result);
        }
    }
}
