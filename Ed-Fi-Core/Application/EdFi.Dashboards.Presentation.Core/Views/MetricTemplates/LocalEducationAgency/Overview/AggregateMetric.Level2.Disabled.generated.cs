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

namespace EdFi.Dashboards.Presentation.Core.Views.MetricTemplates.LocalEducationAgency.Overview
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/MetricTemplates/LocalEducationAgency/Overview/AggregateMetric.Level2.Disa" +
        "bled.cshtml")]
    public class AggregateMetric_Level2_Disabled : System.Web.Mvc.WebViewPage<AggregateMetric>
    {
        public AggregateMetric_Level2_Disabled()
        {
        }
        public override void Execute()
        {

WriteLiteral("\r\n<tr id=\"vMetric");


            
            #line 3 "..\..\Views\MetricTemplates\LocalEducationAgency\Overview\AggregateMetric.Level2.Disabled.cshtml"
           Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\" style=\"height: 35px;\">\r\n    <td title=\"");


            
            #line 4 "..\..\Views\MetricTemplates\LocalEducationAgency\Overview\AggregateMetric.Level2.Disabled.cshtml"
          Write(Model.ToolTip);

            
            #line default
            #line hidden
WriteLiteral("\" class=\"bulleted-metric bulleted-metric-indented bulleted-metric-disabled\">\r\n   " +
"     <i class=\"icon-blank\"></i>\r\n        <h3>\r\n            <a href=\"");


            
            #line 7 "..\..\Views\MetricTemplates\LocalEducationAgency\Overview\AggregateMetric.Level2.Disabled.cshtml"
                 Write(Url.Content(Model.Url));

            
            #line default
            #line hidden
WriteLiteral("\" id=\"vName");


            
            #line 7 "..\..\Views\MetricTemplates\LocalEducationAgency\Overview\AggregateMetric.Level2.Disabled.cshtml"
                                                     Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\">\r\n                ");


            
            #line 8 "..\..\Views\MetricTemplates\LocalEducationAgency\Overview\AggregateMetric.Level2.Disabled.cshtml"
           Write(Model.DisplayName);

            
            #line default
            #line hidden
WriteLiteral(" <span class=\"metricId\">");


            
            #line 8 "..\..\Views\MetricTemplates\LocalEducationAgency\Overview\AggregateMetric.Level2.Disabled.cshtml"
                                                      Write(Model.MetricId);

            
            #line default
            #line hidden
WriteLiteral("-");



            #line 8 "..\..\Views\MetricTemplates\LocalEducationAgency\Overview\AggregateMetric.Level2.Disabled.cshtml"
                                                                        Write(Model.MetricVariantType);
            
            
            #line default
            #line hidden
WriteLiteral("</span>\r\n            </a>\r\n");


            
            #line 10 "..\..\Views\MetricTemplates\LocalEducationAgency\Overview\AggregateMetric.Level2.Disabled.cshtml"
             if (!string.IsNullOrEmpty(Model.Context))
            {

            
            #line default
            #line hidden
WriteLiteral("                <span id=\"vContext");


            
            #line 12 "..\..\Views\MetricTemplates\LocalEducationAgency\Overview\AggregateMetric.Level2.Disabled.cshtml"
                              Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\">\r\n                    &nbsp;(");


            
            #line 13 "..\..\Views\MetricTemplates\LocalEducationAgency\Overview\AggregateMetric.Level2.Disabled.cshtml"
                      Write(Model.Context);

            
            #line default
            #line hidden
WriteLiteral(")\r\n                </span>\r\n");


            
            #line 15 "..\..\Views\MetricTemplates\LocalEducationAgency\Overview\AggregateMetric.Level2.Disabled.cshtml"
            }

            
            #line default
            #line hidden
WriteLiteral("            <span id=\"vDescription");


            
            #line 16 "..\..\Views\MetricTemplates\LocalEducationAgency\Overview\AggregateMetric.Level2.Disabled.cshtml"
                              Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\" class=\"description\">\r\n                ");


            
            #line 17 "..\..\Views\MetricTemplates\LocalEducationAgency\Overview\AggregateMetric.Level2.Disabled.cshtml"
           Write(Model.Description);

            
            #line default
            #line hidden
WriteLiteral("\r\n            </span>\r\n        </h3>\r\n    </td>\r\n    <td style=\"text-align: left;" +
"\" align=\"left\"></td>\r\n    <td style=\"text-align: left;\" align=\"left\"></td>\r\n</tr" +
">\r\n");


        }
    }
}
#pragma warning restore 1591