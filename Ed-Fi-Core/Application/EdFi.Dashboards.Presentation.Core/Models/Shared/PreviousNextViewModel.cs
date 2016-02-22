// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EdFi.Dashboards.Presentation.Core.Models.Shared
{
    public class PreviousNextViewModel
    {
        public string ListLink { get; set; }
        public string BackToListText { get; set; }
        public string PreviousEntityLink { get; set; }
        public string NextEntityLink { get; set; }
        public string PositionText { get; set; }

    }
}