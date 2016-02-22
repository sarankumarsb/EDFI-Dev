using System.Collections.Generic;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;

namespace EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Models.StudentList
{
    public class StudentListModel
    {
        public bool IsCurrentUserListOwner { get; set; }
        public bool IsCustomStudentList { get; set; }
        public long ListId { get; set; }

        public GridTable GridTable { get; set; }

        public List<string> LegendViewNames { get; set; }

        public string Cohort { get; set; }
        public IEnumerable<StudentListMenuModel> MenuModel { get; set; }
        public IEnumerable<StudentListMenuModel> WatchListMenuModel { get; set; }
    }
}