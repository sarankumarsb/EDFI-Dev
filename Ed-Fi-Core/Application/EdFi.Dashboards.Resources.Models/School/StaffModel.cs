// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Models.School
{
    [Serializable]
    public class StaffModel
    {
        public StaffModel()
        {
            Staff = new List<StaffMember>();
        }

        public IEnumerable<StaffMember> Staff { get; set; }

        [Serializable]
        public class StaffMember
        {
            public long StaffUSI { get; set; }
            public string Name { get; set; }
            public string ProfileThumbnail { get; set; }
            public string EmailAddress { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public string Gender { get; set; }
            public string StaffCategory { get; set; }
            public decimal? YearsOfPriorProfessionalExperience { get; set; }
            public string HighestLevelOfEducationCompleted { get; set; }
            public string HighlyQualifiedTeacher { get; set; }
        }

    }
}
