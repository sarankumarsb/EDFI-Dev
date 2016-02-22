// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Models.Common
{
    [Serializable]
    public class Demographics
    {
        public Demographics()
        {
            Female = new AttributeItemWithTrend<decimal?>();
            Male = new AttributeItemWithTrend<decimal?>();
            ByRace = new List<AttributeItemWithTrend<decimal?>>();
            ByEthnicity = new List<AttributeItemWithTrend<decimal?>>();
        }

        public AttributeItemWithTrend<decimal?> Female { get; set; }
        public AttributeItemWithTrend<decimal?> Male { get; set; }
        public List<AttributeItemWithTrend<decimal?>> ByRace { get; set; }
        public List<AttributeItemWithTrend<decimal?>> ByEthnicity { get; set; }
    }
}
