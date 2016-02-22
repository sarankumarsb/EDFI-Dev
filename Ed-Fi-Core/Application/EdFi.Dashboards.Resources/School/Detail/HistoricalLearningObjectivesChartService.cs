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
using MetricStateType = EdFi.Dashboards.Metric.Resources.Models.MetricStateType;
using MetricVariantType = EdFi.Dashboards.Metric.Resources.Models.MetricVariantType;

namespace EdFi.Dashboards.Resources.School.Detail
{
    public class HistoricalLearningObjectivesChartRequest
    {
        public int SchoolId { get; set; }
        public int MetricVariantId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoricalLearningObjectivesChartRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="HistoricalLearningObjectivesChartRequest"/> instance.</returns>
        public static HistoricalLearningObjectivesChartRequest Create(int schoolId, int metricVariantId)
        {
            return new HistoricalLearningObjectivesChartRequest { SchoolId = schoolId, MetricVariantId = metricVariantId };
        }
    }

    public interface IHistoricalLearningObjectivesChartService : IService<HistoricalLearningObjectivesChartRequest, ChartData> { }

    public class HistoricalLearningObjectivesChartService : IHistoricalLearningObjectivesChartService
    {
        private const string schoolGoalFormat = "School Goal: {0}";
        private readonly IList<string> yearPeriods = new List<string> { "BOY", "MOY", "EOY" };
        private readonly IMetricDataService<SchoolMetricInstanceSetRequest> metricDataService;
        private readonly IMetricMetadataTreeService metricMetadataNodeService;
        private readonly IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        private readonly IMetricGoalProvider metricGoalProvider;
        private readonly IMetricNodeResolver metricNodeResolver;

        public HistoricalLearningObjectivesChartService(IMetricDataService<SchoolMetricInstanceSetRequest> metricDataService, 
                                                  IMetricMetadataTreeService metricMetadataNodeService, 
                                                  IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver, 
                                                  IMetricGoalProvider metricGoalProvider,
                                                  IMetricNodeResolver metricNodeResolver)
        {
            this.metricDataService = metricDataService;
            this.metricMetadataNodeService = metricMetadataNodeService;
            this.metricInstanceSetKeyResolver = metricInstanceSetKeyResolver;
            this.metricGoalProvider = metricGoalProvider;
            this.metricNodeResolver = metricNodeResolver;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public ChartData Get(HistoricalLearningObjectivesChartRequest request)
        {
            var schoolId = request.SchoolId;
            var metricVariantId = request.MetricVariantId;
            var metricMetadataNode = metricNodeResolver.GetMetricNodeForSchoolFromMetricVariantId(schoolId, metricVariantId);
            var metricId = metricMetadataNode.MetricId;

            if (String.IsNullOrEmpty(metricMetadataNode.Format))
                throw new ArgumentNullException(string.Format("List Format is null for metricVariantId:{0}", request.MetricVariantId));

            var metricInstanceSetKey = metricInstanceSetKeyResolver.GetMetricInstanceSetKey(SchoolMetricInstanceSetRequest.Create(schoolId, metricVariantId));
            var metricDataContainer = metricDataService.Get(SchoolMetricInstanceSetRequest.Create(schoolId, metricVariantId));
            var metricMetadata = new MetricMetadataNode(null) { MetricVariantType = MetricVariantType.CurrentYear };
            var metricData = metricDataContainer.GetMetricData(metricMetadata);

            var metricTreeForCurrentEntity = metricMetadataNodeService.Get(MetricMetadataTreeRequest.Create()).AllNodesByMetricNodeId[metricMetadataNode.MetricNodeId];

            if (metricTreeForCurrentEntity == null || metricData == null || metricData.MetricInstanceExtendedProperties == null || !metricData.MetricInstanceExtendedProperties.Any())
                return null;

            var data = metricData.MetricInstanceExtendedProperties.Where(miep => miep.MetricId == metricId);

            if (!data.Any())
                return null;

            var distinctYears = GetOrderedYears(data.Select(prop => prop.Name.Trim()));

            //if there is not a Name that contains a year, we will return a null
            if (!distinctYears.Any())
                return null;

            var goal = metricGoalProvider.GetMetricGoal(metricInstanceSetKey, metricId);
            var goalValue = Convert.ToDouble(goal.Value);

            var dataPoints = GetChartDataPoints(data, distinctYears, goalValue, metricMetadataNode.Format);

            if (dataPoints.Count == 0)
                return null;

            double maxValue = dataPoints.Max(x => x.Value);
            if (goalValue > maxValue)
                maxValue = goalValue;

            string chartId = String.Format("{0}_HLOC_{1}", schoolId, metricVariantId);
            var series = new ChartData.Series { Name = "Historical", Points = dataPoints };
            var stripLine = new ChartData.StripLine { Value = goalValue, Tooltip = String.Format(schoolGoalFormat, String.Format(metricMetadataNode.Format, goal.Value)), IsNegativeThreshold = goal.Interpretation == TrendInterpretation.Inverse };
            
            return new ChartData
                        {
                            SeriesCollection = new List<ChartData.Series> { series },
                            StripLines = new List<ChartData.StripLine> { stripLine },
                            YAxisMaxValue = maxValue,
                            ChartId = chartId,
                            Goal = String.Format(schoolGoalFormat, String.Format(metricMetadataNode.Format, goal.Value))
                        };
        }

        private static IEnumerable<int> GetOrderedYears(IEnumerable<string> propertyNames)
        {
            var results = new List<int>();

            foreach (var propertyName in propertyNames)
            {
                var splitValues = propertyName.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                //if count != 2, then this means this is not in the expected format of "MOY 2012"
                if (splitValues.Count() != 2)
                    continue;

                var year = -1;

                //if the 2nd value is not a valid number, we need to skip it
                if (!int.TryParse(splitValues[1], out year))
                    continue;

                if (!results.Contains(year))
                    results.Add(year);
            }

            return results.OrderBy(value => value);
        }

        private List<ChartData.Point> GetChartDataPoints(IEnumerable<MetricInstanceExtendedProperty> data, IEnumerable<int> distinctYears, double goal, string labelFormat)
        {
            var dataPoints = new List<ChartData.Point>();

            foreach (var distinctYear in distinctYears)
            {
                foreach (var yearPeriod in yearPeriods)
                {
                    var period = String.Format("{0} {1}", yearPeriod, distinctYear);
                    var chartDataPoint = GetChartDataPoint(data.Where(d => String.Compare(d.Name, period, true) == 0), goal, labelFormat);
                    if (chartDataPoint != null)
                        dataPoints.Add(chartDataPoint);
                }
            }

            return dataPoints;
        }

        private static ChartData.Point GetChartDataPoint(IEnumerable<MetricInstanceExtendedProperty> matchingData, double goal, string labelFormat)
        {
            if (!matchingData.Any())
                return null;

            var matchingProp = matchingData.First();

            if (string.IsNullOrEmpty(matchingProp.Value))
                return null;

            var pointValue = Convert.ToDouble(matchingProp.Value);

            return new ChartData.Point
            {
                Value = pointValue,
                Label = String.Format(labelFormat, pointValue),
                AxisLabel = matchingProp.Name,
                State = GetDataPointState(matchingProp, goal),
                Trend = TrendEvaluation.None,
                IsValueShownAsLabel = true
            };
        }

        private static MetricStateType GetDataPointState(MetricInstanceExtendedProperty property, double baseLineValue)
        {
            var v = Convert.ToDouble(property.Value);
            return v >= baseLineValue ? MetricStateType.Good : MetricStateType.Low;
        }
    }
}
