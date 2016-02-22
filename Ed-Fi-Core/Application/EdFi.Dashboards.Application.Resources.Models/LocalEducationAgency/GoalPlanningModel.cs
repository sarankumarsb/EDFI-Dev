using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resource.Models.Common;

namespace EdFi.Dashboards.Application.Resources.Models.LocalEducationAgency
{
    [Serializable]
    public class GoalPlanningModel : ResourceModelBase
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
        public decimal GoalDifference { get; set; }
        public string DisplayGoalDifference { get; set; }
    }
}
