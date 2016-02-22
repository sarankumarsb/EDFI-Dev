// *************************************************************************
// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Models.Student;

namespace EdFi.Dashboards.Resources.Models.Search
{
    [Serializable]
    public class SearchModel
    {
        public SearchModel()
        {
            Students = new List<StudentSearchItem>();
            Schools = new List<SearchItem>();
            Teachers = new List<TeacherSearchItem>();
            Staff = new List<StaffSearchItem>();
        }

        public List<StudentSearchItem> Students { get; set; }
        public List<SearchItem> Schools { get; set; }
        public List<TeacherSearchItem> Teachers { get; set; }
        public List<StaffSearchItem> Staff { get; set; }

        /// <summary>
        /// Gets or sets the total number of schools matching the search criteria, including those not returned due to result set size restrictions.
        /// </summary>
        public int AbsoluteSchoolsCount { get; set; }

        /// <summary>
        /// Gets or sets the total number of teachers matching the search criteria, including those not returned due to result set size restrictions.
        /// </summary>
        public int AbsoluteTeachersCount { get; set; }

        /// <summary>
        /// Gets or sets the total number of students matching the search criteria, including those not returned due to result set size restrictions.
        /// </summary>
        public int AbsoluteStudentsCount { get; set; }

        public int AbsoluteStaffCount { get; set; }

        [Serializable]
        public class SearchItem
        {
            public string Text { get; set; }
            public int SchoolId { get; set; }
            public Link Link { get; set; }
        }

        [Serializable]
        public class StudentSearchItem : SearchItem, IStudent
        {
            public StudentSearchItem()
            {
                GradeLevel = new GradeLevelItem();			
            }
			
            public StudentSearchItem(long studentUSI) : this()
            {
                StudentUSI = studentUSI;
            }

            public long StudentUSI { get; set; }
            public string School { get; set; }
            public GradeLevelItem GradeLevel { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastSurname { get; set; }
            public string IdentificationSystem { get; set; }
            public string IdentificationCode { get; set; }

            [Serializable]
            public class GradeLevelItem
            {
                public GradeLevelItem()
                {
                }

                public GradeLevelItem(string DV, int V)
                {
                    this.DV = DV;
                    this.V = V;
                }

                /// <summary>
                /// The Display Value optimized for JSON transmission.
                /// </summary>
                public string DV { get; set; }

                /// <summary>
                /// The Value used generally for sorting.
                /// </summary>
                public int V { get; set; }
            }
        }

        [Serializable]
        public class TeacherSearchItem : SearchItem
        {
            public long StaffUSI { get; set; }
            public string School { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastSurname { get; set; }
        }

        [Serializable]
        public class StaffSearchItem : SearchItem
        {
            public long StaffUSI { get; set; }
            public int LeaId { get; set; }
            public string Email { get; set; }
            public string Schools { get; set; }
            public string PositionTitle { get; set; }
            public string IdentificationSystem { get; set; }
            public string IdentificationCode { get; set; }
        }

        [IgnoreDataMember]
        public IQueryable<StudentSearchData> StudentQuery { get; set; }

        [Serializable]
        public class StudentSearchData : IStudent
        {
            public long StudentUSI { get; set; }
            public string FullName { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastSurname { get; set; }
            public string GradeLevel { get; set; }
            public int SchoolId { get; set; }
            public string School { get; set; }
            public int LocalEducationAgencyId { get; set; }
            public string LocalEducationAgency { get; set; }
            public string IdentificationSystem { get; set; }
            public string StudentIdentificationCode { get; set; }
        }
    }
}
