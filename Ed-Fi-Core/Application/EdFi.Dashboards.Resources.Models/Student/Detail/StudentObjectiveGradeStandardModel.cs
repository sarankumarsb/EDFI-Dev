// *************************************************************************
// ?
// *************************************************************************
using System;
using System.Collections.Generic;
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Resources.Models.Student.Detail
{

    [Serializable]
    public class StudentObjectiveGradeStandardModel 
    {

        public StudentObjectiveGradeStandardModel()
        {
            Grades = new List<StudentObjectiveGradeStandardModel.GradeModel>();
            Benchmarks = new List<StudentObjectiveGradeStandardModel.BenchmarkModel>();
        }

        public string ObjectiveDescription { get; set; }
        public string ObjectiveItem { get; set; }
        public string ObjectiveName { get; set; }
        public List<StudentObjectiveGradeStandardModel.GradeModel> Grades { get; set; }
        public List<StudentObjectiveGradeStandardModel.BenchmarkModel> Benchmarks { get; set; }

        // nested grade model
        [Serializable]
        public class GradeModel
        {

            public GradeModel()
            {
                Standards = new List<StudentObjectiveGradeStandardModel.StandardModel>();

            }

            public string GradeLevel { get; set; }
            public int GradeSort { get; set; }
            public int? MetricStateTypeId { get; set; }
            public string Value { get; set; }
            public List<StudentObjectiveGradeStandardModel.StandardModel> Standards { get; set; }

        }

        // nested standard model
        [Serializable]
        public class StandardModel 
        {

            public StandardModel()
            {
                Assessments = new List<StudentObjectiveGradeStandardModel.AssessmentModel>();
            }

            public int Version { get; set; }
            public string LearningStandard { get; set; }
            public string Description { get; set; }
            public string GradeLevel { get; set; }
            public StudentObjectiveGradeStandardModel.AssessmentModel AssessmentLast { get; set; }
            public List<StudentObjectiveGradeStandardModel.AssessmentModel> Assessments { get; set; }

        }

        // nested assessment model
        [Serializable]
        public class AssessmentModel 
        {

            public string AssessmentTitle { get; set; }
            public DateTime DateAdministration { get; set; }
            public int? MetricStateTypeId { get; set; }
            public string Value { get; set; }

        }

        // nested benchmark model
        [Serializable]
        public class BenchmarkModel
        {

            public BenchmarkModel() 
            {
                Assessments = new List<BenchmarkAssessmentModel>();
            }

            public string GradeLevel { get; set; }
            public List<BenchmarkAssessmentModel> Assessments { get; set; }

        }

        // nested benchmark assessment model
        [Serializable]
        public class BenchmarkAssessmentModel
        {

            public BenchmarkAssessmentModel() { }

            public DateTime DateAdministration { get; set; }
            public string AssessmentTitle { get; set; }
            public int Version { get; set; }
            public string Value { get; set; }
            public string ValueType { get; set; }
            public int? TrendDirection { get; set; }

        }

    }

}
