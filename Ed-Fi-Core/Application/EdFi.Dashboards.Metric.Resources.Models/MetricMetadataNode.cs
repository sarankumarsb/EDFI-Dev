// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Infrastructure.Implementations;
using Newtonsoft.Json;

namespace EdFi.Dashboards.Metric.Resources.Models
{
    public enum MetricType
    {
        None = 0,
        ContainerMetric = 1,
        AggregateMetric = 2,
        GranularMetric = 3
    }

    public enum MetricVariantType
    {
        None = 0,
        CurrentYear = 1,
        PriorYear = 2
    }

    /// <summary>
    /// Contains metric metadata for a single node in a metric metadata hierarchy.
    /// </summary>
    [Serializable]
    public class MetricMetadataNode : IContainerNode<MetricMetadataNode>
    {
        /// <summary>
        /// Default constructor (added for serialization purposes only).
        /// </summary>
        public MetricMetadataNode() : this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricMetadataNode"/> class.
        /// </summary>
        public MetricMetadataNode(MetricMetadataTree tree)
        {
            Tree = tree;
            Children = new List<MetricMetadataNode>();
            Actions = new List<MetricAction>();
            States = new List<MetricState>();
        }

        [JsonIgnore, IgnoreDataMember] // Don't need to serialize root/tree references to JSON
        public MetricMetadataTree Tree { get; set; }
        public int MetricNodeId { get; set; } //9/29/2011 is the metric instance id from metric.metricinstance
        public int MetricVariantId { get; set; }
        public int MetricId { get; set; }
        [JsonIgnore, IgnoreDataMember] // Don't need to serialize parent references to JSON
        public MetricMetadataNode Parent { get; set; }
        public int RootNodeId { get; set; }
        public string DisplayName { get; set; }
        public int DisplayOrder { get; set; }
        public MetricType MetricType { get; set; }
        public MetricVariantType MetricVariantType { get; set; }
        public string DomainEntityType { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Tooltip { get; set; }
        public string Format { get; set; }
        public string ListFormat { get; set; }
        public string ListDataLabel { get; set; }
        public string NumeratorDenominatorFormat { get; set; }
        public int? TrendInterpretation { get; set; }
        public bool Enabled { get; set; }
        public int? ChildDomainEntityMetricId { get; set; }

        private List<MetricMetadataNode> children;

        /// <summary>
        /// Gets or sets the children for the current node in the metadata hierarchy.
        /// </summary>
        public IEnumerable<MetricMetadataNode> Children
        {
            get { return children; }
            set { children = value.ToList(); }
        }

        /// <summary>
        /// Gets or sets the actions available to the metric (i.e. for linking to other web pages, or displaying drilldown charts and lists).
        /// </summary>
        public IEnumerable<MetricAction> Actions { get; set; }
        
        /// <summary>
        /// Gets or sets the states associated with the metric (applied based on minimum and maximum thresholds of the metric's value).
        /// </summary>
        public IEnumerable<MetricState> States { get; set; }

        /// <summary>
        /// Gets an enumerable sequence of metadata nodes up the hierarchy.
        /// </summary>
        [JsonIgnore, IgnoreDataMember]
        public IEnumerable<MetricMetadataNode> Ancestors
        {
            get
            {
                var current = this;

                while (current.Parent != null)
                {
                    yield return current.Parent;
                    current = current.Parent;
                }
            }
        }

        /// <summary>
        /// Gets an enumerable sequence of metadata nodes up the hierarchy, including the current node.
        /// </summary>
        [JsonIgnore, IgnoreDataMember]
        public IEnumerable<MetricMetadataNode> AncestorsOrSelf
        {
            get
            {
                yield return this;

                foreach (var node in Ancestors)
                    yield return node;
            }
        }

        /// <summary>
        /// Finds a metric metadata node in the Descendants collection by metric Id using an index for performance.
        /// </summary>
        /// <param name="metricId">The metric id to find.</param>
        /// <returns>The matching metadata nodes.</returns>
        public IEnumerable<MetricMetadataNode> FindInDescendantsByMetricId(int metricId)
        {
            return FindInDescendants(Tree.AllNodesByMetricId.ValuesByKey(metricId));
        }

        /// <summary>
        /// Finds a metric metadata node in the DescendantsOrSelf collection by metric Id using an index for performance.
        /// </summary>
        /// <param name="metricId">The metric id to find.</param>
        /// <returns>The matching metadata nodes.</returns>
        public IEnumerable<MetricMetadataNode> FindInDescendantsOrSelfByMetricId(int metricId)
        {
            return FindInDescendants(Tree.AllNodesByMetricId.ValuesByKey(metricId), true);
        }

        /// <summary>
        /// Finds a metric metadata node in the Descendants collection by metric variant Id using an index for performance.
        /// </summary>
        /// <param name="metricVariantId">The metric variant id to find.</param>
        /// <returns>The matching metadata nodes.</returns>
        public IEnumerable<MetricMetadataNode> FindInDescendantsByMetricVariantId(int metricVariantId)
        {
            return FindInDescendants(Tree.AllNodesByMetricVariantId.ValuesByKey(metricVariantId));
        }

        /// <summary>
        /// Finds a metric metadata node in the DescendantsOrSelf collection by metric variant Id using an index for performance.
        /// </summary>
        /// <param name="metricVariantId">The metric variant id to find.</param>
        /// <returns>The matching metadata nodes.</returns>
        public IEnumerable<MetricMetadataNode> FindInDescendantsOrSelfByMetricVariantId(int metricVariantId)
        {
            return FindInDescendants(Tree.AllNodesByMetricVariantId.ValuesByKey(metricVariantId), true);
        }

        /// <summary>
        /// Finds a metric metadata node in the Descendants collection by metric node Id using an index for performance.
        /// </summary>
        /// <param name="metricNodeId">The metric node id to find.</param>
        /// <returns>The matching metadata nodes.</returns>
        public MetricMetadataNode FindInDescendantsByMetricNodeId(int metricNodeId)
        {
            var metricMetadataNode = Tree.AllNodesByMetricNodeId.GetValueOrDefault(metricNodeId);

            if (metricMetadataNode == null)
                return null;

            return FindInDescendants(metricMetadataNode.ToEnumerable()).SingleOrDefault();
        }

        /// <summary>
        /// Finds a metric metadata node in the DescendantsOrSelf collection by metric node Id using an index for performance.
        /// </summary>
        /// <param name="metricNodeId">The metric node id to find.</param>
        /// <returns>The matching metadata nodes.</returns>
        public MetricMetadataNode FindInDescendantsOrSelfByMetricNodeId(int metricNodeId)
        {
            var metricMetadataNode = Tree.AllNodesByMetricNodeId.GetValueOrDefault(metricNodeId);

            if (metricMetadataNode == null)
                return null;

            return FindInDescendants(metricMetadataNode.ToEnumerable(), true).SingleOrDefault();
        }

        private IEnumerable<MetricMetadataNode> FindInDescendants(IEnumerable<MetricMetadataNode> possibleNodes, bool includeSelf = false)
        {
            int startIndex = includeSelf ? DescendantsStartIndex - 1 : DescendantsStartIndex;
            int endIndex = includeSelf ? (startIndex + DescendantsCount + 1) : (startIndex + DescendantsCount);

            // Filter set to viable descendants
            var actualNode =
                from n in possibleNodes
                let index = Tree.IndexOf(n)
                where index >= startIndex && index <= endIndex
                select n;

            return actualNode;
        }

        private int descendantsStartIndex = int.MinValue;

        private int DescendantsStartIndex
        {
            get
            {
                if (descendantsStartIndex == int.MinValue)
                {
                    if (Children.Any())
                    {
                        int i = Tree.IndexOf(Children.First());
                        int j;

                        if (NextSibling != null)
                            j = Tree.IndexOf(NextSibling) - 1;
                        else
                        {
                            var current = Parent;

                            while (current != null && current.NextSibling == null)
                                current = current.Parent;

                            if (current != null && current.NextSibling != null)
                                j = Tree.IndexOf(current.NextSibling) - 1;
                            else
                                j = Tree.AllNodesInTreeOrder.Count() - 1;
                        }

                        descendantsStartIndex = i;
                        DescendantsCount = j - i + 1;
                    }
                    else
                    {
                        descendantsStartIndex = -1;
                        DescendantsCount = 0;
                    }
                }

                return descendantsStartIndex;
            }
        }

        private int DescendantsCount { get; set; }

        /// <summary>
        /// Gets an enumerable sequence of metadata nodes down the hierarchy in tree order.
        /// </summary>
        [JsonIgnore, IgnoreDataMember]
        public IEnumerable<MetricMetadataNode> Descendants
        {
            get
            {
                if (DescendantsStartIndex == -1)
                    return Enumerable.Empty<MetricMetadataNode>();

                return Tree.AllNodesInTreeOrder.Skip(DescendantsStartIndex).Take(DescendantsCount);
            }
        }

        /// <summary>
        /// Gets an enumerable sequence of metadata nodes down the hierarchy in tree order, including the current node.
        /// </summary>
        [JsonIgnore, IgnoreDataMember]
        public IEnumerable<MetricMetadataNode> DescendantsOrSelf
        {
            get
            {
                yield return this;

                foreach (var node in Descendants)
                    yield return node;
            }
        }

        [JsonIgnore, IgnoreDataMember]
        public MetricMetadataNode NextSibling { get; set; }

        public override string ToString()
        {
            return Name + " (metricId: " + MetricId + ", metricNodeId: " + MetricNodeId + ")";
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            MetricMetadataNode previousChild = null;

            // Set the children's references to the parent
            foreach (var child in Children)
            {
                child.Parent = this;

                // Cascade OnDeserialized call to child if this is being called within 
                // the context of a serializer that doesn't support the method natively
                if (Serialization.IsDeserializingEntireGraph(context))
                    Serialization.InvokeOnDeserializedMethod(child.GetType(), child);

                if (previousChild != null)
                    previousChild.NextSibling = child;

                previousChild = child;
            }
        }
    }
}
