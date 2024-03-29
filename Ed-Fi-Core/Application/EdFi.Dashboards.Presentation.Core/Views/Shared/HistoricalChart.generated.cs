﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18051
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EdFi.Dashboards.Presentation.Core.Views.Shared
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
    
    #line 2 "..\..\Views\Shared\HistoricalChart.cshtml"
    using Newtonsoft.Json;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "1.5.4.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Shared/HistoricalChart.cshtml")]
    public partial class HistoricalChart : System.Web.Mvc.WebViewPage<EdFi.Dashboards.Presentation.Core.Models.Shared.Detail.HistoricalChartModel>
    {
        public HistoricalChart()
        {
        }
        public override void Execute()
        {


WriteLiteral("         \r\n");


            
            #line 4 "..\..\Views\Shared\HistoricalChart.cshtml"
  
    string urlForWebService = (Model.WebServiceUrl).Resolve();

    var blueHeaderSpan = "#blueHeaderSpan" + Model.MetricVariantId + Model.ActionTitle;


            
            #line default
            #line hidden
WriteLiteral("\r\n<div class=\"historical-chart\" id=\"historical-chart-");

            
            
            #line 10 "..\..\Views\Shared\HistoricalChart.cshtml"
                                               Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\">\r\n    \r\n    <div id=\"dataLoading");


            
            #line 12 "..\..\Views\Shared\HistoricalChart.cshtml"
                    Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\" class=\"loading\">\r\n        <span class=\"loading-message\">Loading data...</span>\r" +
"\n    </div>\r\n    \r\n    <br id=\"topSpace");


            
            #line 16 "..\..\Views\Shared\HistoricalChart.cshtml"
                Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\" style=\"display:none;\"/>\r\n    <div class=\"chart-context\">\r\n            <h4 id=\"c" +
"hartContext");


            
            #line 18 "..\..\Views\Shared\HistoricalChart.cshtml"
                            Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\"></h4>\r\n            <h4 id=\"chartSubContext");


            
            #line 19 "..\..\Views\Shared\HistoricalChart.cshtml"
                               Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\"></h4>\r\n    </div>\r\n    <div class=\"chart\" aria-hidden=\"true\">\r\n        \r\n      " +
"  <div class=\"school-goal\" id=\"placeHolderForSchoolGoal");


            
            #line 23 "..\..\Views\Shared\HistoricalChart.cshtml"
                                                         Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\"></div>\r\n        <div class=\"chart-placeholder\" id=\"placeHolderForChart");


            
            #line 24 "..\..\Views\Shared\HistoricalChart.cshtml"
                                                          Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\" ></div>\r\n    \r\n        <div class=\"tooltip\" id=\"chartTooltip");


            
            #line 26 "..\..\Views\Shared\HistoricalChart.cshtml"
                                         Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\">\r\n            <div class=\"tooltip-value\" id=\"tooltipValue");


            
            #line 27 "..\..\Views\Shared\HistoricalChart.cshtml"
                                                   Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\"></div>\r\n        </div>\r\n\r\n        <div class=\"tooltip-indicator\" id=\"tooltipInd" +
"icator");


            
            #line 30 "..\..\Views\Shared\HistoricalChart.cshtml"
                                                       Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\">\r\n            <div class=\"tooltip-indicator-value\" id=\"tooltipIndicatorValue");


            
            #line 31 "..\..\Views\Shared\HistoricalChart.cshtml"
                                                                      Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\"></div>\r\n            <div class=\"tooltip-indicator-pointer\">---</div>\r\n        <" +
"/div>\r\n\r\n        <div class=\"overview\" id=\"placeHolderForChartOverview");


            
            #line 35 "..\..\Views\Shared\HistoricalChart.cshtml"
                                                         Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\"></div>\r\n    \r\n        <div class=\"slider-container doNotPrint\">\r\n            <d" +
