using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Resources.Models.Common;

namespace EdFi.Dashboards.Resources.Models.LocalEducationAgency
{
    [Serializable]
    public class StudentSchoolCategoryMenuModel
    {
        public StudentSchoolCategoryMenuModel()
        {
			SchoolCategories = new List<AttributeItemWithSelected<string>>();
			DynamicWatchLists = new List<AttributeItemWithSelected<string>>();
        }

		public IList<AttributeItemWithSelected<string>> SchoolCategories { get; set; }
		public IList<AttributeItemWithSelected<string>> DynamicWatchLists { get; set; }
    }
}
