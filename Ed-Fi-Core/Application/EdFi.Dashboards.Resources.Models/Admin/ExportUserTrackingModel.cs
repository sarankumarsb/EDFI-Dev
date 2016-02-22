// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resource.Models.Common;

namespace EdFi.Dashboards.Resources.Models.Admin
{
    [Serializable]
    public class ExportUserTrackingModel : ResourceModelBase
    {
        public ExportUserTrackingModel()
        {
            Rows = new List<SiteUser>();
        }

        public IEnumerable<SiteUser> Rows { get; set; }

        [Serializable]
        public class SiteUser
        {
            public long StaffUSI { get; set; }
            public string FullName { get; set; }
            public string EmailAddress { get; set; }
            public string TrackingCode { get; set; }
        }
    }
}
