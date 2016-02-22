// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Core;

namespace EdFi.Dashboards.Resources.Models.Student
{
    [Serializable]
    public class Accommodations : Enumeration<Accommodations>
    {
        public static Accommodations None = new Accommodations(0, "None");
        public static Accommodations GiftedAndTalented = new Accommodations(1, "Gifted & Talented");
        public static Accommodations SpecialEducation = new Accommodations(2, "Special Education");
        public static Accommodations ESLAndLEP = new Accommodations(3, "ESL, LEP or Bilingual");
        public static Accommodations Overage = new Accommodations(4, "Overage");
        public static Accommodations Repeater = new Accommodations(5, "Repeater");
        public static Accommodations LateEnrollment = new Accommodations(6, "Late Enrollment");
        public static Accommodations PartialTranscript = new Accommodations(7, "Partial Transcript");
        public static Accommodations TestAccommodation = new Accommodations(8, "Test Accommodation");
        public static Accommodations Designation = new Accommodations(9, "504 Designation");
        public static Accommodations LEPMonitoredFirst = new Accommodations(10, "LEP Monitored First");
        public static Accommodations LEPMonitoredSecond = new Accommodations(11, "LEP Monitored Second");

        public Accommodations(int value, string displayName) : base(value, displayName) { }
    }

    [Serializable]
    public class Accommodation : IStudent
    {
        public Accommodation() 
        {
            AccommodationsList = new List<Accommodations>();
        }
        
        public Accommodation(long studentUSI)
        {
            StudentUSI = studentUSI;
            AccommodationsList = new List<Accommodations>();
        }

        public Accommodation(long studentUSI, List<Accommodations> accommodationsList)
        {
            StudentUSI = studentUSI;
            AccommodationsList = accommodationsList;
        }

        public long StudentUSI { get; set; }
        public List<Accommodations> AccommodationsList { get; set; }
    }
}
