using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resources.Models.CustomGrid;

namespace EdFi.Dashboards.Resources.Models.LocalEducationAgency
{
    [Serializable]
    public class StudentDemographicListMetaModel
    {
        public List<MetadataColumnGroup> ListMetadata { get; set; }
        public string SelectedDemographic { get; set; }
    }
}