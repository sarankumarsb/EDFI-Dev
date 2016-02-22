using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Models.Charting;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using MetricVariantType = EdFi.Dashboards.Metric.Resources.Models.MetricVariantType;

namespace EdFi.Dashboards.Resources.StudentSchool.Detail
{
    public class HistoricalLearningObjectivesChartRequest
    {
        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }
        public int MetricVariantId { get; set; }
        [AuthenticationIgnore("Title is not a relevant property outside of forming summaries")]
        public string Title { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoricalLearningObjectivesChartRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="HistoricalLearningObjectivesChartRequest"/> instance.</returns>
        public static HistoricalLearningObjectivesChartRequest Create(long studentUSI, int schoolId, int metricVariantId, string title)
        {
            return new HistoricalLearningObjectivesChartRequest { StudentUSI = studentUSI, SchoolId = schoolId, MetricVariantId = metricVariantId, Title = title };
        }
    }

    public interface IHistoricalLearningObjectivesChartService : IService<HistoricalLearningObjectivesChartRequest, ChartData> { }

    public class HistoricalLearningObjectivesChartService : IHistoricalLearningObjectivesChartService
    {
        private readonly IList<string> yearPeriods = new List<string> { "BOY", "MOY", "EOY" };
        private readonly IMetricDataService<StudentSchoolMetricInstanceSetRequest> metricDataService;
        private readonly IMetricInstanceSetKeyResolver<StudentSchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        private readonly IHistoricalLearningObjectiveProvider historicalLearningObjectiveProvider;
        private readonly IMetricGoalProvider metricGoalProvider;
        private readonly IMetricNodeResolver metricNodeResolver;

