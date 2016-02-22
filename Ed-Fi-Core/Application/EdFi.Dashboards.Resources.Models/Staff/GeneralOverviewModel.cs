// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;

namespace EdFi.Dashboards.Resources.Models.Staff
{
    [Serializable]
    public class GeneralOverviewModel
    {
        public GeneralOverviewModel()
        {
            ListMetadata = new List<MetadataColumnGroup>();
            EntityIds = new List<StudentSchoolIdentifier>();
            Students = new List<StudentWithMetricsAndAccommodations>();
        }

        public List<MetadataColumnGroup> ListMetadata { get; set; }
        public List<StudentWithMetricsAndAccommodations> Students { get; set; }
        public List<StudentSchoolIdentifier> EntityIds { get; set; }
    }
}
