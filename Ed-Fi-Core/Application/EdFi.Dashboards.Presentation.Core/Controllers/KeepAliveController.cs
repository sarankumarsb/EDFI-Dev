using System.Web.Mvc;

namespace EdFi.Dashboards.Presentation.Core.Controllers
{
    public class KeepAliveController : Controller
    {
        public HttpStatusCodeResult Get()
        {
            return new HttpStatusCodeResult(204); // No Content
        }
    }
}
