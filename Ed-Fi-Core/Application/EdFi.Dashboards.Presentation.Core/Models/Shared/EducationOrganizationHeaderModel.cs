// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EdFi.Dashboards.Presentation.Core.Models.Shared
{
    public class EducationOrganizationHeaderModel
    {
        public EducationOrganizationHeaderModel()
        {
            AssociatedEducationOrganizations = new List<AssociatedEducationOrganization>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ProfileThumbnail { get; set; }
        public IList<AssociatedEducationOrganization> AssociatedEducationOrganizations { get; private set; }

        public class AssociatedEducationOrganization
        {
            public string Name { get; set; }
            public string Href { get; set; }
        }
    }
}