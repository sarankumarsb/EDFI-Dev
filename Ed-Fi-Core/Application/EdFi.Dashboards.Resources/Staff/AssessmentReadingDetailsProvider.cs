using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Resources.Staff
{
    public interface IAssessmentReadingDetailsProvider
    {
        int[] GetMetricIdsForObjectives(dynamic metricAssessmentArea);
        string GetObjectiveColumnWidth();
    }

    public class AssessmentReadingDetailsProvider : IAssessmentReadingDetailsProvider
    {
        public virtual int[] GetMetricIdsForObjectives(dynamic metricAssessmentArea)
        {
            return new int[] { };
        }

        public virtual string GetObjectiveColumnWidth()
        {
            return string.Empty;
        }
    }
}
