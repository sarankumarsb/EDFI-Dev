// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using EdFi.Dashboards.Resource.Models.Common;

namespace EdFi.Dashboards.Resources.Models.Staff
{
    [Serializable]
    public class BriefModel : ResourceModelBase
    {
        public long StaffUSI { get; set; }
        public string FullName { get; set; }
        public string ProfileThumbnail { get; set; }
        public string Gender { get; set; }
    }
}
