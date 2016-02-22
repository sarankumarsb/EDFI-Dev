using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resource.Models.Common;

namespace EdFi.Dashboards.Application.Resources.Models.LocalEducationAgency.Detail
{
    [Serializable]
    public class GoalPlanningSchoolListModel : ResourceModelBase
    {
        public IEnumerable<ProposedGoal> ProposedGoals { get; set; }
        public IEnumerable<PublishedGoal> PublishedGoals { get; set; }
    }

    [Serializable]
    public class ProposedGoal
    {
        public int EducationOrganizationId { get; set; }
        public int MetricId { get; set; }
        public decimal Goal { get; set; }
    }

    [Serializable]
    public class PublishedGoal
    {
        public int EducationOrganizationId { get; set; }
        public int MetricId { get; set; }
        public decimal Goal { get; set; }
        public string DisplayGoal { get; set; }
    }
}
