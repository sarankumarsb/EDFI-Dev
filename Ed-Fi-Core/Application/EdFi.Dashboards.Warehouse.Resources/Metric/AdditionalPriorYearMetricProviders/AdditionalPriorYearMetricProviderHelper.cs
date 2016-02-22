// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Warehouse.Data.Entities;

namespace EdFi.Dashboards.Warehouse.Resources.Metric.AdditionalPriorYearMetricProviders
{
    public static class AdditionalPriorYearMetricProviderHelper
    {
        public class AdditionalPriorYearMetricProviderHelperResult
        {
            public AdditionalPriorYearMetricProviderHelperResult(long studentUSI, int schoolId, int metricId, int? metricStateTypeId, string value, string valueTypeName, int? trendDirection)
            {
                StudentUSI = studentUSI;
                SchoolId = schoolId;
                MetricId = metricId;
                MetricStateTypeId = metricStateTypeId;
                Value = value;
                ValueTypeName = valueTypeName;
                TrendDirection = trendDirection;
            }

            public long StudentUSI { get; set; }
            public int SchoolId { get; set; }
            public int MetricId { get; set; }
            public int? MetricStateTypeId { get; set; }
            public string Value { get; set; }
            public string ValueTypeName { get; set; }
            public int? TrendDirection { get; set; }
        }

        public static AdditionalPriorYearMetricProviderHelperResult GetMetricToUse(StudentSchoolMetricInstance priorYearMetric, StudentMetric currentMetric, string columnPrefix)
        {
            if (columnPrefix.StartsWith("PriorYear"))
                return new AdditionalPriorYearMetricProviderHelperResult
                    (priorYearMetric.StudentUSI, priorYearMetric.SchoolId, priorYearMetric.MetricId, priorYearMetric.MetricStateTypeId, priorYearMetric.Value, priorYearMetric.ValueTypeName, priorYearMetric.TrendDirection);
            return new AdditionalPriorYearMetricProviderHelperResult
                (currentMetric.StudentUSI, currentMetric.SchoolId, currentMetric.MetricId, currentMetric.MetricStateTypeId, currentMetric.Value, currentMetric.ValueTypeName, currentMetric.TrendDirection);
        }
    }
}