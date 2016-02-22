// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.StudentSchool.Detail;
using NUnit.Framework;
using Rhino.Mocks;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.Charting;
using EdFi.Dashboards.Testing;

namespace EdFi.Dashboards.Resources.Tests.Student.Detail
{
    public abstract class When_getting_credit_accumulation_chart_base : TestFixtureBase
    {
        protected const string suppliedChartTitle = "CreditAccumulationHistoricalGraph";
        protected int suppliedMetricId = (int)StudentMetricEnum.CreditAccumulation;

        public MetricMetadataNode GetSuppliedMetricMetadataNode()
        {
            return new MetricMetadataNode(null)
            {
                MetricId = suppliedMetricId,
                Actions = new List<MetricAction>
                        {
                            new MetricAction{ Title = "Bogus Title", DrilldownHeader = "Incorrect Chart Title"},
                            new MetricAction{ Title = suppliedChartTitle, DrilldownHeader = "Correct Chart Title"},
                        }
            };
        }
    }

    public class When_getting_credit_accumulation_chart : When_getting_credit_accumulation_chart_base
    {
        private IRepository<StudentMetricCreditAccumulation> repository;
        private IMetricNodeResolver metricNodeResolver;

        private IQueryable<StudentMetricCreditAccumulation> suppliedData;
        private const int suppliedStudentUSI = 1000;
        private const int suppliedSchoolId = 2000;

        private const string suppliedGradeLevel1 = "9th Grade";
        private const int suppliedMetricState1 = 3;
        private const decimal suppliedCumulative1 = 1.22m;
        private const decimal suppliedRecommended1 = 4m;
        private const decimal suppliedMinimum1 = 2.7m;
        private const int suppliedSchoolYear1 = 2010;

        private const string suppliedGradeLevel2 = "apple";
        private const int suppliedMetricState2 = 1;
        private const decimal suppliedCumulative2 = 2.344m;
        private const decimal suppliedRecommended2 = 3.33m;
        private const decimal suppliedMinimum2 = 6m;
        private const int suppliedSchoolYear2 = 2011;

        private const string suppliedGradeLevel3 = "zoo";
        private const decimal suppliedCumulative3 = 1.4m;
        private const decimal suppliedRecommended3 = 3.6m;
        private const decimal suppliedMinimum3 = 2.7m;
        private const int suppliedSchoolYear3 = 2012;

        private const string suppliedGradeLevel4 = "fire";
        private const decimal suppliedRecommended4 = 1m;
        private const decimal suppliedMinimum4 = 5m;
        private const int suppliedSchoolYear4 = 2013;

        
        private ChartData actualModel;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            suppliedData = GetData();
            repository = mocks.StrictMock<IRepository<StudentMetricCreditAccumulation>>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();

            Expect.Call(metricNodeResolver.GetMetricNodeForStudentFromMetricVariantId(suppliedSchoolId, suppliedMetricId)).Return(GetSuppliedMetricMetadataNode());

            Expect.Call(repository.GetAll()).Return(suppliedData);
        }

        protected IQueryable<StudentMetricCreditAccumulation> GetData()
        {
            var data = new List<StudentMetricCreditAccumulation>
                           {
                               new StudentMetricCreditAccumulation {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId + 1, MetricId=(int) StudentMetricEnum.CreditAccumulation, GradeLevel = "wrong"},
                               new StudentMetricCreditAccumulation {StudentUSI = suppliedStudentUSI + 1, SchoolId = suppliedSchoolId, MetricId=(int) StudentMetricEnum.CreditAccumulation, GradeLevel = "wrong"},
                               new StudentMetricCreditAccumulation {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId=(int) StudentMetricEnum.CreditAccumulation + 1, GradeLevel = "wrong"},
                               new StudentMetricCreditAccumulation {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId=(int) StudentMetricEnum.CreditAccumulation, SchoolYear = suppliedSchoolYear3, GradeLevel = suppliedGradeLevel3, CumulativeCredits = suppliedCumulative3, MinimumCredits = suppliedMinimum3, RecommendedCredits = suppliedRecommended3},
                               new StudentMetricCreditAccumulation {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId=(int) StudentMetricEnum.CreditAccumulation, SchoolYear = suppliedSchoolYear1, GradeLevel = suppliedGradeLevel1,  MetricStateTypeId = suppliedMetricState1, CumulativeCredits = suppliedCumulative1, MinimumCredits = suppliedMinimum1, RecommendedCredits = suppliedRecommended1 },
                               new StudentMetricCreditAccumulation {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId=(int) StudentMetricEnum.CreditAccumulation, SchoolYear = suppliedSchoolYear4, GradeLevel = suppliedGradeLevel4, MinimumCredits = suppliedMinimum4, RecommendedCredits = suppliedRecommended4},
                               new StudentMetricCreditAccumulation {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId=(int) StudentMetricEnum.CreditAccumulation, SchoolYear = suppliedSchoolYear2, GradeLevel = suppliedGradeLevel2, MetricStateTypeId = suppliedMetricState2, CumulativeCredits = suppliedCumulative2, MinimumCredits = suppliedMinimum2, RecommendedCredits = suppliedRecommended2},
                           };
            return data.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new CreditAccumulationChartService(repository, metricNodeResolver);
            actualModel = service.Get(new CreditAccumulationChartRequest
                                      {
                                          StudentUSI = suppliedStudentUSI,
                                          SchoolId = suppliedSchoolId,
                                          MetricVariantId = suppliedMetricId,
                                          Title = suppliedChartTitle
                                      });
        }

