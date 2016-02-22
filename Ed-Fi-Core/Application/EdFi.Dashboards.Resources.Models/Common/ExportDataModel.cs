using EdFi.Dashboards.Resources.Models.CustomGrid;
using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Models.Common
{
    public class ExportDataModel
    {
        public List<MetadataColumnGroup> ExportColumns { get; set; }
        public List<Models.Student.StudentWithMetricsAndAccommodations> ExportRows { get; set; }
    }
}