        public HistoricalLearningObjectivesChartService(IMetricDataService<StudentSchoolMetricInstanceSetRequest> metricDataService, 
                                                        IMetricInstanceSetKeyResolver<StudentSchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver, 
                                                        IHistoricalLearningObjectiveProvider historicalLearningObjectiveProvider, 
                                                        IMetricGoalProvider metricGoalProvider,
                                                        IMetricNodeResolver metricNodeResolver)
        {
            this.metricDataService = metricDataService;
            this.metricInstanceSetKeyResolver = metricInstanceSetKeyResolver;
            this.historicalLearningObjectiveProvider = historicalLearningObjectiveProvider;
            this.metricGoalProvider = metricGoalProvider;
            this.metricNodeResolver = metricNodeResolver;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public virtual ChartData Get(HistoricalLearningObjectivesChartRequest request)
        {
            var studentUSI = request.StudentUSI;
            int schoolId = request.SchoolId;
            int metricVariantId = request.MetricVariantId;
            int metricId = metricNodeResolver.ResolveMetricId(metricVariantId);
            var metricMetadataNode = metricNodeResolver.GetMetricNodeForStudentFromMetricVariantId(schoolId, metricVariantId);

            var metricInstanceSetKey = metricInstanceSetKeyResolver.GetMetricInstanceSetKey(StudentSchoolMetricInstanceSetRequest.Create(schoolId, studentUSI, metricVariantId));

            var metricDataContainer = metricDataService.Get(StudentSchoolMetricInstanceSetRequest.Create(schoolId, studentUSI, metricVariantId));
            var metricData = metricDataContainer.GetMetricData(metricMetadataNode);
            if (metricData == null || metricData.MetricInstanceExtendedProperties == null || !metricData.MetricInstanceExtendedProperties.Any())
                return null;

            var data = metricData.MetricInstanceExtendedProperties.Where(miep => miep.MetricId == metricId);

            if (!data.Any())
                return null;

            var distinctYears = GetOrderedYears(data.Select(prop => prop.Name.Trim()));

            //if there is not a Name that contains a year, we will return a null
            if (!distinctYears.Any())
                return null;

            double baseLine = -1;
            var metricGoal = metricGoalProvider.GetMetricGoal(metricInstanceSetKey, metricId);
            if (metricGoal != null && metricGoal.Value.HasValue)
                baseLine = Convert.ToDouble(metricGoal.Value);

            var chartData = new ChartData();

            var dataPoints = GetChartDataPoints(metricVariantId, data, distinctYears, baseLine);

            if (!dataPoints.Any())
                return null;

            chartData.SeriesCollection.Add(new ChartData.Series
            {
                Name = "Historical",
                Points = dataPoints
            });

            chartData.ChartTitle = metricMetadataNode.Actions.GetChartTitle(request.Title);

            var stripLine = historicalLearningObjectiveProvider.GetStripeLine(metricVariantId, baseLine);

            if (stripLine != null)
                chartData.StripLines.Add(stripLine);

            chartData.Goal = historicalLearningObjectiveProvider.GetCampusGoal(metricVariantId, baseLine);

            return chartData;
        }

        protected virtual IEnumerable<int> GetOrderedYears(IEnumerable<string> propertyNames)
        {
            var results = new List<int>();

            foreach (var propertyName in propertyNames)
            {
                var splitValues = propertyName.Split(new [] { " " }, StringSplitOptions.RemoveEmptyEntries);

                //if count != 2, then this means this is not in the expected format of "MOY 2012"
                if (splitValues.Count() != 2)
                    continue;

                int year;

                //if the 2nd value is not a valid number, we need to skip it
                if (!int.TryParse(splitValues[1], out year))
                    continue;

                if (!results.Contains(year))
                    results.Add(year);
            }

            return results.OrderBy(value => value);
        }

        protected virtual List<ChartData.Point> GetChartDataPoints(int metricVariantId, IEnumerable<MetricInstanceExtendedProperty> data, IEnumerable<int> distinctYears, double baseLineValue)
        {
            var dataPoints = new List<ChartData.Point>();

            foreach (var distinctYear in distinctYears)
            {
                foreach (var yearPeriod in yearPeriods)
                {
                    var numerator = data.Where(prop => String.Compare(string.Format("{0} {1} Numerator", yearPeriod, distinctYear), prop.Name, StringComparison.OrdinalIgnoreCase) == 0);
                    var denominator = data.Where(prop => String.Compare(string.Format("{0} {1} Denominator", yearPeriod, distinctYear), prop.Name, StringComparison.OrdinalIgnoreCase) == 0);
                    var metricState = data.FirstOrDefault(prop => String.Compare(string.Format("{0} {1} State", yearPeriod, distinctYear), prop.Name, StringComparison.OrdinalIgnoreCase) == 0);
                    if (metricState != null) baseLineValue = Convert.ToDouble(metricState.Value);
                    var dataPoint = GetChartDataPoint(metricVariantId, data.Where(prop => String.Compare(string.Format("{0} {1}", yearPeriod, distinctYear), prop.Name, StringComparison.OrdinalIgnoreCase) == 0), numerator, denominator, baseLineValue);

                    if (dataPoint != null)
                        dataPoints.Add(dataPoint);
                }
            }

            return dataPoints;
        }

        protected virtual ChartData.Point GetChartDataPoint(int metricVariantId, IEnumerable<MetricInstanceExtendedProperty> matchingData, IEnumerable<MetricInstanceExtendedProperty> numeratorData, IEnumerable<MetricInstanceExtendedProperty> denominatorData, double baseLineValue)
        {
            if (!matchingData.Any())
                return null;

            var matchingProp = matchingData.First();

            if (string.IsNullOrEmpty(matchingProp.Value))
                return null;

            return new ChartData.Point
            {
                Value = historicalLearningObjectiveProvider.GetDataPointValue(metricVariantId, matchingProp),
                AxisLabel = matchingProp.Name,
                Tooltip = historicalLearningObjectiveProvider.GetDataPointToolTip(metricVariantId, matchingProp, numeratorData, denominatorData),
                State = historicalLearningObjectiveProvider.GetDataPointState(metricVariantId, matchingProp, baseLineValue),
                Trend = TrendEvaluation.None,
                IsValueShownAsLabel = true,
                Label = historicalLearningObjectiveProvider.GetDataPointLabel(metricVariantId, matchingProp)
            };
        }
    }
}
