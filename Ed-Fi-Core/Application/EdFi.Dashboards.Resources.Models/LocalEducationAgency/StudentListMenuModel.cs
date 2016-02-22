using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Models.Common;

namespace EdFi.Dashboards.Resources.Models.LocalEducationAgency
{
    [Serializable]
    public class StudentListMenuModel
    {
        public long SectionId { get; set; }
        public string Description { get; set; }
        public StudentListType ListType { get; set; }
        public bool Selected { get; set; }
        public string Href { get; set; }
		public string MenuType { get; set; }
    }
}
