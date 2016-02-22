// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

namespace EdFi.Dashboards.Metric.Resources.Providers
{
    public class UnderConstructionProvider : IUnderConstructionProvider
    {
        public bool IsMetricUnderConstruction(int metricId)
        {
            return false;
        }
    }
}