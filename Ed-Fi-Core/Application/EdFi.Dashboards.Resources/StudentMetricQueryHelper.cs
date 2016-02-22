using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Helpers;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.StudentSchool;

namespace EdFi.Dashboards.Resources
{
    public static class StudentMetricQueryHelper
    {
        public static MetadataColumn GetSortColumn(this List<MetadataColumnGroup> metadataColumnGroups,
                                                   int? sortColumnIndex)
        {
            if (!sortColumnIndex.HasValue)
                return null;
            var columns = new List<MetadataColumn>();
            foreach (var group in metadataColumnGroups)
            {
                columns.AddRange(group.Columns);
                columns.Add(null); // spacer column in EdFiGrid between groups
            }
            return columns.Skip(sortColumnIndex.Value - 1).FirstOrDefault();
        }

        public static IEnumerable<int> GetMetricVariantIds(this IEnumerable<MetadataColumnGroup> metadataColumnGroups)
        {
            return
                metadataColumnGroups.Where(group => @group.GroupType != GroupType.EntityInformation)
                                    .SelectMany(@group => @group.Columns.Select(column => column.MetricVariantId));
        }

        public static StudentWithMetrics.Metric ToListCellTypedMetric(this StudentMetric metricData,
                                                                      MetadataColumn cell,
                                                                      IStudentListUtilitiesProvider utilitiesProvider,
                                                                      ITrendRenderingDispositionProvider trendRenderingDispositionProvider)
        {
            switch (cell.MetricListCellType)
            {
                case MetricListCellType.TrendMetric:
                    return utilitiesProvider.PrepareTrendMetric(metricData.StudentUSI, metricData.SchoolId,
                                                                cell.UniqueIdentifier, metricData.MetricVariantId,
                                                                metricData.Value, metricData.MetricStateTypeId,
                                                                metricData.ValueTypeName, metricData.TrendDirection,
                                                                trendRenderingDispositionProvider);
                case MetricListCellType.AssessmentMetric:
                    return utilitiesProvider.PrepareIndicatorMetric(metricData.StudentUSI, metricData.SchoolId,
                                                                    cell.UniqueIdentifier, metricData.MetricVariantId,
                                                                    metricData.Value, metricData.MetricStateTypeId,
                                                                    metricData.ValueTypeName);
            }

            return utilitiesProvider.PrepareMetric(metricData.StudentUSI, metricData.SchoolId, cell.UniqueIdentifier,
                                                   metricData.MetricVariantId, metricData.Value,
                                                   metricData.MetricStateTypeId, metricData.ValueTypeName);
        }
    }
}