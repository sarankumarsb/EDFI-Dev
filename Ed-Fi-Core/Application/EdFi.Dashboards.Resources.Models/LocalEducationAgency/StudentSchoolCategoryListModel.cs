using System;
using System.Collections.Generic;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;

namespace EdFi.Dashboards.Resources.Models.LocalEducationAgency
{
    [Serializable]
    public class StudentSchoolCategoryListModel
    {
        public StudentSchoolCategoryListModel()
        {
            ListMetadata = new List<MetadataColumnGroup>();
            Students = new List<StudentWithMetrics>();
            EntityIds = new List<StudentSchoolIdentifier>();
        }

        public List<MetadataColumnGroup> ListMetadata { get; set; }
        public List<StudentWithMetrics> Students { get; set; }
        public List<StudentSchoolIdentifier> EntityIds { get; set; }
        public int LocalEducationAgencyId { get; set; }
        public SchoolCategory SelectedSchoolCategory { get; set; }
    }
}
