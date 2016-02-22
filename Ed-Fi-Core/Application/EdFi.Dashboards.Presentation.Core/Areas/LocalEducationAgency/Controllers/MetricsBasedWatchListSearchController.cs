using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Navigation;

namespace EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers
{
    public class MetricsBasedWatchListSearchController : Controller
    {
        protected readonly IMetricsBasedWatchListSearchService MetricsBasedWatchListSearchService;
        protected readonly IGeneralLinks GeneralLinks;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricsBasedWatchListSearchController" /> class.
        /// </summary>
        /// <param name="metricsBasedWatchListSearchService">The metrics based watch list search service.</param>
        /// <param name="generalLinks">The general links.</param>
        public MetricsBasedWatchListSearchController(
            IMetricsBasedWatchListSearchService metricsBasedWatchListSearchService,
            IGeneralLinks generalLinks)
        {
            MetricsBasedWatchListSearchService = metricsBasedWatchListSearchService;
            GeneralLinks = generalLinks;
        }

        public ActionResult Get(MetricsBasedWatchListSearchRequest request)
        {
            var response = MetricsBasedWatchListSearchService.Get(request);
            response.DescriptionServiceUrl = GeneralLinks.MetricsBasedWatchList("MetricsBasedWatchListDescription");
            response.UnshareServiceUrl = GeneralLinks.MetricsBasedWatchList("MetricsBasedWatchListUnshare");

            if (Request.UrlReferrer != null)
            {
                response.ReferringController = Request.UrlReferrer.AbsoluteUri;
            }
            
            return View(response);
        }

        [HttpPost]
        public ActionResult Post(MetricsBasedWatchListSearchRequest request)
        {
            var urlString = MetricsBasedWatchListSearchService.Post(request);
            var response = new
            {
                Url = urlString
            };
            
            return Json(response);
        }
    }
}
