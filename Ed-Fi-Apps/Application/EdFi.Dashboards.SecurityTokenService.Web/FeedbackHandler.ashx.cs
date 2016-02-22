// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Web;
using System.Web.Services;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Resources.Application;

namespace EdFi.Dashboards.SecurityTokenService.Web
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class FeedbackHandler : IHttpHandler
	{
        public void ProcessRequest(HttpContext context)
        {
            string name = String.Empty;
            try
            {
                name = context.Request["name"];

                var supportLoggingProvider = IoC.Resolve<IFeedbackService>();
                supportLoggingProvider.Post(new FeedbackRequest()
                                                {
                                                    LocalEducationAgency = context.Request["lea"],
                                                    Name = name,
                                                    Email = context.Request["email"],
                                                    Subject = context.Request["subject"],
                                                    Issue = context.Request["issue"],
                                                    PhoneNumber = context.Request["phoneNumber"],
                                                    Feedback = context.Request["feedback"]
                                                });

                context.Response.ContentType = "text/plain";
                context.Response.Write("Thank you for your feedback!");
            }
            catch (Exception ex)
            {
                var errorLoggingProvider = IoC.Resolve<IErrorLoggingService>();
                errorLoggingProvider.Post(new ErrorLoggingRequest { Exception = ex, UserName = name, Request = new HttpRequestWrapper(context.Request) });
            }
        }

		public bool IsReusable
		{
			get
			{
				return true;
			}
		}
	}
}
