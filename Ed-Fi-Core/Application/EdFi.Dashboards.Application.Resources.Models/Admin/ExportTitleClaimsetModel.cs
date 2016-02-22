using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Infrastructure;

namespace EdFi.Dashboards.Application.Resources.Models.Admin
{
    [Serializable]
    public class ExportTitleClaimSetModel : ICsvSerializable
    {
        public ExportTitleClaimSetModel()
        {
            Rows = new List<Row>();
        }

        public IEnumerable<Row> Rows { get; set; }

        [Serializable]
        public class Row
        {
            public Row()
            {
                Cells = new List<KeyValuePair<string, object>>();
            }

            public IEnumerable<KeyValuePair<string, object>> Cells { get; set; }
        }

        public IEnumerable<IEnumerable<KeyValuePair<string, object>>> ToSerializableEnumerable()
        {
            return Rows.Select(row => row.Cells);
        }
    }
}
