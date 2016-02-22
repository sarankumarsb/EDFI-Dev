// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Reflection;

namespace EdFi.Dashboards.Resources.Navigation.WebForms.Areas
{
    public class Common
    {
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification="Breaking change to mark static as per MSDN ok to suppress")]
		public string Default()
        {
            return EdFiWebFormsDashboards.Site.ContentUrl + "Default.aspx"; 
        }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Breaking change to mark static as per MSDN ok to suppress")]
        public string Default(string returnUrl)
        {
            return Default() + EdFiWebFormsDashboards.Site.BuildUrlParameters(MethodBase.GetCurrentMethod(), returnUrl); 
        }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Breaking change to mark static as per MSDN ok to suppress")]
		public string ThemeImage(string image)
        {
            return EdFiWebFormsDashboards.Site.ContentImages + image;
        }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Breaking change to mark static as per MSDN ok to suppress")]
        public string Feedback()
        {
            return "~/FeedbackHandler.ashx";
        }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Breaking change to mark static as per MSDN ok to suppress")]
        public string EdFiGridWebService_SetSessionObject(int localEducationAgencyId)
        {
            return "~/Common/ListSortContext";
        }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Breaking change to mark static as per MSDN ok to suppress")]
        public string Logout()
        {
            return "~/Logout";
        }
    }
}
