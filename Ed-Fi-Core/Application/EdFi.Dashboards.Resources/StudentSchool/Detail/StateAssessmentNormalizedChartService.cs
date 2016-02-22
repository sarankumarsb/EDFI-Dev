using System;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Models.Charting;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.StudentSchool.Detail
{
    public class StateAssessmentNormalizedChartRequest
    {
        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }
        public int MetricVariantId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateAssessmentNormalizedChartRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="StateAssessmentNormalizedChartRequest"/> instance.</returns>
        public static StateAssessmentNormalizedChartRequest Create(long studentUSI, int schoolId, int metricVariantId)
        {
            return new StateAssessmentNormalizedChartRequest { StudentUSI = studentUSI, SchoolId = schoolId, MetricVariantId = metricVariantId };
        }
    }

    public interface IStateAssessmentNormalizedChartService : IService<StateAssessmentNormalizedChartRequest, ChartData> { }

    public class StateAssessmentNormalizedChartService : IStateAssessmentNormalizedChartService
    {
        private const int limitMetStandard = 60;
        private const int limitCommended = 90;
        private const string metStandard = "Met Standard";
        private const string commended = "Commended";
        private const string titleFormat = "Normalized {0} Scores";
        private const int numberOfYearsToShow = 4;
        private readonly IRepository<StudentMetricStateAssessmentHistorical> repository;
        private readonly IMetricNodeResolver metricNodeResolver;

        public StateAssessmentNormalizedChartService(IRepository<StudentMetricStateAssessmentHistorical> repository, IMetricNodeResolver metricNodeResolver)
        {
            this.repository = repository;
            this.metricNodeResolver = metricNodeResolver;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public ChartData Get(StateAssessmentNormalizedChartRequest request)
        {
            int metricVariantId = request.MetricVariantId;
            int schoolId = request.SchoolId;
            var studentUSI = request.StudentUSI;

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

            model.AxisXTitle = String.Format(titleFormat, metricMetadataNode.DisplayName);
            model.StripLines.Add(new ChartData.StripLine { Value = limitMetStandard, Tooltip = metStandard, Label = metStandard });
            model.StripLines.Add(new ChartData.StripLine { Value = limitCommended, Tooltip = commended, Label = commended, Color = ChartColors.Green });


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

                    if (!metric.Normalized.HasValue)
                        continue;

                    var point = new ChartData.Point
                    {
                        AxisLabel = ShortenMetricName(metricMetadataNode.Children.Single(x => x.MetricId == metric.MetricId && x.MetricVariantType == metricMetadataNode.MetricVariantType).DisplayName),
                        IsValueShownAsLabel = true,
                        Value = (double)metric.Normalized,
                        Label = metric.NonNormalized.ToString(),
                        Tooltip = metric.NonNormalized.ToString(),
                        State = MetricStateType.None
                    };
                    if (i == yearCount)
                    {
                        point.State = metric.Normalized >= limitMetStandard ? MetricStateType.Good : MetricStateType.Low;
                    }
                    series.Points.Add(point);
                }

                model.SeriesCollection.Add(series);
            }

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
