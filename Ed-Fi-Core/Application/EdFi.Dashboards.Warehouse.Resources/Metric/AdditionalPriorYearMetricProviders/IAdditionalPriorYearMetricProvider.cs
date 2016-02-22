// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Warehouse.Data.Entities;

namespace EdFi.Dashboards.Warehouse.Resources.Metric.AdditionalPriorYearMetricProviders
{
    public class AdditionalPriorYearMetricRequest
    {
        public MetadataColumn MetadataItem { get; set; }
        public StudentSchoolMetricInstance PriorYearStudentList { get; set; }
        public StudentMetric StudentDataRow { get; set; }

        public static AdditionalPriorYearMetricRequest Create(MetadataColumn metadataItem, StudentSchoolMetricInstance priorYearStudentList, StudentMetric studentDataRow)
        {
            return new AdditionalPriorYearMetricRequest
            {
                MetadataItem = metadataItem,
                PriorYearStudentList = priorYearStudentList,
                StudentDataRow = studentDataRow
            };
        }
    }

    public interface IAdditionalPriorYearMetricProvider
    {
        StudentWithMetrics.Metric GetAdditionalPriorYearMetric(AdditionalPriorYearMetricRequest request);
    }
}