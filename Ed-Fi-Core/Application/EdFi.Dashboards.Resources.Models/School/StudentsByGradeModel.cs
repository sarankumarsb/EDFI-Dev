// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Models.Student;

namespace EdFi.Dashboards.Resources.Models.School
{
    [Serializable]
    public class StudentsByGradeModel
    {
        public StudentsByGradeModel()
        {
            Grades = new List<Grade>();
        }

        public int SchoolId { get; set; }
        public IEnumerable<Grade> Grades { get; set; }

        [Serializable]
        public class Grade
        {
            public Grade()
            {
                Students = new List<Student>();
            }

            public string GradeLevel { get; set; }
            public IEnumerable<Student> Students { get; set; }
        }
    }
}
