// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resources.Models.Student;

namespace EdFi.Dashboards.Resources.Models.Staff
{
    [Serializable]
    public class AssessmentDetailsModel
    {
        public AssessmentDetailsModel()
        {
            Students = new List<StudentWithMetricsAndAssessments>();
            ObjectiveTitles = new List<ObjectiveTitle>();
        }

        public List<StudentWithMetricsAndAssessments> Students { get; set; }
        public List<ObjectiveTitle> ObjectiveTitles { get; set; }
        public string MetricTitle { get; set; }
        public string UniqueListId { get; set; }
        public string FixedRowTitle { get; set; }

        [Serializable]
        public class ObjectiveTitle
        {
            public int UniqueIdentifier { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Mastery { get; set; }
            public string Width { get; set; }
        }
    }
}
