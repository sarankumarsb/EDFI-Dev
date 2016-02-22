// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;

namespace EdFi.Dashboards.Metric.Resources.Factories
{
    public interface IContainerMetricFactory 
    {
        ContainerMetric CreateMetric(MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metadataNode, MetricData metricData, MetricBase parent);
    }

    public class ContainerMetricFactory : MetricFactoryBase<ContainerMetric>, IContainerMetricFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerMetricFactory"/> class.
        /// </summary>
        /// <param name="metricRouteProvider">Provider that will generate application-specific routes for the individual metrics.</param>
        /// <param name="metricActionRouteProvider">Gets or sets the provider to generate metric action links.</param>
        /// <param name="serializer">The data serializer.</param>
        /// <param name="underConstructionProvider"></param>
        public ContainerMetricFactory(IMetricRouteProvider metricRouteProvider, IMetricActionRouteProvider metricActionRouteProvider,
            ISerializer serializer, IUnderConstructionProvider underConstructionProvider) 
            : base(metricRouteProvider, metricActionRouteProvider, serializer, underConstructionProvider) {}

        public override ContainerMetric CreateMetric(MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metadataNode, MetricData metricData, MetricBase parent)
        {
            var returnMetric = new ContainerMetric();

            SetMetricBaseMetricMetadataValues(returnMetric, metricInstanceSetRequest, metadataNode, parent);
            SetMetricBaseMetricInstanceValues(returnMetric, metadataNode, metricData);

            return returnMetric;
        }
    }
}