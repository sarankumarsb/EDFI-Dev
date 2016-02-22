using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Resources.Models.CustomGrid;

namespace EdFi.Dashboards.Resources.Models.LocalEducationAgency.Detail
{
    [Serializable]
    public class SchoolMetricTableModel
    {
        public SchoolMetricTableModel()
        {
            ListMetadata = new List<MetadataColumnGroup>();
            SchoolMetrics = new List<SchoolMetricModel>();
        }

        public List<MetadataColumnGroup> ListMetadata { get; set; }
        public List<SchoolMetricModel> SchoolMetrics { get; set; }
    }
}
