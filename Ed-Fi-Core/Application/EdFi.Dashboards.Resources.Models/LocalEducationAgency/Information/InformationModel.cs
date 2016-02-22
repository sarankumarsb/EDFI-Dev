// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Models.Common;

namespace EdFi.Dashboards.Resources.Models.LocalEducationAgency.Information
{
    [Serializable]
    public class InformationModel : ResourceModelBase
    {
        public InformationModel()
        {
            AddressLines = new List<string>();
            TelephoneNumbers = new List<TelephoneNumber>();
            Administrators = new List<Administrator>();
            Accountability = new List<AttributeItem<string>>();
            SchoolAccountabilityRatings = new List<AttributeItem<string>>();
            NumberOfSchools = new List<AttributeItemWithUrl<string>>();
            StudentTeacherRatios = new List<AttributeItem<string>>();
            StudentDemographics = new Demographics();
            StudentsByProgram = new List<AttributeItemWithUrl<decimal?>>();
            StudentIndicatorPopulation = new List<AttributeItemWithUrl<decimal?>>();
        }

        public int LocalEducationAgencyId { get; set; }

        public string Name { get; set; }

        public string ProfileThumbnail { get; set; }

        public List<string> AddressLines { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public List<TelephoneNumber> TelephoneNumbers { get; set; }

        public string WebSite { get; set; }

        public List<Administrator> Administrators { get; set; }

        public List<AttributeItem<string>> Accountability { get; set; }

        public List<AttributeItem<string>> SchoolAccountabilityRatings { get; set; }

        public int LocalEducationAgencyEnrollment { get; set; }

        public List<AttributeItemWithUrl<string>> NumberOfSchools { get; set; }

        public AttributeItemWithUrl<decimal?> LateEnrollmentStudents { get; set; }

        public List<AttributeItem<string>> StudentTeacherRatios { get; set; }

        public Demographics StudentDemographics { get; set; }

        public List<AttributeItemWithUrl<decimal?>> StudentsByProgram { get; set; }

        public List<AttributeItemWithUrl<decimal?>> StudentIndicatorPopulation { get; set; }
    }
}
