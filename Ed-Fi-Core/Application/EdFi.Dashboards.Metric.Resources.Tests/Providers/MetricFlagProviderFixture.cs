// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Tests;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using EdFi.Dashboards.Metric.Data.Entities;

namespace EdFi.Dashboards.Metric.Resources.Tests.Providers
{
    [TestFixture]
    class MetricFlagProviderFixture : TestFixtureBase
    {
        private MetricFlagProvider provider;
        private MetricMetadataNode metadataNode;
        private MetricData suppliedMetricDataFlagged;
        private MetricData suppliedMetricDataNotFlagged;
        private GranularMetric<Int32> suppliedGranularMetricFlagged;
        private GranularMetric<Int32> suppliedGranularMetricNotFlagged;

        protected override void EstablishContext()
        {
            metadataNode = getSuppliedMetaDataNode();
            suppliedGranularMetricFlagged = new GranularMetric<int> { MetricId = 20 };
            suppliedGranularMetricNotFlagged = new GranularMetric<int> { MetricId = 40 };
            suppliedMetricDataFlagged = new CurrentYearMetricData { MetricInstances = (new List<MetricInstance>() { new MetricInstance { MetricId = 20, Flag = true } }).AsQueryable() };
            suppliedMetricDataNotFlagged = new CurrentYearMetricData { MetricInstances = (new List<MetricInstance>() { new MetricInstance { MetricId = 40, Flag = false } }).AsQueryable() };
        }

        protected override void ExecuteTest()
        {
            provider = new MetricFlagProvider();
        }

        private MetricMetadataNode getSuppliedMetaDataNode()
        {
            var tree = new TestMetricMetadataTree();

            return new MetricMetadataNode(tree)
            {
                MetricNodeId = 1,
                MetricId = 10,
                RootNodeId = 1,
                DisplayName = "DisplayName1",
                Name = "MetricName1",
                ShortName = "Metric Short Name1",
                Description = "MetricDescription1",
                ChildDomainEntityMetricId = 1,
                Enabled = true,
                Format = "{0:P1}",
                ListFormat = "{0:p1}",
                DisplayOrder = 10,
                Tooltip = "tool tip 1",
                MetricType = Models.MetricType.ContainerMetric,
                Url = "metric url1",
                NumeratorDenominatorFormat = "N D Format1",
                TrendInterpretation = 1,
                //Actions = new List<MetricAction>(),
                //States = new List<MetricState>(),
                Children = new List<MetricMetadataNode>
                                                     {
                                                new MetricMetadataNode(tree)
                                                   {
                                                       MetricNodeId = 2,
                                                       MetricId = 20,
                                                       RootNodeId = 1,
                                                       DisplayName = "DisplayName20",
                                                       Name = "MetricName20",
                                                       ShortName = "Metric Short Name20",
                                                       Description = "MetricDescription20",
                                                       ChildDomainEntityMetricId = 1,
                                                       Enabled = true,
                                                       Format = "{0:P20}",
                                                       ListFormat = "{0:p20}",
                                                       DisplayOrder = 10,
                                                       Tooltip = "tool tip 20",
                                                       MetricType = Models.MetricType.GranularMetric,
                                                       Url = "metric url20",
                                                       NumeratorDenominatorFormat = "N D Format20",
                                                       TrendInterpretation = 1,
                                                       //Actions = new List<MetricAction>(),
                                                       //States = new List<MetricState>(),
                                                   },
                                                   new MetricMetadataNode(tree)
                                                   {
                                                       MetricNodeId = 3,
                                                       MetricId = 30,
                                                       RootNodeId = 1,
                                                       DisplayName = "DisplayName20",
                                                       Name = "MetricName20",
                                                       ShortName = "Metric Short Name20",
                                                       Description = "MetricDescription20",
                                                       ChildDomainEntityMetricId = 1,
                                                       Enabled = true,
                                                       Format = "{0:P20}",
                                                       ListFormat = "{0:p20}",
                                                       DisplayOrder = 10,
                                                       Tooltip = "tool tip 20",
                                                       MetricType = Models.MetricType.AggregateMetric,
                                                       Url = "metric url20",
                                                       NumeratorDenominatorFormat = "N D Format20",
                                                       TrendInterpretation = -1,
                                                       //Actions = new List<MetricAction>(),
                                                       //States = new List<MetricState>(),
                                                       Children = new List<MetricMetadataNode>
                                                                                 {
                                                                                     new MetricMetadataNode(tree)
                                                                                           {
                                                                                               MetricNodeId = 4,
                                                                                               MetricId = 40,
                                                                                               RootNodeId = 1,
                                                                                               DisplayName = "DisplayName20",
                                                                                               Name = "MetricName20",
                                                                                               ShortName = "Metric Short Name20",
                                                                                               Description = "MetricDescription20",
                                                                                               ChildDomainEntityMetricId = 1,
                                                                                               Enabled = true,
                                                                                               Format = "{0:P20}",
                                                                                               ListFormat = "{0:p20}",
                                                                                               DisplayOrder = 10,
                                                                                               Tooltip = "tool tip 20",
                                                                                               MetricType = Models.MetricType.GranularMetric,
                                                                                               Url = "metric url20",
                                                                                               NumeratorDenominatorFormat = "N D Format20",
                                                                                               TrendInterpretation = -1,
                                                                                               //Actions = new List<MetricAction>(),
                                                                                               //States = new List<MetricState>(),
                                                                                           }
                                                                                 }
                                                }
                                        },
            };
        }

        [Test]
        public void Should_not_return_null()
        {
            Assert.NotNull(provider.GetMetricFlag(suppliedGranularMetricFlagged, metadataNode, suppliedMetricDataFlagged));
        }

        [Test]
        public void Should_return_correct_values()
        {
            Assert.IsTrue(provider.GetMetricFlag(suppliedGranularMetricFlagged, metadataNode, suppliedMetricDataFlagged));
            Assert.IsFalse(provider.GetMetricFlag(suppliedGranularMetricNotFlagged, metadataNode, suppliedMetricDataNotFlagged));
        }
    }
}
