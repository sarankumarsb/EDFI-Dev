// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Models.Common;

namespace EdFi.Dashboards.Resources.Models.Staff
{
    [Serializable]
    public class StaffModel
    {
        public StaffModel()
        {
            SectionsAndCohorts = new List<SectionOrCohort>();
            Schools = new List<School>();
            Views = new List<View>();
        }

        public long StaffUSI { get; set; }
        public string FullName { get; set; }
        public string ProfileThumbnail { get; set; }
        public School CurrentSchool { get; set; }
        public List<School> Schools { get; set; }
        public SectionOrCohort CurrentSection { get; set; }
        public List<SectionOrCohort> SectionsAndCohorts { get; set; }
        public List<View> Views { get; set; }

        [Serializable]
        public class School
        {
            public int SchoolId { get; set; }
            public string Name { get; set; }
            public Link Link { get; set; }
            public bool Selected { get; set; }
        }
        
        [Serializable]
        public class SectionOrCohort
        {
            public SectionOrCohort()
            {
                ChildSections = new List<SectionOrCohort>();
                PageOptions = new List<NameValueModel>();
            }

            public long SectionId { get; set; }
            public string Description { get; set; }
            public StudentListType ListType { get; set; }
            public bool Selected { get; set; }
            public Link Link { get; set; }
            public List<NameValueModel> PageOptions { get; set; }
            public List<SectionOrCohort> ChildSections { get; set; }
        }

        [Serializable]
        public class View
        {
            public View()
            {
                ChildViews = new List<View>();
            }

            public string Description { get; set; }
            public Link Link { get; set; }
            public bool Selected { get; set; }
            public int MenuLevel { get; set; }
            public string MenuSubType { get; set; }
            public List<View> ChildViews { get; set; }
        }

        public enum ViewType
        {
            GeneralOverview,
            SubjectSpecificOverview,
            AssessmentDetails,
            PriorYear
        }

        public enum AssessmentSubType
        {
            StateStandardized,
            Benchmark,
            Reading
        }

        public enum SubjectArea
        {
            None,

            ELA,
            Writing,
            Mathematics,
            Science,
            SocialStudies
        }
    }
}
