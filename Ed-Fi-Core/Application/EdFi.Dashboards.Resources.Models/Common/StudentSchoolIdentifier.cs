using System;
using EdFi.Dashboards.Resources.Models.Student;

namespace EdFi.Dashboards.Resources.Models.Common
{
    [Serializable]
    public class StudentSchoolIdentifier : IStudent
    {
        public int SchoolId { get; set; }
        public long StudentUSI { get; set; }
    }
}