"iv class=\"slider\" id=\"sliderContainer");


            
            #line 38 "..\..\Views\Shared\HistoricalChart.cshtml"
                                               Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\" title=\"* Move the slider to the left or right to change the time period.  Click" +
" either side of the highlighted area to expand the number of time periods.\">\r\n  " +
"              <div class=\"left-arrow\" id=\"sliderLeftArrow");


            
            #line 39 "..\..\Views\Shared\HistoricalChart.cshtml"
                                                       Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\"></div>\r\n                <div class=\"middle-area\">\r\n                    <div cla" +
"ss=\"slider-bar ui-draggable\" id=\"sliderBar");


            
            #line 41 "..\..\Views\Shared\HistoricalChart.cshtml"
                                                                  Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\"></div>\r\n                    <div class=\"slider-adjuster slider-adjuster-left\" i" +
"d=\"sliderRangeSelectorLeft");


            
            #line 42 "..\..\Views\Shared\HistoricalChart.cshtml"
                                                                                             Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\"></div>\r\n                    <div class=\"slider-adjuster slider-adjuster-right\" " +
"id=\"sliderRangeSelectorRight");


            
            #line 43 "..\..\Views\Shared\HistoricalChart.cshtml"
                                                                                               Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\"></div>\r\n                </div>\r\n                <div class=\"right-arrow\" id=\"sl" +
"iderRightArrow");


            
            #line 45 "..\..\Views\Shared\HistoricalChart.cshtml"
                                                         Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\"></div>\r\n            </div>\r\n        </div>\r\n    </div>\r\n\r\n        <div id=\"char" +
"t-grid-");


            
            #line 50 "..\..\Views\Shared\HistoricalChart.cshtml"
                        Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\" class=\"hidden\" aria-live=\"assertive\"></div>\r\n\r\n    <br id=\"bottomSpace");


            
            #line 52 "..\..\Views\Shared\HistoricalChart.cshtml"
                   Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\" style=\"display:none;\"/>\r\n    <span id=\"msgsLabel");


            
            #line 53 "..\..\Views\Shared\HistoricalChart.cshtml"
                   Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\" class=\"MetricNoData\" style=\"display: none;\">No data available. (Coming soon.)</" +
