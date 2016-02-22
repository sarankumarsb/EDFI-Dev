// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Helpers;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using MetricComponent = EdFi.Dashboards.Metric.Resources.Models.MetricComponent;
using MetricStateType = EdFi.Dashboards.Metric.Resources.Models.MetricStateType;

namespace EdFi.Dashboards.Metric.Resources.Factories
{
    public interface IGranularMetricFactory 
    {
        MetricBase CreateMetric(MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metadataNode, MetricData metricData, MetricBase parent);
    }

    public class GranularMetricFactory : MetricFactoryBase<MetricBase>, IGranularMetricFactory
    {
        private readonly ITrendRenderingDispositionProvider trendRenderingDispositionProvider;
        private readonly IMetricFlagProvider metricFlagProvider;
        private readonly IMetricStateProvider metricStateProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="GranularMetricFactory"/> class.
        /// </summary>
        /// <param name="trendRenderingDispositionProvider">Provider to get the correct RenderingDisposition on the trend of the metric</param>
        /// <param name="metricFlagProvider">Provider to get the correct IsFlagged value of the metric</param>
        /// <param name="metricStateProvider">Service that will return the State for the metric if one is not provided</param>
        /// <param name="metricRouteProvider">Provider that will generate application-specific routes for the individual metrics.</param>
        /// <param name="metricActionRouteProvider">Gets or sets the provider to generate metric action links.</param>
        /// <param name="serializer">The data serializer.</param>
        /// <param name="underConstructionProvider"></param>
        public GranularMetricFactory(ITrendRenderingDispositionProvider trendRenderingDispositionProvider,
                                     IMetricFlagProvider metricFlagProvider, 
                                     IMetricStateProvider metricStateProvider,
                                     IMetricRouteProvider metricRouteProvider, 
                                     IMetricActionRouteProvider metricActionRouteProvider,
                                     ISerializer serializer,
                                     IUnderConstructionProvider underConstructionProvider)
            : base(metricRouteProvider, metricActionRouteProvider, serializer, underConstructionProvider)
        {
            this.trendRenderingDispositionProvider = trendRenderingDispositionProvider;
            this.metricFlagProvider = metricFlagProvider;
            this.metricStateProvider = metricStateProvider;
        }

        private Dictionary<string, Type> granularMetricTypesByTypeName = new Dictionary<string, Type>();
        private Dictionary<string, Type> valueTypesByTypeName = new Dictionary<string, Type>();

        public override MetricBase CreateMetric(MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metadataNode, MetricData metricData, MetricBase parent)
        {
            //get metric instance
            var metricInstance = metricData.MetricInstancesByMetricId.GetValueOrDefault(metadataNode.MetricId);
            try
            {

                Type granularMetricType;
                Type valueType;


                if (metricInstance == null || metricInstance.ValueTypeName == null)
                {
                    granularMetricType = typeof(GranularMetric<string>);
                    valueType = Type.GetType("System.String");
                }
                else
                {
                    if (!valueTypesByTypeName.TryGetValue(metricInstance.ValueTypeName, out valueType))
                    {
                        valueType = Type.GetType(metricInstance.ValueTypeName);
                        valueTypesByTypeName[metricInstance.ValueTypeName] = valueType;
                    }

                    if (!granularMetricTypesByTypeName.TryGetValue(metricInstance.ValueTypeName, out granularMetricType))
                    {
                        granularMetricType = typeof(GranularMetric<>).MakeGenericType(new[] { valueType });
                        granularMetricTypesByTypeName[metricInstance.ValueTypeName] = granularMetricType;
                    }
                }

                IGranularMetric granularMetric = (IGranularMetric) Activator.CreateInstance((granularMetricType));

                //get metricBase values
                var metricBase = granularMetric as MetricBase;
                SetMetricBaseMetricMetadataValues(metricBase, metricInstanceSetRequest, metadataNode, parent);
                SetMetricBaseMetricInstanceValues(metricBase, metadataNode, metricData);

                //overlay granular metric specific values
                OverlayGranularMetricInstanceData(granularMetric, valueType, metadataNode, metricData);

                //overlay granular metric values from providers
                OverlayGranularMetricProviderValues(granularMetric, metadataNode);

                granularMetric.IsFlagged = metricFlagProvider.GetMetricFlag(metricBase, metadataNode, metricData);

                return metricBase;
            }
            catch (ArgumentNullException ane)
            {
                var s = string.Format("metricid: '{0}', metricvariantid: '{2}', valuetypename: '{1}'", metadataNode.MetricId, metricInstance.ValueTypeName, metadataNode.MetricVariantId);
                throw new ArgumentNullException(s, ane);
            }
        }

