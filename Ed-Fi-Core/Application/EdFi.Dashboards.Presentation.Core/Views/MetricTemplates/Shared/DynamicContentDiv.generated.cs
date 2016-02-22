﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18033
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EdFi.Dashboards.Presentation.Core.Views.MetricTemplates.Shared
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "1.5.4.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/MetricTemplates/Shared/DynamicContentDiv.cshtml")]
    public partial class DynamicContentDiv : System.Web.Mvc.WebViewPage<MetricBase   >
    {
        public DynamicContentDiv()
        {
        }
        public override void Execute()
        {



            #line 2 "..\..\Views\MetricTemplates\Shared\DynamicContentDiv.cshtml"
  
    //Default value is 7 columns.
    var columnSpan = this.GetMetricRenderingContextValue("colCount") ?? 7;
                    

            
            #line default
            #line hidden

            
            #line 6 "..\..\Views\MetricTemplates\Shared\DynamicContentDiv.cshtml"
 foreach(var metricAction in Model.Actions.Where(x => x.ActionType == MetricActionType.DynamicContent || x.ActionType == MetricActionType.AlwaysDisplayedDynamicContent))
{
    var isPriorYearRow = Model.MetricVariantType == MetricVariantType.PriorYear ? "PriorYearRow" : "";
    var dynamicContentName = EdFi.Dashboards.Presentation.Web.Utilities.Legacy.GetDynamicContentNameFromAction(metricAction);

            
            #line default
            #line hidden
WriteLiteral("    <tr id=\"vMetricChart");


            
            #line 10 "..\..\Views\MetricTemplates\Shared\DynamicContentDiv.cshtml"
                    Write(dynamicContentName);

            
            #line default
            #line hidden
WriteLiteral("\" class=\"dynamic-row ");


            
            #line 10 "..\..\Views\MetricTemplates\Shared\DynamicContentDiv.cshtml"
                                                             Write(isPriorYearRow);

            
            #line default
            #line hidden
WriteLiteral("\">\r\n        <td colspan=\"");


            
            #line 11 "..\..\Views\MetricTemplates\Shared\DynamicContentDiv.cshtml"
                 Write(columnSpan);

            
            #line default
            #line hidden
WriteLiteral("\" headers=\"MetricName");


            
            #line 11 "..\..\Views\MetricTemplates\Shared\DynamicContentDiv.cshtml"
                                                   Write(this.GetMetricRenderingContextValue("ParentContainerMetricId"));

            
            #line default
            #line hidden
WriteLiteral("\">\r\n            <div id=\"DynamicContentDiv");


            
            #line 12 "..\..\Views\MetricTemplates\Shared\DynamicContentDiv.cshtml"
                                  Write(dynamicContentName);

            
            #line default
            #line hidden
WriteLiteral("\" style=\"display: none;\" class=\"dynamic-content\">\r\n                ");


            
            #line 13 "..\..\Views\MetricTemplates\Shared\DynamicContentDiv.cshtml"
           Write(Html.Partial("BlueHeader", metricAction));

            
            #line default
            #line hidden
WriteLiteral("\r\n                <div id=\"DrillDownDiv");


            
            #line 14 "..\..\Views\MetricTemplates\Shared\DynamicContentDiv.cshtml"
                                 Write(dynamicContentName);

            
            #line default
            #line hidden
WriteLiteral("\" class=\"drilldown\" style=\"display: none;\"></div>\r\n                <div id=\"Loadi" +
"ngDiv");


            
            #line 15 "..\..\Views\MetricTemplates\Shared\DynamicContentDiv.cshtml"
                               Write(dynamicContentName);

            
            #line default
            #line hidden
WriteLiteral("\">\r\n                    <div class=\"MetricNoData\">\r\n                        <span" +
" class=\"loading-message\">Loading data...</span>\r\n                    </div>\r\n   " +
"             </div>\r\n            </div>\r\n        </td>\r\n    </tr>\r\n");


            
            #line 23 "..\..\Views\MetricTemplates\Shared\DynamicContentDiv.cshtml"
}

            
            #line default
            #line hidden

        }
    }
}
#pragma warning restore 1591