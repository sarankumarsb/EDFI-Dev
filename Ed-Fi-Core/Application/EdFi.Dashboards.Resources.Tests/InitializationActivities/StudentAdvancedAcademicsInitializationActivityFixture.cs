// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.InitializationActivities;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.InitializationActivities
{
    [TestFixture]
    public abstract class When_invoking_Student_Advanced_Academics_Initialization_Activity : TestFixtureBase
    {
        //The Injected Dependencies.
        protected IDomainSpecificMetricNodeResolver domainSpecificMetricNodeResolver;

        //The Actual Model.
        protected IGranularMetric actualModel;

        //The supplied Data models.
        protected IGranularMetric suppliedGranularMetric;
        protected MetricMetadataNode suppliedMetricMetadataNode;

        protected override void EstablishContext()
        {
            //Prepare supplied data collections
            suppliedGranularMetric = GetSuppliedGM();
            suppliedMetricMetadataNode = GetSuppliedMetricMetadataNode();

            //Set up the mocks
            domainSpecificMetricNodeResolver = mocks.StrictMock<IDomainSpecificMetricNodeResolver>();

            //Set expectations
            var tree = new TestMetricMetadataTree();
            Expect.Call(domainSpecificMetricNodeResolver.GetAdvancedCourseEnrollmentMetricNode()).Return(new MetricMetadataNode(tree) { MetricId = 1 });
            Expect.Call(domainSpecificMetricNodeResolver.GetAdvancedCoursePotentialMetricNode()).Return(new MetricMetadataNode(tree) { MetricId = 2 });
        }

        protected virtual GranularMetric<int> GetSuppliedGM()
        {
            return new GranularMetric<int>
            {
                Parent = new ContainerMetric { MetricId = 1 },
                State = new State
                {
                    StateType = MetricStateType.Good,
                    StateText = "Default StateText",
                    DisplayStateText = "Default DisplayStateText",
                },
                Values = new Hashtable
                                        {
                                            {"Mastery", "Yes"},
                                            {"Taking", "Maybe"}
                                        }

            };
        }

        protected virtual MetricMetadataNode GetSuppliedMetricMetadataNode()
        {
            var tree = new TestMetricMetadataTree();

            return new MetricMetadataNode(tree)
            {
                DomainEntityType = MetricInstanceSetType.StudentSchool.ToString(),
            };
        }
    

        protected override void ExecuteTest()
        {
            var activity = new StudentAdvancedAcademicsMetricInitializationActivity(domainSpecificMetricNodeResolver);
            activity.InitializeMetric(suppliedGranularMetric, null, suppliedMetricMetadataNode, null, null);
        }
    }

    [TestFixture]
    public class When_invoking_Student_Advanced_Academics_Initialization_Activity_on_a_student_metric_that_has_Mastery : When_invoking_Student_Advanced_Academics_Initialization_Activity
    {
        protected override GranularMetric<int> GetSuppliedGM()
        {
            return new GranularMetric<int>
            {
                Parent = new ContainerMetric { MetricId = 1 },
                State = new State
                {
                    StateType = MetricStateType.Good,
                    StateText = "Default StateText",
                    DisplayStateText = "Default DisplayStateText",
                },
                Values = new Hashtable { {"Mastery", "Yes"}, }
            };
        }

        [Test]
        public void Should_override_the_Display_State_Text()
        {
            Assert.That(suppliedGranularMetric.State.DisplayStateText, Is.EqualTo("Yes"));
        }
    }

    [TestFixture]
    public class When_invoking_Student_Advanced_Academics_Initialization_Activity_on_a_student_metric_that_has_Taking : When_invoking_Student_Advanced_Academics_Initialization_Activity
    {

        protected override GranularMetric<int> GetSuppliedGM()
        {
            return new GranularMetric<int>
            {
                Parent = new ContainerMetric { MetricId = 2 },
                State = new State
                {
                    StateType = MetricStateType.Good,
                    StateText = "Default StateText",
                    DisplayStateText = "Default DisplayStateText",
                },
                Values = new Hashtable { {"Taking", "Maybe"} }

            };
        }

        [Test]
        public void Should_override_the_Display_State_Text()
        {
            Assert.That(suppliedGranularMetric.State.DisplayStateText, Is.EqualTo("Maybe"));
        }
    }

    [TestFixture]
    public class When_invoking_Student_Advanced_Academics_Initialization_Activity_on_a_metric_that_is_not_a_student_one : When_invoking_Student_Advanced_Academics_Initialization_Activity
    {
        protected override void EstablishContext()
        {
            //Prepare supplied data collections
            suppliedGranularMetric = GetSuppliedGM();
            suppliedMetricMetadataNode = GetSuppliedMetricMetadataNode();

            //Set up the mocks
            domainSpecificMetricNodeResolver = mocks.StrictMock<IDomainSpecificMetricNodeResolver>();

            //No Set expectations because the code does not get to call them.
            
        }

        protected override MetricMetadataNode GetSuppliedMetricMetadataNode()
        {
            var tree = new TestMetricMetadataTree();

            return new MetricMetadataNode(tree) { DomainEntityType = "School" };
        }

        [Test]
        public void Should_override_the_Display_State_Text()
        {
            Assert.That(suppliedGranularMetric.State.DisplayStateText, Is.EqualTo("Default DisplayStateText"));
        }
    }
}
