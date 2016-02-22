using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Application.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Application.Resources.LocalEducationAgency
{
    public class PublishGoalsRequest
    {
        public int LocalEducationAgencyId { get; set; }

        [AuthenticationIgnore("Indicates if planned goals for all education organizations associated with this local education agency should be published")]
        public bool PublishAllLocalEducationAgencyGoals { get; set; }

        public static PublishGoalsRequest Create(int localEducationAgencyId, bool publishAllLocalEducationAgencyGoals)
        {
            var request = new PublishGoalsRequest
            {
                LocalEducationAgencyId = localEducationAgencyId,
                PublishAllLocalEducationAgencyGoals = publishAllLocalEducationAgencyGoals
            };

            return request;
        }
    }

    public interface IPublishGoalsService : IPostHandler<PublishGoalsRequest, PublishGoalsModel> { }

    public class PublishGoalsService : IPublishGoalsService
    {
        private readonly IPersistingRepository<EducationOrganizationGoalPlanning> goalPlanningRepository;
        private readonly IPersistingRepository<EducationOrganizationGoal> goalRepository;
        private readonly IRepository<SchoolInformation> schoolRepository;

        public PublishGoalsService(IPersistingRepository<EducationOrganizationGoalPlanning> goalPlanningRepository, IPersistingRepository<EducationOrganizationGoal> goalRepository, IRepository<SchoolInformation> schoolRepository)
        {
            this.goalPlanningRepository = goalPlanningRepository;
            this.goalRepository = goalRepository;
            this.schoolRepository = schoolRepository;
        }

        [NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.ManageGoals)]
        public PublishGoalsModel Post(PublishGoalsRequest request)
        {
            var goalPlanningQuery = from goal in goalPlanningRepository.GetAll()
                                    where goal.EducationOrganizationId == request.LocalEducationAgencyId
                                    select goal;

            var goalQuery = from goal in goalRepository.GetAll()
                            where goal.EducationOrganizationId == request.LocalEducationAgencyId
                            select goal;

            IEnumerable<int> edOrgIds = null;
            if (request.PublishAllLocalEducationAgencyGoals)
            {
                var q = (from school in schoolRepository.GetAll()
                                 where school.LocalEducationAgencyId == request.LocalEducationAgencyId
                                 select school.SchoolId).ToList();
                q.Add(request.LocalEducationAgencyId);

                edOrgIds = q.AsEnumerable();

                goalPlanningQuery = from goal in goalPlanningRepository.GetAll()
                                    where edOrgIds.Contains(goal.EducationOrganizationId)
                                    select goal;

                goalQuery = from goal in goalRepository.GetAll()
                            where edOrgIds.Contains(goal.EducationOrganizationId)
                            select goal;
            }

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

            if (!request.PublishAllLocalEducationAgencyGoals)
                goalPlanningRepository.Delete(x => x.EducationOrganizationId == request.LocalEducationAgencyId);
            else
                goalPlanningRepository.Delete(x => edOrgIds.Contains(x.EducationOrganizationId));

            return new PublishGoalsModel();
        }
    }
}
