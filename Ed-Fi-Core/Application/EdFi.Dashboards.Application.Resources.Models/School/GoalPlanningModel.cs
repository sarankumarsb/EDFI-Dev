using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.Application.Resources.Models.School
{
    [Serializable]
    public class GoalPlanningModel
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
