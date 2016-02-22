// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using EdFi.Dashboards.Resource.Models.Common;

namespace EdFi.Dashboards.Resources.Models.Admin
{
    [Serializable]
    public class ConfigurationModel : ResourceModelBase
    {
        public string LocalEducationAgencyName { get; set; }
        public int LocalEducationAgencyId { get; set; }
        public bool IsKillSwitchActivated { get; set; }
        public string SystemMessage { get; set; }
    }
}
