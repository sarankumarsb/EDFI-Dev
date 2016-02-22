// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;

namespace EdFi.Dashboards.Resources.Models.Common
{
    [Serializable]
    public class LevelCrumbModel
    {
        public LocalEducationAgency.BriefModel LocalEducationAgencyBriefModel { get; set; }
        public School.BriefModel SchoolBriefModel { get; set; }
        public Staff.BriefModel BriefModel { get; set; }

        /// <summary>
        /// The Home Icon resource image path.
        /// </summary>
        public string HomeIconHref { get; set; }

        /// <summary>
        /// The URL to take the user to when he clicks on the Home Icon.
        /// </summary>
        public string HomeHref { get; set; }
    }
}
