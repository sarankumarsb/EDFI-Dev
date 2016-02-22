// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Metric.Resources.Providers
{
    public interface IMetricNodeResolver
    {
        int ResolveMetricId(int metricVariantId);
        MetricVariantType ResolveMetricVariantType(int metricVariantId);
        MetricMetadataNode ResolveFromMetricVariantId(int metricVariantId);
        MetricMetadataNode GetMetricNodeForStudentFromMetricVariantId(int schoolId, int metricVariantId);
        MetricMetadataNode GetMetricNodeForSchoolFromMetricVariantId(int schoolId, int metricVariantId);
        MetricMetadataNode GetMetricNodeForLocalEducationAgencyMetricVariantId(int metricVariantId);
        IEnumerable<MetricMetadataNode> ResolveFromMetricId(int metricId);
        IEnumerable<MetricMetadataNode> GetMetricNodesForStudentFromMetricId(int schoolId, int metricId);
        IEnumerable<MetricMetadataNode> GetMetricNodesForSchoolFromMetricId(int schoolId, int metricId);
        IEnumerable<MetricMetadataNode> GetMetricNodesForLocalEducationAgencyFromMetricId(int metricId);
    }
}
