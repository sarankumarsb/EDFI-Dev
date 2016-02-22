using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Models.Student.Detail;
using EdFi.Dashboards.Resources.StudentSchool.Detail;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Student.Detail
{
    public abstract class LearningObjectiveServiceFixtureBase : TestFixtureBase
    {        
        private IRepository<StudentMetricLearningObjective> repository;
        private IMetricNodeResolver metricNodeResolver;

        protected int suppliedSchoolId = 100;
        protected int suppliedStudentUSI = 200;
        protected int suppliedMetricId = 300;
        protected int suppliedMetricVariantId = 30099;
        protected string suppliedMetricDisplayName = "metric display name";
        protected string suppliedMetricName = "metric name";
        protected LearningObjectiveModel actualModel;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            repository = mocks.StrictMock<IRepository<StudentMetricLearningObjective>>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            Expect.Call(metricNodeResolver.GetMetricNodeForStudentFromMetricVariantId(suppliedSchoolId, suppliedMetricVariantId)).Return(GetMetricMetadataNode());
            Expect.Call(repository.GetAll()).Return(GetData());
        }

        protected MetricMetadataNode GetMetricMetadataNode()
        {
            return new MetricMetadataNode(null) { MetricId = suppliedMetricId, MetricVariantId = suppliedMetricVariantId, Name = suppliedMetricName, DisplayName = suppliedMetricDisplayName };
        }

        protected override void ExecuteTest()
        {
            var service = new LearningObjectiveService(repository, metricNodeResolver);
            actualModel = service.Get(new LearningObjectiveRequest()
                                    {
                                        StudentUSI = suppliedStudentUSI,
                                        SchoolId = suppliedSchoolId,
                                        MetricVariantId = suppliedMetricVariantId
                                    });
        }

        protected abstract IQueryable<StudentMetricLearningObjective> GetData();

        [Test]
        public void Should_initialize_model_correctly()
        {
            Assert.That(actualModel.MetricId, Is.EqualTo(suppliedMetricId));
            Assert.That(actualModel.InventoryName, Is.EqualTo(suppliedMetricDisplayName));
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }

    public class When_loading_learning_objective_data_with_no_objective_found : LearningObjectiveServiceFixtureBase
    {
        protected override IQueryable<StudentMetricLearningObjective> GetData()
        {
            var data = new List<StudentMetricLearningObjective>
                           {
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 1},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId + 1, MetricId = suppliedMetricId},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI + 1, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId}
                           };

            return data.AsQueryable();
        }

        [Test]
        public void Should_create_correct_number_of_rows()
        {
            Assert.That(actualModel.LearningObjectiveSkills.Count(), Is.EqualTo(0));
            Assert.That(actualModel.AssessmentTitles.Count(), Is.EqualTo(0));
        }
    }

    public class When_loading_learning_objective_data_with_one_section_one_skill_one_time_period : LearningObjectiveServiceFixtureBase
    {        
        public string suppliedAssessmentTitle1 = "BOY";
        public string suppliedLearningObjective1 = "Phonemic Awareness";
        public string suppliedObjectiveName1 = "Blending Words";

        protected override IQueryable<StudentMetricLearningObjective> GetData()
        {
            var data = new List<StudentMetricLearningObjective>
                           {
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 1},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId + 1, MetricId = suppliedMetricId},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI + 1, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle1, MetricStateTypeId = 1, Value = "D", LearningObjective = suppliedLearningObjective1, ObjectiveName = suppliedObjectiveName1}
                           };

            return data.AsQueryable();
        }

        [Test]
        public void Should_create_correct_number_of_rows()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.AssessmentTitles.Count, Is.EqualTo(1));
            Assert.That(actualModel.LearningObjectiveSkills.Count(), Is.EqualTo(1));
            Assert.That(actualModel.LearningObjectiveSkills.First().SkillValues.Count(), Is.EqualTo(1));
        }
    }

    public class When_loading_learning_objective_data_with_one_section_one_skill_multiple_time_periods : LearningObjectiveServiceFixtureBase
    {
        public string suppliedAssessmentTitle1 = "BOY";
        public string suppliedAssessmentTitle2 = "MOY";
        public string suppliedLearningObjective1 = "Phonemic Awareness";
        public string suppliedObjectiveName1 = "Blending Words";

        protected override IQueryable<StudentMetricLearningObjective> GetData()
        {
            var data = new List<StudentMetricLearningObjective>
                           {
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 1},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId + 1, MetricId = suppliedMetricId},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI + 1, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle1, MetricStateTypeId = 1, Value = "D", LearningObjective = suppliedLearningObjective1, ObjectiveName = suppliedObjectiveName1},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle2, MetricStateTypeId = 1, Value = "SD", LearningObjective = suppliedLearningObjective1, ObjectiveName = suppliedObjectiveName1}
                           };

            return data.AsQueryable();
        }

        [Test]
        public void Should_create_correct_number_of_rows()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.AssessmentTitles.Count, Is.EqualTo(2));
            Assert.That(actualModel.LearningObjectiveSkills.Count(), Is.EqualTo(1));
            Assert.That(actualModel.LearningObjectiveSkills.First().SkillValues.Count(), Is.EqualTo(2));
            Assert.That(actualModel.LearningObjectiveSkills.First().SkillValues.Where(skill => skill.Title == suppliedAssessmentTitle1).Count(), Is.EqualTo(1));
            Assert.That(actualModel.LearningObjectiveSkills.First().SkillValues.Where(skill => skill.Title == suppliedAssessmentTitle1).First().Value, Is.EqualTo("D"));
            Assert.That(actualModel.LearningObjectiveSkills.First().SkillValues.Where(skill => skill.Title == suppliedAssessmentTitle2).First().Value, Is.EqualTo("SD"));
        }
    }

    public class When_loading_learning_objective_data_with_one_section_multiple_skills_one_time_period : LearningObjectiveServiceFixtureBase
    {
        public string suppliedAssessmentTitle1 = "BOY";
        public string suppliedLearningObjective1 = "Phonemic Awareness";
        public string suppliedObjectiveName1 = "Blending Words";
        public string suppliedObjectiveName2 = "Skill 2";

        protected override IQueryable<StudentMetricLearningObjective> GetData()
        {
            var data = new List<StudentMetricLearningObjective>
                           {
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 1},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId + 1, MetricId = suppliedMetricId},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI + 1, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle1, MetricStateTypeId = 1, Value = "D", LearningObjective = suppliedLearningObjective1, ObjectiveName = suppliedObjectiveName1},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle1, MetricStateTypeId = 1, Value = "SD", LearningObjective = suppliedLearningObjective1, ObjectiveName = suppliedObjectiveName2}
                           };

            return data.AsQueryable();
        }

        [Test]
        public void Should_create_correct_number_of_rows()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.AssessmentTitles.Count, Is.EqualTo(1));
            Assert.That(actualModel.LearningObjectiveSkills.Count(), Is.EqualTo(2));

            Assert.That(actualModel.LearningObjectiveSkills.Where(skill => skill.SkillName == suppliedObjectiveName1).Count(), Is.EqualTo(1));
            Assert.That(actualModel.LearningObjectiveSkills.Where(skill => skill.SkillName == suppliedObjectiveName1).First().SkillValues.Where(skill => skill.Title == suppliedAssessmentTitle1).Count(), Is.EqualTo(1));
            Assert.That(actualModel.LearningObjectiveSkills.Where(skill => skill.SkillName == suppliedObjectiveName1).First().SkillValues.First().Value, Is.EqualTo("D"));

            Assert.That(actualModel.LearningObjectiveSkills.Where(skill => skill.SkillName == suppliedObjectiveName2).Count(), Is.EqualTo(1));
            Assert.That(actualModel.LearningObjectiveSkills.Where(skill => skill.SkillName == suppliedObjectiveName2).First().SkillValues.First().Value, Is.EqualTo("SD"));
        }
    }

    public class When_loading_learning_objective_data_with_one_section_multiple_skills_multiple_time_periods : LearningObjectiveServiceFixtureBase
    {
        public string suppliedAssessmentTitle1 = "BOY";
        public string suppliedAssessmentTitle2 = "MOY";
        public string suppliedLearningObjective1 = "Phonemic Awareness";
        public string suppliedObjectiveName1 = "Blending Words";
        public string suppliedObjectiveName2 = "Skill 2";

        protected override IQueryable<StudentMetricLearningObjective> GetData()
        {
            var data = new List<StudentMetricLearningObjective>
                           {
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 1},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId + 1, MetricId = suppliedMetricId},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI + 1, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle1, MetricStateTypeId = 1, Value = "D", LearningObjective = suppliedLearningObjective1, ObjectiveName = suppliedObjectiveName1},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle1, MetricStateTypeId = 1, Value = "SD", LearningObjective = suppliedLearningObjective1, ObjectiveName = suppliedObjectiveName2},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle2, MetricStateTypeId = 1, Value = "D", LearningObjective = suppliedLearningObjective1, ObjectiveName = suppliedObjectiveName2},
                           };

            return data.AsQueryable();
        }

        [Test]
        public void Should_create_correct_number_of_rows()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.AssessmentTitles.Count, Is.EqualTo(2));
            Assert.That(actualModel.LearningObjectiveSkills.Count(), Is.EqualTo(2));

            Assert.That(actualModel.LearningObjectiveSkills.Where(skill => skill.SkillName == suppliedObjectiveName1).First().SkillValues.Count(), Is.EqualTo(1));
            Assert.That(actualModel.LearningObjectiveSkills.Where(skill => skill.SkillName == suppliedObjectiveName2).First().SkillValues.Count(), Is.EqualTo(2));
        }
    }

    public class When_loading_learning_objective_data_with_multiple_sections_one_skill_one_time_period : LearningObjectiveServiceFixtureBase
    {
        public string suppliedAssessmentTitle1 = "BOY";
        public string suppliedLearningObjective1 = "Phonemic Awareness";
        public string suppliedLearningObjective2 = "Learning Objective 2";
        public string suppliedObjectiveName1 = "Blending Words";
        public string suppliedObjectiveName2 = "Skill 2";

        protected override IQueryable<StudentMetricLearningObjective> GetData()
        {
            var data = new List<StudentMetricLearningObjective>
                           {
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 1},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId + 1, MetricId = suppliedMetricId},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI + 1, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle1, MetricStateTypeId = 1, Value = "D", LearningObjective = suppliedLearningObjective1, ObjectiveName = suppliedObjectiveName1},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle1, MetricStateTypeId = 1, Value = "SD", LearningObjective = suppliedLearningObjective2, ObjectiveName = suppliedObjectiveName2},
                           };

            return data.AsQueryable();
        }

        [Test]
        public void Should_create_correct_number_of_rows()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.AssessmentTitles.Count, Is.EqualTo(1));
            Assert.That(actualModel.LearningObjectiveSkills.Count(), Is.EqualTo(2));
            Assert.That(actualModel.LearningObjectiveSkills.Where(section => section.SectionName == suppliedLearningObjective1).First().SkillValues.Count(), Is.EqualTo(1));
            Assert.That(actualModel.LearningObjectiveSkills.Where(section => section.SectionName == suppliedLearningObjective2).First().SkillValues.Count(), Is.EqualTo(1));
        }
    }

    public class When_loading_learning_objective_data_with_multiple_sections_one_skill_multiple_time_periods : LearningObjectiveServiceFixtureBase
    {
        public string suppliedAssessmentTitle1 = "BOY";
        public string suppliedAssessmentTitle2 = "MOY";
        public string suppliedAssessmentTitle3 = "EOY";
        public string suppliedLearningObjective1 = "Phonemic Awareness";
        public string suppliedLearningObjective2 = "Learning Objective 2";
        public string suppliedObjectiveName1 = "Blending Words";
        public string suppliedObjectiveName2 = "Skill 2";

        protected override IQueryable<StudentMetricLearningObjective> GetData()
        {
            var data = new List<StudentMetricLearningObjective>
                           {
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 1},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId + 1, MetricId = suppliedMetricId},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI + 1, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle1, MetricStateTypeId = 1, Value = "D", LearningObjective = suppliedLearningObjective1, ObjectiveName = suppliedObjectiveName1},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle2, MetricStateTypeId = 1, Value = "D", LearningObjective = suppliedLearningObjective1, ObjectiveName = suppliedObjectiveName1},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle1, MetricStateTypeId = 1, Value = "SD", LearningObjective = suppliedLearningObjective2, ObjectiveName = suppliedObjectiveName2},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle2, MetricStateTypeId = 1, Value = "SD", LearningObjective = suppliedLearningObjective2, ObjectiveName = suppliedObjectiveName2},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle3, MetricStateTypeId = 1, Value = "SD", LearningObjective = suppliedLearningObjective2, ObjectiveName = suppliedObjectiveName2}
                           };

            return data.AsQueryable();
        }

        [Test]
        public void Should_create_correct_number_of_rows()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.AssessmentTitles.Count, Is.EqualTo(3));
            Assert.That(actualModel.LearningObjectiveSkills.Count(), Is.EqualTo(2));
            Assert.That(actualModel.LearningObjectiveSkills.Where(section => section.SectionName == suppliedLearningObjective1).First().SkillValues.Count(), Is.EqualTo(2));
            Assert.That(actualModel.LearningObjectiveSkills.Where(section => section.SectionName == suppliedLearningObjective2).First().SkillValues.Count(), Is.EqualTo(3));
        }
    }

    public class When_loading_learning_objective_data_with_multiple_sections_multiple_skills_one_time_period : LearningObjectiveServiceFixtureBase
    {
        public string suppliedAssessmentTitle1 = "BOY";
        public string suppliedLearningObjective1 = "Phonemic Awareness";
        public string suppliedLearningObjective2 = "Learning Objective 2";
        public string suppliedObjectiveName1 = "Blending Words";
        public string suppliedObjectiveName2 = "Skill 2";
        public string suppliedObjectiveName3 = "Skill 3";

        protected override IQueryable<StudentMetricLearningObjective> GetData()
        {
            var data = new List<StudentMetricLearningObjective>
                           {
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 1},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId + 1, MetricId = suppliedMetricId},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI + 1, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle1, MetricStateTypeId = 1, Value = "D", LearningObjective = suppliedLearningObjective1, ObjectiveName = suppliedObjectiveName1},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle1, MetricStateTypeId = 1, Value = "SD", LearningObjective = suppliedLearningObjective2, ObjectiveName = suppliedObjectiveName2},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle1, MetricStateTypeId = 1, Value = "SD", LearningObjective = suppliedLearningObjective2, ObjectiveName = suppliedObjectiveName3}
                           };

            return data.AsQueryable();
        }

        [Test]
        public void Should_create_correct_number_of_rows()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.AssessmentTitles.Count, Is.EqualTo(1));
            Assert.That(actualModel.LearningObjectiveSkills.Count(), Is.EqualTo(3));
            Assert.That(actualModel.LearningObjectiveSkills.Where(section => section.SectionName == suppliedLearningObjective1).Count(), Is.EqualTo(1));
            Assert.That(actualModel.LearningObjectiveSkills.Where(section => section.SectionName == suppliedLearningObjective2).Count(), Is.EqualTo(2));

            Assert.That(actualModel.LearningObjectiveSkills.Where(section => section.SkillName == suppliedObjectiveName1).First().SkillValues.Count(), Is.EqualTo(1));
            Assert.That(actualModel.LearningObjectiveSkills.Where(section => section.SkillName == suppliedObjectiveName2).First().SkillValues.Count(), Is.EqualTo(1));
            Assert.That(actualModel.LearningObjectiveSkills.Where(section => section.SkillName == suppliedObjectiveName3).First().SkillValues.Count(), Is.EqualTo(1));
        }
    }

    public class When_loading_learning_objective_data_with_multiple_sections_multiple_skills_multiple_time_periods : LearningObjectiveServiceFixtureBase
    {
        public string suppliedAssessmentTitle1 = "BOY";
        public string suppliedAssessmentTitle2 = "MOY";
        public string suppliedAssessmentTitle3 = "EOY";
        public string suppliedLearningObjective1 = "Phonemic Awareness";
        public string suppliedLearningObjective2 = "Learning Objective 2";
        public string suppliedObjectiveName1 = "Blending Words";
        public string suppliedObjectiveName2 = "Skill 2";
        public string suppliedObjectiveName3 = "Skill 3";
        public string suppliedObjectiveName4 = "Skill 4";

        protected override IQueryable<StudentMetricLearningObjective> GetData()
        {
            var data = new List<StudentMetricLearningObjective>
                           {
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 1},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId + 1, MetricId = suppliedMetricId},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI + 1, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId},

                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle1, MetricStateTypeId = 1, Value = "D", LearningObjective = suppliedLearningObjective1, ObjectiveName = suppliedObjectiveName1},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle2, MetricStateTypeId = 1, Value = "D", LearningObjective = suppliedLearningObjective1, ObjectiveName = suppliedObjectiveName1},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle1, MetricStateTypeId = 1, Value = "D", LearningObjective = suppliedLearningObjective1, ObjectiveName = suppliedObjectiveName4},

                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle1, MetricStateTypeId = 1, Value = "SD", LearningObjective = suppliedLearningObjective2, ObjectiveName = suppliedObjectiveName2},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle1, MetricStateTypeId = 1, Value = "SD", LearningObjective = suppliedLearningObjective2, ObjectiveName = suppliedObjectiveName3},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle2, MetricStateTypeId = 1, Value = "SD", LearningObjective = suppliedLearningObjective2, ObjectiveName = suppliedObjectiveName2},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle2, MetricStateTypeId = 1, Value = "SD", LearningObjective = suppliedLearningObjective2, ObjectiveName = suppliedObjectiveName3},
                               new StudentMetricLearningObjective {StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, AssessmentTitle = suppliedAssessmentTitle3, MetricStateTypeId = 1, Value = "SD", LearningObjective = suppliedLearningObjective2, ObjectiveName = suppliedObjectiveName2}
                           };

            return data.AsQueryable();
        }

        [Test]
        public void Should_create_correct_number_of_rows()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.AssessmentTitles.Count, Is.EqualTo(3));
            Assert.That(actualModel.LearningObjectiveSkills.Count(), Is.EqualTo(4));

            Assert.That(actualModel.LearningObjectiveSkills.Where(section => section.SectionName == suppliedLearningObjective1).Count(), Is.EqualTo(2));
            Assert.That(actualModel.LearningObjectiveSkills.Where(section => section.SectionName == suppliedLearningObjective2).Count(), Is.EqualTo(2));

            Assert.That(actualModel.LearningObjectiveSkills.Where(section => section.SkillName == suppliedObjectiveName1).Count(), Is.EqualTo(1));
            Assert.That(actualModel.LearningObjectiveSkills.Where(section => section.SkillName == suppliedObjectiveName2).Count(), Is.EqualTo(1));
            Assert.That(actualModel.LearningObjectiveSkills.Where(section => section.SkillName == suppliedObjectiveName3).Count(), Is.EqualTo(1));
            Assert.That(actualModel.LearningObjectiveSkills.Where(section => section.SkillName == suppliedObjectiveName4).Count(), Is.EqualTo(1));

            Assert.That(actualModel.LearningObjectiveSkills.Where(section => section.SectionName == suppliedLearningObjective1 && section.SkillName == suppliedObjectiveName1).First().SkillValues.Count(), Is.EqualTo(2));
            Assert.That(actualModel.LearningObjectiveSkills.Where(section => section.SectionName == suppliedLearningObjective1 && section.SkillName == suppliedObjectiveName4).First().SkillValues.Count(), Is.EqualTo(1));
            Assert.That(actualModel.LearningObjectiveSkills.Where(section => section.SectionName == suppliedLearningObjective2 && section.SkillName == suppliedObjectiveName2).First().SkillValues.Count(), Is.EqualTo(3));
            Assert.That(actualModel.LearningObjectiveSkills.Where(section => section.SectionName == suppliedLearningObjective2 && section.SkillName == suppliedObjectiveName3).First().SkillValues.Count(), Is.EqualTo(2));
        }
    }
}
