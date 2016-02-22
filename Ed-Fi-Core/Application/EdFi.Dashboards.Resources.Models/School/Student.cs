using System;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Models.Student;

namespace EdFi.Dashboards.Resources.Models.School
{
    [Serializable]
    public class Student : ResourceModelBase, IStudent
    {
        public Student() {}
        
        public Student(long studentUSI)
        {
            StudentUSI = studentUSI;
        }

        public Student(long studentUSI, string fullName)
        {
            StudentUSI = studentUSI;
            FullName = fullName;
        }

        public long StudentUSI { get; set; }
        public string FullName { get; set; }
    }
}
