// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.StudentSchool;

namespace EdFi.Dashboards.Warehouse.Resources.Metric.AdditionalPriorYearMetricProviders
{
    public class AdditionalPriorYearMetricProvider : ChainOfResponsibilityBase<IAdditionalPriorYearMetricProvider, AdditionalPriorYearMetricRequest, StudentWithMetrics.Metric>, IAdditionalPriorYearMetricProvider
    {
        private readonly IStudentListUtilitiesProvider _studentListUtilitiesProvider;

        public AdditionalPriorYearMetricProvider(IAdditionalPriorYearMetricProvider next, IStudentListUtilitiesProvider studentListUtilitiesProvider)
            : base(next)
        {
            _studentListUtilitiesProvider = studentListUtilitiesProvider;
        }

        public StudentWithMetrics.Metric GetAdditionalPriorYearMetric(AdditionalPriorYearMetricRequest request)
        {
            return base.ProcessRequest(request);
        }

        protected override bool CanHandleRequest(AdditionalPriorYearMetricRequest request)
        {
            return request.MetadataItem.MetricListCellType == MetricListCellType.Metric;
        }

        protected override StudentWithMetrics.Metric HandleRequest(AdditionalPriorYearMetricRequest request)
        {
            var metadataItem = request.MetadataItem;
            var priorYearStudentList = request.PriorYearStudentList;
            var studentListEntity = request.StudentDataRow;
            StudentWithMetrics.Metric result = null;

            var metricToUse = AdditionalPriorYearMetricProviderHelper.GetMetricToUse(priorYearStudentList, studentListEntity, metadataItem.ColumnPrefix);
            var metricVariantId = metadataItem.MetricVariantId;

            if (metricToUse != null)
                result = _studentListUtilitiesProvider.PrepareMetric(metricToUse.StudentUSI, studentListEntity.SchoolId,
                    metadataItem.UniqueIdentifier, metricVariantId, metricToUse.Value, metricToUse.MetricStateTypeId,
                    metricToUse.ValueTypeName);

            return result;
        }
    }
}