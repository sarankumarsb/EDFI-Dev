// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Resources.Metric
{
    public interface IDomainSpecificMetricNodeResolver
    {
        MetricMetadataNode GetOperationalDashboardMetricNode(MetricInstanceSetType metricInstanceSetType = MetricInstanceSetType.None, int? schoolId = null);
        MetricMetadataNode GetAdvancedCoursePotentialMetricNode();
        MetricMetadataNode GetAdvancedCourseEnrollmentMetricNode();
        MetricMetadataNode GetSchoolHighSchoolGraduationPlan();
    }
}
