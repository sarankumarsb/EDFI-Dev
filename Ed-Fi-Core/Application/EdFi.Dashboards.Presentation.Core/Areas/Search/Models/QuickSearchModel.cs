// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EdFi.Dashboards.Presentation.Web.Areas.Search.Models
{
    public class QuickSearchModel
    {
        public int? LocalEducationAgencyId { get; set; }
        public bool CanSearch { get; set; }
    }
}