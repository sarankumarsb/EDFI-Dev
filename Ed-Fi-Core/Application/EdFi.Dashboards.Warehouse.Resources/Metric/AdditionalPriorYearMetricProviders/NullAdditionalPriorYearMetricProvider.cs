// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using EdFi.Dashboards.Resources.Models.Student;

namespace EdFi.Dashboards.Warehouse.Resources.Metric.AdditionalPriorYearMetricProviders
{
    public class NullAdditionalPriorYearMetricProvider : IAdditionalPriorYearMetricProvider
    {
        public StudentWithMetrics.Metric GetAdditionalPriorYearMetric(AdditionalPriorYearMetricRequest request)
        {
            throw new ArgumentOutOfRangeException();
        }
    }
}