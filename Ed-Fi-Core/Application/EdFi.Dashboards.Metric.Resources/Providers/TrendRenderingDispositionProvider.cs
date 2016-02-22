// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Metric.Resources.Providers
{
    public class TrendRenderingDispositionProvider : ITrendRenderingDispositionProvider
    {
        public TrendEvaluation GetTrendRenderingDisposition(TrendDirection trendDirection, TrendInterpretation trendInterpretation)
        {
            if (trendInterpretation == TrendInterpretation.Standard)
            {
                switch (trendDirection)
                {
                    case TrendDirection.Decreasing:
                        {
                            return TrendEvaluation.DownBad;
                        }
                    case TrendDirection.Increasing:
                        {
                            return TrendEvaluation.UpGood;
                        }
                    case TrendDirection.None:
                        {
                            return TrendEvaluation.None;
                        }
                    case TrendDirection.Unchanged:
                        {
                            return TrendEvaluation.NoChangeNoOpinion;
                        }
                }
            }
            if (trendInterpretation == TrendInterpretation.Inverse)
            {
                switch (trendDirection)
                {
                    case TrendDirection.Decreasing:
                        {
                            return TrendEvaluation.UpGood;
                        }
                    case TrendDirection.Increasing:
                        {
                            return TrendEvaluation.DownBad;
                        }
                    case TrendDirection.None:
                        {
                            return TrendEvaluation.None;
                        }
                    case TrendDirection.Unchanged:
                        {
                            return TrendEvaluation.NoChangeNoOpinion;
                        }
                }
            }
            return TrendEvaluation.None;
        }
    }
}
