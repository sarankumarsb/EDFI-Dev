// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using EdFi.Dashboards.Infrastructure.Implementations;
using Newtonsoft.Json;

namespace EdFi.Dashboards.Metric.Resources.Models
{
    [Serializable]
    public class ContainerMetric : MetricBase, IContainerNode<MetricBase>
    {
        public ContainerMetric()
        {
            Children = new List<MetricBase>();
            MetricType = MetricType.ContainerMetric;
        }
        
        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            // Set the children's references to the parent
            foreach (var child in Children)
            {
                child.Parent = this;

                // Cascade OnDeserialized call to child if this is being called within 
                // the context of a serializer that doesn't support the method natively
                if (Serialization.IsDeserializingEntireGraph(context))
                    Serialization.InvokeOnDeserializedMethod(child.GetType(), child);
            }
        }

        /// <summary>
        /// Gets and sets the list of child metrics.
        /// </summary>
        public List<MetricBase> Children { get; set; }

        #region Explicit implementation of IContainerNode<MetricBase> Children

        //[JsonIgnore]
        IEnumerable<MetricBase> IContainerNode<MetricBase>.Children
        {
            get { return Children; }
            set { Children = value.ToList(); }
        }

        #endregion

        [JsonIgnore, IgnoreDataMember]
        public IEnumerable<MetricBase> Descendants
        {
            get
            {
                return new TreeOrderNodeEnumerable<MetricBase>(this);
            }
        }

        [JsonIgnore, IgnoreDataMember]
        public IEnumerable<MetricBase> DescendantsOrSelf
        {
            get
            {
                yield return this;

                foreach (var metric in Descendants)
                    yield return metric;
            }
        }
    }
}
