using System.Linq;
using System.Web.Mvc;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Presentation.Core.Models.Shared;
using EdFi.Dashboards.Resources.Common;

namespace EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers
{
    public class SessionStudentListController : Controller
    {
        private readonly ISessionStateProvider session;
        private readonly IUniqueListIdProvider uniqueListProvider;

        public SessionStudentListController(ISessionStateProvider session, IUniqueListIdProvider uniqueListProvider)
        {
            this.session = session;
            this.uniqueListProvider = uniqueListProvider;
        }

        [HttpPost]
        public JsonResult Get(int? sessionUniqueId = null)
        {
            var persistenceUniqueId = sessionUniqueId.HasValue ? uniqueListProvider.GetUniqueId(sessionUniqueId.Value) : uniqueListProvider.GetUniqueId(Request.UrlReferrer);
            var model = session[persistenceUniqueId] as PreviousNextDataModel;

            if (model == null || !model.EntityIdArray.Any()) return Json(null);

            var students = model.EntityIdArray.Select(s => s[0]).ToList();
            var displayValue = string.Format("* {0} Student{1} Selected", students.Count, students.Count > 1 ? "s" : string.Empty);

            return Json(new { displayValue, selectedData = students.Select(s => new { StudentUSI = s }) });
        }
    }
}