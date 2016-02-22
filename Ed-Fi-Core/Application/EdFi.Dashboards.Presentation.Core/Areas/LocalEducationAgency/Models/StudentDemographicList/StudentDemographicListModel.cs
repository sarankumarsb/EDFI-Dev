using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;

namespace EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Models.StudentDemographicList
{
    public class StudentDemographicListModel
    {
        public string Demographic { get; set; }
        public StudentDemographicMenuModel MenuModel { get; set; }
        public GridTable GridData { get; set; }
        public string PreviousNextSessionPage { get; set; }
        public string ExportGridDataUrl { get; set; }
        public ListType ListType { get; set; }
    }
}
