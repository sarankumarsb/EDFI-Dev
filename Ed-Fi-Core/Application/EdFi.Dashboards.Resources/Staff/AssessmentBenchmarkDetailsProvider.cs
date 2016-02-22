using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.StudentSchool;

namespace EdFi.Dashboards.Resources.Staff
{
    public interface IAssessmentBenchmarkDetailsProvider
    {
        int[] GetMetricIdsForObjectives(dynamic metricAssessmentArea);
        string GetObjectiveColumnWidth();
        StudentWithMetrics.IndicatorMetric OnStudentAssessmentInitialized(
            StudentWithMetricsAndAssessments studentWithMetricsAndAssessments, List<StudentMetric> studentList, StaffModel.SubjectArea subjectArea);
    }

    public class AssessmentBenchmarkDetailsProvider : IAssessmentBenchmarkDetailsProvider
    {
        public IStudentListUtilitiesProvider StudentListUtilitiesProvider { get; set; }

        public virtual int[] GetMetricIdsForObjectives(dynamic metricAssessmentArea)
        {
            return new int[] { };
        }

        public virtual string GetObjectiveColumnWidth()
        {
            return string.Empty;
        }

        public virtual StudentWithMetrics.IndicatorMetric OnStudentAssessmentInitialized(
            StudentWithMetricsAndAssessments studentWithMetricsAndAssessments, List<StudentMetric> studentList, StaffModel.SubjectArea subjectArea)
        {
            System.Diagnostics.Debug.Assert(studentList.Count > 0);
            return new StudentWithMetrics.IndicatorMetric(studentList[0].StudentUSI)
            {
                MetricVariantId = -1,
                MetricIndicator = (int)MetricIndicatorType.None,
                State = MetricStateType.None,
                Value = null
            };
        }
    }
}
