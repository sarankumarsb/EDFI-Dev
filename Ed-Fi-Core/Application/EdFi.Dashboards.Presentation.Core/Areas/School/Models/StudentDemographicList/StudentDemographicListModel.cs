using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.School;

namespace EdFi.Dashboards.Presentation.Core.Areas.School.Models.StudentDemographicList
{
    public class StudentDemographicListModel
    {
        public StudentDemographicListModel()
        {
            Demographic = string.Empty;    
        }

        public string Demographic { get; set; }
        public string PageTitle { get; set; }
        public string PreviousNextSessionPage { get; set; }
        public string ExportGridDataUrl { get; set; }
        public StudentDemographicMenuModel MenuModel { get; set; }
        public GridTable GridData { get; set; }
        public ListType ListType { get; set; }
    }
}
