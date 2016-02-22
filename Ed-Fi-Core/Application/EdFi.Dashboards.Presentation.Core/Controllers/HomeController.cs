// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Web.Mvc;
using EdFi.Dashboards.Resources.Application;
using System.Web.SessionState;

namespace EdFi.Dashboards.Presentation.Core.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public class HomeController : Controller
    {
        private readonly IHomeService service;

        public HomeController(IHomeService service)
        {
            this.service = service;
        }

        public virtual ActionResult Get()
        {
            var model = service.Get(new HomeRequest());

            return View(model);
        }
    }
}
