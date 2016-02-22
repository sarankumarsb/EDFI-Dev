using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Application.Resources.School;
using EdFi.Dashboards.Application.Resources.Models.School;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Tests;
using EdFi.Dashboards.Testing;
using Moq;
using NUnit.Framework;

namespace EdFi.Dashboards.Application.Resources.Tests.School
{
    public class GoalPlanningServiceFixtureBase : TestFixtureBase
    {
        protected const int suppliedSchoolId = 1000;
        protected const int suppliedMetricVariantId1 = 19999;
        protected const int suppliedMetricVariantId102 = 19192;
        protected const int suppliedMetricVariantId2 = 29999;
        protected const int suppliedMetricVariantId3 = 39999;
        protected const int suppliedMetricId1 = 10000;
        protected const int suppliedMetricId102 = 10102;
        protected const int suppliedMetricId2 = 20000;
        protected const int suppliedMetricId3 = 30000;
        protected decimal suppliedGoal1 = .444m;
        protected decimal suppliedProposedGoal1 = .555m;
        protected double suppliedValue1 = .666;
        protected int suppliedGoalPlanningId = 123;
        protected decimal suppliedGoal2 = .222m;
        protected decimal suppliedProposedGoal2 = .333m;
        protected double suppliedValue2 = .111;
        protected decimal suppliedGoal3 = .777m;
        protected decimal suppliedProposedGoal3 = .888m;
        protected double suppliedValue3 = .999;

        protected Mock<IPersistingRepository<EducationOrganizationGoalPlanning>> goalPlanningRepository = new Mock<IPersistingRepository<EducationOrganizationGoalPlanning>>();
        protected Mock<IRepository<EducationOrganizationGoal>> goalRepository = new Mock<IRepository<EducationOrganizationGoal>>();
        protected Mock<IRootMetricNodeResolver> rootMetricNodeResolver = new Mock<IRootMetricNodeResolver>();
        protected Mock<IDomainMetricService<SchoolMetricInstanceSetRequest>> domainMetricService = new Mock<IDomainMetricService<SchoolMetricInstanceSetRequest>>();
        protected Mock<IMetricNodeResolver> metricNodeResolver = new Mock<IMetricNodeResolver>();
        protected GoalPlanningGetRequest request;
        protected GoalPlanningModel actualResults;

        protected override void EstablishContext()
        {
            goalPlanningRepository.Setup(x => x.GetAll()).Returns(GetSuppliedEducationOrganizationGoalPlanning());
            goalRepository.Setup(x => x.GetAll()).Returns(GetSuppliedEducationOrganizationGoals());
            base.EstablishContext();
        }

        protected IQueryable<EducationOrganizationGoalPlanning> GetSuppliedEducationOrganizationGoalPlanning()
        {
            var list = new List<EducationOrganizationGoalPlanning>
                           {
                               new EducationOrganizationGoalPlanning { EducationOrganizationId = suppliedSchoolId, MetricId = suppliedMetricId1, Goal = suppliedProposedGoal1, EducationOrganizationGoalPlanningId = suppliedGoalPlanningId },
                               new EducationOrganizationGoalPlanning { EducationOrganizationId = suppliedSchoolId, MetricId = suppliedMetricId2, Goal = suppliedProposedGoal2, EducationOrganizationGoalPlanningId = 2 },
                               new EducationOrganizationGoalPlanning { EducationOrganizationId = suppliedSchoolId, MetricId = suppliedMetricId3, Goal = suppliedProposedGoal3, EducationOrganizationGoalPlanningId = 3 },
                               new EducationOrganizationGoalPlanning { EducationOrganizationId = suppliedSchoolId + 1, MetricId = suppliedMetricId1, Goal = .4m, EducationOrganizationGoalPlanningId = 9999 },
                               new EducationOrganizationGoalPlanning { EducationOrganizationId = suppliedSchoolId, MetricId = suppliedMetricId1 + 123, Goal = .4m, EducationOrganizationGoalPlanningId = 9999 },
                           };
            return list.AsQueryable();
        }

