using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Application;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EdFi.Dashboards.Presentation.Core.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class MetricsBasedWatchListController : Controller
    {
        protected readonly IService<MetricsBasedWatchListGetRequest, EdFiGridWatchListModel> MetricsBasedWatchListService;
        protected readonly IPostHandler<MetricsBasedWatchListPostRequest, string> MetricsBasedWatchListPostHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricsBasedWatchListController" /> class.
        /// </summary>
        /// <param name="metricsBasedWatchListService">The metrics based watch list service.</param>
        /// <param name="metricsBasedWatchListPostHandler">The metrics based watch list post handler.</param>
        public MetricsBasedWatchListController(IService<MetricsBasedWatchListGetRequest, EdFiGridWatchListModel> metricsBasedWatchListService,
            IPostHandler<MetricsBasedWatchListPostRequest, string> metricsBasedWatchListPostHandler)
        {
            MetricsBasedWatchListService = metricsBasedWatchListService;
            MetricsBasedWatchListPostHandler = metricsBasedWatchListPostHandler;
        }

        /// <summary>
        /// Gets the watch list by the specified identifier.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Get(MetricsBasedWatchListGetRequest request)
        {
            if (Request.QueryString != null && Request.QueryString.Count >= 3)
            {
                request.Id = int.Parse(Request.QueryString["Id"]);
                request.StaffUSI = long.Parse(Request.QueryString["StaffUSI"]);
                request.SchoolId = int.Parse(Request.QueryString["SchoolId"]);
            }
            
            var model = MetricsBasedWatchListService.Get(request);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Saves the watch list.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Post(MetricsBasedWatchListPostRequest request)
        {
            if (request.SelectedValuesJson != null)
            {
                request.SelectedOptions = System.Web.Helpers.Json.Decode<List<NameValuesType>>(request.SelectedValuesJson);
            }

            var result = MetricsBasedWatchListPostHandler.Post(request);
            return Json(result);
        }
    }
}
