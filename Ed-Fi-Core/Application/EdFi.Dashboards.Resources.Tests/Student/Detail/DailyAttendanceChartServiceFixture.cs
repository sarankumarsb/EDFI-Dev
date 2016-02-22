// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class When_loading_daily_attendance_chart : TestFixtureBase
    {
        private IRepository<StudentMetricAttendanceRate> repository;
        private IMetricNodeResolver metricNodeResolver;

        private IQueryable<StudentMetricAttendanceRate> suppliedData;
        private const int suppliedStudentUSI = 1000;
        private const int suppliedSchoolId = 2000;
        private readonly decimal? suppliedAttendanceRate1 = .21m;
        private readonly decimal? suppliedExcusedRate1 = .31m;
        private readonly decimal? suppliedUnexcusedRate1 = .41m;
        private readonly decimal? suppliedAttendanceRate2 = .22m;
        private readonly decimal? suppliedExcusedRate2 = .32m;
        private readonly decimal? suppliedUnexcusedRate2 = .42m;
        private readonly decimal? suppliedAttendanceRate3 = 0m;
        private readonly decimal? suppliedExcusedRate3 = .34m;
        private readonly decimal? suppliedUnexcusedRate3 = .44m;
        private readonly int? suppliedAttendanceDays1 = 50;
        private readonly int? suppliedExcusedDays1 = 51;
        private readonly int? suppliedUnexcusedDays1 = 52;
        private readonly int? suppliedTotalDays1 = 53;
        private readonly int suppliedMetricId = 2;
        private const string suppliedChartTitle = "AttendanceRateChart";

        private const string suppliedContext1 = "apple context";
        private const string suppliedContext2 = "banana context";
        private const string suppliedContext3 = "carrot context";
        private const string suppliedContext4 = "durian context";
        private readonly decimal? wrongRate = .11m;
        private ChartData actualModel;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            suppliedData = GetData();
            repository = mocks.StrictMock<IRepository<StudentMetricAttendanceRate>>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            Expect.Call(metricNodeResolver.GetMetricNodeForStudentFromMetricVariantId(suppliedSchoolId, suppliedMetricId)).Return(GetSuppliedMetricMetadataNode());
            Expect.Call(repository.GetAll()).Return(suppliedData);
        }

        protected IQueryable<StudentMetricAttendanceRate> GetData()
        {
            var data = new List<StudentMetricAttendanceRate>
                           {
                               new StudentMetricAttendanceRate {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, PeriodSequence=2, Context=suppliedContext2, AttendanceRate=suppliedAttendanceRate2, ExcusedRate=suppliedExcusedRate2, UnexcusedRate=suppliedUnexcusedRate2, AttendanceDays=7, ExcusedDays = 8, UnexcusedDays = 9, TotalDays = 10},
                               new StudentMetricAttendanceRate {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, PeriodSequence=1, Context=suppliedContext1, AttendanceRate=suppliedAttendanceRate1, ExcusedRate=suppliedExcusedRate1, UnexcusedRate=suppliedUnexcusedRate1, AttendanceDays=suppliedAttendanceDays1, ExcusedDays = suppliedExcusedDays1, UnexcusedDays = suppliedUnexcusedDays1, TotalDays = suppliedTotalDays1},
                               new StudentMetricAttendanceRate {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, PeriodSequence=4, Context=suppliedContext4, AttendanceRate=null, ExcusedRate=null, UnexcusedRate=null},
                               new StudentMetricAttendanceRate {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, PeriodSequence=3, Context=suppliedContext3, AttendanceRate=suppliedAttendanceRate3, ExcusedRate=suppliedExcusedRate3, UnexcusedRate=suppliedUnexcusedRate3, AttendanceDays=7, ExcusedDays = 8, UnexcusedDays = 9, TotalDays = 10},
                               new StudentMetricAttendanceRate {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId + 1, MetricId = suppliedMetricId, PeriodSequence=1, Context=suppliedContext1, AttendanceRate=wrongRate, ExcusedRate=wrongRate, UnexcusedRate=wrongRate},
                               new StudentMetricAttendanceRate {StudentUSI = suppliedStudentUSI + 1, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, PeriodSequence=1, Context=suppliedContext1, AttendanceRate=wrongRate, ExcusedRate=wrongRate, UnexcusedRate=wrongRate},
                               new StudentMetricAttendanceRate {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 1000, PeriodSequence=2, Context=suppliedContext2, AttendanceRate=wrongRate, ExcusedRate=wrongRate, UnexcusedRate=wrongRate, AttendanceDays=7, ExcusedDays = 8, UnexcusedDays = 9, TotalDays = 10},
                           };
            return data.AsQueryable();
        }

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

        protected override void ExecuteTest()
        {
            var service = new DailyAttendanceChartService(repository, metricNodeResolver);
            actualModel = service.Get(new DailyAttendanceChartRequest()
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
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.SeriesCollection.Count, Is.EqualTo(3));
            Assert.That(actualModel.SeriesCollection[0].Name, Is.EqualTo("In Attendance"));
            Assert.That(actualModel.SeriesCollection[0].Style.BackgroundColor, Is.EqualTo(ChartColors.Green));
            Assert.That(actualModel.SeriesCollection[1].Name, Is.EqualTo("Excused Absence"));
			Assert.That(actualModel.SeriesCollection[1].Style.BackgroundColor, Is.EqualTo(ChartColors.Blue));
            Assert.That(actualModel.SeriesCollection[2].Name, Is.EqualTo("Unexcused Absence"));
			Assert.That(actualModel.SeriesCollection[2].Style.BackgroundColor, Is.EqualTo(ChartColors.Red));
            Assert.That(actualModel.StripLines.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_populate_points_correctly()
        {
            Assert.That(actualModel.SeriesCollection[0].Points.Count, Is.EqualTo(4));
            Assert.That(actualModel.SeriesCollection[1].Points.Count, Is.EqualTo(4));
            Assert.That(actualModel.SeriesCollection[2].Points.Count, Is.EqualTo(4));

            Assert.That(actualModel.SeriesCollection[0].Points[0].Label, Is.EqualTo(String.Format("{0:P1}", suppliedAttendanceRate1)));
            Assert.That(actualModel.SeriesCollection[0].Points[1].Label, Is.EqualTo(String.Format("{0:P1}", suppliedAttendanceRate2)));
            Assert.That(actualModel.SeriesCollection[0].Points[2].Label, Is.EqualTo(String.Format("{0:P1}", suppliedAttendanceRate3)));
            Assert.That(actualModel.SeriesCollection[0].Points[3].Label, Is.EqualTo(String.Format("{0:P1}", 0)));

            Assert.That(actualModel.SeriesCollection[1].Points[0].Label, Is.EqualTo(String.Format("{0:P1}", suppliedExcusedRate1)));
            Assert.That(actualModel.SeriesCollection[1].Points[1].Label, Is.EqualTo(String.Format("{0:P1}", suppliedExcusedRate2)));
            Assert.That(actualModel.SeriesCollection[1].Points[2].Label, Is.EqualTo(String.Format("{0:P1}", suppliedExcusedRate3)));
            Assert.That(actualModel.SeriesCollection[1].Points[3].Label, Is.EqualTo(String.Format("{0:P1}", 0)));

            Assert.That(actualModel.SeriesCollection[2].Points[0].Label, Is.EqualTo(String.Format("{0:P1}", suppliedUnexcusedRate1)));
            Assert.That(actualModel.SeriesCollection[2].Points[1].Label, Is.EqualTo(String.Format("{0:P1}", suppliedUnexcusedRate2)));
            Assert.That(actualModel.SeriesCollection[2].Points[2].Label, Is.EqualTo(String.Format("{0:P1}", suppliedUnexcusedRate3)));
            Assert.That(actualModel.SeriesCollection[1].Points[3].Label, Is.EqualTo(String.Format("{0:P1}", 0)));

            Assert.That(actualModel.SeriesCollection[0].Points[0].AxisLabel, Is.EqualTo(suppliedContext1));
            Assert.That(actualModel.SeriesCollection[1].Points[0].AxisLabel, Is.EqualTo(suppliedContext1));
            Assert.That(actualModel.SeriesCollection[2].Points[0].AxisLabel, Is.EqualTo(suppliedContext1));
            Assert.That(actualModel.SeriesCollection[0].Points[3].AxisLabel, Is.EqualTo(suppliedContext4));

            Assert.That(actualModel.SeriesCollection[0].Points[0].Value, Is.EqualTo(suppliedAttendanceRate1));
            Assert.That(actualModel.SeriesCollection[1].Points[0].Value, Is.EqualTo(suppliedExcusedRate1));
            Assert.That(actualModel.SeriesCollection[2].Points[0].Value, Is.EqualTo(suppliedUnexcusedRate1));
            Assert.That(actualModel.SeriesCollection[0].Points[3].Value, Is.EqualTo(0));

            Assert.That(actualModel.SeriesCollection[0].Points[0].Tooltip, Is.EqualTo(String.Format("Days In Attendance: {0} of {1}", suppliedAttendanceDays1, suppliedTotalDays1)));
            Assert.That(actualModel.SeriesCollection[1].Points[0].Tooltip, Is.EqualTo(String.Format("Excused Days: {0}", suppliedExcusedDays1)));
            Assert.That(actualModel.SeriesCollection[2].Points[0].Tooltip, Is.EqualTo(String.Format("Unexcused Days: {0}", suppliedUnexcusedDays1)));
            Assert.That(actualModel.SeriesCollection[0].Points[3].Tooltip, Is.EqualTo(String.Empty));

            Assert.That(actualModel.SeriesCollection[0].Points[0].IsValueShownAsLabel, Is.True);
            Assert.That(actualModel.SeriesCollection[1].Points[0].IsValueShownAsLabel, Is.True);
            Assert.That(actualModel.SeriesCollection[2].Points[0].IsValueShownAsLabel, Is.True);
            Assert.That(actualModel.SeriesCollection[0].Points[3].IsValueShownAsLabel, Is.False);
            Assert.That(actualModel.SeriesCollection[0].Points[2].IsValueShownAsLabel, Is.False);

            Assert.That(actualModel.SeriesCollection[0].Points[0].State, Is.EqualTo(MetricStateType.None));
            Assert.That(actualModel.SeriesCollection[0].Points[0].Trend, Is.EqualTo(TrendEvaluation.None));
        }

        [Test]
        public void Should_sort_periods()
        {
            var startPeriod = String.Empty;
            foreach(var series in actualModel.SeriesCollection)
            {
                foreach (var point in series.Points)
                {
                    Assert.That(point.AxisLabel, Is.GreaterThan(startPeriod));
                    startPeriod = point.AxisLabel;
                }
                startPeriod = String.Empty;
            }
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
            var suppliedTitle = GetSuppliedMetricMetadataNode().Actions.Single(x => x.Title == When_loading_daily_attendance_chart.suppliedChartTitle).DrilldownHeader;
            Assert.That(actualModel.ChartTitle, Is.EqualTo(suppliedTitle));
        }
    }
}
