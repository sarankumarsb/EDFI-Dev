// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using EdFi.Dashboards.Metric.Rendering;

namespace EdFi.Dashboards.Presentation.Web.Architecture
{
    public class AspNetMvcMetricRenderer : IMetricRenderer
    {
        public virtual HtmlHelper<object> Html { get; set; }

        public virtual void Render(string templateName, dynamic metricBase, int depth, IDictionary<string, object> viewData)
        {
            Html.RenderPartial(templateName, (object)metricBase, (ViewDataDictionary)viewData);
        }
    }
}