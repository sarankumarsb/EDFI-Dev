// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests
{
    [TestFixture]
    public class When_invoking_Utilities_BoolToYesNo_method_with_false : TestFixtureBase
    {
        //The Actual Model.
        private string actualModel;

        protected override void ExecuteTest()
        {
            actualModel = false.ToYesNo();
        }

        [Test]
        public void Should_return_No()
        {
            Assert.That(actualModel, Is.EqualTo("No"));
        }
    }

    [TestFixture]
    public class When_invoking_Utilities_BoolToYesNo_method_with_true : TestFixtureBase
    {
        //The Actual Model.
        private string actualModel;

        protected override void ExecuteTest()
        {
            actualModel = true.ToYesNo();
        }

        [Test]
        public void Should_return_Yes()
        {
            Assert.That(actualModel, Is.EqualTo("Yes"));
        }
    }

    [TestFixture]
    public class When_invoking_Utilities_FormatPersonNameByLastName_method_with_First_Middle_and_Last : TestFixtureBase
    {
        //The Actual Model.
        private string actualModel;

        protected override void ExecuteTest()
        {
            actualModel = Utilities.FormatPersonNameByLastName("John", "M", "Doe");
        }

        [Test]
        public void Should_return_name_formatted_by_last_name()
        {
            Assert.That(actualModel, Is.EqualTo("Doe, John M."));
        }
    }

    [TestFixture]
    public class When_invoking_Utilities_FormatPersonNameByLastName_method_with_First_and_Last : TestFixtureBase
    {
        //The Actual Model.
        private string actualModel;

        protected override void ExecuteTest()
        {
            actualModel = Utilities.FormatPersonNameByLastName("John", string.Empty, "Doe");
        }

        [Test]
        public void Should_return_name_formatted_by_last_name()
        {
            Assert.That(actualModel, Is.EqualTo("Doe, John"));
        }
    }

    [TestFixture]
    public class When_invoking_Utilities_Metric_GetMetricName_method_with_a_metric_base_tree_that_has_3_metrics_in_the_hierarchy : TestFixtureBase
    {
        //The Actual Model.
        private string actualModel;

        //The supplied Data models.
        private MetricBase suppliedModel;

        protected override void EstablishContext()
        {
            //Prepare supplied data collections
            suppliedModel = GetMetricBaseHierarchy();
        }

        protected MetricBase GetMetricBaseHierarchy()
        {
            var m = new ContainerMetric
                        {
                            DisplayName = "Container 1",
                            Children = new List<MetricBase>
                                           {
                                               new ContainerMetric
                                                   {
                                                       DisplayName = "Container 2",
                                                       Children = new List<MetricBase>
                                                                      {
                                                                          new GranularMetric<int>{DisplayName = "Granular Metric"}
                                                                      }
                                                   }
                                           }

                        };

            setParents(m, null);

            var granular = m.DescendantsOrSelf.Single(x => x.MetricType == MetricType.GranularMetric);

            return granular;
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
            actualModel = Utilities.Metrics.GetMetricName(suppliedModel);
        }

        [Test]
        public void Should_return_meaningful_name()
        {
            Assert.That(actualModel, Is.EqualTo("Container 1 - Container 2 - Granular Metric"));
        }
    }

    [TestFixture]
    public class When_invoking_Utilities_Metric_GetMetricName_method_with_a_metric_base_tree_that_has_2_metrics_in_the_hierarchy : TestFixtureBase
    {
        //The Actual Model.
        private string actualModel;

        //The supplied Data models.
        private MetricBase suppliedModel;

        protected override void EstablishContext()
        {
            //Prepare supplied data collections
            suppliedModel = GetMetricBaseHierarchy();
        }

        protected MetricBase GetMetricBaseHierarchy()
        {
            var m = new ContainerMetric
                        {
                            DisplayName = "Container 1",

                            Children = new List<MetricBase>
                                           {
                                               new GranularMetric<int> {DisplayName = "Granular Metric"}
                                           }

                        };

            setParents(m, null);

            var granular = m.DescendantsOrSelf.Single(x => x.MetricType == MetricType.GranularMetric);

            return granular;
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
            actualModel = Utilities.Metrics.GetMetricName(suppliedModel);
        }

        [Test]
        public void Should_return_meaningful_name()
        {
            Assert.That(actualModel, Is.EqualTo("Container 1 - Granular Metric"));
        }
    }

    [TestFixture]
    public class When_invoking_Utilities_Metric_GetMetricName_method_with_a_metric_base_tree_that_has_1_metric_in_the_hierarchy : TestFixtureBase
    {
        //The Actual Model.
        private string actualModel;

        //The supplied Data models.
        private MetricBase suppliedModel;

        protected override void EstablishContext()
        {
            //Prepare supplied data collections
            suppliedModel = GetMetricBaseHierarchy();
        }

        protected MetricBase GetMetricBaseHierarchy()
        {
            return new GranularMetric<int> { DisplayName = "Granular Metric" };
        }

        protected override void ExecuteTest()
        {
            actualModel = Utilities.Metrics.GetMetricName(suppliedModel);
        }

        [Test]
        public void Should_return_meaningful_name()
        {
            Assert.That(actualModel, Is.EqualTo("Granular Metric"));
        }
    }

    [TestFixture]
    public class When_invoking_Utilities_Metric_GetMetricName_method_with_a_metric_metadata_node_tree_that_has_3_metrics_in_the_hierarchy : TestFixtureBase
    {
        //The Actual Model.
        private string actualModel;

        //The supplied Data models.
        private MetricMetadataNode suppliedModel;

        protected override void EstablishContext()
        {
            //Prepare supplied data collections
            suppliedModel = GetMetricBaseHierarchy();
        }

        protected MetricMetadataNode GetMetricBaseHierarchy()
        {
            var tree = new TestMetricMetadataTree();

            var m = new MetricMetadataNode(tree)
            {
                DisplayName = "Container 1",
                MetricType = MetricType.ContainerMetric,
                Children = new List<MetricMetadataNode>
                                           {
                                               new MetricMetadataNode(tree)
                                                   {
                                                       DisplayName = "Container 2",
                                                       MetricType = MetricType.ContainerMetric,
                                                       Children = new List<MetricMetadataNode>
                                                                      {
                                                                          new MetricMetadataNode(tree){DisplayName = "Granular Metric", MetricType = MetricType.GranularMetric}
                                                                      }
                                                   }
                                           }

            };

            tree.Children = new List<MetricMetadataNode>{m};

            var granular = m.DescendantsOrSelf.Single(x => x.MetricType == MetricType.GranularMetric);

            return granular;
        }

        protected override void ExecuteTest()
        {
            actualModel = Utilities.Metrics.GetMetricName(suppliedModel);
        }

        [Test]
        public void Should_return_meaningful_name()
        {
            Assert.That(actualModel, Is.EqualTo("Container 1 - Container 2 - Granular Metric"));
        }
    }

    [TestFixture]
    public class When_invoking_Utilities_Metric_GetMetricName_method_with_a_metric_metadata_node_tree_that_has_2_metrics_in_the_hierarchy : TestFixtureBase
    {
        //The Actual Model.
        private string actualModel;

        //The supplied Data models.
        private MetricMetadataNode suppliedModel;

        protected override void EstablishContext()
        {
            //Prepare supplied data collections
            suppliedModel = GetMetricBaseHierarchy();
        }

        protected MetricMetadataNode GetMetricBaseHierarchy()
        {
            var tree = new TestMetricMetadataTree();

            var m = new MetricMetadataNode(tree)
                        {
                            DisplayName = "Container 1",
                            MetricType = MetricType.ContainerMetric,
                            Children = new List<MetricMetadataNode>
                                           {

                                               new MetricMetadataNode(tree)
                                                   {DisplayName = "Granular Metric", MetricType = MetricType.GranularMetric}
                                           }

                        };

            tree.Children = new List<MetricMetadataNode>{m};

            var granular = m.DescendantsOrSelf.Single(x => x.MetricType == MetricType.GranularMetric);

            return granular;
        }

        //Method used to tag the parents to the hierarchy
        //private void setParents(MetricMetadataNode metric, MetricMetadataNode parentMetric)
        //{
        //    metric.Parent = parentMetric;

        //    foreach (var childMetric in metric.Children)
        //        setParents(childMetric, metric);
        //}

        protected override void ExecuteTest()
        {
            actualModel = Utilities.Metrics.GetMetricName(suppliedModel);
        }

        [Test]
        public void Should_return_meaningful_name()
        {
            Assert.That(actualModel, Is.EqualTo("Container 1 - Granular Metric"));
        }
    }
}
