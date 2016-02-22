using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Models.Student;

namespace EdFi.Dashboards.Resources.Staff
{
    public interface IAssessmentDetailsProvider
    {
        StudentWithMetrics.IndicatorMetric OnStudentAssessmentInitialized(StudentWithMetricsAndAssessments studentWithMetricsAndAssessments, IEnumerable<StudentMetric> studentMetrics, dynamic subjectArea);
        int[] GetMetricIdsForObjectives(dynamic metricAssessmentArea);
        string GetObjectiveColumnWidth();
        string GetStudentsFixedRowTitle(StaffModel.AssessmentSubType assessmentSubType, dynamic metricAssessmentArea);
    }

    public class AssessmentDetailsProvider: IAssessmentDetailsProvider
    {
        public virtual int[] GetMetricIdsForObjectives(dynamic metricAssessmentArea)
        {
            // this is the default implementation of the provider
            // state specific provider has to be created which will select specific metric IDs needed for the state
            // assessment views will not display any data because of this empty list
            return new int[] { };
        }

        public virtual StudentWithMetrics.IndicatorMetric OnStudentAssessmentInitialized(StudentWithMetricsAndAssessments studentWithMetricsAndAssessments, IEnumerable<StudentMetric> studentMetrics, dynamic subjectArea)
        {
            return new StudentWithMetrics.IndicatorMetric(studentWithMetricsAndAssessments.StudentUSI)
                        {
                            MetricVariantId = -1,
                            MetricIndicator = (int)MetricIndicatorType.None,
                            State = MetricStateType.None,
                            Value = null
                        };
        }

        public virtual string GetObjectiveColumnWidth()
        {
            return string.Empty;
        }

        public virtual string GetStudentsFixedRowTitle(StaffModel.AssessmentSubType assessmentSubType, dynamic metricAssessmentArea)
        {
            return "# Students Mastering Objective";
        }
    }
}
