// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Metric.Requests;

namespace EdFi.Dashboards.Resources.Navigation.MetricRouteProviders
{
    /// <summary>
    /// Terminates a <see cref="IMetricRouteProvider"/> chain of responsibility.
    /// </summary>
    public class NullMetricRouteProvider : IMetricRouteProvider
    {
        /// <summary>
        /// Throws an exception describing the unhandled request for a metric route.
        /// </summary>
        /// <param name="metricInstanceSetRequest">The metric instance set request providing the domain-specific context for the metric.</param>
        /// <param name="metadataNode">The metadata for the metric.</param>
        /// <returns>The URL for the application-specific location for the metric.</returns>
        public IEnumerable<Link> GetRoute(MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metadataNode)
        {
            throw new NotSupportedException(
                string.Format("Metric route generation for request type '{0}' was unhandled.", metricInstanceSetRequest.GetType().Name));
        }
    }
}
