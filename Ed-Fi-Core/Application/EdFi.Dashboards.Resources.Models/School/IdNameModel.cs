// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;

namespace EdFi.Dashboards.Resources.Models.School
{
    [Serializable]
    public class IdNameModel
    {
        public int LocalEducationAgencyId { get; set; }
        public int SchoolId { get; set; }
        public string Name { get; set; }
        public string SchoolCategory { get; set; }    // Added to optimize the SchoolCategoryProvider's data access
    }
}
