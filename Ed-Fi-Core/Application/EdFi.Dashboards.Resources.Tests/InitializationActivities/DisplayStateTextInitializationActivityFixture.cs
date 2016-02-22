// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.InitializationActivities;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.InitializationActivities
{
    [TestFixture]
    public abstract class When_invoking_the_Display_State_Text_Initialization_Activity : TestFixtureBase
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
            originalStateDisplayStateText = "This should be overridden.";
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
            return new MetricMetadataNode (new TestMetricMetadataTree()) { DomainEntityType = "School" };
        }

        protected override void ExecuteTest()
        {
            var activity = new DisplayStateTextMetricInitializationActivity();
            activity.InitializeMetric(suppliedGranularMetric, null, suppliedMetricMetadataNode, null, null);
        }

        [Test]
        public void Should_override_Display_State_Text_with_the_Metric_Display_value()
        {
            Assert.That(suppliedGranularMetric.State.DisplayStateText, Is.EqualTo(suppliedGranularMetric.DisplayValue));
        }

        [Test]
        public void Should_override_State_Type_with_the_result_of_the_evaluation_of_the_Goal_against_the_metric_value()
        {
            var suppliedGoalState = (suppliedGoalInterpretation == TrendInterpretation.Standard)
                                        ? ((suppliedGranularMetricValue >= suppliedGoalValue)
                                               ? MetricStateType.Good
                                               : MetricStateType.Low)
                                        : ((suppliedGranularMetricValue <= suppliedGoalValue)
                                               ? MetricStateType.Good
                                               : MetricStateType.Low);

            Assert.That(suppliedGranularMetric.State.StateType, Is.EqualTo(suppliedGoalState));
        }
    }

    public class When_invoking_the_Display_State_Text_Initialization_Activity_with_a_school_metric_that_has_a_goal_with_Standard_interpretation_and_lower_limit_value : When_invoking_the_Display_State_Text_Initialization_Activity
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            suppliedGranularMetricValue = .49m;
            suppliedGranularMetric = GetSuppliedGranularMetric();
        }
    }

    public class When_invoking_the_Display_State_Text_Initialization_Activity_with_a_school_metric_that_has_a_goal_with_Standard_interpretation_and_higher_limit_value : When_invoking_the_Display_State_Text_Initialization_Activity
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            suppliedGranularMetricValue = .51m;
            suppliedGranularMetric = GetSuppliedGranularMetric();
        }
    }

    public class When_invoking_the_Display_State_Text_Initialization_Activity_with_a_school_metric_that_has_a_goal_with_Inverse_interpretation_and_lower_limit_value : When_invoking_the_Display_State_Text_Initialization_Activity
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            suppliedGoalInterpretation = TrendInterpretation.Inverse;
            suppliedGranularMetricValue = .49m;
            suppliedGranularMetric = GetSuppliedGranularMetric();
        }
    }

    public class When_invoking_the_Display_State_Text_Initialization_Activity_with_a_school_metric_that_has_a_goal_with_Inverse_interpretation_and_higher_limit_value : When_invoking_the_Display_State_Text_Initialization_Activity
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            suppliedGoalInterpretation = TrendInterpretation.Inverse;
            suppliedGranularMetricValue = .51m;
            suppliedGranularMetric = GetSuppliedGranularMetric();
        }
    }

    //Testing that it also works with LEA's
    public class When_invoking_the_Display_State_Text_Initialization_Activity_with_a_Local_Education_Activity_metric_that_has_a_goal_with_Standard_interpretation_and_lower_limit_value : When_invoking_the_Display_State_Text_Initialization_Activity
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            suppliedDomainEntityType = "Local Education Agency";
            suppliedGranularMetricValue = .49m;
            suppliedGranularMetric = GetSuppliedGranularMetric();
            suppliedMetricMetadataNode = GetSuppliedMetricMetadataNode();
        }
    }

    //Testing that it should not do anything if its a student metric
    public class When_invoking_the_Display_State_Text_Initialization_Activity_with_a_student_metric : When_invoking_the_Display_State_Text_Initialization_Activity
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            suppliedDomainEntityType = MetricInstanceSetType.StudentSchool.ToString();
            suppliedMetricMetadataNode = GetSuppliedMetricMetadataNode();
        }

        public new void Should_override_Display_State_Text_with_the_Metric_Display_value()
        {
            Assert.That(suppliedGranularMetric.State.StateText, Is.EqualTo(originalStateDisplayStateText));
        }

        public new void Should_override_State_Type_with_the_result_of_the_evaluation_of_the_Goal_against_the_metric_value()
        {
            Assert.That(suppliedGranularMetric.State.StateType, Is.EqualTo(originalStateStateType));
        }
    }
}
