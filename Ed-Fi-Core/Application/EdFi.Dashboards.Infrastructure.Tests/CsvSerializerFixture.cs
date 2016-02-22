// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Testing;
using NUnit.Framework;

namespace EdFi.Dashboards.Infrastructure.Tests
{
    [TestFixture]
    public class When_serializing_a_one_line_list_to_csv : TestFixtureBase
    {
        private string actualModel;
        private IEnumerable<IEnumerable<KeyValuePair<string, object>>> suppliedObject;

        protected override void EstablishContext()
        {
            suppliedObject = GetSuppliedModelToSerialize();
        }

        public class SerializableWrapper : ICsvSerializable
        {
            private readonly IEnumerable<IEnumerable<KeyValuePair<string, object>>> data;

            public SerializableWrapper(IEnumerable<IEnumerable<KeyValuePair<string, object>>> data)
            {
                this.data = data;
            }

            public IEnumerable<IEnumerable<KeyValuePair<string, object>>> ToSerializableEnumerable()
            {
                return data;
            }
        }

        protected virtual IEnumerable<IEnumerable<KeyValuePair<string, object>>> GetSuppliedModelToSerialize()
        {
            return new List<IEnumerable<KeyValuePair<string, object>>>
                       {
                           new List<KeyValuePair<string,object>>
                               {
                                   new KeyValuePair<string, object>("Some Property Name 01", 12),
                                   new KeyValuePair<string, object>("Some Property Name, 02", .89),
                                   new KeyValuePair<string, object>("Some - Property Name 03", "string"),
                                   new KeyValuePair<string, object>("Some \"Property\" 'Name' 04", 123),
                                   new KeyValuePair<string, object>("Some Property Name 05", "Smith, John"),
                                   new KeyValuePair<string, object>("Some Property Name 06", null),
                               }
                       };
        }

        protected override void ExecuteTest()
        {
            var serializer = new CsvSerializer();
            actualModel = serializer.Serialize(new SerializableWrapper(suppliedObject));
        }

        [Test]
        public void Should_return_column_names_in_first_row_of_csv()
        {
            var reader = new StringReader(actualModel);
            var actualFirstLine = reader.ReadLine();

            Assert.That(actualFirstLine, Is.EqualTo(GetExpectedCSVHeaderString()));
        }

        [Test]
        public void Should_return_rows_of_data_from_second_line_and_forward()
        {
            var reader = new StringReader(actualModel);
            //Lets skip the first line...
            reader.ReadLine();

            var actualLineCount = 0;
            var expectedReader = new StringReader(GetExpectedCSVDataRowsString());
            string actualLine;
            while ((actualLine = reader.ReadLine())!=null)
            {
                var expectedLine = expectedReader.ReadLine();
                Assert.That(actualLine, Is.EqualTo(expectedLine));
                actualLineCount++;
            }

            //We should make sure there are right number of rows in actual.
            Assert.That(actualLineCount, Is.EqualTo(suppliedObject.Count()));
        }

        private string GetExpectedCSVHeaderString()
        {
            var sampleFirstRow = suppliedObject.First();
            var headers = sampleFirstRow.Select(x => x.Key).ToArray();
            for (var i = 0; i < headers.Count(); i++)
            {
                headers[i] = headers[i].Replace('"', ' ');
                if (headers[i].Contains(","))
                    headers[i] = "\"" + headers[i] + "\"";
            }

            return string.Join(",", headers);
        }

        private string GetExpectedCSVDataRowsString()
        {
            var rows = new StringBuilder();

            foreach (var row in suppliedObject)
            {
                var values = row.Select(x => x.Value != null && x.Value.ToString().Contains(",") ? "\"" + x.Value + "\"" : x.Value);
                rows.AppendLine(string.Join(",", values));
            }

            return rows.ToString();
        }
    }

    public class When_serializing_a_multiple_line_list_to_csv : When_serializing_a_one_line_list_to_csv
    {
        protected override IEnumerable<IEnumerable<KeyValuePair<string, object>>> GetSuppliedModelToSerialize()
        {
            return new List<IEnumerable<KeyValuePair<string, object>>>
                       {
                           new List<KeyValuePair<string,object>>
                               {
                                   new KeyValuePair<string, object>("a", 12),
                                   new KeyValuePair<string, object>("b", .89),
                                   new KeyValuePair<string, object>("c", "string1"),
                                   new KeyValuePair<string, object>("d", 123),
                               },
                            new List<KeyValuePair<string,object>>
                               {
                                   new KeyValuePair<string, object>("a", 13),
                                   new KeyValuePair<string, object>("b", .90),
                                   new KeyValuePair<string, object>("c", "string2"),
                                   new KeyValuePair<string, object>("d", 124),
                               },
                            new List<KeyValuePair<string,object>>
                               {
                                   new KeyValuePair<string, object>("a", 14),
                                   new KeyValuePair<string, object>("b", .91),
                                   new KeyValuePair<string, object>("c", "string3"),
                                   new KeyValuePair<string, object>("d", 125),
                               }
                       };
        }
    }
}
