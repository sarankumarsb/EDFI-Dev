// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Web.Mvc;
using EdFi.Dashboards.Core.Providers.Context;

namespace EdFi.Dashboards.Presentation.Core.Areas.Search.Controllers
{
    public class ResultsController : Controller
    {
        public ActionResult Get(EdFiDashboardContext context)
        {
            return View();
        }

    }
}
