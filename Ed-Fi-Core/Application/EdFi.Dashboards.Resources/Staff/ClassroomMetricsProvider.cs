// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Resources.Metric.AdditionalMetricProviders;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;

namespace EdFi.Dashboards.Resources.Staff
{
    public interface IClassroomMetricsProvider
    {
        List<StudentWithMetrics.Metric> GetAdditionalMetrics(IEnumerable<StudentMetric> metric, List<MetadataColumnGroup> listMetadata);
    }

    public class ClassroomMetricsProvider : IClassroomMetricsProvider
    {
        private readonly IAdditionalMetricProvider _additionalMetricProvider;

        public ClassroomMetricsProvider(IAdditionalMetricProvider additionalMetricProvider)
        {
            _additionalMetricProvider = additionalMetricProvider;
        }

        public List<StudentWithMetrics.Metric> GetAdditionalMetrics(IEnumerable<StudentMetric> studentMetrics, List<MetadataColumnGroup> listMetadata)
        {
            return CreateAdditionalMetrics(studentMetrics, listMetadata);
        }

        protected virtual List<StudentWithMetrics.Metric> CreateAdditionalMetrics(IEnumerable<StudentMetric> studentMetrics, List<MetadataColumnGroup> listMetadata)
        {
            var metrics = new List<StudentWithMetrics.Metric>();
            foreach (var metadataColumn in listMetadata.Where(@group => @group.GroupType != GroupType.EntityInformation).SelectMany(@group => @group.Columns))
            {
                var column = metadataColumn;
                var metric = (from studentMetric in studentMetrics
                              where studentMetric != null && studentMetric.MetricVariantId == column.MetricVariantId
                              select studentMetric).FirstOrDefault();
                // TODO: is this okay that it doesn't add anything instead of adding an empty one? [WD]
                if (metric != null)
                {
                    var request = AdditionalMetricRequest.Create(column, metric);
                    metrics.Add(_additionalMetricProvider.GetAdditionalMetric(request));
                }
            }
            return metrics;
        }
    }
}
