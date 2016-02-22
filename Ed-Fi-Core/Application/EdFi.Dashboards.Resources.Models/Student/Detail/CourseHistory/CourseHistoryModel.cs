// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Models.Student.Detail.CourseHistory
{
	[Serializable]
	public class CourseHistoryModel : IStudent
	{
        public CourseHistoryModel()
        {
            SubjectAreas = new List<SubjectArea>();
        }

        public long StudentUSI { get; set; }
        public List<SubjectArea> SubjectAreas { get; set; }
        public decimal? CumulativeCreditsEarned { get; set; }
	}

    [Serializable]
    public class SubjectArea : IStudent
    {
        public SubjectArea()
        {
            Courses = new List<Course>();
        }

        public long StudentUSI { get; set; }
        public string Name { get; set; }
        public decimal? TotalCreditsEarned { get; set; }
        public List<Course> Courses { get; set; }
    }

    [Serializable]
    public class Course : IStudent
    {
        public Course()
        {
            ActualSemester = new Semester();
            FinalGrade = new Grade();
        }

        public long StudentUSI { get; set; }
        public Semester ActualSemester { get; set; }
        public string LocalCourseCode { get; set; }
        public string CourseTitle { get; set; }
        public string SubjectArea { get; set; }
        public string Instructor { get; set; }
        public string GradeLevel { get; set; }
        public decimal? CreditsEarned { get; set; }
        public Grade FinalGrade { get; set; }
    }

    [Serializable]
    public class Semester : IStudent
    {
        /// <summary>
        /// The term of the semester. Fall, Spring, Summer, Year Round
        /// </summary>
        public string TermType { get; set; }
        public long StudentUSI { get; set; }
    }

    [Serializable]
    public class Grade : IStudent
    {
        public long StudentUSI { get; set; }
        public string Value { get; set; }
    }
}
