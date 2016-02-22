using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resources.Models.Common;

namespace EdFi.Dashboards.Resources.Models.School
{
    [Serializable]
    public class StudentGradeMenuModel
    {
        public const string AllGradesItemText = "All Students";
        public StudentGradeMenuModel()
        {
            Grades = new List<AttributeItemWithUrl<decimal>>();
        }

        public IList<AttributeItemWithUrl<decimal>> Grades { get; set; }
    }
}
