// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Warehouse.Data.Entities;
using EdFi.Dashboards.Warehouse.Resources.Application;
using EdFi.Dashboards.Warehouse.Resources.School.Detail;
using NUnit.Framework;
using Rhino.Mocks;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.Charting;
using EdFi.Dashboards.Testing;

namespace EdFi.Dashboards.Warehouse.Resources.Tests.School.Detail
{
    [TestFixture]
    public class When_getting_model_for_school_historical_chart_with_no_period : TestFixtureBase
    {
        protected IRepository<SchoolMetricHistorical> schoolMetricHistoricalRepository;
        protected IMetricGoalProvider metricGoalProvider;
        protected IMetricStateProvider metricStateProvider;
        protected IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        protected ITrendRenderingDispositionProvider trendRenderingDispositionProvider;
        protected IMetricNodeResolver metricNodeResolver;
        protected IWarehouseAvailabilityProvider warehouseAvailabilityProvider;

        protected HistoricalChartModel actualModel;
        protected IQueryable<SchoolMetricHistorical> suppliedSchoolMetricHistorical;

        protected long studentUSI1 = 1;
        protected int schoolId1 = 100;
        protected int metricId1 = 1000;
        protected int metricVariantId1 = 1000999;
        protected int periodId1 = 2;
        protected Goal schoolGoal1 = new Goal { Value = .65m, Interpretation = TrendInterpretation.Standard };
        protected Goal schoolGoal2 = new Goal { Value = .65m, Interpretation = TrendInterpretation.Inverse };
        protected Guid suppliedSchoolMetricInstanceSetKey1 = Guid.NewGuid();
        protected string suppliedFormat = "{0:P2} test";
        protected string suppliedNumeratorDenominatorFormat = "{0} test {1}";
        protected string suppliedChartTitle = "HistoricalChart";

        protected override void EstablishContext()
        {
            schoolMetricHistoricalRepository = mocks.StrictMock<IRepository<SchoolMetricHistorical>>();
            metricGoalProvider = mocks.StrictMock<IMetricGoalProvider>();
            metricStateProvider = mocks.StrictMock<IMetricStateProvider>();
            metricInstanceSetKeyResolver = mocks.StrictMock<IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest>>();
            trendRenderingDispositionProvider = mocks.StrictMock<ITrendRenderingDispositionProvider>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            warehouseAvailabilityProvider = mocks.StrictMock<IWarehouseAvailabilityProvider>();

            suppliedSchoolMetricHistorical = GetSuppliedSchoolMetricHistorical();

            Expect.Call(warehouseAvailabilityProvider.Get()).Return(true);
            Expect.Call(metricNodeResolver.GetMetricNodeForSchoolFromMetricVariantId(schoolId1, metricVariantId1)).Return(GetSuppliedMetricMetadataNode());
            Expect.Call(schoolMetricHistoricalRepository.GetAll()).Repeat.Any().Return(suppliedSchoolMetricHistorical);
            Expect.Call(metricGoalProvider.GetMetricGoal(suppliedSchoolMetricInstanceSetKey1, metricId1)).Return(schoolGoal1);
            //This is tested elsewhere so we ignore and return a fixed value.
            Expect.Call(metricStateProvider.GetState(1, ".0", "System.IgnoreThisParam")).IgnoreArguments().Repeat.Any().
                Return(new State(MetricStateType.Good, "Good"));

            Expect.Call(
                metricInstanceSetKeyResolver.GetMetricInstanceSetKey(null))
                .Constraints(
                    new ActionConstraint<SchoolMetricInstanceSetRequest>(x =>
                    {
                        Assert.That(x.SchoolId == schoolId1);
                        Assert.That(x.MetricVariantId == metricVariantId1);
                    })
                ).
                Return(suppliedSchoolMetricInstanceSetKey1);

            Expect.Call(trendRenderingDispositionProvider.GetTrendRenderingDisposition(TrendDirection.Decreasing, TrendInterpretation.Standard)).IgnoreArguments().Repeat.Any().Return(TrendEvaluation.UpNoOpinion);
        }

