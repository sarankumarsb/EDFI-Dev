using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Presentation.Architecture.Mvc.ActionResults;
using EdFi.Dashboards.Presentation.Web.Utilities;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Warehouse.Resource.Models.School.Detail;
using EdFi.Dashboards.Warehouse.Resources.School.Detail;

namespace EdFi.Dashboards.Presentation.Core.Areas.School.Controllers.Detail
{
   public class StudentPriorYearMetricExportController : Controller
    {
        private readonly IStudentPriorYearMetricListService _service;

        public StudentPriorYearMetricExportController(IStudentPriorYearMetricListService service)
        {
            _service = service;
        }

        public CsvResult Get(EdFiDashboardContext context)
        {
            var metricVariantId = context.MetricVariantId.GetValueOrDefault();
            var results = _service.Get(StudentPriorYearMetricListRequest.Create(context.SchoolId.GetValueOrDefault(), context.LocalEducationAgencyId.GetValueOrDefault(), metricVariantId));
            var dataHeader = GetColumns(results);
            var dataRows = results.ListMetadata.GenerateRows(results.Students.OrderBy(x => x.Name).ToList<StudentWithMetrics>(), results.UniqueListId);
            
            var export = new ExportAllModel
                             {
                                 Rows = (from row in dataRows
                                         select row.Where(x => x.GetType() != typeof (SpacerCellItem<double>)).ToList()
                                         into values
                                         select
                                             dataHeader.Select(
                                                 (t, i) => new KeyValuePair<string, object>(t, ((dynamic) values[i]).V))
                                                       .ToList()
                                         into cells
                                         select new ExportAllModel.Row() {Cells = cells}).ToList()
                             };

            return new CsvResult(export);
        }

       private static IEnumerable<string> GetColumns(StudentPriorYearMetricListModel results)
       {
           return (from metadataColumnGroup in results.ListMetadata from column in metadataColumnGroup.Columns select column.ColumnName).ToList();
       }
    }
}
