// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

namespace EdFi.Dashboards.Presentation.Core.Areas.StudentSchool.Models.Shared
{
    public class TermCoursesModel
    {
        public EdFi.Dashboards.Resources.Models.Student.Detail.CurrentCourses.Semester Model { get; set; }
        public bool DisplayCreditText { get; set; }
		public string TermKey { get; set; }
    }
}