        protected IQueryable<SchoolMetricHistorical> GetSuppliedSchoolMetricHistorical()
        {
            var list = new List<SchoolMetricHistorical>
                        {
                            new SchoolMetricHistorical{PeriodIdentifierId = 1, PeriodTypeId = 1, PeriodType = "Day", StartDate = new DateTime(2011,1,1), EndDate = new DateTime(2011,1,1), MetricId = metricId1, SchoolId = schoolId1, Context = "01/01", Value = ".80", ValueType = "System.Double", Numerator = 80, Denominator = 100, Flag = true, TrendDirection = 1},
                            new SchoolMetricHistorical{PeriodIdentifierId = 2, PeriodTypeId = 1, PeriodType = "Day", StartDate = new DateTime(2011,1,2), EndDate = new DateTime(2011,1,2), MetricId = metricId1, SchoolId = schoolId1, Context = "01/02", Value = ".95", ValueType = "System.Double", Numerator = 95, Denominator = 100, Flag = null, TrendDirection = null},
                            new SchoolMetricHistorical{PeriodIdentifierId = 3, PeriodTypeId = 1, PeriodType = "Day", StartDate = new DateTime(2011,1,3), EndDate = new DateTime(2011,1,3), MetricId = metricId1, SchoolId = schoolId1, Context = "01/03", Value = ".55", ValueType = "System.Double", Numerator = 110, Denominator = 200, Flag = true, TrendDirection = 1},
                            new SchoolMetricHistorical{PeriodIdentifierId = 4, PeriodTypeId = 1, PeriodType = "Day", StartDate = new DateTime(2011,1,1), EndDate = new DateTime(2011,1,1), MetricId = metricId1, SchoolId = schoolId1, Context = "01/04", Value = ".44", ValueType = "System.Double", Numerator = 44, Denominator = 100, Flag = true, TrendDirection = 1},
                            new SchoolMetricHistorical{PeriodIdentifierId = 5, PeriodTypeId = 1, PeriodType = "Day", StartDate = new DateTime(2011,1,2), EndDate = new DateTime(2011,1,2), MetricId = metricId1, SchoolId = schoolId1, Context = "01/05", Value = ".79", ValueType = "System.Double", Numerator = 79, Denominator = 100, Flag = null, TrendDirection = null},
                            new SchoolMetricHistorical{PeriodIdentifierId = 6, PeriodTypeId = 1, PeriodType = "Day", StartDate = new DateTime(2011,1,3), EndDate = new DateTime(2011,1,3), MetricId = metricId1, SchoolId = schoolId1, Context = "01/06", Value = ".67", ValueType = "System.Double", Numerator = 67, Denominator = 100, Flag = true, TrendDirection = 1},

                            new SchoolMetricHistorical{PeriodIdentifierId = 1, PeriodTypeId = 3, PeriodType = "Grading Period", StartDate = new DateTime(2011,1,1), EndDate = new DateTime(2011,1,31), MetricId = metricId1, SchoolId = schoolId1, Context = "01/01", Value = ".81", ValueType = "System.Double", Numerator = 81, Denominator = 100, Flag = true, TrendDirection = 1},
                            new SchoolMetricHistorical{PeriodIdentifierId = 2, PeriodTypeId = 3, PeriodType = "Grading Period", StartDate = new DateTime(2011,2,1), EndDate = new DateTime(2011,1,28), MetricId = metricId1, SchoolId = schoolId1, Context = "01/02", Value = ".92", ValueType = "System.Double", Numerator = 92, Denominator = 100, Flag = null, TrendDirection = null},
                            new SchoolMetricHistorical{PeriodIdentifierId = 3, PeriodTypeId = 3, PeriodType = "Grading Period", StartDate = new DateTime(2011,3,1), EndDate = new DateTime(2011,1,31), MetricId = metricId1, SchoolId = schoolId1, Context = "01/03", Value = ".53", ValueType = "System.Double", Numerator = 53, Denominator = 100, Flag = true, TrendDirection = 1},

                            new SchoolMetricHistorical{PeriodIdentifierId = 1, PeriodTypeId = 2, PeriodType = "Week", StartDate = new DateTime(2011,1,1), EndDate = new DateTime(2011,1,5), MetricId = metricId1, SchoolId = schoolId1, Context = "01/01", Value = ".80", ValueType = "System.Double", Numerator = 80, Denominator = 100, Flag = true, TrendDirection = 1},
                            new SchoolMetricHistorical{PeriodIdentifierId = 4, PeriodTypeId = 2, PeriodType = "Week", StartDate = new DateTime(2011,1,16), EndDate = new DateTime(2011,1,20), MetricId = metricId1, SchoolId = schoolId1, Context = "01/03", Value = ".55", ValueType = "System.Double", Numerator = 110, Denominator = 200, Flag = true, TrendDirection = 1},
                            new SchoolMetricHistorical{PeriodIdentifierId = 2, PeriodTypeId = 2, PeriodType = "Week", StartDate = new DateTime(2011,1,6), EndDate = new DateTime(2011,1,10), MetricId = metricId1, SchoolId = schoolId1, Context = "01/02", Value = ".95", ValueType = "System.Double", Numerator = 95, Denominator = 100, Flag = null, TrendDirection = null},
                            new SchoolMetricHistorical{PeriodIdentifierId = 3, PeriodTypeId = 2, PeriodType = "Week", StartDate = new DateTime(2011,1,11), EndDate = new DateTime(2011,1,15), MetricId = metricId1, SchoolId = schoolId1, Context = "01/03", Value = ".55", ValueType = "System.Double", Numerator = 110, Denominator = 200, Flag = true, TrendDirection = 1},
                        };

            return list.AsQueryable();
        }

