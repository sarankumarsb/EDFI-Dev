// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

namespace EdFi.Dashboards.Metric.Resources.Providers
{
    public interface IUnderConstructionProvider
    {
        bool IsMetricUnderConstruction(int metricId);
    }
}