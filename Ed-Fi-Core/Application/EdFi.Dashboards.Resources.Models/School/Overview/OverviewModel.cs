// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resource.Models.Common;

namespace EdFi.Dashboards.Resources.Models.School.Overview
{
    [Serializable]
    public class OverviewModel
    {
        public OverviewModel()
        {
            Accountability = new Accountability();
        }

        public int MetricVariantId { get; set; }
        public string RenderingMode { get; set; }
        public Accountability Accountability { get; set; }
    }

    [Serializable]
    public class Accountability : ResourceModelBase
    {
        public Accountability()
        {
            AccountabilityRatings = new List<AccountabilityRating>();
        }

        public int SchoolId { get; set; }
        public string Name { get; set; }
        public string ProfileThumbnail { get; set; }
        public IList<AccountabilityRating> AccountabilityRatings { get; set; }
    }

    [Serializable]
    public class AccountabilityRating
    {
        public string Attribute { get; set; }
        public string Value { get; set; }
    }
}
