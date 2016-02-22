// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using EdFi.Dashboards.Resources.Models.Student.Detail.AssessmentHistory;
using EdFi.Dashboards.Resources.Models.Student.Detail.CourseHistory;
using EdFi.Dashboards.Resources.Models.Student.Detail.CurrentCourses;

namespace EdFi.Dashboards.Resources.Models.Student.AcademicProfile
{
    [Serializable]
    public class AcademicProfileModel : IStudent
    {
        public long StudentUSI { get; set; }
        public CourseHistoryModel CourseHistory { get; set; }
        public CurrentCoursesModel CurrentCourses { get; set; }
        public AssessmentHistoryModel AssessmentHistory { get; set; }
    }
}
