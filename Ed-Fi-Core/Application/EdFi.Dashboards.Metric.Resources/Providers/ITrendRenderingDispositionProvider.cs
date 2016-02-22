// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Metric.Resources.Providers
{
	/// <summary>
	/// Provides a method for defining how a trend should be rendered in the view.
	/// </summary>
    public interface ITrendRenderingDispositionProvider
    {
		/// <summary>
		/// Gets the <see cref="TrendEvaluation"/> that represents how the metric should be <i>rendered</i> in the views for the system, not how the metric was actually evaluated.
		/// </summary>
		/// <param name="trendDirection">An enumerated value indicating the numerical direction of the metric values.</param>
		/// <param name="trendInterpretation">An enumerated value indicating how the numerical direction of the metric values should be interpreted.</param>
		/// <returns>A <see cref="TrendEvaluation"/> value indicating how the trend should be rendered in the view.</returns>
		/// <remarks>This interface provides an extensibility point around defining how trends should be rendered in the user interface.
		/// 
		/// For example, in one implementation of the dashboards, the trends are rendered such that trend arrows point up if the numerical value of the metric 
		/// is increasing, and down if the numerical value of the metric is decreasing. To avoid potential confusion, the arrows are always rendered as gray 
		/// (ignoring interpretations of "good" and "bad" for rendering purposes).
		/// 
		/// However, in another implementation, the trends are rendered with green trend arrows pointing up if the metric is moving in a "good" direction, 
		/// and red down arrows if they are moving in a "bad" direction (ignoring the numerical trend direction).  
		/// 
		/// By centralizing this decision on the server rather than the view, it allows the same view implementation to serve both installations, and will
		/// ease the possible integration of future 3rd-party applications with a particular implementation of the dashboards, enabling it to be consistent 
		/// with the installed system without the need for client-side customization (on this point, at least).
		/// </remarks>
        TrendEvaluation GetTrendRenderingDisposition(TrendDirection trendDirection, TrendInterpretation trendInterpretation);
    }
}
