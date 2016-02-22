using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Models.Student.Detail;
using EdFi.Dashboards.Resources.StudentSchool.Detail;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Student.Detail
{
    public class When_loading_days_absent_list : TestFixtureBase
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
        private readonly int? suppliedAttendanceDays2 = 54;
        private readonly int? suppliedExcusedDays2 = 55;
        private readonly int? suppliedUnexcusedDays2 = 56;
        private readonly int? suppliedTotalDays2 = 57;
        private readonly int? suppliedAttendanceDays3 = 58;
        private readonly int? suppliedExcusedDays3 = 59;
        private readonly int? suppliedUnexcusedDays3 = 60;
        private readonly int? suppliedTotalDays3 = 61;
        private readonly int suppliedMetricId = 2;

        private const string suppliedContext1 = "apple context";
        private const string suppliedContext2 = "banana context";
        private const string suppliedContext3 = "carrot context";
        private const string suppliedContext4 = "durian context";
        private readonly decimal? wrongRate = .11m;
        private readonly int? wrongDays = 9999;
        private IList<DaysAbsentModel> actualModel;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            suppliedData = GetData();
            repository = mocks.StrictMock<IRepository<StudentMetricAttendanceRate>>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            Expect.Call(metricNodeResolver.ResolveMetricId(suppliedMetricId)).Return(suppliedMetricId);
            Expect.Call(repository.GetAll()).Return(suppliedData);
        }


        protected IQueryable<StudentMetricAttendanceRate> GetData()
        {
            var data = new List<StudentMetricAttendanceRate>
                           {
                               new StudentMetricAttendanceRate {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, PeriodSequence=2, Context=suppliedContext2, AttendanceRate=suppliedAttendanceRate2, ExcusedRate=suppliedExcusedRate2, UnexcusedRate=suppliedUnexcusedRate2, AttendanceDays=suppliedAttendanceDays2, ExcusedDays = suppliedExcusedDays2, UnexcusedDays = suppliedUnexcusedDays2, TotalDays = suppliedTotalDays2},
                               new StudentMetricAttendanceRate {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, PeriodSequence=1, Context=suppliedContext1, AttendanceRate=suppliedAttendanceRate1, ExcusedRate=suppliedExcusedRate1, UnexcusedRate=suppliedUnexcusedRate1, AttendanceDays=suppliedAttendanceDays1, ExcusedDays = suppliedExcusedDays1, UnexcusedDays = suppliedUnexcusedDays1, TotalDays = suppliedTotalDays1},
                               new StudentMetricAttendanceRate {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, PeriodSequence=4, Context=suppliedContext4, AttendanceRate=null, ExcusedRate=null, UnexcusedRate=null, AttendanceDays=null, ExcusedDays = null, UnexcusedDays = null, TotalDays = null},
                               new StudentMetricAttendanceRate {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, PeriodSequence=3, Context=suppliedContext3, AttendanceRate=suppliedAttendanceRate3, ExcusedRate=suppliedExcusedRate3, UnexcusedRate=suppliedUnexcusedRate3, AttendanceDays=suppliedAttendanceDays3, ExcusedDays = suppliedExcusedDays3, UnexcusedDays = suppliedUnexcusedDays3, TotalDays = suppliedTotalDays3},
                               new StudentMetricAttendanceRate {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId + 1, MetricId = suppliedMetricId, PeriodSequence=1, Context=suppliedContext1, AttendanceRate=wrongRate, ExcusedRate=wrongRate, UnexcusedRate=wrongRate},
                               new StudentMetricAttendanceRate {StudentUSI = suppliedStudentUSI + 1, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, PeriodSequence=1, Context=suppliedContext1, AttendanceRate=wrongRate, ExcusedRate=wrongRate, UnexcusedRate=wrongRate},
                               new StudentMetricAttendanceRate {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 1000, PeriodSequence=2, Context=suppliedContext2, AttendanceRate=wrongRate, ExcusedRate=wrongRate, UnexcusedRate=wrongRate, AttendanceDays=wrongDays, ExcusedDays = wrongDays, UnexcusedDays = wrongDays, TotalDays = wrongDays},
                           };
            return data.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new DaysAbsentListService(repository, metricNodeResolver);
            actualModel = service.Get(DaysAbsentListRequest.Create(suppliedStudentUSI, suppliedSchoolId, suppliedMetricId));
        }
        
        [Test]
        public void Should_return_correct_number_of_results()
        {
            Assert.That(actualModel.Count, Is.EqualTo(4));
        }

        [Test]
        public void Should_load_results_correctly()
        {
            var result = actualModel[0];
            Assert.That(result.StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(result.SchoolId, Is.EqualTo(suppliedSchoolId));
            Assert.That(result.Context, Is.EqualTo(suppliedContext1));
            Assert.That(result.TotalDays, Is.EqualTo(suppliedTotalDays1));
            Assert.That(result.AttendanceDays, Is.EqualTo(suppliedAttendanceDays1));
            Assert.That(result.ExcusedDays, Is.EqualTo(suppliedExcusedDays1));
            Assert.That(result.UnexcusedDays, Is.EqualTo(suppliedUnexcusedDays1));

            result = actualModel[1];
            Assert.That(result.StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(result.SchoolId, Is.EqualTo(suppliedSchoolId));
            Assert.That(result.Context, Is.EqualTo(suppliedContext2));
            Assert.That(result.TotalDays, Is.EqualTo(suppliedTotalDays2));
            Assert.That(result.AttendanceDays, Is.EqualTo(suppliedAttendanceDays2));
            Assert.That(result.ExcusedDays, Is.EqualTo(suppliedExcusedDays2));
            Assert.That(result.UnexcusedDays, Is.EqualTo(suppliedUnexcusedDays2));

            result = actualModel[2];
            Assert.That(result.StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(result.SchoolId, Is.EqualTo(suppliedSchoolId));
            Assert.That(result.Context, Is.EqualTo(suppliedContext3));
            Assert.That(result.TotalDays, Is.EqualTo(suppliedTotalDays3));
            Assert.That(result.AttendanceDays, Is.EqualTo(suppliedAttendanceDays3));
            Assert.That(result.ExcusedDays, Is.EqualTo(suppliedExcusedDays3));
            Assert.That(result.UnexcusedDays, Is.EqualTo(suppliedUnexcusedDays3));

            result = actualModel[3];
            Assert.That(result.StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(result.SchoolId, Is.EqualTo(suppliedSchoolId));
            Assert.That(result.Context, Is.EqualTo(suppliedContext4));
            Assert.That(result.TotalDays, Is.Null);
            Assert.That(result.AttendanceDays, Is.Null);
            Assert.That(result.ExcusedDays, Is.Null);
            Assert.That(result.UnexcusedDays, Is.Null);
        }

        [Test]
        public void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel[0].EnsureNoDefaultValues();
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }
}
