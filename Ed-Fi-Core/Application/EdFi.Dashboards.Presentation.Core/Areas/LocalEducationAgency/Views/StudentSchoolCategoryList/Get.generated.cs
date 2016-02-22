﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Views.StudentSchoolCategoryList
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
    
    #line 1 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
    using EdFi.Dashboards.Common;
    
    #line default
    #line hidden
    
    #line 2 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
    using EdFi.Dashboards.Common.Utilities;
    
    #line default
    #line hidden
    
    #line 3 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
    using EdFi.Dashboards.Core.Providers.Context;
    
    #line default
    #line hidden
    using EdFi.Dashboards.Metric.Resources.Models;
    using EdFi.Dashboards.Presentation.Architecture.Mvc.Extensions;
    
    #line 4 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
    using EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers;
    
    #line default
    #line hidden
    
    #line 5 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
    using EdFi.Dashboards.Presentation.Core.Models.Shared;
    
    #line default
    #line hidden
    
    #line 6 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
    using EdFi.Dashboards.Presentation.Core.Providers.Models;
    
    #line default
    #line hidden
    using EdFi.Dashboards.Presentation.Web.Utilities;
    using EdFi.Dashboards.Resources.Navigation;
    
    #line 7 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
    using EdFi.Dashboards.Resources.Navigation.Mvc;
    
    #line default
    #line hidden
    
    #line 8 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
    using EdFi.Dashboards.SecurityTokenService.Authentication;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Areas/LocalEducationAgency/Views/StudentSchoolCategoryList/Get.cshtml")]
    public partial class Get : System.Web.Mvc.WebViewPage<EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Models.StudentSchoolCategoryList.StudentSchoolCategoryListModel>
    {
        public Get()
        {
        }
        public override void Execute()
        {








WriteLiteral("\n");


WriteLiteral("           \n");


            
            #line 12 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
  
    var safeName = Model.Title.Replace(" ", "").Replace("/","");
    var localEducationAgencyId = EdFiDashboardContext.Current.LocalEducationAgencyId.GetValueOrDefault();
    var staffUSI = UserInformation.Current.StaffUSI;


            
            #line default
            #line hidden
WriteLiteral("\n");


DefineSection("ContentPlaceHolderHead", () => {

WriteLiteral("\n    <title>");


            
            #line 19 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
       Write(Html.Action("Title", typeof(LocalEducationAgencyLayoutController).GetControllerName(), new { subTitle = Model.Title }));

            
            #line default
            #line hidden
WriteLiteral("</title>\n    <script>\n        var pageArea = { Area: \'local education agency\', Pa" +
"ge: \'Students by Level\' };\n    </script>\n");


});

WriteLiteral("\n\n");


DefineSection("ContentPlaceHolder1", () => {

WriteLiteral("\n\t<div id=\"gradeSelector\" class=\"student-drop-down\">\n\t<h2>Students by Level</h2>\n" +
"\t<label class=\"content-label\" for=\"categorySelect\">Select level to show:</label>" +
"\n\t<select id=\"categorySelect\" class=\"drop-down\">\n");


            
            #line 30 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
 		if (Model.Title.ToLower() == "select level")
		{

            
            #line default
            #line hidden
WriteLiteral("\t\t\t<option selected=\"selected\" >");


            
            #line 32 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
                           Write(Model.Title.ToUpper());

            
            #line default
            #line hidden
WriteLiteral("</option>\n");


            
            #line 33 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
		}

            
            #line default
            #line hidden

            
            #line 34 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
 		foreach (var schoolCategory in Model.MenuModel.SchoolCategories)
		{

            
            #line default
            #line hidden
WriteLiteral("\t\t\t<option value=\"");


            
            #line 36 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
             Write(schoolCategory.Url);

            
            #line default
            #line hidden
WriteLiteral("\"\n");


            
            #line 37 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
 				if (Request.Url != null && schoolCategory.Selected)
				{

            
            #line default
            #line hidden
WriteLiteral("\t\t\t\t\t");

WriteLiteral(" selected=\'selected\'\n");


            
            #line 40 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
				}
            
            #line default
            #line hidden
WriteLiteral(">\n\t\t\t\t");


            
            #line 41 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
Write(schoolCategory.Attribute);

            
            #line default
            #line hidden
WriteLiteral("\n\t\t\t</option>\n");


            
            #line 43 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
		}

            
            #line default
            #line hidden

            
            #line 44 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
 		if (Model.MenuModel.DynamicWatchLists.Any())
		{

            
            #line default
            #line hidden
WriteLiteral("\t\t\t<optgroup label=\"Dynamic Lists\">\n");


            
            #line 47 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
 			foreach (var watchList in Model.MenuModel.DynamicWatchLists)
			{

            
            #line default
            #line hidden
WriteLiteral("\t\t\t\t<option value=\"");


            
            #line 49 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
              Write(watchList.Url);

            
            #line default
            #line hidden
WriteLiteral("\"\n");


            
            #line 50 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
 					if (Request.Url != null && watchList.Selected) 
					{ 

            
            #line default
            #line hidden
WriteLiteral("\t\t\t\t\t\t");

WriteLiteral(" selected=\'selected\'\n");


            
            #line 53 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
					}
            
            #line default
            #line hidden
WriteLiteral(">\n\t\t\t\t\t");


            
            #line 54 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
Write(watchList.Attribute);

            
            #line default
            #line hidden
WriteLiteral("\n\t\t\t\t</option>\n");


            
            #line 56 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
			}

            
            #line default
            #line hidden
WriteLiteral("\t\t\t</optgroup>\n");


            
            #line 58 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
		}

            
            #line default
            #line hidden
WriteLiteral("\t</select>\n    </div>\n    <script>\n        $(document).ready(function () {\n      " +
"      $(\"#categorySelect\").navigateOnChange();\n            $(\'#SchoolCategory");


            
            #line 64 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
                          Write(safeName);

            
            #line default
            #line hidden
WriteLiteral("-EdFiGrid-interaction-custom\').append($(\'#buttonExportAll");


            
            #line 64 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
                                                                                              Write(safeName);

            
            #line default
            #line hidden
WriteLiteral("\'));\n            $(\'#buttonExportAll");


            
            #line 65 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
                           Write(safeName);

            
            #line default
            #line hidden
WriteLiteral("\').show();\n            $(\'#SchoolCategory");


            
            #line 66 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
                          Write(safeName);

            
            #line default
            #line hidden
WriteLiteral("-EdFiGrid\').bind(\'afterDrawEvent\', function(e, table) {\n                        S" +
"choolCategory");


            
            #line 67 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
                                  Write(safeName);

            
            #line default
            #line hidden
WriteLiteral("CustomStudentList.redrawCheckboxes();\n                        });\n        });\n   " +
" </script>\n\n");


            
            #line 72 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
      
        if (Model.GridData != null)
        {
            var callbackUrl = EdFiDashboardContext.Current.RoutedUrl;

            var edFiGridModelProvider = IoC.Resolve<IEdFiGridModelProvider>();

            var edfiGridModel = edFiGridModelProvider.GetEdFiGridModel(true, true, 0, "SchoolCategory" + safeName, 
                null, Model.GridData, null, null, true, "StudentList", new List<string> { "Default" }, null, true, callbackUrl, true,
                Model.GridData.WatchList, null, Model.Level, null, Model.PreviousNextSessionPage, Model.ExportGridDataUrl, Model.ListType);
            
            var customStudentListModel = new CustomStudentListModel
            {
                ControlId = "SchoolCategory" + safeName,
                IsCustomStudentList = false,
                LinkParentIdentifier = "#SchoolCategory" + safeName + "-EdFiGrid-interaction-custom",
                CheckboxParentIentifier = "#SchoolCategory" + safeName + "-EdFiGrid-fixed-data-table",
                SelectAllCheckboxParentIentifier = "#SchoolCategory" + safeName + "-EdFiGrid-fixed-header-table",
                CustomStudentListId = null,
                LocalEducationAgencyId = localEducationAgencyId,
                SchoolId = null,
                StaffUSI = staffUSI,
                CustomStudentListUrl = EdFiDashboards.Site.Staff.LocalEducationAgencyCustomStudentList(localEducationAgencyId, staffUSI, null, new { format = "json" }),
                UniqueId = localEducationAgencyId
            };


            
            #line default
            #line hidden
WriteLiteral("            <a id=\"buttonExportAll");


            
            #line 98 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
                              Write(safeName);

            
            #line default
            #line hidden
WriteLiteral("\" class=\"btn\" style=\"display: none;\" onclick=\"SchoolCategory");


            
            #line 98 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
                                                                                                     Write(safeName);

            
            #line default
            #line hidden
WriteLiteral("EdFiGrid.exportGrid(); \"><i class=\"icon-floppy\"></i> EXPORT ALL</a>\n");


            
            #line 99 "..\..\Areas\LocalEducationAgency\Views\StudentSchoolCategoryList\Get.cshtml"
            Html.RenderPartial("EdFiGrid", edfiGridModel);
            Html.RenderPartial("CustomStudentList", model: customStudentListModel);
        }
    

            
            #line default
            #line hidden

});

WriteLiteral("\n");


        }
    }
}
#pragma warning restore 1591