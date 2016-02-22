// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;

namespace EdFi.Dashboards.Resources.Models.Student.Detail
{
    [Serializable]
    public class DisciplineReferralModel : IStudent
    {
        public DisciplineReferralModel() {}
        
        public DisciplineReferralModel(long studentUSI)
        {
            StudentUSI = studentUSI;
        }

        public long StudentUSI { get; set; }
        public DateTime Date { get; set; }
        public string IncidentCode { get; set; }
        public string IncidentDescription { get; set; }
        public string Action { get; set; }
    }
}
