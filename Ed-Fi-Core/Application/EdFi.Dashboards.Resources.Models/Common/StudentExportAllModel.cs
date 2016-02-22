// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Models.Student;

namespace EdFi.Dashboards.Resources.Models.Common
{
    [Serializable]
    public class StudentExportAllModel : ICsvSerializable
    {
        public StudentExportAllModel()
        {
            Rows = new List<Row>();
        }

        public IEnumerable<Row> Rows { get; set; }

        [Serializable]
        public class Row : IStudent
        {
            public Row()
            {
                Cells = new List<KeyValuePair<string, object>>();
            }

            public IEnumerable<KeyValuePair<string, object>> Cells { get; set; }
            
            public long StudentUSI { get; set; }
            public int SchoolId { get; set; }
        }

        public IEnumerable<IEnumerable<KeyValuePair<string, object>>> ToSerializableEnumerable()
        {
            return Rows.Select(row => row.Cells);
        }
    }
}
