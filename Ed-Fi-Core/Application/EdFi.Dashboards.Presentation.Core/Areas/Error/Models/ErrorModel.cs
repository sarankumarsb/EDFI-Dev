using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Presentation.Web.Areas.Error.Models
{
    public class ErrorModel
    {
        public bool ShowExceptionDetails { get; set; }
        public string ErrorMessage { get; set; }
        public UserInformation UserInformation { get; set; }
    }
}