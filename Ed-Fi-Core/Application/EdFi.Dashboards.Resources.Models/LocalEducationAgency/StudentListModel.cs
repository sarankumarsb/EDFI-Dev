using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;

namespace EdFi.Dashboards.Resources.Models.LocalEducationAgency
{
    [Serializable]
    public class StudentListModel
    {
        public StudentListModel()
        {
            ListMetadata = new List<MetadataColumnGroup>();
            Students = new List<StudentWithMetrics>();
        }

        public List<MetadataColumnGroup> ListMetadata { get; set; }

        public List<StudentWithMetrics> Students { get; set; }
        
        public int LocalEducationAgencyId { get; set; }

        public SchoolCategory SchoolCategory { get; set; }
        
        public string UniqueListId { get; set; }
    }
}
