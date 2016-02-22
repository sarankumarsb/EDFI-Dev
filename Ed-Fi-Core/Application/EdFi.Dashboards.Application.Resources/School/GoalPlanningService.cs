using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Application.Resources.Models.School;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Application.Resources.School
{
    public class GoalPlanningGetRequest
    {
        public int SchoolId { get; set; }

        public int? MetricVariantId { get; set; }

        public static GoalPlanningGetRequest Create(int schoolId, int? metricVariantId)
        {
            var request = new GoalPlanningGetRequest
                               {
                                   SchoolId = schoolId,
                                   MetricVariantId = metricVariantId
                               };

            return request;
        }
    }

    public class GoalPlanningPostRequest
    {
        public int SchoolId { get; set; }

        public int? MetricVariantId { get; set; }

        [AuthenticationIgnore("The goals to update")]
        public IEnumerable<GoalPlanningAction> GoalPlanningActions { get; set; }

        public class GoalPlanningAction
        {
            public PostAction Action { get; set; }
            public int MetricId { get; set; }
            public decimal Goal { get; set; }
        }

        public static GoalPlanningPostRequest Create(int schoolId, int? metricVariantId, IEnumerable<GoalPlanningAction> goalPlanningActions)
        {
            var request = new GoalPlanningPostRequest
                                {
                                    SchoolId = schoolId,
                                    MetricVariantId = metricVariantId,
                                    GoalPlanningActions = goalPlanningActions
                                };

            if (request.GoalPlanningActions == null)
                request.GoalPlanningActions = new List<GoalPlanningAction>();

            return request;
        }
    
    }

    public interface IGoalPlanningService : IService<GoalPlanningGetRequest, GoalPlanningModel>,
                                            IPostHandler<GoalPlanningPostRequest, GoalPlanningModel> { }

    public class GoalPlanningService : IGoalPlanningService
    {
        private readonly IPersistingRepository<EducationOrganizationGoalPlanning> goalPlanningRepository;
        private readonly IRepository<EducationOrganizationGoal> goalRepository;
        private readonly IRootMetricNodeResolver rootMetricNodeResolver;
        private readonly IDomainMetricService<SchoolMetricInstanceSetRequest> domainMetricService;
        private readonly IMetricNodeResolver metricNodeResolver;

        public GoalPlanningService(IPersistingRepository<EducationOrganizationGoalPlanning> goalPlanningRepository, 
                                    IRepository<EducationOrganizationGoal> goalRepository, 
                                    IRootMetricNodeResolver rootMetricNodeResolver, 
                                    IDomainMetricService<SchoolMetricInstanceSetRequest> domainMetricService,
                                    IMetricNodeResolver metricNodeResolver)
        {
            this.goalPlanningRepository = goalPlanningRepository;
            this.goalRepository = goalRepository;
            this.rootMetricNodeResolver = rootMetricNodeResolver;
            this.domainMetricService = domainMetricService;
            this.metricNodeResolver = metricNodeResolver;
        }

        [NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.ManageGoals)]
        public GoalPlanningModel Get(GoalPlanningGetRequest request)
        {
            var metricVariantId = request.MetricVariantId;
            if (!metricVariantId.HasValue)
                metricVariantId = rootMetricNodeResolver.GetRootMetricNode().MetricVariantId;
            var metricTree = domainMetricService.Get(SchoolMetricInstanceSetRequest.Create(request.SchoolId, metricVariantId.Value));
            var containerMetric = metricTree.RootNode as ContainerMetric;
            IEnumerable<int> metricIds = containerMetric != null ? containerMetric.DescendantsOrSelf.Where(x => x.MetricType == MetricType.GranularMetric).Select(x => x.MetricId)
                                                                 : new List<int> { metricTree.RootNode.MetricId };

            var goalPlanningQuery = from goal in goalPlanningRepository.GetAll()
                        where goal.EducationOrganizationId == request.SchoolId
                            && metricIds.Contains(goal.MetricId)
                        select goal;

            var goalQuery = from goal in goalRepository.GetAll()
                            where goal.EducationOrganizationId == request.SchoolId
                                && metricIds.Contains(goal.MetricId)
                                && goal.IsUpdated == true
                            select goal;

            var publishedGoals = new List<PublishedGoal>();
            foreach (var publishedGoalData in goalQuery)
            {
                dynamic metric = containerMetric != null ? containerMetric.DescendantsOrSelf.SingleOrDefault(x => x.MetricId == publishedGoalData.MetricId && x.MetricVariantType == MetricVariantType.CurrentYear) : metricTree.RootNode;
                if (metric == null)
                    continue;

                var format = metric.Values["GoalFormat"] as string ?? "{0:P1}";
                var publishedGoal = new PublishedGoal
                                        {
                                            EducationOrganizationId = publishedGoalData.EducationOrganizationId,
                                            MetricId = publishedGoalData.MetricId,
                                            Goal = publishedGoalData.Goal,
                                            DisplayGoal = String.Format(format, publishedGoalData.Goal),
                                        };
                
                if (metric.Goal.Interpretation == TrendInterpretation.Standard)
                    publishedGoal.GoalDifference = Convert.ToDecimal(metric.Value) - publishedGoal.Goal;
                else
                    publishedGoal.GoalDifference = publishedGoal.Goal - Convert.ToDecimal(metric.Value);
                publishedGoal.DisplayGoalDifference = String.Format(format, publishedGoal.GoalDifference);
                publishedGoals.Add(publishedGoal);
            }

            var result = new GoalPlanningModel
            {
                ProposedGoals = goalPlanningQuery.ToList().Select(x => new ProposedGoal
                                                                            {
                                                                                EducationOrganizationId = x.EducationOrganizationId,
                                                                                MetricId = x.MetricId,
                                                                                Goal = x.Goal
                                                                            }),
                PublishedGoals = publishedGoals
            };

            return result;
        }

        [NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.ManageGoals)]
        public GoalPlanningModel Post(GoalPlanningPostRequest request)
        {
            var existingGoals = goalPlanningRepository.GetAll().ToList();

            foreach (var goalPlanningAction in request.GoalPlanningActions)
            {
                switch (goalPlanningAction.Action)
                {
                    case PostAction.Add:
                    case PostAction.Set:
                        SetGoal(goalPlanningAction, request.SchoolId, existingGoals);
                        break;

                    case PostAction.Remove:
                        DeleteGoal(goalPlanningAction, request.SchoolId);
                        break;
                }
            }

            return Get(GoalPlanningGetRequest.Create(request.SchoolId, request.MetricVariantId));
        }

        private void SetGoal(GoalPlanningPostRequest.GoalPlanningAction request, int schoolId, IEnumerable<EducationOrganizationGoalPlanning> existingGoals)
        {
            var existingGoal = existingGoals.SingleOrDefault(x => x.EducationOrganizationId == schoolId && x.MetricId == request.MetricId);
            
            if (existingGoal == null)
            {
                existingGoal = new EducationOrganizationGoalPlanning
                                    {
                                        EducationOrganizationId = schoolId,
                                        MetricId = request.MetricId,
                                        Goal = request.Goal
                                    };
            }
            else
            {
                existingGoal.Goal = request.Goal;
            }
            goalPlanningRepository.Save(existingGoal);
        }

        private void DeleteGoal(GoalPlanningPostRequest.GoalPlanningAction request, int schoolId)
        {
            goalPlanningRepository.Delete(x => x.EducationOrganizationId == schoolId && x.MetricId == request.MetricId);
        }
    }
}
