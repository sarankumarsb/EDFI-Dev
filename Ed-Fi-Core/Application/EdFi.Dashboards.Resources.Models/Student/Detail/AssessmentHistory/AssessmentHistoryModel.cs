// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Resources.Models.Student.Detail.AssessmentHistory
{
	[Serializable]
	public class AssessmentHistoryModel : IStudent
	{
        public AssessmentHistoryModel()
        {
            SubjectAreas = new List<SubjectArea>();
        }

	    public long StudentUSI { get; set; }

        public List<SubjectArea> SubjectAreas { get; set; }
	}

    [Serializable]
    public class SubjectArea : IStudent
    {
        public SubjectArea()
        {
            Assessments = new List<Assessment>();
        }

        public long StudentUSI { get; set; }
        public string Name { get; set; }
        public List<Assessment> Assessments { get; set; }
    }

    [Serializable]
    public class Assessment : IStudent
    {
        public Assessment()
        {
            ScoreState = new State();
        }

        public long StudentUSI { get; set; }
        public string AssessmentTitle { get; set; }
        public short SchoolYear { get; set; }
        public DateTime DateTaken { get; set; }
        public string GradeLevel { get; set; }
        public string Accommodations { get; set; }
        public string Score { get; set; }
        public State ScoreState { get; set; }
        public string MetMinimumScore { get; set; }
        public string MetStandardScore { get; set; }
        public string CommendedScore { get; set; }
    }
}
