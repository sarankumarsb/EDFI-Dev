// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class When_loading_learning_standard_data_with_one_assessment_two_standards : TestFixtureBase
    {
        private IRepository<StudentMetricLearningStandard> repository;
        private IMetricNodeResolver metricNodeResolver;

        private IQueryable<StudentMetricLearningStandard> suppliedData;
        private const int suppliedStudentUSI = 1000;
        private const int suppliedSchoolId = 2000;
        private const int suppliedMetricId = 1230;

        private readonly DateTime suppliedDate1 = new DateTime(2010, 12, 1);
        private const string suppliedAssessmentTitle1 = "Assessment1";
        private const int suppliedVersion1 = 1;
        private const string suppliedLearningStandard11 = "LS1.1";
        private const string suppliedLearningStandard12 = "LS1.2";
        private const string suppliedDescription11 = "description 1.1";
        private const string suppliedDescription12 = "description 1.2";
        private const int suppliedMetricState11 = 1;
        private const int suppliedMetricState12 = 3;
        private const string suppliedValue11 = "value 1";
        private const string suppliedValue12 = "value 1.2";


        private IList<LearningStandardModel> actualModel;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            suppliedData = GetData();
            repository = mocks.StrictMock<IRepository<StudentMetricLearningStandard>>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            Expect.Call(metricNodeResolver.ResolveMetricId(suppliedMetricId)).Return(suppliedMetricId);
            Expect.Call(repository.GetAll()).Return(suppliedData);
        }

        protected IQueryable<StudentMetricLearningStandard> GetData()
        {
            var data = new List<StudentMetricLearningStandard>
                           {
                               new StudentMetricLearningStandard {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 1},
                               new StudentMetricLearningStandard {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId + 1, MetricId = suppliedMetricId},
                               new StudentMetricLearningStandard {StudentUSI = suppliedStudentUSI + 1, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId},
                               new StudentMetricLearningStandard {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, DateAdministration = suppliedDate1, AssessmentTitle = suppliedAssessmentTitle1, Version = suppliedVersion1, LearningStandard = suppliedLearningStandard11, Description = suppliedDescription11, MetricStateTypeId = suppliedMetricState11, Value = suppliedValue11  },
                               new StudentMetricLearningStandard {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, DateAdministration = suppliedDate1, AssessmentTitle = suppliedAssessmentTitle1, Version = suppliedVersion1, LearningStandard = suppliedLearningStandard12, Description = suppliedDescription12, MetricStateTypeId = suppliedMetricState12, Value = suppliedValue12  },
                           };
            return data.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new LearningStandardService(repository, metricNodeResolver);
            actualModel = service.Get(new LearningStandardRequest()
                                      {
                                          StudentUSI = suppliedStudentUSI,
                                          SchoolId = suppliedSchoolId,
                                          MetricVariantId = suppliedMetricId
                                      });
        }

        [Test]
        public void Should_create_correct_number_of_rows()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.Count, Is.EqualTo(2));
            Assert.That(actualModel[0].Assessments.Count, Is.EqualTo(1));
            Assert.That(actualModel[1].Assessments.Count, Is.EqualTo(1));
        }

        [Test]
        public void Should_select_and_bind_correct_data()
        {
            Assert.That(actualModel[0].StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(actualModel[0].MetricId, Is.EqualTo(suppliedMetricId));
            Assert.That(actualModel[0].SchoolId, Is.EqualTo(suppliedSchoolId));
            Assert.That(actualModel[0].LearningStandard, Is.EqualTo(suppliedLearningStandard11));
            Assert.That(actualModel[0].Description, Is.EqualTo(suppliedDescription11));
            Assert.That(actualModel[0].Assessments[0].StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(actualModel[0].Assessments[0].DateAdministration, Is.EqualTo(suppliedDate1));
            Assert.That(actualModel[0].Assessments[0].MetricStateTypeId, Is.EqualTo(suppliedMetricState11));
            Assert.That(actualModel[0].Assessments[0].Value, Is.EqualTo(suppliedValue11));
            Assert.That(actualModel[0].Assessments[0].AssessmentTitle, Is.EqualTo(suppliedAssessmentTitle1));
            Assert.That(actualModel[0].Assessments[0].Version, Is.EqualTo(suppliedVersion1));
            Assert.That(actualModel[0].Assessments[0].Administered, Is.True);


            Assert.That(actualModel[1].StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(actualModel[1].MetricId, Is.EqualTo(suppliedMetricId));
            Assert.That(actualModel[1].SchoolId, Is.EqualTo(suppliedSchoolId));
            Assert.That(actualModel[1].LearningStandard, Is.EqualTo(suppliedLearningStandard12));
            Assert.That(actualModel[1].Description, Is.EqualTo(suppliedDescription12));
            Assert.That(actualModel[1].Assessments[0].StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(actualModel[1].Assessments[0].DateAdministration, Is.EqualTo(suppliedDate1));
            Assert.That(actualModel[1].Assessments[0].MetricStateTypeId, Is.EqualTo(suppliedMetricState12));
            Assert.That(actualModel[1].Assessments[0].Value, Is.EqualTo(suppliedValue12));
            Assert.That(actualModel[1].Assessments[0].AssessmentTitle, Is.EqualTo(suppliedAssessmentTitle1));
            Assert.That(actualModel[1].Assessments[0].Version, Is.EqualTo(suppliedVersion1));
            Assert.That(actualModel[1].Assessments[0].Administered, Is.True);
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

    public class When_loading_learning_standard_data_with_one_standard_two_tests : TestFixtureBase
    {
        private IRepository<StudentMetricLearningStandard> repository;
        private IMetricNodeResolver metricNodeResolver;

        private IQueryable<StudentMetricLearningStandard> suppliedData;
        private const int suppliedStudentUSI = 1000;
        private const int suppliedSchoolId = 2000;
        private const int suppliedMetricId = 1230;

        private readonly DateTime suppliedDate1 = new DateTime(2010, 12, 1);
        private readonly DateTime suppliedDate2 = new DateTime(2010, 12, 2);
        private const string suppliedAssessmentTitle1 = "Assessment1";
        private const string suppliedAssessmentTitle2 = "Assessment2";
        private const int suppliedVersion1 = 1;
        private const int suppliedVersion2 = 2;
        private const string suppliedLearningStandard1 = "LS1";
        private const string suppliedDescription1 = "description 1";
        private const int suppliedMetricState11 = 1;
        private const int suppliedMetricState12 = 3;
        private const string suppliedValue1 = "value 1";
        private const string suppliedValue2 = "value 2";


        private IList<LearningStandardModel> results;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            suppliedData = GetData();
            repository = mocks.StrictMock<IRepository<StudentMetricLearningStandard>>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            Expect.Call(metricNodeResolver.ResolveMetricId(suppliedMetricId)).Return(suppliedMetricId);
            Expect.Call(repository.GetAll()).Return(suppliedData);
        }

        protected IQueryable<StudentMetricLearningStandard> GetData()
        {
            var data = new List<StudentMetricLearningStandard>
                           {
                               new StudentMetricLearningStandard {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 1},
                               new StudentMetricLearningStandard {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId + 1, MetricId = suppliedMetricId},
                               new StudentMetricLearningStandard {StudentUSI = suppliedStudentUSI + 1, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId},
                               new StudentMetricLearningStandard {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, DateAdministration = suppliedDate2, AssessmentTitle = suppliedAssessmentTitle2, Version = suppliedVersion2, LearningStandard = suppliedLearningStandard1, Description = suppliedDescription1, MetricStateTypeId = suppliedMetricState12, Value = suppliedValue2  },
                               new StudentMetricLearningStandard {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, DateAdministration = suppliedDate1, AssessmentTitle = suppliedAssessmentTitle1, Version = suppliedVersion1, LearningStandard = suppliedLearningStandard1, Description = suppliedDescription1, MetricStateTypeId = suppliedMetricState11, Value = suppliedValue1  },
                           };
            return data.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new LearningStandardService(repository, metricNodeResolver);
            results = service.Get(new LearningStandardRequest()
                                        {
                                            StudentUSI = suppliedStudentUSI,
                                            SchoolId = suppliedSchoolId,
                                            MetricVariantId = suppliedMetricId
                                        });
        }

        [Test]
        public void Should_create_correct_number_of_rows()
        {
            Assert.That(results, Is.Not.Null);
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0].Assessments.Count, Is.EqualTo(2));
        }

        [Test]
        public void Should_select_and_bind_correct_data()
        {
            Assert.That(results[0].StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(results[0].MetricId, Is.EqualTo(suppliedMetricId));
            Assert.That(results[0].SchoolId, Is.EqualTo(suppliedSchoolId));
            Assert.That(results[0].LearningStandard, Is.EqualTo(suppliedLearningStandard1));
            Assert.That(results[0].Description, Is.EqualTo(suppliedDescription1));
            Assert.That(results[0].Assessments[0].StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(results[0].Assessments[0].DateAdministration, Is.EqualTo(suppliedDate1));
            Assert.That(results[0].Assessments[0].MetricStateTypeId, Is.EqualTo(suppliedMetricState11));
            Assert.That(results[0].Assessments[0].Value, Is.EqualTo(suppliedValue1));
            Assert.That(results[0].Assessments[0].AssessmentTitle, Is.EqualTo(suppliedAssessmentTitle1));
            Assert.That(results[0].Assessments[0].Version, Is.EqualTo(suppliedVersion1));
            Assert.That(results[0].Assessments[0].Administered, Is.True);

            Assert.That(results[0].Assessments[1].StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(results[0].Assessments[1].DateAdministration, Is.EqualTo(suppliedDate2));
            Assert.That(results[0].Assessments[1].MetricStateTypeId, Is.EqualTo(suppliedMetricState12));
            Assert.That(results[0].Assessments[1].Value, Is.EqualTo(suppliedValue2));
            Assert.That(results[0].Assessments[1].AssessmentTitle, Is.EqualTo(suppliedAssessmentTitle2));
            Assert.That(results[0].Assessments[1].Version, Is.EqualTo(suppliedVersion2));
            Assert.That(results[0].Assessments[1].Administered, Is.True);
        }

        [Test]
        public void Should_have_no_unassigned_values_on_presentation_model()
        {
            results[0].EnsureNoDefaultValues();
        }
    }

    public class When_loading_learning_standard_data_with_two_standards_and_two_tests : TestFixtureBase
    {
        private IRepository<StudentMetricLearningStandard> repository;
        private IMetricNodeResolver metricNodeResolver;

        private IQueryable<StudentMetricLearningStandard> suppliedData;
        private const int suppliedStudentUSI = 1000;
        private const int suppliedSchoolId = 2000;
        private const int suppliedMetricId = 1230;

        private readonly DateTime suppliedDate1 = new DateTime(2010, 12, 1);
        private readonly DateTime suppliedDate2 = new DateTime(2010, 12, 2);
        private const string suppliedAssessmentTitle1 = "Assessment1";
        private const string suppliedAssessmentTitle2 = "Assessment2";
        private const int suppliedVersion1 = 1;
        private const int suppliedVersion2 = 2;
        private const string suppliedLearningStandard1 = "LS1";
        private const string suppliedLearningStandard2 = "LS2";
        private const string suppliedDescription1 = "description 1";
        private const string suppliedDescription2 = "description 2";
        private const int suppliedMetricState11 = 1;
        private const int suppliedMetricState12 = 3;
        private const int suppliedMetricState22 = 3;
        private const string suppliedValue11 = "value 1.1";
        private const string suppliedValue12 = "value 1.2";
        private const string suppliedValue22 = "value 2.2";


        private IList<LearningStandardModel> results;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            suppliedData = GetData();
            repository = mocks.StrictMock<IRepository<StudentMetricLearningStandard>>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            Expect.Call(metricNodeResolver.ResolveMetricId(suppliedMetricId)).Return(suppliedMetricId);
            Expect.Call(repository.GetAll()).Return(suppliedData);
        }

        protected IQueryable<StudentMetricLearningStandard> GetData()
        {
            var data = new List<StudentMetricLearningStandard>
                           {
                               new StudentMetricLearningStandard {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 1},
                               new StudentMetricLearningStandard {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId + 1, MetricId = suppliedMetricId},
                               new StudentMetricLearningStandard {StudentUSI = suppliedStudentUSI + 1, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId},
                               new StudentMetricLearningStandard {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, DateAdministration = suppliedDate1, AssessmentTitle = suppliedAssessmentTitle1, Version = suppliedVersion1, LearningStandard = suppliedLearningStandard1, Description = suppliedDescription1, MetricStateTypeId = suppliedMetricState11, Value = suppliedValue11  },
                               new StudentMetricLearningStandard {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, DateAdministration = suppliedDate1, AssessmentTitle = suppliedAssessmentTitle1, Version = suppliedVersion1, LearningStandard = suppliedLearningStandard2, Description = suppliedDescription2, MetricStateTypeId = suppliedMetricState12, Value = suppliedValue12  },
                               new StudentMetricLearningStandard {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, DateAdministration = suppliedDate2, AssessmentTitle = suppliedAssessmentTitle2, Version = suppliedVersion2, LearningStandard = suppliedLearningStandard2, Description = suppliedDescription2, MetricStateTypeId = suppliedMetricState22, Value = suppliedValue22  },
                           };
            return data.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new LearningStandardService(repository, metricNodeResolver);
            results = service.Get(new LearningStandardRequest()
                                        {
                                            StudentUSI = suppliedStudentUSI,
                                            SchoolId = suppliedSchoolId,
                                            MetricVariantId = suppliedMetricId
                                        });
        }

        [Test]
        public void Should_create_correct_number_of_rows()
        {
            Assert.That(results, Is.Not.Null);
            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results[0].Assessments.Count, Is.EqualTo(2));
            Assert.That(results[1].Assessments.Count, Is.EqualTo(2));
        }

        [Test]
        public void Should_select_and_bind_correct_data()
        {
            Assert.That(results[0].StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(results[0].MetricId, Is.EqualTo(suppliedMetricId));
            Assert.That(results[0].SchoolId, Is.EqualTo(suppliedSchoolId));
            Assert.That(results[0].LearningStandard, Is.EqualTo(suppliedLearningStandard1));
            Assert.That(results[0].Description, Is.EqualTo(suppliedDescription1));
            Assert.That(results[0].Assessments[0].StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(results[0].Assessments[0].DateAdministration, Is.EqualTo(suppliedDate1));
            Assert.That(results[0].Assessments[0].MetricStateTypeId, Is.EqualTo(suppliedMetricState11));
            Assert.That(results[0].Assessments[0].Value, Is.EqualTo(suppliedValue11));
            Assert.That(results[0].Assessments[0].AssessmentTitle, Is.EqualTo(suppliedAssessmentTitle1));
            Assert.That(results[0].Assessments[0].Version, Is.EqualTo(suppliedVersion1));
            Assert.That(results[0].Assessments[0].Administered, Is.True);

            Assert.That(results[0].Assessments[1].StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(results[0].Assessments[1].DateAdministration, Is.EqualTo(suppliedDate2));
            Assert.That(results[0].Assessments[1].MetricStateTypeId, Is.Null);
            Assert.That(results[0].Assessments[1].Value, Is.Null);
            Assert.That(results[0].Assessments[1].AssessmentTitle, Is.EqualTo(suppliedAssessmentTitle2));
            Assert.That(results[0].Assessments[1].Version, Is.EqualTo(suppliedVersion2));
            Assert.That(results[0].Assessments[1].Administered, Is.False);


            Assert.That(results[1].StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(results[1].MetricId, Is.EqualTo(suppliedMetricId));
            Assert.That(results[1].SchoolId, Is.EqualTo(suppliedSchoolId));
            Assert.That(results[1].LearningStandard, Is.EqualTo(suppliedLearningStandard2));
            Assert.That(results[1].Description, Is.EqualTo(suppliedDescription2));
            Assert.That(results[1].Assessments[0].StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(results[1].Assessments[0].DateAdministration, Is.EqualTo(suppliedDate1));
            Assert.That(results[1].Assessments[0].MetricStateTypeId, Is.EqualTo(suppliedMetricState12));
            Assert.That(results[1].Assessments[0].Value, Is.EqualTo(suppliedValue12));
            Assert.That(results[1].Assessments[0].AssessmentTitle, Is.EqualTo(suppliedAssessmentTitle1));
            Assert.That(results[1].Assessments[0].Version, Is.EqualTo(suppliedVersion1));
            Assert.That(results[1].Assessments[0].Administered, Is.True);

            Assert.That(results[1].Assessments[1].StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(results[1].Assessments[1].DateAdministration, Is.EqualTo(suppliedDate2));
            Assert.That(results[1].Assessments[1].MetricStateTypeId, Is.EqualTo(suppliedMetricState22));
            Assert.That(results[1].Assessments[1].Value, Is.EqualTo(suppliedValue22));
            Assert.That(results[1].Assessments[1].AssessmentTitle, Is.EqualTo(suppliedAssessmentTitle2));
            Assert.That(results[1].Assessments[1].Version, Is.EqualTo(suppliedVersion2));
            Assert.That(results[1].Assessments[1].Administered, Is.True);
        }

        [Test]
        public void Should_have_no_unassigned_values_on_presentation_model()
        {
            results[0].EnsureNoDefaultValues("LearningStandardModel.Assessments[1].MetricStateTypeId",
	                                        "LearningStandardModel.Assessments[1].Value",
	                                        "LearningStandardModel.Assessments[1].Administered");
        }
    }

    public class When_loading_learning_standard_data_with_one_standard_two_tests_taken_on_the_same_day : TestFixtureBase
    {
        private IRepository<StudentMetricLearningStandard> repository;
        private IMetricNodeResolver metricNodeResolver;

        private IQueryable<StudentMetricLearningStandard> suppliedData;
        private const int suppliedStudentUSI = 1000;
        private const int suppliedSchoolId = 2000;
        private const int suppliedMetricId = 1230;

        private readonly DateTime suppliedDate1 = new DateTime(2010, 12, 1);
        private readonly DateTime suppliedDate2 = new DateTime(2010, 12, 1);
        private const string suppliedAssessmentTitle1 = "Assessment1";
        private const string suppliedAssessmentTitle2 = "Assessment2";
        private const int suppliedVersion1 = 1;
        private const int suppliedVersion2 = 2;
        private const string suppliedLearningStandard1 = "LS1";
        private const string suppliedDescription1 = "description 1";
        private const int suppliedMetricState11 = 1;
        private const int suppliedMetricState12 = 3;
        private const string suppliedValue1 = "value 1";
        private const string suppliedValue2 = "value 2";


        private IList<LearningStandardModel> results;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            suppliedData = GetData();
            repository = mocks.StrictMock<IRepository<StudentMetricLearningStandard>>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            Expect.Call(metricNodeResolver.ResolveMetricId(suppliedMetricId)).Return(suppliedMetricId);
            Expect.Call(repository.GetAll()).Return(suppliedData);
        }

        protected IQueryable<StudentMetricLearningStandard> GetData()
        {
            var data = new List<StudentMetricLearningStandard>
                           {
                               new StudentMetricLearningStandard {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 1},
                               new StudentMetricLearningStandard {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId + 1, MetricId = suppliedMetricId},
                               new StudentMetricLearningStandard {StudentUSI = suppliedStudentUSI + 1, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId},
                               new StudentMetricLearningStandard {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, DateAdministration = suppliedDate2, AssessmentTitle = suppliedAssessmentTitle2, Version = suppliedVersion2, LearningStandard = suppliedLearningStandard1, Description = suppliedDescription1, MetricStateTypeId = suppliedMetricState12, Value = suppliedValue2  },
                               new StudentMetricLearningStandard {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, DateAdministration = suppliedDate1, AssessmentTitle = suppliedAssessmentTitle1, Version = suppliedVersion1, LearningStandard = suppliedLearningStandard1, Description = suppliedDescription1, MetricStateTypeId = suppliedMetricState11, Value = suppliedValue1  },
                           };
            return data.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new LearningStandardService(repository, metricNodeResolver);
            results = service.Get(new LearningStandardRequest()
                                        {
                                            StudentUSI = suppliedStudentUSI,
                                            SchoolId = suppliedSchoolId,
                                            MetricVariantId = suppliedMetricId
                                        });
        }

        [Test]
        public void Should_create_correct_number_of_rows()
        {
            Assert.That(results, Is.Not.Null);
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0].Assessments.Count, Is.EqualTo(2));
        }

        [Test]
        public void Should_select_and_bind_correct_data()
        {
            Assert.That(results[0].StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(results[0].MetricId, Is.EqualTo(suppliedMetricId));
            Assert.That(results[0].SchoolId, Is.EqualTo(suppliedSchoolId));
            Assert.That(results[0].LearningStandard, Is.EqualTo(suppliedLearningStandard1));
            Assert.That(results[0].Description, Is.EqualTo(suppliedDescription1));
            Assert.That(results[0].Assessments[0].StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(results[0].Assessments[0].DateAdministration, Is.EqualTo(suppliedDate1));
            Assert.That(results[0].Assessments[0].MetricStateTypeId, Is.EqualTo(suppliedMetricState11));
            Assert.That(results[0].Assessments[0].Value, Is.EqualTo(suppliedValue1));
            Assert.That(results[0].Assessments[0].AssessmentTitle, Is.EqualTo(suppliedAssessmentTitle1));
            Assert.That(results[0].Assessments[0].Version, Is.EqualTo(suppliedVersion1));
            Assert.That(results[0].Assessments[0].Administered, Is.True);

            Assert.That(results[0].Assessments[1].StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(results[0].Assessments[1].DateAdministration, Is.EqualTo(suppliedDate2));
            Assert.That(results[0].Assessments[1].MetricStateTypeId, Is.EqualTo(suppliedMetricState12));
            Assert.That(results[0].Assessments[1].Value, Is.EqualTo(suppliedValue2));
            Assert.That(results[0].Assessments[1].AssessmentTitle, Is.EqualTo(suppliedAssessmentTitle2));
            Assert.That(results[0].Assessments[1].Version, Is.EqualTo(suppliedVersion2));
            Assert.That(results[0].Assessments[1].Administered, Is.True);
        }

        [Test]
        public void Should_have_no_unassigned_values_on_presentation_model()
        {
            results[0].EnsureNoDefaultValues();
        }
    }
}
