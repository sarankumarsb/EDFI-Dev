// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.MetricInstanceSetKeyResolvers;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Models.Student.Detail;
using EdFi.Dashboards.Resources.StudentSchool.Detail;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Student.Detail
{
    [TestFixture]
    public class When_building_historical_benchmark_assessment : TestFixtureBase
    {
        #region Fields
        //The Injected Dependencies.
        private IRepository<StudentMetricBenchmarkAssessment> studentMetricBenchmarkAssessmentRepository;
        private IMetricGoalProvider metricGoalProvider;
        private IMetricStateProvider metricStateProvider;
        private IMetricInstanceSetKeyResolver<StudentSchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        private IMetricNodeResolver metricNodeResolver;

        //The Actual Model.
        private BenchmarkModel actualModel;

        //The supplied Data models.
        private const int suppliedStudentUSI = 1;
        private const int suppliedSchoolId = 2;
        private const int suppliedMetricId = 3;
        private const int suppliedMetricVariantId = 4;
        private readonly Guid suppliedMetricInstanceSetKey = Guid.NewGuid();
        private Goal suppliedSchoolGoal;
        private State suppliedMetricState;
        private const string suppliedMetricValueStr = "84.3";
        private const string suppliedAssessmentTitle1 = "Assessment Title 1";
        private const string suppliedAssessmentTitle2 = "Assessment Title 2";
        private const string suppliedAssessmentTitle3 = "Assessment Title 3";
        private const string suppliedAssessmentTitle4 = "Assessment Title 4";
        private const string suppliedMetricName = "Metric Name";
        private const string suppliedMetricDisplayName = "Metric Display Name";
        private const string suppliedMetricFormat = "{0:P2}";
        private IQueryable<StudentMetricBenchmarkAssessment> suppliedStudentMetricBenchmarkAssessmentData;
        private const string suppliedChartTitle = "HistoricalChart";

        #endregion

        protected override void EstablishContext()
        {
            //Prepare supplied data collections
            suppliedStudentMetricBenchmarkAssessmentData = GetSuppliedStudentMetricBenchmarkAssessment();
            suppliedSchoolGoal = GetSuppliedMetricGoal();
            suppliedMetricState = GetSuppliedMetricState();

            //Set up the mocks
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            studentMetricBenchmarkAssessmentRepository = mocks.StrictMock<IRepository<StudentMetricBenchmarkAssessment>>();
            metricGoalProvider = mocks.StrictMock<IMetricGoalProvider>();
            metricStateProvider = mocks.StrictMock<IMetricStateProvider>();
            metricInstanceSetKeyResolver = mocks.StrictMock<IMetricInstanceSetKeyResolver<StudentSchoolMetricInstanceSetRequest>>();

            //Set expectations
            Expect.Call(metricNodeResolver.GetMetricNodeForStudentFromMetricVariantId(suppliedSchoolId, suppliedMetricVariantId)).Return(GetMetricMetadataNode());
            Expect.Call(studentMetricBenchmarkAssessmentRepository.GetAll()).Return(suppliedStudentMetricBenchmarkAssessmentData);
            Expect.Call(
                metricInstanceSetKeyResolver.GetMetricInstanceSetKey(null))
                .Constraints(
                    new ActionConstraint<StudentSchoolMetricInstanceSetRequest>(x =>
                    {
                        Assert.That(x.SchoolId == suppliedSchoolId);
                        Assert.That(x.MetricVariantId == suppliedMetricVariantId);
                        Assert.That(x.StudentUSI == suppliedStudentUSI);
                    })
                ).Return(suppliedMetricInstanceSetKey);
            Expect.Call(metricGoalProvider.GetMetricGoal(suppliedMetricInstanceSetKey, suppliedMetricId)).Return(suppliedSchoolGoal);
            Expect.Call(metricStateProvider.GetState(suppliedMetricId, suppliedMetricValueStr, "System.Double")).Repeat.Any().Return(suppliedMetricState);
            Expect.Call(metricStateProvider.GetState(suppliedMetricId, "", "System.Double")).Return(suppliedMetricState);
        }

        protected IQueryable<StudentMetricBenchmarkAssessment> GetSuppliedStudentMetricBenchmarkAssessment()
        {
            var list = new List<StudentMetricBenchmarkAssessment>
            {
                new StudentMetricBenchmarkAssessment { StudentUSI=suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, Value = suppliedMetricValueStr, TrendDirection = 1, AssessmentTitle = "Assessment Title 1", Date = new DateTime(2011, 6, 1), Version = 1, ValueType = "System.Double" },
                new StudentMetricBenchmarkAssessment { StudentUSI=suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, Value = String.Empty, TrendDirection = 0, AssessmentTitle = "Assessment Title 2", Date = new DateTime(2011, 5, 31), Version=2, ValueType = "System.Double" },
                new StudentMetricBenchmarkAssessment { StudentUSI=suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, Value = suppliedMetricValueStr, TrendDirection = -1, AssessmentTitle = "Assessment Title 3", Date = new DateTime(2011, 5, 31), Version = 3, ValueType = "System.Double" },
                new StudentMetricBenchmarkAssessment { StudentUSI=suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, Value = suppliedMetricValueStr, TrendDirection = null, AssessmentTitle = "Assessment Title 4", Date = new DateTime(2011, 5, 30), Version = 4, ValueType = "System.Double" },
            };
            return list.AsQueryable();
        }

        protected MetricMetadataNode GetMetricMetadataNode()
        {
            return new MetricMetadataNode(null)
            {
                MetricId = suppliedMetricId,
                MetricVariantId = suppliedMetricVariantId,
                Name = suppliedMetricName,
                DisplayName = suppliedMetricDisplayName,
                Format = suppliedMetricFormat,
                Actions = new List<MetricAction>
                        {
                            new MetricAction{ Title = "Bogus Title", DrilldownHeader = "Incorrect Chart Title"},
                            new MetricAction{ Title = suppliedChartTitle, DrilldownHeader = "Correct Chart Title"},
                        }
            };
        }

        protected Goal GetSuppliedMetricGoal()
        {
            return new Goal { Value = .53m, Interpretation = TrendInterpretation.Standard };
        }

        protected State GetSuppliedMetricState()
        {
            return new State(MetricStateType.Good, "Good");
        }

        protected override void ExecuteTest()
        {
            var service = new BenchmarkHistoricalChartService(studentMetricBenchmarkAssessmentRepository, metricGoalProvider, metricStateProvider, metricInstanceSetKeyResolver, metricNodeResolver);
            actualModel = service.Get(new BenchmarkHistoricalChartRequest()
                                          {
                                              StudentUSI = suppliedStudentUSI,
                                              SchoolId = suppliedSchoolId,
                                              MetricVariantId = suppliedMetricVariantId,
                                              Title = suppliedChartTitle
                                          });
        }

        [Test]
        public virtual void Should_bind_chart_data_title()
        {
            Assert.That(actualModel.ChartData.ChartTitle, Is.Not.Empty);
            var suppliedTitle = GetMetricMetadataNode().Actions.Single(x => x.Title == When_building_historical_benchmark_assessment.suppliedChartTitle).DrilldownHeader;
            Assert.That(actualModel.ChartData.ChartTitle, Is.EqualTo(suppliedTitle));
        }

        [Test]
        public void Should_return_model_that_is_not_null()
        {
            Assert.That(actualModel, Is.Not.Null);
        }

        [Test]
        public void Should_return_model_with_chart_data_chart_id_bound()
        {
            Assert.That(actualModel.ChartData.ChartId, Is.EqualTo(suppliedSchoolId + "_" + suppliedStudentUSI + "_" + suppliedMetricVariantId));
        }

        [Test]
        public void Should_return_model_with_a_strip_line()
        {
            Assert.That(actualModel.ChartData.StripLines.Count, Is.EqualTo(1));
            Assert.That(actualModel.ChartData.StripLines.Single().Value, Is.EqualTo(suppliedSchoolGoal.Value));
            Assert.That(actualModel.ChartData.StripLines.Single().IsNegativeThreshold, Is.EqualTo(suppliedSchoolGoal.Interpretation != TrendInterpretation.Standard));
            Assert.That(actualModel.ChartData.StripLines.Single().Tooltip, Is.EqualTo(string.Format("School Goal: {0:P2}", suppliedSchoolGoal.Value)));
        }

        [Test]
        public void Should_create_series_correctly()
        {
            Assert.That(actualModel.ChartData.SeriesCollection.Count(), Is.EqualTo(1));
            Assert.That(actualModel.ChartData.SeriesCollection[0].Name, Is.EqualTo(suppliedMetricDisplayName));
        }

        [Test]
        public void Should_create_points_correctly_bound_and_in_order()
        {
            var suppliedDataInExpectedOrder = suppliedStudentMetricBenchmarkAssessmentData
                .OrderBy(x => x.Date)
                .ThenBy(x => x.Version);

            Assert.That(actualModel.ChartData.SeriesCollection[0].Points.Count(), Is.EqualTo(suppliedDataInExpectedOrder.Count()));

            var i = 0;
            foreach (var suppliedBenchmarkAssessment in suppliedDataInExpectedOrder)
            {
                Assert.That(actualModel.ChartData.SeriesCollection[0].Points[i].Label, Is.EqualTo(suppliedBenchmarkAssessment.Date.ToShortDateString()));
                Assert.That(actualModel.ChartData.SeriesCollection[0].Points[i].Value, Is.EqualTo((string.IsNullOrEmpty(suppliedBenchmarkAssessment.Value)) ? 0 : Convert.ToDouble(suppliedBenchmarkAssessment.Value)));
                Assert.That(actualModel.ChartData.SeriesCollection[0].Points[i].Tooltip, Is.EqualTo(suppliedBenchmarkAssessment.AssessmentTitle));
                Assert.That(actualModel.ChartData.SeriesCollection[0].Points[i].State, Is.EqualTo(suppliedMetricState.StateType));
                i++;
            }
        }

        [Test]
        public void Should_create_points_with_trend_correctly()
        {
            var suppliedDataInExpectedOrder = suppliedStudentMetricBenchmarkAssessmentData
                .OrderBy(x => x.Date)
                .ThenBy(x => x.Version);

            Assert.That(actualModel.ChartData.SeriesCollection[0].Points.Count(), Is.EqualTo(suppliedDataInExpectedOrder.Count()));

            var i = 0;
            decimal? prevValue = null;
            foreach (var suppliedBenchmarkAssessment in suppliedDataInExpectedOrder)
            {
                decimal? suppliedValue = (string.IsNullOrEmpty(suppliedBenchmarkAssessment.Value))
                                             ? (decimal?)null
                                             : Convert.ToDecimal(suppliedBenchmarkAssessment.Value);

                Assert.That(actualModel.ChartData.SeriesCollection[0].Points[i].Trend, Is.EqualTo(GetTrend(prevValue, suppliedValue)));
                prevValue = suppliedValue;

                i++;
            }
        }

        private static TrendEvaluation GetTrend(decimal? previousValue, decimal? currentValue)
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

        [Test]
        public void Should_select_correct_benchmark_data_for_context()
        {
            Assert.That(actualModel.ChartData.SeriesCollection[0].Points.Count, Is.EqualTo(4));

            Assert.That(actualModel.ChartData.SeriesCollection[0].Points.SingleOrDefault(x => x.Tooltip == suppliedAssessmentTitle1), Is.Not.Null);
            Assert.That(actualModel.ChartData.SeriesCollection[0].Points.SingleOrDefault(x => x.Tooltip == suppliedAssessmentTitle2), Is.Not.Null);
            Assert.That(actualModel.ChartData.SeriesCollection[0].Points.SingleOrDefault(x => x.Tooltip == suppliedAssessmentTitle3), Is.Not.Null);
            Assert.That(actualModel.ChartData.SeriesCollection[0].Points.SingleOrDefault(x => x.Tooltip == suppliedAssessmentTitle4), Is.Not.Null);
        }


        [Test]
        public void Should_have_no_unassigned_values_on_presentation_model()
        {
            // ChartData.SeriesCollection[0].Points[1].Value is actually set to 0 b/c String.Empty are converted that way
            // actualModel.SeriesCollection[0].Points[3].Trend is actually set to Trend.None
            actualModel.EnsureNoDefaultValues("BenchmarkModel.ChartData.SeriesCollection[0].Points[0].Trend",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].ShowInLegend",
                                              "BenchmarkModel.ChartData.StripLines[0].IsNegativeThreshold",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[1].Trend",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[1].Value",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[3].Trend",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[2].Trend",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[0].Numerator",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[0].Denominator",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[1].Numerator",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[1].Denominator",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[2].Numerator",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[2].Denominator",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[3].Numerator",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[3].Denominator",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[0].IsValueShownAsLabel",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[1].IsValueShownAsLabel",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[2].IsValueShownAsLabel",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[3].IsValueShownAsLabel",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[0].AxisLabel",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[1].AxisLabel",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[2].AxisLabel",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[3].AxisLabel",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[0].State",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[1].State",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[2].State",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[3].State",
                                              "BenchmarkModel.ChartData.AxisXTitle",
                                              "BenchmarkModel.ChartData.AxisYTitle",
                                              "BenchmarkModel.ChartData.YAxisMaxValue",
                                              "BenchmarkModel.ChartData.StripLines",
                                              "BenchmarkModel.ChartData.StripLines[0].Label",
	                                          "BenchmarkModel.ChartData.SeriesCollection[0].Points[0].ValueAsText",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[0].TooltipHeader",
	                                          "BenchmarkModel.ChartData.SeriesCollection[0].Points[0].RatioLocation",
	                                          "BenchmarkModel.ChartData.SeriesCollection[0].Points[0].SubLabel",
	                                          "BenchmarkModel.ChartData.SeriesCollection[0].Points[1].ValueAsText",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[1].TooltipHeader",
	                                          "BenchmarkModel.ChartData.SeriesCollection[0].Points[1].RatioLocation",
	                                          "BenchmarkModel.ChartData.SeriesCollection[0].Points[1].SubLabel",
	                                          "BenchmarkModel.ChartData.SeriesCollection[0].Points[2].ValueAsText",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[2].TooltipHeader",
	                                          "BenchmarkModel.ChartData.SeriesCollection[0].Points[2].RatioLocation",
	                                          "BenchmarkModel.ChartData.SeriesCollection[0].Points[2].SubLabel",
	                                          "BenchmarkModel.ChartData.SeriesCollection[0].Points[3].ValueAsText",
                                              "BenchmarkModel.ChartData.SeriesCollection[0].Points[3].TooltipHeader",
	                                          "BenchmarkModel.ChartData.SeriesCollection[0].Points[3].RatioLocation",
	                                          "BenchmarkModel.ChartData.SeriesCollection[0].Points[3].SubLabel",
	                                          "BenchmarkModel.ChartData.DisplayType",
	                                          "BenchmarkModel.ChartData.Context",
	                                          "BenchmarkModel.ChartData.SubContext"
);

        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }
}
