using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Models.Student.Detail;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.Resources.StudentSchool.Detail;
using EdFi.Dashboards.Resources.Tests.Common;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Student.Detail
{

    //
    public abstract class StudentMetricLearningStandardMetaDataServiceFixtureBase : TestFixtureBase
    {

        //
        private IRepository<StudentMetricLearningStandardMetaData> repositoryObjective;
        private IMetricNodeResolver metricNodeResolver;
        private IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;
        //
        protected List<StudentMetricLearningStandardMetaDataModel> actualModel;

        // student , school, metric
        protected int suppliedStudentUSI = 1686;
        protected int suppliedSchoolId = 350775;
        protected int suppliedMetricVariantId = 1234;
        // objective keys
        protected string suppliedObjectivePrefix = "Science Objective ";
        protected DateTime suppliedAssessmentDate1 = new DateTime(2012, 1, 15);
        // 
        protected int suppliedObjectiveCount;
        protected int suppliedGradeCount;
        protected int suppliedStandardCount;
        protected int suppliedAssessmentCount;

        //
        protected override void EstablishContext()
        {

            //
            base.EstablishContext();

            //
            repositoryObjective = mocks.StrictMock<IRepository<StudentMetricLearningStandardMetaData>>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            gradeLevelUtilitiesProvider = new FakeGradeLevelUtilitiesProvider();

            // expected calls
            Expect.Call(metricNodeResolver.ResolveMetricId(suppliedMetricVariantId)).Return(suppliedMetricVariantId);
            Expect.Call(repositoryObjective.GetAll()).Return(GetData_Objective());

        }

        //
        protected override void ExecuteTest()
        {

            // service
            var service = new StudentMetricLearningStandardMetaDataService(repositoryObjective, metricNodeResolver, gradeLevelUtilitiesProvider);
            actualModel = service.Get(new StudentMetricLearningStandardMetaDataRequest()
            {
                MetricVariantId = suppliedMetricVariantId
            }).ToList();

        }

        //
        protected virtual IQueryable<StudentMetricLearningStandardMetaData> GetData_Objective()
        {
            // create mix of objective x grade x standard records
            return new List<StudentMetricLearningStandardMetaData> { 
                // add row that should be excluded from results
                new StudentMetricLearningStandardMetaData{ 
                                    MetricId            = -9,
                                    LearningObjective   = "Objective X",
                                    GradeLevel          = "Grade X" ,
                                    LearningStandard    = "X.X.X",
                                    Description         = "don't use this stuff for science" },
                // add 1 row for 1st objective + 1 grade + 1 standard
                new StudentMetricLearningStandardMetaData{ 
                                    MetricId            = suppliedMetricVariantId,
                                    LearningObjective   = "Objective 1",
                                    GradeLevel          = "Grade 1" ,
                                    LearningStandard    = "1.1.1",
                                    Description         = "use stuff for science" },
                // add 4 rows for 2nd objective + 2 grades + 2 standards (symmetrical dimensions)
                new StudentMetricLearningStandardMetaData{ 
                                    MetricId            = suppliedMetricVariantId,
                                    LearningObjective   = "Objective 2",
                                    GradeLevel          = "Grade 2" ,
                                    LearningStandard    = "2.2.1",
                                    Description         = "use stuff for science" },
                new StudentMetricLearningStandardMetaData{ 
                                    MetricId            = suppliedMetricVariantId,
                                    LearningObjective   = "Objective 2",
                                    GradeLevel          = "Grade 2" ,
                                    LearningStandard    = "2.2.2",
                                    Description         = "use stuff for science" },
                new StudentMetricLearningStandardMetaData{ 
                                    MetricId            = suppliedMetricVariantId,
                                    LearningObjective   = "Objective 2",
                                    GradeLevel          = "Grade 3" ,
                                    LearningStandard    = "2.3.1",
                                    Description         = "use stuff for science" },
                new StudentMetricLearningStandardMetaData{ 
                                    MetricId            = suppliedMetricVariantId,
                                    LearningObjective   = "Objective 2",
                                    GradeLevel          = "Grade 3" ,
                                    LearningStandard    = "2.3.2",
                                    Description         = "use stuff for science" },
                // add 7 rows for 3rd objective + 3 grades + 2/3/4 standards (asymmetrical dimensions)
                new StudentMetricLearningStandardMetaData{ 
                                    MetricId            = suppliedMetricVariantId,
                                    LearningObjective   = "Objective 3",
                                    GradeLevel          = "Grade 4" ,
                                    LearningStandard    = "3.4.1",
                                    Description         = "use stuff for science" },
                new StudentMetricLearningStandardMetaData{ 
                                    MetricId            = suppliedMetricVariantId,
                                    LearningObjective   = "Objective 3",
                                    GradeLevel          = "Grade 4" ,
                                    LearningStandard    = "3.4.2",
                                    Description         = "use stuff for science" },
                new StudentMetricLearningStandardMetaData{ 
                                    MetricId            = suppliedMetricVariantId,
                                    LearningObjective   = "Objective 3",
                                    GradeLevel          = "Grade 5" ,
                                    LearningStandard    = "3.5.1",
                                    Description         = "use stuff for science" },
                new StudentMetricLearningStandardMetaData{ 
                                    MetricId            = suppliedMetricVariantId,
                                    LearningObjective   = "Objective 3",
                                    GradeLevel          = "Grade 5" ,
                                    LearningStandard    = "3.5.2",
                                    Description         = "use stuff for science" },
                new StudentMetricLearningStandardMetaData{ 
                                    MetricId            = suppliedMetricVariantId,
                                    LearningObjective   = "Objective 3",
                                    GradeLevel          = "Grade 5" ,
                                    LearningStandard    = "3.5.3",
                                    Description         = "use stuff for science" },
                new StudentMetricLearningStandardMetaData{ 
                                    MetricId            = suppliedMetricVariantId,
                                    LearningObjective   = "Objective 3",
                                    GradeLevel          = "Grade 6" ,
                                    LearningStandard    = "3.6.1",
                                    Description         = "use stuff for science" },
                new StudentMetricLearningStandardMetaData{ 
                                    MetricId            = suppliedMetricVariantId,
                                    LearningObjective   = "Objective 3",
                                    GradeLevel          = "Grade 6" ,
                                    LearningStandard    = "3.6.2",
                                    Description         = "use stuff for science" },
                new StudentMetricLearningStandardMetaData{ 
                                    MetricId            = suppliedMetricVariantId,
                                    LearningObjective   = "Objective 3",
                                    GradeLevel          = "Grade 6" ,
                                    LearningStandard    = "3.6.3",
                                    Description         = "use stuff for science" },
                new StudentMetricLearningStandardMetaData{ 
                                    MetricId            = suppliedMetricVariantId,
                                    LearningObjective   = "Objective 3",
                                    GradeLevel          = "Grade 6" ,
                                    LearningStandard    = "3.6.4",
                                    Description         = "use stuff for science" }
            }.AsQueryable();
        }

    }

    [TestFixture]
    public class When_calling_StudentMetricLearningStandardMetaDataService_should_return_model_default : StudentMetricLearningStandardMetaDataServiceFixtureBase
    {

        [Test]
        public void When_calling_StudentMetricLearningStandardMetaDataService_should_return_model_default_supplied()
        {

            //
            Assert.That(actualModel, Is.Not.Null);

            // model lcollection counts
            Assert.That(actualModel.Count, Is.EqualTo(3));
            // 1st objective should have 1 x 1
            Assert.That(actualModel[0].Grades.Count, Is.EqualTo(1));
            Assert.That(actualModel[0].Grades[0].Standards.Count, Is.EqualTo(1));
            // 2nd objective should have 2 x 2
            Assert.That(actualModel[1].Grades.Count, Is.EqualTo(2));
            Assert.That(actualModel[1].Grades[0].Standards.Count, Is.EqualTo(2));
            Assert.That(actualModel[1].Grades[1].Standards.Count, Is.EqualTo(2));
            // 3rd objective should have 1 x 2, 1 x 3, 1 x 4
            Assert.That(actualModel[2].Grades.Count, Is.EqualTo(3));
            Assert.That(actualModel[2].Grades[0].Standards.Count, Is.EqualTo(2));
            Assert.That(actualModel[2].Grades[1].Standards.Count, Is.EqualTo(3));
            Assert.That(actualModel[2].Grades[2].Standards.Count, Is.EqualTo(4));

            // objective attributes
            Assert.That(actualModel[0].LearningObjective, Is.EqualTo("Objective 1"));
            Assert.That(actualModel[0].Grades[0].Standards[0].LearningStandard, Is.EqualTo("1.1.1"));
            Assert.That(actualModel[1].LearningObjective, Is.EqualTo("Objective 2"));
            Assert.That(actualModel[1].Grades[1].Standards[0].LearningStandard, Is.EqualTo("2.3.1"));
            Assert.That(actualModel[2].LearningObjective, Is.EqualTo("Objective 3"));
            Assert.That(actualModel[2].Grades[2].Standards[2].LearningStandard, Is.EqualTo("3.6.3"));

        }

    }

    [TestFixture]
    public class When_calling_StudentMetricLearningStandardMetaDataService_should_return_model_3_3_3 : StudentMetricLearningStandardMetaDataServiceFixtureBase
    {

        // condition objective meta data across dimensions
        protected override IQueryable<StudentMetricLearningStandardMetaData> GetData_Objective()
        {

            // set objective count
            suppliedObjectiveCount = 3;
            suppliedGradeCount = 3;
            suppliedStandardCount = 3;

            // create new list
            var objectives = new List<StudentMetricLearningStandardMetaData>();

            // add objectives
            for (int i = 0; i < suppliedObjectiveCount; i++)
            {
                // add grades
                for (int j = 0; j < suppliedGradeCount; j++)
                {
                    // add standards
                    for (int k = 0; k < suppliedStandardCount; k++)
                    {
                        objectives.Add(new StudentMetricLearningStandardMetaData()
                        {
                            MetricId = suppliedMetricVariantId,
                            LearningObjective = suppliedObjectivePrefix + (i + 1).ToString(),
                            GradeLevel = Utilities.GetGradeLevelFromSort(j + 1),
                            LearningStandard = (i + 1).ToString() + "." + (j + 1).ToString() + "." + (k + 1).ToString(),
                        });

                    }
                }
            }

            // return list
            return objectives.AsQueryable();

        }


        [Test]
        public void When_calling_StudentMetricLearningStandardMetaDataService_should_return_model_3_3_3_supplied()
        {

            //
            Assert.That(actualModel, Is.Not.Null);

            // model lcollection counts
            Assert.That(actualModel.Count, Is.EqualTo(suppliedObjectiveCount));
            for (int i = 0; i < suppliedObjectiveCount; i++)
            {
                Assert.That(actualModel[i].Grades.Count, Is.EqualTo(suppliedGradeCount));
                //
                for (int j = 0; j < suppliedGradeCount; j++)
                {
                    Assert.That(actualModel[i].Grades[j].Standards.Count, Is.EqualTo(suppliedStandardCount));
                }
            }

            // objective attributes
            for (int i = 0; i < suppliedObjectiveCount; i++)
            {
                Assert.That(actualModel[i].LearningObjective.EndsWith((i + 1).ToString()), Is.True);
                for (int j = 0; j < suppliedGradeCount; j++)
                {
                    Assert.That(actualModel[i].Grades[j].GradeLevel, Is.EqualTo(Utilities.GetGradeLevelFromSort(j + 1)));
                    for (int k = 0; k < suppliedStandardCount; k++)
                    {
                        Assert.That(actualModel[i].Grades[j].Standards[k].LearningStandard.EndsWith((k + 1).ToString()), Is.True);
                    }
                }
            }

        }

    }

}

