// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Web;
using System.Web.Security;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Resources.Application;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.SecurityTokenService.Web.Mvp.BaseArchitecture;
using EdFi.Dashboards.SecurityTokenService.Web.Mvp.Models;
using Microsoft.IdentityModel.Protocols.WSFederation;
using log4net;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Data.Entities;

namespace EdFi.Dashboards.SecurityTokenService.Web.Mvp.Presenters
{
    public interface ILoginView : IViewBase<LoginModel>
    {
        void Redirect(string url);
        void ShowLoginMessage(string message);
        void HideLoginMessage();
    }

    public class LoginPresenter : PresenterBase<LoginModel, ILoginView>
    {
        private static readonly ILog siteUseLog = LogManager.GetLogger("SiteUseLogger");
        //private readonly IAuthenticationProvider authenticationProvider;
        private IAuthenticationProvider authenticationProvider;
        //private readonly IRepository<LoginDetails> _LoginInfoRepository;

        public static class Messages
        {
            public const string GeneralError = "There was an error processing your login.";
            public const string UserNameOrPasswordIncorrect = "The username or password is incorrect.";
        }

        public LoginPresenter(IAuthenticationProvider authenticationProvider, IRepository<LoginDetails> LoginInfoRepo)
        {
            //			this.authenticationProvider = authenticationProvider;      
            //this._LoginInfoRepository = LoginInfoRepo;
        }

        public void Initialize(string wa, string wreply)
        {
            // this is here b/c if the user attempts to sign out after the session is expired
            // it will ask him to log in so that the system can log him out.
            if (wa == WSFederationConstants.Actions.SignOut)
                View.Redirect(wreply);

            View.Model = new LoginModel();
        }

        public void AuthenticateAndRedirect(string username, string password, string returnUrl, string serviceType)
        {
            try
            {
                View.HideLoginMessage();
                var authorizeUserName = username;
                if (username.Contains(":"))
                {
                    var position = username.IndexOf(':');
                    authorizeUserName = username.Substring(position + 1);
                    username = username.Substring(0, position);
                }

                // EDFIDB-56
                bool isAuthReq = Convert.ToBoolean(System.Web.Configuration.WebConfigurationManager.AppSettings["AuthReq"]);
                if (isAuthReq && serviceType == "Normal")
                    authenticationProvider = new EdFi.Dashboards.Resources.Security.Implementations.QuadraLMSAuthenticationProvider(); //IoC.Resolve<IAuthenticationProvider>();                 
                else if (serviceType != "Normal")
                    authenticationProvider = new EdFi.Dashboards.Resources.Security.Implementations.QuadraLMSTokenAuthenticationProvider();
                else
                    authenticationProvider = new EdFi.Dashboards.Resources.Security.Implementations.AlwaysValidAuthenticationProvider();

                // VIN25112015
                IStaffInformationProvider _staffInformationRepository;
                if (!authenticationProvider.ValidateUser(username, password))
                {
                    View.ShowLoginMessage(Messages.UserNameOrPasswordIncorrect);
                    return;
                }
                else
                {
                    // Fix : EDFIDB-136 Resolve User details in Dashboard database.                     
                    _staffInformationRepository = IoC.Resolve<IStaffInformationProvider>();
                    if (serviceType != "Normal")
                    {
                        siteUseLog.Info(String.Format("{0} username retrived from QIIS Service.", authenticationProvider.UserName));
                        username = authenticationProvider.UserName;
                    }
                    long staffUSI = _staffInformationRepository.ResolveStaffUSI(authenticationProvider, username);
                    int userType = _staffInformationRepository.UserType; // VIN22012016
                    if (staffUSI == 0)
                    {
                        View.ShowLoginMessage(Messages.UserNameOrPasswordIncorrect);
                        return;
                    }

                    //if (serviceType != "Normal")
                    //{
                    //    authorizeUserName = _staffInformationRepository.ResolveUsername(authenticationProvider, Convert.ToString(staffUSI), userType); // VIN22012016
                    //}
                }
                siteUseLog.Info(String.Format("{0} logged in.", authorizeUserName));
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    FormsAuthentication.RedirectFromLoginPage(authorizeUserName, false);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(authorizeUserName, false);
                    View.Redirect("default.aspx");
                }
            }
            catch (Exception ex)
            {
                var errorLoggingProvider = IoC.Resolve<IErrorLoggingService>();
                errorLoggingProvider.Post(new ErrorLoggingRequest { Exception = ex, UserName = username, Request = new HttpRequestWrapper(HttpContext.Current.Request) });

                View.ShowLoginMessage(ex.Message); //Messages.GeneralError);
            }
        }
    }
}
