// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resource.Models.Common;

namespace EdFi.Dashboards.Resources.Models.Student
{
    [Serializable]
    public class BriefModel : ResourceModelBase, IStudent
    {
        public BriefModel()
        {
            Accommodations = new List<Accommodations>();
        }

        public long StudentUSI { get; set; }

        public string FullName { get; set; }

        public string Gender { get; set; }

        public string Race { get; set; }

        public string ProfileThumbnail { get; set; }

        public string GradeLevel { get; set; }

        public string Homeroom { get; set; }

        public List<Accommodations> Accommodations { get; set; }
        
    }
}
