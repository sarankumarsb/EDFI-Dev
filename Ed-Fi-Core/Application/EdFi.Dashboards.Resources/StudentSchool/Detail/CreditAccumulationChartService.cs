// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.Charting;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.StudentSchool.Detail
{
    public class CreditAccumulationChartRequest
    {
        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }
        public int MetricVariantId { get; set; }
        [AuthenticationIgnore("Title is not a relevant property outside of forming summaries")]
        public string Title { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreditAccumulationChartRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="CreditAccumulationChartRequest"/> instance.</returns>
        public static CreditAccumulationChartRequest Create(long studentUSI, int schoolId, int metricVariantId, string title) 
		{
            return new CreditAccumulationChartRequest { StudentUSI = studentUSI, SchoolId = schoolId, MetricVariantId = metricVariantId, Title = title };
		}
	}

    public interface ICreditAccumulationChartService : IService<CreditAccumulationChartRequest, ChartData> { }

    public class CreditAccumulationChartService : ICreditAccumulationChartService
    {
        protected virtual string AxisXTitle
        {
            get { return "Grade"; }
        }

        protected virtual string AxisYTitle
        {
            get { return "Cumulative Credits"; }
        }

        protected virtual string RecommendedSeriesName
        {
            get { return "Recommended"; }
        }

        protected virtual string MinimumSeriesName
        {
            get { return "Minimum"; }
        }

        protected virtual string ActualSeriesName
        {
            get { return "Actual"; }
        }

        private const string tooltipFormat = "{0} credits";
        private const string topLabelStyle = "Top";
        private const string bottomLabelStyle = "Bottom";
        private const string blueSquareImage = "Graph/CreditAccumulation/BlueSquare.png";
        private const string blackArrowImage = "Graph/CreditAccumulation/BlackArrow.png";

        private readonly IRepository<StudentMetricCreditAccumulation> repository;
        private readonly IMetricNodeResolver metricNodeResolver;

        public CreditAccumulationChartService(IRepository<StudentMetricCreditAccumulation> repository, IMetricNodeResolver metricNodeResolver)
        {
            this.repository = repository;
            this.metricNodeResolver = metricNodeResolver;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public ChartData Get(CreditAccumulationChartRequest request)
        {
            var studentUSI = request.StudentUSI;
            int schoolId = request.SchoolId;
            int metricVariantId = request.MetricVariantId;
            var metricMetadataNode = metricNodeResolver.GetMetricNodeForStudentFromMetricVariantId(schoolId, metricVariantId);

            int metricId = metricMetadataNode.MetricId;


            var credits = GetStudentCreditAccumulation(studentUSI, schoolId, metricId);
            if (credits == null || credits.Count == 0)
                return null;

            var series = new List<ChartData.Series>
                             {
                                 new ChartData.Series
                                     {Name = ActualSeriesName, Style = ChartSeriesStyle.Blue}
                             };

            if (credits.Any(credit => credit.RecommendedCredits.HasValue))
            {
                series.Add(new ChartData.LineSeries
                        {
                            Name = RecommendedSeriesName,
                            Style = ChartSeriesStyle.BlueLimit,
                            ShowInLegend = true,
                            LabelStyle = topLabelStyle,
                            MarkerImage = EdFiWebFormsDashboards.Site.Common.ThemeImage(blueSquareImage)
                        });
            }
            if (credits.Any(credit => credit.MinimumCredits.HasValue))
            {
                series.Add(new ChartData.LineSeries
                        {
                            Name = MinimumSeriesName,
                            Style = ChartSeriesStyle.Black,
                            ShowInLegend = true,
                            LabelStyle = bottomLabelStyle,
                            MarkerImage = EdFiWebFormsDashboards.Site.Common.ThemeImage(blackArrowImage)
                        });
            }

            var result = new ChartData
                             {
                                  SeriesCollection = series
                             };

            result.ChartTitle = metricMetadataNode.Actions.GetChartTitle(request.Title);

            foreach (var credit in credits)
            {
                result.SeriesCollection[0].Points.Add(new ChartData.Point
                                                          {
                                                              Value = Convert.ToDouble(credit.CumulativeCredits),
                                                              AxisLabel = credit.GradeLevel,
                                                              Tooltip = String.Format(tooltipFormat, credit.CumulativeCredits),
                                                              State = credit.MetricState,
                                                              Trend = TrendEvaluation.None
                                                          });
                if (credit.RecommendedCredits.HasValue)
                {
                    result.SeriesCollection[1].Points.Add(new ChartData.Point
                                                              {
                                                                  Value = Convert.ToDouble(credit.RecommendedCredits.Value),
                                                                  IsValueShownAsLabel = true,
                                                                  State = MetricStateType.None,
                                                                  Trend = TrendEvaluation.None
                                                              });
                }
                if (credit.MinimumCredits.HasValue)
                {
                    result.SeriesCollection[2].Points.Add(new ChartData.Point
                                                              {
                                                                  Value = Convert.ToDouble(credit.MinimumCredits.Value),
                                                                  IsValueShownAsLabel = true,
                                                                  State = MetricStateType.None,
                                                                  Trend = TrendEvaluation.None
                                                              });
                }
            }

            result.AxisXTitle = AxisXTitle;
            result.AxisYTitle = AxisYTitle;

            return result;
        }

        private class Credit
        {
            public string GradeLevel { get; set; }
            public MetricStateType MetricState { get; set; }
            public decimal CumulativeCredits { get; set; }
            public decimal? RecommendedCredits { get; set; }
            public decimal? MinimumCredits { get; set; }
        }

        private List<Credit> GetStudentCreditAccumulation(long studentUSI, int schoolId, int metricId)
        {
            var results = (from data in repository.GetAll()
                           where data.StudentUSI == studentUSI 
                                    && data.SchoolId == schoolId 
                                    && data.MetricId == metricId
                           orderby data.SchoolYear
                           select new
                                      {
                                          data.GradeLevel,
                                          data.MetricStateTypeId,
                                          data.CumulativeCredits,
                                          data.RecommendedCredits,
                                          data.MinimumCredits
                                      }).ToList();

            // this has to be split out b/c Subsonic doesn't support constructors with parameters
            return results.Select(x => new Credit
                                           {
                                               GradeLevel = x.GradeLevel,
                                               MetricState =
                                                   x.MetricStateTypeId == null
                                                       ? MetricStateType.None
                                                       : (MetricStateType) x.MetricStateTypeId,
                                               CumulativeCredits = x.CumulativeCredits ?? 0m,
                                               RecommendedCredits = x.RecommendedCredits,
                                               MinimumCredits = x.MinimumCredits
                                           }).ToList();
        }
    }
}
