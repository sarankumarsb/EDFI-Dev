using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.School;

namespace EdFi.Dashboards.Presentation.Core.Areas.School.Models.StudentGradeList
{
    public class StudentGradeListModel
    {
        public StudentGradeListModel()
        {
            Grade = string.Empty;    
        }

        public string Grade { get; set; }
        public StudentGradeMenuModel MenuModel { get; set; }
        public GridTable GridData { get; set; }
        public ListType ListType { get; set; }
        public string PreviousNextSessionPage { get; set; }
        public string ExportGridDataUrl { get; set; }
    }
}
