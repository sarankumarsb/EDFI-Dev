using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EdFi.Dashboards.Resources.Navigation;

namespace EdFi.Dashboards.Presentation.Web.Architecture.HtmlHelperExtensions
{

    public static partial class Html
    {
        public static IHtmlString Flag(this HtmlHelper html, bool isFlagged, string tooltip = "")
        {
            var val = "<img src=\""
                      + EdFiWebFormsDashboards.Site.Common.ThemeImage("FlagRed.gif").Resolve()
                      + "\" alt=\"Alert\" title=\"" + tooltip
                      + "\" style=\"" + ((!isFlagged) ? "display:none;" : String.Empty) + "\" />";

            return new MvcHtmlString(val);
        }
    }
}