using System;

namespace EdFi.Dashboards.Resources.Models.Student.Detail
{
    [Serializable]
    public class DaysAbsentModel : IStudent
    {
        public DaysAbsentModel() {}
        
        public DaysAbsentModel(long studentUSI)
        {
            StudentUSI = studentUSI;
        }

        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }
        public string Context { get; set; }
        public int? TotalDays { get; set; }
        public int? AttendanceDays { get; set; }
        public int? ExcusedDays { get; set; }
        public int? UnexcusedDays { get; set; }
    }
}
