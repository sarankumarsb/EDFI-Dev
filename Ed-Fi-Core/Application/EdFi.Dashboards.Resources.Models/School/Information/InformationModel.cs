// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Resources.Models.Common;

namespace EdFi.Dashboards.Resources.Models.School.Information
{
    [Serializable]
    public class InformationModel
    {
        public InformationModel()
        {
            AddressLines = new List<string>();
            TelephoneNumbers = new List<TelephoneNumber>();
            Administration = new List<Administrator>();
            Accountability = new List<AttributeItem<string>>();
            GradePopulation = new GradePopulationItem();
            StudentDemographics = new Demographics();
            StudentsByProgram = new List<AttributeItemWithTrend<decimal?>>();
            FeederSchoolDistribution = new List<AttributeItemWithTrend<decimal?>>();
            HighSchoolGraduationPlan = new List<AttributeItemWithTrend<decimal?>>();
        }

        public int SchoolId { get; set; }

        public SchoolCategory SchoolCategory { get; set; }

        public string Name { get; set; }

        public string ProfileThumbnail { get; set; }

        public List<string> AddressLines { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public List<TelephoneNumber> TelephoneNumbers { get; set; }

        public string WebSite { get; set; }

        public List<Administrator> Administration { get; set; }

        public List<AttributeItem<string>> Accountability { get; set; }

        public GradePopulationItem GradePopulation { get; set; }

        public Demographics StudentDemographics { get; set; }

        public List<AttributeItemWithTrend<decimal?>> StudentsByProgram { get; set; }

        public List<AttributeItemWithTrend<decimal?>> FeederSchoolDistribution { get; set; }

        public List<AttributeItemWithTrend<decimal?>> HighSchoolGraduationPlan { get; set; }
    }

    [Serializable]
    public class GradePopulationItem
    {
        public GradePopulationItem()
        {
            TotalNumberOfStudents = new AttributeItemWithTrend<decimal?>();
            TotalNumberOfStudentsByGrade = new List<AttributeItemWithTrend<decimal?>>();
            Indicators = new List<AttributeItemWithTrend<decimal?>>();
            SchoolLateEnrollment = new AttributeItemWithUrl<decimal?>();
        }

        public AttributeItemWithTrend<decimal?> TotalNumberOfStudents { get; set; }

        public List<AttributeItemWithTrend<decimal?>> TotalNumberOfStudentsByGrade { get; set; }

        public List<AttributeItemWithTrend<decimal?>> Indicators { get; set; }

        public AttributeItemWithUrl<decimal?> SchoolLateEnrollment { get; set; }
    }
}
