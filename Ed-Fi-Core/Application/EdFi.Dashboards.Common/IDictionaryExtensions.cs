// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EdFi.Dashboards.Common
{
    public static class IDictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value;

            if (dictionary.TryGetValue(key, out value))
                return value;

            return default(TValue);
        }

        public static string ContentsToString<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            // Build up each line one-by-one and them trim the end
            var builder = new StringBuilder();

            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
            {
                builder.Append('\t').Append(pair.Key).Append(": ").Append(pair.Value).Append('\n');
            }

            string result = builder.ToString();
            // Remove the final delimiter
            //result = result.TrimEnd(',');

            return result;
        }

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IEnumerable<TValue> source, Expression<Func<TValue, TKey>> keyValueAccessor)
        {
            var expr = keyValueAccessor.Body as MemberExpression;

            if (expr == null)
                throw new ArgumentException("Expression must be a member access expression.", "keyValueAccessor");

            var accessor = keyValueAccessor.Compile();

            foreach (var item in source)
                dictionary.Add(accessor(item), item);
        }
    }
}
