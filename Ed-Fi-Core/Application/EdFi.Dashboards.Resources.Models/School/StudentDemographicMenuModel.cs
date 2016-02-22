using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resources.Models.Common;

namespace EdFi.Dashboards.Resources.Models.School
{
    [Serializable]
    public class StudentDemographicMenuModel
    {
        public StudentDemographicMenuModel()
        {
            Gender = new List<AttributeItemWithUrl<decimal>>();
            Ethnicity = new List<AttributeItemWithUrl<decimal>>();
            Race = new List<AttributeItemWithUrl<decimal>>();
            Program = new List<AttributeItemWithUrl<decimal>>();
            Indicator = new List<AttributeItemWithUrl<decimal>>();
        }

        public IList<AttributeItemWithUrl<decimal>> Gender { get; set; }
        public IList<AttributeItemWithUrl<decimal>> Ethnicity { get; set; }
        public IList<AttributeItemWithUrl<decimal>> Race { get; set; }
        public IList<AttributeItemWithUrl<decimal>> Program { get; set; }
        public IList<AttributeItemWithUrl<decimal>> Indicator { get; set; }
    }
}
