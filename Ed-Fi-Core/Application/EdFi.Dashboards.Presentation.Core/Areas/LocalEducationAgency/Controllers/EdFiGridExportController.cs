using EdFi.Dashboards.Common;
using EdFi.Dashboards.Presentation.Architecture.Mvc.ActionResults;
using EdFi.Dashboards.Presentation.Web.Utilities;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers
{
    public class EdFiGridExportController : Controller
    {
        protected readonly IService<EdFiGridExportRequest, ExportDataModel> EdFiGridExportService;

        public EdFiGridExportController(IService<EdFiGridExportRequest, ExportDataModel> edFiGridExportService)
        {
            EdFiGridExportService = edFiGridExportService;
        }

        [HttpPost]
        public CsvResult Get(EdFiGridExportRequest request)
        {
            // for some reason the json being sent from the export form isn't
            // properly serialized so this will do the serialization
            var serializer = new JavaScriptSerializer();
            request.StudentWatchListData = serializer.Deserialize<List<NameValuesType>>(request.StudentWatchListJson);

            var exportResult = EdFiGridExportService.Get(request);

            // flatten out the columns for the export
            var exportColumns = new List<MetadataColumn>();
            foreach (var columnGroup in exportResult.ExportColumns)
            {
                exportColumns.AddRange(columnGroup.Columns);
            }

            // use the column groups to create the row data
            var exportRows =
                exportResult.ExportColumns.GenerateRowsForExport(exportResult.ExportRows.ToList<StudentWithMetrics>());

            var export = new ExportAllModel();
            var rows =
                exportRows.Select(
                    exportRow =>
                        exportRow.Select(
                            (t, i) => new KeyValuePair<string, object>(exportColumns[i].ColumnName, GetCellItemValue(t)))
                            .ToList()).Select(cells => new ExportAllModel.Row
                            {
                                Cells = cells
                            }).ToList();

            export.Rows = rows;
            return new CsvResult(export);
        }

        private static object GetCellItemValue(object cellItem)
        {
            var type = cellItem.GetType();
            return type.GetProperty("DV").GetValue(cellItem, null);
        }
    }
}
