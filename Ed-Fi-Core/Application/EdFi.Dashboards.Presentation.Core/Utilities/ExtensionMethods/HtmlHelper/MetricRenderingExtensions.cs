// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections;
using System.Diagnostics;
using System.Web.Mvc;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Metric.Rendering;
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Presentation.Web.Architecture
{
    public static class MetricRenderingExtensions
    {
        public static void RenderMetrics(this HtmlHelper<object> helper, MetricBase metric, RenderingMode renderingMode)
        {
            var renderer = (AspNetMvcMetricRenderer) IoC.Resolve<IMetricRenderer>();
            renderer.Html = helper;

            var engine = IoC.Resolve<IMetricRenderingEngine>();
            engine.RenderMetrics(metric, renderingMode, renderer, helper.ViewData);
        }
    }
}