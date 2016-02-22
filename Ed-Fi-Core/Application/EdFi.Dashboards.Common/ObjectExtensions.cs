// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Helpers;
using System.Web.Mvc;

namespace EdFi.Dashboards.Common
{
    public static class ObjectExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this T obj)
        {
            yield return obj;
        }

        public static IQueryable<T> ToQueryable<T>(this T obj)
        {
            return obj.ToEnumerable().AsQueryable();
        }

        public static T[] ToArray<T>(this T obj)
        {
            return new[] { obj };
        }

        public static IEnumerable<KeyValuePair<string, dynamic>> ToKeyValuePairs(this object @object, Func<string, object> nullHandler = null, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance, Func<PropertyInfo, bool> shouldInclude = null)
        {
            if (@object == null)
                yield break;

            var properties = @object.GetType().GetProperties(bindingFlags);

            if (shouldInclude == null)
                shouldInclude = p => true; // Always include properties

            foreach (var p in properties.Where(shouldInclude))
            {
                var key = p.Name;
                var value = p.GetValue(@object, null);

                if (value != null)
                {
                    yield return new KeyValuePair<string, dynamic>(key, value);
                }
                else if (nullHandler != null)
                {
                    yield return new KeyValuePair<string, dynamic>(key, nullHandler(key));
                }
            }
        }

        public static Dictionary<string, dynamic> ToDictionary(this object @object, Func<string, object> nullHandler = null, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance, Func<PropertyInfo, bool> shouldInclude = null)
        {
            var dictionary = new Dictionary<string, dynamic>();

            foreach (var keyValuePair in ToKeyValuePairs(@object, nullHandler, bindingFlags, shouldInclude))
                dictionary.Add(keyValuePair.Key, keyValuePair.Value);

            return dictionary;
        }

        /// <summary>
        /// Turns an object into a json string and assigns it to a javascript variable.
        /// </summary>
        /// <param name="objectToSerialize">The object to serialize.</param>
        /// <param name="javascriptVariableName">Name of the javascript variable to store the json in.</param>
        /// <returns></returns>
        public static MvcHtmlString ToJson(this object objectToSerialize, string javascriptVariableName = "")
        {
            if (javascriptVariableName != string.Empty)
            {
                return
                    MvcHtmlString.Create(string.Format("var {0} = {1};", javascriptVariableName,
                        Json.Encode(objectToSerialize)));
            }
            else
            {
                return MvcHtmlString.Create(Json.Encode(objectToSerialize));
            }
        }
    }
}