        [Test]
        public void Should_create_correct_number_of_series()
        {
            Assert.That(actualModel.SeriesCollection.Count, Is.EqualTo(3));
        }

        [Test]
        public void Should_populate_student_points_correctly()
        {
            Assert.That(actualModel.SeriesCollection[0], Is.TypeOf(typeof(ChartData.Series)));
            Assert.That(actualModel.SeriesCollection[0].Name, Is.EqualTo("Actual"));
            Assert.That(actualModel.SeriesCollection[0].ShowInLegend, Is.False);
            Assert.That(actualModel.SeriesCollection[0].Points.Count, Is.EqualTo(4));
            Assert.That(actualModel.SeriesCollection[0].Points[0].Value, Is.EqualTo(suppliedCumulative1));
            Assert.That(actualModel.SeriesCollection[0].Points[1].Value, Is.EqualTo(suppliedCumulative2));
            Assert.That(actualModel.SeriesCollection[0].Points[2].Value, Is.EqualTo(suppliedCumulative3));
            Assert.That(actualModel.SeriesCollection[0].Points[3].Value, Is.EqualTo(0));
            Assert.That(actualModel.SeriesCollection[0].Points[0].AxisLabel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(actualModel.SeriesCollection[0].Points[1].AxisLabel, Is.EqualTo(suppliedGradeLevel2));
            Assert.That(actualModel.SeriesCollection[0].Points[2].AxisLabel, Is.EqualTo(suppliedGradeLevel3));
            Assert.That(actualModel.SeriesCollection[0].Points[3].AxisLabel, Is.EqualTo(suppliedGradeLevel4));
            Assert.That(actualModel.SeriesCollection[0].Points[0].Tooltip, Is.EqualTo("1.22 credits"));
            Assert.That(actualModel.SeriesCollection[0].Points[1].Tooltip, Is.EqualTo("2.344 credits"));
            Assert.That(actualModel.SeriesCollection[0].Points[2].Tooltip, Is.EqualTo("1.4 credits"));
            Assert.That(actualModel.SeriesCollection[0].Points[3].Tooltip, Is.EqualTo("0 credits"));
            Assert.That(actualModel.SeriesCollection[0].Points[0].IsValueShownAsLabel, Is.False);
            Assert.That(actualModel.SeriesCollection[0].Points[1].IsValueShownAsLabel, Is.False);
            Assert.That(actualModel.SeriesCollection[0].Points[2].IsValueShownAsLabel, Is.False);
            Assert.That(actualModel.SeriesCollection[0].Points[3].IsValueShownAsLabel, Is.False);
            Assert.That(actualModel.SeriesCollection[0].Points[0].State, Is.EqualTo(MetricStateType.Low));
            Assert.That(actualModel.SeriesCollection[0].Points[1].State, Is.EqualTo(MetricStateType.Good));
            Assert.That(actualModel.SeriesCollection[0].Points[2].State, Is.EqualTo(MetricStateType.None));
            Assert.That(actualModel.SeriesCollection[0].Points[3].State, Is.EqualTo(MetricStateType.None));

        }

        [Test]
        public void Should_populate_recommended_points_correctly()
        {
            Assert.That(actualModel.SeriesCollection[1], Is.TypeOf(typeof (ChartData.LineSeries)));
            Assert.That(actualModel.SeriesCollection[1].Name, Is.EqualTo("Recommended"));
            Assert.That(actualModel.SeriesCollection[1].ShowInLegend, Is.True);
            Assert.That(actualModel.SeriesCollection[1].Points.Count, Is.EqualTo(4));
            Assert.That(actualModel.SeriesCollection[1].Points[0].Value, Is.EqualTo(suppliedRecommended1));
            Assert.That(actualModel.SeriesCollection[1].Points[1].Value, Is.EqualTo(suppliedRecommended2));
            Assert.That(actualModel.SeriesCollection[1].Points[2].Value, Is.EqualTo(suppliedRecommended3));
            Assert.That(actualModel.SeriesCollection[1].Points[3].Value, Is.EqualTo(suppliedRecommended4));
            Assert.That(actualModel.SeriesCollection[1].Points[0].IsValueShownAsLabel, Is.True);
            Assert.That(actualModel.SeriesCollection[1].Points[1].IsValueShownAsLabel, Is.True);
            Assert.That(actualModel.SeriesCollection[1].Points[2].IsValueShownAsLabel, Is.True);
            Assert.That(actualModel.SeriesCollection[1].Points[3].IsValueShownAsLabel, Is.True);
            Assert.That(actualModel.SeriesCollection[1].Points[0].State, Is.EqualTo(MetricStateType.None));
            Assert.That(actualModel.SeriesCollection[1].Points[1].State, Is.EqualTo(MetricStateType.None));
            Assert.That(actualModel.SeriesCollection[1].Points[2].State, Is.EqualTo(MetricStateType.None));
            Assert.That(actualModel.SeriesCollection[1].Points[3].State, Is.EqualTo(MetricStateType.None));
        }

        [Test]
        public void Should_populate_minimum_points_correctly()
        {
            Assert.That(actualModel.SeriesCollection[2], Is.TypeOf(typeof(ChartData.LineSeries)));
            Assert.That(actualModel.SeriesCollection[2].Name, Is.EqualTo("Minimum"));
            Assert.That(actualModel.SeriesCollection[2].ShowInLegend, Is.True);
            Assert.That(actualModel.SeriesCollection[2].Points.Count, Is.EqualTo(4));
            Assert.That(actualModel.SeriesCollection[2].Points[0].Value, Is.EqualTo(suppliedMinimum1));
            Assert.That(actualModel.SeriesCollection[2].Points[1].Value, Is.EqualTo(suppliedMinimum2));
            Assert.That(actualModel.SeriesCollection[2].Points[2].Value, Is.EqualTo(suppliedMinimum3));
            Assert.That(actualModel.SeriesCollection[2].Points[3].Value, Is.EqualTo(suppliedMinimum4));
            Assert.That(actualModel.SeriesCollection[2].Points[0].IsValueShownAsLabel, Is.True);
            Assert.That(actualModel.SeriesCollection[2].Points[1].IsValueShownAsLabel, Is.True);
            Assert.That(actualModel.SeriesCollection[2].Points[2].IsValueShownAsLabel, Is.True);
            Assert.That(actualModel.SeriesCollection[2].Points[3].IsValueShownAsLabel, Is.True);
            Assert.That(actualModel.SeriesCollection[2].Points[0].State, Is.EqualTo(MetricStateType.None));
            Assert.That(actualModel.SeriesCollection[2].Points[1].State, Is.EqualTo(MetricStateType.None));
            Assert.That(actualModel.SeriesCollection[2].Points[2].State, Is.EqualTo(MetricStateType.None));
            Assert.That(actualModel.SeriesCollection[2].Points[3].State, Is.EqualTo(MetricStateType.None));
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
            var suppliedTitle = GetSuppliedMetricMetadataNode().Actions.Single(x => x.Title == When_getting_credit_accumulation_chart.suppliedChartTitle).DrilldownHeader;
            Assert.That(actualModel.ChartTitle, Is.EqualTo(suppliedTitle));
        }
    }

