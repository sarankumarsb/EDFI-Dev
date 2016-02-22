// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;

namespace EdFi.Dashboards.Metric.Resources.Models
{
    [Serializable]
    public class AggregateMetric : ContainerMetric
    {
        public AggregateMetric()
        {
            MetricType = MetricType.AggregateMetric;
        }

        /// <summary>
        /// The value to know if the metric is flagged or not.
        /// </summary>
        public bool IsFlagged { get; set; }
    }
}
