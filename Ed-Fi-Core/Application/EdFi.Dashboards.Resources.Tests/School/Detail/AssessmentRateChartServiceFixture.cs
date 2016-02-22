// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.MetricInstanceSetKeyResolvers;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.School.Detail;
using NUnit.Framework;
using Rhino.Mocks;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.Charting;
using EdFi.Dashboards.Testing;
using MetricStateType = EdFi.Dashboards.Metric.Resources.Models.MetricStateType;

namespace EdFi.Dashboards.Resources.Tests.School.Detail
{
    public class When_loading_assessment_rate_chart : TestFixtureBase
    {
        private IRepository<SchoolMetricAssessmentRate> repository;
        private IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        private IRepository<MetricInstanceFootnote> footnoteRepository;
        private IMetricNodeResolver metricNodeResolver;

        private IQueryable<SchoolMetricAssessmentRate> suppliedData;
        private IQueryable<MetricInstanceFootnote> suppliedFootnoteList;

        private const int suppliedSchoolId = 2000;
        private const int suppliedMetricId = 2;
        private const int suppliedMetricVariantId = 3;
        private readonly Guid suppliedMetricInstanceSetKey = Guid.NewGuid();
        private readonly decimal? suppliedBelowRate1 = .21m;
        private readonly decimal? suppliedMetStandardRate1 = .31m;
        private readonly decimal? suppliedCommendedRate1 = .41m;
        private readonly decimal? suppliedBelowRate2 = .22m;
        private readonly decimal? suppliedMetStandardRate2 = .32m;
        private readonly decimal? suppliedCommendedRate2 = .42m;
        private readonly decimal? suppliedBelowRate3 = 0m;
        private readonly decimal? suppliedMetStandardRate3 = .34m;
        private readonly decimal? suppliedCommendedRate3 = .44m;
        private readonly decimal? wrongRate = .11m;
        private const string suppliedGradeLevel1 = "9 grade level";
        private const string suppliedGradeLevel2 = "10 grade level";
        private const string suppliedGradeLevel3 = "11 grade level";
        private const string suppliedGradeLevel4 = "12 grade level";
        private const string suppliedFootnoteText1 = "supplied footnote 1";
        private const string suppliedFootnoteText2 = "supplied footnote 2";
        private const string suppliedFootnoteText3 = "supplied footnote 3";
        private const string suppliedMetricFormat = "{0:P2} test";

        private AssessmentRateChartModel actualModel;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            suppliedData = GetData();
            suppliedFootnoteList = GetSuppliedFootnoteList();

            repository = mocks.StrictMock<IRepository<SchoolMetricAssessmentRate>>();
            footnoteRepository = mocks.StrictMock<IRepository<MetricInstanceFootnote>>();
            metricInstanceSetKeyResolver = mocks.StrictMock<IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest>>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();

            Expect.Call(metricNodeResolver.GetMetricNodeForSchoolFromMetricVariantId(suppliedSchoolId, suppliedMetricVariantId)).Return(GetSuppliedMetricMetadata());
            Expect.Call(repository.GetAll()).Return(suppliedData);
            Expect.Call(footnoteRepository.GetAll()).Repeat.Any().Return(suppliedFootnoteList);
            Expect.Call(metricInstanceSetKeyResolver.GetMetricInstanceSetKey(null))
                .Constraints(
                    new ActionConstraint<SchoolMetricInstanceSetRequest>(x =>
                    {
                        Assert.That(x.SchoolId == suppliedSchoolId);
                        Assert.That(x.MetricVariantId == suppliedMetricVariantId);
                    })
                ).Return(suppliedMetricInstanceSetKey);
        }

        protected IQueryable<SchoolMetricAssessmentRate> GetData()
        {
            var data = new List<SchoolMetricAssessmentRate>
                           {
                               new SchoolMetricAssessmentRate {SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, GradeLevelTypeId=2, GradeLevel = suppliedGradeLevel2, BelowRate=suppliedBelowRate2, MetStandardRate=suppliedMetStandardRate2, CommendedRate=suppliedCommendedRate2},
                               new SchoolMetricAssessmentRate {SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, GradeLevelTypeId=1, GradeLevel = suppliedGradeLevel1, BelowRate=suppliedBelowRate1, MetStandardRate=suppliedMetStandardRate1, CommendedRate=suppliedCommendedRate1},
                               new SchoolMetricAssessmentRate {SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, GradeLevelTypeId=4, GradeLevel = suppliedGradeLevel4, BelowRate=null, CommendedRate=null, MetStandardRate=null},
                               new SchoolMetricAssessmentRate {SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, GradeLevelTypeId=3, GradeLevel = suppliedGradeLevel3, BelowRate=suppliedBelowRate3, MetStandardRate=suppliedMetStandardRate3, CommendedRate=suppliedCommendedRate3},
                               new SchoolMetricAssessmentRate {SchoolId = suppliedSchoolId + 1, MetricId = suppliedMetricId, GradeLevelTypeId=1, BelowRate=wrongRate, CommendedRate=wrongRate, MetStandardRate=wrongRate},
                               new SchoolMetricAssessmentRate {SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 1, GradeLevelTypeId=1, BelowRate=wrongRate, CommendedRate=wrongRate, MetStandardRate=wrongRate}
                           };
            return data.AsQueryable();
        }

