// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Models.Student.Detail;
using EdFi.Dashboards.Resources.StudentSchool.Detail;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Student.Detail
{
    public class When_loading_metric_objectives : TestFixtureBase
    {
        private IRepository<StudentMetricObjective> repository;
        private IMetricNodeResolver metricNodeResolver;

        private IQueryable<StudentMetricObjective> suppliedData;
        private const int suppliedStudentUSI = 1000;
        private const int suppliedSchoolId = 2000;
        private const int suppliedMetricId = 88;
        private const string suppliedBadDescription = "Bad data";

        private const string suppliedDescription1 = "ace";
        private const bool suppliedFlag1 = false;
        private const string suppliedValue1 = "value 1";
        private const int suppliedObjectiveItem1 = 4;
        private const int suppliedMetricStateId1 = 6;

        private const string suppliedDescription2 = "ace";
        private const bool suppliedFlag2 = true;
        private const string suppliedValue2 = "value 2";
        private const int suppliedObjectiveItem2 = 1;
        private const int suppliedMetricStateId2 = 3;

        private const string suppliedDescription3 = "apple";
        private const string suppliedValue3 = "value 3";
        private const int suppliedObjectiveItem3 = 1;

        private IList<MetricObjectiveModel> actualModel;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            suppliedData = GetData();
            repository = mocks.StrictMock<IRepository<StudentMetricObjective>>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            Expect.Call(metricNodeResolver.ResolveMetricId(suppliedMetricId)).Return(suppliedMetricId);
            Expect.Call(repository.GetAll()).Return(suppliedData);
        }

        protected IQueryable<StudentMetricObjective> GetData()
        {
            var data = new List<StudentMetricObjective>
                           {
                               new StudentMetricObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, Description = suppliedDescription1, Value=suppliedValue1, Flag=suppliedFlag1, ObjectiveItem = suppliedObjectiveItem1, MetricStateTypeId=suppliedMetricStateId1 },
                               new StudentMetricObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, Description = suppliedDescription3, Value=suppliedValue3, ObjectiveItem = suppliedObjectiveItem3},
                               new StudentMetricObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 1, Description = suppliedBadDescription },
                               new StudentMetricObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId + 1, MetricId = suppliedMetricId, Description = suppliedBadDescription },
                               new StudentMetricObjective {StudentUSI = suppliedStudentUSI + 1, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, Description = suppliedBadDescription},
                               new StudentMetricObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, Description = suppliedDescription2, Value=suppliedValue2, Flag=suppliedFlag2, ObjectiveItem = suppliedObjectiveItem2, MetricStateTypeId=suppliedMetricStateId2},
                           };
            return data.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new MetricObjectivesListService(repository, metricNodeResolver);
            actualModel = service.Get(new MetricObjectivesListRequest()
                                      {
                                          StudentUSI = suppliedStudentUSI,
                                          SchoolId = suppliedSchoolId,
                                          MetricVariantId = suppliedMetricId
                                      });
        }

        [Test]
        public void Should_create_correct_number_of_rows()
        {
            Assert.That(actualModel.Count, Is.EqualTo(3));
        }

        [Test]
        public void Should_select_correct_data_in_order()
        {
            Assert.That(actualModel[0].Description, Is.EqualTo(suppliedDescription2));
            Assert.That(actualModel[1].Description, Is.EqualTo(suppliedDescription3));
            Assert.That(actualModel[2].Description, Is.EqualTo(suppliedDescription1));
        }

        [Test]
        public void Should_bind_all_data_correctly()
        {
            Assert.That(actualModel[0].StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(actualModel[0].Description, Is.EqualTo(suppliedDescription2));
            Assert.That(actualModel[0].Value, Is.EqualTo(suppliedValue2));
            Assert.That(actualModel[0].State.StateType, Is.EqualTo(MetricStateType.Low));
            Assert.That(actualModel[0].IsFlagged, Is.True);

            Assert.That(actualModel[1].StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(actualModel[1].Description, Is.EqualTo(suppliedDescription3));
            Assert.That(actualModel[1].Value, Is.EqualTo(suppliedValue3));
            Assert.That(actualModel[1].State.StateType, Is.EqualTo(MetricStateType.NoData));
            Assert.That(actualModel[1].IsFlagged, Is.False);
        }

        [Test]
        public void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel[0].EnsureNoDefaultValues("MetricObjectiveModel.State.StateText",
                "MetricObjectiveModel.State.MaxValue",
                "MetricObjectiveModel.State.IsMaxValueInclusive",
                "MetricObjectiveModel.State.MinValue",
                "MetricObjectiveModel.State.IsMinValueInclusive",
                "MetricObjectiveModel.State.Format");
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }
}
