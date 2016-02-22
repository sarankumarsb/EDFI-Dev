using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Resources.Models.Common;

namespace EdFi.Dashboards.Resources.Models.LocalEducationAgency
{
    [Serializable]
    public class StudentDemographicMenuModel
    {
        public StudentDemographicMenuModel()
        {
			Gender = new List<AttributeItemWithSelected<decimal>>();
			Ethnicity = new List<AttributeItemWithSelected<decimal>>();
			Race = new List<AttributeItemWithSelected<decimal>>();
			Program = new List<AttributeItemWithSelected<decimal>>();
			Indicator = new List<AttributeItemWithSelected<decimal>>();
			WatchLists = new List<AttributeItemWithSelected<string>>();
        }

		public IList<AttributeItemWithSelected<decimal>> Gender { get; set; }
		public IList<AttributeItemWithSelected<decimal>> Ethnicity { get; set; }
		public IList<AttributeItemWithSelected<decimal>> Race { get; set; }
		public IList<AttributeItemWithSelected<decimal>> Program { get; set; }
		public IList<AttributeItemWithSelected<decimal>> Indicator { get; set; }
		public IList<AttributeItemWithSelected<string>> WatchLists { get; set; }
    }
}