        protected IQueryable<MetricInstanceFootnote> GetSuppliedFootnoteList()
        {
            var list = new List<MetricInstanceFootnote>
                           {
                               new MetricInstanceFootnote { MetricId = suppliedMetricId + 1, MetricInstanceSetKey = suppliedMetricInstanceSetKey, FootnoteTypeId = 1, FootnoteText = "wrong data"},
                               new MetricInstanceFootnote { MetricId = suppliedMetricId, MetricInstanceSetKey = Guid.NewGuid(), FootnoteText = "wrong data"},
                               new MetricInstanceFootnote { MetricId = suppliedMetricId, MetricInstanceSetKey = suppliedMetricInstanceSetKey, FootnoteTypeId = 2, FootnoteText = suppliedFootnoteText3 },
                               new MetricInstanceFootnote { MetricId = suppliedMetricId, MetricInstanceSetKey = suppliedMetricInstanceSetKey, FootnoteTypeId = 1, FootnoteText = suppliedFootnoteText1 },
                               new MetricInstanceFootnote { MetricId = suppliedMetricId, MetricInstanceSetKey = suppliedMetricInstanceSetKey, FootnoteTypeId = 2, FootnoteText = suppliedFootnoteText2 },
                           };
            return list.AsQueryable();
        }

        public MetricMetadataNode GetSuppliedMetricMetadata()
        {
            return new MetricMetadataNode(null) { MetricId = suppliedMetricId, MetricVariantId = suppliedMetricVariantId, Format = suppliedMetricFormat };
        }

        protected override void ExecuteTest()
        {
            var service = new AssessmentRateChartService(repository, footnoteRepository, metricInstanceSetKeyResolver, metricNodeResolver);
            actualModel = service.Get(new AssessmentRateChartRequest()
                                          {
                                              SchoolId = suppliedSchoolId,
                                              MetricVariantId = suppliedMetricVariantId
                                          });
        }

