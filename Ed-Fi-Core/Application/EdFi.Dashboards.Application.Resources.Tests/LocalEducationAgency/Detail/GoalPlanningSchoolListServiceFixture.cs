using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Application.Resources.LocalEducationAgency.Detail;
using EdFi.Dashboards.Application.Resources.Models.LocalEducationAgency.Detail;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Testing;
using Moq;
using NUnit.Framework;

namespace EdFi.Dashboards.Application.Resources.Tests.LocalEducationAgency.Detail
{
    public abstract class GoalPlanningSchoolListServiceFixture : TestFixtureBase
    {
        protected Mock<IPersistingRepository<EducationOrganizationGoalPlanning>> goalPlanningRepository = new Mock<IPersistingRepository<EducationOrganizationGoalPlanning>>();
        protected Mock<IRepository<EducationOrganizationGoal>> goalRepository = new Mock<IRepository<EducationOrganizationGoal>>();
        protected Mock<IRepository<SchoolInformation>> schoolRepository = new Mock<IRepository<SchoolInformation>>();
        protected GoalPlanningSchoolListModel result;

        protected int suppliedLocalEducationAgencyId = 1000;
        protected int suppliedSchoolId1 = 1001;
        protected int suppliedSchoolId2 = 1002;
        protected int suppliedSchoolId3 = 1003;
        protected int suppliedSchoolId4 = 1004;
        protected int suppliedMetricId1 = 10000;
        protected int suppliedMetricId2 = 10001;
        protected decimal suppliedGoalPlanning1 = .111m;
        protected decimal suppliedGoalPlanning2 = .222m;
        protected int suppliedMetricId3 = 10002;
        protected int suppliedMetricId4 = 10003;
        protected decimal suppliedGoalPlanning3 = .333m;
        protected decimal suppliedGoalPlanning4 = .444m;
        protected decimal suppliedGoal1 = .666m;
        protected decimal suppliedGoal3 = .555m;
        protected int suppliedEducationOrganizationGoalId1 = 1;
        protected int suppliedEducationOrganizationGoalId2 = 2;
        protected int suppliedEducationOrganizationGoalPlanningId1 = 3;
        protected int suppliedEducationOrganizationGoalPlanningId2 = 4;
        protected int suppliedEducationOrganizationGoalPlanningId3 = 5;
        protected int suppliedEducationOrganizationGoalPlanningId4 = 6;
        protected override void EstablishContext()
        {
            goalPlanningRepository.Setup(x => x.GetAll()).Returns(GetEducationOrganizationGoalPlanning());
            goalRepository.Setup(x => x.GetAll()).Returns(GetEducationOrganizationGoal());
            schoolRepository.Setup(x => x.GetAll()).Returns(GetSchoolInformation());
        }

        protected IQueryable<EducationOrganizationGoalPlanning> GetEducationOrganizationGoalPlanning()
        {
            var list = new List<EducationOrganizationGoalPlanning>
                            {
                                new EducationOrganizationGoalPlanning{ EducationOrganizationGoalPlanningId = suppliedEducationOrganizationGoalPlanningId1, EducationOrganizationId = suppliedSchoolId1, MetricId = suppliedMetricId1, Goal = suppliedGoalPlanning1},
                                new EducationOrganizationGoalPlanning{ EducationOrganizationGoalPlanningId = suppliedEducationOrganizationGoalPlanningId2, EducationOrganizationId = suppliedSchoolId1, MetricId = suppliedMetricId2, Goal = suppliedGoalPlanning2},
                                new EducationOrganizationGoalPlanning{ EducationOrganizationGoalPlanningId = suppliedEducationOrganizationGoalPlanningId3, EducationOrganizationId = suppliedSchoolId1, MetricId = suppliedMetricId4, Goal = 999m},
                                new EducationOrganizationGoalPlanning{ EducationOrganizationGoalPlanningId = suppliedEducationOrganizationGoalPlanningId4, EducationOrganizationId = suppliedSchoolId2, MetricId = suppliedMetricId3, Goal = suppliedGoalPlanning3},
                                new EducationOrganizationGoalPlanning{ EducationOrganizationId = suppliedSchoolId4, MetricId = suppliedMetricId3, Goal = 9999m},
                            };
            return list.AsQueryable();
        }

