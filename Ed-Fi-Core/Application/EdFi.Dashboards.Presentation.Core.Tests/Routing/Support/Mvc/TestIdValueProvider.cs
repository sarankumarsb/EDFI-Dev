using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Web.Mvc;

namespace EdFi.Dashboards.Presentation.Core.Tests.Routing.Support.Mvc
{
    public class TestIdValueProvider : IValueProvider
    {
        private readonly Dictionary<string, string> routeDataValuesByKey;

        public TestIdValueProvider(Dictionary<string, string> routeDataValuesByKey)
        {
            if (routeDataValuesByKey.ContainsKey("operationalDashboardSubtype"))
                routeDataValuesByKey["metricVariant"] = "placeholder-so-that-operationalDashboardSubtype-gets-translated-to-metricVariantId"; 

            this.routeDataValuesByKey = routeDataValuesByKey;
        }

        public bool ContainsPrefix(string prefix)
        {
            var lookupKey = GetMatchingLookupKeyFromPrefix(prefix);

            if (lookupKey != null)
            {
                return routeDataValuesByKey.ContainsKey(lookupKey)
                       && routeDataValuesByKey[lookupKey] != null;
            }

            return false;
        }

        private static string GetMatchingLookupKeyFromPrefix(string targetPropertyName)
        {
            string lookupKey = null;

            if (targetPropertyName.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
                lookupKey = targetPropertyName.Remove(targetPropertyName.Length - 2);
            else if (targetPropertyName.EndsWith("USI", StringComparison.OrdinalIgnoreCase))
                lookupKey = targetPropertyName.Remove(targetPropertyName.Length - 3);
            return lookupKey;
        }

        public ValueProviderResult GetValue(string targetPropertyName)
        {
            if (!ContainsPrefix(targetPropertyName))
                return null;

            string routeKey = GetMatchingLookupKeyFromPrefix(targetPropertyName);
            string attemptedValue = routeDataValuesByKey[routeKey];

            var fieldInfo = typeof(TestId).GetField(routeKey, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static);
            object rawValue = fieldInfo.GetValue(null);

            return new ValueProviderResult(rawValue, attemptedValue, CultureInfo.CurrentCulture);
        }
    }
}