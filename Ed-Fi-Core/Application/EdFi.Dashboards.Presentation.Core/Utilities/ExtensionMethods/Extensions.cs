// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using log4net;

namespace EdFi.Dashboards.Presentation.Web.Utilities
{
	public static class Extensions
    {
        private static readonly ILog errorLog = LogManager.GetLogger("ErrorReporter");

		public static T QueryString<T>(this HttpRequest request, string key)
		{
			string text = request[key];

			try
			{
				Type sourceType = typeof(T);
				Type destinationType;

				if (sourceType.IsGenericType
					&& sourceType.GetGenericTypeDefinition() == typeof(Nullable<>))
					destinationType = Nullable.GetUnderlyingType(sourceType);
				else
				{
					destinationType = sourceType;
				}

				var value = (T)Convert.ChangeType(text, destinationType);

				return value;
			}
			catch (Exception ex)
			{
			    errorLog.Warn(String.Format("T='{0}' key='{1}' text='{2}'", typeof (T), key, text), ex);
				return default(T);
			}
		}

        public static object GetMetricRenderingContextValue(this WebViewPage view, string key)
        {
            var dictionary = view.ViewBag.MetricRenderingContext;

            if (dictionary.ContainsKey(key))
                return dictionary[key];
            
            //If we dont have it create the key and return an empty one.
            dictionary.Add(key,null);

            return dictionary[key];
        }

        public static void SetMetricRenderingContextValue(this WebViewPage view, string key, object value)
        {
            var dictionary = view.ViewBag.MetricRenderingContext;

            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }
    }
}
