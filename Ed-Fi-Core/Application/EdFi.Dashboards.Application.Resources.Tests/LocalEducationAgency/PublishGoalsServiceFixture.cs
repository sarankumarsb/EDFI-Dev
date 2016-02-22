using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Application.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Application.Resources.LocalEducationAgency;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Testing;
using Moq;
using NUnit.Framework;

namespace EdFi.Dashboards.Application.Resources.Tests.LocalEducationAgency
{
    public class PublishGoalsServiceFixture : TestFixtureBase
    {
        protected Mock<IPersistingRepository<EducationOrganizationGoalPlanning>> goalPlanningRepository = new Mock<IPersistingRepository<EducationOrganizationGoalPlanning>>();
        protected Mock<IPersistingRepository<EducationOrganizationGoal>> goalRepository = new Mock<IPersistingRepository<EducationOrganizationGoal>>();
        protected Mock<IRepository<SchoolInformation>> schoolRepository = new Mock<IRepository<SchoolInformation>>();
        protected PublishGoalsRequest request;
        protected PublishGoalsModel result;
        protected List<EducationOrganizationGoal> postedEducationOrganizationGoal = new List<EducationOrganizationGoal>();
        protected List<Expression<Func<EducationOrganizationGoalPlanning, bool>>> deletedEducationOrganizationGoalPlanning = new List<Expression<Func<EducationOrganizationGoalPlanning, bool>>>();

        protected int suppliedLocalEducationAgencyId = 1000;
        protected int suppliedSchoolId = 1001;
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

        protected override void EstablishContext()
        {
            goalPlanningRepository.Setup(x => x.GetAll()).Returns(GetEducationOrganizationGoalPlanning());
            goalRepository.Setup(x => x.GetAll()).Returns(GetEducationOrganizationGoal());
            goalRepository.Setup(x => x.Save(It.IsAny<EducationOrganizationGoal>())).Callback<EducationOrganizationGoal>(x => postedEducationOrganizationGoal.Add(x));
            goalPlanningRepository.Setup(x => x.Delete(It.IsAny<Expression<Func<EducationOrganizationGoalPlanning, bool>>>())).Callback<Expression<Func<EducationOrganizationGoalPlanning, bool>>>(x => deletedEducationOrganizationGoalPlanning.Add(x));

            base.EstablishContext();
        }

        protected IQueryable<EducationOrganizationGoalPlanning> GetEducationOrganizationGoalPlanning()
        {
            var list = new List<EducationOrganizationGoalPlanning>
                            {
                                new EducationOrganizationGoalPlanning{ EducationOrganizationId = suppliedLocalEducationAgencyId, MetricId = suppliedMetricId1, Goal = suppliedGoalPlanning1},
                                new EducationOrganizationGoalPlanning{ EducationOrganizationId = suppliedLocalEducationAgencyId, MetricId = suppliedMetricId2, Goal = suppliedGoalPlanning2},
                                new EducationOrganizationGoalPlanning{ EducationOrganizationId = suppliedSchoolId, MetricId = suppliedMetricId3, Goal = suppliedGoalPlanning3},
                                new EducationOrganizationGoalPlanning{ EducationOrganizationId = suppliedSchoolId, MetricId = suppliedMetricId4, Goal = suppliedGoalPlanning4},
                                new EducationOrganizationGoalPlanning{ EducationOrganizationId = suppliedLocalEducationAgencyId + 100, MetricId = 9999, Goal = 9999m},
                            };
            return list.AsQueryable();
        }

        protected IQueryable<EducationOrganizationGoal> GetEducationOrganizationGoal()
        {
            var list = new List<EducationOrganizationGoal>
                           {
                                new EducationOrganizationGoal{ EducationOrganizationGoalId = suppliedEducationOrganizationGoalId1, EducationOrganizationId = suppliedLocalEducationAgencyId, MetricId = suppliedMetricId1, Goal = suppliedGoal1},
                                new EducationOrganizationGoal{ EducationOrganizationGoalId = suppliedEducationOrganizationGoalId2, EducationOrganizationId = suppliedSchoolId, MetricId = suppliedMetricId3, Goal = suppliedGoal3},
                                new EducationOrganizationGoal{ EducationOrganizationGoalId = 999, EducationOrganizationId = suppliedLocalEducationAgencyId + 100, MetricId = suppliedMetricId1, Goal = 99m},
                           };
            return list.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new PublishGoalsService(goalPlanningRepository.Object, goalRepository.Object, schoolRepository.Object);
            result = service.Post(request);
        }
    }

    public class When_publishing_local_education_agency_goals : PublishGoalsServiceFixture
    {
        protected override void EstablishContext()
        {
            request = PublishGoalsRequest.Create(suppliedLocalEducationAgencyId, false);
            base.EstablishContext();
        }

