using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EdFi.Dashboards.Infrastructure.Implementations.Caching
{
    public class CacheKeyGenerator : ICacheKeyGenerator
    {
        private const string Delimiter = "|";
        private const string NullWithDelimiter = "null|";

        public string GenerateCacheKey(MethodInfo methodInvocationTarget, object[] arguments)
        {
            var sb = new StringBuilder();

            sb.Append(GetCacheKeyPrefix(methodInvocationTarget));

            foreach (var arg in arguments)
            {
                if (arg == null)
                {
                    sb.Append(NullWithDelimiter);
                    continue;
                }

                //We can only cache/build a cache Key for ValueTypes and Objects that have properties that are ValueTypes.
                if (!(arg.GetType().IsValueType || arg is string))
                {
                    AppendParameterNameValueString(sb, arg);
                }
                else
                {
                    sb.Append(arg.GetHashCode());
                    sb.Append(Delimiter);
                }

            }

            return sb.ToString();
        }

        private static ConcurrentDictionary<MethodInfo, string> cacheKeyPrefixByMethodInfo = new ConcurrentDictionary<MethodInfo, string>();

        private static string GetCacheKeyPrefix(MethodInfo invocationTarget)
        {
            return cacheKeyPrefixByMethodInfo.GetOrAdd(invocationTarget,
                i => string.Format("{0}.{1}$", i.DeclaringType.FullName, invocationTarget.Name));
        }

        private static ConcurrentDictionary<Type, PropertyInfo[]> propertiesByType = new ConcurrentDictionary<Type, PropertyInfo[]>();

        private static PropertyInfo[] GetProperties(Type type)
        {
            return propertiesByType.GetOrAdd(type, t => t.GetProperties());
        }

        public static void AppendParameterNameValueString<T>(StringBuilder stringBuilder, T obj)
        {
            var objectProperties = GetProperties(obj.GetType());

            foreach (var objectProperty in objectProperties)
            {
                var value = objectProperty.GetValue(obj, null);

                if (objectProperty.PropertyType.IsValueType || objectProperty.PropertyType == typeof(string))
                    stringBuilder.AppendFormat("{0}:{1},", objectProperty.Name, value);
                else if (value != null)
                {
                    var collection = value as IEnumerable;
                    if (collection != null)
                    {
                        var sbEnumItems = new StringBuilder();

                        foreach (var i in collection)
                        {
                            sbEnumItems.Append(i.GetHashCode());
                            sbEnumItems.Append("|");
                        }
                        stringBuilder.AppendFormat("{0}:{1}", objectProperty.Name, sbEnumItems.ToString());
                    }
                    else
                    {
                        stringBuilder.AppendFormat("{0}:{1},", objectProperty.Name, value.GetHashCode());
                    }
                }
                else
                    stringBuilder.AppendFormat("{0}:null,", objectProperty.Name);
            }
        }
    }
}