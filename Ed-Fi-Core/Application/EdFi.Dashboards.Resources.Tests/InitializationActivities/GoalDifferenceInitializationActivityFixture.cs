// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Linq;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.InitializationActivities;
using NUnit.Framework;
using EdFi.Dashboards.Testing;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.InitializationActivities
{
    [TestFixture]
    class GoalDifferenceInitializationActivityFixture : TestFixtureBase
    {
        private GoalDifferenceMetricInitializationActivity metricInitializationActivity;
        private GranularMetric<int> granularMetricInt;
        private GranularMetric<decimal> granularMetricDecimal;
        private GranularMetric<double> granularMetricDouble;
        private GranularMetric<string> granularMetricString;
        private MetricMetadataNode metadataNodeInt;
        private MetricMetadataNode metadataNodeDecimal;
        private MetricMetadataNode metadataNodeDouble;
        private MetricMetadataNode metadataNodeString;

        protected override void EstablishContext()
        {
            //initialize Granular Metrics
            granularMetricInt = getSuppliedGranularMetric_int();
            granularMetricDecimal = getSuppliedGranularMetric_decimal();
            granularMetricDouble = getSuppliedGranularMetric_double();
            granularMetricString = getSuppliedGranularMetric_string();

            //initialize metadata nodes
            metadataNodeInt = getSuppliedMetricMetadataNode_int();
            metadataNodeDecimal = getSuppliedMetricMetadataNode_decimal();
            metadataNodeDouble = getSuppliedMetricMetadataNode_double();
            metadataNodeString = getSuppliedMetricMetadataNode_string();
        }

        private GranularMetric<int> getSuppliedGranularMetric_int()
        {
            return new GranularMetric<int>
            {
                MetricId = 1,
                Value = 5,
                Goal = new Goal { Interpretation = TrendInterpretation.Standard, Value = 5 },
                Trend = new Trend { Direction = TrendDirection.Increasing, Evaluation = TrendEvaluation.DownBad, Interpretation = TrendInterpretation.Standard, RenderingDisposition = TrendEvaluation.DownBad }
            };
        }

        private MetricMetadataNode getSuppliedMetricMetadataNode_int()
        {
            return new MetricMetadataNode(new TestMetricMetadataTree())
                       {
                           DomainEntityType = "School",
                           Format = "{0:P0}"
                       };
        }

        private GranularMetric<decimal> getSuppliedGranularMetric_decimal()
        {
            return new GranularMetric<decimal>
            {
                MetricId = 2,
                Value = 1.5m,
                Goal = new Goal { Interpretation = TrendInterpretation.Inverse, Value = 2 },
                Trend = new Trend { Direction = TrendDirection.Decreasing, Evaluation = TrendEvaluation.DownBad, Interpretation = TrendInterpretation.Inverse, RenderingDisposition = TrendEvaluation.DownBad }
            };
        }

        private MetricMetadataNode getSuppliedMetricMetadataNode_decimal()
        {
            return new MetricMetadataNode(new TestMetricMetadataTree())
            {
                DomainEntityType = "LocalEducationAgency",
                Format = "{0:P1}"
            };
        }

        private GranularMetric<double> getSuppliedGranularMetric_double()
        {
            return new GranularMetric<double>
            {
                MetricId = 2,
                Value = 200,
                Goal = new Goal { Interpretation = TrendInterpretation.Inverse, Value = 180 },
                Trend = new Trend { Direction = TrendDirection.Decreasing, Evaluation = TrendEvaluation.DownBad, Interpretation = TrendInterpretation.Inverse, RenderingDisposition = TrendEvaluation.DownBad }
            };
        }

        private MetricMetadataNode getSuppliedMetricMetadataNode_double()
        {
            return new MetricMetadataNode((new TestMetricMetadataTree()))
            {
                DomainEntityType = MetricInstanceSetType.StudentSchool.ToString(),
                Format = "{0:P0}"
            };
        }

        private GranularMetric<string> getSuppliedGranularMetric_string()
        {
            return new GranularMetric<string>
            {
                MetricId = 2,
                Value = "string",
                Goal = new Goal { Interpretation = TrendInterpretation.Inverse },
                Trend = new Trend { Direction = TrendDirection.Decreasing, Evaluation = TrendEvaluation.DownBad, Interpretation = TrendInterpretation.Inverse, RenderingDisposition = TrendEvaluation.DownBad }
            };
        }

        private MetricMetadataNode getSuppliedMetricMetadataNode_string()
        {
            return new MetricMetadataNode((new TestMetricMetadataTree()))
            {
                DomainEntityType = "School",
                Format = "{0:P0}"
            };
        }

        protected override void ExecuteTest()
        {
            metricInitializationActivity = new GoalDifferenceMetricInitializationActivity();
            metricInitializationActivity.InitializeMetric(granularMetricInt, null, metadataNodeInt, null, null);
            metricInitializationActivity.InitializeMetric(granularMetricDecimal, null, metadataNodeDecimal, null, null);
            metricInitializationActivity.InitializeMetric(granularMetricDouble, null, metadataNodeDouble, null, null);
            metricInitializationActivity.InitializeMetric(granularMetricString, null, metadataNodeString, null, null);
        }

        [Test]
        public void Should_return_granular_metric_with_the_new_extended_properties_when_appropriate()
        {
            //granularMetric_int
            Assert.NotNull(granularMetricInt.Values["DisplayGoal"]);
            Assert.NotNull(granularMetricInt.Values["GoalDifference"]);
            Assert.NotNull(granularMetricInt.Values["DisplayGoalDifference"]);

            //granularMetric_decimal 
            Assert.NotNull(granularMetricDecimal.Values["DisplayGoal"]);
            Assert.NotNull(granularMetricDecimal.Values["GoalDifference"]);
            Assert.NotNull(granularMetricDecimal.Values["DisplayGoalDifference"]);

            //granularMetric_double 
            Assert.NotNull(granularMetricDouble.Values["DisplayGoal"]);
            Assert.NotNull(granularMetricDouble.Values["GoalDifference"]);
            Assert.NotNull(granularMetricDouble.Values["DisplayGoalDifference"]);

            //granularMetric_string should not add because it is a string value type
            Assert.IsTrue(granularMetricString.Values.Count == 0);
        }

        [Test]
        public void Should_not_add_extended_properties_when_T_is_string()
        {
            Assert.IsTrue(granularMetricString.Values.Count == 0);
        }
        
        [Test]
        public void Should_return_data_correctly()
        {
            
        }
    }

    [TestFixture]
    public abstract class When_invoking_the_goal_difference_initialization_activity : TestFixtureBase
    {
        //The supplied Data models.
        protected string suppliedDomainEntityType;
        protected MetricMetadataNode suppliedMetricMetadataNode;
        protected decimal suppliedGranularMetricValue;
        protected TrendInterpretation suppliedGoalInterpretation;
        protected decimal suppliedGoalValue;
        protected GranularMetric<decimal> suppliedGranularMetric;

        protected string originalStateDisplayStateText;
        protected string originalStateStateText;
        protected MetricStateType originalStateStateType;

        protected override void EstablishContext()
        {
            //Prepare supplied data collections
            suppliedDomainEntityType = "School";
            suppliedMetricMetadataNode = GetSuppliedMetricMetadataNode();
            suppliedGranularMetricValue = .55m;
            suppliedGoalInterpretation = TrendInterpretation.Standard;
            suppliedGoalValue = .50m;
            suppliedGranularMetric = GetSuppliedGranularMetric();

            //This is for the original values in the state object for the granular metric.
            originalStateDisplayStateText = "This should be overriden.";
            originalStateStateText = "Should maintain value.";
            originalStateStateType = MetricStateType.None;
        }

        protected GranularMetric<decimal> GetSuppliedGranularMetric()
        {
            return new GranularMetric<decimal>
            {
                Value = suppliedGranularMetricValue,
                DisplayValue = string.Format("{0:P2}", suppliedGranularMetricValue),
                Goal = new Goal { Interpretation = suppliedGoalInterpretation, Value = suppliedGoalValue },
                State = new State { DisplayStateText = originalStateDisplayStateText, StateText = originalStateStateText, StateType = originalStateStateType }
            };
        }

        protected MetricMetadataNode GetSuppliedMetricMetadataNode()
        {
            return new MetricMetadataNode((new TestMetricMetadataTree())) { DomainEntityType = "School", Format = "{0:P2}" };
        }

        protected override void ExecuteTest()
        {
            var activity = new GoalDifferenceMetricInitializationActivity();
            activity.InitializeMetric(suppliedGranularMetric, null, suppliedMetricMetadataNode, null, null);
        }

        [Test]
        public void Should_add_a_extended_property_with_goal_difference_value_correctly()
        {
            var expectedGoalDifference = (suppliedGoalInterpretation == TrendInterpretation.Standard) ? (suppliedGranularMetricValue - suppliedGoalValue) : (suppliedGoalValue-suppliedGranularMetricValue);
            Assert.That(((decimal)suppliedGranularMetric.Values["GoalDifference"]), Is.EqualTo(expectedGoalDifference));
        }

        [Test]
        public void Should_add_a_extended_property_with_display_goal_value_correctly()
        {
            Assert.That(suppliedGranularMetric.Values["DisplayGoal"].ToString(), Is.EqualTo(string.Format("{0:P2}", suppliedGoalValue)));
        }

        [Test]
        public void Should_add_a_extended_property_with_display_goal_difference_value_correctly()
        {
            var expectedGoalDifference = (suppliedGoalInterpretation == TrendInterpretation.Standard) ? (suppliedGranularMetricValue - suppliedGoalValue) : (suppliedGoalValue - suppliedGranularMetricValue);
            Assert.That(suppliedGranularMetric.Values["DisplayGoalDifference"].ToString(), Is.EqualTo(string.Format("{0:P2}", expectedGoalDifference)));
        }
    }

    [TestFixture]
    public class When_invoking_the_goal_difference_initialization_activity_with_a_school_metric_that_has_a_standard_interpretation_and_the_metric_value_is_on_the_lower_limit : When_invoking_the_goal_difference_initialization_activity
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            suppliedGranularMetricValue = .49m;
            suppliedGranularMetric = GetSuppliedGranularMetric();
        }

        [Test]
        public void Should_have_not_Met_The_Goal()
        {
            var expectedGoalDifference = (suppliedGoalInterpretation == TrendInterpretation.Standard) ? (suppliedGranularMetricValue - suppliedGoalValue) : (suppliedGoalValue - suppliedGranularMetricValue);
            //Was the Goal met?
            bool expectedGoalMetValue = expectedGoalDifference > 0;//If it is positive.

            Assert.That(expectedGoalMetValue, Is.EqualTo(false));
        }
    }

    [TestFixture]
    public class When_invoking_the_goal_difference_initialization_activity_with_a_school_metric_that_has_a_standard_interpretation_and_the_metric_value_is_on_the_upper_limit : When_invoking_the_goal_difference_initialization_activity
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            suppliedGranularMetricValue = .51m;
            suppliedGranularMetric = GetSuppliedGranularMetric();
        }

        [Test]
        public void Should_have_Met_The_Goal()
        {
            var expectedGoalDifference = (suppliedGoalInterpretation == TrendInterpretation.Standard) ? (suppliedGranularMetricValue - suppliedGoalValue) : (suppliedGoalValue - suppliedGranularMetricValue);
            //Was the Goal met?
            bool expectedGoalMetValue = expectedGoalDifference > 0;//If it is positive.

            Assert.That(expectedGoalMetValue, Is.EqualTo(true));
        }
    }

    [TestFixture]
    public class When_invoking_the_goal_difference_initialization_activity_with_a_school_metric_that_has_a_Inverse_interpretation_and_the_metric_value_is_on_the_lower_limit : When_invoking_the_goal_difference_initialization_activity
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            suppliedGranularMetricValue = .49m;
            suppliedGoalInterpretation = TrendInterpretation.Inverse;
            suppliedGranularMetric = GetSuppliedGranularMetric();
        }

        [Test]
        public void Should_have_Met_The_Goal()
        {
            var expectedGoalDifference = (suppliedGoalInterpretation == TrendInterpretation.Standard) ? (suppliedGranularMetricValue - suppliedGoalValue) : (suppliedGoalValue - suppliedGranularMetricValue);
            //Was the Goal met?
            bool expectedGoalMetValue = expectedGoalDifference > 0;//If it is positive.

            Assert.That(expectedGoalMetValue, Is.EqualTo(true));
        }
    }

    [TestFixture]
    public class When_invoking_the_goal_difference_initialization_activity_with_a_school_metric_that_has_a_Inverse_interpretation_and_the_metric_value_is_on_the_upper_limit : When_invoking_the_goal_difference_initialization_activity
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            suppliedGranularMetricValue = .51m;
            suppliedGoalInterpretation = TrendInterpretation.Inverse;
            suppliedGranularMetric = GetSuppliedGranularMetric();
        }

        [Test]
        public void Should_have_not_Met_The_Goal()
        {
            var expectedGoalDifference = (suppliedGoalInterpretation == TrendInterpretation.Standard) ? (suppliedGranularMetricValue - suppliedGoalValue) : (suppliedGoalValue - suppliedGranularMetricValue);
            //Was the Goal met?
            bool expectedGoalMetValue = expectedGoalDifference > 0;//If it is positive.

            Assert.That(expectedGoalMetValue, Is.EqualTo(false));
        }
    }
}