        public MetricMetadataNode GetSuppliedMetricMetadataNode()
        {
            return new MetricMetadataNode(null)
            {
                MetricId = metricId1,
                MetricVariantId = metricVariantId1,
                Format = suppliedFormat,
                NumeratorDenominatorFormat = suppliedNumeratorDenominatorFormat,
                TrendInterpretation = 1,
                Actions = new List<MetricAction>
                        {
                            new MetricAction{ Title = "Bogus Title", DrilldownHeader = "Incorrect Chart Title"},
                            new MetricAction{ Title = suppliedChartTitle, DrilldownHeader = "Correct Chart Title"},
                        }
            };
        }

        protected override void ExecuteTest()
        {
            var service = new HistoricalChartService(schoolMetricHistoricalRepository, metricGoalProvider, metricStateProvider, metricInstanceSetKeyResolver, trendRenderingDispositionProvider, metricNodeResolver, warehouseAvailabilityProvider);
            actualModel = service.Get(new HistoricalChartRequest
                                          {
                                              SchoolId = schoolId1,
                                              MetricVariantId = metricVariantId1,
                                              Title = suppliedChartTitle
                                          });
        }

        [Test]
        public virtual void Should_bind_chart_data_title()
        {
            Assert.That(actualModel.ChartData.ChartTitle, Is.Not.Empty);
            var suppliedTitle = GetSuppliedMetricMetadataNode().Actions.Single(x => x.Title == suppliedChartTitle).DrilldownHeader;
            Assert.That(actualModel.ChartData.ChartTitle, Is.EqualTo(suppliedTitle));
        }

        [Test]
        public virtual void Should_calculate_drilldown_title_correctly()
        {
            var maxPeriodId = suppliedSchoolMetricHistorical.Max(x => x.PeriodTypeId);
            var suppliedPeriod = suppliedSchoolMetricHistorical.First(x => x.PeriodTypeId == maxPeriodId);
            //The count should be the same.			
            Assert.That(actualModel.DrillDownTitle, Is.EqualTo(suppliedPeriod.PeriodType));
        }

        [Test]
        public void Should_calculate_available_periods_correctly()
        {
            var suppliedAvaialablePeriods = GetSuppliedAvailablePeriods();
            //The count should be the same.			
            Assert.That(actualModel.AvailablePeriods.Count, Is.EqualTo(suppliedAvaialablePeriods.Count));

            for (int i = 0; i < suppliedAvaialablePeriods.Count; i++)
            {
                //The content should be the same.
                Assert.That(actualModel.AvailablePeriods[i].Id, Is.EqualTo(suppliedAvaialablePeriods[i].Id), "The IDS dont Match...Could be a sorting problem.");
                Assert.That(actualModel.AvailablePeriods[i].Text, Is.EqualTo(suppliedAvaialablePeriods[i].Text), "The Text does not Match...Could be a sorting problem.");

                //The order should be period 1 to N.
                Assert.That(actualModel.AvailablePeriods[i].Id, Is.EqualTo(i + 1));
            }
        }

