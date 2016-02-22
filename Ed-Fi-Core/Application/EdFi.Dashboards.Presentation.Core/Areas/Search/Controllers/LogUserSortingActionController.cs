using System.Threading;
using System.Web.Mvc;
using log4net;

namespace EdFi.Dashboards.Presentation.Core.Areas.Search.Controllers
{
    public class LogUserSortingActionController : Controller
    {
        private const string sortingActionFormat = "{0} sorted search results by {1}.";

        private static readonly ILog searchLogger = LogManager.GetLogger("SearchLogger");

        [HttpPost]
        public bool Get(string sortCriteria)
        {
            //Logging search results for future enhancements...
            var userName = Thread.CurrentPrincipal.Identity.Name;

            //[User Name] sorted search results by [Sort Criteria].
            searchLogger.InfoFormat(sortingActionFormat, userName, sortCriteria);

            return true;
        }

    }
}
