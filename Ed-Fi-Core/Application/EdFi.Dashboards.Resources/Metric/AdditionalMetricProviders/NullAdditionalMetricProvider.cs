// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using EdFi.Dashboards.Resources.Models.Student;

namespace EdFi.Dashboards.Resources.Metric.AdditionalMetricProviders
{
    public class NullAdditionalMetricProvider : IAdditionalMetricProvider
    {
        public StudentWithMetrics.Metric GetAdditionalMetric(AdditionalMetricRequest request)
        {
            throw new ArgumentOutOfRangeException();
        }
    }
}