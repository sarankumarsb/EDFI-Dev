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

namespace EdFi.Dashboards.Presentation.Core.Views.MetricTemplates.StudentSchool.Metric
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/MetricTemplates/StudentSchool/Metric/ContainerMetric.Level3.Disabled.csht" +
        "ml")]
    public class ContainerMetric_Level3_Disabled : System.Web.Mvc.WebViewPage<ContainerMetric>
    {
        public ContainerMetric_Level3_Disabled()
        {
        }
        public override void Execute()
        {


            
            #line 2 "..\..\Views\MetricTemplates\StudentSchool\Metric\ContainerMetric.Level3.Disabled.cshtml"
  
    var columnCountValue = ViewBag.MetricRenderingContext.ContainsKey("colCount") ? ViewBag.MetricRenderingContext["colCount"] : null;
    var columnSpan = (columnCountValue ?? 6) - 1;//-1 for the Details Column that will always exist in this template.
 

            
            #line default
            #line hidden
WriteLiteral("<tr id=\"vMetric");


            
            #line 6 "..\..\Views\MetricTemplates\StudentSchool\Metric\ContainerMetric.Level3.Disabled.cshtml"
           Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\" class=\"container\"  data-metric-id=\"");


            
            #line 6 "..\..\Views\MetricTemplates\StudentSchool\Metric\ContainerMetric.Level3.Disabled.cshtml"
                                                                        Write(Model.MetricId);

            
            #line default
            #line hidden
WriteLiteral("\" data-metric-variant-id=\"");


            
            #line 6 "..\..\Views\MetricTemplates\StudentSchool\Metric\ContainerMetric.Level3.Disabled.cshtml"
                                                                                                                   Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\">\r\n    <td headers=\"MetricName");


            
            #line 7 "..\..\Views\MetricTemplates\StudentSchool\Metric\ContainerMetric.Level3.Disabled.cshtml"
                       Write(this.GetMetricRenderingContextValue("ParentContainerMetricId"));

            
            #line default
            #line hidden
WriteLiteral("\" data-item-type=\"DisplayName\" colspan=\"");


            
            #line 7 "..\..\Views\MetricTemplates\StudentSchool\Metric\ContainerMetric.Level3.Disabled.cshtml"
                                                                                                                                Write(columnSpan);

            
            #line default
            #line hidden
WriteLiteral("\" title=\"");


            
            #line 7 "..\..\Views\MetricTemplates\StudentSchool\Metric\ContainerMetric.Level3.Disabled.cshtml"
                                                                                                                                                     Write(Model.ToolTip);

            
            #line default
            #line hidden
WriteLiteral("\">\r\n        <p class=\"title deemphasized\">\r\n            <span class=\"metricId\">");


            
            #line 9 "..\..\Views\MetricTemplates\StudentSchool\Metric\ContainerMetric.Level3.Disabled.cshtml"
                               Write(Model.MetricId);

            
            #line default
            #line hidden
WriteLiteral("-");


            
            #line 9 "..\..\Views\MetricTemplates\StudentSchool\Metric\ContainerMetric.Level3.Disabled.cshtml"
                                                 Write(Model.MetricVariantType);

            
            #line default
            #line hidden
WriteLiteral("</span>\r\n            ");


            
            #line 10 "..\..\Views\MetricTemplates\StudentSchool\Metric\ContainerMetric.Level3.Disabled.cshtml"
        Write(Model.DisplayName);

            
            #line default
            #line hidden
WriteLiteral("\r\n");


            
            #line 11 "..\..\Views\MetricTemplates\StudentSchool\Metric\ContainerMetric.Level3.Disabled.cshtml"
             if (!String.IsNullOrWhiteSpace(Model.Context))
            {

            
            #line default
            #line hidden
WriteLiteral("                <span>&nbsp;(");


            
            #line 13 "..\..\Views\MetricTemplates\StudentSchool\Metric\ContainerMetric.Level3.Disabled.cshtml"
                        Write(Model.Context);

            
            #line default
            #line hidden
WriteLiteral(")</span>\r\n");


            
            #line 14 "..\..\Views\MetricTemplates\StudentSchool\Metric\ContainerMetric.Level3.Disabled.cshtml"
            }

            
            #line default
            #line hidden
WriteLiteral("            ");


            
            #line 15 "..\..\Views\MetricTemplates\StudentSchool\Metric\ContainerMetric.Level3.Disabled.cshtml"
       Write(Html.Partial("MetricFootnoteLabelSuperText", Model.Footnotes));

            
            #line default
            #line hidden
WriteLiteral("\r\n            <a name=\"m");


            
            #line 16 "..\..\Views\MetricTemplates\StudentSchool\Metric\ContainerMetric.Level3.Disabled.cshtml"
                  Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\">&nbsp;</a>\r\n");


            
            #line 17 "..\..\Views\MetricTemplates\StudentSchool\Metric\ContainerMetric.Level3.Disabled.cshtml"
             foreach (var metricBase in Model.Children)
            {              

            
            #line default
            #line hidden
WriteLiteral("                <a name=\"m");


            
            #line 19 "..\..\Views\MetricTemplates\StudentSchool\Metric\ContainerMetric.Level3.Disabled.cshtml"
                      Write(metricBase.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\">&nbsp;</a>\r\n");


            
            #line 20 "..\..\Views\MetricTemplates\StudentSchool\Metric\ContainerMetric.Level3.Disabled.cshtml"
            }

            
            #line default
            #line hidden
WriteLiteral("        </p>\r\n        <p class=\"description deemphasized\"><span>");


            
            #line 22 "..\..\Views\MetricTemplates\StudentSchool\Metric\ContainerMetric.Level3.Disabled.cshtml"
                                             Write(Model.Description);

            
            #line default
            #line hidden
WriteLiteral("</span></p>\r\n    </td>\r\n    <td headers=\"MetricDetails");


            
            #line 24 "..\..\Views\MetricTemplates\StudentSchool\Metric\ContainerMetric.Level3.Disabled.cshtml"
                          Write(this.GetMetricRenderingContextValue("ParentContainerMetricId"));

            
            #line default
            #line hidden
WriteLiteral("\" data-item-type=\"ContextMenu\">\r\n    </td>\r\n</tr>");


        }
    }
}
#pragma warning restore 1591
