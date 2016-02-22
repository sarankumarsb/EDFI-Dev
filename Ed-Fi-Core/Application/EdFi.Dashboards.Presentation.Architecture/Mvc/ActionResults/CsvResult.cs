// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Infrastructure;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.ActionResults
{
    public class CsvResult : ActionResult
    {
        private readonly ICsvSerializable model;

        public CsvResult(ICsvSerializable model)
        {
            this.model = model;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;

            var sb = new StringBuilder();

            foreach (var v in context.RouteData.Values)
            {
                if (v.Key != "action")
                    sb.Append(v.Value + "-");
            }

            string fileName = String.Format("{0}{1}.csv", sb, DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
            response.ContentType = "text/csv";
            response.AddHeader("content-disposition", "attachment; filename=" + fileName);

            var serializer = IoC.Resolve<ICsvSerializer>();
            response.Write(serializer.Serialize(model));
        }
    }
}