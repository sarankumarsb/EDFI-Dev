// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Metric.Rendering
{
    public interface IMetricRenderingEngine
    {
        void RenderMetrics(MetricBase metricBase, RenderingMode renderingMode, IMetricRenderer renderer, IDictionary<string, object> viewData);
    }

    public class MetricRenderingEngine : IMetricRenderingEngine
    {
        private readonly IMetricTemplateBinder metricTemplateBinder;
        private readonly IMetricRenderingContextProvider metricRenderingContextProvider;

        public MetricRenderingEngine(IMetricTemplateBinder metricTemplateBinder, IMetricRenderingContextProvider metricRenderingContextProvider)
        {
            this.metricTemplateBinder = metricTemplateBinder;
            this.metricRenderingContextProvider = metricRenderingContextProvider;
        }

        public void RenderMetrics(MetricBase metricBase, RenderingMode renderingMode, IMetricRenderer renderer, IDictionary<string, object> viewData)
        {
            RenderMetrics(metricBase, renderingMode, renderer, 0, viewData);
        }

        private void RenderMetrics(MetricBase metricBase, RenderingMode renderingMode, IMetricRenderer renderer, int depth, IDictionary<string, object> viewData)
        {
            try
            {
                var renderingContextValues = GetMetricRenderingContextValues(metricBase, depth, renderingMode, true);
                renderer.Render(metricTemplateBinder.GetTemplateName(renderingContextValues), metricBase, depth, viewData);
                var containerMetric = metricBase as ContainerMetric;
                if (containerMetric != null)
                {
                    foreach (var child in containerMetric.Children)
                    {
                        metricRenderingContextProvider.ProvideContext(child, viewData);

                        RenderMetrics(child, renderingMode, renderer, depth + 1, viewData);
                    }
                }
                renderingContextValues = GetMetricRenderingContextValues(metricBase, depth, renderingMode, false);
                renderer.Render(metricTemplateBinder.GetTemplateName(renderingContextValues), metricBase, depth, viewData);
            }
            catch (Exception ex)
            {
				throw new InvalidOperationException(string.Format("Failed to render metricId:{0}  metricVariantId:{1}  name:{2}", metricBase.MetricId, metricBase.MetricVariantId, metricBase.Name), ex);
            }
        }

        private static Dictionary<string, string> GetMetricRenderingContextValues(MetricBase metricBase, int depth, RenderingMode renderingMode, bool open)
        {
            var contextValues = new Dictionary<string, string>
                                   {
                                       { "MetricInstanceSetType", string.IsNullOrEmpty(metricBase.DomainEntityType) ? "" : metricBase.DomainEntityType.Replace(" ","") },
                                       { "MetricType", metricBase.MetricType.ToString() },
                                       { "Depth", "Level" + depth.ToString(CultureInfo.InvariantCulture) },
                                       { "Enabled", metricBase.Enabled.ToString(CultureInfo.InvariantCulture).ToLower() },
                                       { "GrandParentMetricId", metricBase.Parent==null || metricBase.Parent.Parent==null ? string.Empty : metricBase.Parent.Parent.MetricId.ToString(CultureInfo.InvariantCulture) },
                                       { "ParentMetricId", metricBase.Parent==null ? string.Empty : metricBase.Parent.MetricId.ToString(CultureInfo.InvariantCulture) },
                                       { "MetricId", metricBase.MetricId.ToString(CultureInfo.InvariantCulture) },
                                       { "ParentMetricVariantId", metricBase.Parent==null ? string.Empty : metricBase.Parent.MetricVariantId.ToString(CultureInfo.InvariantCulture) },
                                       { "MetricVariantId", metricBase.MetricVariantId.ToString(CultureInfo.InvariantCulture) },
                                       { "RenderingMode", renderingMode.ToString() },
                                       { "NullValue", AllGranularDescendantsHaveNullValues(metricBase)},
                                       { "MetricVariantType", metricBase.MetricVariantType.ToString() },
                                       { "Open", open.ToString(CultureInfo.InvariantCulture).ToLower() },
                                       // Note: We may want to introduce an extensibility point here to populate domain-specific values into the rendering context to allow for template overriding based on domain values
                                       //{ "LocalEducationAgencyId", "" },
                                       //{ "SchoolId","" },
                                       //{ "StudentUSI", "" },
                                   };

            return contextValues;
        }

        private static string AllGranularDescendantsHaveNullValues(MetricBase metricBase)
        {
            if (metricBase.MetricType == MetricType.GranularMetric)
            {
                if (((IGranularMetric) metricBase).Value == null)
                    return "true";
                else
                    return "false";
            }

            if (metricBase.MetricType == MetricType.ContainerMetric ||
                     metricBase.MetricType == MetricType.AggregateMetric)
            {
                bool anyGranularsWithValues = 
                   (from m in ((ContainerMetric) metricBase).Descendants
                    where m.MetricType == MetricType.GranularMetric 
                        && ((IGranularMetric) m).Value != null
                    select m)
                    .Any();

                if (!anyGranularsWithValues)
                    return "true";
                else
                    return "false";
            }

            return "false";

            // TODO: Should we throw an exception to call out this condition if it were ever to occur?
            // throw new Exception(string.Format("Unrecognized metric type '{0}'.", metricBase.MetricType));
        }
    }
    public enum RenderingMode
    {
        Overview,
        Metric,
        GoalPlanning
    }
}
