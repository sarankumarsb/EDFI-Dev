// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Models.Charting;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Models.Student.Detail;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.StudentSchool.Detail
{
    public class BenchmarkHistoricalChartRequest
    {
        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }
        public int MetricVariantId { get; set; }
        [AuthenticationIgnore("Title is not a relevant property outside of forming summaries")]
        public string Title { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BenchmarkHistoricalChartRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="BenchmarkHistoricalChartRequest"/> instance.</returns>
        public static BenchmarkHistoricalChartRequest Create(long studentUSI, int schoolId, int metricVariantId, string title) 
        {
            return new BenchmarkHistoricalChartRequest { StudentUSI = studentUSI, SchoolId = schoolId, MetricVariantId = metricVariantId, Title = title};
        }
    }

    public interface IBenchmarkHistoricalChartService : IService<BenchmarkHistoricalChartRequest, BenchmarkModel> { }

    public class BenchmarkHistoricalChartService : IBenchmarkHistoricalChartService
    {
        private readonly IRepository<StudentMetricBenchmarkAssessment> studentMetricBenchmarkAssessmentRepository;
        private readonly IMetricGoalProvider metricGoalProvider;
        private readonly IMetricStateProvider metricStateProvider;
        private readonly IMetricInstanceSetKeyResolver<StudentSchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        private readonly IMetricNodeResolver metricNodeResolver;

        public BenchmarkHistoricalChartService(IRepository<StudentMetricBenchmarkAssessment> studentMetricBenchmarkAssessmentRepository,
                                                IMetricGoalProvider metricGoalProvider,
                                                IMetricStateProvider metricStateProvider,
                                                IMetricInstanceSetKeyResolver<StudentSchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver,
                                                IMetricNodeResolver metricNodeResolver)
        {
            this.studentMetricBenchmarkAssessmentRepository = studentMetricBenchmarkAssessmentRepository;
            this.metricGoalProvider = metricGoalProvider;
            this.metricStateProvider = metricStateProvider;
            this.metricInstanceSetKeyResolver = metricInstanceSetKeyResolver;
            this.metricNodeResolver = metricNodeResolver;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public BenchmarkModel Get(BenchmarkHistoricalChartRequest request)
        {
            var metricVariantId = request.MetricVariantId;
            var schoolId = request.SchoolId;
            var studentUSI = request.StudentUSI;
            var metricMetadataNode = metricNodeResolver.GetMetricNodeForStudentFromMetricVariantId(schoolId, metricVariantId);
            var metricId = metricMetadataNode.MetricId;

            if (String.IsNullOrEmpty(metricMetadataNode.Format))
                throw new ArgumentNullException(string.Format("Format is null for metric variant Id:{0}", metricVariantId));
            if (String.IsNullOrEmpty(metricMetadataNode.DisplayName))
                throw new ArgumentNullException(string.Format("Metric Display Name is null for metric variant Id:{0}", metricVariantId));

            string chartId = String.Format("{0}_{1}_{2}", schoolId, studentUSI, metricVariantId);

            var model = new BenchmarkModel(studentUSI);

            model.ChartData.ChartId = chartId;

            var benchmarkData = (from d in studentMetricBenchmarkAssessmentRepository.GetAll()
                                 where d.StudentUSI == studentUSI
                                         && d.SchoolId == schoolId
                                         && d.MetricId == metricId
                                 orderby d.Date, d.Version
                                 select d).ToList();

            if (benchmarkData.Count == 0)
                return model;

            //Prepare metadata that is needed.
            var tempObjForValueType = benchmarkData.FirstOrDefault(x => !string.IsNullOrEmpty(x.ValueType));

            if(tempObjForValueType==null)
                throw new ArgumentNullException(string.Format("ValueType is null for [schoolId:{0}, studentUSI:{1}, metricId:{2}].", schoolId, studentUSI, metricMetadataNode.MetricId));

            var valueType = tempObjForValueType.ValueType;
            var format = metricMetadataNode.Format;
            var metricName = metricMetadataNode.DisplayName;

            model.DrillDownTitle = "";//metricName;

            //Get the metric Goal...
            GetStripLine(studentUSI, model, metricId, metricVariantId, schoolId, valueType, format);

            //Build the needed series and return the chart data.
            var series = new ChartData.Series { Name = metricName };
            series.Points = GetChartData(benchmarkData, metricId);
            model.ChartData.SeriesCollection.Add(series);
            model.ChartData.Goal = model.ChartData.StripLines[0].Tooltip;

            model.ChartData.ChartTitle = metricMetadataNode.Actions.GetChartTitle(request.Title);

            return model;
        }

        private void GetStripLine(long studentUSI, BenchmarkModel model, int metricId, int metricVariantId, int schoolId, string valueType, string format)
        {
            var metricInstanceSetKey = metricInstanceSetKeyResolver.GetMetricInstanceSetKey(StudentSchoolMetricInstanceSetRequest.Create(schoolId, studentUSI, metricVariantId));
            var metricGoal = metricGoalProvider.GetMetricGoal(metricInstanceSetKey, metricId);
            if (metricGoal != null)
            {
                var schoolGoalValue = metricGoal.Value.ToString();
                if (valueType == "System.Int32")
                {
                    schoolGoalValue = Convert.ToInt32(metricGoal.Value).ToString();
                }

                //If we have a goal then add a StripLine.
                var stripLine = new ChartData.StripLine
                                    {
                                        Value = Convert.ToDouble(metricGoal.Value),
                                        Tooltip = string.Format("School Goal: {0}", String.Format(format, InstantiateValue.FromValueType(schoolGoalValue, valueType))),
                                        IsNegativeThreshold = metricGoal.Interpretation == TrendInterpretation.Inverse
                                    };
                model.ChartData.StripLines.Add(stripLine);
            }
        }

        private List<ChartData.Point> GetChartData(IEnumerable<StudentMetricBenchmarkAssessment> data, int metricId)
        {
            decimal? previousValue = null;
            var result = new List<ChartData.Point>();
            foreach (var assessment in data)
            {
                decimal? value = null;
                if (!String.IsNullOrEmpty(assessment.Value))
                    value = Convert.ToDecimal(assessment.Value);

                var point = new ChartData.Point
                                {
                                    Value = (string.IsNullOrEmpty(assessment.Value)) ? 0 : Convert.ToDouble(assessment.Value),
                                    ValueType = assessment.ValueType,
                                    Tooltip = assessment.AssessmentTitle,
                                    Label = assessment.Date.ToShortDateString(),
                                    State = metricStateProvider.GetState(metricId, assessment.Value, assessment.ValueType).StateType, 
                                    Trend = GetTrend(previousValue, value)
                                };
                previousValue = value;
                result.Add(point);
            }
            return result;
        }

        private static TrendEvaluation GetTrend(decimal? previousValue, decimal? currentValue)
        {
            if (!previousValue.HasValue || !currentValue.HasValue)
                return TrendEvaluation.None;

            var compare = currentValue.Value.CompareTo(previousValue);
            switch(compare)
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
