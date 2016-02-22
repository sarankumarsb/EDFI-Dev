// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Reflection;

namespace EdFi.Dashboards.Resources.Navigation.Mvc.Areas
{
    public class Common : ICommonLinks
    {
        //public virtual string Default()
        //{
        //    return EdFiWebFormsDashboards.Site.SiteAppUrl + "Default.aspx"; 
        //}

        //public virtual string Default(string returnUrl)
        //{
        //    return Default() + EdFiWebFormsDashboards.Site.BuildUrlParameters(MethodBase.GetCurrentMethod(), returnUrl); 
        //}

        //public virtual string Image(string image, string imageType)
        //{
        //    return EdFiWebFormsDashboards.Site.SiteAppUrl + "Common/Image.axd" + EdFiWebFormsDashboards.Site.BuildUrlParameters(MethodBase.GetCurrentMethod(), image, imageType);
        //}

        //public virtual string Image(string image, string display, string imageType)
        //{
        //    return EdFiWebFormsDashboards.Site.SiteAppUrl + "Common/Image.axd" + EdFiWebFormsDashboards.Site.BuildUrlParameters(MethodBase.GetCurrentMethod(), image, display, imageType);
        //}

        //public virtual string ApplicationImage(string image)
        //{
        //    return EdFiWebFormsDashboards.Site.RootImages + image;
        //}

        public virtual string ThemeImage(string image)
        {
            return EdFiWebFormsDashboards.Site.ContentImages + image;
        }

        //public virtual string ThemeResource(string fileName)
        //{
        //    return EdFiWebFormsDashboards.Site.Theme + fileName;
        //}

        public virtual string Content(string fileName)
        {
            return EdFiWebFormsDashboards.Site.ContentUrl + fileName;
        }

        public virtual string Feedback()
        {
            return "~/Common/Feedback";
        }

        public virtual string EdFiGridWebService_SetSessionObject(int localEducationAgencyId)
        {
            return "~/Common/ListSortContext";
        }
    }
}
