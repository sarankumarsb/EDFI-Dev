// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EdFi.Dashboards.Presentation.Core.Models.Shared
{
    public class FeedbackModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string SupportLinkControlId { get; set; }
        //public string DisableInputFields { get; set; }
        public string DisableFeedbackName { get; set; }
        public string DisableFeedbackEmail { get; set; }
        public bool AllowNameEdit { get; set; }
        public int LocalEducationAgencyId { get; set; }
    }
}