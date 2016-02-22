// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resource.Models.Common;

namespace EdFi.Dashboards.Resources.Models.School
{
    [Serializable]
    public class TeachersModel
    {
        public TeachersModel()
	    {
            Teachers = new List<Teacher>();
	    }

        public IEnumerable<Teacher> Teachers { get; set; }

        [Serializable]
        public class Teacher : ResourceModelBase
	    {
            public long StaffUSI { get; set; }
            public string Name { get; set; }
            public string ProfileThumbnail { get; set; }
            public string EmailAddress { get; set; }
            public DateTime? DateOfBirth { get; set; }
	        public string Gender { get; set; }
            public decimal? YearsOfPriorProfessionalExperience { get; set; }
            public string HighestLevelOfEducationCompleted { get; set; }
            public string HighlyQualifiedTeacher { get; set; }
	    }
    }
}
