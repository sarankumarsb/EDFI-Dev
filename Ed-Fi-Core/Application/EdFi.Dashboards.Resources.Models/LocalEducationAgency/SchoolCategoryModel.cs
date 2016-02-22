// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resource.Models.Common;

namespace EdFi.Dashboards.Resources.Models.LocalEducationAgency
{
    [Serializable]
    public class SchoolCategoryModel
    {
        public SchoolCategoryModel()
        {
            Schools = new List<School>();
        }
        public string Category { get; set; }
        public IEnumerable<School> Schools { get; set; }

        [Serializable]
        public class School : ResourceModelBase
        {
            public int SchoolId { get; set; }
            public string Name { get; set; }
        }
    }
}
