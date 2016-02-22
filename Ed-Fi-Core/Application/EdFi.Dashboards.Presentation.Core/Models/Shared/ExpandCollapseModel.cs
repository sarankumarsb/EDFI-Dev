// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EdFi.Dashboards.Presentation.Core.Models.Shared
{
    public class ExpandCollapseModel
    {
        public InitialState CollapseExpandInitialState { get; set; }
        public string Tooltip { get; set; }
        public string Title { get; set; }
        public string TitleCSS { get; set; }
        public string DivId { get; set; }

        public enum InitialState
        {
            Collapsed,
            Expanded
        }
    }
}