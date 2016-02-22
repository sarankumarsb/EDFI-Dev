using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resource.Models.Common;

namespace EdFi.Dashboards.Resources.Models.Staff
{
    [Serializable]
    public class StaffCohortModel : ResourceModelBase
    {
        public long StaffUSI { get; set; }
        public List<StaffCohortItem> Cohorts { get; set; }
    }

    [Serializable]
    public class StaffCohortItem
    {
        public int StaffCohortId { get; set; }
        public string CohortName { get; set; }
    }
}
