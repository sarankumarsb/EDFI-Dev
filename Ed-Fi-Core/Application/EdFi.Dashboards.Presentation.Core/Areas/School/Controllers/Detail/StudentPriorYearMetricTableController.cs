using System.Linq;
using System.Web.Mvc;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Presentation.Core.Providers.Session;
using EdFi.Dashboards.Presentation.Web.Utilities;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Warehouse.Resources.School.Detail;

namespace EdFi.Dashboards.Presentation.Core.Areas.School.Controllers.Detail
{
    public class StudentPriorYearMetricTableController : Controller
    {
        private readonly IStudentPriorYearMetricListService service;
        private readonly IPreviousNextSessionProvider previousNextSessionProvider;

        public StudentPriorYearMetricTableController(IStudentPriorYearMetricListService service, IPreviousNextSessionProvider previousNextSessionProvider)
        {
            this.service = service;
            this.previousNextSessionProvider = previousNextSessionProvider;
        }

        public ActionResult Get(EdFiDashboardContext context)
        {
            var schoolId = context.SchoolId.GetValueOrDefault();
            var localEducationAgencyId = context.LocalEducationAgencyId.GetValueOrDefault();
            var metricVariantId = context.MetricVariantId.GetValueOrDefault();
            var results = service.Get(StudentPriorYearMetricListRequest.Create(schoolId, localEducationAgencyId, metricVariantId));

            var model = new GridDataWithFootnotes
                            {
                                MetricFootnotes = results.MetricFootnotes,
                                Columns = results.ListMetadata.GenerateHeader(),
                                TotalRows = results.Students.Count,
                                EntityIds = results.Students.Select(x => x.StudentUSI).ToList(),
                                SchoolId = schoolId
                            };

            previousNextSessionProvider.RemovePreviousNextDataModel(context.MetricVariantId);
            return View(model);
        }

        [HttpPost]
        public ActionResult Get(EdFiDashboardContext context, int pageNumber, int pageSize, int? sortColumn, string sortDirection)
        {
            var metricVariantId = context.MetricVariantId.GetValueOrDefault();
            var previousNextModel = previousNextSessionProvider.GetPreviousNextModel(Request.UrlReferrer, "StudentList", metricVariantId);
            var results = service.Get(StudentPriorYearMetricListRequest.Create(context.SchoolId.GetValueOrDefault(), context.LocalEducationAgencyId.GetValueOrDefault(), metricVariantId));
            var data = results.ListMetadata.GenerateRows(results.Students.ToList<StudentWithMetrics>(),
                                                         results.UniqueListId, pageNumber, pageSize, sortColumn, sortDirection);
            var model = new GridDataWithFootnotes
            {
                Rows = data.Item2,
                TotalRows = results.Students.Count
            };

            // TODO: GKM - Review use of int[][]
            previousNextSessionProvider.SetPreviousNextDataModel(previousNextModel, sortColumn, sortDirection, data.Item1.Select(x => new StudentSchoolIdentifier { StudentUSI = x[0], SchoolId = (int)x[1]}).ToList());

            return Json(model);
        }
    }
}
