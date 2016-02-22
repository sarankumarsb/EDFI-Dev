using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Resources.Helpers
{
    public static class UrlHelper
    {
        /// <summary>
        /// Adds a parameter to an existing url string; this method will
        /// inspect the current url to decide how to append the parameter.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter to add.</param>
        /// <param name="parameterValue">The parameter value to add.</param>
        /// <returns></returns>
        public static string AddParameter(this string value, string parameterName, string parameterValue)
        {
            var parameterToAdd = parameterName + "=" + parameterValue;
            var newUrl = value;
            newUrl += value.Contains("?") ? "&" + parameterToAdd : "?" + parameterToAdd;
            return newUrl;
        }

        /// <summary>
        /// Adds a parameter to an existing Uri object; this method will
        /// inspect the current Uri to decide how to append the parameter.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter to add.</param>
        /// <param name="parameterValue">The parameter value to add.</param>
        /// <returns></returns>
        public static Uri AddParameter(this Uri value, string parameterName, string parameterValue)
        {
            var parameterToAdd = parameterName + "=" + parameterValue;
            var newUri = value.AbsoluteUri;
            newUri += newUri.Contains("?") ? "&" + parameterToAdd : "?" + parameterToAdd;
            var returnUri = new Uri(newUri);
            return returnUri;
        }
    }
}
