// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Resources.StudentSchool.Detail;
using NUnit.Framework;
using Rhino.Mocks;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.Student.Detail;
using EdFi.Dashboards.Testing;

namespace EdFi.Dashboards.Resources.Tests.Student.Detail
{
    public class When_getting_class_absences : TestFixtureBase
    {
        private IRepository<StudentMetricAbsencesByClass> repository;
        private IQueryable<StudentMetricAbsencesByClass> suppliedData;
        private const int suppliedStudentUSI = 1000;
        private const int suppliedSchoolId = 2000;
        private const string suppliedSubjectArea1 = "subject A";
        private const string suppliedSubjectArea2 = "subject B";
        private const string suppliedSubjectArea3 = "subject E";
        private const string suppliedSubjectArea4 = "bad data";
        private const string suppliedDay1 = "Tuesday";
        private const string suppliedDay2 = "Wednesday";
        private const string suppliedDay3 = "Friday";


        private readonly DateTime suppliedDate1 = new DateTime(2011, 9, 6); // Tuesday
        private readonly DateTime suppliedDate2 = new DateTime(2011, 9, 7); // Wednesday
        private readonly DateTime suppliedDate3 = new DateTime(2011, 9, 9); // Friday
        private readonly DateTime suppliedDate4 = new DateTime(2011, 9, 12);
        private readonly DateTime suppliedDate5 = new DateTime(2011, 9, 13);
        private readonly DateTime suppliedDate6 = new DateTime(2011, 9, 14);
        private readonly DateTime suppliedDate7 = new DateTime(2011, 9, 15);
        private readonly DateTime suppliedDate8 = new DateTime(2011, 9, 16);
        private const ClassAbsencesModel.AttendanceEvent suppliedEvent1 = ClassAbsencesModel.AttendanceEvent.Present;
        private const ClassAbsencesModel.AttendanceEvent suppliedEvent2 = ClassAbsencesModel.AttendanceEvent.Excused;
        private const ClassAbsencesModel.AttendanceEvent suppliedEvent3 = ClassAbsencesModel.AttendanceEvent.Tardy;
        private const string suppliedReason1 = "good data1";
        private const string suppliedReason2 = "good data2";
        private const string suppliedReason3 = "good data3";
        private ClassAbsencesModel actualModel;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            suppliedData = GetData();
            repository = mocks.StrictMock<IRepository<StudentMetricAbsencesByClass>>();
            Expect.Call(repository.GetAll()).Return(suppliedData);
        }

        protected IQueryable<StudentMetricAbsencesByClass> GetData()
        {
            var data = new List<StudentMetricAbsencesByClass>
                           {
                               new StudentMetricAbsencesByClass{StudentUSI=suppliedStudentUSI, SchoolId = suppliedSchoolId, SubjectArea=suppliedSubjectArea2, DayOfTheWeek=suppliedDay1, DateValue=suppliedDate1, AttendanceEventDescriptorTypeId=(int)suppliedEvent1, AttendanceEventReason=suppliedReason1},
                               new StudentMetricAbsencesByClass{StudentUSI=suppliedStudentUSI, SchoolId = suppliedSchoolId, SubjectArea=suppliedSubjectArea3, DayOfTheWeek=suppliedDay1, DateValue=suppliedDate1, AttendanceEventDescriptorTypeId=(int)suppliedEvent1, AttendanceEventReason=suppliedReason1},
                               new StudentMetricAbsencesByClass{StudentUSI=suppliedStudentUSI, SchoolId = suppliedSchoolId, SubjectArea=suppliedSubjectArea1, DayOfTheWeek=suppliedDay3, DateValue=suppliedDate3, AttendanceEventDescriptorTypeId=(int)suppliedEvent3, AttendanceEventReason=suppliedReason3},
                               new StudentMetricAbsencesByClass{StudentUSI=suppliedStudentUSI+1, SchoolId = suppliedSchoolId, SubjectArea=suppliedSubjectArea4, DayOfTheWeek=suppliedDay1, DateValue=suppliedDate1, AttendanceEventDescriptorTypeId=(int)suppliedEvent1, AttendanceEventReason=suppliedReason1},
                               new StudentMetricAbsencesByClass{StudentUSI=suppliedStudentUSI, SchoolId = suppliedSchoolId+1, SubjectArea=suppliedSubjectArea4, DayOfTheWeek=suppliedDay1, DateValue=suppliedDate1, AttendanceEventDescriptorTypeId=(int)suppliedEvent1, AttendanceEventReason=suppliedReason1},
                               
                               new StudentMetricAbsencesByClass{StudentUSI=suppliedStudentUSI, SchoolId = suppliedSchoolId, SubjectArea=suppliedSubjectArea1, DayOfTheWeek=suppliedDay1, DateValue=suppliedDate1, AttendanceEventDescriptorTypeId=(int)suppliedEvent1, AttendanceEventReason=suppliedReason1},
                               new StudentMetricAbsencesByClass{StudentUSI=suppliedStudentUSI, SchoolId = suppliedSchoolId, SubjectArea=suppliedSubjectArea1, DayOfTheWeek=suppliedDay2, DateValue=suppliedDate2, AttendanceEventDescriptorTypeId=(int)suppliedEvent2, AttendanceEventReason=suppliedReason2},

                               new StudentMetricAbsencesByClass{StudentUSI=suppliedStudentUSI, SchoolId = suppliedSchoolId, SubjectArea=suppliedSubjectArea1, DayOfTheWeek=suppliedDate8.DayOfWeek.ToString(), DateValue=suppliedDate8, AttendanceEventDescriptorTypeId=(int)suppliedEvent2, AttendanceEventReason=suppliedReason2},
                               new StudentMetricAbsencesByClass{StudentUSI=suppliedStudentUSI, SchoolId = suppliedSchoolId, SubjectArea=suppliedSubjectArea1, DayOfTheWeek=suppliedDate4.DayOfWeek.ToString(), DateValue=suppliedDate4, AttendanceEventDescriptorTypeId=(int)suppliedEvent2, AttendanceEventReason=suppliedReason2},
                               new StudentMetricAbsencesByClass{StudentUSI=suppliedStudentUSI, SchoolId = suppliedSchoolId, SubjectArea=suppliedSubjectArea1, DayOfTheWeek=suppliedDate5.DayOfWeek.ToString(), DateValue=suppliedDate5, AttendanceEventDescriptorTypeId=(int)suppliedEvent2, AttendanceEventReason=suppliedReason2},
                               new StudentMetricAbsencesByClass{StudentUSI=suppliedStudentUSI, SchoolId = suppliedSchoolId, SubjectArea=suppliedSubjectArea1, DayOfTheWeek=suppliedDate7.DayOfWeek.ToString(), DateValue=suppliedDate7, AttendanceEventDescriptorTypeId=(int)suppliedEvent2, AttendanceEventReason=suppliedReason2},
                               new StudentMetricAbsencesByClass{StudentUSI=suppliedStudentUSI, SchoolId = suppliedSchoolId, SubjectArea=suppliedSubjectArea1, DayOfTheWeek=suppliedDate6.DayOfWeek.ToString(), DateValue=suppliedDate6, AttendanceEventDescriptorTypeId=(int)suppliedEvent2, AttendanceEventReason=suppliedReason2}
                           };
            return data.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new ClassAbsencesService
                              {
                                Repository = repository  
                              };
            actualModel = service.Get(new ClassAbsencesRequest()
                                      {
                                          StudentUSI = suppliedStudentUSI,
                                          SchoolId = suppliedSchoolId
                                      });
        }

