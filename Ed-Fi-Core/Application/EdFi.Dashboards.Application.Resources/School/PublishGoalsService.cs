using System.Linq;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Application.Resources.Models.School;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Application.Resources.School
{
    public class PublishGoalsRequest
    {
        public int SchoolId { get; set; }

        public static PublishGoalsRequest Create(int schoolId)
        {
            return new PublishGoalsRequest { SchoolId = schoolId };
        }
    }

    public interface IPublishGoalsService : IPostHandler<PublishGoalsRequest, PublishGoalsModel> { }

    public class PublishGoalsService : IPublishGoalsService
    {
        private readonly IPersistingRepository<EducationOrganizationGoalPlanning> goalPlanningRepository;
        private readonly IPersistingRepository<EducationOrganizationGoal> goalRepository;

        public PublishGoalsService(IPersistingRepository<EducationOrganizationGoalPlanning> goalPlanningRepository, IPersistingRepository<EducationOrganizationGoal> goalRepository)
        {
            this.goalPlanningRepository = goalPlanningRepository;
            this.goalRepository = goalRepository;
        }

        [NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.ManageGoals)]
        public PublishGoalsModel Post(PublishGoalsRequest request)
        {
            var goalPlanningQuery = from goal in goalPlanningRepository.GetAll()
                                    where goal.EducationOrganizationId == request.SchoolId
                                    select goal;

            var goalQuery = (from goal in goalRepository.GetAll()
                             where goal.EducationOrganizationId == request.SchoolId
                             select goal);

            foreach(var goalPlanning in goalPlanningQuery)
            {
                var goal = goalQuery.SingleOrDefault(x => x.EducationOrganizationId == goalPlanning.EducationOrganizationId && x.MetricId == goalPlanning.MetricId);
                if (goal == null)
                {
                    goal = new EducationOrganizationGoal
                                      {
                                          EducationOrganizationId = goalPlanning.EducationOrganizationId,
                                          MetricId = goalPlanning.MetricId,
                                          Goal = goalPlanning.Goal,
                                          IsUpdated = true
                                      };
                }
                else
                {
                    goal.Goal = goalPlanning.Goal;
                    goal.IsUpdated = true;
                }
                goalRepository.Save(goal);
            }

            goalPlanningRepository.Delete(x => x.EducationOrganizationId == request.SchoolId);

            return new PublishGoalsModel();
        }
    }
}
