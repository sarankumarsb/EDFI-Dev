// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;

namespace EdFi.Dashboards.Resources.Models.Student.Detail
{
    [Serializable]
    public class CollegeCareerReadinessAssessmentModel : IStudent
    {
        public CollegeCareerReadinessAssessmentModel() {}
        
        public CollegeCareerReadinessAssessmentModel(long studentUSI)
        {
            StudentUSI = studentUSI;
        }

        public long StudentUSI { get; set; }
        public DateTime Date { get; set; }
        public string Subject { get; set; }
        public string Score { get; set; }
        public string StateCriteria { get; set; }
        public bool IsFlagged { get; set; }
    }
}
