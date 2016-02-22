// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Resources.StudentSchool
{
    public interface IGradeStateProvider
    {
        MetricStateType Get(decimal? grade);

        MetricStateType Get(string grade);

        MetricStateType Get(int? grade, int schoolYear);

        MetricStateType Get(string grade, int schoolYear);
    }

    public class GradeStateProvider : IGradeStateProvider
    {
        public MetricStateType Get(decimal? grade)
        {
            if (!grade.HasValue)
                return MetricStateType.None;

            return (grade >= 70) ? MetricStateType.Good : MetricStateType.Low;
        }

        public MetricStateType Get(string grade)
        {
            if (string.IsNullOrEmpty(grade))
                return MetricStateType.None;

            //If its below C- or above G meaning D,D+,D-,F,F+,F- but not elementary S,H,etc...
            if (grade.Substring(0, 1).ToUpper().CompareTo("C") == 1 && grade.ToUpper().CompareTo("G") == -1)
                return MetricStateType.Low;

            return MetricStateType.Good;
        }

        public MetricStateType Get(int? grade, int schoolYear)
        {
            return Get(grade);
        }

        public MetricStateType Get(string grade, int schoolYear)
        {
            return Get(grade);
        }
    }
}
