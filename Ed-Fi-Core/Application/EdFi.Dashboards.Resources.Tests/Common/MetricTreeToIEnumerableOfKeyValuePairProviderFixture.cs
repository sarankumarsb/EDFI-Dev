// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Common
{
    [TestFixture]
    public class MetricTreeToIEnumerableOfKeyValuePairProviderFixture : TestFixtureBase
    {
        //The Injected Dependencies.

        //The Actual Model.
        private IEnumerable<KeyValuePair<string,object>> actualModel;

        //The supplied Data models.
        private ContainerMetric suppliedMetricHirearchy;

        protected override void EstablishContext()
        {
            //Prepare supplied data collections
            suppliedMetricHirearchy = GetSuppliedMetricHierarchy();

            //Set up the mocks


            //Set expectations

        }

        protected ContainerMetric GetSuppliedMetricHierarchy()
        {

            var hirearchy = new ContainerMetric
            {
                MetricNodeId = 1,
                MetricType = MetricType.ContainerMetric,
                DisplayName = "Overview Root",
                Children = new List<MetricBase>
                                          {
                                              new ContainerMetric
                                                  {
                                                      MetricNodeId = 11,
                                                      MetricType = MetricType.ContainerMetric,
                                                      DisplayName="Assessments",
                                                      Children = new List<MetricBase>
                                                                     {
                                                                         new AggregateMetric
                                                                             {
                                                                               MetricNodeId   = 111,
                                                                               MetricType = MetricType.AggregateMetric,
                                                                               DisplayName="State Assessments",
                                                                               Children = new List<MetricBase>
                                                                                              {
                                                                                                  new ContainerMetric
                                                                                                      {
                                                                                                          MetricNodeId = 1111,
                                                                                                          MetricType = MetricType.ContainerMetric,
                                                                                                          DisplayName = "State Assessment one",
                                                                                                          Children = new List<MetricBase>
                                                                                                                          {
                                                                                                                              new GranularMetric<int> { MetricNodeId = 11111, DisplayName = "Assessment X 11111", Value = 11111, MetricType = MetricType.GranularMetric},
                                                                                                                              new GranularMetric<int> { MetricNodeId = 11112, DisplayName = "Assessment Y 11112",Value = 11112, MetricType = MetricType.GranularMetric },
                                                                                                                              new GranularMetric<int> { MetricNodeId = 11113, DisplayName = "Assessment Z 11113",Value = 11113, MetricType = MetricType.GranularMetric }
                                                                                                                          }
                                                                                                      }
                                                                                              }
                                                                             },
                                                                             new AggregateMetric
                                                                             {
                                                                               MetricNodeId   = 112,
                                                                               MetricType = MetricType.AggregateMetric,
                                                                               DisplayName="Other Standard Assessments",
                                                                               Children = new List<MetricBase>
                                                                                              {
                                                                                                  new ContainerMetric
                                                                                                      {
                                                                                                          MetricNodeId = 1121,
                                                                                                          MetricType = MetricType.ContainerMetric,
                                                                                                          DisplayName = "SAT",
                                                                                                          Children = new List<MetricBase>
                                                                                                                          {
                                                                                                                              new GranularMetric<int> { MetricNodeId = 11211, DisplayName = "Math 11211", Value = 11211, MetricType = MetricType.GranularMetric },
                                                                                                                              new GranularMetric<int> { MetricNodeId = 11212, DisplayName = "Science 11212", Value = 11212, MetricType = MetricType.GranularMetric },
                                                                                                                              new GranularMetric<int> { MetricNodeId = 11213, DisplayName = "Social Studies 11213", Value = 11213, MetricType = MetricType.GranularMetric }
                                                                                                                          }
                                                                                                      }
                                                                                              }
                                                                             },
                                                                     }
                                                  },
                                            new ContainerMetric
                                                {
                                                    MetricNodeId = 12,
                                                    MetricType = MetricType.ContainerMetric,
                                                    DisplayName = "Attendance & Discipline",
                                                    Children = new List<MetricBase>
                                                                   {
                                                                       new GranularMetric<int> { MetricNodeId = 121, DisplayName = "Attendance 121", Value = 121, MetricType = MetricType.GranularMetric },
                                                                       new GranularMetric<int> { MetricNodeId = 122, DisplayName = "Discipline 122", Value = 122, MetricType = MetricType.GranularMetric }
                                                                   }
                                                }
                                          }
            };

            setParents(hirearchy, null);

            return hirearchy;
        }

        //Method used to tag the parents to the hierarchy
        private void setParents(MetricBase metric, MetricBase parentMetric)
        {
            metric.Parent = parentMetric;

            var container = metric as ContainerMetric;

            if (container != null)
                foreach (var childMetric in container.Children)
                    setParents(childMetric, metric);
        }

        protected override void ExecuteTest()
        {
            var provider = new MetricTreeToIEnumerableOfKeyValuePairProvider();
            actualModel = provider.FlattenMetricTree(suppliedMetricHirearchy);
        }

        [Test]
        public void Should_add_granular_metric_properties_on_model()
        {
            var granularMetrics = suppliedMetricHirearchy.DescendantsOrSelf.Where(x => x.MetricType == MetricType.GranularMetric);

            Assert.That(granularMetrics.Count(), Is.GreaterThan(0));

            var i = 0;
            foreach (dynamic suppliedMetric in granularMetrics)
            {
                Assert.IsTrue(actualModel.ElementAt(i).Key == createPropName(suppliedMetric), "Property names don't match.");
                Assert.IsTrue(actualModel.ElementAt(i).Value.Equals(suppliedMetric.Value as object), "Values are different.");
                i++;
            }
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }

        private string createPropName(MetricBase metric)
        {
            var parentName = "";
            var grandParentName = "";

            if (metric.Parent != null)
            {
                parentName = metric.Parent.DisplayName + " - ";

                if (metric.Parent.Parent != null)
                    if (metric.Parent.Parent.MetricType == MetricType.ContainerMetric)
                        grandParentName = metric.Parent.Parent.DisplayName + " - ";
            }

            return grandParentName + parentName + metric.DisplayName;
        }
    }
}
