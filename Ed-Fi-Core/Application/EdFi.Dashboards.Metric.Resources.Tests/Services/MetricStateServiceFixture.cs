// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Tests;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;
using MetricState = EdFi.Dashboards.Metric.Resources.Models.MetricState;
using MetricStateType = EdFi.Dashboards.Metric.Resources.Models.MetricStateType;
using EdFi.Dashboards.Resources;

namespace EdFi.Dashboards.Metric.Resources.Tests.Services
{
    [TestFixture]
    public abstract class MetricStateServiceFixtureBase : TestFixtureBase
    {
        protected State actualModel;
        protected int suppliedMetricId1 = 1;
        protected int suppliedMetricId2 = 2;
        protected int suppliedMetricId3 = 3;
        
        protected string suppliedMetricValueForLowRange = ".40";
        protected string suppliedMetricValueForGoodRange = ".89";
        
        protected string suppliedMetricValueForLowerLimitBoundaryTestForLow = ".0";
        protected string suppliedMetricValueForUpperLimitBoundaryTestForLow = ".55";
        
        protected string suppliedMetricValueForLowerLimitBoundaryTestForGood = ".56";
        protected string suppliedMetricValueForUpperLimitBoundaryTestForGood = "1";

        protected string suppliedMetricValueForUpperLimitBoundaryTestForLowNotInclusive = ".54";
        protected string suppliedMetricValueForLowerLimitBoundaryTestForGoodNotInclusive = ".56";
        
        protected string suppliedMetricValueType1 = "System.Double";
        protected MetricMetadataTree suppliedMetricMetadataTree;

        protected IMetricMetadataTreeService metricMetadataTreeService;
        protected IMetricStateProvider providerToTest;


        protected override void EstablishContext()
        {
            metricMetadataTreeService = mocks.StrictMock<IMetricMetadataTreeService>();
            suppliedMetricMetadataTree = GetSuppliedMetadataTree();

            providerToTest = new MetricStateProvider(metricMetadataTreeService);
        }

        protected MetricMetadataTree GetSuppliedMetadataTree()
        {
            //root node holder 
            var tree = new TestMetricMetadataTree();

            tree.Children = new List<MetricMetadataNode>
                                        {   //3 nodes underneath
                                            new MetricMetadataNode(tree) {Name = "FirstLevelMetric01", MetricId = 1001},
                                            new MetricMetadataNode(tree) {Name = "FirstLevelMetric02", MetricId = 1002,
                                                Children = new List<MetricMetadataNode> //2 child metrics.
                                                                          {
                                                                              new MetricMetadataNode(tree)
                                                                              {
                                                                                    MetricId = 1, Name = "Metric WITH Inclusive limits.",
                                                                                    States = new List<MetricState>
                                                                                                 {
                                                                                                     new MetricState{MinValue = 0, IsMinValueInclusive = 1, MaxValue = .55m, IsMaxValueInclusive = 1, StateText = "Low", StateType = MetricStateType.Low},
                                                                                                     new MetricState{MinValue = .56m, IsMinValueInclusive = 1, MaxValue = 1, IsMaxValueInclusive = 1, StateText = "Good", StateType = MetricStateType.Good},
                                                                                                 }
                                                                              },
                                                                              new MetricMetadataNode(tree)
                                                                              {
                                                                                    MetricId = 2, Name = "Metric WITHOUT inclusive limits.",
                                                                                    States = new List<MetricState>
                                                                                                 {
                                                                                                     new MetricState{MinValue = .0m, IsMinValueInclusive = 1, MaxValue = .55m, IsMaxValueInclusive = 0, StateText = "Low", StateType = MetricStateType.Low},
                                                                                                     new MetricState{MinValue = .55m, IsMinValueInclusive = 0, MaxValue = 1, IsMaxValueInclusive = 1, StateText = "Good", StateType = MetricStateType.Good},
                                                                                                 }
                                                                              },
                                                                              new MetricMetadataNode(tree)
                                                                              {
                                                                                    MetricId = 3, Name = "Metric WITH inclusive limits and more than one state per state type.",
                                                                                    Enabled = true,
                                                                                    States = new List<MetricState>
                                                                                                 {
                                                                                                     new MetricState{MinValue = .0m, IsMinValueInclusive = 1, MaxValue = .25m, IsMaxValueInclusive = 0, StateText = "Very Low", StateType = MetricStateType.Low},
                                                                                                     new MetricState{MinValue = .25m, IsMinValueInclusive = 1, MaxValue = .55m, IsMaxValueInclusive = 0, StateText = "Low", StateType = MetricStateType.Low},
                                                                                                     new MetricState{MinValue = .55m, IsMinValueInclusive = 0, MaxValue = .70m, IsMaxValueInclusive = 0, StateText = "Good", StateType = MetricStateType.Good},
                                                                                                     new MetricState{MinValue = .70m, IsMinValueInclusive = 0, MaxValue = 1, IsMaxValueInclusive = 1, StateText = "Very Good", StateType = MetricStateType.Good},
                                                                                                 }
                                                                              }
                                                                          }
                                            },
                                            new MetricMetadataNode(tree){Name = "FirstLevelMetric03", MetricId = 1003}
                                        };

            return tree;
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }

