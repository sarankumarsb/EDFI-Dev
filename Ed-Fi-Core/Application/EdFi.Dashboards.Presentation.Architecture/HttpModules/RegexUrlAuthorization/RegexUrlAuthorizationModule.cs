// *************************************************************************
// Home page:   http://www.talifun.com/products/talifun-web/
// Github:      https://github.com/taliesins/talifun-web
// License:     Apache 2.0
// *************************************************************************
using System;
using System.Reflection;
using System.Web;
using Microsoft.IdentityModel.Web;

namespace Talifun.Web.RegexUrlAuthorization
{
    /// <summary>
    /// Module that is used to provide authorization on urls, that match regular expressions, based on a configuration provided.
    /// </summary>
    /// <remarks>
    /// It uses the asp.net "authorization" web.config rules, to check if a user is authorized. 
    /// </remarks>
    public class RegexUrlAuthorizationModule : HttpModuleBase
    {
        /// <summary>
        /// We want to initialize the ip address authentication manager.
        /// </summary>
        private static readonly RegexUrlAuthorizationManager RegexUrlAuthorizationManager = RegexUrlAuthorizationManager.Instance;

        private static readonly MethodInfo GetErrorTextMethod = typeof(System.Web.Configuration.UrlMapping).Assembly.GetType("System.Web.Configuration.UrlAuthFailedErrorFormatter").GetMethod("GetErrorText", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { }, null);
        private static readonly MethodInfo GenerateResponseHeadersForHandlerMethod = typeof(HttpResponse).GetMethod("GenerateResponseHeadersForHandler", BindingFlags.Instance | BindingFlags.NonPublic);

        protected override void InitializeModule(HttpApplication context)
        {
            context.AuthorizeRequest += new EventHandler(OnEnter);
        }

        private static void OnEnter(object source, EventArgs eventArgs)
        {
            var application = (HttpApplication)source;
            var context = application.Context;
            if (context.SkipAuthorization) return;
            
            //Allow for different deployment locations. Remove tilde for matches.
            //AppRelativeCurrentExecutionFilePath - MSDN: Use this property to provide URL information that will stay the same even if the application changes location. This allows the same URL-mapping code to be used in a test environment and in the final deployment environment, or to be used by copies of Web applications in different domains.
            var rawUrl = context.Request.AppRelativeCurrentExecutionFilePath.Replace("~", "");

            var user = context.User;
            var requestType = context.Request.RequestType;

            if (RegexUrlAuthorizationManager.IsAuthorized(rawUrl, user, requestType))
            {
                return;
            }

            context.Response.StatusCode = 401;
            WriteErrorMessage(context);
            application.CompleteRequest();
        }

        private static void WriteErrorMessage(HttpContext context)
        {
            //context.Response.Write(UrlAuthFailedErrorFormatter.GetErrorText());
            //context.Response.GenerateResponseHeadersForHandler();

            context.Response.Write(GetErrorTextMethod.Invoke(null, null));
            GenerateResponseHeadersForHandlerMethod.Invoke(context.Response, null);
        }

        ///// <summary>
        ///// Determines whether the module will be registered for discovery
        ///// in partial trust environments or not.
        ///// </summary>
        //protected override bool SupportDiscoverability
        //{
        //    get { return true; }
        //}
    }
}
