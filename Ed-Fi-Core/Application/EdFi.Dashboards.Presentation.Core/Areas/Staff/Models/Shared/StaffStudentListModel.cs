using System.Collections.Generic;
using EdFi.Dashboards.Resources.Models.CustomGrid;

namespace EdFi.Dashboards.Presentation.Core.Areas.Staff.Models.Shared
{
    public class StaffStudentListModel
    {
        public bool IsCurrentUserListOwner { get; set; }
        public bool IsCustomStudentList { get; set; }
        public long ListId { get; set; }

        public GridTable GridTable { get; set; }

        public List<string> LegendViewNames { get; set; }

        public string AssessmentSubType { get; set; }
        public string SubjectArea { get; set; }
    }
}