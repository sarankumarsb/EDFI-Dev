// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;

namespace EdFi.Dashboards.Metric.Resources.Factories
{
    public interface IAggregateMetricFactory 
    {
        AggregateMetric CreateMetric(MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metadataNode, MetricData metricData, MetricBase parent);
    }

    public class AggregateMetricFactory : MetricFactoryBase<AggregateMetric>, IAggregateMetricFactory
    {
        private readonly IMetricFlagProvider metricFlagProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateMetricFactory"/> class.
        /// </summary>
        /// <param name="metricFlagProvider">Providers that determines if a metric should be flagged for attention or not.</param>
        /// <param name="metricRouteProvider">Provider that will generate application-specific routes for the individual metrics.</param>
        /// <param name="metricActionRouteProvider">Gets or sets the provider to generate metric action links.</param>
        /// <param name="serializer">The data serializer.</param>
        /// <param name="underConstructionProvider"></param>
        public AggregateMetricFactory(IMetricFlagProvider metricFlagProvider, IMetricRouteProvider metricRouteProvider, IMetricActionRouteProvider metricActionRouteProvider,
            ISerializer serializer, IUnderConstructionProvider underConstructionProvider) 
            : base(metricRouteProvider, metricActionRouteProvider, serializer, underConstructionProvider)
        {
            this.metricFlagProvider = metricFlagProvider;
        }

        public override AggregateMetric CreateMetric(MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metadataNode, MetricData metricData, MetricBase parent)
        {
            var returnMetric = new AggregateMetric();

            SetMetricBaseMetricMetadataValues(returnMetric, metricInstanceSetRequest, metadataNode, parent);

            returnMetric.IsFlagged = metricFlagProvider.GetMetricFlag(returnMetric, metadataNode, metricData);

            return returnMetric;
        }
    }
}