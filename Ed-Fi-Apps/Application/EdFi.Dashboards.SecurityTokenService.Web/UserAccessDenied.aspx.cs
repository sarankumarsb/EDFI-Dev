// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.SecurityTokenService.Web.Application.ViewModels;

namespace EdFi.Dashboards.SecurityTokenService.Web
{
    public partial class UserAccessDenied : System.Web.UI.Page
    {
        private ISessionStateProvider session;
        protected UserInformation UserInformation { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            session = IoC.Resolve<ISessionStateProvider>();
            this.UserInformation = UserInformation.Current;
        }

        protected string HomeCustomTags
        {
            get
            {
                var homePage = session[EdFiApp.Session.HomePage];
                if (homePage == null)
                    return "style='display:none;'";

                return "onclick=\"window.location='" + homePage + "';\"";
            }
        }
    }
}