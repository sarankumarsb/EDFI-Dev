using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Application.Resources.Models.LocalEducationAgency.Detail;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Application.Resources.LocalEducationAgency.Detail
{
    public class GoalPlanningSchoolListGetRequest
    {
        public int LocalEducationAgencyId { get; set; }

        [AuthenticationIgnore("The school/metric pairs to return")]
        public IEnumerable<SchoolMetric> SchoolMetrics { get; set; }

        public class SchoolMetric
        {
            public int SchoolId { get; set; }
            public int MetricId { get; set; }
        }

        public static GoalPlanningSchoolListGetRequest Create(int localEducationAgencyId, IEnumerable<SchoolMetric> schoolMetrics)
        {
            var request =  new GoalPlanningSchoolListGetRequest
                               {
                                   LocalEducationAgencyId = localEducationAgencyId,
                                   SchoolMetrics = schoolMetrics ?? new List<SchoolMetric>()
                               };

            return request;
        }
    }

    public class GoalPlanningSchoolListPostRequest
    {
        public int LocalEducationAgencyId { get; set; }

        [AuthenticationIgnore("The school/metric pairs to return")]
        public IEnumerable<GoalPlanningSchoolListGetRequest.SchoolMetric> SchoolMetrics { get; set; }

        [AuthenticationIgnore("The goals to update")]
        public IEnumerable<GoalPlanningAction> GoalPlanningActions { get; set; }

        public class GoalPlanningAction
        {
            public PostAction Action { get; set; }
            public int EducationOrganizationId { get; set; }
            public int MetricId { get; set; }
            public decimal Goal { get; set; }
        }

        public static GoalPlanningSchoolListPostRequest Create(int localEducationAgencyId, IEnumerable<GoalPlanningSchoolListGetRequest.SchoolMetric> schoolMetrics, IEnumerable<GoalPlanningAction> goalPlanningActions)
        {
            var request = new GoalPlanningSchoolListPostRequest
                                {
                                    LocalEducationAgencyId = localEducationAgencyId,
                                    SchoolMetrics = schoolMetrics,
                                    GoalPlanningActions = goalPlanningActions
                                };

            if (schoolMetrics == null)
                request.SchoolMetrics = new List<GoalPlanningSchoolListGetRequest.SchoolMetric>();

            if (request.GoalPlanningActions == null)
                request.GoalPlanningActions = new List<GoalPlanningAction>();

            return request;
        }
    }

    public interface IGoalPlanningSchoolListService : IService<GoalPlanningSchoolListGetRequest, GoalPlanningSchoolListModel>,
                                                      IPostHandler<GoalPlanningSchoolListPostRequest, GoalPlanningSchoolListModel> { }

    public class GoalPlanningSchoolListService : IGoalPlanningSchoolListService
    {
        private readonly IPersistingRepository<EducationOrganizationGoalPlanning> goalPlanningRepository;
        private readonly IRepository<EducationOrganizationGoal> goalRepository;
        private readonly IRepository<SchoolInformation> schoolRepository;

        public GoalPlanningSchoolListService(IPersistingRepository<EducationOrganizationGoalPlanning> goalPlanningRepository, IRepository<EducationOrganizationGoal> goalRepository, IRepository<SchoolInformation> schoolRepository)
        {
            this.goalPlanningRepository = goalPlanningRepository;
            this.goalRepository = goalRepository;
            this.schoolRepository = schoolRepository;
        }

        [NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.ManageGoals)]
        public GoalPlanningSchoolListModel Get(GoalPlanningSchoolListGetRequest request)
        {
            // get the schools for this LEA
            var schoolIds = (from school in schoolRepository.GetAll()
                             where school.LocalEducationAgencyId == request.LocalEducationAgencyId
                             select school.SchoolId).AsEnumerable();

            // get the planned goals 
            var goalPlanning = GetGoalPlanningQuery(schoolIds, request.SchoolMetrics);
            var goalQuery = GetGoalQuery(schoolIds, request.SchoolMetrics);

            var result = new GoalPlanningSchoolListModel
                            {
                                ProposedGoals = goalPlanning.ToList().Select(x => new ProposedGoal
                                                                                        {
                                                                                            EducationOrganizationId = x.EducationOrganizationId,
                                                                                            MetricId = x.MetricId,
                                                                                            Goal = x.Goal
                                                                                        }),
                                PublishedGoals = goalQuery.ToList().Select(x => new PublishedGoal
                                                                                    {
                                                                                        EducationOrganizationId = x.EducationOrganizationId,
                                                                                        MetricId = x.MetricId,
                                                                                        Goal = x.Goal,
                                                                                        DisplayGoal = String.Format("{0:P1}", x.Goal)
                                                                                    })
                            };

            return result;
        }

        [NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.ManageGoals)]
        public GoalPlanningSchoolListModel Post(GoalPlanningSchoolListPostRequest request)
        {
            // get the schools for this LEA
            var schoolIds = (from school in schoolRepository.GetAll()
                             where school.LocalEducationAgencyId == request.LocalEducationAgencyId
                             select school.SchoolId).AsEnumerable();

            // get the planned goals 
            var goalPlanning = GetGoalPlanningQuery(schoolIds, request.SchoolMetrics);
            var existingGoals = goalPlanning.ToList();

            foreach (var goalPlanningAction in request.GoalPlanningActions)
            {
                if (!schoolIds.Contains(goalPlanningAction.EducationOrganizationId))
                    continue;

                switch (goalPlanningAction.Action)
                {
                    case PostAction.Add:
                    case PostAction.Set:
                        SetGoal(goalPlanningAction, existingGoals);
                        break;

                    case PostAction.Remove:
                        DeleteGoal(goalPlanningAction);
                        break;
                }
            }

            return Get(GoalPlanningSchoolListGetRequest.Create(request.LocalEducationAgencyId, request.SchoolMetrics));
        }

        private void SetGoal(GoalPlanningSchoolListPostRequest.GoalPlanningAction request, IEnumerable<EducationOrganizationGoalPlanning> existingGoals)
        {
            var goal = existingGoals.SingleOrDefault(x => x.EducationOrganizationId == request.EducationOrganizationId && x.MetricId == request.MetricId);
            if (goal == null)
            {
                goal = new EducationOrganizationGoalPlanning
                           {
                               EducationOrganizationId = request.EducationOrganizationId,
                               MetricId = request.MetricId,
                               Goal = request.Goal
                           };
            }
            else
            {
                goal.Goal = request.Goal;
            }
            goalPlanningRepository.Save(goal);
        }

        private void DeleteGoal(GoalPlanningSchoolListPostRequest.GoalPlanningAction request)
        {
            goalPlanningRepository.Delete(x => x.EducationOrganizationId == request.EducationOrganizationId && x.MetricId == request.MetricId);
        }
    
        private IQueryable<EducationOrganizationGoalPlanning> GetGoalPlanningQuery(IEnumerable<int> schoolIds, IEnumerable<GoalPlanningSchoolListGetRequest.SchoolMetric> schoolMetrics)
        {
            var goalPlanning = from goal in goalPlanningRepository.GetAll()
                               where schoolIds.Contains(goal.EducationOrganizationId)
                               select goal;

            goalPlanning = goalPlanning.Where(GetExpression<EducationOrganizationGoalPlanning>(schoolMetrics));
            return goalPlanning;
        }

        private IQueryable<EducationOrganizationGoal> GetGoalQuery(IEnumerable<int> schoolIds, IEnumerable<GoalPlanningSchoolListGetRequest.SchoolMetric> schoolMetrics)
        {
            var goalQuery = from goal in goalRepository.GetAll()
                            where schoolIds.Contains(goal.EducationOrganizationId)
                                  && goal.IsUpdated == true
                            select goal;

            goalQuery = goalQuery.Where(GetExpression<EducationOrganizationGoal>(schoolMetrics));
            return goalQuery;
        }

        private static Expression<Func<T, bool>> GetExpression<T>(IEnumerable<GoalPlanningSchoolListGetRequest.SchoolMetric> schoolMetrics)
        {
            Expression goalExpression = Expression.Constant(false);
            ParameterExpression educationOrganizationGoalParameter = Expression.Parameter(typeof(T), "goal");
            foreach (var schoolMetric in schoolMetrics)
            {
                goalExpression = Expression.OrElse(Expression.And(Expression.Equal(Expression.Property(educationOrganizationGoalParameter, "EducationOrganizationId"), Expression.Constant(schoolMetric.SchoolId)),
                                                                          Expression.Equal(Expression.Property(educationOrganizationGoalParameter, "MetricId"), Expression.Constant(schoolMetric.MetricId))),
                                                   goalExpression);
            }
            return Expression.Lambda<Func<T, bool>>(goalExpression, educationOrganizationGoalParameter);
        }
    }
}
