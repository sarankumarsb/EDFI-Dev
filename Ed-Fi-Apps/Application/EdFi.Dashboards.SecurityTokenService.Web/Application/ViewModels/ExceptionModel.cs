// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;

namespace EdFi.Dashboards.SecurityTokenService.Web.Application.ViewModels
{
    public class ExceptionModel
    {
        public ExceptionModel(Exception ex)
        {
            Message = ex.Message;
            StackTrace = ex.ToString();
            Type = ex.GetType();

            if (ex.InnerException != null)
            {
                InnerExceptionMessage = ex.InnerException.Message;
                InnerExceptionStackTrace = ex.InnerException.ToString();
            }
        }

        public string Message { get; set; }
        public string StackTrace { get; set; }
        public Type Type { get; set; }
        public string InnerExceptionMessage { get; set; }
        public string InnerExceptionStackTrace { get; set; }
    }
}