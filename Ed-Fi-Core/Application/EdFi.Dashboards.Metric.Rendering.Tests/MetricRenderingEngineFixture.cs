// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Metric.Resources.Models;
using NUnit.Framework;
using EdFi.Dashboards.Testing;
using Moq;
using Rhino.Mocks;

namespace EdFi.Dashboards.Metric.Rendering.Tests
{
    /// <summary>
    /// Holds the pairing of corresponding expected and actual source data for use in assertions.
    /// </summary>
    public class ExpectedActualPairing
    {
        public ExpectedActualPairing(MetricBase expected, Dictionary<string, string> actual)
        {
            this._expected = expected;
            this._actual = actual;
        }

        private readonly MetricBase _expected;

        public MetricBase Expected
        {
            get { return _expected; }
        }

        private readonly Dictionary<string, string> _actual;

        public Dictionary<string, string> Actual
        {
            get { return _actual; }
        }
    }

    [TestFixture]
    public class When_calling_render_metrics_from_metric_rendering_engine_fixture : TestFixtureBase
    {
        private MetricRenderingEngine metricRenderingEngine;
        private MetricBase metricBase;
        private ContainerMetric rootMetric;

        private Mock<IMetricRenderer> metricRenderer = new Mock<IMetricRenderer>();
        private Mock<IMetricTemplateBinder> metricTemplateBinder = new Mock<IMetricTemplateBinder>();
        private Mock<IMetricRenderingContextProvider> metricRenderingContextProvider = new Mock<IMetricRenderingContextProvider>();

        private List<Dictionary<string, string>> postedContextValues = new List<Dictionary<string, string>>();
        private List<MetricRenderingParameters> postedMetricRenderingValues = new List<MetricRenderingParameters>();
        private IDictionary<string,object> contextDictionary = new Dictionary<string, object>();
        private List<ExpectedActualPairing> assertionPairs;
        private RenderingMode suppliedRenderingMode;

        protected override void EstablishContext()
        {
            metricTemplateBinder.Setup(x => x.GetTemplateName(It.IsAny<Dictionary<string, string>>())).Callback<Dictionary<string, string>>(x => postedContextValues.Add(x)).Returns("template name");
            metricRenderer.Setup(x => x.Render(It.IsAny<String>(), It.IsAny<MetricBase>(), It.IsAny<int>(), It.IsAny<IDictionary<string, object>>())).Callback((string templateName, object mb, int depth, IDictionary<string, object> dictionary) => postedMetricRenderingValues.Add(new MetricRenderingParameters { TemplateName = templateName, MetricBase = (MetricBase)mb, Depth = depth }));

            metricBase = GetSuppliedMetricBase();

            metricRenderingContextProvider.Setup(x => x.ProvideContext(It.IsAny<MetricBase>(), It.IsAny<IDictionary<string, object>>())).Callback((MetricBase m, IDictionary<string, object> d) => contextDictionary["AggregateMetricId"] = m.MetricId);
            rootMetric = (ContainerMetric) metricBase;

            suppliedRenderingMode = RenderingMode.Overview;
        }

        protected override void ExecuteTest()
        {
            metricRenderingEngine = new MetricRenderingEngine(metricTemplateBinder.Object, metricRenderingContextProvider.Object);
            metricRenderingEngine.RenderMetrics(metricBase, suppliedRenderingMode, metricRenderer.Object, null);

            assertionPairs = 
                (from m in rootMetric.DescendantsOrSelf
                from cv in postedContextValues
                where m.MetricId.ToString() == cv["MetricId"]
                    && m.MetricVariantType.ToString() == cv["MetricVariantType"]
                select new ExpectedActualPairing(m, cv))
                .ToList();

            // Make sure that we have something to verify (otherwise we'll have very quiet passing tests)
            Assert.That(assertionPairs.Count(), Is.GreaterThan(0));
        }

