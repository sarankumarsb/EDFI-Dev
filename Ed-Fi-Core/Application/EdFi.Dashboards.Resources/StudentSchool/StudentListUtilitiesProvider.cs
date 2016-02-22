// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Models.Student;

namespace EdFi.Dashboards.Resources.StudentSchool
{
    public interface IStudentListUtilitiesProvider
    {
        StudentWithMetrics.TrendMetric PrepareTrendMetric(long studentUSI, int schoolId, int uniqueIdentifier, int? metricVariantId, string metricValue, int? stateTypeId, string valueType, int? trendDirection, ITrendRenderingDispositionProvider trendRenderingDispositionProvider);
        StudentWithMetrics.TrendMetric PrepareTrendMetric(long studentUSI, int uniqueIdentifier, int? metricVariantId, string metricValue, int? stateTypeId, string valueType, string displayValueFormat, int? trendInterpretation, int? trendDirection, ITrendRenderingDispositionProvider trendRenderingDispositionProvider);
        StudentWithMetrics.Metric PrepareMetric(long studentUSI, int uniqueIdentifier, int? metricVariantId, string metricValue, int? stateTypeId, string valueType, string displayValueFormat);
        StudentWithMetrics.Metric PrepareMetric(long studentUSI, int schoolId, int uniqueIdentifier, int? metricVariantId, string metricValue, int? stateTypeId, string valueType);
        StudentWithMetrics.Metric PrepareStateValueMetric(long studentUSI, int uniqueIdentifier, int? metricVariantId, int? stateTypeId);
        StudentWithMetrics.IndicatorMetric PrepareIndicatorMetric(long studentUSI, int uniqueIdentifier, int? metricVariantId, string metricValue, int? stateTypeId, string valueType, string displayValueFormat);
        StudentWithMetrics.IndicatorMetric PrepareIndicatorMetric(long studentUSI, int schoolId, int uniqueIdentifier, int? metricVariantId, string metricValue, int? stateTypeId, string valueType);
    }

    public class StudentListUtilitiesProvider : IStudentListUtilitiesProvider
    {
        private readonly IMetricStateProvider metricStateProvider;
        private readonly IMetricNodeResolver metricNodeResolver;

        public StudentListUtilitiesProvider(IMetricStateProvider metricStateProvider, IMetricNodeResolver metricNodeResolver)
        {
            this.metricStateProvider = metricStateProvider;
            this.metricNodeResolver = metricNodeResolver;
        }

        public StudentWithMetrics.TrendMetric PrepareTrendMetric(long studentUSI, int schoolId, int uniqueIdentifier, int? metricVariantId, string metricValue, int? stateTypeId, string valueType, int? trendDirection, ITrendRenderingDispositionProvider trendRenderingDispositionProvider)
        {
            if (metricVariantId == null)
                return new StudentWithMetrics.TrendMetric(studentUSI) { UniqueIdentifier = uniqueIdentifier, State = MetricStateType.None, Trend = TrendEvaluation.None };

            var metricMetadataNode = metricNodeResolver.GetMetricNodeForStudentFromMetricVariantId(schoolId, metricVariantId.Value);
            if (metricMetadataNode == null)
                return PrepareTrendMetric(studentUSI, uniqueIdentifier, metricVariantId, metricValue, stateTypeId, valueType, null, null, trendDirection, trendRenderingDispositionProvider);

            return PrepareTrendMetric(studentUSI, uniqueIdentifier, metricVariantId, metricValue, stateTypeId, valueType, metricMetadataNode.Format, metricMetadataNode.TrendInterpretation, trendDirection, trendRenderingDispositionProvider);
        }

        public StudentWithMetrics.TrendMetric PrepareTrendMetric(long studentUSI, int uniqueIdentifier, int? metricVariantId, string metricValue, int? stateTypeId, string valueType, string displayValueFormat, int? trendInterpretation, int? trendDirection, ITrendRenderingDispositionProvider trendRenderingDispositionProvider)
        {
            if (metricVariantId == null)
                return new StudentWithMetrics.TrendMetric(studentUSI) { UniqueIdentifier = uniqueIdentifier, State = MetricStateType.None, Trend = TrendEvaluation.None };

            var additionalMetric = new StudentWithMetrics.TrendMetric(studentUSI);
            PrepareMetric(additionalMetric, uniqueIdentifier, metricVariantId, metricValue, stateTypeId, valueType, displayValueFormat);
            if (!trendDirection.HasValue || !trendInterpretation.HasValue)
                additionalMetric.Trend = TrendEvaluation.None;
            else
                additionalMetric.Trend = trendRenderingDispositionProvider.GetTrendRenderingDisposition((TrendDirection)trendDirection, (TrendInterpretation)trendInterpretation);

            return additionalMetric;
        }

