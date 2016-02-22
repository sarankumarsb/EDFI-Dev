using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EdFi.Dashboards.Resources.Common;

namespace EdFi.Dashboards.Presentation.Core.Controllers
{
    public class MetricsBasedWatchListUnshareController : Controller
    {
        protected readonly IMetricsBasedWatchListUnshareService UnshareService;

        public MetricsBasedWatchListUnshareController(IMetricsBasedWatchListUnshareService unshareService)
        {
            UnshareService = unshareService;
        }

        public ActionResult Post(MetricsBasedWatchListUnshareRequest request)
        {
            var result = UnshareService.Post(request);
            return Json(result);
        }
    }
}
