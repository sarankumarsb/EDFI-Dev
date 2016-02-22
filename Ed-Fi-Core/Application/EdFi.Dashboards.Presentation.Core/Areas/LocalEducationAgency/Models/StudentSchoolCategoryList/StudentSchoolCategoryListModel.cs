using EdFi.Dashboards.Core;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;

namespace EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Models.StudentSchoolCategoryList
{
    public class StudentSchoolCategoryListModel
    {
        public string Title { get; set; }
        public string Level { get; set; }
        public StudentSchoolCategoryMenuModel MenuModel { get; set; }
        public GridTable GridData { get; set; }
        public string PreviousNextSessionPage { get; set; }
        public string ExportGridDataUrl { get; set; }
        public ListType ListType { get; set; }
    }
}