        [Test]
        public void Should_create_correct_number_of_series()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.ChartData.SeriesCollection.Count, Is.EqualTo(3));
            Assert.That(actualModel.ChartData.SeriesCollection[0].Name, Is.EqualTo("Below"));
			Assert.That(actualModel.ChartData.SeriesCollection[0].Style.BackgroundColor, Is.EqualTo(ChartColors.Red));
            Assert.That(actualModel.ChartData.SeriesCollection[1].Name, Is.EqualTo("Met Standard"));
			Assert.That(actualModel.ChartData.SeriesCollection[1].Style.BackgroundColor, Is.EqualTo(ChartColors.Green));
            Assert.That(actualModel.ChartData.SeriesCollection[2].Name, Is.EqualTo("Commended"));
			Assert.That(actualModel.ChartData.SeriesCollection[2].Style.BackgroundColor, Is.EqualTo(ChartColors.Blue));
            Assert.That(actualModel.ChartData.StripLines.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_populate_points_correctly()
        {
            Assert.That(actualModel.ChartData.SeriesCollection[0].Points.Count, Is.EqualTo(4));
            Assert.That(actualModel.ChartData.SeriesCollection[1].Points.Count, Is.EqualTo(4));
            Assert.That(actualModel.ChartData.SeriesCollection[2].Points.Count, Is.EqualTo(4));

            Assert.That(actualModel.ChartData.SeriesCollection[0].Points[0].Label, Is.EqualTo(String.Format(suppliedMetricFormat, suppliedBelowRate1)));
            Assert.That(actualModel.ChartData.SeriesCollection[0].Points[1].Label, Is.EqualTo(String.Format(suppliedMetricFormat, suppliedBelowRate2)));
            Assert.That(actualModel.ChartData.SeriesCollection[0].Points[2].Label, Is.EqualTo(String.Format(suppliedMetricFormat, suppliedBelowRate3)));
            Assert.That(actualModel.ChartData.SeriesCollection[0].Points[3].Label, Is.EqualTo(String.Format(suppliedMetricFormat, 0)));

            Assert.That(actualModel.ChartData.SeriesCollection[1].Points[0].Label, Is.EqualTo(String.Format(suppliedMetricFormat, suppliedMetStandardRate1)));
            Assert.That(actualModel.ChartData.SeriesCollection[1].Points[1].Label, Is.EqualTo(String.Format(suppliedMetricFormat, suppliedMetStandardRate2)));
            Assert.That(actualModel.ChartData.SeriesCollection[1].Points[2].Label, Is.EqualTo(String.Format(suppliedMetricFormat, suppliedMetStandardRate3)));
            Assert.That(actualModel.ChartData.SeriesCollection[1].Points[3].Label, Is.EqualTo(String.Format(suppliedMetricFormat, 0)));

            Assert.That(actualModel.ChartData.SeriesCollection[2].Points[0].Label, Is.EqualTo(String.Format(suppliedMetricFormat, suppliedCommendedRate1)));
            Assert.That(actualModel.ChartData.SeriesCollection[2].Points[1].Label, Is.EqualTo(String.Format(suppliedMetricFormat, suppliedCommendedRate2)));
            Assert.That(actualModel.ChartData.SeriesCollection[2].Points[2].Label, Is.EqualTo(String.Format(suppliedMetricFormat, suppliedCommendedRate3)));
            Assert.That(actualModel.ChartData.SeriesCollection[1].Points[3].Label, Is.EqualTo(String.Format(suppliedMetricFormat, 0)));

            Assert.That(actualModel.ChartData.SeriesCollection[0].Points[0].AxisLabel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(actualModel.ChartData.SeriesCollection[1].Points[0].AxisLabel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(actualModel.ChartData.SeriesCollection[2].Points[0].AxisLabel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(actualModel.ChartData.SeriesCollection[0].Points[3].AxisLabel, Is.EqualTo(suppliedGradeLevel4));

            Assert.That(actualModel.ChartData.SeriesCollection[0].Points[0].Value, Is.EqualTo(suppliedBelowRate1));
            Assert.That(actualModel.ChartData.SeriesCollection[1].Points[0].Value, Is.EqualTo(suppliedMetStandardRate1));
            Assert.That(actualModel.ChartData.SeriesCollection[2].Points[0].Value, Is.EqualTo(suppliedCommendedRate1));
            Assert.That(actualModel.ChartData.SeriesCollection[0].Points[3].Value, Is.EqualTo(0));

            Assert.That(actualModel.ChartData.SeriesCollection[0].Points[0].Tooltip, Is.EqualTo(String.Format(suppliedMetricFormat, suppliedBelowRate1)));
            Assert.That(actualModel.ChartData.SeriesCollection[1].Points[0].Tooltip, Is.EqualTo(String.Format(suppliedMetricFormat, suppliedMetStandardRate1)));
            Assert.That(actualModel.ChartData.SeriesCollection[2].Points[0].Tooltip, Is.EqualTo(String.Format(suppliedMetricFormat, suppliedCommendedRate1)));
            Assert.That(actualModel.ChartData.SeriesCollection[0].Points[3].Tooltip, Is.EqualTo(String.Format(suppliedMetricFormat, 0)));

            Assert.That(actualModel.ChartData.SeriesCollection[0].Points[0].IsValueShownAsLabel, Is.True);
            Assert.That(actualModel.ChartData.SeriesCollection[1].Points[0].IsValueShownAsLabel, Is.True);
            Assert.That(actualModel.ChartData.SeriesCollection[2].Points[0].IsValueShownAsLabel, Is.True);
            Assert.That(actualModel.ChartData.SeriesCollection[0].Points[3].IsValueShownAsLabel, Is.False);
            Assert.That(actualModel.ChartData.SeriesCollection[0].Points[2].IsValueShownAsLabel, Is.False);

            Assert.That(actualModel.ChartData.SeriesCollection[0].Points[0].State, Is.EqualTo(MetricStateType.None));
            Assert.That(actualModel.ChartData.SeriesCollection[0].Points[0].Trend, Is.EqualTo(TrendEvaluation.None));
        }

        [Test]
        public void Should_sort_grades()
        {
            foreach (var series in actualModel.ChartData.SeriesCollection)
            {
                Assert.That(series.Points[0].AxisLabel, Is.EqualTo(suppliedGradeLevel1));
                Assert.That(series.Points[1].AxisLabel, Is.EqualTo(suppliedGradeLevel2));
                Assert.That(series.Points[2].AxisLabel, Is.EqualTo(suppliedGradeLevel3));
                Assert.That(series.Points[3].AxisLabel, Is.EqualTo(suppliedGradeLevel4));
            }
        }

        [Test]
        public void Should_load_correct_footnotes()
        {
            Assert.That(actualModel.MetricFootnotes.Count, Is.EqualTo(2));
            Assert.That(actualModel.MetricFootnotes[0].FootnoteText, Is.EqualTo(suppliedFootnoteText3));
            Assert.That(actualModel.MetricFootnotes[1].FootnoteText, Is.EqualTo(suppliedFootnoteText2));
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }
}
