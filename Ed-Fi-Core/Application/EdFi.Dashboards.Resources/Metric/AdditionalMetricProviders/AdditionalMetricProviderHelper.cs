// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EdFi.Dashboards.Common.Utilities;

namespace EdFi.Dashboards.Resources.Metric.AdditionalMetricProviders
{
    public static class AdditionalMetricProviderHelper
    {
        private static Dictionary<Type, Dictionary<string, PropertyInfo>> propertiesByType = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        public static Dictionary<string, PropertyInfo> GetPropertiesFor(object source)
        {
            Dictionary<string, PropertyInfo> propertiesByName;
            Type type = source.GetType(); // Cannot use generics due to extensibility, need actual runtime type

            if (!propertiesByType.TryGetValue(type, out propertiesByName))
            {
                propertiesByName = type.GetProperties().ToDictionary(info => info.Name, info => info);
                propertiesByType[type] = propertiesByName;
            }

            return propertiesByName;
        }

        public static T GetPropertyValue<T>(IDictionary<string, Object> propertiesByName, dynamic source, string propertyName)
        {
            var value = propertiesByName.ContainsKey(propertyName) ? propertiesByName[propertyName] : null;
            var targetType = typeof(T);

            // No text? Return default value for the target type.
            if (value == null)
                return (T)ReflectionUtility.GetDefault(targetType);

            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // Unwrap the nullable type to the underlying value type
                targetType = targetType.GetGenericArguments()[0]; // Nullable types should only have 1 generic argument
            }

            return (T)Convert.ChangeType(value, targetType);
        }

        public static T GetPropertyValue<T>(Dictionary<string, PropertyInfo> propertiesByName, object source, string propertyName)
        {
            var propertyInfo = propertiesByName[propertyName];
            var value = propertyInfo.GetValue(source, null);

            Type targetType = typeof(T);

            // No text? Return default value for the target type.
            if (value == null)
                return (T)ReflectionUtility.GetDefault(targetType);

            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // Unwrap the nullable type to the underlying value type
                targetType = targetType.GetGenericArguments()[0]; // Nullable types should only have 1 generic argument
            }

            return (T)Convert.ChangeType(value, targetType);
        }
    }
}