// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Web.Mvc;

namespace EdFi.Dashboards.Presentation.Core.Areas.Search.Controllers
{
    public class SearchLayoutController : Controller //EdFi.Dashboards.Presentation.Web.Controllers.MasterPageController
    {
        public SearchLayoutController()
        {
        }

        [ChildActionOnly]
        public ContentResult Title(string subTitle)
        {
            //string title = GetEducationOrganizationBriefModel().Name + " - " + subTitle;
            //return Content(title);
            return Content("Search Page");
        }
    }
}