        private MetricBase GetSuppliedMetricBase()
        {
            var tree = new ContainerMetric
                       {
                           MetricId = 1,
                           MetricVariantId = 1000, // This is intentionally MetricId * 1000
                           MetricVariantType = MetricVariantType.CurrentYear,
                           DomainEntityType = "StudentSchool",
                           Children = new List<MetricBase>
                                          {
                                            new GranularMetric<int>
                                                {
                                                    MetricId = 2,
                                                    MetricVariantId = 2000, // This is intentionally MetricId * 1000
                                                    MetricVariantType = MetricVariantType.CurrentYear,
                                                    Value = 5,
                                                    Enabled = false
                                                },
                                            new AggregateMetric
                                                {
                                                    MetricId = 5,
                                                    MetricVariantId = 5000, // This is intentionally MetricId * 1000
                                                    MetricVariantType = MetricVariantType.CurrentYear,
                                                    Children = new List<MetricBase>
                                                                    {
                                                                        new GranularMetric<decimal>
                                                                            {
                                                                                MetricId = 10,
                                                                                MetricVariantId = 10000, // This is intentionally MetricId * 1000
                                                                                MetricVariantType = MetricVariantType.CurrentYear,
                                                                                Value = 99,
                                                                                Enabled = true
                                                                            },
                                                                        new ContainerMetric
                                                                            {
                                                                                MetricId = 11,
                                                                                MetricVariantId = 11000, // This is intentionally MetricId * 1000
                                                                                MetricVariantType = MetricVariantType.CurrentYear,
                                                                                Children = new List<MetricBase>
                                                                                               {
                                                                                                   new GranularMetric<decimal?>
                                                                                                       {
                                                                                                           MetricId = 12,
                                                                                                           MetricVariantId = 12000, // This is intentionally MetricId * 1000
                                                                                                           MetricVariantType = MetricVariantType.CurrentYear,
                                                                                                           Value = null
                                                                                                       }
                                                                                               }
                                                                            },
                                                                        new ContainerMetric
                                                                            {
                                                                                MetricId = 13,
                                                                                MetricVariantId = 13000, // This is intentionally MetricId * 1000
                                                                                MetricVariantType = MetricVariantType.CurrentYear,
                                                                                Children = new List<MetricBase>
                                                                                               {
                                                                                                   new GranularMetric<decimal?>
                                                                                                       {
                                                                                                           MetricId = 14,
                                                                                                           MetricVariantId = 14000, // This is intentionally MetricId * 1000
                                                                                                           MetricVariantType = MetricVariantType.CurrentYear,
                                                                                                           Value = null
                                                                                                       },
                                                                                                   new GranularMetric<decimal?>
                                                                                                       {
                                                                                                           MetricId = 15,
                                                                                                           MetricVariantId = 15000, // This is intentionally MetricId * 1000
                                                                                                           MetricVariantType = MetricVariantType.CurrentYear,
                                                                                                           Value = 3.4m
                                                                                                       },
                                                                                                   new GranularMetric<decimal?>
                                                                                                       {
                                                                                                           MetricId = 15, // This is intentionally 15 (matching previous entry), testing the metric variants
                                                                                                           MetricVariantId = 30000, // This is intentionally MetricId * 2000
                                                                                                           MetricVariantType = MetricVariantType.PriorYear,
                                                                                                           Value = 3.4m
                                                                                                       }
                                                                                               }
                                                                            }
                                                                    }
                                                }
                                          }
                       };

            InitializeParent(tree, tree.Children);

            return tree;
        }

        private void InitializeParent(ContainerMetric parent, IEnumerable<MetricBase> children )
        {
            foreach (var metric in children)
            {
                metric.Parent = parent;
                var container = metric as ContainerMetric;
                if (container != null)
                    InitializeParent(container, container.Children);
            }
        }

        [Test]
        public void Should_have_correct_number_of_template_invocations()
        {
            Assert.That(postedContextValues.Count, Is.EqualTo(20));
        }

        [Test]
        public void Should_correctly_determine_metric_instance_set_type()
        {
            assertionPairs.Each(
                p =>
                    {
                        // Make sure DomainEntityType is copied to MetricInstanceSetType, if it's present
                        if (string.IsNullOrEmpty(p.Expected.DomainEntityType))
                            Assert.That(p.Actual["MetricInstanceSetType"], Is.EqualTo(String.Empty));
                        else
                            Assert.That(p.Actual["MetricInstanceSetType"], Is.EqualTo(p.Expected.DomainEntityType));
                    });
        }

        [Test]
        public void Should_correctly_determine_metric_type()
        {
            assertionPairs.Each(
                p =>
                    {
                        // Trim off type name at generic marker, if present
                        string expectedTypeName = p.Expected.GetType().Name.Split('`')[0];

                        Assert.That(p.Actual["MetricType"], Is.EqualTo(expectedTypeName));
                    });
        }

