// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Models.Student.Detail.CurrentCourses
{
    [Serializable]
    public class CurrentCoursesModel : IStudent
    {
        public CurrentCoursesModel()
        {
            Semesters = new List<Semester>();
        }

        public long StudentUSI { get; set; }
        public List<Semester> Semesters { get; set; }
    }

    [Serializable]
    public class Semester : IStudent
    {
        public Semester()
        {
            Courses = new List<Course>();
            AvailablePeriods = new List<GradingPeriod>();
        }

        public long StudentUSI { get; set; }

        public string Term { get; set; }

        public List<Course> Courses { get; set; }

        public List<GradingPeriod> AvailablePeriods { get; set; }
    }

    [Serializable]
    public class Course : IStudent
    {
        public Course()
        {
            Grades = new List<Grade>();
        }

        public long StudentUSI { get; set; }
        public string LocalCourseCode { get; set; }
        public string CourseTitle { get; set; }
        public string SubjectArea { get; set; }
        public string Instructor { get; set; }
        public string GradeLevel { get; set; }
        public decimal? CreditsToBeEarned { get; set; }
        public List<Grade> Grades { get; set; }
    }

    [Serializable]
    public class Grade : IStudent
    {
        public long StudentUSI { get; set; }
        public string Value { get; set; }
        public GradingPeriod GradePeriod { get; set; }
    }

    public enum GradingPeriod
    {
        None = 0,
        One = 1, 
        Two = 2, 
        Three = 3, 
        Four = 4, 
        Five = 5, 
        Six = 6, 
        Seven = 7, 
        Eight = 8,
        FinalGrade = 999
    }
}
