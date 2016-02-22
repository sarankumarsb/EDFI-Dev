// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Metric.Resources.Helpers
{
    public class TrendHelper
    {
        public static TrendDirection TrendFromDirection(int? direction)
        {
            switch (direction)
            {
                case 1:
                    return TrendDirection.Increasing;
                case 0:
                    return TrendDirection.Unchanged;
                case -1:
                    return TrendDirection.Decreasing;
                default:
                    return TrendDirection.None;
            }
        }

        public static TrendEvaluation GetTrendEvaluation(TrendDirection trendDirection, TrendInterpretation trendInterpretation)
        {
            if (trendInterpretation == TrendInterpretation.Standard)
                switch (trendDirection)
                {
                    case TrendDirection.Decreasing:
                        return TrendEvaluation.DownBad;
                    case TrendDirection.Increasing:
                        return TrendEvaluation.UpGood;
                    case TrendDirection.None:
                        return TrendEvaluation.None;
                    case TrendDirection.Unchanged:
                        return TrendEvaluation.NoChangeNoOpinion;
                }

            if (trendInterpretation == TrendInterpretation.Inverse)
                switch (trendDirection)
                {
                    case TrendDirection.Decreasing:
                        return TrendEvaluation.DownGood;
                    case TrendDirection.Increasing:
                        return TrendEvaluation.UpBad;
                    case TrendDirection.None:
                        return TrendEvaluation.None;
                    case TrendDirection.Unchanged:
                        return TrendEvaluation.NoChangeNoOpinion;
                }

            return TrendEvaluation.None;
        }
    }
}
