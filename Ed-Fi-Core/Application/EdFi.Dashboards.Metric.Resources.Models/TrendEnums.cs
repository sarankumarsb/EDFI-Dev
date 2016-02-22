// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

namespace EdFi.Dashboards.Metric.Resources.Models
{
	/// <summary>
	/// Provides values that indicate how a metric trend should be interpreted.
	/// </summary>
    public enum TrendInterpretation
    {
        None = 0,
		/// <summary>
		/// Indicates that increasing values in the metric are good, and decreasing values are bad.
		/// </summary>
        Standard = 1,
		/// <summary>
		/// Indicates that increasing values in the metric are bad, and decreasing values are good.
		/// </summary>
        Inverse = -1
    }

	/// <summary>
	/// Provides values that indicate how the numerical values of a metric have been trending.
	/// </summary>
    public enum TrendDirection
    {
		/// <summary>
		/// Indicates the metric has been increasing in value.
		/// </summary>
        Increasing = 1,
		/// <summary>
		/// Indicates the metric has been decreasing in value.
		/// </summary>
		Decreasing = -1,
		/// <summary>
		/// Indicates the metric's value has been unchanged.
		/// </summary>
		Unchanged = 0,
		/// <summary>
		/// Indicates that there is not enough data present to provide a trend.
		/// </summary>
        None = int.MinValue
    }

    /// <summary>
	/// Provides values that indicate how an actual metric value trend has been interpreted (using <see cref="TrendDirection"/> and <see cref="TrendInterpretation"/>).
    /// </summary>
    public enum TrendEvaluation
    {
        /// <summary>
        /// There is not sufficient data to perform a trend evaluation.
        /// </summary>
        None,
        /// <summary>
        /// The metric value is increasing and this is good.
        /// </summary>
        UpGood,
        /// <summary>
        /// The metric value is increasing and this is bad.
        /// </summary>
        UpBad,
        /// <summary>
		/// The metric value is increasing and this is neither good nor bad.
        /// </summary>
        UpNoOpinion,
        /// <summary>
        /// The metric value is unchanged and this is neither good nor bad.
        /// </summary>
        NoChangeNoOpinion,
        /// <summary>
        /// The metric value is decreasing and this is good.
        /// </summary>
        DownGood,
        /// <summary>
        /// The metric value is decreasing and this is bad.
        /// </summary>
        DownBad,
        /// <summary>
        /// The metric value is decreasing and this neither good nor bad.
        /// </summary>
        DownNoOpinion
    }
}
