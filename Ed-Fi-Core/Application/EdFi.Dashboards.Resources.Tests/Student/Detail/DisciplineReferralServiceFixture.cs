// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.StudentSchool.Detail;
using NUnit.Framework;
using Rhino.Mocks;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.Student.Detail;
using EdFi.Dashboards.Testing;

namespace EdFi.Dashboards.Resources.Tests.Student.Detail
{
    public class When_loading_discipline_referrals : TestFixtureBase
    {
        private IRepository<StudentMetricDisciplineReferral> repository;
        private IMetricNodeResolver metricNodeResolver;

        private IQueryable<StudentMetricDisciplineReferral> suppliedData;
        private const int suppliedStudentUSI = 1000;
        private const int suppliedSchoolId = 2000;
        private const int suppliedMetricId = 98;
        private const string suppliedIncidentCode = "Good Data";
        private DateTime suppliedDate = new DateTime(2011, 12, 1);
        private const string suppliedIncidentDescription = "1000";
        private const string suppliedAction = "action";
        private IList<DisciplineReferralModel> actualModel;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            suppliedData = GetData();
            repository = mocks.StrictMock<IRepository<StudentMetricDisciplineReferral>>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            Expect.Call(metricNodeResolver.ResolveMetricId(suppliedMetricId)).Return(suppliedMetricId);
            Expect.Call(repository.GetAll()).Return(suppliedData);
        }

        protected IQueryable<StudentMetricDisciplineReferral> GetData()
        {
            var data = new List<StudentMetricDisciplineReferral>
                           {
                               new StudentMetricDisciplineReferral {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, IncidentCode = suppliedIncidentCode, Date=suppliedDate, IncidentDescription = "Yes", Action = "1000"},
                               new StudentMetricDisciplineReferral {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, IncidentCode = suppliedIncidentCode, Date= suppliedDate.AddDays(-1), IncidentDescription = suppliedIncidentDescription, Action = suppliedAction},
                                new StudentMetricDisciplineReferral {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 1, IncidentCode = "Bad Data", Date=new DateTime(2010, 12, 1), IncidentDescription = "Yes", Action = "1000"},
                               new StudentMetricDisciplineReferral {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId + 1, MetricId = suppliedMetricId, IncidentCode = "Bad Data", Date=new DateTime(2010, 12, 1), IncidentDescription = "Yes", Action = "1000"},
                               new StudentMetricDisciplineReferral {StudentUSI = suppliedStudentUSI + 1, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, IncidentCode = "Bad Data", Date=new DateTime(2010, 12, 1), IncidentDescription = "Yes", Action = "1000"},
                               new StudentMetricDisciplineReferral {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, IncidentCode = suppliedIncidentCode, Date=suppliedDate.AddDays(1), IncidentDescription = "Yes", Action = "1000"},
                           };
            return data.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new DisciplineReferralListService(repository, metricNodeResolver);
            actualModel = service.Get(new DisciplineReferralListRequest()
                                      {
                                          StudentUSI = suppliedStudentUSI,
                                          SchoolId = suppliedSchoolId,
                                          MetricVariantId = suppliedMetricId
                                      });
            //(suppliedStudentUSI, suppliedSchoolId, suppliedMetricId);
        }

        [Test]
        public void Should_create_correct_number_of_rows()
        {
            Assert.That(actualModel.Count, Is.EqualTo(3));
        }

        [Test]
        public void Should_select_correct_data()
        {
            Assert.That(actualModel[0].IncidentCode, Is.EqualTo(suppliedIncidentCode));
            Assert.That(actualModel[1].IncidentCode, Is.EqualTo(suppliedIncidentCode));
            Assert.That(actualModel[2].IncidentCode, Is.EqualTo(suppliedIncidentCode));
        }

        [Test]
        public void Should_sort_selected_data()
        {
            var startDate = DateTime.MinValue;

            foreach (var result in actualModel)
            {
                Assert.That(result.Date, Is.GreaterThan(startDate));
                startDate = result.Date;
            }
        }

        [Test]
        public void Should_bind_all_data_correctly()
        {
            Assert.That(actualModel[0].StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(actualModel[0].Date, Is.EqualTo(suppliedDate.AddDays(-1)));
            Assert.That(actualModel[0].IncidentCode, Is.EqualTo(suppliedIncidentCode));
            Assert.That(actualModel[0].IncidentDescription, Is.EqualTo(suppliedIncidentDescription));
            Assert.That(actualModel[0].Action, Is.EqualTo(suppliedAction));
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
