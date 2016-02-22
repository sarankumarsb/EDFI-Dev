// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Models.Charting;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using MetricStateType = EdFi.Dashboards.Metric.Resources.Models.MetricStateType;

namespace EdFi.Dashboards.Resources.School.Detail
{
    public class AssessmentRateChartRequest
    {
        public int SchoolId { get; set; }
        public int MetricVariantId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentRateChartRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="AssessmentRateChartRequest"/> instance.</returns>
        public static AssessmentRateChartRequest Create(int schoolId, int metricVariantId) 
		{
            return new AssessmentRateChartRequest { SchoolId = schoolId, MetricVariantId = metricVariantId };
		}
	}

    public interface IAssessmentRateChartService : IService<AssessmentRateChartRequest, AssessmentRateChartModel> { }

    public class AssessmentRateChartService : IAssessmentRateChartService
    {
        private const string commendedSeriesName = "Commended";
        private const string metStandardSeriesName = "Met Standard";
        private const string belowSeriesName = "Below";
        private readonly IRepository<SchoolMetricAssessmentRate> repository;
        private readonly IRepository<MetricInstanceFootnote> footnoteRepository;
        private readonly IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        private readonly IMetricNodeResolver metricNodeResolver;

        public AssessmentRateChartService(IRepository<SchoolMetricAssessmentRate> repository, 
                                            IRepository<MetricInstanceFootnote> footnoteRepository,
                                            IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver,
                                            IMetricNodeResolver metricNodeResolver)
        {
            this.repository = repository;
            this.footnoteRepository = footnoteRepository;
            this.metricInstanceSetKeyResolver = metricInstanceSetKeyResolver;
            this.metricNodeResolver = metricNodeResolver;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllMetrics, EdFiClaimTypes.ViewMyMetrics)]
        public AssessmentRateChartModel Get(AssessmentRateChartRequest request)
        {
            int schoolId = request.SchoolId;
            int metricVariantId = request.MetricVariantId;
            var metricMetadataNode = metricNodeResolver.GetMetricNodeForSchoolFromMetricVariantId(schoolId, metricVariantId);
            int metricId = metricMetadataNode.MetricId;

            if (String.IsNullOrEmpty(metricMetadataNode.Format))
                throw new ArgumentNullException(string.Format("Format is null for metric variant Id:{0}", metricVariantId));


            string chartId = String.Format("{0}_{1}", schoolId, metricVariantId);

            var result = from data in repository.GetAll()
                         where data.SchoolId == schoolId
                               && data.MetricId == metricId
                         orderby data.GradeLevelTypeId
                         select data;

            var commended = new ChartData.Series { Name = commendedSeriesName, Style = ChartSeriesStyle.Blue, ShowInLegend = true };
			var met = new ChartData.Series { Name = metStandardSeriesName, Style = ChartSeriesStyle.Green, ShowInLegend = true };
			var below = new ChartData.Series { Name = belowSeriesName, Style = ChartSeriesStyle.Red, ShowInLegend = true };

            foreach (var grade in result)
            {
                commended.Points.Add(GeneratePoint(grade.GradeLevel, grade.CommendedRate, metricMetadataNode.Format));
                met.Points.Add(GeneratePoint(grade.GradeLevel, grade.MetStandardRate, metricMetadataNode.Format));
                below.Points.Add(GeneratePoint(grade.GradeLevel, grade.BelowRate, metricMetadataNode.Format));
            }

            var metricInstanceSetKey = metricInstanceSetKeyResolver.GetMetricInstanceSetKey(SchoolMetricInstanceSetRequest.Create(schoolId, metricVariantId));
            var footnotes = (from data in footnoteRepository.GetAll()
                             where data.MetricInstanceSetKey == metricInstanceSetKey 
                                    && data.MetricId == metricId 
                                    && data.FootnoteTypeId == (int)MetricFootnoteType.DrillDownFootnote
                            select data).ToList();

            var chartData = new ChartData
                                {
                                    ChartId = chartId,
                                    SeriesCollection = new List<ChartData.Series> {below, met, commended}
                                };

            return new AssessmentRateChartModel
                       {
                           ChartData = chartData,
                           MetricFootnotes = footnotes.Select(x => new MetricFootnote
                                                                         {
                                                                             FootnoteTypeId = (MetricFootnoteType) x.FootnoteTypeId,
                                                                             FootnoteText = x.FootnoteText
                                                                         }).ToList()
                       };
        }

        private static ChartData.Point GeneratePoint(string gradeLevel, decimal? rate, string labelFormat)
        {
            var value = rate.HasValue ? Convert.ToDouble(rate.Value) : 0;
            return new ChartData.Point
                            {
                                Label = String.Format(labelFormat, value),
                                AxisLabel = gradeLevel,
                                Value = value,
                                Tooltip = String.Format(labelFormat, value),
                                IsValueShownAsLabel = Math.Abs(value - 0) >= Double.Epsilon,
                                State = MetricStateType.None,
                                Trend = TrendEvaluation.None
                            };
        }
    }

    
}
