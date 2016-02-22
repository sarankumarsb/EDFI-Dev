using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Common
{
    public static class DescriptorHelper
    {
            private static void AppendPartToUniqueIdBuilder(StringBuilder builder, object part)
            {
                if (part == null)
                {
                    builder.Append("[-1]");
                }
                else
                {
                    string partString = Convert.ToString(part, CultureInfo.InvariantCulture);
                    builder.AppendFormat("[{0}]{1}", partString.Length, partString);
                }
            }

            public static string CreateUniqueId(params object[] parts)
            {
                return CreateUniqueId((IEnumerable<object>)parts);
            }

            public static string CreateUniqueId(IDictionary<string, string> parts)
            {
                var keyValues = new Dictionary<string, string>(parts, StringComparer.OrdinalIgnoreCase);
                var keys = parts.Keys.Select(key => key.ToUpperInvariant())
                                           .OrderBy(key => key, StringComparer.Ordinal);

                return CreateUniqueId(keys.Concat(keys.Select(key => keyValues[key])));
            }

            public static string CreateUniqueId(IEnumerable<object> parts)
            {
                // returns a unique string made up of the pieces passed in
                var builder = new StringBuilder();
                foreach (object part in parts)
                {
                    AppendPartToUniqueIdBuilder(builder, part);
                }

                return builder.ToString();
            }
    }
}
