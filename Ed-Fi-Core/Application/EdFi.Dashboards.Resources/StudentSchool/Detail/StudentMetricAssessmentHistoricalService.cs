using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Models.Charting;
using EdFi.Dashboards.Resources.Models.Student.Detail;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.StudentSchool.Detail
{
    public class StudentMetricAssessmentHistoricalRequest
    {
        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }
        public int MetricVariantId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StudentMetricAssessmentHistoricalRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="StudentMetricAssessmentHistoricalRequest"/> instance.</returns>
        public static StudentMetricAssessmentHistoricalRequest Create(long studentUSI, int schoolId, int metricVariantId)
        {
            return new StudentMetricAssessmentHistoricalRequest { StudentUSI = studentUSI, SchoolId = schoolId, MetricVariantId = metricVariantId };
        }
    }

    public interface IStudentMetricAssessmentHistoricalService : IService<StudentMetricAssessmentHistoricalRequest, StudentMetricAssessmentHistoricalModel> { }

    public class StudentMetricAssessmentHistoricalService : IStudentMetricAssessmentHistoricalService
    {
        private readonly IRepository<StudentMetricAssessmentHistorical> studentMetricAssessmentHistoricalRepository;
        private readonly IRepository<StudentMetricAssessmentHistoricalMetaData> studentMetricAssessmentHistoricalMetaDataRepository;
        private readonly IMetricGoalProvider metricGoalProvider;
        private readonly IMetricInstanceSetKeyResolver<StudentSchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        private readonly IMetricNodeResolver metricNodeResolver;

        public StudentMetricAssessmentHistoricalService(IRepository<StudentMetricAssessmentHistorical> studentMetricAssessmentHistoricalRepository,
                                                IRepository<StudentMetricAssessmentHistoricalMetaData> studentMetricAssessmentHistoricalMetaDataRepository,
                                                IMetricGoalProvider metricGoalProvider,
                                                IMetricInstanceSetKeyResolver<StudentSchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver,
                                                IMetricNodeResolver metricNodeResolver)
        {
            this.studentMetricAssessmentHistoricalRepository = studentMetricAssessmentHistoricalRepository;
            this.studentMetricAssessmentHistoricalMetaDataRepository = studentMetricAssessmentHistoricalMetaDataRepository;
            this.metricGoalProvider = metricGoalProvider;
            this.metricInstanceSetKeyResolver = metricInstanceSetKeyResolver;
            this.metricNodeResolver = metricNodeResolver;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public virtual StudentMetricAssessmentHistoricalModel Get(StudentMetricAssessmentHistoricalRequest request)
        {
            var metricVariantId = request.MetricVariantId;
            var schoolId = request.SchoolId;
            var studentUSI = request.StudentUSI;
            var metricMetadataNode = metricNodeResolver.GetMetricNodeForStudentFromMetricVariantId(schoolId, metricVariantId);
            var metricId = metricMetadataNode.MetricId;

            if (String.IsNullOrEmpty(metricMetadataNode.DisplayName))
                throw new ArgumentNullException(string.Format("Metric Display Name is null for metric variant Id:{0}", metricVariantId));

            string chartId = String.Format("{0}_{1}_{2}", schoolId, studentUSI, metricVariantId);

            var model = new StudentMetricAssessmentHistoricalModel{StudentUSI = studentUSI, DrillDownTitle = string.Empty};

            model.ChartData.ChartId = chartId;
            model.ChartData.OverviewChartIsEnabled = false;
            model.ChartData.ShowMouseOverToolTipOnLeft = false;
            model.ChartData.YAxisMaxValue = 1.0;

            var data = GetData(studentUSI, metricId);

            if (data.Count == 0)
                return model;

            var metadata = GetMetaData(studentUSI, metricId);

            if (metadata != null)
            {
                model.ChartData.Context = metadata.Context;
                model.ChartData.SubContext = metadata.SubContext;
                model.ChartData.DisplayType = metadata.DisplayType;
            }

            model.ChartData.StripLines.AddRange(GetStripLines(studentUSI, metricId, metricVariantId, schoolId, metricMetadataNode.Format));

            model.ChartData.Goal = GetGoal(model, data);

            model.ChartData.SeriesCollection.AddRange(GetChartSeries(data, metricId, metricMetadataNode));

            model.ChartData.YAxisLabels.AddRange(GetYAxisLabels(metadata, data));

            return model;
        }

        protected virtual string GetGoal(StudentMetricAssessmentHistoricalModel model, List<StudentMetricAssessmentHistorical> data)
        {
            var stripLines = model.ChartData.StripLines;
            var firstStripLine = stripLines.FirstOrDefault();
            if (firstStripLine != null)
                return firstStripLine.Tooltip;
            return null;
        }

        protected virtual List<StudentMetricAssessmentHistorical> GetData(long studentUSI, int metricId)
        {
            var data = (from d in studentMetricAssessmentHistoricalRepository.GetAll()
                        where d.StudentUSI == studentUSI
                              && d.MetricId == metricId
                        orderby d.DisplayOrder
                        select d).ToList();
            return data;
        }

        protected virtual StudentMetricAssessmentHistoricalMetaData GetMetaData(long studentUSI, int metricId)
        {
            var metadata = (from d in studentMetricAssessmentHistoricalMetaDataRepository.GetAll()
                            where d.StudentUSI == studentUSI
                                  && d.MetricId == metricId
                            select d).FirstOrDefault();
            return metadata;
        }

        protected virtual IEnumerable<ChartData.AxisLabel> GetYAxisLabels(StudentMetricAssessmentHistoricalMetaData metadata, List<StudentMetricAssessmentHistorical> benchmarkData)
        {
            if(metadata == null)
                return new ChartData.AxisLabel[0];

            switch (metadata.LabelType)
            {
                case "Percent":
                    return GetYAxisLabelsDefaultPercentages();
                case "Percentile":
                    return GetYAxisLabelsDefaultPercentiles();
                case "ABELevels":
                    return GetYAxisLabelsByAbeLevels();
                default:
                    return new ChartData.AxisLabel[0];
            }
        }

        protected virtual IEnumerable<ChartData.AxisLabel> GetYAxisLabelsByAbeLevels()
        {
            return new[]
                       {
                           new ChartData.AxisLabel { MinPosition = .00m, MaxPosition = 0.25m, Text = "Below Basic" },
                           new ChartData.AxisLabel { MinPosition = .25m, MaxPosition = 0.50m, Text = "Basic" },
                           new ChartData.AxisLabel { MinPosition = .50m, MaxPosition = 0.75m, Text = "Proficient" },
                           new ChartData.AxisLabel { MinPosition = .75m, MaxPosition = 1.00m, Text = "Advanced" }
                       };
        }

        protected virtual IEnumerable<ChartData.AxisLabel> GetYAxisLabelsDefaultPercentages()
        {
            return new[]
                       {
                           new ChartData.AxisLabel{ Position = 0m,   Text = "0%" },
                           new ChartData.AxisLabel{ Position = .25m, Text = "25%" },
                           new ChartData.AxisLabel{ Position = .50m, Text = "50%" },
                           new ChartData.AxisLabel{ Position = .75m, Text = "75%" },
                           new ChartData.AxisLabel{ Position = 1.0m, Text = "100%" }
                       };
        }

        protected virtual IEnumerable<ChartData.AxisLabel> GetYAxisLabelsDefaultPercentiles()
        {
            return new[]
                       {
                           new ChartData.AxisLabel{ Position = 0m,   Text = "0" },
                           new ChartData.AxisLabel{ Position = .25m, Text = "25" },
                           new ChartData.AxisLabel{ Position = .50m, Text = "50" },
                           new ChartData.AxisLabel{ Position = .75m, Text = "75" },
                           new ChartData.AxisLabel{ Position = 1.0m, Text = "100" }
                       };
        }

        protected virtual ChartData.StripLine[] GetStripLines(long studentUSI, int metricId, int metricVariantId, int schoolId, string format)
        {
            var metricInstanceSetKey = metricInstanceSetKeyResolver.GetMetricInstanceSetKey(StudentSchoolMetricInstanceSetRequest.Create(schoolId, studentUSI, metricVariantId));
            var metricGoal = metricGoalProvider.GetMetricGoal(metricInstanceSetKey, metricId);
            if (metricGoal != null && metricGoal.Value != null)
            {
                //If we have a goal then add a StripLine.
                var stripLine = new ChartData.StripLine
                {
                    Value = Convert.ToDouble(metricGoal.Value),
                    IsNegativeThreshold = metricGoal.Interpretation == TrendInterpretation.Inverse
                };
                return new[] {stripLine};
            }
            return new ChartData.StripLine[0];
        }

        protected virtual ChartData.Series[] GetChartSeries(IEnumerable<StudentMetricAssessmentHistorical> data, int metricId, MetricMetadataNode metricMetadataNode)
        {
            decimal? previousValue = null;
            var result = new List<ChartData.Point>();
            foreach (var entry in data)
            {
                decimal? value = null;
                string valueAsText = null;
                if (!String.IsNullOrEmpty(entry.Value))
                {
                    value = Convert.ToDecimal(entry.Value);
                    if (metricMetadataNode.Format != null)
                        valueAsText = string.Format(metricMetadataNode.Format, value);
                    else
                        valueAsText = entry.Value;
                }
                var trend = GetTrend(previousValue, value);
                var point = MapDataToPoint(entry, valueAsText, trend);
                previousValue = value;
                result.Add(point);
            }
            return new[] {new ChartData.Series
            {
                Name = metricMetadataNode.DisplayName,
                Points = result
            }};
        }

        private static ChartData.Point MapDataToPoint(StudentMetricAssessmentHistorical entry, string valueAsText, TrendEvaluation trend)
        {
            var point = new ChartData.Point
                            {
                                Value = (string.IsNullOrEmpty(entry.Value)) ? 0 : Convert.ToDouble(entry.Value),
                                ValueAsText = valueAsText,
                                ValueType = entry.ValueTypeName,
                                TooltipHeader = (entry.ToolTipContext ?? string.Empty),
                                Tooltip = (entry.ToolTipSubContext ?? string.Empty),
                                Label = (entry.Context ?? string.Empty),
                                SubLabel = (entry.SubContext ?? string.Empty),
                                State = entry.MetricStateTypeId.HasValue
                                        ? (MetricStateType) entry.MetricStateTypeId.Value
                                        : MetricStateType.None,
                                Trend = trend,
                                RatioLocation = entry.PerformanceLevelRatio
                            };
            return point;
        }

        protected virtual TrendEvaluation GetTrend(decimal? previousValue, decimal? currentValue)
        {
            if (!previousValue.HasValue || !currentValue.HasValue)
                return TrendEvaluation.None;

            var compare = currentValue.Value.CompareTo(previousValue);
            switch (compare)
            {
                case 1://Up /\
                    return TrendEvaluation.UpNoOpinion;
                case 0://Stays the same <>
                    return TrendEvaluation.NoChangeNoOpinion;
                case -1://Down \/
                    return TrendEvaluation.DownNoOpinion;
                default:
                    throw new NotSupportedException(string.Format("'{0}' is an supported value for Trend Evaluation.", compare));
            }
        }
    }
}
