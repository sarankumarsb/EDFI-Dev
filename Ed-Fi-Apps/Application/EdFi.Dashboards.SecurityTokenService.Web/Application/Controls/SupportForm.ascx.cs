// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Web;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using Microsoft.IdentityModel.Claims;

namespace EdFi.Dashboards.SecurityTokenService.Web.Controls
{
    public partial class SupportForm : System.Web.UI.UserControl
    {
        protected string UserName
        {
            get
            {
                var identity = Page.User.Identity as IClaimsIdentity;

                if (identity != null)
                    return identity.FullName();

                return string.Empty;
            }
        }

        protected string Email
        {
            get
            {
                var identity = Page.User.Identity as IClaimsIdentity;

                if (identity != null && !string.IsNullOrEmpty(identity.Email()))
                    return identity.Email();

                return String.Empty;
            }
        }

        //protected string DisableInputFields
        //{
        //    get
        //    {
        //        var identity = Page.User.Identity as IClaimsIdentity;

        //        if (identity != null && !AllowNameEdit)
        //            return "disabled='disabled'";

        //        return String.Empty;
        //    }
        //}

        protected string DisableFeedbackName
        {
            get
            {
                var identity = Page.User.Identity as IClaimsIdentity;

                if (identity != null && !AllowNameEdit)
                    return "disabled='disabled'";

                return string.Empty;
            }
        }

        protected string DisableFeedbackEmail
        {
            get
            {
                var identity = Page.User.Identity as IClaimsIdentity;

                if (identity != null && !string.IsNullOrEmpty(identity.Email()) && !AllowNameEdit)
                    return "disabled='disabled'";

                return string.Empty;
            }
        }

        public bool AllowNameEdit { get; set; }
        public string SupportLinkControlId { get; set; }
        public string SupportLinkControlId2 { get; set; }
    }
}