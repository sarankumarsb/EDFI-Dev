// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;

namespace EdFi.Dashboards.Metric.Resources
{
    public class MetricNodeNotFoundException : Exception
    {
        private int metricId;
        public MetricNodeNotFoundException(int metricId, string message)
            : base(message)
        {
            this.metricId = metricId;

        }

        public override string Message
        {
            get
            {
                return string.Format("MetricId '{0}' Not Found: ", metricId) + base.Message;
            }
        }
    }
}