        [Test]
        public void Should_calculate_school_goal_correctly()
        {
            //Lets see that its not null.
            Assert.That(actualModel.ChartData.StripLines, !Is.Null);

            //We should have only one as per this spec..
            Assert.That(actualModel.ChartData.StripLines.Count, Is.EqualTo(1));

            //Lets see if content is correct.
            Assert.That(actualModel.ChartData.StripLines[0].Value, Is.EqualTo(schoolGoal1.Value));
            Assert.That(actualModel.ChartData.StripLines[0].Tooltip, Is.EqualTo(string.Format("School Goal: {0:P2} test", schoolGoal1.Value)));
        }

        [Test]
        public virtual void Should_calculate_chart_data_correctly()
        {
            Assert.That(actualModel.ChartData, !Is.Null);

            Assert.That(actualModel.ChartData.SeriesCollection, !Is.Null);

            //We should have only one as per this spec..
            Assert.That(actualModel.ChartData.SeriesCollection.Count, Is.EqualTo(1));
            Assert.That(actualModel.ChartData.SeriesCollection[0].Name, Is.EqualTo("Historical"));

            ////Should have same amount of points for default period.
            var defaultPeriodId = suppliedSchoolMetricHistorical.Max(x => x.PeriodTypeId);
            var defaultPeriodData = suppliedSchoolMetricHistorical.Where(x => x.PeriodTypeId == defaultPeriodId).OrderBy(x => x.PeriodIdentifierId).ToList();
            Assert.That(actualModel.ChartData.SeriesCollection[0].Points.Count, !Is.EqualTo(0));
            Assert.That(actualModel.ChartData.SeriesCollection[0].Points.Count, Is.EqualTo(defaultPeriodData.Count()));

            //Checking that all points exist and are in the correct order.
            for (int i = 0; i < defaultPeriodData.Count(); i++)
            {
                Assert.That(actualModel.ChartData.SeriesCollection[0].Points[i].Value, Is.EqualTo(Convert.ToDouble(defaultPeriodData[i].Value)));
                Assert.That(actualModel.ChartData.SeriesCollection[0].Points[i].Label, Is.EqualTo(defaultPeriodData[i].Context));
                Assert.That(actualModel.ChartData.SeriesCollection[0].Points[i].State,
                            defaultPeriodData[i].MetricStateTypeId != null
                                ? Is.EqualTo((MetricStateType)defaultPeriodData[i].MetricStateTypeId)
                                : Is.EqualTo(MetricStateType.Good));
            }
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }

        private List<HistoricalChartModel.PeriodItem> GetSuppliedAvailablePeriods()
        {
            var data = (from d in suppliedSchoolMetricHistorical
                        group d by d.PeriodTypeId into g
                        select new
                        {
                            PeriodTypeId = g.Key,
                            g.First().PeriodType
                        });

            return (from p in data.ToList()
                    orderby p.PeriodTypeId
                    select new HistoricalChartModel.PeriodItem
                    {
                        Id = p.PeriodTypeId,
                        Text = p.PeriodType,
                    }
                   ).ToList();
        }
    }

    [TestFixture]
    public class When_getting_model_for_school_historical_chart_with_supplied_period : When_getting_model_for_school_historical_chart_with_no_period
    {

        protected override void ExecuteTest()
        {
            var service = new HistoricalChartService(schoolMetricHistoricalRepository, metricGoalProvider, metricStateProvider, metricInstanceSetKeyResolver, trendRenderingDispositionProvider, metricNodeResolver, warehouseAvailabilityProvider);
            actualModel = service.Get(new HistoricalChartRequest
                                          {
                                              SchoolId = schoolId1,
                                              MetricVariantId = metricVariantId1,
                                              PeriodId = periodId1,
                                              Title = suppliedChartTitle
                                          });
        }

