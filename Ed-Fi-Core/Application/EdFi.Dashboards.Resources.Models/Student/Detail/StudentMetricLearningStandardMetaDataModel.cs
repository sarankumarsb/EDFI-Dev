// *************************************************************************
// ?
// *************************************************************************
using System;
using System.Collections.Generic;
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Resources.Models.Student.Detail
{

    [Serializable]
    public class StudentMetricLearningStandardMetaDataModel 
    {
        public StudentMetricLearningStandardMetaDataModel()
        {
            Grades = new List<StudentMetricLearningStandardMetaDataModel.GradeModel>();
        }

        public StudentMetricLearningStandardMetaDataModel(int metricId)
        {
            MetricId = metricId;
            Grades = new List<StudentMetricLearningStandardMetaDataModel.GradeModel>();
        }

        public int MetricId { get; set; }
        public string LearningObjective { get; set; }
        public List<StudentMetricLearningStandardMetaDataModel.GradeModel> Grades { get; set; }

        // nested grade model
        [Serializable]
        public class GradeModel
        {

            public GradeModel()
            {
                Standards = new List<StudentMetricLearningStandardMetaDataModel.StandardModel>();
            }

            public string GradeLevel { get; set; }
            public int GradeSort { get; set; }
            public List<StudentMetricLearningStandardMetaDataModel.StandardModel> Standards { get; set; }

        }

        // nested standard model
        [Serializable]
        public class StandardModel
        {
            public string LearningStandard { get; set; }
            public string Description { get; set; }
            public string GradeLevel { get; set; }
        }

    }

}