        [Test]
        public void Should_publish_correct_goals()
        {
            Assert.That(postedEducationOrganizationGoal.Count, Is.EqualTo(2));
            var educationOrganizationGoal = postedEducationOrganizationGoal.Single(x => x.MetricId == suppliedMetricId1);
            Assert.That(educationOrganizationGoal.EducationOrganizationId, Is.EqualTo(suppliedLocalEducationAgencyId));
            Assert.That(educationOrganizationGoal.MetricId, Is.EqualTo(suppliedMetricId1));
            Assert.That(educationOrganizationGoal.Goal, Is.EqualTo(suppliedGoalPlanning1));
            Assert.That(educationOrganizationGoal.EducationOrganizationGoalId, Is.EqualTo(suppliedEducationOrganizationGoalId1));
            
            educationOrganizationGoal = postedEducationOrganizationGoal.Single(x => x.MetricId == suppliedMetricId2);
            Assert.That(educationOrganizationGoal.EducationOrganizationId, Is.EqualTo(suppliedLocalEducationAgencyId));
            Assert.That(educationOrganizationGoal.MetricId, Is.EqualTo(suppliedMetricId2));
            Assert.That(educationOrganizationGoal.Goal, Is.EqualTo(suppliedGoalPlanning2));
            Assert.That(educationOrganizationGoal.EducationOrganizationGoalId, Is.EqualTo(0));
        }
    
        [Test]
        public void Should_delete_old_goal_planning()
        {
            Assert.That(deletedEducationOrganizationGoalPlanning.Count, Is.EqualTo(1));   
        }
    }

    public class When_publishing_goals_for_all_education_organizations_in_a_location_education_agency : PublishGoalsServiceFixture
    {
        protected override void EstablishContext()
        {
            request = PublishGoalsRequest.Create(suppliedLocalEducationAgencyId, true);

            schoolRepository.Setup(x => x.GetAll()).Returns(GetSchoolInformation());

            base.EstablishContext();
        }
        
        protected IQueryable<SchoolInformation> GetSchoolInformation()
        {
            var list = new List<SchoolInformation>
                           {
                               new SchoolInformation{ SchoolId = suppliedSchoolId, LocalEducationAgencyId = suppliedLocalEducationAgencyId},
                               new SchoolInformation{ SchoolId = suppliedSchoolId + 1, LocalEducationAgencyId = suppliedLocalEducationAgencyId}
                           };
            return list.AsQueryable();
        }

        [Test]
        public void Should_publish_correct_goals()
        {
            Assert.That(postedEducationOrganizationGoal.Count, Is.EqualTo(4));
            var educationOrganizationGoal = postedEducationOrganizationGoal.Single(x => x.MetricId == suppliedMetricId1);
            Assert.That(educationOrganizationGoal.EducationOrganizationId, Is.EqualTo(suppliedLocalEducationAgencyId));
            Assert.That(educationOrganizationGoal.MetricId, Is.EqualTo(suppliedMetricId1));
            Assert.That(educationOrganizationGoal.Goal, Is.EqualTo(suppliedGoalPlanning1));
            Assert.That(educationOrganizationGoal.EducationOrganizationGoalId, Is.EqualTo(suppliedEducationOrganizationGoalId1));

            educationOrganizationGoal = postedEducationOrganizationGoal.Single(x => x.MetricId == suppliedMetricId2);
            Assert.That(educationOrganizationGoal.EducationOrganizationId, Is.EqualTo(suppliedLocalEducationAgencyId));
            Assert.That(educationOrganizationGoal.MetricId, Is.EqualTo(suppliedMetricId2));
            Assert.That(educationOrganizationGoal.Goal, Is.EqualTo(suppliedGoalPlanning2));
            Assert.That(educationOrganizationGoal.EducationOrganizationGoalId, Is.EqualTo(0));

            educationOrganizationGoal = postedEducationOrganizationGoal.Single(x => x.MetricId == suppliedMetricId3);
            Assert.That(educationOrganizationGoal.EducationOrganizationId, Is.EqualTo(suppliedSchoolId));
            Assert.That(educationOrganizationGoal.MetricId, Is.EqualTo(suppliedMetricId3));
            Assert.That(educationOrganizationGoal.Goal, Is.EqualTo(suppliedGoalPlanning3));
            Assert.That(educationOrganizationGoal.EducationOrganizationGoalId, Is.EqualTo(suppliedEducationOrganizationGoalId2));

            educationOrganizationGoal = postedEducationOrganizationGoal.Single(x => x.MetricId == suppliedMetricId4);
            Assert.That(educationOrganizationGoal.EducationOrganizationId, Is.EqualTo(suppliedSchoolId));
            Assert.That(educationOrganizationGoal.MetricId, Is.EqualTo(suppliedMetricId4));
            Assert.That(educationOrganizationGoal.Goal, Is.EqualTo(suppliedGoalPlanning4));
            Assert.That(educationOrganizationGoal.EducationOrganizationGoalId, Is.EqualTo(0));
        }

        [Test]
        public void Should_delete_old_goal_planning()
        {
            Assert.That(deletedEducationOrganizationGoalPlanning.Count, Is.EqualTo(1));
        }
    }
}
