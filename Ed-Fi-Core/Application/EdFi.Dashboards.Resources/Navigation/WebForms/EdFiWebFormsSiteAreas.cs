// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Reflection;
using System.Text;
using EdFi.Dashboards.Resources.Navigation.WebForms;

namespace EdFi.Dashboards.Resources.Navigation
{
    public class EdFiWebFormsSiteAreas : SiteAreasBase<WebForms.Areas.Common>
    {
        private string contentUrl = "~/Core_Content/";
		public string ContentUrl
		{
			get { return contentUrl; }
			set { contentUrl = value; }
		}

        private string contentImages = "~/Core_Content/Images/";
		public string ContentImages
		{
			get { return contentImages; }
			set { contentImages = value; }
		}

        public string BuildUrlParameters(MethodBase callingMethod, params object[] parameters)
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

        public bool IsParameters(ParameterInfo parameters)
        {
            return Attribute.IsDefined(parameters, typeof(ParamArrayAttribute));
        }

    }
}