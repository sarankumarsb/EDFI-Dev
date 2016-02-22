// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Models.Student.Detail
{
    [Serializable]
    public class ClassAbsencesModel : IStudent
    {
        public ClassAbsencesModel()
        {
            Classes = new List<Class>();
        }

        public long StudentUSI { get; set; }
        public List<Class> Classes { get; set; }

        [Serializable]
        public class Class : IStudent
        {
            public Class() 
            {
                Weeks = new List<Week>();            
            }
            
            public Class(long studentUSI)
                : this()
            {
                StudentUSI = studentUSI;
            }

            public long StudentUSI { get; set; }
            public string Name { get; set; }
            public List<Week> Weeks { get; set; }

            [Serializable]
            public class Week : IStudent
            {
                public Week()
                {
                    WeekDayEvents = new List<WeekDayEvent>();                
                }
                
                public Week(long studentUSI) : this()
                {
                    StudentUSI = studentUSI;
                }

                public long StudentUSI { get; set; }
                public DateTime StartDate { get; set; }
                public DateTime EndDate { get; set; }
                public List<WeekDayEvent> WeekDayEvents { get; set; }

                [Serializable]
                public class WeekDayEvent : IStudent
                {
                    public WeekDayEvent() {}
                    
                    public WeekDayEvent(long studentUSI)
                    {
                        StudentUSI = studentUSI;
                    }

                    public long StudentUSI { get; set; }
                    public DateTime Date { get; set; }
                    public AttendanceEvent AttendanceEventType { get; set; }
                    public string Reason { get; set; }
                }
            }
        }

        public enum AttendanceEvent
        {
            NoData = 0,
            Present = 1,
            Excused = 2,
            Unexcused = 3,
            Tardy = 4,
            NonInstructional = 5,
        }
    }
}
