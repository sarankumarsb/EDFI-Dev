// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resource.Models.Common;

namespace EdFi.Dashboards.Resources.Models.Student
{
    /// <summary>
    /// Class used for List of student drilldowns like: school student list and Teacher views.
    /// It is a composition of Basic Student Info and the metrics related to the student.
    /// </summary>
    [Serializable]
    public class StudentWithMetrics : IStudent
    {
        public StudentWithMetrics()
        {
            Links = new List<Link>();
            Metrics = new List<Metric>();
		}

        public StudentWithMetrics(long studentUSI) : this()
        {
            StudentUSI = studentUSI;
        }

        public long StudentUSI { get; set; }
        public int? SchoolId { get; set; }
        public string SchoolName { get; set; }
        public string Name { get; set; }
        public string ThumbNail { get; set; }
        public bool IsFlagged { get; set; }
        public Link Href { get; set; }
        public List<Link> Links { get; set; }
        public string GradeLevelDisplayValue { get; set; }
        public int GradeLevel { get; set; }
        public string StudentUniqueID { get; set; }

        public List<Metric> Metrics { get; set; }

        [NonSerialized]
        private Dictionary<int, Metric> metricsByUniqueIdentifier;

        public Metric GetMetricByUniqueIdentifier(int uniqueIdentifier)
        {
            if (metricsByUniqueIdentifier == null)
            {
                metricsByUniqueIdentifier = new Dictionary<int, Metric>();

                foreach (var metric in Metrics)
                    metricsByUniqueIdentifier[metric.UniqueIdentifier] = metric;
            }

            Metric returnMetric;
            metricsByUniqueIdentifier.TryGetValue(uniqueIdentifier, out returnMetric);
            return returnMetric;
        }

        [Serializable]
        public class Metric : IStudent
        {
            public Metric() {}
            
            public Metric(long studentUSI)
            {
                StudentUSI = studentUSI;
            }

            public long StudentUSI { get; set; }
            public int UniqueIdentifier { get; set; }
            public int MetricVariantId { get; set; }
            public string DisplayValue { get; set; }
            public dynamic Value { get; set; }
            public MetricStateType State { get; set; }
        }

        [Serializable]
        public class IndicatorMetric : Metric
        {
            public IndicatorMetric() {}
            
            public IndicatorMetric(long studentId) : base(studentId) { }

            public int MetricIndicator { get; set; } // This is an int value that is passed to be used in the call to the javascript function getMetricIndicator(indicator) in the EdFiGridHTMLTemplateHelper.js file
        }

        [Serializable]
        public class TrendMetric : Metric
        {
            public TrendMetric() {}
            
            public TrendMetric(long studentId) : base(studentId) { }

            public TrendEvaluation Trend { get; set; }
        }
    }

    [Serializable]
    public class StudentWithMetricsAndPrimaryMetric : StudentWithMetrics
    {
        public StudentWithMetricsAndPrimaryMetric() {}
        
        public StudentWithMetricsAndPrimaryMetric(long studentUSI) : base(studentUSI) { }

        //For the primary metric in context that is not a part of the additional metric List
        public dynamic PrimaryMetricValue { get; set; }
        public string PrimaryMetricDisplayValue { get; set; }
    }

    [Serializable]
    public class StudentWithMetricsAndAccommodations : StudentWithMetrics
    {
        public StudentWithMetricsAndAccommodations()
        {
            Accommodations = new List<Accommodations>();
        }
        
        public StudentWithMetricsAndAccommodations(long studentUSI) : base(studentUSI)
        {
            Accommodations = new List<Accommodations>();
        }

        public List<Accommodations> Accommodations { get; set; }
    }

    [Serializable]
    public class StudentWithMetricsAndAssessments : StudentWithMetrics
    {
        public StudentWithMetricsAndAssessments() {}

        public StudentWithMetricsAndAssessments(long studentUSI) : base(studentUSI){}

        public IndicatorMetric Score { get; set; }

        [Serializable]
        public class AssessmentMetric : Metric
        {
            public AssessmentMetric() {}
            
            public AssessmentMetric(long studentUSI) : base(studentUSI){}

            public string ObjectiveName { get; set; }
        }
    }
}
