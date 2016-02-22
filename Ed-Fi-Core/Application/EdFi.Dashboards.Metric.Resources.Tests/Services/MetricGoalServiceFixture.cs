// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Metric.Resources.Services.Data;
using EdFi.Dashboards.Resources.Tests;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;
using MetricState = EdFi.Dashboards.Metric.Resources.Models.MetricState;
using MetricStateType = EdFi.Dashboards.Metric.Resources.Models.MetricStateType;

namespace EdFi.Dashboards.Metric.Resources.Tests.Services
{
    public abstract class MetricGoalFixtureBase : TestFixtureBase
    {
        //The Injected Dependencies.
        protected IMetricGoalsService metricGoalsService;
        protected IMetricMetadataTreeService metricMetadataTreeService;
        protected IMetricInstancesService metricInstancesService;
        protected ICacheProvider cacheProvider; 

        //The Actual Model.
        protected Goal actualModel;
        private IMetricGoalProvider provider;

        //The supplied Data models.
        protected int suppliedMetricId = 1;
        protected Guid suppliedDomainEntityId = Guid.NewGuid();
        protected decimal suppliedMetricGoal = 50;
        protected IQueryable<Metric.Data.Entities.MetricGoal> suppliedMetricGoalData;
        protected MetricMetadataTree suppliedMetricMetadataTree;
        protected IQueryable<MetricInstance> suppliedMetricInstanceData;
        

        protected override void EstablishContext()
        {
            metricGoalsService = mocks.StrictMock<IMetricGoalsService>();
            metricMetadataTreeService = mocks.StrictMock<IMetricMetadataTreeService>();
            metricInstancesService = mocks.StrictMock<IMetricInstancesService>();
            cacheProvider = mocks.StrictMock<ICacheProvider>();

            suppliedMetricGoalData = GetSuppliedMetricGoalData();
            suppliedMetricMetadataTree = GetSuppliedMetaDataNode();
            suppliedMetricInstanceData = GetSuppliedMetricInstance();
        }

        private IQueryable<MetricInstance> GetSuppliedMetricInstance()
        {
            return new List<MetricInstance>().AsQueryable();
        }

        private MetricMetadataTree GetSuppliedMetaDataNode()
        {
            var tree = new TestMetricMetadataTree();

            tree.Children = new List<MetricMetadataNode>
                { 
                    new MetricMetadataNode(tree)
                       {
                           MetricId = suppliedMetricId,
                           TrendInterpretation = 1,
                           Enabled = true,
                           States = new List<Models.MetricState>
                                        {
                                            new Models.MetricState{ StateText = "Good", StateType = MetricStateType.Good, MinValue = .55m, IsMaxValueInclusive = 1, MaxValue=1, IsMinValueInclusive = 1},
                                            new Models.MetricState{ StateText = "Low", StateType = MetricStateType.Low, MinValue = 0, IsMaxValueInclusive = 1, MaxValue=.55m, IsMinValueInclusive = 0},
                                        }
                       }
                };

            return tree;
        }

        protected override void ExecuteTest()
        {
            provider = new MetricGoalProvider(metricMetadataTreeService, metricGoalsService, metricInstancesService);
            actualModel = provider.GetMetricGoal(suppliedDomainEntityId, suppliedMetricId);
        }

        private IQueryable<Metric.Data.Entities.MetricGoal> GetSuppliedMetricGoalData()
        {
            return (new List<Metric.Data.Entities.MetricGoal>
                        {
                            new Metric.Data.Entities.MetricGoal{MetricId = suppliedMetricId, MetricInstanceSetKey = suppliedDomainEntityId, Value = suppliedMetricGoal},
                            new Metric.Data.Entities.MetricGoal{MetricId = 5000},//Should be filtered out.
                        }).AsQueryable();
        }

        [Test]
        public void Should_return_model_with_trend_interpretation()
        {
            Assert.That(actualModel.Interpretation, Is.EqualTo((TrendInterpretation)suppliedMetricMetadataTree.Children.First().TrendInterpretation));
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }

