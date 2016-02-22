// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Metric.Resources.InitializationActivity;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services;

namespace EdFi.Dashboards.Metric.Resources.Factories
{
    /// <summary>
    /// Builds a complete hierarchical model representation of the supplied metric data and metadata.
    /// </summary>
    public interface IMetricInstanceTreeFactory
    {
        /// <summary>
        /// Builds a complete hierarchical model representation of the supplied metric data and metadata.
        /// </summary>
        /// <param name="metricInstanceSetRequest">The metric instance set request that was issued, resulting in the construction of this hierarchical model.</param>
        /// <param name="metricMetadataNode">The metadata for the metric instance set specified by the <paramref name="metricInstanceSetRequest"/> parameter.</param>
        /// <param name="metricData">The raw metric data for the metric instance set specified by the <paramref name="metricInstanceSetRequest"/> parameter.</param>
        /// <returns>The newly constructed metric instance model hierarchy.</returns>
        MetricBase CreateTree(MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metricMetadataNode, MetricDataContainer metricDataContainer);
    }

    public class MetricInstanceTreeFactory : IMetricInstanceTreeFactory
    {
        private readonly IContainerMetricFactory containerMetricFactory;
        private readonly IAggregateMetricFactory aggregateMetricFactory;
        private readonly IGranularMetricFactory granularMetricFactory;
        private readonly IMetricInitializationActivity[] metricInitializationActivities;
        private readonly IMetricInitializationActivityDataProvider[] metricInitializationActivityDataProviders;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricInstanceTreeFactory"/> class.
        /// </summary>
        /// <param name="containerMetricFactory">The factory responsible for creating container metrics.</param>
        /// <param name="aggregateMetricFactory">The factory responsible for creating aggregate metrics. </param>
        /// <param name="granularMetricFactory">The factory responsible for creating granular metrics.</param>
        /// <param name="metricInitializationActivities">Array of initialization activities to be executed against each metric after it is created and initialized by the factory.</param>
        /// <param name="metricInitializationActivityDataProviders">Array of data providers to initialize the data necessary for initialization activities.</param>
        public MetricInstanceTreeFactory(IContainerMetricFactory containerMetricFactory,
                                         IAggregateMetricFactory aggregateMetricFactory,
                                         IGranularMetricFactory granularMetricFactory,
                                         IMetricInitializationActivity[] metricInitializationActivities,
                                         IMetricInitializationActivityDataProvider[] metricInitializationActivityDataProviders)
        {
            this.containerMetricFactory = containerMetricFactory;
            this.aggregateMetricFactory = aggregateMetricFactory;
            this.granularMetricFactory = granularMetricFactory;
            this.metricInitializationActivities = metricInitializationActivities;
            this.metricInitializationActivityDataProviders = metricInitializationActivityDataProviders;
        }

        /// <summary>
        /// Returns the created Metric Tree with all data values and metadata included
        /// </summary>
        /// <param name="metricInstanceSetRequest">The request originally passed to the metric service, containing the domain-specific context.</param>
        /// <param name="metricMetadataNode">Should already Filtered by MetricNodeId</param>
        /// <param name="metricDataContainer">Should already be filtered by the domain entity key</param>
        /// <returns>An instance of a class derived from <see cref="MetricBase"/>, and all its descendents.</returns>
        public MetricBase CreateTree(MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metricMetadataNode, MetricDataContainer metricDataContainer)
        {
            // Get the custom metric initialization activity data
            var initializationActivityData = GetMetricInitializationActivityData(metricInstanceSetRequest);

            return CreateMetric(metricInstanceSetRequest, metricMetadataNode, metricDataContainer, null, initializationActivityData);
        }

        private Dictionary<string, object> GetMetricInitializationActivityData(MetricInstanceSetRequestBase metricInstanceSetRequest)
        {
            var data = new Dictionary<string, object>();

            foreach (var activityDataProvider in metricInitializationActivityDataProviders)
            {
                var activityData = activityDataProvider.GetInitializationActivityData(metricInstanceSetRequest);

                // Ignore activity data if null is returned.
                if (activityData == null)
                    continue;

                // Make sure we're not trying to overwrite already retrieved data
                if (data.ContainsKey(activityData.Key))
					throw new InvalidOperationException(string.Format("Initialization activity data already exists for '{0}'.", activityData.Key));

                // Add the entry to the data
                data.Add(activityData.Key, activityData.Data);
            }

            return data;
        }

        private MetricBase CreateMetric(MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metricMetadataNode, MetricDataContainer metricDataContainer, 
                                        MetricBase parentMetric, Dictionary<string, object> initializationActivityData)
        {
            MetricBase metric;
            var metricData = metricDataContainer.GetMetricData(metricMetadataNode);

            switch (metricMetadataNode.MetricType)
            {
                case MetricType.ContainerMetric:
                    metric = containerMetricFactory.CreateMetric(metricInstanceSetRequest, metricMetadataNode, metricData, parentMetric);
                    break;

                case MetricType.AggregateMetric:
                    metric = aggregateMetricFactory.CreateMetric(metricInstanceSetRequest, metricMetadataNode, metricData, parentMetric);
                    break;

                case MetricType.GranularMetric:
                    metric = granularMetricFactory.CreateMetric(metricInstanceSetRequest, metricMetadataNode, metricData, parentMetric);
                    break;

                default:
                    throw new NotSupportedException(string.Format("Unsupported metric type '{0}'.", metricMetadataNode.MetricType));
            }

            // Process children
            var metricWithChildren = metric as ContainerMetric;

            // If this is a node type that has children
            if (metricWithChildren != null)
                ProcessForChildren(metricInstanceSetRequest, metricMetadataNode, metricDataContainer, metricWithChildren, initializationActivityData);

            // Execute initialization activities
            InvokeInitializationActivities(metric, metricInstanceSetRequest, metricMetadataNode, metricData, initializationActivityData);

            return metric;
        }

        protected virtual void ProcessForChildren(MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metricMetadataNode, MetricDataContainer metricDataContainer, 
                                                  ContainerMetric parentMetric, Dictionary<string, object> initializationActivityData)
        {
            // Initialize the list if it's not already initialized
            if (parentMetric.Children == null)
                parentMetric.Children = new List<MetricBase>();

            // Iterate through the children in the metadata
            foreach (var node in metricMetadataNode.Children)
            {
                // Call recursively to create the tree
                var newChildMetric = CreateMetric(metricInstanceSetRequest, node, metricDataContainer, parentMetric, initializationActivityData);
                parentMetric.Children.Add(newChildMetric);
            }
        }

        protected virtual void InvokeInitializationActivities(MetricBase metric, MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metadataNode, 
                                                              MetricData metricData, Dictionary<string, object> initializationActivityData)
        {
            foreach (var initializationActivity in metricInitializationActivities)
                initializationActivity.InitializeMetric(metric, metricInstanceSetRequest, metadataNode, metricData, initializationActivityData);
        }
    }
}
