// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Infrastructure.Implementations
{
    public class CsvSerializer : ICsvSerializer
    {
        private static readonly KeyValuePair<string,object> defaultKvp = default(KeyValuePair<string, object>);
        private const string delimiter = ",";
        private const string wrapInQuotesFormat = "\"{0}\"";

        public string Serialize(ICsvSerializable serializable)
        {
            var keyValuePairList = serializable.ToSerializableEnumerable();

            var rowSample = keyValuePairList.FirstOrDefault();
            if (rowSample == null)
                return string.Empty;

            string[] properties = rowSample.Select(x => x.Key).ToArray();

            PreProcessColumnHeaders(properties);

            string header = string.Join(delimiter, properties);

            var csvdata = new StringBuilder();
            csvdata.AppendLine(header);

            foreach (var row in keyValuePairList)
                csvdata.AppendLine(ToCsvFields(properties, row));

            return csvdata.ToString();
        }

        private static void PreProcessColumnHeaders(string[] properties)
        {
            for (var i = 0; i < properties.Count(); i++)
            {
                properties[i] = WrapInQuotes(CleanString(properties[i]));
            }
        }

        private static string WrapInQuotes(string toWrap)
        {
            return toWrap.Contains(delimiter) ? String.Format(wrapInQuotesFormat, toWrap) : toWrap;
        }

        private static string CleanString(string toClean)
        {
            return toClean.Replace('"',' ');
        }

        private static string ToCsvFields(IEnumerable<string> properties, IEnumerable<KeyValuePair<string, object>> row)
        {
            var lineStringBuilder = new StringBuilder();

            foreach (var p in properties)
            {
                if (lineStringBuilder.Length > 0)
                    lineStringBuilder.Append(delimiter);

                var kvp = row.SingleOrDefault(x => WrapInQuotes(CleanString(x.Key)) == p);

                if (!kvp.Equals(defaultKvp))
                {
                    if (kvp.Value != null)
                        lineStringBuilder.Append(WrapInQuotes(kvp.Value.ToString()));
                    else
                        lineStringBuilder.Append(kvp.Value);

                }
            }

            return lineStringBuilder.ToString();
        }
    }


}