    public class When_getting_credit_accumulation_for_early_grade : When_getting_credit_accumulation_chart_base
    {
        private IRepository<StudentMetricCreditAccumulation> repository;
        private IMetricNodeResolver metricNodeResolver;

        private IQueryable<StudentMetricCreditAccumulation> suppliedData;
        private const int suppliedStudentUSI = 1000;
        private const int suppliedSchoolId = 2000;

        private ChartData results;

        

        protected override void EstablishContext()
        {
            base.EstablishContext();

            suppliedData = GetData();
            repository = mocks.StrictMock<IRepository<StudentMetricCreditAccumulation>>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            Expect.Call(metricNodeResolver.GetMetricNodeForStudentFromMetricVariantId(suppliedSchoolId, suppliedMetricId)).Return(GetSuppliedMetricMetadataNode());

            Expect.Call(repository.GetAll()).Return(suppliedData);
        }

        protected IQueryable<StudentMetricCreditAccumulation> GetData()
        {
            var data = new List<StudentMetricCreditAccumulation>
                           {
                               new StudentMetricCreditAccumulation {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId + 1, MetricId=(int) StudentMetricEnum.CreditAccumulation, GradeLevel = "wrong1"},
                               new StudentMetricCreditAccumulation {StudentUSI = suppliedStudentUSI + 1, SchoolId = suppliedSchoolId, MetricId=(int) StudentMetricEnum.CreditAccumulation, GradeLevel = "wrong2"},
                               new StudentMetricCreditAccumulation {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId=(int) StudentMetricEnum.CreditAccumulation + 1, GradeLevel = "wrong3"},
                           };
            return data.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new CreditAccumulationChartService(repository, metricNodeResolver);
            results = service.Get(new CreditAccumulationChartRequest
                                        {
                                            StudentUSI = suppliedStudentUSI,
                                            SchoolId = suppliedSchoolId,
                                            MetricVariantId = suppliedMetricId
                                        });
        }

