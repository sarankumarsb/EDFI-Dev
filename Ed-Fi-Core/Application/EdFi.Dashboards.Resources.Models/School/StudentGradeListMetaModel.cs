using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resources.Models.CustomGrid;

namespace EdFi.Dashboards.Resources.Models.School
{
    [Serializable]
    public class StudentGradeListMetaModel
    {
        public List<MetadataColumnGroup> ListMetadata { get; set; }
        public string SelectedGrade { get; set; }
    }
}
