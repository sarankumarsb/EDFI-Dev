// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.School;

namespace EdFi.Dashboards.Resources.Metric
{
    public interface IRootMetricNodeResolver
    {
        MetricMetadataNode GetRootMetricNode();

        MetricMetadataNode GetRootMetricNodeForStudent(int schoolId);
        MetricMetadataNode GetRootMetricNodeForSchool(int schoolId);
        MetricMetadataNode GetRootMetricNodeForLocalEducationAgency();

        MetricHierarchyRoot GetMetricHierarchyRoot(int schoolId);
    }
}
