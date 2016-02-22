using System.Threading;
using System.Web.Mvc;
using log4net;

namespace EdFi.Dashboards.Presentation.Core.Areas.Search.Controllers
{
    public class LogUserActionController : Controller
    {
        private const string actionFormat = "{0} went to Url:\"{1}\" {2}.";

        private static readonly ILog searchLogger = LogManager.GetLogger("SearchLogger");


        [HttpPost]
        public JsonResult Get(string url, string controlNameWhoIsCalling)
        {
            //Logging search results for future enhancements...
            var userName = Thread.CurrentPrincipal.Identity.Name;

            //[User Name] simple search for [Search Terms] returned [Count of Results] results.
            searchLogger.InfoFormat(actionFormat, userName, url, controlNameWhoIsCalling);

            return Json(true);
        }

    }
}
