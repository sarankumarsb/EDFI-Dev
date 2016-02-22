// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.ActionResults
{
    public class JsonDotNetResult : ActionResult
    {
        // Methods
        public JsonDotNetResult()
        {
            this.JsonRequestBehavior = JsonRequestBehavior.DenyGet;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if ((this.JsonRequestBehavior == JsonRequestBehavior.DenyGet) 
                && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(); //MvcResources.JsonRequest_GetNotAllowed);
            }

            HttpResponseBase response = context.HttpContext.Response;

            if (!string.IsNullOrEmpty(this.ContentType))
                response.ContentType = this.ContentType;
            else
                response.ContentType = "application/json";

            if (this.ContentEncoding != null)
                response.ContentEncoding = this.ContentEncoding;

            if (this.Data != null)
            {
                response.Write(
#if DEBUG                    
                    // Format JSON for readability (during development)
                    JsonConvert.SerializeObject(this.Data, Formatting.Indented)
#else
                    // Format JSON for size (when deployed)
                    JsonConvert.SerializeObject(this.Data)
#endif
                );
            }

            if (StatusCode != null)
                response.StatusCode = (int) StatusCode.Value;

            if (!string.IsNullOrEmpty(StatusDescription))
                response.StatusDescription = StatusDescription;
        }

        // Properties
        public Encoding ContentEncoding { get; set; }

        public string ContentType { get; set; }

        public object Data { get; set; }

        public JsonRequestBehavior JsonRequestBehavior { get; set; }

        public int? StatusCode { get; set; }
        public string StatusDescription { get; set; } 
    }
}