// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;

namespace EdFi.Dashboards.Resources.Metric.AdditionalMetricProviders
{
    public class AdditionalMetricRequest
    {
        public MetadataColumn ColumnMetadata { get; set; }
        public StudentMetric Metric { get; set; }

        public static AdditionalMetricRequest Create(MetadataColumn metadataItem, dynamic studentDataRow)
        {
            return new AdditionalMetricRequest
            {
                ColumnMetadata = metadataItem,
                Metric = studentDataRow
            };
        }
    }

    public interface IAdditionalMetricProvider
    {
        StudentWithMetrics.Metric GetAdditionalMetric(AdditionalMetricRequest request);
    }
}