        protected IQueryable<EducationOrganizationGoal> GetEducationOrganizationGoal()
        {
            var list = new List<EducationOrganizationGoal>
                           {
                                new EducationOrganizationGoal{ EducationOrganizationGoalId = suppliedEducationOrganizationGoalId1, EducationOrganizationId = suppliedSchoolId1, MetricId = suppliedMetricId1, Goal = suppliedGoal1, IsUpdated = true},
                                new EducationOrganizationGoal{ EducationOrganizationGoalId = suppliedEducationOrganizationGoalId1, EducationOrganizationId = suppliedSchoolId1, MetricId = suppliedMetricId2, Goal = 999},
                                new EducationOrganizationGoal{ EducationOrganizationGoalId = suppliedEducationOrganizationGoalId2, EducationOrganizationId = suppliedSchoolId2, MetricId = suppliedMetricId3, Goal = suppliedGoal3, IsUpdated = true},
                                new EducationOrganizationGoal{ EducationOrganizationGoalId = 999, EducationOrganizationId = suppliedSchoolId4, MetricId = suppliedMetricId3, Goal = 99m, IsUpdated = true},
                           };
            return list.AsQueryable();
        }

        protected IQueryable<SchoolInformation> GetSchoolInformation()
        {
            var list = new List<SchoolInformation>
                           {
                               new SchoolInformation{ SchoolId = suppliedSchoolId1, LocalEducationAgencyId = suppliedLocalEducationAgencyId},
                               new SchoolInformation{ SchoolId = suppliedSchoolId2, LocalEducationAgencyId = suppliedLocalEducationAgencyId},
                               new SchoolInformation{ SchoolId = suppliedSchoolId3, LocalEducationAgencyId = suppliedLocalEducationAgencyId}
                           };
            return list.AsQueryable();
        }

        protected IEnumerable<GoalPlanningSchoolListGetRequest.SchoolMetric> GetSchoolMetrics()
        {
            var list = new List<GoalPlanningSchoolListGetRequest.SchoolMetric>
                           {
                               new GoalPlanningSchoolListGetRequest.SchoolMetric {SchoolId = suppliedSchoolId1, MetricId = suppliedMetricId1},
                               new GoalPlanningSchoolListGetRequest.SchoolMetric {SchoolId = suppliedSchoolId1, MetricId = suppliedMetricId2},
                               new GoalPlanningSchoolListGetRequest.SchoolMetric {SchoolId = suppliedSchoolId1, MetricId = suppliedMetricId3},
                               new GoalPlanningSchoolListGetRequest.SchoolMetric {SchoolId = suppliedSchoolId2, MetricId = suppliedMetricId3},
                               new GoalPlanningSchoolListGetRequest.SchoolMetric {SchoolId = suppliedSchoolId4, MetricId = suppliedMetricId3},
                           };
            return list;
        }

        [Test]
        public void Should_load_correct_planning_goals()
        {
            Assert.That(result.ProposedGoals.Count(), Is.EqualTo(3));
            var goal = result.ProposedGoals.Single(x => x.EducationOrganizationId == suppliedSchoolId1 && x.MetricId == suppliedMetricId1);
            Assert.That(goal.Goal, Is.EqualTo(suppliedGoalPlanning1));
            goal = result.ProposedGoals.Single(x => x.EducationOrganizationId == suppliedSchoolId1 && x.MetricId == suppliedMetricId2);
            Assert.That(goal.Goal, Is.EqualTo(suppliedGoalPlanning2));
            goal = result.ProposedGoals.Single(x => x.EducationOrganizationId == suppliedSchoolId2 && x.MetricId == suppliedMetricId3);
            Assert.That(goal.Goal, Is.EqualTo(suppliedGoalPlanning3));
        }

        [Test]
        public void Should_load_correct_published_goals()
        {
            Assert.That(result.PublishedGoals.Count(), Is.EqualTo(2));
            var goal = result.PublishedGoals.Single(x => x.EducationOrganizationId == suppliedSchoolId1 && x.MetricId == suppliedMetricId1);
            Assert.That(goal.Goal, Is.EqualTo(suppliedGoal1));
            Assert.That(goal.DisplayGoal, Is.EqualTo(String.Format("{0:P1}", suppliedGoal1)));
            goal = result.PublishedGoals.Single(x => x.EducationOrganizationId == suppliedSchoolId2 && x.MetricId == suppliedMetricId3);
            Assert.That(goal.Goal, Is.EqualTo(suppliedGoal3));
            Assert.That(goal.DisplayGoal, Is.EqualTo(String.Format("{0:P1}", suppliedGoal3)));
        }
    }

