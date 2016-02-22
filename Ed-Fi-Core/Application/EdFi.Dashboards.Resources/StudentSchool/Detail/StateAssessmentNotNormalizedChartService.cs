using System;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Models.Charting;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.StudentSchool.Detail
{
    public class StateAssessmentNotNormalizedChartRequest
    {
        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }
        public int MetricVariantId { get; set; }
        [AuthenticationIgnore("Title is not a relevant property outside of forming summaries")]
        public string Title { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateAssessmentNotNormalizedChartRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="StateAssessmentNotNormalizedChartRequest"/> instance.</returns>
        public static StateAssessmentNotNormalizedChartRequest Create(long studentUSI, int schoolId, int metricVariantId, string title)
        {
            return new StateAssessmentNotNormalizedChartRequest { StudentUSI = studentUSI, SchoolId = schoolId, MetricVariantId = metricVariantId, Title = title};
        }
    }

    public interface IStateAssessmentNotNormalizedChartService : IService<StateAssessmentNotNormalizedChartRequest, ChartData> { }

    public class StateAssessmentNotNormalizedChartService : IStateAssessmentNotNormalizedChartService
    {
        private const int numberOfYearsToShow = 4;
        private const string titleFormat = "{0} Scores";
        private readonly IRepository<StudentMetricStateAssessmentHistorical> repository;
        private readonly IMetricNodeResolver metricNodeResolver;

        public StateAssessmentNotNormalizedChartService(IRepository<StudentMetricStateAssessmentHistorical> repository, IMetricNodeResolver metricNodeResolver)
        {
            this.repository = repository;
            this.metricNodeResolver = metricNodeResolver;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public ChartData Get(StateAssessmentNotNormalizedChartRequest request)
        {
            int metricVariantId = request.MetricVariantId;
            var studentUSI = request.StudentUSI;
            int schoolId = request.SchoolId;

            var metricMetadataNode = metricNodeResolver.GetMetricNodeForStudentFromMetricVariantId(schoolId, metricVariantId);
            var childMetricIds = metricMetadataNode.Children.Select(x => x.MetricId).ToArray();

            var results = (from data in repository.GetAll()
                           where data.SchoolId == schoolId && data.StudentUSI == studentUSI && childMetricIds.Contains(data.MetricId)
                           orderby data.MetricId, data.AdministrationYear
                           group data by data.AdministrationYear into metricGroup
                           select new { Year = metricGroup.Key, Data = metricGroup }).ToList().OrderByDescending(x => x.Year).Take(numberOfYearsToShow).OrderBy(x => x.Year);

            if (!results.Any())
                return null;

            var model = new ChartData();

            model.ChartTitle = metricMetadataNode.Actions.GetChartTitle(request.Title);
            model.AxisXTitle = String.Format(titleFormat, metricMetadataNode.DisplayName);
            model.YAxisMaxValue = 6200; // State Assessment Max Value
            var yearCount = results.Count();
            var i = 0;
            foreach (var result in results)
            {
                i++;

                ChartData.Series series = null;
                foreach (var metric in result.Data)
                {
                    if (series == null)
                    {
						series = new ChartData.Series { Name = metric.Context, Style = ChartSeriesStyle.Gray };
                    }

                    if (!metric.NonNormalized.HasValue)
                        continue;

                    var point = new ChartData.Point
                    {
                        AxisLabel = ShortenMetricName(metricMetadataNode.Children.Single(x => x.MetricId == metric.MetricId && x.MetricVariantType == metricMetadataNode.MetricVariantType).DisplayName),
                        IsValueShownAsLabel = true,
                        Value = metric.NonNormalized.HasValue ? (double)metric.NonNormalized : 0,
                        Label = metric.NonNormalized.ToString(),
                        Tooltip = metric.NonNormalized.ToString(),
                        State = MetricStateType.None
                    };
                    if (i == yearCount)
                    {
                        point.State = metric.MetricStateTypeId.HasValue ? (MetricStateType)metric.MetricStateTypeId : MetricStateType.None;
                    }
                    series.Points.Add(point);
                }

                if (series.Points.Any())
                    model.SeriesCollection.Add(series);
            }

            if (!model.SeriesCollection.Any())
                return null;

            return model;
        }

        private static string ShortenMetricName(string metricName)
        {
            switch (metricName)
            {
                case "Reading":
                    return "R";
                case "ELA / Reading":
                    return "ELA";
                case "Mathematics":
                    return "M";
                case "Science":
                    return "Sc";
                case "Social Studies":
                    return "SS";
                case "Writing":
                    return "W";
            }

            return metricName;
        }
    }
}