        [Test]
        public void Should_create_correct_model()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(actualModel.Classes.Count, Is.EqualTo(3));
            Assert.That(actualModel.Classes.Count(x => x.Name == suppliedSubjectArea4), Is.EqualTo(0), "Found a subject area that should be excluded with data filters");
        }

        [Test]
        public void Should_sort_classes_correctly()
        {
            var previousSubjectArea = String.Empty;
            foreach(var c in actualModel.Classes)
            {
                Assert.That(c.Name, Is.GreaterThan(previousSubjectArea));
                previousSubjectArea = c.Name;
            }
        }

        [Test]
        public void Should_build_weeks_correctly()
        {
            var endDate = DateTime.MinValue;

            foreach (var week in actualModel.Classes[0].Weeks)
            {
                Assert.That(week.WeekDayEvents.Count, Is.EqualTo(5));
                Assert.That(week.StartDate, Is.LessThanOrEqualTo(week.EndDate));
                Assert.That(week.StartDate, Is.GreaterThan(endDate));
                endDate = week.EndDate;
            }
        }

        [Test]
        public void Should_build_days_correctly()
        {
            var day1 = actualModel.Classes[0].Weeks[0].WeekDayEvents[0];
            Assert.That(day1.Date, Is.EqualTo(DateTime.MinValue));
            Assert.That(day1.AttendanceEventType, Is.EqualTo(ClassAbsencesModel.AttendanceEvent.NoData));
            Assert.That(day1.Reason, Is.Null);

            var day2 = actualModel.Classes[0].Weeks[0].WeekDayEvents[1];
            Assert.That(day2.Date, Is.EqualTo(suppliedDate1));
            Assert.That(day2.AttendanceEventType, Is.EqualTo(suppliedEvent1));
            Assert.That(day2.Reason, Is.EqualTo(suppliedReason1));

            var day3 = actualModel.Classes[0].Weeks[0].WeekDayEvents[2];
            Assert.That(day3.Date, Is.EqualTo(suppliedDate2));
            Assert.That(day3.AttendanceEventType, Is.EqualTo(suppliedEvent2));
            Assert.That(day3.Reason, Is.EqualTo(suppliedReason2));

            var day4 = actualModel.Classes[0].Weeks[0].WeekDayEvents[3];
            Assert.That(day4.Date, Is.EqualTo(DateTime.MinValue));
            Assert.That(day4.AttendanceEventType, Is.EqualTo(ClassAbsencesModel.AttendanceEvent.NoData));
            Assert.That(day4.Reason, Is.Null);

            var day5 = actualModel.Classes[0].Weeks[0].WeekDayEvents[4];
            Assert.That(day5.Date, Is.EqualTo(suppliedDate3));
            Assert.That(day5.AttendanceEventType, Is.EqualTo(suppliedEvent3));
            Assert.That(day5.Reason, Is.EqualTo(suppliedReason3));
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }
}