        [Test]
        public void Should_correctly_determine_depth()
        {
            assertionPairs.Each(
                p =>
                    {
                        int expectedDepth;

                        // Define expected depths for each metric by MetricId based on test data provided above
                        switch (p.Expected.MetricId)
                        {
                            // ------------------
                            //  Root level nodes
                            // ------------------
                            case 1:
                                expectedDepth = 0;
                                break;
                            // -------------------
                            //  First level nodes
                            // -------------------
                            case 2:
                            case 5:
                                expectedDepth = 1;
                                break;
                            // --------------------
                            //  Second level nodes
                            // --------------------
                            case 10:
                            case 11:
                            case 13:
                                expectedDepth = 2;
                                break;
                            // -------------------
                            //  Third level nodes
                            // -------------------
                            case 12:
                            case 14:
                            case 15:
                            case 16:
                                expectedDepth = 3;
                                break;
                            default:
                                throw new Exception(string.Format("Expected depth for metric id '{0}' has not been defined in the unit test.", p.Expected.MetricId));
                        }

                        Assert.That(p.Actual["Depth"], Is.EqualTo("Level" + expectedDepth));
                    });
        }

        [Test]
        public void Should_correctly_determine_enabled()
        {
            assertionPairs.Each(
                p => Assert.That(p.Actual["Enabled"], Is.EqualTo(p.Expected.Enabled.ToString().ToLower())));
        }

        [Test]
        public void Should_correctly_determine_parent_metric_id()
        {
            assertionPairs.Each(
                p =>
                    {
                        if (p.Expected == rootMetric)
                            // Root metric has no parent, so no context value
                            Assert.That(p.Actual["ParentMetricId"], Is.EqualTo(String.Empty));
                        else
                            Assert.That(p.Actual["ParentMetricId"], Is.EqualTo(p.Expected.Parent.MetricId.ToString()));
                    });
        }

        [Test]
        [Ignore("GrandParentMetricDI is not even an index in these arrays anymore. Is it needed? WD")]
        public void Should_correctly_determine_grand_parent_metric_id()
        {
            assertionPairs.Each(
                p =>
                    {
                        if (p.Expected == rootMetric || p.Expected.Parent == rootMetric)
                            // Root metric and first level children have no grand parent metrics, so no context value
                            Assert.That(p.Actual["GrandParentMetricId"], Is.EqualTo(String.Empty));
                        else
                            Assert.That(p.Actual["GrandParentMetricId"], Is.EqualTo(p.Expected.Parent.Parent.MetricId.ToString()));
                    });
        }

        [Test]
        public void Should_correctly_determine_metric_id()
        {
            assertionPairs.Each(
                p =>
                    {
                        int expectedMetricId;

                        if (p.Expected.MetricVariantType == MetricVariantType.CurrentYear)
                            expectedMetricId = p.Expected.MetricVariantId/1000;
                        else
                            expectedMetricId = p.Expected.MetricVariantId/2000;

                        Assert.That(p.Actual["MetricId"], Is.EqualTo(expectedMetricId.ToString()));
                    });
        }

        [Test]
        public void Should_correctly_determine_parent_metric_variant_id()
        {
            assertionPairs.Each(
                p =>
                {
                    // Root node doesn't have a parent
                    if (p.Expected == rootMetric)
                        Assert.That(p.Actual["ParentMetricVariantId"], Is.EqualTo(String.Empty));
                    else
                        Assert.That(p.Actual["ParentMetricVariantId"], Is.EqualTo(p.Expected.Parent.MetricVariantId.ToString()));
                });
        }

        [Test]
        public void Should_correctly_determine_metric_variant_id()
        {
            assertionPairs.Each(
                p =>
                    {
                        int expectedMetricVariantId;

                        if (p.Expected.MetricVariantType == MetricVariantType.CurrentYear)
                            expectedMetricVariantId = p.Expected.MetricId * 1000;
                        else
                            expectedMetricVariantId = p.Expected.MetricId * 2000;

                        Assert.That(p.Actual["MetricVariantId"], Is.EqualTo(expectedMetricVariantId.ToString()));
                    });
        }

        [Test]
        public void Should_correctly_determine_metric_variant_type()
        {
            assertionPairs.Each(
                p =>
                {
                    // Use formula of Ids to infer expected variant type (For "CurrentYear", Id multiple is 1000x, for "Prior Year", it's 2000x)
                    if (p.Expected.MetricVariantId == p.Expected.MetricId * 1000)
                        Assert.That(p.Actual["MetricVariantType"], Is.EqualTo(MetricVariantType.CurrentYear.ToString()));
                    else
                        Assert.That(p.Actual["MetricVariantType"], Is.EqualTo(MetricVariantType.PriorYear.ToString()));
                });
        }

