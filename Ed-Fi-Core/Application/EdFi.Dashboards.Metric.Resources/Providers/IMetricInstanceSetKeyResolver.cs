// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using EdFi.Dashboards.Metric.Resources.Services;

namespace EdFi.Dashboards.Metric.Resources.Providers
{
    public interface IMetricInstanceSetKeyResolver<in TMetricRequest>
        where TMetricRequest : MetricInstanceSetRequestBase
    {
        Guid GetMetricInstanceSetKey(TMetricRequest metricInstanceSetRequestBase);
    }
}