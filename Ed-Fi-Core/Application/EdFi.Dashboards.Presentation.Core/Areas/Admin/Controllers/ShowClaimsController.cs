using System.Text;
using System.Web.Mvc;
using Microsoft.IdentityModel.Claims;

namespace EdFi.Dashboards.Presentation.Core.Areas.Admin.Controllers
{
    public class ShowClaimsController : Controller
    {
        public ActionResult Get()
        {
			var sb = new StringBuilder();

			var identity = User.Identity as IClaimsIdentity;

			foreach (var claim in identity.Claims)
				sb.AppendLine(claim.ClaimType + ": " + claim.Value + "<br/>");

            return View(MvcHtmlString.Create(sb.ToString()));
        }

    }
}
