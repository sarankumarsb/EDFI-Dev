using System.Web.Mvc;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Presentation.Core.Models.Shared.Detail;
using EdFi.Dashboards.Resources.Navigation.Mvc;

namespace EdFi.Dashboards.Presentation.Core.Areas.StudentSchool.Controllers.Detail
{
    public class HistoricalChartController : Controller
    {
        public ActionResult Get(EdFiDashboardContext context, string title)
        {
            var localEducationAgencyId = context.LocalEducationAgencyId.GetValueOrDefault();
            var schoolId = context.SchoolId.GetValueOrDefault();
            var studentUSI = context.StudentUSI.GetValueOrDefault();
            var metricVariantId = context.MetricVariantId.GetValueOrDefault();

            var request = "{ studentUSI:" + studentUSI + ", schoolId:" + schoolId + ", metricVariantId:" + metricVariantId + ", title: \"" + title + "\" }";

            var model = new HistoricalChartWithPeriodsModel(localEducationAgencyId,
                                                            metricVariantId,
                                                            EdFiDashboards.Site.StudentSchool.Resource(schoolId, studentUSI, "HistoricalChartServices"),
                                                            "Get",
                                                            "Get",
                                                            request,
                                                            "Historical"
                                                          );

            return View(model);
        }

    }
}
