// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Models.Charting;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.School.Detail
{
    public class GradeLevelChartRequest
    {
        public int SchoolId { get; set; }
        public int MetricVariantId { get; set; }
        [AuthenticationIgnore("Title is not a relevant property outside of forming summaries")]
        public string Title { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GradeLevelChartRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="GradeLevelChartRequest"/> instance.</returns>
        public static GradeLevelChartRequest Create(int schoolId, int metricVariantId, string title)
        {
            return new GradeLevelChartRequest { SchoolId = schoolId, MetricVariantId = metricVariantId, Title = title};
        }
    }

    public interface IGradeLevelChartService : IService<GradeLevelChartRequest, ChartData> { }

    public class GradeLevelChartService : IGradeLevelChartService
    {
        private const string seriesName = "Actual";
        private const string schoolGoalFormat = "School Goal: {0}";
        private readonly IRepository<SchoolMetricGradeDistribution> repository;
        private readonly IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        private readonly IMetricGoalProvider metricGoalProvider;
        private readonly IMetricNodeResolver metricNodeResolver;

        public GradeLevelChartService(IRepository<SchoolMetricGradeDistribution> repository,
                                      IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver,
                                      IMetricGoalProvider metricGoalProvider,
                                      IMetricNodeResolver metricNodeResolver)
        {
            this.repository = repository;
            this.metricInstanceSetKeyResolver = metricInstanceSetKeyResolver;
            this.metricGoalProvider = metricGoalProvider;
            this.metricNodeResolver = metricNodeResolver;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllMetrics, EdFiClaimTypes.ViewMyMetrics)]
        public ChartData Get(GradeLevelChartRequest request)
        {
            int schoolId = request.SchoolId;
            int metricVariantId = request.MetricVariantId;
            var metricMetadataNode = metricNodeResolver.GetMetricNodeForSchoolFromMetricVariantId(schoolId, metricVariantId);
            int metricId = metricMetadataNode.MetricId;

            if (String.IsNullOrEmpty(metricMetadataNode.Format))
                throw new ArgumentNullException(string.Format("Format is null for metric variant Id:{0}", metricVariantId));

            if (String.IsNullOrEmpty(metricMetadataNode.NumeratorDenominatorFormat))
                throw new ArgumentNullException(string.Format("Numerator/denominator format is null for metric variant Id:{0}", metricVariantId));

            string chartId = String.Format("{0}_GLC_{1}", schoolId, metricVariantId);

            var results = from data in repository.GetAll()
                          where data.SchoolId == schoolId
                                && data.MetricId == metricId
                          orderby data.GradeLevelTypeId
                          select data;

            var metricInstanceSetKey = metricInstanceSetKeyResolver.GetMetricInstanceSetKey(SchoolMetricInstanceSetRequest.Create(schoolId, metricVariantId));
            var goal = metricGoalProvider.GetMetricGoal(metricInstanceSetKey, metricId);
            var goalValue = Convert.ToDouble(goal.Value);

            var series = new ChartData.Series { Name = seriesName, Style = ChartSeriesStyle.Blue };
            var labelFormat = String.Empty;
            double maxValue = goalValue;
            foreach (var result in results)
            {
                var point = new ChartData.Point
                                        {
                                            AxisLabel = result.GradeLevel,
                                            Value = CalculateValue(result.Numerator, result.Denominator),
                                            Tooltip = CalculateTooltip(result.Numerator, result.Denominator, metricMetadataNode.NumeratorDenominatorFormat),
                                            Trend = TrendEvaluation.None,
                                            IsValueShownAsLabel = true
                                        };
                if (point.Value > maxValue)
                    maxValue = point.Value;
                labelFormat = metricMetadataNode.Format;
                point.Label = String.Format(labelFormat, point.Value);
                if (goal.Interpretation == TrendInterpretation.Standard)
                    point.State = point.Value >= goalValue ? MetricStateType.Good : MetricStateType.Low;
                else
                    point.State = point.Value <= goalValue ? MetricStateType.Good : MetricStateType.Low;
                series.Points.Add(point);
            }

            var stripLine = new ChartData.StripLine { Value = goalValue, Tooltip = String.Format(schoolGoalFormat, String.Format(labelFormat, goal.Value)), IsNegativeThreshold = goal.Interpretation == TrendInterpretation.Inverse };

            return new ChartData
                       {
                           SeriesCollection = new List<ChartData.Series> { series },
                           StripLines = new List<ChartData.StripLine> { stripLine },
                           YAxisMaxValue = maxValue,
                           ChartId = chartId,
                           ChartTitle = metricMetadataNode.Actions.GetChartTitle(request.Title),
                           Goal = String.Format(schoolGoalFormat, String.Format(labelFormat, goal.Value))
                       };
        }

        private static double CalculateValue(decimal? numerator, int? denominator)
        {
            if (!denominator.HasValue)
                return 0;

            return Math.Truncate(Convert.ToDouble(numerator) / Convert.ToDouble(denominator) * 1000) / 1000;
        }

        private static string CalculateTooltip(decimal? numerator, int? denominator, string format)
        {
            if (!denominator.HasValue && !numerator.HasValue)
                return String.Empty;

            if (!denominator.HasValue)
                return String.Format(format, numerator);

            return String.Format(format, numerator, denominator);
        }
    }

}
