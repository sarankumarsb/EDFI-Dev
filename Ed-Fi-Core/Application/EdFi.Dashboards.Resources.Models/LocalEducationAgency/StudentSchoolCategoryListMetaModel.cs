using System;
using System.Collections.Generic;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Resources.Models.CustomGrid;

namespace EdFi.Dashboards.Resources.Models.LocalEducationAgency
{
    [Serializable]
    public class StudentSchoolCategoryListMetaModel
    {
        public List<MetadataColumnGroup> ListMetadata { get; set; }
        public string SelectedSchoolCategory { get; set; }
    }
}