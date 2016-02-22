using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace EdFi.Dashboards.Presentation.Web.Architecture.HtmlHelperExtensions
{
    public static class BrowserDetection
    {
        public static string GetBrowserSpecificCssClasses(this HtmlHelper html)
        {
            var css = new List<string>();
            var browser = html.ViewContext.HttpContext.Request.Browser;
            
            if (browser.Browser == "IE")
            {
                css.Add("ie");

                if(browser.MajorVersion == 10)
                    css.Add("ie10");
                if (browser.MajorVersion <= 10)
                    css.Add("lte10");
                if (browser.MajorVersion == 9)
                    css.Add("ie9");
                if (browser.MajorVersion <= 9)
                    css.Add("lte9");
                if (browser.MajorVersion == 8)
                    css.Add("ie8");
                if (browser.MajorVersion <= 8)
                    css.Add("lte8");
                if (browser.MajorVersion == 7)
                    css.Add("ie7");
                if (browser.MajorVersion <= 7)
                    css.Add("lte7");
            }

            return string.Join(" ", css);
        }
    }
}