    /// <summary>
    /// Testing that state is low when the metric value is in the low limit when supplied.
    /// </summary>
    [TestFixture]
    public class When_getting_the_metric_state_for_a_metric_with_more_than_one_state_for_each_state_type_and_lower_limit : MetricStateServiceFixtureBase
    {
        private MetricState suppliedStateInContext;

        protected override void ExecuteTest()
        {
            actualModel = providerToTest.GetState(suppliedMetricMetadataTree.Descendants.Single(x=>x.MetricId == suppliedMetricId3), GetSuppliedMetricInstance());

            //Picking the state that we are going to evaluate against.
            suppliedStateInContext = suppliedMetricMetadataTree.Descendants.Single(x => x.MetricId == suppliedMetricId3).States.Single(x => x.StateType == MetricStateType.Low && x.MaxValue==.55m);
        }

        private MetricInstance GetSuppliedMetricInstance()
        {
            return new MetricInstance
                       {
                           MetricId = suppliedMetricId3,
                           MetricStateTypeId = 3,
                           Value = ".25",
                           ValueTypeName = suppliedMetricValueType1,
                       };
        }

        [Test]
        public void Should_return_a_low_state_text()
        {
            Assert.That(actualModel.StateText, Is.EqualTo(suppliedStateInContext.StateText));
        }

        [Test]
        public void Should_return_a_low_state()
        {
            Assert.That(actualModel.StateType, Is.EqualTo(suppliedStateInContext.StateType));
        }
    }

    /// <summary>
    /// Testing that state is low when the metric value is in the low limit when supplied.
    /// </summary>
    [TestFixture]
    public class When_getting_the_metric_state_for_a_metric_with_more_than_one_state_for_each_state_type_and_upper_limit : MetricStateServiceFixtureBase
    {
        private MetricState suppliedStateInContext;

        protected override void ExecuteTest()
        {
            actualModel = providerToTest.GetState(suppliedMetricMetadataTree.Descendants.Single(x => x.MetricId == suppliedMetricId3), GetSuppliedMetricInstance());

            //Picking the state that we are going to evaluate against.
            suppliedStateInContext = suppliedMetricMetadataTree.Descendants.Single(x => x.MetricId == suppliedMetricId3).States.Single(x => x.StateType == MetricStateType.Low && x.MaxValue == .25m);
        }

        private MetricInstance GetSuppliedMetricInstance()
        {
            return new MetricInstance
            {
                MetricId = suppliedMetricId3,
                MetricStateTypeId = 3,
                Value = ".24",
                ValueTypeName = suppliedMetricValueType1,
            };
        }

        [Test]
        public void Should_return_a_low_state_text()
        {
            Assert.That(actualModel.StateText, Is.EqualTo(suppliedStateInContext.StateText));
        }

        [Test]
        public void Should_return_a_low_state()
        {
            Assert.That(actualModel.StateType, Is.EqualTo(suppliedStateInContext.StateType));
        }
    }

    /// <summary>
    /// Testing that state is low when the metric value is in the low range.
    /// </summary>
    [TestFixture]
    public class When_getting_metric_states_for_a_metric_with_value_in_the_low_range : MetricStateServiceFixtureBase
    {
        private MetricState suppliedStateInContext;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(metricMetadataTreeService.Get(null)).IgnoreArguments().Return(suppliedMetricMetadataTree);
        }

        protected override void ExecuteTest()
        {
            actualModel = providerToTest.GetState(suppliedMetricId1, suppliedMetricValueForLowRange, suppliedMetricValueType1);

            //Picking the state that we are going to evaluate against.
            suppliedStateInContext = suppliedMetricMetadataTree.Descendants.Single(x => x.MetricId == suppliedMetricId1).States.Single(x => x.StateType == MetricStateType.Low);
        }

        [Test]
        public void Should_return_a_low_state_text()
        {
            Assert.That(actualModel.StateText, Is.EqualTo(suppliedStateInContext.StateText));
        }

        [Test]
        public void Should_return_a_low_state()
        {
            Assert.That(actualModel.StateType, Is.EqualTo(suppliedStateInContext.StateType));
        }
    }

