// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using EdFi.Dashboards.Testing;
using NUnit.Framework;

namespace EdFi.Dashboards.Metric.Resources.Models.Tests
{
    [TestFixture]
    public class TreeOrderNodeEnumerableFixture : TestFixtureBase
    {
        protected override void ExecuteTest()
        {

        }
        private MetricBase GetSuppliedEmptyMetricBaseTree()
        {
            return new ContainerMetric();
        }
        private MetricBase GetSuppliedMetricBaseTree()
        {
            return new ContainerMetric
                       {
                           MetricId = 0,
                           Children = new List<MetricBase>
                                          {
                                              new AggregateMetric
                                                  {
                                                      MetricId = 1,
                                                      Children = new List<MetricBase>
                                                                     {
                                                                         new GranularMetric<int>
                                                                             {
                                                                                 MetricId = 2
                                                                             },
                                                                          new ContainerMetric
                                                                              {
                                                                                  MetricId = 3,
                                                                                  Children = new List<MetricBase>
                                                                                  {
                                                                                        new GranularMetric<string>
                                                                                            {
                                                                                                MetricId = 4
                                                                                            },
                                                                                        new GranularMetric<double>
                                                                                            {
                                                                                                MetricId = 5
                                                                                            }
                                                                                  }
                                                                              }
                                                                     }
                                                  },
                                              new ContainerMetric
                                                  {
                                                      MetricId = 6
                                                  },
                                              new GranularMetric<int>
                                                  {
                                                      MetricId = 7
                                                  }
                                          }
                       };
        }

        [Test]
        public void Should_not_iterate_an_empty_tree()
        {
            var containerMetric = (ContainerMetric)GetSuppliedEmptyMetricBaseTree();
            foreach (var child in containerMetric.Descendants)
            {
                Assert.Fail("Should not reach this code");
            }
        }

        [Test]
        public void Should_return_tree_flattened_properly()
        {
            var containerMetric = (ContainerMetric)GetSuppliedMetricBaseTree();
            int i = 0;
            foreach (var metric in containerMetric.DescendantsOrSelf)
            {
                Assert.IsTrue(metric.MetricId == i);
                i++;
            }
        }

        [Test]
        public void Should_retain_proper_depth_through_iteration()
        {
            var containerMetric = (ContainerMetric)GetSuppliedMetricBaseTree();
            var nodeEnumerator = new TreeOrderNodeEnumerator<MetricBase>(containerMetric);
            while (nodeEnumerator.MoveNext())
            {
                switch (nodeEnumerator.Current.MetricId)
                {
                    case 1:
                    case 6:
                    case 7:
                        {
                            Assert.IsTrue(nodeEnumerator.Depth == 1);
                            break;
                        }
                    case 2:
                    case 3:
                        {
                            Assert.IsTrue(nodeEnumerator.Depth == 2);
                            break;
                        }
                    case 4:
                    case 5:
                        {
                            Assert.IsTrue(nodeEnumerator.Depth == 3);
                            break;
                        }
                }
            }
        }
    }
}
