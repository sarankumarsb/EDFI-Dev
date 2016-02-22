// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.StudentSchool;

namespace EdFi.Dashboards.Resources.Metric.AdditionalMetricProviders
{
    public class AdditionalMetricProvider : ChainOfResponsibilityBase<IAdditionalMetricProvider, AdditionalMetricRequest, StudentWithMetrics.Metric>, IAdditionalMetricProvider
    {
        private readonly IStudentListUtilitiesProvider _studentListUtilitiesProvider;

        public AdditionalMetricProvider(IAdditionalMetricProvider next, IStudentListUtilitiesProvider studentListUtilitiesProvider) : base(next)
        {
            _studentListUtilitiesProvider = studentListUtilitiesProvider;
        }

        public StudentWithMetrics.Metric GetAdditionalMetric(AdditionalMetricRequest request)
        {
            return base.ProcessRequest(request);
        }

        protected override bool CanHandleRequest(AdditionalMetricRequest request)
        {
            return request.ColumnMetadata.MetricListCellType == MetricListCellType.Metric;
        }

        protected override StudentWithMetrics.Metric HandleRequest(AdditionalMetricRequest request)
        {
            return _studentListUtilitiesProvider.PrepareMetric(request.Metric.StudentUSI, request.ColumnMetadata.UniqueIdentifier,
                                                                request.Metric.MetricVariantId,
                                                                request.Metric.Value,
                                                                request.Metric.MetricStateTypeId,
                                                                request.Metric.ValueTypeName,
                                                                request.Metric.Format);
        }
    }
}