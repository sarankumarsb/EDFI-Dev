// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.IdentityModel.Tokens;
using System.Reflection;
using System.Security.Cryptography;
using System.ServiceModel.Security;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;
using Castle.Facilities.Logging;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Diagnostics.Helpers;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Infrastructure.CastleWindsorInstallers;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Infrastructure.log4net;
using EdFi.Dashboards.Presentation.Architecture.Mvc.ActionFilters;
using EdFi.Dashboards.Presentation.Architecture.Mvc.Core;
using EdFi.Dashboards.Resources.Security;
#if DEBUG
using EdFi.Dashboards.Presentation.Architecture.Mvc.Core.Development;
#endif
using EdFi.Dashboards.Presentation.Architecture.Mvc.ValueProviders;
using EdFi.Dashboards.Presentation.Core.Controllers;
using EdFi.Dashboards.Presentation.Core.Utilities.Mvc;
using EdFi.Dashboards.Resources.Application;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Web.Configuration;
using log4net;
using log4net.Config;
using Microsoft.IdentityModel.Web;
using EdFi.Dashboards.Infrastructure.Plugins.Helpers;

namespace EdFi.Dashboards.Presentation.Web
{
    public class Global : HttpApplication
    {
        private static ILog logger = LogManager.GetLogger(typeof(Global));
        
        protected void Application_Start(object sender, EventArgs e)
        {
            //Plugin architecture we use shadow copying so that it doesnt lock the plugin dlls.
            //This also enables the ability to drop in new dlls if needed and they will be picked up.
            AppDomain.CurrentDomain.SetShadowCopyPath(AppDomain.CurrentDomain.BaseDirectory + "; " + PluginHelper.GetPluginPath());
            AppDomain.CurrentDomain.SetShadowCopyFiles();

            // we need to make sure the EdFi.Dashboards.Infrastructure assembly is loaded before log4net tries to load
            var ignored = typeof(SmtpCustomAppender);

            //log4net Configuration.
            var log4netConfigFile = new FileInfo(Server.MapPath("~/log4net.config"));

            if (log4netConfigFile != null && log4netConfigFile.Exists)
            {
                XmlConfigurator.Configure(log4netConfigFile);
                LogManager.GetLogger("SiteUseLogger").Info("Application started.");
            }

            // Initialize Inversion of Control container
            InitializeInversionOfControl();

            //  MVC initialization
            InitializeMvc();

            FederatedAuthentication.ServiceConfigurationCreated += OnServiceConfigurationCreated;
        }

        private static void InitializeMvc()
        {
            var mvcInitializer = IoC.Resolve<IAspNetMvcFrameworkInitializer>();
            mvcInitializer.Initialize(IoC.Container, typeof(Marker_EdFi_Dashboards_Presentation_Web).Assembly);
        }

        private static void InitializeInversionOfControl()
        {
            var containerFactory = new InversionOfControlContainerFactory(
                new AppConfigSectionProvider(), 
                new AppConfigValueProvider());

            // Get the container
            var container = containerFactory.CreateContainer(c =>
                {
                    // Add the logging facility
                    c.AddFacility<LoggingFacility>(f => f
                        .LogUsing(LoggerImplementation.Log4net)
                        .WithConfig("log4net.xml"));

                    // Add the array resolver for resolving arrays of services automatically
                    c.Kernel.Resolver.AddSubResolver(new ArrayResolver(c.Kernel));

                    // Start logging IoC registrations
                    c.Kernel.HandlerRegistered += OnHandlerRegistered;

                    // Initialize the service locator with the container 
                    // (enabling installers to access container through IoC during registration process)
                    IoC.Initialize(c);
                });

            // Stop logging container registrations (they would continue to appear with generic types closed at runtime)
            container.Kernel.HandlerRegistered -= OnHandlerRegistered;

            // Register the wrapped service locator singleton instance
            container.Register(
                Component.For<IServiceLocator>()
                    .Instance(IoC.WrappedServiceLocator));
        }