        protected IQueryable<EducationOrganizationGoal> GetSuppliedEducationOrganizationGoals()
        {
            var list = new List<EducationOrganizationGoal>
                           {
                               new EducationOrganizationGoal { EducationOrganizationId = suppliedSchoolId, MetricId = suppliedMetricId1, Goal = suppliedGoal1, IsUpdated = true, EducationOrganizationGoalId = 1 },
                               new EducationOrganizationGoal { EducationOrganizationId = suppliedSchoolId, MetricId = suppliedMetricId2, Goal = suppliedGoal2, IsUpdated = true, EducationOrganizationGoalId = 2 },
                               new EducationOrganizationGoal { EducationOrganizationId = suppliedSchoolId, MetricId = suppliedMetricId3, Goal = suppliedGoal3, IsUpdated = true, EducationOrganizationGoalId = 3 },
                               new EducationOrganizationGoal { EducationOrganizationId = suppliedSchoolId + 1, MetricId = suppliedMetricId1, Goal = .4m, IsUpdated = true, EducationOrganizationGoalId = 9999 },
                               new EducationOrganizationGoal { EducationOrganizationId = suppliedSchoolId, MetricId = suppliedMetricId1 + 123, Goal = .4m, IsUpdated = true, EducationOrganizationGoalId = 9999 },
                               new EducationOrganizationGoal { EducationOrganizationId = suppliedSchoolId, MetricId = suppliedMetricId1, Goal = .12345m, IsUpdated = false, EducationOrganizationGoalId = 9999 },
                           };
            return list.AsQueryable();
        }

        protected MetricTree GetMetricTree()
        {
            return new MetricTree(
                new ContainerMetric
            {
                MetricId = suppliedMetricId1 + 100,
                MetricVariantId = suppliedMetricVariantId1 + 10000,
                MetricVariantType = MetricVariantType.CurrentYear,
                Children = new List<MetricBase>
                                          {
                                              new ContainerMetric
                                                  {
                                                      MetricId = suppliedMetricId102,
                                                      MetricVariantId = suppliedMetricVariantId102,
                                                      MetricVariantType = MetricVariantType.CurrentYear,
                                                      Children = new List<MetricBase>
                                                                     {
                                                                         new ContainerMetric
                                                                              {
                                                                                  MetricId = suppliedMetricId1 + 101,
                                                                                  MetricVariantId = suppliedMetricVariantId1 + 10100,
                                                                                  MetricVariantType = MetricVariantType.CurrentYear,
                                                                                  Children = new List<MetricBase>
                                                                                                 {
                                                                                                     new GranularMetric<double>
                                                                                                         {
                                                                                                             MetricId = suppliedMetricId2,
                                                                                                             MetricVariantId = suppliedMetricVariantId2,
                                                                                                             MetricVariantType = MetricVariantType.CurrentYear,
                                                                                                             Goal = new Goal{ Interpretation = TrendInterpretation.Inverse, Value = .0001m},
                                                                                                             Value = suppliedValue2
                                                                                                         }
                                                                                                 }
                                                                              },
                                                                            new GranularMetric<double>
                                                                                {
                                                                                    MetricId = suppliedMetricId1,
                                                                                    MetricVariantId = suppliedMetricVariantId1,
                                                                                    MetricVariantType = MetricVariantType.CurrentYear,
                                                                                    Goal = new Goal{ Interpretation = TrendInterpretation.Standard, Value = .0001m},
                                                                                    Value = suppliedValue1
                                                                                },
                                                                                
                                                                            new GranularMetric<double>
                                                                                {
                                                                                    MetricId = suppliedMetricId1,
                                                                                    MetricVariantId = suppliedMetricVariantId1 + 100000,
                                                                                    MetricVariantType = MetricVariantType.PriorYear,
                                                                                    Goal = new Goal{ Interpretation = TrendInterpretation.Standard, Value = .00333m},
                                                                                    Value = suppliedValue1 + 999
                                                                                }
                                                                     }
                                                  },
                                                new GranularMetric<double>
                                                    {
                                                        MetricId = suppliedMetricId3,
                                                        MetricVariantId = suppliedMetricVariantId3,
                                                        MetricVariantType = MetricVariantType.CurrentYear,
                                                        Goal = new Goal{ Interpretation = TrendInterpretation.Standard, Value = .0001m},
                                                        Value = suppliedValue3
                                                    }
                                              
                                          }
            });
        }

