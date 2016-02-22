// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;

namespace EdFi.Dashboards.Metric.Resources.Models
{
    /// <summary>
    /// Goals per definition are always inclusive.
    /// </summary>
    [Serializable]
    public class Goal
    {
        /// <summary>
        /// The actual value for the Goal.
        /// </summary>
        public decimal? Value { get; set; }

        /// <summary>
        /// Interpretation that leads to evaluate if the goal is above this value it is good or bad.
        /// </summary>
        public TrendInterpretation Interpretation { get; set; }

    }
}