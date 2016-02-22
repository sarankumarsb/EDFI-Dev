using System;
using System.Web.Mvc;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Navigation;
using Microsoft.IdentityModel.Web;
using log4net;

namespace EdFi.Dashboards.Presentation.Core.Controllers
{
    public class LogoutController : Controller
    {
        private static readonly ILog siteUseLog = LogManager.GetLogger("SiteUseLogger");

        public ActionResult Get()
        {
            var userName = String.Empty;
            var currentUser = System.Threading.Thread.CurrentPrincipal;
            if (currentUser != null)
                userName = currentUser.Identity.Name;

            siteUseLog.Info(String.Format("{0} logged out.", userName));

            string idouserName = string.Empty, returnURL = string.Empty, token = string.Empty;
            try
            {
                if (HttpContext.Items != null && (HttpContext.Items.Contains("Rewriter.QUERY_STRING") && HttpContext.Items["Rewriter.QUERY_STRING"] != null))
                {
                    var querystringDtls = HttpContext.Items["Rewriter.QUERY_STRING"].ToString().Split('&');

                    foreach (string querystrDtls in querystringDtls)
                    {
                        if (Convert.ToString(querystrDtls.Split('=')[0]) == "idofuser")
                            idouserName = Convert.ToString(querystrDtls.Split('=')[1]);
                        else if (Convert.ToString(querystrDtls.Split('=')[0]) == "returnURL")
                            returnURL = Convert.ToString(querystrDtls.Split('=')[1]);
                        else if (Convert.ToString(querystrDtls.Split('=')[0]) == "token")
                            token = Convert.ToString(querystrDtls.Split('=')[1]);
                    }
                }
            }
            catch { }

            var redirect = ("~/").Resolve();
            if (idouserName != userName)
            {
                var session = IoC.Resolve<ISessionStateProvider>();
                session.Clear();
                FederatedAuthentication.SessionAuthenticationModule.SignOut();
            }

            FederatedAuthentication.SessionAuthenticationModule.SignOut();

            if (!string.IsNullOrEmpty(idouserName))
            {
                redirect = redirect + returnURL + @"?idofuser=" + idouserName + "&token=" + token;  //("~/").Resolve();
            }

            if (currentUser != null && currentUser.Identity.IsAuthenticated)
            {
                // FederatedSignOut will send the redirect headers and start the Response
                WSFederationAuthenticationModule.FederatedSignOut(null, new Uri(redirect));
            }

            return Response.IsRequestBeingRedirected ? null : Redirect(redirect);
        }
    }
}
