﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18034
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EdFi.Dashboards.Presentation.Core.Views.MetricTemplates.LocalEducationAgency.Metric
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    using EdFi.Dashboards.Metric.Resources.Models;
    using EdFi.Dashboards.Presentation.Architecture.Mvc.Extensions;
    using EdFi.Dashboards.Presentation.Web.Utilities;
    using EdFi.Dashboards.Resources.Navigation;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "1.5.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/MetricTemplates/LocalEducationAgency/Metric/GranularMetric.Level2.Disable" +
        "d.MetricVariantType_PriorYear.cshtml")]
    public class GranularMetric_Level2_Disabled_MetricVariantType_PriorYear : System.Web.Mvc.WebViewPage<IGranularMetric>
    {
        public GranularMetric_Level2_Disabled_MetricVariantType_PriorYear()
        {
        }
        public override void Execute()
        {

WriteLiteral("<tr id=\"vMetric");


            
            #line 2 "..\..\Views\MetricTemplates\LocalEducationAgency\Metric\GranularMetric.Level2.Disabled.MetricVariantType_PriorYear.cshtml"
           Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\">\r\n    <td headers=\"MetricName");


            
            #line 3 "..\..\Views\MetricTemplates\LocalEducationAgency\Metric\GranularMetric.Level2.Disabled.MetricVariantType_PriorYear.cshtml"
                       Write(this.GetMetricRenderingContextValue("ParentContainerMetricId"));

            
            #line default
            #line hidden
WriteLiteral("\" data-item-type=\"DisplayName\" class=\"SubMetricBG\" title=\"");


            
            #line 3 "..\..\Views\MetricTemplates\LocalEducationAgency\Metric\GranularMetric.Level2.Disabled.MetricVariantType_PriorYear.cshtml"
                                                                                                                                                 Write(Model.ToolTip);

            
            #line default
            #line hidden
WriteLiteral("\">\r\n       <p class=\"arrow deemphasized\">\r\n            <i class=\"icon-right  deem" +
"phasized\"></i>\r\n            <a name=\"m");


            
            #line 6 "..\..\Views\MetricTemplates\LocalEducationAgency\Metric\GranularMetric.Level2.Disabled.MetricVariantType_PriorYear.cshtml"
                  Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\">&nbsp;</a>\r\n            <span class=\"metricId\">");


            
            #line 7 "..\..\Views\MetricTemplates\LocalEducationAgency\Metric\GranularMetric.Level2.Disabled.MetricVariantType_PriorYear.cshtml"
                               Write(Model.MetricId);

            
            #line default
            #line hidden
WriteLiteral("-");


            
            #line 7 "..\..\Views\MetricTemplates\LocalEducationAgency\Metric\GranularMetric.Level2.Disabled.MetricVariantType_PriorYear.cshtml"
                                                 Write(Model.MetricVariantType);

            
            #line default
            #line hidden
WriteLiteral("</span>\r\n            <span>");


            
            #line 8 "..\..\Views\MetricTemplates\LocalEducationAgency\Metric\GranularMetric.Level2.Disabled.MetricVariantType_PriorYear.cshtml"
              Write(Model.DisplayName);

            
            #line default
            #line hidden
WriteLiteral("</span>\r\n        </p>\r\n    </td>\r\n    <td headers=\"MetricDetails");


            
            #line 11 "..\..\Views\MetricTemplates\LocalEducationAgency\Metric\GranularMetric.Level2.Disabled.MetricVariantType_PriorYear.cshtml"
                          Write(this.GetMetricRenderingContextValue("ParentContainerMetricId"));

            
            #line default
            #line hidden
WriteLiteral("\" colspan=\"6\" data-item-type=\"ContextMenu\">\r\n    </td>\r\n</tr>\r\n");


        }
    }
}
#pragma warning restore 1591