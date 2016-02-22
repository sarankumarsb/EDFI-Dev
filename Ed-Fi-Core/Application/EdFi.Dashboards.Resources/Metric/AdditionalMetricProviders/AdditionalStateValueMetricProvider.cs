using System;
using System.Collections.Generic;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.StudentSchool;

namespace EdFi.Dashboards.Resources.Metric.AdditionalMetricProviders
{
    public class AdditionalStateValueMetricProvider : ChainOfResponsibilityBase<IAdditionalMetricProvider, AdditionalMetricRequest, StudentWithMetrics.Metric>, IAdditionalMetricProvider
    {
        private readonly IStudentListUtilitiesProvider _studentListUtilitiesProvider;

        public AdditionalStateValueMetricProvider(IAdditionalMetricProvider next, IStudentListUtilitiesProvider studentListUtilitiesProvider) : base(next)
        {
            _studentListUtilitiesProvider = studentListUtilitiesProvider;
        }

        public StudentWithMetrics.Metric GetAdditionalMetric(AdditionalMetricRequest request)
        {
            return base.ProcessRequest(request);
        }

        protected override bool CanHandleRequest(AdditionalMetricRequest request)
        {
            return request.ColumnMetadata.MetricListCellType == MetricListCellType.StateValueMetric;
        }

        protected override StudentWithMetrics.Metric HandleRequest(AdditionalMetricRequest request)
        {
            return _studentListUtilitiesProvider.PrepareStateValueMetric(request.Metric.StudentUSI, request.ColumnMetadata.UniqueIdentifier,
                                                                            request.Metric.MetricVariantId,
                                                                            request.Metric.MetricStateTypeId);
        }
    }
}