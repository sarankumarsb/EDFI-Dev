// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Web;
using System.Web.Mvc;
using EdFi.Dashboards.Resources.Models.Common;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.ActionResults
{
    public class ImageResult : ActionResult
    {
        private const string displayParameterName = "DisplayType";

        private readonly ImageModel model;

        public ImageResult(ImageModel model)
        {
            this.model = model;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (model == null)
                return;

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = model.ContentType;
            response.OutputStream.Write(model.Bytes, 0, model.Bytes.Length);
            response.Cache.SetMaxAge(TimeSpan.FromDays(1));
            response.Cache.SetCacheability(HttpCacheability.Public);
            response.Cache.VaryByParams[displayParameterName] = true;
        }
    }
}