        [Test]
        public void Should_correctly_determine_rendering_mode()
        {
            assertionPairs.Each(
                p => Assert.That(p.Actual["RenderingMode"], Is.EqualTo(suppliedRenderingMode.ToString())));
        }

        [Test]
        public void Should_correctly_determine_null_value()
        {
            assertionPairs.Where(p => p.Expected is IGranularMetric).Each(
                p =>
                    {
                        var granularMetric = (IGranularMetric) p.Expected;

                        Assert.That(p.Actual["NullValue"], Is.EqualTo((granularMetric.Value == null).ToString().ToLower()));
                });
        }

        [Test]
        public void Should_correctly_render_metrics_in_depth_first_order_with_open_and_closing_templates()
        {
            int i = 0;

            // Container metric 1
            AssertIsOpening(1, ref i);
            {
                // Granular metric 2
                AssertIsOpening(2, ref i);
                AssertIsClosing(2, ref i);

                // Aggregate metric 5
                AssertIsOpening(5, ref i);
                {
                    // Granular metric 10
                    AssertIsOpening(10, ref i);
                    AssertIsClosing(10, ref i);

                    // Container metric 11
                    AssertIsOpening(11, ref i);
                    {
                        // Granular metric 12
                        AssertIsOpening(12, ref i);
                        AssertIsClosing(12, ref i);
                    }

                    // Container metric 11 (closing)
                    AssertIsClosing(11, ref i);

                    // Container metric 13
                    AssertIsOpening(13, ref i);

                    {
                        // Granular metric 14
                        AssertIsOpening(14, ref i);
                        AssertIsClosing(14, ref i);

                        // Granular metric 15, Current Year
                        AssertIsOpening(15, MetricVariantType.CurrentYear, ref i);
                        AssertIsClosing(15, MetricVariantType.CurrentYear, ref i);

                        // Granular metric 15, Prior Year
                        AssertIsOpening(15, MetricVariantType.PriorYear, ref i);
                        AssertIsClosing(15, MetricVariantType.PriorYear, ref i);
                    }
                 
                    // Container metric 13 (closing)
                    AssertIsClosing(13, ref i);
                }

                // Aggregate metric 5 (closing)
                AssertIsClosing(5, ref i);
            }

            // Container metric 1 (closing)
            AssertIsClosing(1, ref i);
        }

        #region Open/Close support methods

        private void AssertIsOpening(int metricId, ref int i)
        {
            string metricIdText = metricId.ToString();

            Assert.That(postedContextValues[i]["MetricId"], Is.EqualTo(metricIdText), GetOpenCloseDiagnosticMessage(i));
            Assert.That(postedContextValues[i++]["Open"], Is.EqualTo("true"));
        }

        private void AssertIsClosing(int metricId, ref int i)
        {
            string metricIdText = metricId.ToString();

            Assert.That(postedContextValues[i]["MetricId"], Is.EqualTo(metricIdText), GetOpenCloseDiagnosticMessage(i));
            Assert.That(postedContextValues[i++]["Open"], Is.EqualTo("false"));
        }

        private void AssertIsOpening(int metricId, MetricVariantType metricVariantType, ref int i)
        {
            string metricIdText = metricId.ToString();

            Assert.That(postedContextValues[i]["MetricId"], Is.EqualTo(metricIdText), GetOpenCloseDiagnosticMessage(i));
            Assert.That(postedContextValues[i]["MetricVariantType"], Is.EqualTo(metricVariantType.ToString()), GetOpenCloseDiagnosticMessage(i)); 
            Assert.That(postedContextValues[i++]["Open"], Is.EqualTo("true"));
        }

        private void AssertIsClosing(int metricId, MetricVariantType metricVariantType, ref int i)
        {
            string metricIdText = metricId.ToString();

            Assert.That(postedContextValues[i]["MetricId"], Is.EqualTo(metricIdText), GetOpenCloseDiagnosticMessage(i));
            Assert.That(postedContextValues[i]["MetricVariantType"], Is.EqualTo(metricVariantType.ToString()), GetOpenCloseDiagnosticMessage(i));
            Assert.That(postedContextValues[i++]["Open"], Is.EqualTo("false"));
        }

        private string GetOpenCloseDiagnosticMessage(int i)
        {
            return "Context values: { MetricId: " + postedContextValues[i]["MetricId"] + ", Open: " + postedContextValues[i]["Open"] + "}";
        }
        #endregion

        private class MetricRenderingParameters
        {
            public string TemplateName { get; set; }
            public MetricBase MetricBase { get; set; }
            public int Depth { get; set; }
        }
    }
}
