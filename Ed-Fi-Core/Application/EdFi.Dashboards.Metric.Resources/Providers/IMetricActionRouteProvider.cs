using System.Collections.Generic;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resource.Models.Common;

namespace EdFi.Dashboards.Metric.Resources.Providers
{
    /// <summary>
    /// Gets application-specific links for metric instances.
    /// </summary>
    public interface IMetricActionRouteProvider
    {
        //TODO: we need to use metadata from the action url in the database to create these routes
        string GetRoute(MetricInstanceSetRequestBase metricInstanceSetRequest, MetricAction action);
    }
}
