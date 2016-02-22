// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.IO;
using System.Web;
using EdFi.Dashboards.Infrastructure.CastleWindsorInstallers;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Resources.Application;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Infrastructure.log4net;
using EdFi.Dashboards.Resources.Navigation;
using log4net.Config;
using Microsoft.IdentityModel.Web;
using log4net;

namespace EdFi.Dashboards.SecurityTokenService.Web
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // we need to make sure the EdFi.Dashboards.Infrastructure assembly is loaded before log4net tries to load
            var type = typeof(SmtpCustomAppender);

            //log4net Configuration.
            var fileInfo = new FileInfo(Server.MapPath("~/log4net.config"));

            if (fileInfo.Exists)
            {
                XmlConfigurator.Configure(fileInfo);
                LogManager.GetLogger("SiteUseLogger").Info("Application started.");
            }

            // Initialize Inversion of Control container
            InitializeInversionOfControl();
        }

        private static void InitializeInversionOfControl()
        {
            var containerFactory = new InversionOfControlContainerFactory(
                new AppConfigSectionProvider(),
                new AppConfigValueProvider());

            IoC.Initialize(containerFactory.CreateContainer());
        }

        #region Unused global ASP.NET events
        
        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        #endregion

        protected void Application_Error(object sender, EventArgs e)
        {
            //Get reference to the source of the exception.
            Exception ex = Server.GetLastError();

            var userName = String.Empty;
            if (Context.User != null)
                userName = Context.User.Identity.Name;

            var errorLoggingProvider = IoC.Resolve<IErrorLoggingService>();
            var redirectUrl = errorLoggingProvider.Post(new ErrorLoggingRequest { Exception = ex, UserName = userName, Request = new HttpRequestWrapper(Request) });

            //Clear the last error.
            Server.ClearError();

            if (ex is SessionExpiredException)
            {
                FederatedAuthentication.SessionAuthenticationModule.SignOut();
                Response.Redirect(EdFiWebFormsDashboards.Site.Common.Default(Server.UrlEncode(IoC.Resolve<ICurrentUrlProvider>().Url.ToString())).Resolve());
            }
            else
                Response.Redirect(redirectUrl, false);
        }
    }
}