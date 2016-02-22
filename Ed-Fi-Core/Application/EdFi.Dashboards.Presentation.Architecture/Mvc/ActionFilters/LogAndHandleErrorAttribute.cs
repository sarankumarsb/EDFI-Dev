// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Text;
using System.Web.Mvc;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using log4net;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.ActionFilters
{
    public class LogAndHandleErrorAttribute : HandleErrorAttribute
    {
        // right now this is used for SecurityAccessDeniedExceptions which we don't want being sent to JIRA.
        // if you change this logger, please be aware of the consequences.
        private static ILog logger = LogManager.GetLogger(typeof (LogAndHandleErrorAttribute));

        public override void OnException(ExceptionContext filterContext)
        {
            var sb = new StringBuilder();
            if (UserInformation.Current != null)
                sb.AppendFormat("Staff USI: '{0}'  Name: '{1}'", UserInformation.Current.StaffUSI, UserInformation.Current.FullName);
            sb.AppendLine();
            sb.AppendLine(filterContext.RequestContext.HttpContext.Request.Url.ToString());
            logger.Error(sb.ToString(), filterContext.Exception);

            base.OnException(filterContext);
        }
    }
}
