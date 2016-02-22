using System;

namespace EdFi.Dashboards.Resources.Models.LocalEducationAgency
{
    [Serializable]
    public class HomeModel
    {
        public int LocalEducationAgencyId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string SupportContact { get; set; }
        public string SupportEmail { get; set; }
        public string SupportPhone { get; set; }
        public string TrainingAndPlanningHref { get; set; }
        public string LoginUrl { get; set; }
    }
}
