using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Application.Resources.LocalEducationAgency;

namespace EdFi.Dashboards.Presentation.Core.Areas.Admin.Models
{
    public class MetricThresholdMetricModel
    {
        public MetricThresholdGetResponse MetricThresholdGetResponse { get; set; }

        public decimal DefaultValue { get; set; }
        public string MetricTitle { get; set; }
        public string Description { get; set; }
        public decimal Multiplier { get; set; }
    }
}
