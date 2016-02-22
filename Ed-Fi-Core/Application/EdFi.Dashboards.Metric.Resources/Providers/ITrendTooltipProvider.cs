using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Metric.Resources.Providers
{
    public interface ITrendTooltipProvider
    {
        string GetTrendTooltip(TrendDirection trendDirection, TrendInterpretation trendInterpretation);
    }
}
