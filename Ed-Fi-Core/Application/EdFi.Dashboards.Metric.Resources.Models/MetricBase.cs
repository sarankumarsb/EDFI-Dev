// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using EdFi.Dashboards.Resource.Models.Common;
using Newtonsoft.Json;

namespace EdFi.Dashboards.Metric.Resources.Models
{
    public interface IMetricBase
    {
        /// <summary>
        /// the Node Id of the metric
        /// </summary>
        int MetricNodeId { get; set; }

        /// <summary>
        /// the Variant Id of the metric
        /// </summary>
        int MetricVariantId { get; set; }

        /// <summary>
        /// The Id of the metric.
        /// </summary>
        int MetricId { get; set; }

        /// <summary>
        /// the DisplayName from the metric node table
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// The Name of the metric.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// the DomainEntityType of the metric
        /// </summary>
        string DomainEntityType { get; set; }

        /// <summary>
        /// The Short Name of the metric.
        /// </summary>
        string ShortName { get; set; }

        /// <summary>
        /// The Description of the metric.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// The Context of the metric.
        /// </summary>
        string Context { get; set; }

        /// <summary>
        /// The Type of the metric (Container, Aggregate, Granular)
        /// </summary>
        MetricType MetricType { get; set; }

        /// <summary>
        /// The Variant Type of the metric
        /// </summary>
        MetricVariantType MetricVariantType { get; set; }

        /// <summary>
        /// The metric ToolTip.
        /// </summary>
        string ToolTip { get; set; }

        /// <summary>
        /// The list of Indicators for the corresponding metric.
        /// </summary>
        IEnumerable<MetricIndicator> Indicators { get; set; }

        /// <summary>
        /// list of Actions for the corresponding metric.
        /// </summary>
        List<MetricAction> Actions { get; set; }

        /// <summary>
        /// Gets or sets if the metric is Enabled/Disabled
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the TrendDirection.
        /// </summary>
        int? TrendDirection { get; set; }

        /// <summary>
        /// Gets or sets the NumeratorDenominatorFormat
        /// </summary>
        string NumeratorDenominatorFormat { get; set; }

        /// <summary>
        /// The footnotes related to the metrics.
        /// </summary>
        IEnumerable<MetricFootnote> Footnotes { get; set; }

        /// <summary>
        /// The metric Id used to traverse from a LocalEducationAgency metric down to a School metric down to a Student metric.
        /// </summary>
        int? ChildDomainEntityMetricId { get; set; }

        /// <summary>
        /// Gets or sets the Url that can be used to access the model as a resource.
        /// </summary>
        string ResourceUrl { get; set; }
    }

    [Serializable]
    public abstract class MetricBase : ResourceModelBase, INode<MetricBase>, IMetricBase
    {
        public MetricBase()
        {
            Indicators = new List<MetricIndicator>();
            Actions = new List<MetricAction>();
            Footnotes = new List<MetricFootnote>();
            Enabled = true;
        }

        /// <summary>
        /// the Node Id of the metric
        /// </summary>
        public int MetricNodeId { get; set; }

        public int MetricVariantId { get; set; }

        /// <summary>
        /// The Id of the metric.
        /// </summary>
        public int MetricId { get; set; }

        [NonSerialized]
        private MetricBase parent;

        /// <summary>
        /// reference to the Parent metric.
        /// </summary>
        [JsonIgnore, IgnoreDataMember]
        public MetricBase Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        /// <summary>
        /// the DisplayName from the metric node table
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The Name of the metric.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the DomainEntityType of the metric
        /// </summary>
        public string DomainEntityType { get; set; }

        /// <summary>
        /// The Short Name of the metric.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// The Description of the metric.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The Context of the metric.
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// The Type of the metric (Container, Aggregate, Granular)
        /// </summary>
        public MetricType MetricType { get; set; }

        /// <summary>
        /// The Variant Type of the metric
        /// </summary>
        public MetricVariantType MetricVariantType { get; set; }

        /// <summary>
        /// The metric ToolTip.
        /// </summary>
        public string ToolTip { get; set; }

        /// <summary>
        /// The list of Indicators for the corresponding metric.
        /// </summary>
        public IEnumerable<MetricIndicator> Indicators { get; set; }

        /// <summary>
        /// list of Actions for the corresponding metric.
        /// </summary>
        public List<MetricAction> Actions { get; set; }

        /// <summary>
        /// Gets or sets if the metric is Enabled/Disabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the TrendDirection.
        /// </summary>
        public int? TrendDirection { get; set; }

        /// <summary>
        /// Gets or sets the NumeratorDenominatorFormat
        /// </summary>
        public string NumeratorDenominatorFormat { get; set; }

        /// <summary>
        /// The footnotes related to the metrics.
        /// </summary>
        public IEnumerable<MetricFootnote> Footnotes { get; set; }

        /// <summary>
        /// The metric Id used to traverse from a LocalEducationAgency metric down to a School metric down to a Student metric.
        /// </summary>
        public int? ChildDomainEntityMetricId { get; set; }

        /// <summary>
        /// Indicates if the metric is under construction.
        /// </summary>
        public bool IsUnderConstruction { get; set; }
    }
}