"span>\r\n    <span id=\"errorLoading");


            
            #line 54 "..\..\Views\Shared\HistoricalChart.cshtml"
                      Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral("\" class=\"MetricNoData\" style=\"display: none;\">Error loading data.</span>\r\n</div>\r" +
"\n\r\n<script id=\"chart-grid-template-");


            
            #line 57 "..\..\Views\Shared\HistoricalChart.cshtml"
                            Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral(@""" type=""text/html"">
    <table summary=""Historical data for metric ${ChartData.ChartTitle}"">
        <thead>
            <tr id=""chart-grid-header-${metricVariantId}"">
                <th id=""chart-grid-info-${metricVariantId}"">Info</th>
                <th id=""chart-grid-category-${metricVariantId}"">Category</th>
                <th id=""chart-grid-performance-${metricVariantId}"">Performance</th>
            </tr>
        </thead>
        <tbody>
            {{each ChartData.SeriesCollection[0].Points}}
            <tr>
                <td headers=""chart-grid-info-${metricVariantId}"">${Tooltip}</td>
                <td headers=""chart-grid-category-${metricVariantId}"">${Label}</td>
                <td headers=""chart-grid-performance-${metricVariantId}"">${(Value*100).toFixed(1)}%</td>
            </tr>
            {{/each}}
        </tbody>
    </table>
</script>

<script type=""text/javascript"">
		
    $(document).ready(function () {
        //The chart...
        //First get a handle on the placeholders and Dom Objects and vars needed...
        var metricVariantId = ");


            
            #line 83 "..\..\Views\Shared\HistoricalChart.cshtml"
                          Write(Model.MetricVariantId);

            
            #line default
            #line hidden
WriteLiteral(";\r\n        \t\r\n        var schoolGoalPlaceHolder = $(\'#placeHolderForSchoolGoal\'+m" +
"etricVariantId);\r\n\r\n        var blueHeaderSpan = \'");


            
            #line 87 "..\..\Views\Shared\HistoricalChart.cshtml"
                          Write(blueHeaderSpan);

            
            #line default
            #line hidden
WriteLiteral(@"';
        	
        var loadingDiv=$(""#dataLoading""+metricVariantId);
        loadingDiv.css('zIndex', 9999);
        loadingDiv.show();
        
        function loadJsonData(jsonData) {
            loadingDiv.hide();

            //Lets see if we got data back.
            if(hasHistoricalChartasData(jsonData))
            {
                setPeriodTitle(blueHeaderSpan, jsonData.DrillDownTitle);
                var schoolGoal = setSchoolGoal(jsonData.ChartData.StripLines);
                if(schoolGoal)
                    schoolGoalPlaceHolder.text(setSchoolGoal(jsonData.ChartData.StripLines));
                showHistoricalChartComponent(metricVariantId);
                plot(metricVariantId,jsonData.ChartData,null);
                jsonData.metricVariantId = metricVariantId;
                $(""#chart-grid-"" + metricVariantId).html($(""#chart-grid-template-"" + metricVariantId).tmpl(jsonData));
            }
            else //No data so lets hide everything.
            {
                clearPeriodTitle(blueHeaderSpan);
                hideHistoricalChartComponent(metricVariantId);
                $('#msgsLabel'+metricVariantId).show();
            }
        }
        
");


            
            #line 116 "..\..\Views\Shared\HistoricalChart.cshtml"
         if (Model.JsonChartModel == null)
        {

            
            #line default
            #line hidden
WriteLiteral("            ");

WriteLiteral("\r\n            //Then go get data async\r\n            var dataToSendToService = ");


            
            #line 120 "..\..\Views\Shared\HistoricalChart.cshtml"
                                 Write(Html.Raw(Model.ParametersToSendToWebService));

            
            #line default
            #line hidden
WriteLiteral(";\r\n\r\n            $.ajax({\r\n                type: \"POST\",\r\n                url: \'");


            
            #line 124 "..\..\Views\Shared\HistoricalChart.cshtml"
                  Write(urlForWebService);

            
            #line default
            #line hidden
WriteLiteral(@"',
                    data: JSON.stringify(dataToSendToService),
                    contentType: ""application/json; charset=utf-8"",
                    dataType: ""json"",
                    success: function (jsonData) {
                        loadJsonData(jsonData);
                    },
                    error: function (result) {
                        clearPeriodTitle(blueHeaderSpan);
                        loadingDiv.hide();
                        console.log(""AJAX call failed: "" + result.status + ""\n\n "" + result.statusText + ""\n\n obj:""+JSON.stringify(result)); 
                        $('#errorLoading'+metricVariantId).show();
                    }
            });
            ");

WriteLiteral("\r\n");


            
            #line 139 "..\..\Views\Shared\HistoricalChart.cshtml"
        }
        else
        {

            
            #line default
            #line hidden
WriteLiteral("            ");

WriteLiteral("var jsonData = ");


            
            #line 142 "..\..\Views\Shared\HistoricalChart.cshtml"
                         Write(Html.Raw(Newtonsoft.Json.JsonConvert.SerializeObject(Model.JsonChartModel)));

            
            #line default
            #line hidden
WriteLiteral(";\r\n");



WriteLiteral("            ");

WriteLiteral("loadJsonData(jsonData);\r\n");


            
            #line 144 "..\..\Views\Shared\HistoricalChart.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("        \t\r\n        //HistoricalChartFunctions.js Init\r\n        initSlider(metricV" +
"ariantId);\r\n            \t\t\t\r\n    });//End of Document ready.\r\n</script>\r\n    \r\n");


        }
    }
}
#pragma warning restore 1591