    public class When_loading_goal_planning_for_a_school_metric_list : GoalPlanningSchoolListServiceFixture
    {
        protected override void ExecuteTest()
        {
            var service = new GoalPlanningSchoolListService(goalPlanningRepository.Object, goalRepository.Object, schoolRepository.Object);
            result = service.Get(GoalPlanningSchoolListGetRequest.Create(suppliedLocalEducationAgencyId, GetSchoolMetrics()));
        }
    }

    public class When_adding_goal_planning_for_a_school_metric_list : GoalPlanningSchoolListServiceFixture
    {
        protected decimal newGoal1 = .443m;
        protected decimal newGoal2 = .446m;
        protected decimal newGoal3 = .447m;
        protected List<EducationOrganizationGoalPlanning> postedGoalPlanning = new List<EducationOrganizationGoalPlanning>();

        protected override void EstablishContext()
        {
            goalPlanningRepository.Setup(x => x.Save(It.IsAny<EducationOrganizationGoalPlanning>())).Callback<EducationOrganizationGoalPlanning>(x => postedGoalPlanning.Add(x));
            base.EstablishContext();
            suppliedGoalPlanning3 = newGoal2;
        }

        protected override void ExecuteTest()
        {
            var service = new GoalPlanningSchoolListService(goalPlanningRepository.Object, goalRepository.Object, schoolRepository.Object);
            result = service.Post(GoalPlanningSchoolListPostRequest.Create(suppliedLocalEducationAgencyId, GetSchoolMetrics(), GetGoalPlanning()));
        }

        protected IEnumerable<GoalPlanningSchoolListPostRequest.GoalPlanningAction> GetGoalPlanning()
        {
            var list = new List<GoalPlanningSchoolListPostRequest.GoalPlanningAction>
                           {
                               new GoalPlanningSchoolListPostRequest.GoalPlanningAction{Action = PostAction.Add, EducationOrganizationId = suppliedSchoolId1, MetricId = suppliedMetricId3, Goal = newGoal1},
                               new GoalPlanningSchoolListPostRequest.GoalPlanningAction{Action = PostAction.Add, EducationOrganizationId = suppliedSchoolId2, MetricId = suppliedMetricId3, Goal = newGoal2},
                               new GoalPlanningSchoolListPostRequest.GoalPlanningAction{Action = PostAction.Add, EducationOrganizationId = suppliedSchoolId4, MetricId = suppliedMetricId1, Goal = newGoal3},
                           };
            return list;
        }

        [Test]
        public void Should_save_new_goal_planning()
        {
            Assert.That(postedGoalPlanning.Count, Is.EqualTo(2));
            var goal = postedGoalPlanning.Single(x => x.EducationOrganizationId == suppliedSchoolId1);
            Assert.That(goal.Goal, Is.EqualTo(newGoal1));
            Assert.That(goal.EducationOrganizationGoalPlanningId, Is.EqualTo(0));
            goal = postedGoalPlanning.Single(x => x.EducationOrganizationId == suppliedSchoolId2);
            Assert.That(goal.Goal, Is.EqualTo(newGoal2));
            Assert.That(goal.EducationOrganizationGoalPlanningId, Is.EqualTo(suppliedEducationOrganizationGoalPlanningId4));
        }
    }

    public class When_updating_goal_planning_for_a_school_metric_list : GoalPlanningSchoolListServiceFixture
    {
        protected decimal newGoal1 = .443m;
        protected decimal newGoal2 = .446m;
        protected decimal newGoal3 = .447m;
        protected List<EducationOrganizationGoalPlanning> postedGoalPlanning = new List<EducationOrganizationGoalPlanning>();

        protected override void EstablishContext()
        {
            goalPlanningRepository.Setup(x => x.Save(It.IsAny<EducationOrganizationGoalPlanning>())).Callback<EducationOrganizationGoalPlanning>(x => postedGoalPlanning.Add(x));
            base.EstablishContext();
            suppliedGoalPlanning3 = newGoal2;
        }

