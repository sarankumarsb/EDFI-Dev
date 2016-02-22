using System;
using System.Net;
using System.Web.Mvc;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Presentation.Web.Areas.Error.Models;
using EdFi.Dashboards.Resources.Models.Application;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Presentation.Core.Areas.Error.Controllers
{
    public class ErrorController : Controller
    {
        private ISessionStateProvider session;

        public ActionResult Get()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            session = IoC.Resolve<ISessionStateProvider>();

            var errorModel = new ErrorModel();

            errorModel.UserInformation = UserInformation.Current;

            errorModel.ShowExceptionDetails = ShowExceptionDetails;
            if (errorModel.ShowExceptionDetails)
            {
                var ex = session[EdFiApp.Session.LastException] as ExceptionModel;

                errorModel.ErrorMessage =
                    ex == null ?
                    "No exception details available."
                    : ex.StackTrace;//.Replace("\n", "<br/>");
            }

            return View(errorModel);
        }


        protected bool ShowExceptionDetails
        {
            get
            {
                bool showExceptionDetails;
                var configValueProvider = IoC.Resolve<IConfigValueProvider>();

                try
                {
                    showExceptionDetails = Convert.ToBoolean(configValueProvider.GetValue("showExceptionDetails"));
                }
                catch
                {
                    // Fail to not showing the exception details
                    showExceptionDetails = false;
                }

                return showExceptionDetails;
            }
        }
        
        //HeadEXCEPTION
        //HeadFEEDBACK
    }
}
