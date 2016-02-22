﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EdFi.Dashboards.Presentation.Core.Views.MetricTemplates.LocalEducationAgency.GoalPlanning
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/MetricTemplates/LocalEducationAgency/GoalPlanning/ContainerMetric.Level1." +
        "cshtml")]
    public partial class ContainerMetric_Level1 : System.Web.Mvc.WebViewPage<ContainerMetric>
    {
        public ContainerMetric_Level1()
        {
        }
        public override void Execute()
        {


            
            #line 2 "..\..\Views\MetricTemplates\LocalEducationAgency\GoalPlanning\ContainerMetric.Level1.cshtml"
  
    //Define the ParentContainerMetricId to propagate fo the unique columns.
    //This si needed because we have "Complex tables" meaning colspans that then need unique ids in the headers and matching the cells in the rows below with the same headers="uniqueColumnId". 
    this.SetMetricRenderingContextValue("ParentContainerMetricId", Model.MetricVariantId);


            
            #line default
            #line hidden
WriteLiteral("<table summary=\"This table displays metrics regarding ");


            
            #line 7 "..\..\Views\MetricTemplates\LocalEducationAgency\GoalPlanning\ContainerMetric.Level1.cshtml"
                                                  Write(Model.DisplayName);

            
            #line default
            #line hidden
WriteLiteral(".\">\r\n<thead>\r\n    <tr id=\"vMetric");


            
            #line 9 "..\..\Views\MetricTemplates\LocalEducationAgency\GoalPlanning\ContainerMetric.Level1.cshtml"
               Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\" class=\"container\">\r\n        <th id=\"MetricName");


            
            #line 10 "..\..\Views\MetricTemplates\LocalEducationAgency\GoalPlanning\ContainerMetric.Level1.cshtml"
                      Write(this.GetMetricRenderingContextValue("ParentContainerMetricId"));

            
            #line default
            #line hidden
WriteLiteral("\" title=\"");


            
            #line 10 "..\..\Views\MetricTemplates\LocalEducationAgency\GoalPlanning\ContainerMetric.Level1.cshtml"
                                                                                                Write(Model.ToolTip);

            
            #line default
            #line hidden
WriteLiteral("\" style=\"width: 43%\">\r\n            <span id=\"vMetricVariantId");


            
            #line 11 "..\..\Views\MetricTemplates\LocalEducationAgency\GoalPlanning\ContainerMetric.Level1.cshtml"
                                  Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\" class=\"metricId\">\r\n                ");


            
            #line 12 "..\..\Views\MetricTemplates\LocalEducationAgency\GoalPlanning\ContainerMetric.Level1.cshtml"
            Write(Model.MetricId);

            
            #line default
            #line hidden
WriteLiteral("-");


            
            #line 12 "..\..\Views\MetricTemplates\LocalEducationAgency\GoalPlanning\ContainerMetric.Level1.cshtml"
                              Write(Model.MetricVariantType);

            
            #line default
            #line hidden
WriteLiteral("\r\n            </span>\r\n            <a id=\"");


            
            #line 14 "..\..\Views\MetricTemplates\LocalEducationAgency\GoalPlanning\ContainerMetric.Level1.cshtml"
               Write(Model.DisplayName);

            
            #line default
            #line hidden
WriteLiteral("\">");


            
            #line 14 "..\..\Views\MetricTemplates\LocalEducationAgency\GoalPlanning\ContainerMetric.Level1.cshtml"
                                     Write(Model.DisplayName);

            
            #line default
            #line hidden
WriteLiteral("</a>\r\n            <a id=\"m");


            
            #line 15 "..\..\Views\MetricTemplates\LocalEducationAgency\GoalPlanning\ContainerMetric.Level1.cshtml"
                Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\"></a>\r\n        </th>\r\n        <th id=\"MetricValue");


            
            #line 17 "..\..\Views\MetricTemplates\LocalEducationAgency\GoalPlanning\ContainerMetric.Level1.cshtml"
                       Write(this.GetMetricRenderingContextValue("ParentContainerMetricId"));

            
            #line default
            #line hidden
WriteLiteral("\" title=\"Color indicates if meeting school goal\" style=\"width: 11%\">\r\n           " +
" METRIC VALUE<br />\r\n            <small>(% of teachers)</small>\r\n        </th>\r\n" +
"        <th id=\"MetricGoal");


            
            #line 21 "..\..\Views\MetricTemplates\LocalEducationAgency\GoalPlanning\ContainerMetric.Level1.cshtml"
                      Write(this.GetMetricRenderingContextValue("ParentContainerMetricId"));

            
            #line default
            #line hidden
WriteLiteral("\" style=\"width: 15%\">\r\n            CURRENT DISTRICT GOAL<br />\r\n            <smal" +
"l>(% of teachers)</small>\r\n        </th>\r\n        <th id=\"MetricDifferenceFromGo" +
"al");


            
            #line 25 "..\..\Views\MetricTemplates\LocalEducationAgency\GoalPlanning\ContainerMetric.Level1.cshtml"
                                    Write(this.GetMetricRenderingContextValue("ParentContainerMetricId"));

            
            #line default
            #line hidden
WriteLiteral("\" title=\"Color indicates if meeting school goal\" style=\"width: 10%\">\r\n           " +
" DIFFERENCE FROM GOAL\r\n        </th>\r\n        <th id=\"MetricNewGoal");


            
            #line 28 "..\..\Views\MetricTemplates\LocalEducationAgency\GoalPlanning\ContainerMetric.Level1.cshtml"
                         Write(this.GetMetricRenderingContextValue("ParentContainerMetricId"));

            
            #line default
            #line hidden
WriteLiteral("\" style=\"width: 12%\">\r\n            NEW DISTRICT GOAL\r\n        </th>\r\n        <th " +
"id=\"DropDown");


            
            #line 31 "..\..\Views\MetricTemplates\LocalEducationAgency\GoalPlanning\ContainerMetric.Level1.cshtml"
                    Write(this.GetMetricRenderingContextValue("ParentContainerMetricId"));

            
            #line default
            #line hidden
WriteLiteral("\" style=\"width: 9%\">\r\n            SCHOOL GOALS\r\n        </th>\r\n    </tr>\r\n</thead" +
">\r\n<tbody>\r\n");


        }
    }
}
#pragma warning restore 1591