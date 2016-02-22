using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using Newtonsoft.Json;

namespace EdFi.Dashboards.Metric.Resources.Models
{
    [Serializable]
    public class MetricMetadataTree : IContainerNode<MetricMetadataNode>
    {
        [NonSerialized]
        private IDictionary<int, int> indexPosByNodeId;

        public int IndexOf(MetricMetadataNode metadataNode)
        {
            if (indexPosByNodeId == null)
                InitializeOptimizations();

            if (indexPosByNodeId != null && indexPosByNodeId.ContainsKey(metadataNode.MetricNodeId))
                return indexPosByNodeId[metadataNode.MetricNodeId];

            return -1;
        }

        [NonSerialized]
        private IEnumerable<MetricMetadataNode> allNodesInTreeOrder;

        [JsonIgnore, IgnoreDataMember]
        public IEnumerable<MetricMetadataNode> AllNodesInTreeOrder
        {
            get
            {
                if (allNodesInTreeOrder == null)
                    InitializeOptimizations();

                return allNodesInTreeOrder;
            }
        }

        [NonSerialized]
        private MultiValueDictionary<int, MetricMetadataNode> allNodesByMetricId;

        [JsonIgnore, IgnoreDataMember]
        public MultiValueDictionary<int, MetricMetadataNode> AllNodesByMetricId
        {
            get
            {
                if (allNodesByMetricId == null)
                    InitializeOptimizations();

                return allNodesByMetricId;
            }
        }

        [NonSerialized]
        private MultiValueDictionary<int, MetricMetadataNode> allNodesByMetricVariantId;

        [JsonIgnore, IgnoreDataMember]
        public MultiValueDictionary<int, MetricMetadataNode> AllNodesByMetricVariantId
        {
            get
            {
                if (allNodesByMetricVariantId == null)
                    InitializeOptimizations();

                return allNodesByMetricVariantId;
            }
        }

        [NonSerialized]
        private Dictionary<int, MetricMetadataNode> allNodesByMetricNodeId;

        [JsonIgnore, IgnoreDataMember]
        public Dictionary<int, MetricMetadataNode> AllNodesByMetricNodeId
        {
            get
            {
                if (allNodesByMetricNodeId == null)
                    InitializeOptimizations();

                return allNodesByMetricNodeId;
            }
        }

        protected virtual void InitializeOptimizations()
        {
            var enumerator = new TreeOrderNodeEnumerator<MetricMetadataNode>(this);

            var tempAllNodesInTreeOrder = new List<MetricMetadataNode>();

            while (enumerator.MoveNext())
            {
                tempAllNodesInTreeOrder.Add(enumerator.Current);
            }

            allNodesInTreeOrder = tempAllNodesInTreeOrder;


            allNodesByMetricId = new MultiValueDictionary<int, MetricMetadataNode>().CreateMap(allNodesInTreeOrder, node => node.MetricId);

            allNodesByMetricVariantId = new MultiValueDictionary<int, MetricMetadataNode>().CreateMap(allNodesInTreeOrder, node => node.MetricVariantId);

            allNodesByMetricNodeId = new Dictionary<int, MetricMetadataNode>();
            allNodesByMetricNodeId.AddRange(allNodesInTreeOrder, node => node.MetricNodeId);

            // Optimize descendants processing
            // Build dictionary to hold position index for metric node
            indexPosByNodeId = new Dictionary<int, int>();

            for (int i = 0; i < allNodesInTreeOrder.Count(); i++)
                indexPosByNodeId[allNodesInTreeOrder.ElementAt(i).MetricNodeId] = i;
        }

        [JsonIgnore, IgnoreDataMember]
        public MetricMetadataNode Parent
        {
            get { return null; }
            set { throw new NotSupportedException("Cannot set the Parent of a Tree."); }
        }

        private List<MetricMetadataNode> children;

        public virtual IEnumerable<MetricMetadataNode> Children
        {
            get { return children; }
            set { children = value.ToList(); }
        }

        [JsonIgnore, IgnoreDataMember]
        public IEnumerable<MetricMetadataNode> Descendants
        {
            get
            {
                foreach (var child in Children)
                    foreach (var node in child.DescendantsOrSelf)
                        yield return node;
            }
        }

        [JsonIgnore, IgnoreDataMember]
        public IEnumerable<MetricMetadataNode> DescendantsOrSelf
        {
            get { return Descendants; }
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            // Re-establish back references to the Tree.
            foreach (var node in Descendants)
                node.Tree = this;
        }
    }
}