        [Test]
        public void Should_return_null()
        {
            Assert.That(results, Is.Null);
        }
    }
    
    public class When_getting_credit_accumulation_chart_with_missing_data : When_getting_credit_accumulation_chart_base
    {
        private IRepository<StudentMetricCreditAccumulation> repository;
        private IMetricNodeResolver metricNodeResolver;

        private IQueryable<StudentMetricCreditAccumulation> suppliedData;
        private const int suppliedStudentUSI = 1000;
        private const int suppliedSchoolId = 2000;

        private const string suppliedGradeLevel2 = "apple";
        private const int suppliedMetricState2 = 1;
        private const decimal suppliedCumulative2 = 7m;
        private const decimal suppliedRecommended2 = 2m;
        private const decimal suppliedMinimum2 = 6m;
        private const int suppliedSchoolYear2 = 2011;

        private const string suppliedGradeLevel4 = "fire";
        private const decimal suppliedRecommended4 = 4m;
        private const decimal suppliedMinimum4 = 5m;
        private const int suppliedSchoolYear4 = 2013;

        private ChartData results;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            suppliedData = GetData();
            repository = mocks.StrictMock<IRepository<StudentMetricCreditAccumulation>>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            Expect.Call(metricNodeResolver.GetMetricNodeForStudentFromMetricVariantId(suppliedSchoolId, suppliedMetricId)).Return(GetSuppliedMetricMetadataNode());

            Expect.Call(repository.GetAll()).Return(suppliedData);
        }

        protected IQueryable<StudentMetricCreditAccumulation> GetData()
        {
            var data = new List<StudentMetricCreditAccumulation>
                           {
                               new StudentMetricCreditAccumulation {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId + 1, MetricId=(int) StudentMetricEnum.CreditAccumulation, GradeLevel = "wrong"},
                               new StudentMetricCreditAccumulation {StudentUSI = suppliedStudentUSI + 1, SchoolId = suppliedSchoolId, MetricId=(int) StudentMetricEnum.CreditAccumulation, GradeLevel = "wrong"},
                               new StudentMetricCreditAccumulation {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId=(int) StudentMetricEnum.CreditAccumulation + 1, GradeLevel = "wrong"},
                               new StudentMetricCreditAccumulation {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId=(int) StudentMetricEnum.CreditAccumulation, SchoolYear = suppliedSchoolYear4, GradeLevel = suppliedGradeLevel4, MinimumCredits = suppliedMinimum4, RecommendedCredits = suppliedRecommended4},
                               new StudentMetricCreditAccumulation {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId=(int) StudentMetricEnum.CreditAccumulation, SchoolYear = suppliedSchoolYear2, GradeLevel = suppliedGradeLevel2, MetricStateTypeId = suppliedMetricState2, CumulativeCredits = suppliedCumulative2, MinimumCredits = suppliedMinimum2, RecommendedCredits = suppliedRecommended2},
                           };
            return data.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new CreditAccumulationChartService(repository, metricNodeResolver);
            results = service.Get(new CreditAccumulationChartRequest
                                            {
                                                StudentUSI = suppliedStudentUSI,
                                                SchoolId = suppliedSchoolId,
                                                MetricVariantId = suppliedMetricId
                                            });
        }

