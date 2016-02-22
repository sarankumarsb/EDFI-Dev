using System;
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Resources.Tests
{
    public class TestMetricMetadataTree : MetricMetadataTree
    {
        public override System.Collections.Generic.IEnumerable<MetricMetadataNode> Children
        {
            get
            {
                if (base.Children == null)
                    throw new InvalidOperationException("The tree's Children collection has not been initialized.  Did you forget to initialize the Children property in your test metric metadata while establishing context for the test?");

                return base.Children;
            }
            set 
            { 
                base.Children = value; 
                InitializeOptimizations();
            }
        }

        protected override void InitializeOptimizations()
        {
            SetParentsAndSiblings(this, null);

            base.InitializeOptimizations();
        }

        /// <summary>
        /// Sets parents and siblings for the metadata tree.  This should be executed only for unit testing scenarios since the 
        /// metadata tree service will initialize the parent/next sibling properties correctly.  This was done because the development 
        /// cost of retrofitting every existing unit test to set the parent/sibling properties was going to take too much time.
        /// </summary>
        /// <param name="metric"></param>
        /// <param name="parentMetric"></param>
        private void SetParentsAndSiblings(IContainerNode<MetricMetadataNode> metric, IContainerNode<MetricMetadataNode> parentMetric)
        {
            var metricNode = metric as MetricMetadataNode;

            if (metricNode != null)
            {
                metricNode.Parent = parentMetric as MetricMetadataNode;
                EnsureMetricNodeIdInitialized(metricNode);
            }

            MetricMetadataNode lastChild = null;

            foreach (var childMetric in metric.Children)
            {
                SetParentsAndSiblings(childMetric, metric);

                if (lastChild != null)
                    lastChild.NextSibling = childMetric;

                lastChild = childMetric;
            }
        }

        private int offset;

        private void EnsureMetricNodeIdInitialized(MetricMetadataNode metadataNode)
        {
            if (metadataNode.MetricNodeId == 0)
                metadataNode.MetricNodeId = int.MinValue + offset++;
        }
    }
}