        protected override void ExecuteTest()
        {
            var service = new GoalPlanningSchoolListService(goalPlanningRepository.Object, goalRepository.Object, schoolRepository.Object);
            result = service.Post(GoalPlanningSchoolListPostRequest.Create(suppliedLocalEducationAgencyId, GetSchoolMetrics(), GetGoalPlanning()));
        }

        protected IEnumerable<GoalPlanningSchoolListPostRequest.GoalPlanningAction> GetGoalPlanning()
        {
            var list = new List<GoalPlanningSchoolListPostRequest.GoalPlanningAction>
                           {
                               new GoalPlanningSchoolListPostRequest.GoalPlanningAction{Action = PostAction.Set, EducationOrganizationId = suppliedSchoolId1, MetricId = suppliedMetricId3, Goal = newGoal1},
                               new GoalPlanningSchoolListPostRequest.GoalPlanningAction{Action = PostAction.Set, EducationOrganizationId = suppliedSchoolId2, MetricId = suppliedMetricId3, Goal = newGoal2},
                               new GoalPlanningSchoolListPostRequest.GoalPlanningAction{Action = PostAction.Set, EducationOrganizationId = suppliedSchoolId4, MetricId = suppliedMetricId1, Goal = newGoal3},
                           };
            return list;
        }

        [Test]
        public void Should_update_goal_planning()
        {
            Assert.That(postedGoalPlanning.Count, Is.EqualTo(2));
            var goal = postedGoalPlanning.Single(x => x.EducationOrganizationId == suppliedSchoolId1);
            Assert.That(goal.Goal, Is.EqualTo(newGoal1));
            Assert.That(goal.EducationOrganizationGoalPlanningId, Is.EqualTo(0));
            goal = postedGoalPlanning.Single(x => x.EducationOrganizationId == suppliedSchoolId2);
            Assert.That(goal.Goal, Is.EqualTo(newGoal2));
            Assert.That(goal.EducationOrganizationGoalPlanningId, Is.EqualTo(suppliedEducationOrganizationGoalPlanningId4));
        }
    }

    public class When_deleting_goal_planning_for_a_school_metric_list : GoalPlanningSchoolListServiceFixture
    {
        private readonly List<Expression<Func<EducationOrganizationGoalPlanning, bool>>> postedGoalPlanning = new List<Expression<Func<EducationOrganizationGoalPlanning, bool>>>();
        protected decimal newGoal1 = .443m;
        protected decimal newGoal2 = .446m;
        protected decimal newGoal3 = .447m;

        protected override void EstablishContext()
        {
            goalPlanningRepository.Setup(x => x.Delete(It.IsAny<Expression<Func<EducationOrganizationGoalPlanning, bool>>>())).Callback<Expression<Func<EducationOrganizationGoalPlanning, bool>>>(x => postedGoalPlanning.Add(x));

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            var service = new GoalPlanningSchoolListService(goalPlanningRepository.Object, goalRepository.Object, schoolRepository.Object);
            result = service.Post(GoalPlanningSchoolListPostRequest.Create(suppliedLocalEducationAgencyId, GetSchoolMetrics(), GetGoalPlanning()));
        }

        protected IEnumerable<GoalPlanningSchoolListPostRequest.GoalPlanningAction> GetGoalPlanning()
        {
            var list = new List<GoalPlanningSchoolListPostRequest.GoalPlanningAction>
                           {
                               new GoalPlanningSchoolListPostRequest.GoalPlanningAction{Action = PostAction.Remove, EducationOrganizationId = suppliedSchoolId1, MetricId = suppliedMetricId3, Goal = newGoal1},
                               new GoalPlanningSchoolListPostRequest.GoalPlanningAction{Action = PostAction.Remove, EducationOrganizationId = suppliedSchoolId2, MetricId = suppliedMetricId3, Goal = newGoal2},
                               new GoalPlanningSchoolListPostRequest.GoalPlanningAction{Action = PostAction.Remove, EducationOrganizationId = suppliedSchoolId4, MetricId = suppliedMetricId1, Goal = newGoal3},
                           };
            return list;
        }

        [Test]
        public void Should_delete_goal_planning()
        {
            Assert.That(postedGoalPlanning.Count, Is.EqualTo(2));
        }
    }
}
