﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASP
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
    
    #line 3 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
    using EdFi.Dashboards.Common;
    
    #line default
    #line hidden
    
    #line 4 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
    using EdFi.Dashboards.Core.Providers.Context;
    
    #line default
    #line hidden
    using EdFi.Dashboards.Metric.Resources.Models;
    using EdFi.Dashboards.Presentation.Architecture.Mvc.Extensions;
    
    #line 5 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
    using EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers;
    
    #line default
    #line hidden
    
    #line 6 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
    using EdFi.Dashboards.Presentation.Core.Utilities.ExtensionMethods.HtmlHelper;
    
    #line default
    #line hidden
    using EdFi.Dashboards.Presentation.Web.Utilities;
    
    #line 7 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
    using EdFi.Dashboards.Resources.Models.LocalEducationAgency.Information;
    
    #line default
    #line hidden
    using EdFi.Dashboards.Resources.Navigation;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Areas/LocalEducationAgency/Views/Information/Get.cshtml")]
    public partial class _Areas_LocalEducationAgency_Views_Information_Get_cshtml : System.Web.Mvc.WebViewPage<InformationModel>
    {
        public _Areas_LocalEducationAgency_Views_Information_Get_cshtml()
        {
        }
        public override void Execute()
        {


WriteLiteral("\r\n");








            
            #line 8 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
  
    //ViewBag.Title = Model.Name + " - District Information";


            
            #line default
            #line hidden

DefineSection("ContentPlaceHolderHead", () => {

WriteLiteral("\r\n    <title>");


            
            #line 12 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
       Write(Html.Action("Title", typeof(LocalEducationAgencyLayoutController).GetControllerName(), new { subTitle = "District Information" }));

            
            #line default
            #line hidden
WriteLiteral("</title>\r\n    <script type=\"text/javascript\">\r\n        var pageArea = { Area: \'lo" +
"cal education agency\', Page: \'District Information\' };\r\n    </script>\r\n");


});

WriteLiteral("\r\n\r\n");


DefineSection("ContentPlaceHolder1", () => {

WriteLiteral("\r\n");


            
            #line 19 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
      
        var localEducationAgencyId = EdFiDashboardContext.Current.LocalEducationAgencyId.GetValueOrDefault();
    

            
            #line default
            #line hidden
WriteLiteral(@"    <div class=""tab-content"">
        <div role=""tabpanel"" class=""tab-pane active tab-red-damask"" id=""tab-student-info"">
            <div class=""action-btn action-btn-red-damask"">
                <a class=""btn"" href=""#""><i class=""fa fa-download""></i></a>
                <a class=""btn"" href=""#""><i class=""fa fa-print""></i></a>
                <a class=""btn"" href=""#""><i class=""fa fa-pencil""></i></a>
            </div>
            <div class=""clearfix edfitable clear"">
                <div class=""col-sm-6 col-md-4 std-grid-md-4"">
                    <div class=""std-primary-col first"">
                        <div class=""row pt40 mb20"">
                            <h3 class=""std-name std-red-damask schoolhead"">");


            
            #line 33 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                                                       Write(Model.Name);

            
            #line default
            #line hidden
WriteLiteral(@"</h3>
                            <div class=""col-xs-6 sm-pr0 std-primary-xs-small"">
                                <div class=""std-info relative"">
                                    <div class=""left"">
                                        <!-- District-thumbnail -->
                                        <img id=""InformationModel.ProfileThumbnail"" src=""");


            
            #line 38 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                                                                     Write(Model.ProfileThumbnail);

            
            #line default
            #line hidden
WriteLiteral("\" alt=\"Photograph of ");


            
            #line 38 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                                                                                                                   Write(Model.Name);

            
            #line default
            #line hidden
WriteLiteral(@""" />
                                    </div>
                                </div>
                            </div>
                            <div class=""col-xs-6 sm-pl0 sm-pr0 std-primary-xs-small"">
                                <address class=""std-address"">
");


            
            #line 44 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                     foreach (var addressLine in Model.AddressLines)
                                    {
                                        
            
            #line default
            #line hidden
            
            #line 46 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                    Write(addressLine);

            
            #line default
            #line hidden

WriteLiteral("<br />\r\n");


            
            #line 47 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                    }

            
            #line default
            #line hidden
WriteLiteral("                                    <span>");


            
            #line 48 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                      Write(Model.City);

            
            #line default
            #line hidden
WriteLiteral(", ");


            
            #line 48 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                                     Write(Model.State);

            
            #line default
            #line hidden
WriteLiteral(" ");


            
            #line 48 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                                                    Write(Model.ZipCode);

            
            #line default
            #line hidden
WriteLiteral("</span>\r\n                                </address>\r\n                            " +
"    <div class=\"std-desc\">\r\n");


            
            #line 51 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                     foreach (var telephoneNumber in Model.TelephoneNumbers)
                                    {

            
            #line default
            #line hidden
WriteLiteral("                                        <span class=\"tel\"><i class=\"icon icon-tel" +
"\"></i>");


            
            #line 53 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                                                                   Write(telephoneNumber.Number);

            
            #line default
            #line hidden
WriteLiteral("</span>");



WriteLiteral("<span>&nbsp;&nbsp;");


            
            #line 53 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                                                                                                                     Write(telephoneNumber.Type);

            
            #line default
            #line hidden
WriteLiteral("</span>");



WriteLiteral("<br />\r\n");


            
            #line 54 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                    }

            
            #line default
            #line hidden
WriteLiteral("                                    ");



WriteLiteral(@"
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class=""col-sm-6 col-md-4 std-grid-md-5"">
                    <div class=""std-primary-col std-primary-col-second"">
                        <div class=""sm-pl3 clearfix"">
                            <h4 class=""std-title-red-damask"">Administration</h4>
");


            
            #line 65 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                             foreach (var administrator in Model.Administrators)
                            {

            
            #line default
            #line hidden
WriteLiteral("                                <div class=\"split\">\r\n                            " +
"        <h5 class=\"std-sub-title\">");


            
            #line 68 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                                         Write(administrator.Role);

            
            #line default
            #line hidden
WriteLiteral(" : </h5><span> ");


            
            #line 68 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                                                                           Write(administrator.Name);

            
            #line default
            #line hidden
WriteLiteral(" </span>\r\n                                </div>\r\n");


            
            #line 70 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                            }

            
            #line default
            #line hidden
WriteLiteral("                        </div>\r\n                        <div class=\"row\">\r\n      " +
"                      <div class=\"sm-pl30 sm-p130new\">\r\n                        " +
"        <h4 class=\"std-title-red-damask\">Accountability</h4>\r\n");


            
            #line 75 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                 foreach (var accountability in Model.Accountability)
                                {

            
            #line default
            #line hidden
WriteLiteral("                                    <h5 class=\"std-sub-title\">");


            
            #line 77 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                                         Write(accountability.Attribute);

            
            #line default
            #line hidden
WriteLiteral(" : </h5> ");


            
            #line 77 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                                                                                
            
            #line default
            #line hidden
            
            #line 77 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                                                                           Write(accountability.Value);

            
            #line default
            #line hidden

WriteLiteral(" <br />\r\n");


            
            #line 78 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                }

            
            #line default
            #line hidden
WriteLiteral(@"                            </div>
                        </div>
                    </div>
                </div>
                <div class=""clearfix visible-sm""></div>
                <div class=""col-sm-12 col-md-4 std-grid-md-3"">
                    <div class=""std-primary-col std-primary-col-third"">
                        <div class=""sm-pl20"">
                            <h4 class=""std-title-red-damask"">Number of Schools by Rating</h4>
                            <table class=""table table-demograhpics"">
");


            
            #line 89 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                 foreach (var schoolAccountability in Model.SchoolAccountabilityRatings)
                                {

            
            #line default
            #line hidden
WriteLiteral("                                    <tr>\r\n                                       " +
" <td>\r\n                                            <span class=\"label\">");


            
            #line 93 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                                            Write(schoolAccountability.Attribute);

            
            #line default
            #line hidden
WriteLiteral("</span>\r\n                                            <span class=\"value\">");


            
            #line 94 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                                            Write(schoolAccountability.Value);

            
            #line default
            #line hidden
WriteLiteral("</span>\r\n                                        </td>\r\n                         " +
"           </tr>\r\n");


            
            #line 97 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                }

            
            #line default
            #line hidden
WriteLiteral("                            </table>\r\n                            ");



WriteLiteral(@"
                        </div>
                        <div class=""sm-pl20"">
                            <h4 class=""std-title-red-damask"">District Characteristics</h4>
                            <table class=""table table-demograhpics"">
                                <tr>
                                    <td>
                                        <span class=""label"">District Enrollment</span>
                                        <span class=""value"">");


            
            #line 109 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                                        Write( String.Format("{0:#,#}", Model.LocalEducationAgencyEnrollment));

            
            #line default
            #line hidden
WriteLiteral(@"</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span class=""label"">Number of Schools</span>
                                        <span class=""value""></span>
                                    </td>
                                </tr>
");


            
            #line 118 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                 foreach (var schoolCategory in Model.NumberOfSchools)
                                {

            
            #line default
            #line hidden
WriteLiteral("                                    <tr>\r\n                                       " +
" <td>\r\n                                            ");


            
            #line 122 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                        Write(Html.AttributeItemNumberWithUrl(schoolCategory, false));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                        </td>\r\n                                " +
"    </tr>\r\n");


            
            #line 125 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                }

            
            #line default
            #line hidden
WriteLiteral("                                <tr>\r\n                                    <td>\r\n " +
"                                       <span>");


            
            #line 128 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                          Write(Html.AttributeItemPercentageWithUrl(Model.LateEnrollmentStudents));

            
            #line default
            #line hidden
WriteLiteral("</span>\r\n                                    </td>\r\n                             " +
"   </tr>\r\n");


            
            #line 131 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                 if (Model.StudentTeacherRatios.Any())
                                {

            
            #line default
            #line hidden
WriteLiteral(@"                                    <tr>
                                        <td>
                                            <span class=""label"">Student Teacher Ratio</span>
                                            <span class=""value""></span>
                                        </td>
                                    </tr>
");


            
            #line 139 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                    foreach (var studentTeacherRatio in Model.StudentTeacherRatios)
                                    {

            
            #line default
            #line hidden
WriteLiteral("                                        <tr>\r\n                                   " +
"         <td>\r\n                                                <span class=\"labe" +
"l\">");


            
            #line 143 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                                                Write(studentTeacherRatio.Attribute);

            
            #line default
            #line hidden
WriteLiteral("</span>\r\n                                                <span class=\"value\">");


            
            #line 144 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                                                Write(studentTeacherRatio.Value);

            
            #line default
            #line hidden
WriteLiteral("</span>\r\n                                            </td>\r\n                     " +
"                   </tr>\r\n");


            
            #line 147 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                    }
                                }

            
            #line default
            #line hidden
WriteLiteral(@"                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class=""row std-secondarymain"">
            <div class=""std-secondary no-gutter clearfix"">
                <div class=""col-sm-6 col-md-4 std-tablefirst"">
                    <h4 class=""std-secondary-title first"">Student Demographics</h4>
                    <div class=""std-secondary-col first"">
                        <table class=""table std-table"">
                            <tr>
                                <td><b>Gender</b></td>
                                <td>");


            
            #line 163 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                Write(Html.AttributeItemPercentageWithUrl(Model.StudentDemographics.Female));

            
            #line default
            #line hidden
WriteLiteral(" <br /> ");


            
            #line 163 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                                                                                                Write(Html.AttributeItemPercentageWithUrl(Model.StudentDemographics.Male));

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                            </tr>\r\n                            <tr>\r\n     " +
"                           <td><b>Ethnicity</b></td>\r\n                          " +
"      <td>\r\n");


            
            #line 168 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                     foreach (var ethnicity in Model.StudentDemographics.ByEthnicity)
                                    {
                                        
            
            #line default
            #line hidden
            
            #line 170 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                    Write(Html.AttributeItemPercentageWithUrl(ethnicity));

            
            #line default
            #line hidden

WriteLiteral(" <br />\r\n");


            
            #line 171 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                    }

            
            #line default
            #line hidden
WriteLiteral("                                </td>\r\n                            </tr>\r\n       " +
"                     <tr>\r\n                                <td><b>Race</b></td>\r" +
"\n                                <td>\r\n");


            
            #line 177 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                     foreach (var race in Model.StudentDemographics.ByRace)
                                    {
                                        
            
            #line default
            #line hidden
            
            #line 179 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                    Write(Html.AttributeItemPercentageWithUrl(race));

            
            #line default
            #line hidden

WriteLiteral(" <br />\r\n");


            
            #line 180 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                    }

            
            #line default
            #line hidden
WriteLiteral(@"                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div class=""col-sm-6 col-md-4"">
                    <h4 class=""std-secondary-title"">Students by Program</h4>
                    <div class=""std-secondary-col"">
                        <table class=""table std-table"">
");


            
            #line 190 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                             foreach (var program in Model.StudentsByProgram)
                            {

            
            #line default
            #line hidden
WriteLiteral("                                <tr>\r\n                                    <td>");


            
            #line 193 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                    Write(Html.AttributeItemPercentageWithUrl(program));

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                                </tr>\r\n");


            
            #line 195 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                            }

            
            #line default
            #line hidden
WriteLiteral(@"                        </table>
                    </div>
                </div>
                <div class=""col-sm-6 col-md-4"">
                    <h4 class=""std-secondary-title"">Other Student Information</h4>
                    <div class=""std-secondary-col"">
                        <table class=""table std-table"">
");


            
            #line 203 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                             foreach (var indicator in Model.StudentIndicatorPopulation)
                            {

            
            #line default
            #line hidden
WriteLiteral("                                <tr>\r\n                                    <td>\r\n " +
"                                       ");


            
            #line 207 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                                    Write(Html.AttributeItemPercentageWithUrl(indicator));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                    </td>\r\n                                </tr" +
">\r\n");


            
            #line 210 "..\..\Areas\LocalEducationAgency\Views\Information\Get.cshtml"
                            }

            
            #line default
            #line hidden
WriteLiteral("                        </table>\r\n                    </div>\r\n                </d" +
"iv>\r\n            </div>\r\n        </div>\r\n    </div>\r\n");


});

WriteLiteral("\r\n\r\n");


        }
    }
}
#pragma warning restore 1591