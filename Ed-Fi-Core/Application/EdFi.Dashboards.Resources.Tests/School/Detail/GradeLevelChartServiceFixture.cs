// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.MetricInstanceSetKeyResolvers;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Models.Charting;
using EdFi.Dashboards.Resources.School.Detail;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;
using MetricStateType = EdFi.Dashboards.Metric.Resources.Models.MetricStateType;

namespace EdFi.Dashboards.Resources.Tests.School.Detail
{
    public abstract class GradeLevelChartServiceFixture : TestFixtureBase
    {
        private IRepository<SchoolMetricGradeDistribution> repository;
        private IMetricNodeResolver metricNodeResolver;
        private IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        private IMetricGoalProvider metricGoalProvider;

        private IQueryable<SchoolMetricGradeDistribution> suppliedData;
        protected Goal suppliedGoal;
        private const string suppliedGradeLevel1 = "9 grade level";
        private const string suppliedGradeLevel2 = "10 grade level";
        private const string suppliedGradeLevel3 = "11 grade level";
        private const string suppliedGradeLevel4 = "12 grade level";
        private const decimal suppliedNumerator1 = 21m;
        private const int suppliedDenomiator1 = 200;
        private const decimal suppliedNumerator2 = 200m;
        private const int suppliedDenomiator2 = 300;
        private const decimal suppliedNumerator3 = 41m;
        private const int suppliedDenomiator3 = 400;

        private const int suppliedSchoolId = 2000;
        private const int suppliedMetricId = 2;
        private const int suppliedMetricVariantId = 2999;
        private readonly Guid suppliedMetricInstanceSetKey = Guid.NewGuid();
        private readonly decimal? wrongRate = .11m;
        private const int wrongDenominator = 100;
        private const string suppliedMetricFormat = "{0:P2} test";
        private const string suppliedNumeratorDenominatorFormat = "{0} test test {1}";
        private const string suppliedChartTitle = "GradeLevelChart";

        protected ChartData actualModel;

        protected abstract void SetSuppliedGoal();

        protected override void EstablishContext()
        {
            SetSuppliedGoal();

            base.EstablishContext();

            suppliedData = GetData();

            repository = mocks.StrictMock<IRepository<SchoolMetricGradeDistribution>>();
            metricInstanceSetKeyResolver = mocks.StrictMock<IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest>>();
            metricGoalProvider = mocks.StrictMock<IMetricGoalProvider>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();

            Expect.Call(metricNodeResolver.GetMetricNodeForSchoolFromMetricVariantId(suppliedSchoolId, suppliedMetricVariantId)).Return(GetSuppliedMetricMetadataNode());
            Expect.Call(repository.GetAll()).Return(suppliedData);
            Expect.Call(metricInstanceSetKeyResolver.GetMetricInstanceSetKey(null))
                .Constraints(
                    new ActionConstraint<SchoolMetricInstanceSetRequest>(x =>
                    {
                        Assert.That(x.SchoolId == suppliedSchoolId);
                        Assert.That(x.MetricVariantId == suppliedMetricVariantId);
                    })
                ).Return(suppliedMetricInstanceSetKey);
            Expect.Call(metricGoalProvider.GetMetricGoal(suppliedMetricInstanceSetKey, suppliedMetricId)).Return(suppliedGoal);
        }

        protected IQueryable<SchoolMetricGradeDistribution> GetData()
        {
            var data = new List<SchoolMetricGradeDistribution>
                           {
                               new SchoolMetricGradeDistribution {SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, GradeLevelTypeId=2, GradeLevel = suppliedGradeLevel2, Numerator=suppliedNumerator2, Denominator=suppliedDenomiator2},
                               new SchoolMetricGradeDistribution {SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, GradeLevelTypeId=1, GradeLevel = suppliedGradeLevel1, Numerator=suppliedNumerator1, Denominator=suppliedDenomiator1},
                               new SchoolMetricGradeDistribution {SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, GradeLevelTypeId=4, GradeLevel = suppliedGradeLevel4, Numerator=null, Denominator=null},
                               new SchoolMetricGradeDistribution {SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, GradeLevelTypeId=3, GradeLevel = suppliedGradeLevel3, Numerator=suppliedNumerator3, Denominator=suppliedDenomiator3},
                               new SchoolMetricGradeDistribution {SchoolId = suppliedSchoolId + 1, MetricId = suppliedMetricId, GradeLevelTypeId=1, Numerator=wrongRate, Denominator=wrongDenominator},
                               new SchoolMetricGradeDistribution {SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 1, GradeLevelTypeId=1, Numerator=wrongRate, Denominator=wrongDenominator}
                           };
            return data.AsQueryable();
        }

