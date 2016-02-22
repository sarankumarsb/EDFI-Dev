// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Web.Mvc;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.ActionResults
{
    public enum HttpStatusCode
    {
        OK = 200,
        Created = 201,
        NoContent = 204,
        BadRequest = 400,
    }

    public class ViewWithStatusCodeResult : ViewResult
    {
        public int StatusCode { get; private set; }
        public string StatusDescription { get; private set; }

        public ViewWithStatusCodeResult(int statusCode, string statusDescription = null)
            : this(null, statusCode, statusDescription) { }

        public ViewWithStatusCodeResult(string viewName, int statusCode, string statusDescription = null)
        {
            StatusCode = statusCode;
            StatusDescription = statusDescription;
            ViewName = viewName;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var httpContext = context.HttpContext;
            var response = httpContext.Response;

            response.StatusCode = StatusCode;
            response.StatusDescription = StatusDescription;

            base.ExecuteResult(context);
        }
    }
}