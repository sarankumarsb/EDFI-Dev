// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;

namespace EdFi.Dashboards.Resources.Models.Student.Overview
{
    [Serializable]
    public class OverviewModel : IStudent
    {
        public long StudentUSI { get; set; }
        public string FullName { get; set; }
        public string ProfileThumbnail { get; set; }
        public string ParentContactInfoLink { get; set; }
        public int MetricVariantId { get; set; }
        public string RenderingMode { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? AgeAsOfToday { get; set; }
        public string Language { get; set; }
        public string HomeLanguage { get; set; }
        public string GradeLevel { get; set; }
        public bool LimitedEnglishProficiency { get; set; }
        public bool LimitedEnglishMonitoredFirstProficiency { get; set; }
        public bool LimitedEnglishMonitoredSecondProficiency { get; set; }
        public bool BilingualProgram { get; set; }
        public bool EnglishAsSecondLanguage { get; set; }
        public bool GiftedAndTalented { get; set; }
        public bool SpecialEducation { get; set; }
        public bool Designation { get; set; }
    }
}
