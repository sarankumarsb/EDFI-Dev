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
    public class When_loading_college_career_assessments : When_loading_college_career_assessmentsBase<CollegeReadinessAssessmentListRequest, CollegeCareerReadinessAssessmentModel,
        StudentMetricCollegeReadinessAssessment,
        CollegeReadinessAssessmentListService> { }

    public abstract class When_loading_college_career_assessmentsBase<TRequest, TResponse, TEntity, TService> : TestFixtureBase
        where TRequest : CollegeReadinessAssessmentListRequest
        where TResponse : CollegeCareerReadinessAssessmentModel, new()
        where TEntity : StudentMetricCollegeReadinessAssessment, new()
        where TService : CollegeReadinessAssessmentListServiceBase<TRequest, TResponse>, new()

    {
        protected IRepository<TEntity> Repository;
        protected IMetricNodeResolver MetricNodeResolver;

        protected IQueryable<TEntity> SuppliedData;
        protected const int SuppliedStudentUSI = 1000;
        protected const int SuppliedSchoolId = 2000;
        protected const int SuppliedMetricId = 98;
        protected const string SuppliedSubject = "Good Data";
        protected DateTime SuppliedDate = new DateTime(2011, 12, 1);
        protected const string SuppliedScore = "1000";
        protected const string SuppliedStateCriteria = "fire";
        protected IEnumerable<TResponse> ActualModel;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            SuppliedData = GetData();
            Repository = mocks.StrictMock<IRepository<TEntity>>();
            MetricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            Expect.Call(MetricNodeResolver.ResolveMetricId(SuppliedMetricId)).Return(SuppliedMetricId);
            Expect.Call(Repository.GetAll()).Return(SuppliedData);
        }

        protected virtual TEntity StudentMetricCollegeReadinessAssessmentFactory(long studentUSI, int schoolId, int metricId, string subject, DateTime date, string stateCriteria, string score, bool flag)
        {
            return new TEntity {StudentUSI = studentUSI, SchoolId = schoolId, MetricId = metricId, Subject = subject, Date=date, StateCriteria = stateCriteria, Score = score, Flag = flag};
        }

        protected virtual IQueryable<TEntity> GetData()
        {
            var data = new List<TEntity>
                           {
                               StudentMetricCollegeReadinessAssessmentFactory(SuppliedStudentUSI, SuppliedSchoolId, SuppliedMetricId, SuppliedSubject, SuppliedDate, "Yes", "1000", true),
                               StudentMetricCollegeReadinessAssessmentFactory(SuppliedStudentUSI, SuppliedSchoolId, SuppliedMetricId, SuppliedSubject, SuppliedDate.AddDays(-1), SuppliedStateCriteria, SuppliedScore, true),
                               StudentMetricCollegeReadinessAssessmentFactory(SuppliedStudentUSI, SuppliedSchoolId, SuppliedMetricId + 1, "Bad Data",new DateTime(2010, 12, 1), "Yes", "1000", true),
                               StudentMetricCollegeReadinessAssessmentFactory(SuppliedStudentUSI, SuppliedSchoolId + 1, SuppliedMetricId, "Bad Data",new DateTime(2010, 12, 1), "Yes", "1000", true),
                               StudentMetricCollegeReadinessAssessmentFactory(SuppliedStudentUSI + 1, SuppliedSchoolId, SuppliedMetricId, "Bad Data",new DateTime(2010, 12, 1), "Yes", "1000", true),
                               StudentMetricCollegeReadinessAssessmentFactory(SuppliedStudentUSI, SuppliedSchoolId, SuppliedMetricId, SuppliedSubject, SuppliedDate.AddDays(1), "Yes", "1000", false),
                           };
            return data.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new TService { MetricNodeResolver = MetricNodeResolver, StudentMetricCollegeReadinessAssessmentRepository = Repository};
            ActualModel = service.Get(CreateRequest());
        }

        protected virtual TRequest CreateRequest()
        {
            return (TRequest) new CollegeReadinessAssessmentListRequest
                           {
                               StudentUSI = SuppliedStudentUSI,
                               SchoolId = SuppliedSchoolId,
                               MetricVariantId = SuppliedMetricId
                           };
        }

        [Test]
        public void Should_create_correct_number_of_rows()
        {
            Assert.That(ActualModel.Count(), Is.EqualTo(3));
        }

        [Test]
        public void Should_select_correct_data()
        {
            Assert.That(ActualModel.ElementAt(0).Subject, Is.EqualTo(SuppliedSubject));
            Assert.That(ActualModel.ElementAt(1).Subject, Is.EqualTo(SuppliedSubject));
            Assert.That(ActualModel.ElementAt(2).Subject, Is.EqualTo(SuppliedSubject));
        }

        [Test]
        public void Should_sort_selected_data()
        {
            var startDate = DateTime.MinValue;

            foreach (var result in ActualModel)
            {
                Assert.That(result.Date, Is.GreaterThan(startDate));
                startDate = result.Date;
            }
        }

        [Test]
        public void Should_bind_all_data_correctly()
        {
            Assert.That(ActualModel.ElementAt(0).StudentUSI, Is.EqualTo(SuppliedStudentUSI));
            Assert.That(ActualModel.ElementAt(0).Date, Is.EqualTo(SuppliedDate.AddDays(-1)));
            Assert.That(ActualModel.ElementAt(0).Subject, Is.EqualTo(SuppliedSubject));
            Assert.That(ActualModel.ElementAt(0).Score, Is.EqualTo(SuppliedScore));
            Assert.That(ActualModel.ElementAt(0).StateCriteria, Is.EqualTo(SuppliedStateCriteria));
            Assert.That(ActualModel.ElementAt(0).IsFlagged, Is.True);
            Assert.That(ActualModel.ElementAt(2).IsFlagged, Is.False);
        }

        [Test]
        public void Should_have_no_unassigned_values_on_presentation_model()
        {
            ActualModel.ElementAt(0).EnsureNoDefaultValues();
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            ActualModel.EnsureSerializableModel();
        }
    }
}
