// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Web;
using EdFi.Dashboards.SecurityTokenService.Web.Mvp.BaseArchitecture;
using EdFi.Dashboards.SecurityTokenService.Web.Mvp.Models;
using EdFi.Dashboards.SecurityTokenService.Web.Mvp.Presenters;

namespace EdFi.Dashboards.SecurityTokenService.Web
{
	public partial class Login : MvpPageBase<LoginModel, ILoginView, LoginPresenter>, ILoginView
	{
        private const string EDFI_USER_NAME = "EdFiUserName";
    
		protected void Page_Load(object sender, EventArgs e)
		{
            if (IsPostBack)
            {
                string username = Request.Form["inputUsername"];
                string password = Request.Form["inputPassword"];

                // save username into cookie, if needed
                if (checkRememberMe.Checked)
                {
                    SaveUserName(username);
                }
                Session["ServiceType"] = "Normal"; // VINLOGINTYP			
                Presenter.AuthenticateAndRedirect(username, password, Request["ReturnUrl"], Convert.ToString(Session["ServiceType"]));	// EDFIDB-56
            }
            else
            {
                if (!String.IsNullOrEmpty(Request.QueryString["idofuser"]))
                {
                    //Request.Form["idofuser"];
                    string idofuser = Request.QueryString["idofuser"];
                    string idoftoken = Request.QueryString["idoftoken"];
                    string username = idofuser;// Request.Form["inputUsername"];
                    string password = idoftoken;// Request.Form["inputPassword"];

                    // save username into cookie, if needed
                    if (checkRememberMe.Checked)
                    {
                        SaveUserName(username);
                    }
                    Session["ServiceType"] = "Moodel"; // VINLOGINTYP			                    
                    Presenter.AuthenticateAndRedirect(username, password, Request["ReturnUrl"], Convert.ToString(Session["ServiceType"])); // EDFIDB-56
                }
                checkRememberMe.Checked = !string.IsNullOrWhiteSpace(Username);
            }
		}

	    public void Redirect(string url)
		{
			Response.Redirect(url, false);
		}

		public void ShowLoginMessage(string message)
		{
			ErrorMessageLabel.Visible = true;
			ErrorMessageLabel.Text = message;
		}

		public void HideLoginMessage()
		{
			ErrorMessageLabel.Visible = false;
		}

	    public string Username
	    {
	        get
	        {
                if (Model == null || string.IsNullOrWhiteSpace(Model.Username))
                {
                    // check cookies for saved username
                    var cookie = Page.Request.Cookies[EDFI_USER_NAME];
                    return cookie == null ? string.Empty : cookie.Value;
                }
	            return Model.Username;
	        }
	    }

        public override void PerformDataBinding() {}

        private void SaveUserName(string username)
        {
            var cookie = Page.Request.Cookies[EDFI_USER_NAME] ?? new HttpCookie(EDFI_USER_NAME);

            cookie.Value = username;
            cookie.Expires = DateTime.Now.AddYears(99);

            Page.Response.Cookies.Set(cookie);
        }
	}
}