        protected override void ExecuteTest()
        {
            var service = new GoalPlanningService(goalPlanningRepository.Object, goalRepository.Object, rootMetricNodeResolver.Object, domainMetricService.Object, metricNodeResolver.Object);
            actualResults = service.Get(request);
        }
    }

    public class When_loading_all_goal_planning_for_a_school : GoalPlanningServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            rootMetricNodeResolver.Setup(x => x.GetRootMetricNode()).Returns(new MetricMetadataNode(new TestMetricMetadataTree()) { MetricId = 888, MetricVariantId = 88800 });
            domainMetricService.Setup(x => x.Get(It.Is<SchoolMetricInstanceSetRequest>(i => i.SchoolId == suppliedSchoolId && i.MetricVariantId == 88800)))
                                            .Returns(GetMetricTree());

            request = GoalPlanningGetRequest.Create(suppliedSchoolId, null);
            base.EstablishContext();
        }

        [Test]
        public void Should_load_correct_proposed_goals()
        {
            Assert.That(actualResults.ProposedGoals.Count(), Is.EqualTo(3));
            var proposedGoal = actualResults.ProposedGoals.Single(x => x.MetricId == suppliedMetricId1);
            Assert.That(proposedGoal.Goal, Is.EqualTo(suppliedProposedGoal1));
            Assert.That(proposedGoal.MetricId, Is.EqualTo(suppliedMetricId1));
            Assert.That(proposedGoal.EducationOrganizationId, Is.EqualTo(suppliedSchoolId));
            proposedGoal = actualResults.ProposedGoals.Single(x => x.MetricId == suppliedMetricId2);
            Assert.That(proposedGoal.Goal, Is.EqualTo(suppliedProposedGoal2));
            Assert.That(proposedGoal.MetricId, Is.EqualTo(suppliedMetricId2));
            Assert.That(proposedGoal.EducationOrganizationId, Is.EqualTo(suppliedSchoolId));
            proposedGoal = actualResults.ProposedGoals.Single(x => x.MetricId == suppliedMetricId3);
            Assert.That(proposedGoal.Goal, Is.EqualTo(suppliedProposedGoal3));
            Assert.That(proposedGoal.MetricId, Is.EqualTo(suppliedMetricId3));
            Assert.That(proposedGoal.EducationOrganizationId, Is.EqualTo(suppliedSchoolId));
        }

        [Test]
        public void Should_load_correct_published_goals()
        {
            Assert.That(actualResults.PublishedGoals.Count(), Is.EqualTo(3));
            var publishedGoal = actualResults.PublishedGoals.Single(x => x.MetricId == suppliedMetricId1);
            Assert.That(publishedGoal.Goal, Is.EqualTo(suppliedGoal1));
            Assert.That(publishedGoal.MetricId, Is.EqualTo(suppliedMetricId1));
            Assert.That(publishedGoal.EducationOrganizationId, Is.EqualTo(suppliedSchoolId));
            Assert.That(publishedGoal.DisplayGoal, Is.EqualTo(String.Format("{0:P1}", suppliedGoal1)));
            var goalDifference = Convert.ToDecimal(suppliedValue1) - suppliedGoal1;
            Assert.That(publishedGoal.GoalDifference, Is.EqualTo(goalDifference));
            Assert.That(publishedGoal.DisplayGoalDifference, Is.EqualTo(String.Format("{0:P1}", goalDifference)));

            publishedGoal = actualResults.PublishedGoals.Single(x => x.MetricId == suppliedMetricId2);
            Assert.That(publishedGoal.Goal, Is.EqualTo(suppliedGoal2));
            Assert.That(publishedGoal.MetricId, Is.EqualTo(suppliedMetricId2));
            Assert.That(publishedGoal.EducationOrganizationId, Is.EqualTo(suppliedSchoolId));
            Assert.That(publishedGoal.DisplayGoal, Is.EqualTo(String.Format("{0:P1}", suppliedGoal2)));
            goalDifference = suppliedGoal2 - Convert.ToDecimal(suppliedValue2);
            Assert.That(publishedGoal.GoalDifference, Is.EqualTo(goalDifference));
            Assert.That(publishedGoal.DisplayGoalDifference, Is.EqualTo(String.Format("{0:P1}", goalDifference)));

            publishedGoal = actualResults.PublishedGoals.Single(x => x.MetricId == suppliedMetricId3);
            Assert.That(publishedGoal.Goal, Is.EqualTo(suppliedGoal3));
            Assert.That(publishedGoal.MetricId, Is.EqualTo(suppliedMetricId3));
            Assert.That(publishedGoal.EducationOrganizationId, Is.EqualTo(suppliedSchoolId));
            Assert.That(publishedGoal.DisplayGoal, Is.EqualTo(String.Format("{0:P1}", suppliedGoal3)));
            goalDifference = Convert.ToDecimal(suppliedValue3) - suppliedGoal3;
            Assert.That(publishedGoal.GoalDifference, Is.EqualTo(goalDifference));
            Assert.That(publishedGoal.DisplayGoalDifference, Is.EqualTo(String.Format("{0:P1}", goalDifference)));
        }
    }

    public class When_loading_goal_planning_for_a_container_metric : GoalPlanningServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            var metricTree = GetMetricTree();
            var rootNode = metricTree.RootNode as ContainerMetric;

            domainMetricService.Setup(x => x.Get(It.Is<SchoolMetricInstanceSetRequest>(i => i.SchoolId == suppliedSchoolId && i.MetricVariantId == suppliedMetricVariantId102)))
                                            .Returns(new MetricTree(rootNode.DescendantsOrSelf.Single(x => x.MetricId == suppliedMetricId102)));

            request = GoalPlanningGetRequest.Create(suppliedSchoolId, suppliedMetricVariantId102);
            base.EstablishContext();
        }

        [Test]
        public void Should_load_correct_proposed_goals()
        {
            Assert.That(actualResults.ProposedGoals.Count(), Is.EqualTo(2));
            var proposedGoal = actualResults.ProposedGoals.Single(x => x.MetricId == suppliedMetricId1);
            Assert.That(proposedGoal.Goal, Is.EqualTo(suppliedProposedGoal1));
            Assert.That(proposedGoal.MetricId, Is.EqualTo(suppliedMetricId1));
            Assert.That(proposedGoal.EducationOrganizationId, Is.EqualTo(suppliedSchoolId));
            proposedGoal = actualResults.ProposedGoals.Single(x => x.MetricId == suppliedMetricId2);
            Assert.That(proposedGoal.Goal, Is.EqualTo(suppliedProposedGoal2));
            Assert.That(proposedGoal.MetricId, Is.EqualTo(suppliedMetricId2));
            Assert.That(proposedGoal.EducationOrganizationId, Is.EqualTo(suppliedSchoolId));
        }

        [Test]
        public void Should_load_correct_published_goals()
        {
            Assert.That(actualResults.PublishedGoals.Count(), Is.EqualTo(2));
            var publishedGoal = actualResults.PublishedGoals.Single(x => x.MetricId == suppliedMetricId1);
            Assert.That(publishedGoal.Goal, Is.EqualTo(suppliedGoal1));
            Assert.That(publishedGoal.MetricId, Is.EqualTo(suppliedMetricId1));
            Assert.That(publishedGoal.EducationOrganizationId, Is.EqualTo(suppliedSchoolId));
            Assert.That(publishedGoal.DisplayGoal, Is.EqualTo(String.Format("{0:P1}", suppliedGoal1)));
            var goalDifference = Convert.ToDecimal(suppliedValue1) - suppliedGoal1;
            Assert.That(publishedGoal.GoalDifference, Is.EqualTo(goalDifference));
            Assert.That(publishedGoal.DisplayGoalDifference, Is.EqualTo(String.Format("{0:P1}", goalDifference)));

            publishedGoal = actualResults.PublishedGoals.Single(x => x.MetricId == suppliedMetricId2);
            Assert.That(publishedGoal.Goal, Is.EqualTo(suppliedGoal2));
            Assert.That(publishedGoal.MetricId, Is.EqualTo(suppliedMetricId2));
            Assert.That(publishedGoal.EducationOrganizationId, Is.EqualTo(suppliedSchoolId));
            Assert.That(publishedGoal.DisplayGoal, Is.EqualTo(String.Format("{0:P1}", suppliedGoal2)));
            goalDifference = suppliedGoal2 - Convert.ToDecimal(suppliedValue2);
            Assert.That(publishedGoal.GoalDifference, Is.EqualTo(goalDifference));
            Assert.That(publishedGoal.DisplayGoalDifference, Is.EqualTo(String.Format("{0:P1}", goalDifference)));
        }
    }

    public class When_loading_goal_planning_for_a_single_metric : GoalPlanningServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            var metricTree = GetMetricTree();
            var rootNode = metricTree.RootNode as ContainerMetric;

            domainMetricService.Setup(x => x.Get(It.Is<SchoolMetricInstanceSetRequest>(i => i.SchoolId == suppliedSchoolId && i.MetricVariantId == suppliedMetricVariantId1)))
                                            .Returns(new MetricTree(rootNode.DescendantsOrSelf.Single(x => x.MetricVariantId == suppliedMetricVariantId1)));
            request = GoalPlanningGetRequest.Create(suppliedSchoolId, suppliedMetricVariantId1);
            base.EstablishContext();
        }
            
        [Test]
        public void Should_load_correct_proposed_goals()
        {
            Assert.That(actualResults.ProposedGoals.Count(), Is.EqualTo(1));
            Assert.That(actualResults.ProposedGoals.ElementAt(0).Goal, Is.EqualTo(suppliedProposedGoal1));
            Assert.That(actualResults.ProposedGoals.ElementAt(0).MetricId, Is.EqualTo(suppliedMetricId1));
            Assert.That(actualResults.ProposedGoals.ElementAt(0).EducationOrganizationId, Is.EqualTo(suppliedSchoolId));
        }

        [Test]
        public void Should_load_correct_published_goals()
        {
            Assert.That(actualResults.PublishedGoals.Count(), Is.EqualTo(1));
            Assert.That(actualResults.PublishedGoals.ElementAt(0).Goal, Is.EqualTo(suppliedGoal1));
            Assert.That(actualResults.PublishedGoals.ElementAt(0).MetricId, Is.EqualTo(suppliedMetricId1));
            Assert.That(actualResults.PublishedGoals.ElementAt(0).EducationOrganizationId, Is.EqualTo(suppliedSchoolId));
            Assert.That(actualResults.PublishedGoals.ElementAt(0).DisplayGoal, Is.EqualTo(String.Format("{0:P1}", suppliedGoal1)));
            var goalDifference = Convert.ToDecimal(suppliedValue1) - suppliedGoal1;
            Assert.That(actualResults.PublishedGoals.ElementAt(0).GoalDifference, Is.EqualTo(goalDifference));
            Assert.That(actualResults.PublishedGoals.ElementAt(0).DisplayGoalDifference, Is.EqualTo(String.Format("{0:P1}", goalDifference)));
        }
    }

    public class When_adding_goal_planning : When_loading_goal_planning_for_a_container_metric
    {
        private readonly List<EducationOrganizationGoalPlanning> postedGoalPlanning = new List<EducationOrganizationGoalPlanning>();
        private GoalPlanningPostRequest postRequest;

        protected override void EstablishContext()
        {
            postRequest = GoalPlanningPostRequest.Create(suppliedSchoolId, suppliedMetricVariantId102, GetGoalPlanningActions());

            goalPlanningRepository.Setup(x => x.Save(It.IsAny<EducationOrganizationGoalPlanning>())).Callback<EducationOrganizationGoalPlanning>(x => postedGoalPlanning.Add(x));

            base.EstablishContext();
            suppliedProposedGoal1 = .111m;
        }

        private IEnumerable<GoalPlanningPostRequest.GoalPlanningAction> GetGoalPlanningActions()
        {
            var actions = new List<GoalPlanningPostRequest.GoalPlanningAction>
                              {
                                  new GoalPlanningPostRequest.GoalPlanningAction
                                      {
                                          Action = PostAction.Add,
                                          MetricId = suppliedMetricId1,
                                          Goal = .111m
                                      },
                                  new GoalPlanningPostRequest.GoalPlanningAction
                                      {
                                          Action = PostAction.Add,
                                          MetricId = suppliedMetricId1 + 10,
                                          Goal = .888m
                                      },
                                  new GoalPlanningPostRequest.GoalPlanningAction
                                      {
                                          Action = PostAction.Add,
                                          MetricId = suppliedMetricId1 + 10,
                                          Goal = .999m
                                      },
                              };
            return actions;
        }

        protected override void ExecuteTest()
        {
            var service = new GoalPlanningService(goalPlanningRepository.Object, goalRepository.Object, rootMetricNodeResolver.Object, domainMetricService.Object, metricNodeResolver.Object);
            actualResults = service.Post(postRequest);
        }
    
        [Test]
        public void Should_add_correct_goals()
        {
            Assert.That(postedGoalPlanning.Count, Is.EqualTo(3));
            Assert.That(postedGoalPlanning.AsQueryable().Count(x => x.EducationOrganizationGoalPlanningId == suppliedGoalPlanningId), Is.EqualTo(1));
            Assert.That(postedGoalPlanning.AsQueryable().Count(x => x.EducationOrganizationId == suppliedSchoolId && x.MetricId == suppliedMetricId1 + 10 && x.Goal == .888m), Is.EqualTo(1));
            //Assert.That(postedGoalPlanning.AsQueryable().Count(x => x.EducationOrganizationId == suppliedSchoolId4 && x.MetricId == suppliedMetricId1 + 10 && x.Goal == .999m), Is.EqualTo(1));
        }
    }

    public class When_updating_goal_planning : When_loading_goal_planning_for_a_container_metric
    {
        private readonly List<EducationOrganizationGoalPlanning> postedGoalPlanning = new List<EducationOrganizationGoalPlanning>();
        private GoalPlanningPostRequest postRequest;

        protected override void EstablishContext()
        {
            postRequest = GoalPlanningPostRequest.Create(suppliedSchoolId, suppliedMetricVariantId102, GetGoalPlanningActions());

            goalPlanningRepository.Setup(x => x.Save(It.IsAny<EducationOrganizationGoalPlanning>())).Callback<EducationOrganizationGoalPlanning>(x => postedGoalPlanning.Add(x));

            base.EstablishContext();
            suppliedProposedGoal1 = .111m;
        }

        private IEnumerable<GoalPlanningPostRequest.GoalPlanningAction> GetGoalPlanningActions()
        {
            var actions = new List<GoalPlanningPostRequest.GoalPlanningAction>
                              {
                                  new GoalPlanningPostRequest.GoalPlanningAction
                                      {
                                          Action = PostAction.Set,
                                          MetricId = suppliedMetricId1,
                                          Goal = .111m
                                      },
                                  new GoalPlanningPostRequest.GoalPlanningAction
                                      {
                                          Action = PostAction.Set,
                                          MetricId = suppliedMetricId1 + 10,
                                          Goal = .888m
                                      },
                                  new GoalPlanningPostRequest.GoalPlanningAction
                                      {
                                          Action = PostAction.Set,
                                          MetricId = suppliedMetricId1 + 10,
                                          Goal = .999m
                                      },
                              };
            return actions;
        }

        protected override void ExecuteTest()
        {
            var service = new GoalPlanningService(goalPlanningRepository.Object, goalRepository.Object, rootMetricNodeResolver.Object, domainMetricService.Object, metricNodeResolver.Object);
            actualResults = service.Post(postRequest);
        }

        [Test]
        public void Should_update_correct_goals()
        {
            Assert.That(postedGoalPlanning.Count, Is.EqualTo(3));
            Assert.That(postedGoalPlanning.AsQueryable().Count(x => x.EducationOrganizationGoalPlanningId == suppliedGoalPlanningId), Is.EqualTo(1));
            Assert.That(postedGoalPlanning.AsQueryable().Count(x => x.EducationOrganizationId == suppliedSchoolId && x.MetricId == suppliedMetricId1 && x.Goal == .111m), Is.EqualTo(1));
            //Assert.That(postedGoalPlanning.AsQueryable().Count(x => x.EducationOrganizationId == suppliedSchoolId1 && x.MetricId == suppliedMetricId2 && x.Goal == .999m), Is.EqualTo(1));
        }
    }

    public class When_deleting_goal_planning : When_loading_goal_planning_for_a_container_metric
    {
        private readonly List<Expression<Func<EducationOrganizationGoalPlanning, bool>>> postedGoalPlanning = new List<Expression<Func<EducationOrganizationGoalPlanning, bool>>>();
        private GoalPlanningPostRequest postRequest;

        protected override void EstablishContext()
        {
            postRequest = GoalPlanningPostRequest.Create(suppliedSchoolId, suppliedMetricVariantId102, GetGoalPlanningActions());
            
            goalPlanningRepository.Setup(x => x.Delete(It.IsAny<Expression<Func<EducationOrganizationGoalPlanning, bool>>>())).Callback<Expression<Func<EducationOrganizationGoalPlanning, bool>>>(x => postedGoalPlanning.Add(x));

            base.EstablishContext();
        }

        private IEnumerable<GoalPlanningPostRequest.GoalPlanningAction> GetGoalPlanningActions()
        {
            var actions = new List<GoalPlanningPostRequest.GoalPlanningAction>
                              {
                                  new GoalPlanningPostRequest.GoalPlanningAction
                                      {
                                          Action = PostAction.Remove,
                                          MetricId = suppliedMetricId1,
                                          Goal = .111m
                                      },
                                  new GoalPlanningPostRequest.GoalPlanningAction
                                      {
                                          Action = PostAction.Remove,
                                          MetricId = suppliedMetricId1 + 10,
                                          Goal = .888m
                                      },
                                  new GoalPlanningPostRequest.GoalPlanningAction
                                      {
                                          Action = PostAction.Remove,
                                          MetricId = suppliedMetricId1 + 10,
                                          Goal = .999m
                                      },
                              };
            return actions;
        }

        protected override void ExecuteTest()
        {
            var service = new GoalPlanningService(goalPlanningRepository.Object, goalRepository.Object, rootMetricNodeResolver.Object, domainMetricService.Object, metricNodeResolver.Object);
            actualResults = service.Post(postRequest);
        }

        [Test]
        public void Should_delete_correct_goals()
        {
            Assert.That(postedGoalPlanning.Count, Is.EqualTo(3));
            //Assert.That(postedGoalPlanning[0].Compile()());
        }
    }

}
 