        [Test]
        public void Should_create_correct_number_of_series()
        {
            Assert.That(results.SeriesCollection.Count, Is.EqualTo(3));
        }

        [Test]
        public void Should_populate_student_points_correctly()
        {
            Assert.That(results.SeriesCollection[0], Is.TypeOf(typeof(ChartData.Series)));
            Assert.That(results.SeriesCollection[0].Name, Is.EqualTo("Actual"));
            Assert.That(results.SeriesCollection[0].ShowInLegend, Is.False);
            Assert.That(results.SeriesCollection[0].Points.Count, Is.EqualTo(2));
            Assert.That(results.SeriesCollection[0].Points[0].Value, Is.EqualTo(suppliedCumulative2));
            Assert.That(results.SeriesCollection[0].Points[1].Value, Is.EqualTo(0));
            Assert.That(results.SeriesCollection[0].Points[0].AxisLabel, Is.EqualTo(suppliedGradeLevel2));
            Assert.That(results.SeriesCollection[0].Points[1].AxisLabel, Is.EqualTo(suppliedGradeLevel4));
            Assert.That(results.SeriesCollection[0].Points[0].Tooltip, Is.EqualTo("7 credits"));
            Assert.That(results.SeriesCollection[0].Points[1].Tooltip, Is.EqualTo("0 credits"));
            Assert.That(results.SeriesCollection[0].Points[0].IsValueShownAsLabel, Is.False);
            Assert.That(results.SeriesCollection[0].Points[1].IsValueShownAsLabel, Is.False);
            Assert.That(results.SeriesCollection[0].Points[0].State, Is.EqualTo(MetricStateType.Good));
            Assert.That(results.SeriesCollection[0].Points[1].State, Is.EqualTo(MetricStateType.None));

        }

        [Test]
        public void Should_populate_recommended_points_correctly()
        {
            Assert.That(results.SeriesCollection[1], Is.TypeOf(typeof(ChartData.LineSeries)));
            Assert.That(results.SeriesCollection[1].Name, Is.EqualTo("Recommended"));
            Assert.That(results.SeriesCollection[1].ShowInLegend, Is.True);
            Assert.That(results.SeriesCollection[1].Points.Count, Is.EqualTo(2));
            Assert.That(results.SeriesCollection[1].Points[0].Value, Is.EqualTo(suppliedRecommended2));
            Assert.That(results.SeriesCollection[1].Points[1].Value, Is.EqualTo(suppliedRecommended4));
            Assert.That(results.SeriesCollection[1].Points[0].IsValueShownAsLabel, Is.True);
            Assert.That(results.SeriesCollection[1].Points[1].IsValueShownAsLabel, Is.True);
            Assert.That(results.SeriesCollection[1].Points[0].State, Is.EqualTo(MetricStateType.None));
            Assert.That(results.SeriesCollection[1].Points[1].State, Is.EqualTo(MetricStateType.None));
        }

        [Test]
        public void Should_populate_minimum_points_correctly()
        {
            Assert.That(results.SeriesCollection[2], Is.TypeOf(typeof(ChartData.LineSeries)));
            Assert.That(results.SeriesCollection[2].Name, Is.EqualTo("Minimum"));
            Assert.That(results.SeriesCollection[2].ShowInLegend, Is.True);
            Assert.That(results.SeriesCollection[2].Points.Count, Is.EqualTo(2));
            Assert.That(results.SeriesCollection[2].Points[0].Value, Is.EqualTo(suppliedMinimum2));
            Assert.That(results.SeriesCollection[2].Points[1].Value, Is.EqualTo(suppliedMinimum4));
            Assert.That(results.SeriesCollection[2].Points[0].IsValueShownAsLabel, Is.True);
            Assert.That(results.SeriesCollection[2].Points[1].IsValueShownAsLabel, Is.True);
            Assert.That(results.SeriesCollection[2].Points[0].State, Is.EqualTo(MetricStateType.None));
            Assert.That(results.SeriesCollection[2].Points[1].State, Is.EqualTo(MetricStateType.None));
        }
    }
}
