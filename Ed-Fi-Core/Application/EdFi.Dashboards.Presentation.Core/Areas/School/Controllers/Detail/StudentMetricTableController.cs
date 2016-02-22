using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EdFi.Dashboards.Presentation.Core.Providers.Session;
using EdFi.Dashboards.Presentation.Web.Utilities;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.School.Detail;

namespace EdFi.Dashboards.Presentation.Core.Areas.School.Controllers.Detail
{
    public class StudentMetricTableController : Controller
    {
        private readonly IStudentMetricListService service;
        private readonly IStudentMetricListMetaService metaService;
        private readonly IPreviousNextSessionProvider previousNextSessionProvider;

        public StudentMetricTableController(IStudentMetricListService service, IStudentMetricListMetaService metaService, IPreviousNextSessionProvider previousNextSessionProvider)
        {
            this.service = service;
            this.metaService = metaService;
            this.previousNextSessionProvider = previousNextSessionProvider;
        }

        public ActionResult Get(StudentMetricListMetaRequest context)
        {
            var results = metaService.Get(context);

            var model = new GridDataWithFootnotes
                            {
                                MetricFootnotes = results.MetricFootnotes,
                                Columns = results.ListMetadata.GenerateHeader(),
                                SchoolId = context.SchoolId,
                                MetricVariantId = context.MetricVariantId
                            };

            previousNextSessionProvider.RemovePreviousNextDataModel(context.MetricVariantId);
            return View(model);
        }

        [HttpPost]
        public ActionResult Get(StudentMetricListRequest context)
        {
            var previousNextModel = previousNextSessionProvider.GetPreviousNextModel(Request.UrlReferrer, "Metric", context.MetricVariantId);
            var totalRows = previousNextModel.EntityIdArray != null ? previousNextModel.EntityIdArray.Count() : 0;

            context.StudentIdList = totalRows == 0
                ? new List<long>()
                : previousNextSessionProvider.GetStudentIdList(previousNextModel, context.PageNumber, context.PageSize, context.SortColumn, context.SortDirection);
            
            context.UniqueListId = previousNextModel.ListPersistenceUniqueId;
            
            var results = service.Get(context);
            var rows = results.ListMetadata.GenerateRows(results.Students.ToList<StudentWithMetrics>(), previousNextModel.ListPersistenceUniqueId);
            var model = new GridDataWithFootnotes
                            {
                                Rows = rows,
                                TotalRows = results.EntityIds.Any() ? results.EntityIds.Count : totalRows
                            };

            // TODO: GKM - Review use of int[][]
            previousNextSessionProvider.SetPreviousNextDataModel(previousNextModel, context.SortColumn, context.SortDirection, results.EntityIds.Select(x => new StudentSchoolIdentifier { StudentUSI = x[0], SchoolId = (int)x[1] }).ToList());

            // TODO: GKM - Review use of JsonResult instead of ViewResult + conneg action invoker
            return Json(model);
        }
    }
}
