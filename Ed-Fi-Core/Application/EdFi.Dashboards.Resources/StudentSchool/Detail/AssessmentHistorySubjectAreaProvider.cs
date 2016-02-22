using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Core;

namespace EdFi.Dashboards.Resources.StudentSchool.Detail
{
    public interface IAssessmentHistorySubjectAreaProvider
    {
        string GetSubjectArea(int? metricVariantId);
    }

    public class AssessmentHistorySubjectAreaProvider : IAssessmentHistorySubjectAreaProvider
    {
        public string GetSubjectArea(int? metricVariantId)
        {
            switch (metricVariantId)
            {
                case (int)StudentMetricEnum.AdvancedCourseEnrollmentELA:
                    return "ELA";
                case (int)StudentMetricEnum.AdvancedCourseEnrollmentMath:
                    return "Mathematics";
                case (int)StudentMetricEnum.AdvancedCourseEnrollmentScience:
                    return "Science";
                case (int)StudentMetricEnum.AdvancedCourseEnrollmentSocialStudies:
                    return "Social Studies";
                case (int)StudentMetricEnum.CourseGradeAlgebraI:
                    return "Mathematics";
            }
            return String.Empty;
        }
    }
}
