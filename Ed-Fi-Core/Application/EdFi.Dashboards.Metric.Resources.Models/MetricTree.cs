using System;
using System.Runtime.Serialization;
using EdFi.Dashboards.Infrastructure.Implementations;

namespace EdFi.Dashboards.Metric.Resources.Models
{
    /// <summary>
    /// Provides a container to hold metric instances.
    /// </summary>
    [Serializable]
    public class MetricTree
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetricTree"/> class.
        /// </summary>
        public MetricTree() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricTree"/> class with the specified root metric node.
        /// </summary>
        /// <param name="rootMetricNode">The root metric node of the tree.</param>
        public MetricTree(MetricBase rootMetricNode)
        {
            RootNode = rootMetricNode;
        }

        /// <summary>
        /// Gets or sets the root metric node of the tree (derived from <see cref="MetricBase"/>).
        /// </summary>
        public MetricBase RootNode { get; set; }

        internal void OnDeserialized(StreamingContext context)
        {
            if (Serialization.IsDeserializingEntireGraph(context))
                Serialization.InvokeOnDeserializedMethod(RootNode.GetType(), RootNode);
        }
    }
}