        private static void OnHandlerRegistered(IHandler handler, ref bool changed)
        {
            logger.DebugFormat(@"
Service:         {0}
Implementation:  {1}
Lifestyle:       {2}
-------------------------------------------------------------------------------------------------------------------
", handler.GetComponentName(), handler.ComponentModel.Implementation.FullName, handler.ComponentModel.LifestyleType);
        }
        private static bool? _errorLoggingDisabled;

        private static bool ErrorLoggingDisabled
        {
            get
            {
                if (_errorLoggingDisabled == null)
                {
                    var configValueProvider = IoC.Resolve<IConfigValueProvider>();
                    _errorLoggingDisabled = Convert.ToBoolean(configValueProvider.GetValue("ErrorLoggingDisabled"));
                }

                return _errorLoggingDisabled.GetValueOrDefault();
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //Get reference to the source of the exception.
            var ex = sender as Exception;
            
            if (ex == null)
                ex = Server.GetLastError();

            if (RedirectToFileNotFound(ex))
                return;

            if (RedirectForCryptographicException(ex))
                return;

            // Uses appSetting of "ErrorLoggingDisabled = true" to turn off error logging (added for load testing troubleshooting).
            if (ErrorLoggingDisabled)
            {
                var links = IoC.Resolve<IErrorAreaLinks>();
                Response.Redirect(links.ErrorPage(string.Empty));
                return;
            }

            var userName = String.Empty;
            if (Context.User != null)
                userName = Context.User.Identity.Name;

            var errorLoggingProvider = IoC.Resolve<IErrorLoggingService>();
            var redirectUrl = errorLoggingProvider.Post(new ErrorLoggingRequest { Exception = ex, UserName = userName, Request = new HttpRequestWrapper(Request) });

            //Clear the last error.
            Server.ClearError();

            if (ex is SessionExpiredException)
            {
                RedirectWithLogin();
                return;
            }

            Response.Redirect(redirectUrl);
        }

        #region Unused Global events

        protected void Session_Start(object sender, EventArgs e)
        {            

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        #endregion

        // This fixes the problem of error ID1073 when attempting to decrypt a cookie issued on another server
        // http://consultingblogs.emc.com/simonevans/archive/2010/11/19/common-windows-identity-foundation-ws-federation-exceptions-explained.aspx
        protected void OnServiceConfigurationCreated(object sender, ServiceConfigurationCreatedEventArgs e)
        {

        }

        protected void WSFederationAuthenticationModule_RedirectingToIdentityProvider(object sender, RedirectingToIdentityProviderEventArgs e)
        {
            try
            {
                var localEducationAgencyContextProvider = IoC.Resolve<ILocalEducationAgencyContextProvider>();
                string code = localEducationAgencyContextProvider.GetCurrentLocalEducationAgencyCode();

                // Did we fail to determine the local education agency that is in context?
                if (code == null)
					throw new InvalidOperationException(
                        string.Format("Unable to determine local education agency context from URL '{0}' prior to redirecting to identity provider.", HttpContext.Current.Request.Url));

                var service = IoC.Resolve<IIdCodeService>();
                var lea = service.Get(new IdCodeRequest {Code = code});

                // Did we fail to determine the local education agency that is in context?
                if (lea == null)
					throw new InvalidOperationException(
                        string.Format("Unable to determine local education agency context from URL '{0}' prior to redirecting to identity provider.", HttpContext.Current.Request.Url));

                // VINLOGINOUT
                string idouserName = string.Empty, token = string.Empty;
                try
                {
                    if (HttpContext.Current.Items != null && (HttpContext.Current.Items.Contains("Rewriter.QUERY_STRING") && HttpContext.Current.Items["Rewriter.QUERY_STRING"] != null))
                    {
                        var querystringDtls = HttpContext.Current.Items["Rewriter.QUERY_STRING"].ToString().Split('&');

                        foreach (string querystrDtls in querystringDtls)
                        {
                            if (Convert.ToString(querystrDtls.Split('=')[0]) == "idofuser")
                                idouserName = Convert.ToString(querystrDtls.Split('=')[1]);
                            else if (Convert.ToString(querystrDtls.Split('=')[0]) == "token")
                                token = Convert.ToString(querystrDtls.Split('=')[1]);
                        }
                    }
                }
                catch { }

                var signInRequestProvider = IoC.Resolve<ISignInRequestMessageProvider>();
                var signInRequestAdornerModel = new SignInRequestAdornModel()
                {
                    LocalEducationAgencyId = lea.LocalEducationAgencyId,
                    LocalEducationAgencyCode = lea.Code,
                    LocalEducationAgencyName = lea.Name,
                    idofuser = idouserName,
                    idoftoken = token
                };
                signInRequestProvider.Adorn(e.SignInRequestMessage, signInRequestAdornerModel);
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                Application_Error(ex, null);
            }
        }

        private void WSFederationAuthenticationModule_SecurityTokenValidated(object sender, SecurityTokenValidatedEventArgs e)
        {
            FederatedAuthentication.SessionAuthenticationModule.IsSessionMode = true;
        }

        protected bool RedirectToFileNotFound(Exception ex)
        {
            var httpException = ex as HttpException;
            var controllerNotFound = ex as ControllerNotFoundException;
            var leaNotFound = ex as LocalEducationAgencyNotFoundException;
            if ((httpException != null && httpException.GetHttpCode() == 404) || controllerNotFound != null || leaNotFound != null)
            {
                string orgCode = null;

                try
                {
                    var leaProvider = IoC.Resolve<ILocalEducationAgencyContextProvider>();
                    orgCode = leaProvider.GetCurrentLocalEducationAgencyCode();
                }
                catch (InvalidOperationException exIO)
                {
                    // this is thrown if the LEA can't be determined
                    logger.Warn("Invalid Operation: ", exIO);
                }
                catch (LocalEducationAgencyNotFoundException exLEANF)
                {
                    // this is thrown if the LEA can't be determined
                    logger.Warn("Local Education Agency Not Found: ", exLEANF);
                }

                logger.Warn("Page not found: ", ex);

                // clear error on server
                Server.ClearError();
                string redirect = EdFiDashboards.Site.Error.NotFound(orgCode);
                Response.Redirect(redirect, false);
                return true;
            }
            return false;
        }

        protected bool RedirectForCryptographicException(Exception ex)
        {
            var invalidOperationException = ex as InvalidOperationException;
            var innerCryptographicException = ex.InnerException as CryptographicException;
            var cryptographicException = ex as CryptographicException;
            var securityTokenException = ex as SecurityTokenException;

            if ((invalidOperationException != null && innerCryptographicException != null) || cryptographicException != null || securityTokenException != null)
            {
                logger.Error("InvalidOperationException/CryptographicException has been thrown:", ex);

                // clear error on server
                Server.ClearError();
                return RedirectWithLogin();
            }

            return false;
        }

        protected bool RedirectWithLogin()
        {
            FederatedAuthentication.SessionAuthenticationModule.SignOut();

            var redirect = IoC.Resolve<ICurrentUrlProvider>().Url;

            var currentUser = System.Threading.Thread.CurrentPrincipal;
            if (currentUser != null && currentUser.Identity.IsAuthenticated)
            {
                // FederatedSignOut will send the redirect headers and start the Response
                WSFederationAuthenticationModule.FederatedSignOut(null, redirect);
            }

            Response.Redirect(redirect.ToString(), false);
            return true;
        }
    }
}