    /// <summary>
    /// Testing that state is good when the metric value is in the good range.
    /// </summary>
    [TestFixture]
    public class When_getting_metric_states_for_a_metric_with_value_in_the_good_range : MetricStateServiceFixtureBase
    {
        private MetricState suppliedStateInContext;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(metricMetadataTreeService.Get(null)).IgnoreArguments().Return(suppliedMetricMetadataTree);
        }

        protected override void ExecuteTest()
        {
            actualModel = providerToTest.GetState(suppliedMetricId1, suppliedMetricValueForGoodRange, suppliedMetricValueType1);

            //Picking the state that we are going to evaluate against.
            suppliedStateInContext = suppliedMetricMetadataTree.Descendants.Single(x => x.MetricId == suppliedMetricId1).States.Single(x => x.StateType == MetricStateType.Good);
        }

        [Test]
        public void Should_return_a_good_state_text()
        {
            Assert.That(actualModel.StateText, Is.EqualTo(suppliedStateInContext.StateText));
        }

        [Test]
        public void Should_return_a_good_state()
        {
            Assert.That(actualModel.StateType, Is.EqualTo(suppliedStateInContext.StateType));
        }
    }

    /// <summary>
    /// Testing that state is low when the metric value is in the LOWER boundary and the metric state rule has inclusive values.
    /// </summary>
    [TestFixture]
    public class When_getting_metric_states_for_a_metric_with_value_in_the_low_range_and_lower_boundary_and_metric_state_rule_set_as_inclusive : MetricStateServiceFixtureBase
    {
        private MetricState suppliedStateInContext;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(metricMetadataTreeService.Get(null)).IgnoreArguments().Return(suppliedMetricMetadataTree);
        }

        protected override void ExecuteTest()
        {
            actualModel = providerToTest.GetState(suppliedMetricId1, suppliedMetricValueForLowerLimitBoundaryTestForLow, suppliedMetricValueType1);

            //Picking the state that we are going to evaluate against.
            suppliedStateInContext = suppliedMetricMetadataTree.Descendants.Single(x => x.MetricId == suppliedMetricId1).States.Single(x => x.StateType == MetricStateType.Low);
        }

        [Test]
        public void Should_return_a_low_state_text()
        {
            Assert.That(actualModel.StateText, Is.EqualTo(suppliedStateInContext.StateText));
        }

        [Test]
        public void Should_return_a_low_state()
        {
            Assert.That(actualModel.StateType, Is.EqualTo(suppliedStateInContext.StateType));
        }
    }

    /// <summary>
    /// Testing that state is low when the metric value is in the UPPER boundary and the metric state rule has inclusive values.
    /// </summary>
    [TestFixture]
    public class When_getting_metric_states_for_a_metric_with_value_in_the_low_range_and_upper_boundary_and_metric_state_rule_set_as_inclusive : MetricStateServiceFixtureBase
    {
        private MetricState suppliedStateInContext;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(metricMetadataTreeService.Get(null)).IgnoreArguments().Return(suppliedMetricMetadataTree);
        }

        protected override void ExecuteTest()
        {
            actualModel = providerToTest.GetState(suppliedMetricId1, suppliedMetricValueForUpperLimitBoundaryTestForLow, suppliedMetricValueType1);

            //Picking the state that we are going to evaluate against.
            suppliedStateInContext = suppliedMetricMetadataTree.Descendants.Single(x => x.MetricId == suppliedMetricId1).States.Single(x => x.StateType == MetricStateType.Low);
        }

        [Test]
        public void Should_return_a_low_state_text()
        {
            Assert.That(actualModel.StateText, Is.EqualTo(suppliedStateInContext.StateText));
        }

        [Test]
        public void Should_return_a_low_state()
        {
            Assert.That(actualModel.StateType, Is.EqualTo(suppliedStateInContext.StateType));
        }
    }

    /// <summary>
    /// Testing that state is good when the metric value is in the LOWER boundary and the metric state rule has inclusive values.
    /// </summary>
    [TestFixture]
    public class When_getting_metric_states_for_a_metric_with_value_in_the_good_range_and_lower_boundary_and_metric_state_rule_set_as_inclusive : MetricStateServiceFixtureBase
    {
        private MetricState suppliedStateInContext;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(metricMetadataTreeService.Get(null)).IgnoreArguments().Return(suppliedMetricMetadataTree);
        }

        protected override void ExecuteTest()
        {
            actualModel = providerToTest.GetState(suppliedMetricId1, suppliedMetricValueForLowerLimitBoundaryTestForGood, suppliedMetricValueType1);

            //Picking the state that we are going to evaluate against.
            suppliedStateInContext = suppliedMetricMetadataTree.Descendants.Single(x => x.MetricId == suppliedMetricId1).States.Single(x => x.StateType == MetricStateType.Good);
        }

        [Test]
        public void Should_return_a_good_state_text()
        {
            Assert.That(actualModel.StateText, Is.EqualTo(suppliedStateInContext.StateText));
        }

        [Test]
        public void Should_return_a_good_state()
        {
            Assert.That(actualModel.StateType, Is.EqualTo(suppliedStateInContext.StateType));
        }
    }

    /// <summary>
    /// Testing that state is good when the metric value is in the UPPER boundary and the metric state rule has inclusive values.
    /// </summary>
    [TestFixture]
    public class When_getting_metric_states_for_a_metric_with_value_in_the_good_range_and_upper_boundary_and_metric_state_rule_set_as_inclusive : MetricStateServiceFixtureBase
    {
        private MetricState suppliedStateInContext;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(metricMetadataTreeService.Get(null)).IgnoreArguments().Return(suppliedMetricMetadataTree);
        }

        protected override void ExecuteTest()
        {
            actualModel = providerToTest.GetState(suppliedMetricId1, suppliedMetricValueForUpperLimitBoundaryTestForGood, suppliedMetricValueType1);

            //Picking the state that we are going to evaluate against.
            suppliedStateInContext = suppliedMetricMetadataTree.Descendants.Single(x => x.MetricId == suppliedMetricId1).States.Single(x => x.StateType == MetricStateType.Good);
        }

        [Test]
        public void Should_return_a_good_state_text()
        {
            Assert.That(actualModel.StateText, Is.EqualTo(suppliedStateInContext.StateText));
        }

        [Test]
        public void Should_return_a_good_state()
        {
            Assert.That(actualModel.StateType, Is.EqualTo(suppliedStateInContext.StateType));
        }
    }

    /// <summary>
    /// Testing that state is low when the metric value is in the UPPER boundary and the metric state rule has NO inclusive values.
    /// </summary>
    [TestFixture]
    public class When_getting_metric_states_for_a_metric_with_value_in_the_low_range_and_upper_boundary_and_metric_state_rule_set_as_not_inclusive : MetricStateServiceFixtureBase
    {
        private MetricState suppliedStateInContext;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(metricMetadataTreeService.Get(null)).IgnoreArguments().Return(suppliedMetricMetadataTree);
        }

        protected override void ExecuteTest()
        {
            actualModel = providerToTest.GetState(suppliedMetricId2, suppliedMetricValueForUpperLimitBoundaryTestForLowNotInclusive, suppliedMetricValueType1);

            //Picking the state that we are going to evaluate against.
            suppliedStateInContext = suppliedMetricMetadataTree.Descendants.Single(x => x.MetricId == suppliedMetricId1).States.Single(x => x.StateType == MetricStateType.Low);
        }

        [Test]
        public void Should_return_a_low_state_text()
        {
            Assert.That(actualModel.StateText, Is.EqualTo(suppliedStateInContext.StateText));
        }

        [Test]
        public void Should_return_a_low_state()
        {
            Assert.That(actualModel.StateType, Is.EqualTo(suppliedStateInContext.StateType));
        }
    }

    /// <summary>
    /// Testing that state is good when the metric value is in the LOWER boundary and the metric state rule has inclusive values.
    /// </summary>
    [TestFixture]
    public class When_getting_metric_states_for_a_metric_with_value_in_the_good_range_and_lower_boundary_and_metric_state_rule_set_as_not_inclusive : MetricStateServiceFixtureBase
    {
        private MetricState suppliedStateInContext;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(metricMetadataTreeService.Get(null)).IgnoreArguments().Return(suppliedMetricMetadataTree);
        }

        protected override void ExecuteTest()
        {
            actualModel = providerToTest.GetState(suppliedMetricId2, suppliedMetricValueForLowerLimitBoundaryTestForGoodNotInclusive, suppliedMetricValueType1);
            //actualModel = serviceToTest.Get(suppliedMetricId2, ".55", suppliedMetricValueType1);

            //Picking the state that we are going to evaluate against.
            suppliedStateInContext = suppliedMetricMetadataTree.Descendants.Single(x => x.MetricId == suppliedMetricId1).States.Single(x => x.StateType == MetricStateType.Good);
        }

        [Test]
        public void Should_return_a_good_state_text()
        {
            Assert.That(actualModel.StateText, Is.EqualTo(suppliedStateInContext.StateText));
        }

        [Test]
        public void Should_return_a_good_state()
        {
            Assert.That(actualModel.StateType, Is.EqualTo(suppliedStateInContext.StateType));
        }
    }
}
