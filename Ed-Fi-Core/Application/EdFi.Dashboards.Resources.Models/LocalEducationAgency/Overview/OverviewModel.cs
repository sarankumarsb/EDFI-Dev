// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Models.LocalEducationAgency.Overview
{
    [Serializable]
    public class OverviewModel 
    {
        public int LocalEducationAgencyId { get; set; }
        public string LocalEducationAgencyName { get; set; }
        public string ProfileThumbnail { get; set; }
        public int MetricVariantId { get; set; }
        public string RenderingMode { get; set; }
        public IEnumerable<AccountabilityRating> AccountabilityRatings { get; set; }
    }

    [Serializable]
    public class AccountabilityRating
    {
        public string Attribute { get; set; }
        public string Value { get; set; }
    }
}