        [Test]
        public override void Should_calculate_drilldown_title_correctly()
        {
            var suppliedPeriod = suppliedSchoolMetricHistorical.First(x => x.PeriodTypeId == periodId1);
            //The count should be the same.			
            Assert.That(actualModel.DrillDownTitle, Is.EqualTo(suppliedPeriod.PeriodType));
        }

        [Test]
        public override void Should_calculate_chart_data_correctly()
        {
            Assert.That(actualModel.ChartData, !Is.Null);

            Assert.That(actualModel.ChartData.SeriesCollection, !Is.Null);

            //We should have only one as per this spec..
            Assert.That(actualModel.ChartData.SeriesCollection.Count, Is.EqualTo(1));
            Assert.That(actualModel.ChartData.SeriesCollection[0].Name, Is.EqualTo("Historical"));

            ////Should have same amount of points for default period.
            var defaultPeriodId = periodId1;
            var defaultPeriodData = suppliedSchoolMetricHistorical.Where(x => x.PeriodTypeId == defaultPeriodId).OrderBy(x => x.PeriodIdentifierId).ToList();
            Assert.That(actualModel.ChartData.SeriesCollection[0].Points.Count, !Is.EqualTo(0));
            Assert.That(actualModel.ChartData.SeriesCollection[0].Points.Count, Is.EqualTo(defaultPeriodData.Count()));

            //Checking that all points exist and are in the correct order.
            for (int i = 0; i < defaultPeriodData.Count(); i++)
            {
                Assert.That(actualModel.ChartData.SeriesCollection[0].Points[i].Value, Is.EqualTo(Convert.ToDouble(defaultPeriodData[i].Value)));
                Assert.That(actualModel.ChartData.SeriesCollection[0].Points[i].Label, Is.EqualTo(defaultPeriodData[i].Context));
                Assert.That(actualModel.ChartData.SeriesCollection[0].Points[i].State,
                            defaultPeriodData[i].MetricStateTypeId != null
                                ? Is.EqualTo((MetricStateType)defaultPeriodData[i].MetricStateTypeId)
                                : Is.EqualTo(MetricStateType.Good));
            }
        }

    }

    [TestFixture]
    public class When_getting_historical_chart_but_warehouse_is_unavailable : TestFixtureBase
    {

        protected IRepository<SchoolMetricHistorical> studentMetricHistoricalRepository;
        protected IMetricGoalProvider metricGoalProvider;
        protected IMetricStateProvider metricStateProvider;
        protected IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        protected ITrendRenderingDispositionProvider trendRenderingDispositionProvider;
        protected IMetricNodeResolver metricNodeResolver;
        protected IWarehouseAvailabilityProvider warehouseAvailabilityProvider;

        protected HistoricalChartModel actualModel;

        protected int schoolId1 = 100;
        protected int metricId1 = 1000;
        protected int metricVariantId1 = 100099;
        protected int periodId1 = 2;

        protected override void EstablishContext()
        {
            studentMetricHistoricalRepository = mocks.StrictMock<IRepository<SchoolMetricHistorical>>();
            metricGoalProvider = mocks.StrictMock<IMetricGoalProvider>();
            metricStateProvider = mocks.StrictMock<IMetricStateProvider>();
            metricInstanceSetKeyResolver = mocks.StrictMock<IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest>>();
            trendRenderingDispositionProvider = mocks.StrictMock<ITrendRenderingDispositionProvider>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            warehouseAvailabilityProvider = mocks.StrictMock<IWarehouseAvailabilityProvider>();

            Expect.Call(warehouseAvailabilityProvider.Get()).Return(false);
        }

        protected override void ExecuteTest()
        {
            var service = new HistoricalChartService(studentMetricHistoricalRepository, metricGoalProvider, metricStateProvider, metricInstanceSetKeyResolver, trendRenderingDispositionProvider, metricNodeResolver, warehouseAvailabilityProvider);
            actualModel = service.Get(new HistoricalChartRequest
            {
                SchoolId = schoolId1,
                MetricVariantId = metricVariantId1,
                PeriodId = periodId1,
            });
        }

        [Test]
        public void Should_return_empty_model()
        {
            Assert.That(actualModel, Is.Not.Null);
        }
    }

}
