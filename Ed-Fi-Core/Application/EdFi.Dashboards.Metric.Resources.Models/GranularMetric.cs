// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using EdFi.Dashboards.Resource.Models.Common;

namespace EdFi.Dashboards.Metric.Resources.Models
{
    /// <summary>
    /// This is an interface to facilitate typed views in the templates
    /// </summary>
    public interface IGranularMetric : IMetricBase, INode<MetricBase>, IResourceModelBase
    {
        //Granular Metric Items
        object Value { get; set; }
        List<MetricComponent> Components { get; set; }
        Hashtable Values { get; set; }
        State State { get; set; }
        string DisplayValue { get; set; }
        Trend Trend { get; set; }
        bool IsFlagged { get; set; }
        Goal Goal { get; set; }
    }

    [Serializable]
    public class GranularMetric<T> : MetricBase, IGranularMetric
    {
        public GranularMetric()
        {
            MetricType = MetricType.GranularMetric;
            Values = new Hashtable();
            Components = new List<MetricComponent>();
            State = new State();
        }
        
        /// <summary>
        /// The Components of the metric
        /// </summary>
        public List<MetricComponent> Components { get; set; }

        /// <summary>
        /// The Extended Property items
        /// </summary>
        public Hashtable Values { get; set; }

        public State State { get; set; }

        /// <summary>
        /// The value to display for the metric.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// The value to display for the metric.
        /// </summary>
        public string DisplayValue { get; set; }

        /// <summary>
        /// The trend of the metric.
        /// </summary>
        public Trend Trend { get; set; }

        /// <summary>
        /// The value to know if the metric is flagged or not.
        /// </summary>
        public bool IsFlagged { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Goal Goal { get; set; }

        object IGranularMetric.Value
        {
            get { return Value; }
            set { Value = (T) value; }
        }
    }

    [Serializable]
    public class State
    {
        public State()
        {
            
        }
        public State(MetricStateType stateType, string stateText)
        {
            StateType = stateType;
            StateText = stateText;
            DisplayStateText = stateText;
        }
        
        public MetricStateType StateType { get; set; }
        public string StateText { get; set; }
        public string DisplayStateText { get; set; }
    }

    /// <summary>
    /// Contains properties describing the states for a metric, including minimum and maximum thresholds and the text to be displayed.
    /// </summary>
    [Serializable]
    public class MetricState
    {
        /// <summary>
        /// Gets or sets the threshold definition.
        /// </summary>
        public MetricStateType StateType { get; set; }

        /// <summary>
        /// Text for the State
        /// </summary>
        public string StateText { get; set; }

        /// <summary>
        /// Gets or sets the top most limit for the threshold.
        /// </summary>
        public decimal? MaxValue { get; set; }

        /// <summary>
        /// Gets or sets if the upper limit is inclusive
        /// </summary>
        public int? IsMaxValueInclusive { get; set; }

        /// <summary>
        /// Gets or sets the lower limit for the threshold.
        /// </summary>
        public decimal? MinValue { get; set; }

        /// <summary>
        /// Gets or sets if the lower limit is inclusive
        /// </summary>
        public int? IsMinValueInclusive { get; set; }

        /// <summary>
        /// Gets or sets string format
        /// </summary>
        public string Format { get; set; }
    }

    /// <summary>
    /// Provides values for the possible status of the metric.
    /// </summary>
    public enum MetricStateType
    {
        /// <summary>
        /// The value indicates there is no data available (typically a gray color would be used for rendering purposes).
        /// </summary>
        NoData = -1,

        /// <summary>
        /// The value is neutral.
        /// </summary>
        Neutral = 0,

        /// <summary>
        /// The value is good.
        /// </summary>
        Good = 1,

        /// <summary>
        /// The value is acceptable.
        /// </summary>
        Acceptable = 2,

        /// <summary>
        /// The value is low.
        /// </summary>
        Low = 3,

        /// <summary>
        /// The value is NA.
        /// </summary>
        Na = 4,

        /// <summary>
        /// There is no value.
        /// </summary>
        None = 5,

        /// <summary>
        /// The value is very good.
        /// </summary>
        VeryGood = 6,

        /// <summary>
        /// The value is very low.
        /// </summary>
        VeryLow = 7
    }
}