        public StudentWithMetrics.IndicatorMetric PrepareIndicatorMetric(long studentUSI, int schoolId, int uniqueIdentifier, int? metricVariantId, string metricValue, int? stateTypeId, string valueType)
        {
            if (metricVariantId == null)
                return new StudentWithMetrics.IndicatorMetric(studentUSI) { UniqueIdentifier = uniqueIdentifier, State = MetricStateType.None, MetricIndicator = (int)MetricIndicatorType.None };

            var metricMetadataNode = metricNodeResolver.GetMetricNodeForStudentFromMetricVariantId(schoolId, metricVariantId.Value);
            if (metricMetadataNode == null)
                return PrepareIndicatorMetric(studentUSI, uniqueIdentifier, metricVariantId, metricValue, stateTypeId, valueType, null);
            return PrepareIndicatorMetric(studentUSI, uniqueIdentifier, metricVariantId, metricValue, stateTypeId, valueType, metricMetadataNode.Format);
        }

        public StudentWithMetrics.IndicatorMetric PrepareIndicatorMetric(long studentUSI, int uniqueIdentifier, int? metricVariantId, string metricValue, int? stateTypeId, string valueType, string displayValueFormat)
        {
            if (metricVariantId == null)
                return new StudentWithMetrics.IndicatorMetric(studentUSI) { UniqueIdentifier = uniqueIdentifier, State = MetricStateType.None, MetricIndicator = (int)MetricIndicatorType.None };

            var additionalMetric = new StudentWithMetrics.IndicatorMetric(studentUSI);
            PrepareMetric(additionalMetric, uniqueIdentifier, metricVariantId, metricValue, stateTypeId, valueType, displayValueFormat);
            additionalMetric.MetricIndicator = (int)MetricIndicatorType.None; 

            return additionalMetric;
        }

        public StudentWithMetrics.Metric PrepareMetric(long studentUSI, int schoolId, int uniqueIdentifier, int? metricVariantId, string metricValue, int? stateTypeId, string valueType)
        {
            if (metricVariantId == null)
                return new StudentWithMetrics.Metric(studentUSI) { UniqueIdentifier = uniqueIdentifier, State = MetricStateType.None };

            var additionalMetric = new StudentWithMetrics.Metric(studentUSI);
            var metricMetadataNode = metricNodeResolver.GetMetricNodeForStudentFromMetricVariantId(schoolId, metricVariantId.Value);
            if (metricMetadataNode == null)
                PrepareMetric(additionalMetric, uniqueIdentifier, metricVariantId, metricValue, stateTypeId, valueType, null);
            else
                PrepareMetric(additionalMetric, uniqueIdentifier, metricVariantId, metricValue, stateTypeId, valueType, metricMetadataNode.Format);

            return additionalMetric;
        }

        public StudentWithMetrics.Metric PrepareMetric(long studentUSI, int uniqueIdentifier, int? metricVariantId, string metricValue, int? stateTypeId, string valueType, string displayValueFormat)
        {
            if (metricVariantId == null)
                return new StudentWithMetrics.Metric(studentUSI) { UniqueIdentifier = uniqueIdentifier, State = MetricStateType.None };

            var additionalMetric = new StudentWithMetrics.Metric(studentUSI);
            PrepareMetric(additionalMetric, uniqueIdentifier, metricVariantId, metricValue, stateTypeId, valueType, displayValueFormat);

            return additionalMetric;
        }

        public StudentWithMetrics.Metric PrepareStateValueMetric(long studentUSI, int uniqueIdentifier, int? metricVariantId, int? stateTypeId)
        {
            if (metricVariantId == null || !stateTypeId.HasValue)
                return new StudentWithMetrics.Metric(studentUSI) { UniqueIdentifier = uniqueIdentifier, State = MetricStateType.None };

            var additionalMetric = new StudentWithMetrics.Metric(studentUSI);
            additionalMetric.UniqueIdentifier = uniqueIdentifier;
            additionalMetric.MetricVariantId = metricVariantId.Value;

            var metricId = metricNodeResolver.ResolveMetricId(metricVariantId.Value);
            var state = metricStateProvider.GetState(metricId, stateTypeId.Value);
            additionalMetric.Value = state.StateText;
            additionalMetric.DisplayValue = state.DisplayStateText;
            additionalMetric.State = (MetricStateType)stateTypeId.Value;
            return additionalMetric;
        }

        protected void PrepareMetric(StudentWithMetrics.Metric additionalMetric, int uniqueIdentifier, int? metricVariantId, string metricValue, int? stateTypeId, string valueType, string displayValueFormat)
        {
            if (metricVariantId == null)
                return;

            additionalMetric.UniqueIdentifier = uniqueIdentifier;
            additionalMetric.MetricVariantId = metricVariantId.Value;
            additionalMetric.Value = InstantiateValue.FromValueType(metricValue, valueType);
            if (additionalMetric.Value == null)
                additionalMetric.DisplayValue = String.Empty;
            else if (displayValueFormat != null)
                additionalMetric.DisplayValue = String.Format(displayValueFormat, additionalMetric.Value);
            else
                additionalMetric.DisplayValue = String.Format("{0}", additionalMetric.Value);

            if (stateTypeId.HasValue)
                additionalMetric.State = (MetricStateType)stateTypeId.Value;
            else
            {
                var metricId = metricNodeResolver.ResolveMetricId(metricVariantId.Value);
                additionalMetric.State = metricStateProvider.GetState(metricId, metricValue, valueType).StateType;
            }
        }
    }
}
