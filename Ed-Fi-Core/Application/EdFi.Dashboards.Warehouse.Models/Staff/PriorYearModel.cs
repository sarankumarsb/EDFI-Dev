// *************************************************************************
// �2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;

namespace EdFi.Dashboards.Warehouse.Resource.Models.Staff
{
    [Serializable]
    public class PriorYearModel
    {
        public PriorYearModel()
        {
            ListMetadata = new List<MetadataColumnGroup>();
            Students = new List<StudentWithMetricsAndAccommodations>();
        }

        public List<MetadataColumnGroup> ListMetadata { get; set; }
        public List<StudentWithMetricsAndAccommodations> Students { get; set; }
        public SchoolCategory SchoolCategory { get; set; }
        public string UniqueListId { get; set; }
    }
}
