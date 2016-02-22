using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;

namespace EdFi.Dashboards.Resources.Models.School
{
    [Serializable]
    public class StudentDemographicListModel
    {
        public StudentDemographicListModel()
        {
            ListMetadata = new List<MetadataColumnGroup>();
            Students = new List<StudentWithMetrics>();
            EntityIds = new List<StudentSchoolIdentifier>();
        }

        public List<MetadataColumnGroup> ListMetadata { get; set; }
        public List<StudentWithMetrics> Students { get; set; }
        public List<StudentSchoolIdentifier> EntityIds { get; set; }
        public int SchoolId { get; set; }
        public string SelectedDemographic { get; set; }
    }
}
