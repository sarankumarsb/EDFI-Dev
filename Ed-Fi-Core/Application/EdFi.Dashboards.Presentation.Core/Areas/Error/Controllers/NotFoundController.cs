using System.Web.Mvc;

namespace EdFi.Dashboards.Presentation.Core.Areas.Error.Controllers
{
    public class NotFoundController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get)]
        public ViewResult Get(int localEducationAgencyId)
        {
            return View("Get");
        }
    }
}
