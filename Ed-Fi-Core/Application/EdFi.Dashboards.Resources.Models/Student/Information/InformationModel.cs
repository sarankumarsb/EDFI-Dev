// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Core;

namespace EdFi.Dashboards.Resources.Models.Student.Information
{
    [Serializable]
    public class InformationModel : IStudent 
    {
        public InformationModel()
        {
            AddressLines = new List<string>();
            Parents = new List<ParentInformation>();
            Programs = new List<StudentProgramParticipation>();
            SchoolInformation = new SchoolInformationDetail();
            OtherStudentInformation = new List<OtherInformation>();
            SpecialServices = new List<SpecialService>();
        }

        private const string cityStateZipFormat = "{0}, {1} {2}";

        public long StudentUSI { get; set; }

        public SchoolCategory SchoolCategory { get; set; }

        public string FullName { get; set; }

        public string ProfileThumbnail { get; set; }

        public IEnumerable<string> AddressLines { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public string CityStateZipCode 
        {
            get
            {
                return !string.IsNullOrEmpty(City) ? String.Format(cityStateZipFormat, City, State, ZipCode) : string.Empty;
            }
        }

        public string TelephoneNumber { get; set; }

        public string EmailAddress { get; set; }

        public DateTime DateOfBirth { get; set; }

        public int? CurrentAge { get; set; }

        public int? CohortYear { get; set; }

        public string PlaceOfBirth { get; set; }

        public string Gender { get; set; }

        public string OldEthnicity { get; set; }

        public string HispanicLatinoEthnicity { get; set; }

        public string Race { get; set; }

        public string HomeLanguage { get; set; }

        public string Language { get; set; }

        public string ParentMilitary { get; set; }

        public string SingleParentPregnantTeen { get; set; }

        public IEnumerable<ParentInformation> Parents { get; set; }

        public IEnumerable<StudentProgramParticipation> Programs { get; set; }

        public SchoolInformationDetail SchoolInformation { get; set; }

        public IEnumerable<OtherInformation> OtherStudentInformation { get; set; }

        public IEnumerable<SpecialService> SpecialServices { get; set; }
    }

    [Serializable]
    public class SchoolInformationDetail : IStudent
    {
        public SchoolInformationDetail()
        {
            FeederSchools = new List<string>();
        }

        public long StudentUSI { get; set; }

        public string GradeLevel { get; set; }

        public string Homeroom { get; set; }

        public string LateEnrollment { get; set; }

        public DateTime? DateOfEntry { get; set; }

        public DateTime? DateOfWithdrawal { get; set; }

        public List<string> FeederSchools { get; set; }

        public string GraduationPlan { get; set; }

        public string ExpectedGraduationYear { get; set; }
    }

    [Serializable]
    public class ParentInformation : IStudent
    {
        private const string cityStateZipFormat = "{0}, {1} {2}";

        public ParentInformation()
        {
            AddressLines = new List<string>();
        }

        public long StudentUSI { get; set; }

        public int ParentUSI { get; set; }

        public string FullName { get; set; }

        public string Relation { get; set; }

        public bool? PrimaryContact { get; set; }

        public bool? LivesWith { get; set; }

        public List<string> AddressLines { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public string CityStateZipCode
        {
            get
            {
                return !string.IsNullOrEmpty(City) ? String.Format(cityStateZipFormat, City, State, ZipCode) : string.Empty;
            }
        }

        public string TelephoneNumber { get; set; }

        public string WorkTelephoneNumber { get; set; }

        public string EmailAddress { get; set; }
    }

    [Serializable]
    public abstract class StudentIndicatorBase : IStudent
    {
        protected StudentIndicatorBase()
        {
            Children = new List<StudentIndicatorBase>();
        }

        public long StudentUSI { get; set; }

        public string Name { get; set; }

        public bool Status { get; set; }

        public IEnumerable<StudentIndicatorBase> Children { get; set; }
    }

    [Serializable]
    public class StudentProgramParticipation : StudentIndicatorBase
    {
    }

    [Serializable]
    public class OtherInformation : StudentIndicatorBase
    {
    }

    [Serializable]
    public class SpecialService : StudentIndicatorBase
    {
    }

    
}
