using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Metric.Rendering
{
    public interface IMetricRenderingContextProvider
    {
        void ProvideContext(MetricBase metric, IDictionary<string, object> context);
    }

    public class MetricRenderingContextProvider : IMetricRenderingContextProvider
    {
        public void ProvideContext(MetricBase metric, IDictionary<string, object> context)
        {
            if(!context.ContainsKey("MetricRenderingContext"))
            {
                context["MetricRenderingContext"] = new Dictionary<string, object>();
            }
        }
    }
}
