// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Resources.Models.Staff
{
    [Serializable]
    public class StaffSectionModel
    {
        public long StaffUSI { get; set; }
        public string FullName { get; set; }
        public string ProfileThumbnail { get; set; }
        public IList<Cohort> Cohorts { get; set; }
        public IList<Section> Sections { get; set; }
        public int CurrentSchoolId { get; set; }
        public IList<School> Schools { get; set; }
        public string CurrentSectionId { get; set; }

        [Serializable]
        public class Cohort
        {
            public int CohortId { get; set; }
            public string Description { get; set; }
        }

        [Serializable]
        public class Section
        {
            public int SectionId { get; set; }
            public string Description { get; set; }
        }

        [Serializable]
        public class School
        {
            public int SchoolId { get; set; }
            public string Name { get; set; }
        }
    }
}