        private void OverlayGranularMetricProviderValues(dynamic granularMetric, MetricMetadataNode metadataNode)
        {
            granularMetric.Trend.RenderingDisposition = trendRenderingDispositionProvider.GetTrendRenderingDisposition(granularMetric.Trend.Direction, granularMetric.Trend.Interpretation);
        }

        private void OverlayGranularMetricInstanceData(dynamic granularMetric, Type granularMetricValueType, MetricMetadataNode metadataNode, MetricData metricData)
        {
            var metricDataInstance = metricData.MetricInstancesByMetricId.GetValueOrDefault(metadataNode.MetricId);
            var metricDataInstanceValue = metricDataInstance == null ? null : metricDataInstance.Value;

            if (metricDataInstanceValue != null)
            {
                if (granularMetricValueType == typeof(string))
                {
                    dynamic value = Activator.CreateInstance(granularMetricValueType, metricDataInstanceValue.ToCharArray());
                    granularMetric.Value = value;
                }
                else
                {
                    dynamic value = Activator.CreateInstance(granularMetricValueType);
                    value = Convert.ChangeType(metricDataInstanceValue, granularMetricValueType);
                    granularMetric.Value = value;
                }
                
                if (!string.IsNullOrEmpty(metadataNode.Format) && granularMetric.Value != null)
                    granularMetric.DisplayValue = String.Format(metadataNode.Format, granularMetric.Value);
                else
                    granularMetric.DisplayValue = Convert.ToString(granularMetric.Value);
            }

            var metricInstance = metricData.MetricInstancesByMetricId.GetValueOrDefault(metadataNode.MetricId);

            if (metricInstance != null)
            {
                granularMetric.IsFlagged = metricInstance.Flag.GetValueOrDefault();
                granularMetric.Trend = new Trend
                                            {
                                                Direction = TrendHelper.TrendFromDirection(metricInstance.TrendDirection),
                                                Interpretation = (TrendInterpretation)metadataNode.TrendInterpretation.GetValueOrDefault(),
                                                Evaluation = TrendHelper.GetTrendEvaluation(TrendHelper.TrendFromDirection(metricInstance.TrendDirection), 
                                                                                (TrendInterpretation)metadataNode.TrendInterpretation.GetValueOrDefault()),
                                            };
            }
            else
            {
                granularMetric.Trend = new Trend
                {
                    Direction = TrendDirection.None,
                    Interpretation = (TrendInterpretation)metadataNode.TrendInterpretation.GetValueOrDefault(),
                    Evaluation = TrendHelper.GetTrendEvaluation(TrendDirection.None, (TrendInterpretation)metadataNode.TrendInterpretation.GetValueOrDefault()),
                };
            }

            if (metricData.MetricComponents != null)
                granularMetric.Components = (from m in metricData.MetricComponentsByMetricId.ValuesByKey(metadataNode.MetricId)
                                             select new MetricComponent
                                             {
                                                 Name = m.Name,
                                                 Value = m.Value,
                                                 Format = m.Format,
                                                 MetricStateType = (MetricStateType) m.MetricStateTypeId.GetValueOrDefault(),
                                                 TrendDirection = TrendHelper.TrendFromDirection(m.TrendDirection),
                                                 ValueTypeName = m.ValueTypeName
                                             }).ToList();

            foreach (MetricInstanceExtendedProperty property in metricData.MetricInstanceExtendedPropertiesByMetricId.ValuesByKey(metadataNode.MetricId))
            {
                try
                {
                    granularMetric.Values.Add(property.Name, Convert.ChangeType(property.Value, Type.GetType(property.ValueTypeName)));
                }
                catch (Exception ex)
                {
					throw new InvalidOperationException(string.Format("Failed to convert Metric Instance Extended Properties value: {0} Value Type Name: {1} for Metric Instance Set Key: {2} Property Name: {3}", property.Value, property.ValueTypeName, metricInstance.MetricInstanceSetKey, property.Name), ex);
                }
            }

            granularMetric.State = metricStateProvider.GetState(metadataNode, metricInstance);
        }
    }
}