        public MetricMetadataNode GetSuppliedMetricMetadataNode()
        {
            return new MetricMetadataNode(null)
                {
                    MetricId = suppliedMetricId, 
                    MetricVariantId = suppliedMetricVariantId, 
                    Format = suppliedMetricFormat, 
                    NumeratorDenominatorFormat = suppliedNumeratorDenominatorFormat,
                    Actions = new List<MetricAction>
                        {
                            new MetricAction{ Title = "Bogus Title", DrilldownHeader = "Incorrect Chart Title"},
                            new MetricAction{ Title = suppliedChartTitle, DrilldownHeader = "Correct Chart Title"},
                        }
                };
        }

        protected override void ExecuteTest()
        {
            var service = new GradeLevelChartService(repository, metricInstanceSetKeyResolver, metricGoalProvider, metricNodeResolver);
            actualModel = service.Get(new GradeLevelChartRequest()
                                          {
                                              SchoolId = suppliedSchoolId,
                                              MetricVariantId = suppliedMetricVariantId,
                                              Title = suppliedChartTitle
                                          });
        }

        [Test]
        public void Should_create_correct_number_of_series()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.SeriesCollection.Count, Is.EqualTo(1));
            Assert.That(actualModel.SeriesCollection[0].Name, Is.EqualTo("Actual"));
			Assert.That(actualModel.SeriesCollection[0].Style.BackgroundColor, Is.EqualTo(ChartColors.Blue));
        }

        [Test]
        public void Should_populate_points_correctly()
        {
            var value1 = Math.Truncate(Convert.ToDouble(suppliedNumerator1) / Convert.ToDouble(suppliedDenomiator1) * 1000) / 1000;
            var value2 = Math.Truncate(Convert.ToDouble(suppliedNumerator2) / Convert.ToDouble(suppliedDenomiator2) * 1000) / 1000;
            var value3 = Math.Truncate(Convert.ToDouble(suppliedNumerator3) / Convert.ToDouble(suppliedDenomiator3) * 1000) / 1000;

            Assert.That(actualModel.SeriesCollection[0].Points.Count, Is.EqualTo(4));

            Assert.That(actualModel.SeriesCollection[0].Points[0].Label, Is.EqualTo(String.Format(suppliedMetricFormat, value1)));
            Assert.That(actualModel.SeriesCollection[0].Points[1].Label, Is.EqualTo(String.Format(suppliedMetricFormat, value2)));
            Assert.That(actualModel.SeriesCollection[0].Points[2].Label, Is.EqualTo(String.Format(suppliedMetricFormat, value3)));
            Assert.That(actualModel.SeriesCollection[0].Points[3].Label, Is.EqualTo(String.Format(suppliedMetricFormat, 0)));

            Assert.That(actualModel.SeriesCollection[0].Points[0].AxisLabel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(actualModel.SeriesCollection[0].Points[1].AxisLabel, Is.EqualTo(suppliedGradeLevel2));
            Assert.That(actualModel.SeriesCollection[0].Points[2].AxisLabel, Is.EqualTo(suppliedGradeLevel3));
            Assert.That(actualModel.SeriesCollection[0].Points[3].AxisLabel, Is.EqualTo(suppliedGradeLevel4));

            Assert.That(actualModel.SeriesCollection[0].Points[0].Value, Is.EqualTo(value1));
            Assert.That(actualModel.SeriesCollection[0].Points[1].Value, Is.EqualTo(value2));
            Assert.That(actualModel.SeriesCollection[0].Points[2].Value, Is.EqualTo(value3));
            Assert.That(actualModel.SeriesCollection[0].Points[3].Value, Is.EqualTo(0));

            Assert.That(actualModel.SeriesCollection[0].Points[0].Tooltip, Is.EqualTo(String.Format(suppliedNumeratorDenominatorFormat, suppliedNumerator1, suppliedDenomiator1)));
            Assert.That(actualModel.SeriesCollection[0].Points[1].Tooltip, Is.EqualTo(String.Format(suppliedNumeratorDenominatorFormat, suppliedNumerator2, suppliedDenomiator2)));
            Assert.That(actualModel.SeriesCollection[0].Points[2].Tooltip, Is.EqualTo(String.Format(suppliedNumeratorDenominatorFormat, suppliedNumerator3, suppliedDenomiator3)));
            Assert.That(actualModel.SeriesCollection[0].Points[3].Tooltip, Is.EqualTo(String.Empty));

            Assert.That(actualModel.SeriesCollection[0].Points[0].IsValueShownAsLabel, Is.True);
            Assert.That(actualModel.SeriesCollection[0].Points[1].IsValueShownAsLabel, Is.True);
            Assert.That(actualModel.SeriesCollection[0].Points[2].IsValueShownAsLabel, Is.True);
            Assert.That(actualModel.SeriesCollection[0].Points[3].IsValueShownAsLabel, Is.True);

            Assert.That(actualModel.SeriesCollection[0].Points[0].Trend, Is.EqualTo(TrendEvaluation.None));
        }

        [Test]
        public void Should_sort_grades()
        {
            Assert.That(actualModel.SeriesCollection[0].Points[0].AxisLabel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(actualModel.SeriesCollection[0].Points[1].AxisLabel, Is.EqualTo(suppliedGradeLevel2));
            Assert.That(actualModel.SeriesCollection[0].Points[2].AxisLabel, Is.EqualTo(suppliedGradeLevel3));
            Assert.That(actualModel.SeriesCollection[0].Points[3].AxisLabel, Is.EqualTo(suppliedGradeLevel4));
        }

        [Test]
        public void Should_create_goal_strip_line()
        {
            Assert.That(actualModel.StripLines.Count, Is.EqualTo(1));
            Assert.That(actualModel.StripLines[0].Value, Is.EqualTo(suppliedGoal.Value));
            Assert.That(actualModel.StripLines[0].Tooltip, Is.EqualTo("School Goal: " + String.Format(suppliedMetricFormat, suppliedGoal.Value)));
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }

        [Test]
        public virtual void Should_bind_chart_data_title()
        {
            Assert.That(actualModel.ChartTitle, Is.Not.Empty);
            var suppliedTitle = GetSuppliedMetricMetadataNode().Actions.Single(x => x.Title == GradeLevelChartServiceFixture.suppliedChartTitle).DrilldownHeader;
            Assert.That(actualModel.ChartTitle, Is.EqualTo(suppliedTitle));
        }


    }

    [TestFixture]
    public class When_building_the_grade_level_chart_for_school_with_standard_goal_interpretation : GradeLevelChartServiceFixture
    {
        protected override void SetSuppliedGoal()
        {
            suppliedGoal = new Goal { Interpretation = TrendInterpretation.Standard, Value = .50m };
        }

        [Test]
        public void Should_set_goal_interpretation()
        {
            Assert.That(actualModel.StripLines[0].IsNegativeThreshold, Is.False);
        }

        [Test]
        public void Should_set_point_state()
        {
            Assert.That(actualModel.SeriesCollection[0].Points[0].State, Is.EqualTo(MetricStateType.Low));
            Assert.That(actualModel.SeriesCollection[0].Points[1].State, Is.EqualTo(MetricStateType.Good));
            Assert.That(actualModel.SeriesCollection[0].Points[2].State, Is.EqualTo(MetricStateType.Low));
            Assert.That(actualModel.SeriesCollection[0].Points[3].State, Is.EqualTo(MetricStateType.Low));
        }
    }

    [TestFixture]
    public class When_building_the_grade_level_chart_for_school_with_inverse_goal_interpretation : GradeLevelChartServiceFixture
    {
        protected override void SetSuppliedGoal()
        {
            suppliedGoal = new Goal { Interpretation = TrendInterpretation.Inverse, Value = .50m };
        }

        [Test]
        public void Should_set_goal_interpretation()
        {
            Assert.That(actualModel.StripLines[0].IsNegativeThreshold, Is.True);
        }

        [Test]
        public void Should_set_point_state()
        {
            Assert.That(actualModel.SeriesCollection[0].Points[0].State, Is.EqualTo(MetricStateType.Good));
            Assert.That(actualModel.SeriesCollection[0].Points[1].State, Is.EqualTo(MetricStateType.Low));
            Assert.That(actualModel.SeriesCollection[0].Points[2].State, Is.EqualTo(MetricStateType.Good));
            Assert.That(actualModel.SeriesCollection[0].Points[3].State, Is.EqualTo(MetricStateType.Good));
        }
    }
}