    [TestFixture]
    public class When_getting_a_metric_goal_from_a_metric_that_has_a_goal_override : MetricGoalFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            Expect.Call(metricGoalsService.Get(null)).IgnoreArguments().Return(suppliedMetricGoalData);
            Expect.Call(metricMetadataTreeService.Get(null)).IgnoreArguments().Return(suppliedMetricMetadataTree);
            Expect.Call(metricInstancesService.Get(null)).IgnoreArguments().Return(suppliedMetricInstanceData);
        }

        [Test]
        public void Should_return_goal_value_from_goal_data()
        {
            var goalData = suppliedMetricGoalData.Single(x => x.MetricId == suppliedMetricId && x.MetricInstanceSetKey == suppliedDomainEntityId);
            Assert.That(actualModel.Value, Is.EqualTo(goalData.Value));
        }
    }

    [TestFixture]
    public class When_getting_a_metric_goal_from_a_metric_that_does_not_have_a_goal_override_and_has_its_good_state_on_the_min_value : MetricGoalFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            suppliedMetricGoalData = GetSuppliedMetricGoalData();

            //Expect.Call(metricGoalsService.GetAll()).Return(suppliedMetricGoalData);
            //object suppliedMetricGoalDataList;

            //Expect.Call(cacheProvider.TryGetCachedObject("MetricGoalProviderMetricGoals" + suppliedDomainEntityId, out suppliedMetricGoalDataList)).Return(true).OutRef(suppliedMetricGoalData.ToList());
            Expect.Call(metricGoalsService.Get(null)).IgnoreArguments().Return(suppliedMetricGoalData);
            Expect.Call(metricMetadataTreeService.Get(null)).IgnoreArguments().Return(suppliedMetricMetadataTree);
            Expect.Call(metricInstancesService.Get(null)).IgnoreArguments().Return(suppliedMetricInstanceData);
        }

        private IQueryable<Metric.Data.Entities.MetricGoal> GetSuppliedMetricGoalData()
        {
            return (new List<Metric.Data.Entities.MetricGoal>()).AsQueryable();
        }

        [Test]
        public void Should_return_goal_value_from_metric_state_data()
        {
            Assert.That(actualModel.Value, Is.EqualTo(suppliedMetricMetadataTree.Children.First().States.Single(x=>x.StateType==MetricStateType.Good).MinValue));
        }
    }

    [TestFixture]
    public class When_getting_a_metric_goal_from_a_metric_that_does_not_have_a_goal_override_and_has_its_good_state_on_the_max_value : MetricGoalFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            suppliedMetricGoalData = GetSuppliedMetricGoalData();
            suppliedMetricMetadataTree = GetSuppliedMetaDataTree();

            Expect.Call(metricGoalsService.Get(null)).IgnoreArguments().Return(suppliedMetricGoalData);
            Expect.Call(metricMetadataTreeService.Get(null)).IgnoreArguments().Return(suppliedMetricMetadataTree);
            Expect.Call(metricInstancesService.Get(null)).IgnoreArguments().Return(suppliedMetricInstanceData);
        }

        private IQueryable<Metric.Data.Entities.MetricGoal> GetSuppliedMetricGoalData()
        {
            return (new List<Metric.Data.Entities.MetricGoal>()).AsQueryable();
        }

        private MetricMetadataTree GetSuppliedMetaDataTree()
        {
            var tree = new TestMetricMetadataTree();

            tree.Children = new List<MetricMetadataNode>
            {
                new MetricMetadataNode(tree)
                {
                    MetricId = suppliedMetricId,
                    Enabled = true,
                    TrendInterpretation = 1,
                    States = new List<Models.MetricState>
                                            {
                                                new Models.MetricState{ StateText = "Good", StateType = MetricStateType.Low, MinValue = .55m, IsMaxValueInclusive = 1, MaxValue=1, IsMinValueInclusive = 1},
                                                new Models.MetricState{ StateText = "Low", StateType = MetricStateType.Good, MinValue = 0, IsMaxValueInclusive = 1, MaxValue=.55m, IsMinValueInclusive = 0},
                                            }
                }
            };

            return tree;
        }

        [Test]
        public void Should_return_goal_value_from_metric_state_data()
        {
            Assert.That(actualModel.Value, Is.EqualTo(suppliedMetricMetadataTree.Children.First().States.Single(x=>x.StateType==MetricStateType.Good).MaxValue));
        }
    }

    [TestFixture]
    public class When_getting_the_metric_goal_for_a_metric_that_has_more_than_one_metric_state : MetricGoalFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            suppliedMetricMetadataTree = GetSuppliedMetaDataTree();
            suppliedMetricGoalData = GetSuppliedMetricGoalData();
            suppliedMetricInstanceData = GetSuppliedMetricInstance();

            //Testing the Cache is being called right.
            //object suppliedMetricGoalDataList; // suppliedMetricGoalData.ToList();

            //Expect.Call(cacheProvider.TryGetCachedObject("MetricGoalProviderMetricGoals" + suppliedDomainEntityId, out suppliedMetricGoalDataList)).Return(false);
            Expect.Call(metricGoalsService.Get(null)).IgnoreArguments().Return(suppliedMetricGoalData);
            //cacheProvider.SetCachedObject("MetricGoalProviderMetricGoals" + suppliedDomainEntityId, suppliedMetricGoalData.ToList());

            Expect.Call(metricMetadataTreeService.Get(null)).IgnoreArguments().Return(suppliedMetricMetadataTree);
            Expect.Call(metricInstancesService.Get(null)).IgnoreArguments().Return(suppliedMetricInstanceData);
        }

        private MetricMetadataTree GetSuppliedMetaDataTree()
        {
            var tree = new TestMetricMetadataTree();

            tree.Children = new List<MetricMetadataNode>
            {
                new MetricMetadataNode(tree)
                {
                    MetricId = suppliedMetricId,
                    TrendInterpretation = 1,
                    Enabled = true,
                    States = new List<Models.MetricState>
                                            {
                                                new Models.MetricState{ StateText = "Low", StateType = MetricStateType.Low, MinValue = 0, IsMinValueInclusive = 1, MaxValue=.55m, IsMaxValueInclusive = 0},
                                                new Models.MetricState{ StateText = "Good", StateType = MetricStateType.Good, MinValue = .55m, IsMinValueInclusive = 1, MaxValue=.70m, IsMaxValueInclusive = 0},
                                                new Models.MetricState{ StateText = "Good", StateType = MetricStateType.Good, MinValue = .70m, IsMinValueInclusive = 1, MaxValue=1, IsMaxValueInclusive = 1},
                                            }
                }                                    
            };

            return tree;
        }

        private IQueryable<Metric.Data.Entities.MetricGoal> GetSuppliedMetricGoalData()
        {
            return (new List<Metric.Data.Entities.MetricGoal>()).AsQueryable();
        }

        private IQueryable<Metric.Data.Entities.MetricInstance> GetSuppliedMetricInstance()
        {
            return (new List<Metric.Data.Entities.MetricInstance>
                        {
                            new MetricInstance
                                {
                                    MetricInstanceSetKey = suppliedDomainEntityId,
                                    MetricId = suppliedMetricId, 
                                    Value = ".70", 
                                    ValueTypeName = "System.Decimal"
                                }
                        }).AsQueryable();
        }

        [Test]
        public void Should_return_correct_goal()
        {
            Assert.That(actualModel.Value, Is.EqualTo(.7m));
        }

    }

    
    public abstract class When_getting_a_metric_goal_from_a_metric_that_has_no_goal_and_no_metric_state : TestFixtureBase
    {
        //The Injected Dependencies.
        protected IMetricMetadataTreeService metricMetadataNodeService;
        protected IMetricGoalsService metricGoalsService;
        protected IMetricInstancesService metricInstancesService;
        protected ICacheProvider cacheProvider;

        //The Actual Model.
        protected Goal actualModel;

        //The supplied Data models.
        protected int suppliedMetricId = 1;
        protected MetricMetadataTree suppliedMetricMetadataTree;
        protected MetricData suppliedMetricData;

        protected override void EstablishContext()
        {
            //Prepare supplied data collections
            suppliedMetricData = GetSuppliedMetricData();
            suppliedMetricMetadataTree = GetSuppliedMetricMetadataTree();

            //Set up the mocks
            metricMetadataNodeService = mocks.StrictMock<IMetricMetadataTreeService>();
            metricGoalsService = mocks.StrictMock<IMetricGoalsService>();
            metricInstancesService = mocks.StrictMock<IMetricInstancesService>();
            cacheProvider = mocks.StrictMock<ICacheProvider>();
        }

        protected virtual MetricMetadataTree GetSuppliedMetricMetadataTree()
        {
            var tree = new TestMetricMetadataTree();

            tree.Children = new List<MetricMetadataNode>
                                {
                                    new MetricMetadataNode(tree) {MetricId = suppliedMetricId, Enabled = false}
                                };

            return tree;
        }

        private MetricData GetSuppliedMetricData()
        {
            return new CurrentYearMetricData
            {
                MetricGoals = new List<MetricGoal>().AsQueryable(),
                MetricInstances = new List<MetricInstance>().AsQueryable()
            };
        }
    }

    [TestFixture]
    public class When_getting_a_metric_goal_from_a_metric_that_is_Disabled_and_has_no_goal_and_no_metric_state 
        : When_getting_a_metric_goal_from_a_metric_that_has_no_goal_and_no_metric_state
    {
        protected override void EstablishContext()
        {
            //Prepare supplied data collections
            suppliedMetricData = GetSuppliedMetricData();
            suppliedMetricMetadataTree = GetSuppliedMetricMetadataTree();
        }

        private MetricData GetSuppliedMetricData()
        {
            return new CurrentYearMetricData
                       {
                           MetricGoals = new List<MetricGoal>().AsQueryable(),
                           MetricInstances = new List<MetricInstance>().AsQueryable()
                       };
        }

        protected override void ExecuteTest()
        {
            var provider = new MetricGoalProvider(metricMetadataNodeService, metricGoalsService, metricInstancesService);
            actualModel = provider.GetMetricGoal(suppliedMetricMetadataTree.Children.First(), suppliedMetricData);
        }

        [Test]
        public void Should_return_a_default_goal()
        {
            var expectedGoal = new Goal { Interpretation = TrendInterpretation.None, Value = null };
            Assert.That(actualModel.Interpretation, Is.EqualTo(expectedGoal.Interpretation));
            Assert.That(actualModel.Value, Is.EqualTo(expectedGoal.Value));
        }

    }

    [TestFixture]
    public class When_getting_the_metric_goal_from_a_metric_that_is_Enabled_and_has_no_goal_and_no_metric_state : When_getting_a_metric_goal_from_a_metric_that_has_no_goal_and_no_metric_state
    {
        private Exception actualException;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            //Prepare supplied data collections
            suppliedMetricMetadataTree = GetSuppliedMetricMetadataTree();
        }

        protected override void ExecuteTest()
        {
            var provider = new MetricGoalProvider(metricMetadataNodeService, metricGoalsService, metricInstancesService);

            try
            {
                actualModel = provider.GetMetricGoal(suppliedMetricMetadataTree.Children.First(), suppliedMetricData);
            }
            catch (Exception ex)
            {
                actualException = ex;
            }
        }

        protected override MetricMetadataTree GetSuppliedMetricMetadataTree()
        {
            var tree = new TestMetricMetadataTree();

            tree.Children = new List<MetricMetadataNode>
                                {
                                    new MetricMetadataNode(tree) {MetricId = suppliedMetricId, Enabled = true},
                                };
            return tree;
        }
        
        [Test]
        public void Should_throw_an_invalid_operation_exception()
        {
            Assert.That(actualException, Is.Not.Null);
            Assert.That(actualException, Is.TypeOf<InvalidOperationException>());
        }
    }

    public class When_getting_a_metric_goal_from_a_metric_that_has_no_goal_and_a_none_metric_state : TestFixtureBase
    {
        //The Injected Dependencies.
        protected IMetricMetadataTreeService metricMetadataNodeService;
        protected IMetricGoalsService metricGoalsService;
        protected IMetricInstancesService metricInstancesService;
        protected ICacheProvider cacheProvider;

        //The Actual Model.
        protected Goal actualModel;

        //The supplied Data models.
        protected int suppliedMetricId = 1;
        protected MetricMetadataTree suppliedMetricMetadataTree;
        protected MetricData suppliedMetricData;

        protected override void EstablishContext()
        {
            //Prepare supplied data collections
            suppliedMetricData = GetSuppliedMetricData();
            suppliedMetricMetadataTree = GetSuppliedMetricMetadataTree();

            //Set up the mocks
            metricMetadataNodeService = mocks.StrictMock<IMetricMetadataTreeService>();
            metricGoalsService = mocks.StrictMock<IMetricGoalsService>();
            metricInstancesService = mocks.StrictMock<IMetricInstancesService>();
            cacheProvider = mocks.StrictMock<ICacheProvider>();
        }

        protected override void ExecuteTest()
        {
            var provider = new MetricGoalProvider(metricMetadataNodeService, metricGoalsService, metricInstancesService);
            actualModel = provider.GetMetricGoal(suppliedMetricMetadataTree.Children.First(), suppliedMetricData);
        }

        [Test]
        public void Should_return_a_default_goal()
        {
            var expectedGoal = new Goal { Interpretation = TrendInterpretation.None, Value = null };
            Assert.That(actualModel.Interpretation, Is.EqualTo(expectedGoal.Interpretation));
            Assert.That(actualModel.Value, Is.EqualTo(expectedGoal.Value));
        }

        protected virtual MetricMetadataTree GetSuppliedMetricMetadataTree()
        {
            var tree = new TestMetricMetadataTree();

            tree.Children = new List<MetricMetadataNode>
                                {
                                    new MetricMetadataNode(tree) {MetricId = suppliedMetricId, Enabled = true, TrendInterpretation = (int)TrendInterpretation.None, States = new List<MetricState>{new MetricState{StateType = MetricStateType.None}}}
                                };

            return tree;
        }

        private MetricData GetSuppliedMetricData()
        {
            return new CurrentYearMetricData
            {
                MetricGoals = new List<MetricGoal>().AsQueryable(),
                MetricInstances = new List<MetricInstance>().AsQueryable()
            };
        }
    }


    public class When_getting_a_metric_goal_from_a_metric_that_has_no_goal_and_a_NA_metric_state : TestFixtureBase
    {
        //The Injected Dependencies.
        protected IMetricMetadataTreeService metricMetadataNodeService;
        protected IMetricGoalsService metricGoalsService;
        protected IMetricInstancesService metricInstancesService;
        protected ICacheProvider cacheProvider;

        //The Actual Model.
        protected Goal actualModel;

        //The supplied Data models.
        protected int suppliedMetricId = 1;
        protected MetricMetadataTree suppliedMetricMetadataTree;
        protected MetricData suppliedMetricData;

        protected override void EstablishContext()
        {
            //Prepare supplied data collections
            suppliedMetricData = GetSuppliedMetricData();
            suppliedMetricMetadataTree = GetSuppliedMetricMetadataTree();

            //Set up the mocks
            metricMetadataNodeService = mocks.StrictMock<IMetricMetadataTreeService>();
            metricGoalsService = mocks.StrictMock<IMetricGoalsService>();
            metricInstancesService = mocks.StrictMock<IMetricInstancesService>();
            cacheProvider = mocks.StrictMock<ICacheProvider>();
        }

        protected override void ExecuteTest()
        {
            var provider = new MetricGoalProvider(metricMetadataNodeService, metricGoalsService, metricInstancesService);
            actualModel = provider.GetMetricGoal(suppliedMetricMetadataTree.Children.First(), suppliedMetricData);
        }

        [Test]
        public void Should_return_a_default_goal()
        {
            var expectedGoal = new Goal { Interpretation = TrendInterpretation.None, Value = null };
            Assert.That(actualModel.Interpretation, Is.EqualTo(expectedGoal.Interpretation));
            Assert.That(actualModel.Value, Is.EqualTo(expectedGoal.Value));
        }

        protected virtual MetricMetadataTree GetSuppliedMetricMetadataTree()
        {
            var tree = new TestMetricMetadataTree();

            tree.Children = new List<MetricMetadataNode>
                                {
                                    new MetricMetadataNode(tree) {MetricId = suppliedMetricId, Enabled = true, TrendInterpretation = (int)TrendInterpretation.None, States = new List<MetricState>{new MetricState{StateType = MetricStateType.Na}}}
                                };

            return tree;
        }

        private MetricData GetSuppliedMetricData()
        {
            return new CurrentYearMetricData
            {
                MetricGoals = new List<MetricGoal>().AsQueryable(),
                MetricInstances = new List<MetricInstance>().AsQueryable()
            };
        }
    }
}
