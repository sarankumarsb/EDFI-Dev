// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Web;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Application;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using Microsoft.IdentityModel.Protocols.WSFederation;
using Microsoft.IdentityModel.Web;

namespace EdFi.Dashboards.SecurityTokenService.Web
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_PreRender(object sender, EventArgs e)
        {
            string action = Request.QueryString[WSFederationConstants.Parameters.Action];

            try
            {
                if (action == WSFederationConstants.Actions.SignIn)
                {
                    var requestMessage = (SignInRequestMessage)WSFederationMessage.CreateFromUri(Request.Url);
                    // need to get the LEA home page URL out of the request message in case the user does not have any claims

                    // Process sign in request.
                    try
                    {
                        if (User != null && User.Identity.IsAuthenticated)
                        {
                            Microsoft.IdentityModel.SecurityTokenService.SecurityTokenService sts = new CustomSecurityTokenService(CustomSecurityTokenServiceConfiguration.Current);
                            SignInResponseMessage responseMessage =
                                FederatedPassiveSecurityTokenServiceOperations.ProcessSignInRequest(requestMessage, User, sts);
                            FederatedPassiveSecurityTokenServiceOperations.ProcessSignInResponse(responseMessage, Response);
                        }
                        else
                        {
                            // append the LEA home page
                            Response.Redirect("UserAccessDenied.aspx", true);
                        }
                    }
                    catch (DashboardsAuthenticationException dae)
                    {
                        RedirectToUserAccessDenied(dae);
                    }
                    catch (UserAccessDeniedException ex)
                    {
                        RedirectToUserAccessDenied(ex);
                    }
                }
                else if (action == WSFederationConstants.Actions.SignOut)
                {
                    // Process sign out request.
                    var requestMessage = (SignOutRequestMessage)WSFederationMessage.CreateFromUri(Request.Url);
                    FederatedPassiveSecurityTokenServiceOperations.ProcessSignOutRequest(requestMessage, User, requestMessage.Reply, Response);
                }
                else
                {
                    throw new InvalidOperationException(
                        String.Format(CultureInfo.InvariantCulture,
                                      "The action '{0}' (Request.QueryString['{1}']) is unexpected. Expected actions are: '{2}' or '{3}'.",
                                      String.IsNullOrEmpty(action) ? "<EMPTY>" : action,
                                      WSFederationConstants.Parameters.Action,
                                      WSFederationConstants.Actions.SignIn,
                                      WSFederationConstants.Actions.SignOut));
                }
            }
            catch (ThreadAbortException)
            {
                // [System.Threading.ThreadAbortException] = {Unable to evaluate expression because the code is optimized or a native frame is on top of the call stack.}
                // This appears to be happening because of a Response.Redirect being invoked by the FederatedPassiveSecurityTokenServiceOperations.ProcessSignInRequest call,
                // causing the subsequent call to ProcessSignInResponse to fail. However, the token is issued correctly and the redirect occurs, so we have decided to place 
                // a low priority on resolving this.
            }            
        }

        private void RedirectToUserAccessDenied(Exception ex)
        {
            var dashboardsAuthenticationException = ex as DashboardsAuthenticationException;
            if (dashboardsAuthenticationException != null && !dashboardsAuthenticationException.IsImpersonating)
            {
                var feedbackService = IoC.Resolve<IFeedbackService>();
                feedbackService.Post(new FeedbackRequest
                                            {
                                                LocalEducationAgency = HttpContext.Current.Request["lea"],
                                                Name = dashboardsAuthenticationException.Name,
                                                //Email = dae.Email,
                                                StaffUSI = dashboardsAuthenticationException.StaffUSI.ToString(),
                                                Subject = "User authenticated yet not issued claims.",
                                                Issue = "Question",
                                                Feedback = dashboardsAuthenticationException.ToErrorLogText()
                                            });
            }
            else if (dashboardsAuthenticationException == null)
            {
                var username = User != null ? User.Identity.Name : "unknown";
                var errorLoggingProvider = IoC.Resolve<IErrorLoggingService>();
                errorLoggingProvider.Post(new ErrorLoggingRequest { Exception = ex, UserName = username, Request = new HttpRequestWrapper(Request) }); 
            }

            var requestUrlParams = WSFederationMessage.ParseQueryString(Request.Url);
            var baseUri = WSFederationMessage.GetBaseUrl(Request.Url);
            requestUrlParams[WSFederationConstants.Parameters.Action] = WSFederationConstants.Actions.SignOut;
            var signoutMessage = (SignOutRequestMessage) WSFederationMessage.CreateFromNameValueCollection(baseUri, requestUrlParams);
            FederatedPassiveSecurityTokenServiceOperations.ProcessSignOutRequest(signoutMessage, User, signoutMessage.Reply, Response);
            // append the LEA home page
            var session = IoC.Resolve<ISessionStateProvider>();
            session[EdFiApp.Session.HomePage] = requestUrlParams["home"];
            Response.Redirect("UserAccessDenied.aspx", true);
        }
    }
}