// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resource.Models.Common;

namespace EdFi.Dashboards.Metric.Resources.Providers
{
    /// <summary>
    /// Gets application-specific links for metric instances.
    /// </summary>
    public interface IMetricRouteProvider
    {
        /// <summary>
        /// Gets the direct (self) and web (browser-based) links for the metric specified by the provided metric instance set request and metric metadata node.
        /// </summary>
        /// <param name="metricInstanceSetRequest">The metric instance set request that is in context for the current link rendering.</param>
        /// <param name="metadataNode">The metric metadata that is associated with the </param>
        /// <returns>A collection of links associated with the metric.</returns>
        /// <remarks>The main metric link should have its <see cref="Link.Rel"/> set to <see cref="LinkRel.AsResource"/>, while a second link should be returned
        /// for browser display using <see cref="LinkRel.Web"/>.</remarks>
        IEnumerable<Link> GetRoute(MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metadataNode);
    }
}
