using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Models.Student.Detail;
using EdFi.Dashboards.Resources.StudentSchool.Detail;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using EdFi.Dashboards.Metric.Resources.Services;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Student.Detail
{
    [TestFixture]
    public class When_building_assessment_historical_model : TestFixtureBase
    {
        #region Fields
        //The Injected Dependencies.
        private IRepository<StudentMetricAssessmentHistorical> _studentMetricAssessmentHistorical;
        private IRepository<StudentMetricAssessmentHistoricalMetaData> _studentMetricAssessmentHistoricalMetaData;
        private IMetricGoalProvider _metricGoalProvider;
        private IMetricInstanceSetKeyResolver<StudentSchoolMetricInstanceSetRequest> _metricInstanceSetKeyResolver;
        private IMetricNodeResolver _metricNodeResolver;

        //The Actual Model.
        private StudentMetricAssessmentHistoricalModel _actualModel;

        //The supplied Data models.
        private const int SuppliedStudentUsi = 1;
        private const int SuppliedSchoolId = 2;
        private const int SuppliedMetricId = 3;
        private const int SuppliedMetricVariantId = 4;
        private readonly Guid _suppliedMetricInstanceSetKey = Guid.NewGuid();
        private Goal _suppliedSchoolGoal;
        private const string SuppliedMetricValueStr1 = "84.3";
        private const string SuppliedMetricValueStr2 = "90.2";
        private const string SuppliedMetricValueStr3 = "80.6";
        private const string SuppliedMetricValueStr4 = "80.6";
        private const string SuppliedContext1 = "Context1";
        private const string SuppliedContext2 = "Context2";
        private const string SuppliedContext3 = "Context3";
        private const string SuppliedContext4 = "Context4";
        private const string SuppliedSubContext1 = "SubContext1";
        private const string SuppliedSubContext2 = "SubContext2";
        private const string SuppliedSubContext3 = "SubContext3";
        private const string SuppliedSubContext4 = "SubContext4";
        private const string SuppliedToolTipContext1 = "ToolTipContext1";
        private const string SuppliedToolTipContext2 = "ToolTipContext2";
        private const string SuppliedToolTipContext3 = "ToolTipContext3";
        private const string SuppliedToolTipContext4 = "ToolTipContext4";
        private const string SuppliedToolTipSubContext1 = "ToolTipSubContext1";
        private const string SuppliedToolTipSubContext2 = "ToolTipSubContext2";
        private const string SuppliedToolTipSubContext3 = "ToolTipSubContext3";
        private const string SuppliedToolTipSubContext4 = "ToolTipSubContext4";
        private const int SuppliedMetricState1 = 1;
        private const int SuppliedMetricState2 = 1;
        private const int SuppliedMetricState3 = 3;
        private const int SuppliedMetricState4 = 3;
        private const string SuppliedValueTypeName = "System.Double";
        private const string SuppliedContext = "Context";
        private const string SuppliedSubContext = "SubContext";
        private const string SuppliedDisplayType = "LineGraph";
        private const string SuppliedLabelType = "Percent";
        private const string SuppliedMetricName = "Metric Name";
        private const string SuppliedMetricDisplayName = "Metric Display Name";
        private const string SuppliedMetricFormat = "{0:0.0}";
        private IQueryable<StudentMetricAssessmentHistorical> _suppliedStudentMetricAssessmentHistorical;
        private IQueryable<StudentMetricAssessmentHistoricalMetaData> _suppliedStudentMetricAssessmentHistoricalMetaData;

        #endregion

        protected override void EstablishContext()
        {
            //Prepare supplied data collections
            _suppliedStudentMetricAssessmentHistorical = GetSuppliedStudentMetricAssessmentHistorical();
            _suppliedStudentMetricAssessmentHistoricalMetaData = GetSuppliedStudentMetricAssessmentHistoricalMetaData();
            _suppliedSchoolGoal = GetSuppliedMetricGoal();

            //Set up the mocks
            _metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            _studentMetricAssessmentHistorical = mocks.StrictMock<IRepository<StudentMetricAssessmentHistorical>>();
            _studentMetricAssessmentHistoricalMetaData = mocks.StrictMock<IRepository<StudentMetricAssessmentHistoricalMetaData>>();
            _metricGoalProvider = mocks.StrictMock<IMetricGoalProvider>();
            _metricInstanceSetKeyResolver = mocks.StrictMock<IMetricInstanceSetKeyResolver<StudentSchoolMetricInstanceSetRequest>>();

            //Set expectations
            Expect.Call(_metricNodeResolver.GetMetricNodeForStudentFromMetricVariantId(SuppliedSchoolId, SuppliedMetricVariantId)).Return(GetMetricMetadataNode());
            Expect.Call(_studentMetricAssessmentHistorical.GetAll()).Return(_suppliedStudentMetricAssessmentHistorical);
            Expect.Call(_studentMetricAssessmentHistoricalMetaData.GetAll()).Return(_suppliedStudentMetricAssessmentHistoricalMetaData);
            Expect.Call(
                _metricInstanceSetKeyResolver.GetMetricInstanceSetKey(null))
                .Constraints(
                    new ActionConstraint<StudentSchoolMetricInstanceSetRequest>(x =>
                    {
                        Assert.That(x.SchoolId == SuppliedSchoolId);
                        Assert.That(x.MetricVariantId == SuppliedMetricVariantId);
                        Assert.That(x.StudentUSI == SuppliedStudentUsi);
                    })
                ).Return(_suppliedMetricInstanceSetKey);
            Expect.Call(_metricGoalProvider.GetMetricGoal(_suppliedMetricInstanceSetKey, SuppliedMetricId)).Return(_suppliedSchoolGoal);
        }

        private IQueryable<StudentMetricAssessmentHistoricalMetaData> GetSuppliedStudentMetricAssessmentHistoricalMetaData()
        {
            var list = new List<StudentMetricAssessmentHistoricalMetaData>
            {
                new StudentMetricAssessmentHistoricalMetaData
                    {
                        StudentUSI=SuppliedStudentUsi,
                        MetricId = SuppliedMetricId,
                        Context = SuppliedContext,
                        SubContext = SuppliedSubContext,
                        DisplayType = SuppliedDisplayType,
                        LabelType = SuppliedLabelType
                    },
            };
            return list.AsQueryable();
        }

        private IQueryable<StudentMetricAssessmentHistorical> GetSuppliedStudentMetricAssessmentHistorical()
        {
            var list = new List<StudentMetricAssessmentHistorical>
            {
                new StudentMetricAssessmentHistorical { StudentUSI=SuppliedStudentUsi, MetricId = SuppliedMetricId, Value = SuppliedMetricValueStr1, Context = SuppliedContext1, SubContext = SuppliedSubContext1, DisplayOrder = 0, MetricStateTypeId = SuppliedMetricState1, PerformanceLevelRatio = .75m, ToolTipContext = SuppliedToolTipContext1, ToolTipSubContext = SuppliedToolTipSubContext1, ValueTypeName = SuppliedValueTypeName },
                new StudentMetricAssessmentHistorical { StudentUSI=SuppliedStudentUsi, MetricId = SuppliedMetricId, Value = SuppliedMetricValueStr2, Context = SuppliedContext2, SubContext = SuppliedSubContext2, DisplayOrder = 1, MetricStateTypeId = SuppliedMetricState2, PerformanceLevelRatio = .75m, ToolTipContext = SuppliedToolTipContext2, ToolTipSubContext = SuppliedToolTipSubContext2, ValueTypeName = SuppliedValueTypeName },
                new StudentMetricAssessmentHistorical { StudentUSI=SuppliedStudentUsi, MetricId = SuppliedMetricId, Value = SuppliedMetricValueStr3, Context = SuppliedContext3, SubContext = SuppliedSubContext3, DisplayOrder = 2, MetricStateTypeId = SuppliedMetricState3, PerformanceLevelRatio = .75m, ToolTipContext = SuppliedToolTipContext3, ToolTipSubContext = SuppliedToolTipSubContext3, ValueTypeName = SuppliedValueTypeName },
                new StudentMetricAssessmentHistorical { StudentUSI=SuppliedStudentUsi, MetricId = SuppliedMetricId, Value = SuppliedMetricValueStr4, Context = SuppliedContext4, SubContext = SuppliedSubContext4, DisplayOrder = 3, MetricStateTypeId = SuppliedMetricState4, PerformanceLevelRatio = .75m, ToolTipContext = SuppliedToolTipContext4, ToolTipSubContext = SuppliedToolTipSubContext4, ValueTypeName = SuppliedValueTypeName },
            };
            return list.AsQueryable();
        }

        protected MetricMetadataNode GetMetricMetadataNode()
        {
            return new MetricMetadataNode(null) { MetricId = SuppliedMetricId, MetricVariantId = SuppliedMetricVariantId, Name = SuppliedMetricName, DisplayName = SuppliedMetricDisplayName, Format = SuppliedMetricFormat };
        }

        protected Goal GetSuppliedMetricGoal()
        {
            return new Goal { Value = .53m, Interpretation = TrendInterpretation.Standard };
        }

        protected override void ExecuteTest()
        {
            var service = new StudentMetricAssessmentHistoricalService(_studentMetricAssessmentHistorical, _studentMetricAssessmentHistoricalMetaData, _metricGoalProvider, _metricInstanceSetKeyResolver, _metricNodeResolver);
            _actualModel = service.Get(new StudentMetricAssessmentHistoricalRequest
            {
                StudentUSI = SuppliedStudentUsi,
                SchoolId = SuppliedSchoolId,
                MetricVariantId = SuppliedMetricVariantId
            });
        }

        [Test]
        public void Should_return_model_that_is_not_null()
        {
            Assert.That(_actualModel, Is.Not.Null);
        }

        [Test]
        public void Should_return_model_with_chart_data_chart_id_bound()
        {
            Assert.That(_actualModel.ChartData.ChartId, Is.EqualTo(SuppliedSchoolId + "_" + SuppliedStudentUsi + "_" + SuppliedMetricVariantId));
        }

        [Test]
        public void Should_return_model_with_chart_data_from_metadata()
        {
            Assert.That(_actualModel.ChartData.Context, Is.EqualTo(SuppliedContext));
            Assert.That(_actualModel.ChartData.SubContext, Is.EqualTo(SuppliedSubContext));
            Assert.That(_actualModel.ChartData.DisplayType, Is.EqualTo(SuppliedDisplayType));
        }

        [Test]
        public void Should_return_model_with_a_strip_line()
        {
            Assert.That(_actualModel.ChartData.StripLines.Count, Is.EqualTo(1));
            Assert.That(_actualModel.ChartData.StripLines.Single().Value, Is.EqualTo(_suppliedSchoolGoal.Value));
            Assert.That(_actualModel.ChartData.StripLines.Single().IsNegativeThreshold, Is.EqualTo(_suppliedSchoolGoal.Interpretation != TrendInterpretation.Standard));
            Assert.That(_actualModel.ChartData.StripLines.Single().Tooltip, Is.Null);
        }

        [Test]
        public void Should_create_series_correctly()
        {
            Assert.That(_actualModel.ChartData.SeriesCollection.Count(), Is.EqualTo(1));
            Assert.That(_actualModel.ChartData.SeriesCollection[0].Name, Is.EqualTo(SuppliedMetricDisplayName));
        }

        [Test]
        public void Should_create_points_correctly_bound_and_in_order()
        {
            var suppliedDataInExpectedOrder = _suppliedStudentMetricAssessmentHistorical
                .OrderBy(x => x.DisplayOrder);

            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points.Count(), Is.EqualTo(suppliedDataInExpectedOrder.Count()));

            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[0].Label, Is.EqualTo(SuppliedContext1));
            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[0].SubLabel, Is.EqualTo(SuppliedSubContext1));
            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[0].TooltipHeader, Is.EqualTo(SuppliedToolTipContext1));
            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[0].Tooltip, Is.EqualTo(SuppliedToolTipSubContext1));
            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[0].ValueAsText, Is.EqualTo(suppliedDataInExpectedOrder.Skip(0).First().Value));
            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[0].State, Is.EqualTo(MetricStateType.Good));
            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[0].Trend, Is.EqualTo(TrendEvaluation.None));

            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[1].Label, Is.EqualTo(SuppliedContext2));
            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[1].SubLabel, Is.EqualTo(SuppliedSubContext2));
            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[1].TooltipHeader, Is.EqualTo(SuppliedToolTipContext2));
            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[1].Tooltip, Is.EqualTo(SuppliedToolTipSubContext2));
            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[1].ValueAsText, Is.EqualTo(suppliedDataInExpectedOrder.Skip(1).First().Value));
            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[1].State, Is.EqualTo(MetricStateType.Good));
            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[1].Trend, Is.EqualTo(TrendEvaluation.UpNoOpinion));

            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[2].Label, Is.EqualTo(SuppliedContext3));
            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[2].SubLabel, Is.EqualTo(SuppliedSubContext3));
            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[2].TooltipHeader, Is.EqualTo(SuppliedToolTipContext3));
            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[2].Tooltip, Is.EqualTo(SuppliedToolTipSubContext3));
            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[2].ValueAsText, Is.EqualTo(suppliedDataInExpectedOrder.Skip(2).First().Value));
            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[2].State, Is.EqualTo(MetricStateType.Low));
            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[2].Trend, Is.EqualTo(TrendEvaluation.DownNoOpinion));

            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[3].Label, Is.EqualTo(SuppliedContext4));
            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[3].SubLabel, Is.EqualTo(SuppliedSubContext4));
            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[3].TooltipHeader, Is.EqualTo(SuppliedToolTipContext4));
            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[3].Tooltip, Is.EqualTo(SuppliedToolTipSubContext4));
            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[3].ValueAsText, Is.EqualTo(suppliedDataInExpectedOrder.Skip(3).First().Value));
            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[3].State, Is.EqualTo(MetricStateType.Low));
            Assert.That(_actualModel.ChartData.SeriesCollection[0].Points[3].Trend, Is.EqualTo(TrendEvaluation.NoChangeNoOpinion));
        }

        [Test]
        public void Should_have_no_unassigned_values_on_presentation_model()
        {
            // ChartData.SeriesCollection[0].Points[1].Value is actually set to 0 b/c String.Empty are converted that way
            // actualModel.SeriesCollection[0].Points[3].Trend is actually set to Trend.None
            _actualModel.EnsureNoDefaultValues(
                //We're not using axis titles, or goals in StudentMetricAssessmentHistorical
                "StudentMetricAssessmentHistoricalModel.ChartData.AxisXTitle",
                "StudentMetricAssessmentHistoricalModel.ChartData.AxisYTitle",
                "StudentMetricAssessmentHistoricalModel.ChartData.Goal",
                "StudentMetricAssessmentHistoricalModel.ChartData.SeriesCollection[0].ShowInLegend",
                "StudentMetricAssessmentHistoricalModel.ChartData.SeriesCollection[0].Points[0].AxisLabel",
                "StudentMetricAssessmentHistoricalModel.ChartData.SeriesCollection[0].Points[0].Numerator",
                "StudentMetricAssessmentHistoricalModel.ChartData.SeriesCollection[0].Points[0].Denominator",
                "StudentMetricAssessmentHistoricalModel.ChartData.SeriesCollection[0].Points[0].IsValueShownAsLabel",
                "StudentMetricAssessmentHistoricalModel.ChartData.SeriesCollection[0].Points[0].Trend",
                "StudentMetricAssessmentHistoricalModel.ChartData.SeriesCollection[0].Points[1].AxisLabel",
                "StudentMetricAssessmentHistoricalModel.ChartData.SeriesCollection[0].Points[1].Numerator",
                "StudentMetricAssessmentHistoricalModel.ChartData.SeriesCollection[0].Points[1].Denominator",
                "StudentMetricAssessmentHistoricalModel.ChartData.SeriesCollection[0].Points[1].IsValueShownAsLabel",
                "StudentMetricAssessmentHistoricalModel.ChartData.SeriesCollection[0].Points[2].AxisLabel",
                "StudentMetricAssessmentHistoricalModel.ChartData.SeriesCollection[0].Points[2].Numerator",
                "StudentMetricAssessmentHistoricalModel.ChartData.SeriesCollection[0].Points[2].Denominator",
                "StudentMetricAssessmentHistoricalModel.ChartData.SeriesCollection[0].Points[2].IsValueShownAsLabel",
                "StudentMetricAssessmentHistoricalModel.ChartData.SeriesCollection[0].Points[3].AxisLabel",
                "StudentMetricAssessmentHistoricalModel.ChartData.SeriesCollection[0].Points[3].Numerator",
                "StudentMetricAssessmentHistoricalModel.ChartData.SeriesCollection[0].Points[3].Denominator",
                "StudentMetricAssessmentHistoricalModel.ChartData.SeriesCollection[0].Points[3].IsValueShownAsLabel",
                //We don't use tooltip, so it should be null
                "StudentMetricAssessmentHistoricalModel.ChartData.StripLines[0].Tooltip",
                //We don't have negative threshold, so it sohuld be false
                "StudentMetricAssessmentHistoricalModel.ChartData.StripLines[0].IsNegativeThreshold",
                "StudentMetricAssessmentHistoricalModel.ChartData.StripLines[0].Label",
                //The first position is zero, which is the default.
                "StudentMetricAssessmentHistoricalModel.ChartData.YAxisLabels[0].Position",
                //We're using Percent YAxisLabels, which don't use MinPosition & MaxPosition
                "StudentMetricAssessmentHistoricalModel.ChartData.YAxisLabels[0].MinPosition",
                "StudentMetricAssessmentHistoricalModel.ChartData.YAxisLabels[0].MaxPosition",
                "StudentMetricAssessmentHistoricalModel.ChartData.YAxisLabels[1].MinPosition",
                "StudentMetricAssessmentHistoricalModel.ChartData.YAxisLabels[1].MaxPosition",
                "StudentMetricAssessmentHistoricalModel.ChartData.YAxisLabels[2].MinPosition",
                "StudentMetricAssessmentHistoricalModel.ChartData.YAxisLabels[2].MaxPosition",
                "StudentMetricAssessmentHistoricalModel.ChartData.YAxisLabels[3].MinPosition",
                "StudentMetricAssessmentHistoricalModel.ChartData.YAxisLabels[3].MaxPosition",
                "StudentMetricAssessmentHistoricalModel.ChartData.YAxisLabels[4].MinPosition",
                "StudentMetricAssessmentHistoricalModel.ChartData.YAxisLabels[4].MaxPosition",
                //We're wanting OverviewChartIsEnabled to be false.
                "StudentMetricAssessmentHistoricalModel.ChartData.OverviewChartIsEnabled",
                "StudentMetricAssessmentHistoricalModel.ChartData.ShowMouseOverToolTipOnLeft",
                "StudentMetricAssessmentHistoricalModel.ChartData.ChartTitle"
            );

        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            _actualModel.EnsureSerializableModel();
        }
    }
}
