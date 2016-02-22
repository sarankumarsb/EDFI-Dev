using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Core;

namespace EdFi.Dashboards.Resources.Staff
{
    public interface ISubjectSpecificOverviewMetricComponentProvider
    {
        int[] GetMetricIdsForComponents();
    }

    public class SubjectSpecificOverviewMetricComponentProvider : ISubjectSpecificOverviewMetricComponentProvider
    {
        public int[] GetMetricIdsForComponents()
        {
            return new[] 
            { 
                (int)StudentMetricEnum.AbsenceLevelCurrentPeriod,
                (int)StudentMetricEnum.ClassGradeGradesFalling10PercentOrMore
            };
        }
    }
}
