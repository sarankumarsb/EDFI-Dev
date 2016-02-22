// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Reflection;
using System.Text;

namespace EdFi.Dashboards.Resources.Navigation
{
    public static class EdFiDashboardsSite
    {
        static EdFiDashboardsSite()
        {
            Common = new WebForms.Areas.Common();
        }

        public const string ContentUrl = "~/Core_Content/";
        public const string ThemeImages = "~/App_Themes/Theme1/img/";
        public const string Theme = "~/App_Themes/Theme1/";
        //public const string RootImages = ContentUrl + "img/";
        public const string Scripts = ContentUrl + "Scripts/";

        public static WebForms.Areas.Common Common { get; private set; }

        public static bool IsParameters(ParameterInfo parameters)
        {
            return Attribute.IsDefined(parameters, typeof(ParamArrayAttribute));
        }

        public static string BuildUrlParameters(MethodBase callingMethod, params object[] parameters)
        {
            var parmInfos = callingMethod.GetParameters();

            var sb = new StringBuilder();

            for (int i = 0; i < parmInfos.Length; i++)
            {
                // Quit if we've gone beyond the parameters passed (and into the additionalParameters argument)
                if (IsParameters(parmInfos[i]))
                    break;

                if (parameters[i] != null && !string.IsNullOrEmpty(parameters[i].ToString()))
                    sb.Append("&" + parmInfos[i].Name + "=" + parameters[i]);
            }

            if (sb.Length > 0)
                return "?" + sb.ToString(1, sb.Length - 1);

            return null;
        }
    }
}
