// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Models.Student.Detail
{
    [Serializable]
    public class LearningStandardModel : IStudent
    {
        public LearningStandardModel() {}
        
        public LearningStandardModel(long studentUSI)
        {
            StudentUSI = studentUSI;
        }

        public long StudentUSI { get; set; }
        public int MetricId { get; set; }
        public int SchoolId { get; set; }
        public string LearningStandard { get; set; }
        public string Description { get; set; }
        public List<Assessment> Assessments { get; set; }

        [Serializable]
        public class Assessment : IStudent
        {
            public Assessment() {}

            public Assessment(long studentUSI)
            {
                StudentUSI = studentUSI;
            }

            public long StudentUSI { get; set; }
            public DateTime DateAdministration { get; set; }
            public int? MetricStateTypeId { get; set; }
            public string Value { get; set; }
            public string AssessmentTitle { get; set; }
            public int Version { get; set; }
            public bool Administered { get; set; }
        }
    }
}
