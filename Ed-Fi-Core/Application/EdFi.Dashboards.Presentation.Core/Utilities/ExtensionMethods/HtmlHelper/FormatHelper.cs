// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Text;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Navigation;

namespace EdFi.Dashboards.Presentation.Core.Utilities.ExtensionMethods.HtmlHelper
{
    public static class FormatHelper
    {
        public static string FormatPercentage(this System.Web.Mvc.HtmlHelper html, decimal? value)
        {
            if (!value.HasValue)
                return String.Empty;
            return FormatPercentage(html, value.Value);
        }

        public static string FormatPercentage(this System.Web.Mvc.HtmlHelper html, decimal value)
        {
            return String.Format("{0:0.0%}", value);
        }

        public static string FormatPercentage(this System.Web.Mvc.HtmlHelper html, object value)
        {
            return FormatPercentage(html, Convert.ToDecimal(value));
        }

        public static string FormatCredits(this System.Web.Mvc.HtmlHelper html, decimal? credits)
        {
            if (credits.HasValue)
                return String.Format("{0:##.##}", credits);
            return "0";
        }

        public static string FormatPercentage(this decimal? value)
        {
            if (!value.HasValue)
                return String.Empty;
            return FormatPercentage(value.Value);
        }

        public static string FormatPercentage(this decimal value)
        {
            return String.Format("{0:0.0%}", value);
        }

        public static string FormatPercentage(this object value)
        {
            return FormatPercentage(Convert.ToDecimal(value));
        }

        public static MvcHtmlString EdFiCheckbox(this System.Web.Mvc.HtmlHelper html, string text, bool isChecked, bool isGrayedOut)
        {
            var imageToUse = isChecked ? checkedImage : uncheckedImage;
            var style = isGrayedOut ? "ContentBoldTextStyleGrayOut" : String.Empty;
            var result = String.Format(edfiCheckboxFormat, EdFiDashboardsSite.Common.ThemeImage(imageToUse).Resolve(), style, text);
            return new MvcHtmlString(result);
        }

        public static MvcHtmlString AttributeItemPercentage<T>(this System.Web.Mvc.HtmlHelper html, AttributeItem<T> attributeItemWithUrl, bool boldLabelStyle = true)
        {
            if (attributeItemWithUrl == null)
                return new MvcHtmlString(String.Empty);

            var result = ItemWithUrl(attributeItemWithUrl.Attribute, attributeItemWithUrl.Value.FormatPercentage(), null, boldLabelStyle);
            return new MvcHtmlString(result);
        }

        public static MvcHtmlString AttributeItemPercentageWithUrl<T>(this System.Web.Mvc.HtmlHelper html, AttributeItemWithUrl<T> attributeItemWithUrl, bool boldLabelStyle = true)
        {
            if (attributeItemWithUrl == null)
                return new MvcHtmlString(String.Empty);

            var result = ItemWithUrl(attributeItemWithUrl.Attribute, attributeItemWithUrl.Value.FormatPercentage(), attributeItemWithUrl.Url, boldLabelStyle);
            return new MvcHtmlString(result);
        }

        public static MvcHtmlString AttributeItemNumberWithUrl<T>(this System.Web.Mvc.HtmlHelper html, AttributeItemWithUrl<T> attributeItemWithUrl, bool boldLabelStyle = true)
        {
            if (attributeItemWithUrl == null)
                return new MvcHtmlString(String.Empty);

            var result = ItemWithUrl(attributeItemWithUrl.Attribute, String.Format("{0:#,#}", attributeItemWithUrl.Value), attributeItemWithUrl.Url, boldLabelStyle);
            return new MvcHtmlString(result);
        }

        private static string ItemWithUrl(string attribute, string value, string url, bool boldLabelStyle)
        {
            string format = String.IsNullOrEmpty(url) ? attributeItemFormat : attributeItemWithUrlFormat;
            string labelStyle = boldLabelStyle ? "ContentBoldTextStyle SecondLineIndent" : "ContentLabelStyle";
            return String.Format(format, attribute, value, url, labelStyle); 
        }

        #region Html Templates

        private const string checkedImage = "CheckBox/Checked.gif";
        private const string uncheckedImage = "CheckBox/UnChecked.gif";

        private static string edfiCheckboxFormat = @"<img src='{0}' /><span class='{1}'>{2}</span>";

        private static string attributeItemFormat = @"<span class='label'>{0}</span><span class='value'>{1}</span>";
        private static string attributeItemWithUrlFormat = @"<span class='label'>{0}</span><span class='value'><a href='{2}'>{1}</a></span>";
        
        #endregion
    }
}