using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Application;
using EdFi.Dashboards.Resources.Models.Student.Detail;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.Resources.StudentSchool.Detail;
using EdFi.Dashboards.Resources.Tests.Common;
using EdFi.Dashboards.Testing;
using EdFi.Dashboards.Warehouse.Data.Entities;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Student.Detail
{

    // create derived fixture base for overriding test data
    public abstract class StudentObjectiveGradeStandardServiceFixtureBase : TestFixtureBase
    {

        private IStudentMetricLearningStandardMetaDataService serviceObjective;
        private IRepository<StudentSchoolInformation> repositoryStudent;
        private IRepository<StudentMetricLearningStandard> repositoryStandardCurrent;
        private IRepository<StudentMetricLearningStandardHistorical> repositoryStandardHistory;
        private IRepository<StudentMetricBenchmarkAssessment> repositoryBenchmarkCurrent;
        private IRepository<StudentMetricBenchmarkAssessmentHistorical> repositoryBenchmarkHistory;
        private IWarehouseAvailabilityProviderResource providerWarehouseAvailability;
        private IMetricNodeResolver metricNodeResolver;
        private IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;

        protected List<StudentObjectiveGradeStandardModel> actualModel;
        protected List<StudentMetricLearningStandardMetaDataModel> suppliedObjectives;
        protected IQueryable<StudentMetricLearningStandard> suppliedStandardsCurrent;
        protected IQueryable<StudentMetricLearningStandardHistorical> suppliedStandardsHistory;

        // default student , school, metric
        protected int suppliedStudentUSI = 1686;
        protected int suppliedSchoolId = 350775;
        protected int suppliedMetricVariantId = 1234;
        // other default test values
        protected string suppliedObjectivePrefix = "Objective ";
        protected DateTime suppliedAssessmentDate1 = new DateTime(2013, 1, 15);
        protected string suppliedBenchmarkValueBase = ".04";
        protected int suppliedStudentGradeCurrent = 6;
        protected short suppliedSchoolYearCurrent = 2013;
        protected bool suppliedWarehouseAvailable = true;
        // default collection counts
        protected int suppliedObjectiveCount = 3;
        protected int suppliedGradeStart = 1;
        protected int suppliedGradeCount = 3;
        protected int suppliedGradePrior = 2;
        protected int suppliedGradeAhead = 2;
        protected int suppliedStandardCount = 3;
        protected int suppliedAssessmentCount = 3;
        protected int suppliedYearsHistory = 3;

        // indicator test variables
        protected int suppliedIndicatorKey;
        protected int[] suppliedIndicatorValues;
        protected int suppliedIndicatorResult;


        // condition default meta data matrix for Objective x Grade x Standard
        protected virtual List<StudentMetricLearningStandardMetaDataModel> GetObjectives()
        {

            // create new list
            suppliedObjectives = new List<StudentMetricLearningStandardMetaDataModel>();

            // add objectives
            for (int i = 0; i < suppliedObjectiveCount; i++)
            {
                var objective = new StudentMetricLearningStandardMetaDataModel(suppliedMetricVariantId)
                {
                    LearningObjective = suppliedObjectivePrefix + (i + 1)
                };
                // add grades
                for (int j = suppliedGradeStart - 1; j < suppliedGradeStart + suppliedGradeCount - 1; j++)
                {
                    int gradeSort = j + 1;
                    var grade = new StudentMetricLearningStandardMetaDataModel.GradeModel()
                    {
                        GradeLevel = Utilities.GetGradeLevelFromSort(gradeSort),
                        GradeSort = gradeSort,
                    };
                    // add standards
                    for (int k = 0; k < suppliedStandardCount; k++)
                    {
                        var standard = new StudentMetricLearningStandardMetaDataModel.StandardModel()
                        {
                            LearningStandard = (i + 1) + "." + gradeSort + "." + (k + 1)
                        };
                        grade.Standards.Add(standard);
                    }
                    objective.Grades.Add(grade);
                }
                suppliedObjectives.Add(objective);
            }

            // return list
            return suppliedObjectives;

        }

        // current standards data provided by test fixture
        protected abstract IQueryable<StudentMetricLearningStandard> GetStandardsCurrent();

        // historical standards data provided by test fixture
        protected virtual IQueryable<StudentMetricLearningStandardHistorical> GetStandardsHistory()
        {
            return new List<StudentMetricLearningStandardHistorical>().AsQueryable();
        }

        // default student data
        protected virtual IQueryable<StudentSchoolInformation> GetStudent()
        {

            var students = new List<StudentSchoolInformation>();

            students.Add(new StudentSchoolInformation()
            {
                StudentUSI = suppliedStudentUSI,
                SchoolId = suppliedSchoolId,
                GradeLevel = Utilities.GetGradeLevelFromSort(suppliedStudentGradeCurrent)
            });

            return students.AsQueryable();

        }

        // default benchmark current assessment data
        protected virtual IQueryable<StudentMetricBenchmarkAssessment> GetBenchmarksCurrent()
        {

            // set benchmark count
            int benchmarkCount = suppliedAssessmentCount;

            // create new list
            var benchmarks = new List<StudentMetricBenchmarkAssessment>();

            // add benchmarks
            for (int i = 0; i < benchmarkCount; i++)
            {
                benchmarks.Add(new StudentMetricBenchmarkAssessment()
                {
                    StudentUSI = suppliedStudentUSI,
                    SchoolId = suppliedSchoolId,
                    MetricId = StudentObjectiveGradeStandardService.GetBenchmarkMetricId(suppliedMetricVariantId),
                    Date = suppliedAssessmentDate1.AddMonths(i),
                    AssessmentTitle = "Assessment Title",
                    Version = suppliedSchoolYearCurrent,
                    Value = suppliedBenchmarkValueBase + (i + 1)
                });
            }

            // return list
            return benchmarks.AsQueryable();

        }

        // default benchmark history assessment data
        protected virtual IQueryable<StudentMetricBenchmarkAssessmentHistorical> GetBenchmarksHistory()
        {

            // set benchmark count
            int benchmarkCount = suppliedAssessmentCount;

            // create new standard list
            var benchmarks = new List<StudentMetricBenchmarkAssessmentHistorical>();

            // add benchmarks
            for (int i = 0; i < benchmarkCount; i++)
            {
                // inlcude irrelevant year
                for (int j = 0; j < suppliedGradePrior + 1; j++)
                {
                    benchmarks.Add(new StudentMetricBenchmarkAssessmentHistorical()
                    {
                        StudentUSI = suppliedStudentUSI,
                        SchoolId = suppliedSchoolId,
                        SchoolYear = (short)(suppliedSchoolYearCurrent - (j + 1)),
                        MetricId = StudentObjectiveGradeStandardService.GetBenchmarkMetricId(suppliedMetricVariantId),
                        Date = suppliedAssessmentDate1.AddYears(-(j + 1)).AddMonths(i),
                        AssessmentTitle = "Assessment Title",
                        Version = suppliedSchoolYearCurrent - (j + 1),
                        Value = suppliedBenchmarkValueBase + (i + 1)
                    });

                }
            }

            // return list
            return benchmarks.AsQueryable();

        }

        protected override void EstablishContext()
        {

            // mock interfaces
            serviceObjective = mocks.StrictMock<IStudentMetricLearningStandardMetaDataService>();
            repositoryStudent = mocks.StrictMock<IRepository<StudentSchoolInformation>>();
            repositoryStandardCurrent = mocks.StrictMock<IRepository<StudentMetricLearningStandard>>();
            repositoryStandardHistory = mocks.StrictMock<IRepository<StudentMetricLearningStandardHistorical>>();
            repositoryBenchmarkCurrent = mocks.StrictMock<IRepository<StudentMetricBenchmarkAssessment>>();
            repositoryBenchmarkHistory = mocks.StrictMock<IRepository<StudentMetricBenchmarkAssessmentHistorical>>();
            providerWarehouseAvailability = mocks.StrictMock<IWarehouseAvailabilityProviderResource>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            gradeLevelUtilitiesProvider = new FakeGradeLevelUtilitiesProvider();

            // expected calls
            Expect.Call(metricNodeResolver.ResolveMetricId(suppliedMetricVariantId)).Return(suppliedMetricVariantId);
            Expect.Call(repositoryStandardCurrent.GetAll()).Return(GetStandardsCurrent());
            Expect.Call(providerWarehouseAvailability.Get()).Return(suppliedWarehouseAvailable);
            Expect.Call(repositoryStudent.GetAll()).Return(GetStudent());
            Expect.Call(serviceObjective.Get(null)).IgnoreArguments().Return(GetObjectives());
            Expect.Call(repositoryBenchmarkCurrent.GetAll()).Return(GetBenchmarksCurrent());

            if (suppliedWarehouseAvailable)
            {
                Expect.Call(repositoryStandardHistory.GetAll()).Return(GetStandardsHistory());
                Expect.Call(repositoryBenchmarkHistory.GetAll()).Return(GetBenchmarksHistory());
            }

            base.EstablishContext();

        }

        protected override void ExecuteTest()
        {
            // call service
            var service = new StudentObjectiveGradeStandardService(serviceObjective, repositoryStudent, repositoryBenchmarkCurrent, repositoryStandardCurrent,
                                                                   providerWarehouseAvailability, repositoryBenchmarkHistory, repositoryStandardHistory, metricNodeResolver, gradeLevelUtilitiesProvider);
            actualModel = service.Get(StudentObjectiveGradeStandardRequest.Create(suppliedStudentUSI, suppliedSchoolId, suppliedMetricVariantId)).ToList();
        }

    }


    // derived base for test of matrix dimensions
    public abstract class StudentObjectiveGradeStandardServiceFixtureBaseDimensions : StudentObjectiveGradeStandardServiceFixtureBase
    {

        // condition test data for learning standard for full meta data matrix
        protected override IQueryable<StudentMetricLearningStandard> GetStandardsCurrent()
        {

            // create new standard list
            var standards = new List<StudentMetricLearningStandard>();

            // for each objective
            for (int i = 0; i < suppliedObjectiveCount; i++)
            {
                // for configured grades
                for (int j = suppliedStudentGradeCurrent - suppliedGradePrior; j <= suppliedStudentGradeCurrent + suppliedGradeAhead; j++)
                {
                    // for each standard
                    for (int k = 0; k < suppliedStandardCount; k++)
                    {
                        // set standard key
                        string standardKey = (i + 1) + "." + j + "." + (k + 1);
                        // add assessments
                        for (int l = 0; l < suppliedAssessmentCount; l++)
                        {
                            standards.Add(new StudentMetricLearningStandard()
                            {
                                StudentUSI = suppliedStudentUSI,
                                MetricId = suppliedMetricVariantId,
                                SchoolId = suppliedSchoolId,
                                GradeLevel = Utilities.GetGradeLevelFromSort(j),
                                AssessmentTitle = "(" + standardKey + ") SY 2013,District: 35, Science Grade 7 Genetics...",
                                Version = suppliedSchoolYearCurrent,
                                LearningObjective = suppliedObjectivePrefix + (i + 1),
                                LearningStandard = standardKey,
                                Description = "(" + standardKey + ") using stuff in science...",
                                DateAdministration = suppliedAssessmentDate1.AddMonths(l),
                                MetricStateTypeId = 3,
                                Value = "1 of 2"
                            });
                        }
                    }
                }
            }

            // return list
            suppliedStandardsCurrent = standards.AsQueryable();
            return suppliedStandardsCurrent;

        }

        // condition history data for learning standard for full meta data matrix
        protected override IQueryable<StudentMetricLearningStandardHistorical> GetStandardsHistory()
        {

            // create new standard list
            var standards = new List<StudentMetricLearningStandardHistorical>();

            // for each historical year
            for (int year = suppliedSchoolYearCurrent - suppliedYearsHistory; year < suppliedSchoolYearCurrent; year++)
            {
                // for each objective i
                for (int i = 0; i < suppliedObjectiveCount; i++)
                {
                    // for each grade j
                    int gradeHistory = suppliedStudentGradeCurrent - (suppliedSchoolYearCurrent - year);
                    int gradeStart = gradeHistory - suppliedGradePrior < 1 ? 1 : gradeHistory - suppliedGradePrior;
                    for (int j = gradeStart; j <= gradeHistory + suppliedGradeAhead; j++)
                    {
                        // for each standard k
                        for (int k = 0; k < suppliedStandardCount; k++)
                        {
                            // set standard key
                            string standardKey = (i + 1).ToString() + "." + j.ToString() + "." + (k + 1).ToString();
                            // add assessments
                            for (int l = 0; l < suppliedAssessmentCount; l++)
                            {
                                standards.Add(new StudentMetricLearningStandardHistorical()
                                {
                                    StudentUSI = suppliedStudentUSI,
                                    MetricId = suppliedMetricVariantId,
                                    SchoolId = suppliedSchoolId,
                                    SchoolYear = (short)year, //(short)((int)suppliedSchoolYearCurrent - suppliedGradeCount + j + 1),
                                    GradeLevel = Utilities.GetGradeLevelFromSort(j), // suppliedStudentGradeSort - suppliedGradeCount + j + 1
                                    AssessmentTitle = "(" + standardKey + ") SY 2013,District: 35, Science Grade 7 Genetics...",
                                    Version = (short)year,
                                    LearningObjective = suppliedObjectivePrefix + (i + 1).ToString(),
                                    LearningStandard = standardKey,
                                    Description = "(" + standardKey + ") using stuff in science...",
                                    DateAdministration = suppliedAssessmentDate1.AddYears(-(suppliedSchoolYearCurrent - year)).AddMonths(l),
                                    MetricStateTypeId = 3,
                                    Value = "1 of 2"
                                });
                            }
                        }
                    }
                }
            }

            // return list
            return standards.AsQueryable();

        }

        public virtual void Should_return_model_O_G_S_A_supplied()
        {

            // assertions
            // model exists
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.Count, Is.EqualTo(suppliedObjectiveCount));
            // model collections have correct counts
            for (int i = 0; i < suppliedObjectiveCount; i++)
            {
                // grades
                Assert.That(actualModel[i].ObjectiveDescription, Is.EqualTo(suppliedObjectivePrefix + (i + 1).ToString()));
                // grade count should be equal to 3 + grade ahead
                Assert.That(actualModel[i].Grades.Count, Is.EqualTo(3 + suppliedGradeAhead));
                // standards
                for (int j = 0; j < (3 + suppliedGradeAhead); j++)
                {
                    // determine grade expected in chain
                    int gradeExpected = j + (suppliedStudentGradeCurrent - 2);
                    Assert.That(actualModel[i].Grades[j].GradeLevel, Is.EqualTo(Utilities.GetGradeLevelFromSort(gradeExpected)));
                    Assert.That(actualModel[i].Grades[j].Standards.Count, Is.EqualTo(suppliedStandardCount));
                    // assessments
                    for (int k = 0; k < suppliedStandardCount; k++)
                    {
                        Assert.That(actualModel[i].Grades[j].Standards[k].LearningStandard, Is.EqualTo((i + 1) + "." + gradeExpected + "." + (k + 1)));
                        // assessment count depends on prior / ahead
                        int assessmentCount = suppliedAssessmentCount + (suppliedAssessmentCount * suppliedGradePrior);
                        if (gradeExpected > suppliedStudentGradeCurrent)
                        {
                            assessmentCount -= (gradeExpected - suppliedStudentGradeCurrent) * suppliedAssessmentCount;
                        }
                        Assert.That(actualModel[i].Grades[j].Standards[k].Assessments.Count, Is.EqualTo(assessmentCount));
                    }
                }
                // benchmarks should be on each object
                Assert.That(actualModel[i].Benchmarks.Count, Is.EqualTo(3));
                Assert.That(actualModel[i].Benchmarks[0].Assessments.Count, Is.EqualTo(suppliedAssessmentCount));
            }

        }

        public virtual void Should_return_model_O_G_S_A_Assessments_supplied()
        {
            // check that assessment dates are correct
            // get first standard for current grade on first objective
            var standard = actualModel[0].Grades[suppliedGradePrior].Standards[0];
            // check that last assessment date is correct
            Assert.That(standard.AssessmentLast.DateAdministration, Is.EqualTo(suppliedAssessmentDate1.AddMonths(suppliedAssessmentCount - 1)));
            // assessments collection on standard
            for (int j = 0; j < suppliedAssessmentCount; j++)
            {
                Assert.That(standard.Assessments[j].DateAdministration, Is.EqualTo(suppliedAssessmentDate1.AddMonths(j)));
            }
        }

        public virtual void Should_return_model_O_G_S_A_Benchmarks_supplied()
        {
            // check benchmarks
            for (int j = 0; j < suppliedGradePrior - 1; j++)
            {
                // benchmarks are descending from current grade
                Assert.That(actualModel[0].Benchmarks[j].GradeLevel, Is.EqualTo(Utilities.GetGradeLevelFromSort(suppliedStudentGradeCurrent - j)));
                for (int k = 0; k < suppliedAssessmentCount; k++)
                {
                    Assert.That(actualModel[0].Benchmarks[j].Assessments[k].DateAdministration, Is.EqualTo(suppliedAssessmentDate1.AddYears(-j).AddMonths(k)));
                    Assert.That(actualModel[0].Benchmarks[j].Assessments[k].Value, Is.EqualTo(suppliedBenchmarkValueBase + (k + 1).ToString()));
                    Assert.That(actualModel[0].Benchmarks[j].Assessments[k].Version, Is.EqualTo(suppliedSchoolYearCurrent - j));
                }
            }
        }

    }

    [TestFixture]
    // test population of service model for meta data matrix 1 x 12 x 1 x 1
    public class StudentObjectiveGradeStandardServiceFixture_Should_return_model_1_12_1_1 : StudentObjectiveGradeStandardServiceFixtureBaseDimensions
    {

        protected override void EstablishContext()
        {
            suppliedObjectiveCount = 1;
            suppliedGradeCount = 12;
            suppliedStandardCount = 1;
            suppliedAssessmentCount = 1;
            suppliedStudentGradeCurrent = 4;
            base.EstablishContext();
        }

        [Test]
        public void When_Calling_StudentObjectiveGradeStandardService_Should_return_model_1_12_1_1_supplied()
        {
            base.Should_return_model_O_G_S_A_supplied();
        }

        [Test]
        public void When_Calling_StudentObjectiveGradeStandardService_Should_return_model_1_12_1_1_Assessments_supplied()
        {
            base.Should_return_model_O_G_S_A_Assessments_supplied();
        }

        [Test]
        public void When_Calling_StudentObjectiveGradeStandardService_Should_return_model_1_12_1_1_Benchmarks_supplied()
        {
            base.Should_return_model_O_G_S_A_Benchmarks_supplied();
        }

    }

    [TestFixture]
    // test population of service model for meta data matrix 1 x 12 x 3 x 4
    public class StudentObjectiveGradeStandardServiceFixture_should_return_model_1_12_3_4 : StudentObjectiveGradeStandardServiceFixtureBaseDimensions
    {

        // condition matrix dimensions
        protected override void EstablishContext()
        {
            suppliedObjectiveCount = 1;
            suppliedGradeCount = 12;
            suppliedStandardCount = 3;
            suppliedAssessmentCount = 4;
            suppliedStudentGradeCurrent = 5;
            base.EstablishContext();
        }

        [Test]
        public void When_Calling_StudentObjectiveGradeStandardService_Should_return_model_1_12_3_4_supplied()
        {
            base.Should_return_model_O_G_S_A_supplied();
        }

        [Test]
        public void When_Calling_StudentObjectiveGradeStandardService_Should_return_model_1_12_3_4_Assessments_supplied()
        {
            base.Should_return_model_O_G_S_A_Assessments_supplied();
        }

        [Test]
        public void When_Calling_StudentObjectiveGradeStandardService_Should_return_model_1_12_3_4_Benchmarks_supplied()
        {
            base.Should_return_model_O_G_S_A_Benchmarks_supplied();
        }

    }

    [TestFixture]
    // test population of service model for meta data matrix 2 x 12 x 2 x 2
    public class StudentObjectiveGradeStandardServiceFixture_Should_return_model_2_12_2_2 : StudentObjectiveGradeStandardServiceFixtureBaseDimensions
    {

        // condition matrix dimensions
        protected override void EstablishContext()
        {
            suppliedObjectiveCount = 2;
            suppliedGradeCount = 12;
            suppliedStandardCount = 2;
            suppliedAssessmentCount = 2;
            suppliedStudentGradeCurrent = 6;
            base.EstablishContext();
        }

        [Test]
        public void When_Calling_StudentObjectiveGradeStandardService_Should_return_model_2_12_2_2_supplied()
        {
            base.Should_return_model_O_G_S_A_supplied();
        }

        [Test]
        public void When_Calling_StudentObjectiveGradeStandardService_Should_return_model_2_12_2_2_Assessments_supplied()
        {
            base.Should_return_model_O_G_S_A_Assessments_supplied();
        }

        [Test]
        public void When_Calling_StudentObjectiveGradeStandardService_Should_return_model_2_12_2_2_Benchmarks_supplied()
        {
            base.Should_return_model_O_G_S_A_Benchmarks_supplied();
        }

    }

    [TestFixture]
    // test population of service model for meta data matrix 2 x 12 x 4 x 5
    public class StudentObjectiveGradeStandardServiceFixture_Should_return_model_2_12_4_5 : StudentObjectiveGradeStandardServiceFixtureBaseDimensions
    {

        // condition matrix dimensions
        protected override void EstablishContext()
        {
            suppliedObjectiveCount = 2;
            suppliedGradeCount = 12;
            suppliedStandardCount = 4;
            suppliedAssessmentCount = 5;
            suppliedStudentGradeCurrent = 7;
            base.EstablishContext();
        }

        [Test]
        public void When_Calling_StudentObjectiveGradeStandardService_Should_return_model_2_12_4_5_supplied()
        {
            base.Should_return_model_O_G_S_A_supplied();
        }

        [Test]
        public void When_Calling_StudentObjectiveGradeStandardService_Should_return_model_2_12_4_5_Assessments_supplied()
        {
            base.Should_return_model_O_G_S_A_Assessments_supplied();
        }

        [Test]
        public void When_Calling_StudentObjectiveGradeStandardService_Should_return_model_2_12_4_5_Benchmarks_supplied()
        {
            base.Should_return_model_O_G_S_A_Benchmarks_supplied();
        }

    }

    [TestFixture]
    // test population of service model for meta data matrix 3 x 12 x 3 x 3
    public class StudentObjectiveGradeStandardServiceFixture_should_return_model_3_12_3_3 : StudentObjectiveGradeStandardServiceFixtureBaseDimensions
    {

        // condition matrix dimensions
        protected override void EstablishContext()
        {
            suppliedObjectiveCount = 3;
            suppliedGradeCount = 12;
            suppliedStandardCount = 3;
            suppliedAssessmentCount = 3;
            suppliedStudentGradeCurrent = 8;
            base.EstablishContext();
        }

        [Test]
        public void When_Calling_StudentObjectiveGradeStandardService_Should_return_model_3_12_3_3_supplied()
        {
            base.Should_return_model_O_G_S_A_supplied();
        }

        [Test]
        public void When_Calling_StudentObjectiveGradeStandardService_Should_return_model_3_12_3_3_Assessments_supplied()
        {
            base.Should_return_model_O_G_S_A_Assessments_supplied();
        }

        [Test]
        public void When_Calling_StudentObjectiveGradeStandardService_Should_return_model_3_12_3_3_Benchmarks_supplied()
        {
            base.Should_return_model_O_G_S_A_Benchmarks_supplied();
        }

    }

    [TestFixture]
    // test population of service model for meta data matrix 3 x 12 x 5 x 6
    public class StudentObjectiveGradeStandardServiceFixture_Should_return_model_3_12_5_6 : StudentObjectiveGradeStandardServiceFixtureBaseDimensions
    {

        // condition matrix dimensions
        protected override void EstablishContext()
        {
            suppliedObjectiveCount = 3;
            suppliedGradeCount = 12;
            suppliedStandardCount = 5;
            suppliedAssessmentCount = 6;
            suppliedStudentGradeCurrent = 9;
            base.EstablishContext();
        }

        [Test]
        public void When_Calling_StudentObjectiveGradeStandardService_Should_return_model_3_12_5_6_supplied()
        {
            base.Should_return_model_O_G_S_A_supplied();
        }

        [Test]
        public void When_Calling_StudentObjectiveGradeStandardService_Should_return_model_3_12_5_6_Assessments_supplied()
        {
            base.Should_return_model_O_G_S_A_Assessments_supplied();
        }

        [Test]
        public void When_Calling_StudentObjectiveGradeStandardService_Should_return_model_3_12_5_6_Benchmarks_supplied()
        {
            base.Should_return_model_O_G_S_A_Benchmarks_supplied();
        }

    }

    [TestFixture]
    // test targeted population of service model for meta data matrix
    public class StudentObjectiveGradeStandardServiceFixture_Should_return_specific_assessment_3_3_3_1 : StudentObjectiveGradeStandardServiceFixtureBase
    {

        // condition matrix dimensions
        protected override void EstablishContext()
        {
            suppliedObjectiveCount = 3;
            suppliedGradeCount = 3;
            suppliedGradeStart = 1;
            suppliedStudentGradeCurrent = 3;
            suppliedGradePrior = -1;
            suppliedStandardCount = 3;
            suppliedAssessmentCount = 1;
            base.EstablishContext();
        }

        // condition test data for single assessment within matrix
        protected override IQueryable<StudentMetricLearningStandard> GetStandardsCurrent()
        {

            // create new standard list
            var standards = new List<StudentMetricLearningStandard>();

            // add assessment
            standards.Add(new StudentMetricLearningStandard()
            {
                StudentUSI = suppliedStudentUSI,
                MetricId = suppliedMetricVariantId,
                SchoolId = suppliedSchoolId,
                GradeLevel = Utilities.GetGradeLevelFromSort(suppliedStudentGradeCurrent),
                AssessmentTitle = "Assessment Title",
                Version = suppliedSchoolYearCurrent,
                LearningObjective = suppliedObjectivePrefix + "2",
                LearningStandard = "2.3.2",
                Description = "using stuff in science...",
                DateAdministration = suppliedAssessmentDate1.AddMonths(0),
                MetricStateTypeId = 3,
                Value = "1 of 2"
            });

            // return list
            return standards.AsQueryable();

        }

        [Test]
        public void When_Calling_StudentObjectiveGradeStandardServiceFixture_Should_return_specific_assessment_3_3_3_1_supplied()
        {

            // assertions
            // model exists
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.Count, Is.EqualTo(suppliedObjectiveCount));
            // assessment record is in right place
            Assert.That(actualModel[1].Grades[2].Standards[1].LearningStandard, Is.EqualTo("2.3.2"));
            Assert.That(actualModel[1].Grades[2].Standards[1].Assessments.Count, Is.EqualTo(suppliedAssessmentCount));
            // other collections are empty
            Assert.That(actualModel[0].Grades[0].Standards[0].Assessments.Count, Is.EqualTo(0));
            Assert.That(actualModel[2].Grades[2].Standards[2].Assessments.Count, Is.EqualTo(0));
            // benchmarks should be on each objective
            Assert.That(actualModel[0].Benchmarks.Count, Is.EqualTo(suppliedAssessmentCount));
            Assert.That(actualModel[0].Benchmarks[0].Assessments.Count, Is.EqualTo(suppliedAssessmentCount));
            Assert.That(actualModel[2].Benchmarks.Count, Is.EqualTo(suppliedAssessmentCount));
            Assert.That(actualModel[2].Benchmarks[0].Assessments.Count, Is.EqualTo(suppliedAssessmentCount));

            // check that assessment dates are correct
            var standard = actualModel[1].Grades[2].Standards[1];
            Assert.That(standard.Assessments[0].DateAdministration, Is.EqualTo(suppliedAssessmentDate1.AddMonths(0)));
            // benchmarks
            Assert.That(actualModel[0].Benchmarks[0].Assessments[0].DateAdministration, Is.EqualTo(suppliedAssessmentDate1.AddMonths(0)));
            Assert.That(actualModel[0].Benchmarks[0].Assessments[0].Value, Is.EqualTo(suppliedBenchmarkValueBase + "1"));

            // check that last assessment date is correct
            Assert.That(standard.AssessmentLast.DateAdministration, Is.EqualTo(suppliedAssessmentDate1.AddMonths(0)));

        }

    }

    [TestFixture]
    // test random objective/grade/assessments and indicator roll-up
    public class StudentObjectiveGradeStandardServiceFixture_Should_return_random_assessment_3_3_3_3 : StudentObjectiveGradeStandardServiceFixtureBase
    {

        // condition matrix dimensions
        protected override void EstablishContext()
        {
            suppliedObjectiveCount = 3;
            suppliedGradeCount = 3;
            suppliedStandardCount = 3;
            suppliedAssessmentCount = 3;
            suppliedStudentGradeCurrent = 3;
            base.EstablishContext();
        }

        // 
        protected override IQueryable<StudentMetricBenchmarkAssessment> GetBenchmarksCurrent()
        {

            // create new standard list
            var benchmarks = new List<StudentMetricBenchmarkAssessment>();

            // add benchmarks
            benchmarks.Add(new StudentMetricBenchmarkAssessment()
            {
                StudentUSI = suppliedStudentUSI,
                SchoolId = suppliedSchoolId,
                MetricId = StudentObjectiveGradeStandardService.GetBenchmarkMetricId(suppliedMetricVariantId),
                Date = suppliedAssessmentDate1.AddMonths(4),
                AssessmentTitle = "Assessment Title",
                Version = 2013,
                Value = suppliedBenchmarkValueBase + "4"
            });
            benchmarks.Add(new StudentMetricBenchmarkAssessment()
            {
                StudentUSI = suppliedStudentUSI,
                SchoolId = suppliedSchoolId,
                MetricId = StudentObjectiveGradeStandardService.GetBenchmarkMetricId(suppliedMetricVariantId),
                Date = suppliedAssessmentDate1.AddMonths(5),
                AssessmentTitle = "Assessment Title",
                Version = 2013,
                Value = suppliedBenchmarkValueBase + "5"
            });
            benchmarks.Add(new StudentMetricBenchmarkAssessment()
            {
                StudentUSI = suppliedStudentUSI,
                SchoolId = suppliedSchoolId,
                MetricId = StudentObjectiveGradeStandardService.GetBenchmarkMetricId(suppliedMetricVariantId),
                Date = suppliedAssessmentDate1.AddMonths(6),
                AssessmentTitle = "Assessment Title",
                Version = 2013,
                Value = suppliedBenchmarkValueBase + "6"
            });

            // return list
            return benchmarks.AsQueryable();

        }

        // condition test data for random assessments within matrix
        protected override IQueryable<StudentMetricLearningStandard> GetStandardsCurrent()
        {

            // create new standard list
            var standards = new List<StudentMetricLearningStandard>();

            // add assessment
            standards.Add(new StudentMetricLearningStandard()
            {
                StudentUSI = suppliedStudentUSI,
                MetricId = suppliedMetricVariantId,
                SchoolId = suppliedSchoolId,
                GradeLevel = Utilities.GetGradeLevelFromSort(3),
                AssessmentTitle = "Assessment Title",
                Version = 2013,
                LearningObjective = suppliedObjectivePrefix + "1",
                LearningStandard = "1.3.2",
                Description = "using stuff in science...",
                DateAdministration = suppliedAssessmentDate1.AddMonths(4),
                MetricStateTypeId = 3,
                Value = "1 of 2"
            });
            standards.Add(new StudentMetricLearningStandard()
            {
                StudentUSI = suppliedStudentUSI,
                MetricId = suppliedMetricVariantId,
                SchoolId = suppliedSchoolId,
                GradeLevel = Utilities.GetGradeLevelFromSort(1),
                AssessmentTitle = "Assessment Title",
                Version = 2013,
                LearningObjective = suppliedObjectivePrefix + "2",
                LearningStandard = "2.1.3",
                Description = "using stuff in science...",
                DateAdministration = suppliedAssessmentDate1.AddMonths(5),
                MetricStateTypeId = 1,
                Value = "1 of 2"
            });
            standards.Add(new StudentMetricLearningStandard()
            {
                StudentUSI = suppliedStudentUSI,
                MetricId = suppliedMetricVariantId,
                SchoolId = suppliedSchoolId,
                GradeLevel = Utilities.GetGradeLevelFromSort(2),
                AssessmentTitle = "Assessment Title",
                Version = 2013,
                LearningObjective = suppliedObjectivePrefix + "3",
                LearningStandard = "3.2.1",
                Description = "using stuff in science...",
                DateAdministration = suppliedAssessmentDate1.AddMonths(6),
                MetricStateTypeId = 6,
                Value = "1 of 2"
            });

            // return list
            return standards.AsQueryable();

        }

        [Test]
        public void When_calling_StudentObjectiveGradeStandardServiceFixture_should_return_random_assessment_3_3_3_R_supplied()
        {

            // assertions
            // model exists
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.Count, Is.EqualTo(3));
            // assessment records are in the right place
            Assert.That(actualModel[0].Grades[2].Standards[1].LearningStandard, Is.EqualTo("1.3.2"));
            Assert.That(actualModel[0].Grades[2].Standards[1].Assessments.Count, Is.EqualTo(1));
            Assert.That(actualModel[0].Grades[2].Standards[1].AssessmentLast.DateAdministration, Is.EqualTo(suppliedAssessmentDate1.AddMonths(4)));
            Assert.That(actualModel[1].Grades[0].Standards[2].LearningStandard, Is.EqualTo("2.1.3"));
            //Assert.That(actualModel[1].Grades[0].Standards[2].Assessments.Count, Is.EqualTo(1));
            //Assert.That(actualModel[1].Grades[0].Standards[2].AssessmentLast.DateAdministration, Is.EqualTo(suppliedAssessmentDate1.AddMonths(5)));
            Assert.That(actualModel[2].Grades[1].Standards[0].LearningStandard, Is.EqualTo("3.2.1"));
            //Assert.That(actualModel[2].Grades[1].Standards[0].Assessments.Count, Is.EqualTo(1));
            //Assert.That(actualModel[2].Grades[1].Standards[0].AssessmentLast.DateAdministration, Is.EqualTo(suppliedAssessmentDate1.AddMonths(6)));
            // benchmarks
            Assert.That(actualModel[0].Benchmarks.Count, Is.EqualTo(3));
            Assert.That(actualModel[0].Benchmarks[0].Assessments[0].DateAdministration, Is.EqualTo(suppliedAssessmentDate1.AddMonths(4)));
            Assert.That(actualModel[0].Benchmarks[0].Assessments[1].DateAdministration, Is.EqualTo(suppliedAssessmentDate1.AddMonths(5)));
            Assert.That(actualModel[0].Benchmarks[0].Assessments[2].DateAdministration, Is.EqualTo(suppliedAssessmentDate1.AddMonths(6)));
            // grade (objective) indicators
            // one 3 should be 3
            //Assert.That(actualModel[0].Grades[2].MetricStateTypeId.GetValueOrDefault(), Is.EqualTo(3));
            // one 1 should be 6
            //Assert.That(actualModel[1].Grades[0].MetricStateTypeId.GetValueOrDefault(), Is.EqualTo(6));
            // one 6 should be 6
            //Assert.That(actualModel[2].Grades[1].MetricStateTypeId.GetValueOrDefault(), Is.EqualTo(6));

        }

    }

    // second derived base for test of indicator roll-up from last assessment to grade (objective) level
    public abstract class StudentObjectiveGradeStandardServiceFixtureBaseIndicator : StudentObjectiveGradeStandardServiceFixtureBase
    {

        // condition matrix dimensions
        protected override void EstablishContext()
        {
            suppliedObjectiveCount = 1;
            suppliedGradeCount = 1;
            suppliedStudentGradeCurrent = 1;
            suppliedIndicatorResult = SetIndicatorValues();
            base.EstablishContext();
        }

        // condition standards for test
        protected override IQueryable<StudentMetricLearningStandard> GetStandardsCurrent()
        {

            // create new standard list
            var standards = new List<StudentMetricLearningStandard>();

            // add 1st assessment
            standards.Add(new StudentMetricLearningStandard()
            {
                StudentUSI = suppliedStudentUSI,
                MetricId = suppliedMetricVariantId,
                SchoolId = suppliedSchoolId,
                GradeLevel = Utilities.GetGradeLevelFromSort(1),
                LearningObjective = suppliedObjectivePrefix + "1",
                LearningStandard = "1.1.1",
                DateAdministration = suppliedAssessmentDate1.AddMonths(8),
                MetricStateTypeId = suppliedIndicatorValues[0],
            });
            // add 2nd assessment
            if (suppliedIndicatorKey > 999)
            {
                standards.Add(new StudentMetricLearningStandard()
                {
                    StudentUSI = suppliedStudentUSI,
                    MetricId = suppliedMetricVariantId,
                    SchoolId = suppliedSchoolId,
                    GradeLevel = Utilities.GetGradeLevelFromSort(1),
                    LearningObjective = suppliedObjectivePrefix + "1",
                    LearningStandard = "1.1.2",
                    DateAdministration = suppliedAssessmentDate1.AddMonths(8),
                    MetricStateTypeId = suppliedIndicatorValues[1],
                });
                // add 3rd assessment
            }
            if (suppliedIndicatorKey > 9999)
            {
                standards.Add(new StudentMetricLearningStandard()
                {
                    StudentUSI = suppliedStudentUSI,
                    MetricId = suppliedMetricVariantId,
                    SchoolId = suppliedSchoolId,
                    GradeLevel = Utilities.GetGradeLevelFromSort(1),
                    LearningObjective = suppliedObjectivePrefix + "1",
                    LearningStandard = "1.1.3",
                    DateAdministration = suppliedAssessmentDate1.AddMonths(8),
                    MetricStateTypeId = suppliedIndicatorValues[2],
                });
            }
            // add 4th assessment
            if (suppliedIndicatorKey > 99999)
            {
                standards.Add(new StudentMetricLearningStandard()
                {
                    StudentUSI = suppliedStudentUSI,
                    MetricId = suppliedMetricVariantId,
                    SchoolId = suppliedSchoolId,
                    GradeLevel = Utilities.GetGradeLevelFromSort(1),
                    LearningObjective = suppliedObjectivePrefix + "1",
                    LearningStandard = "1.1.4",
                    DateAdministration = suppliedAssessmentDate1.AddMonths(8),
                    MetricStateTypeId = suppliedIndicatorValues[3],
                });
            }
            // add 5th assessment
            if (suppliedIndicatorKey > 999999)
            {
                standards.Add(new StudentMetricLearningStandard()
                {
                    StudentUSI = suppliedStudentUSI,
                    MetricId = suppliedMetricVariantId,
                    SchoolId = suppliedSchoolId,
                    GradeLevel = Utilities.GetGradeLevelFromSort(1),
                    LearningObjective = suppliedObjectivePrefix + "1",
                    LearningStandard = "1.1.5",
                    DateAdministration = suppliedAssessmentDate1.AddMonths(8),
                    MetricStateTypeId = suppliedIndicatorValues[4],
                });
            }

            // return list
            return standards.AsQueryable();

        }

        // set default indicator values
        protected int SetIndicatorValues()
        {

            // parse out integer into state values
            // 1 assessment
            if (suppliedIndicatorKey < 999)
            {
                suppliedIndicatorValues = new int[] {
                    int.Parse(suppliedIndicatorKey.ToString().Substring(1, 1))
                };
                // return last value
                return int.Parse(suppliedIndicatorKey.ToString().Substring(2, 1));
            }
            // 2 assessments
            else if (suppliedIndicatorKey < 9999)
            {
                suppliedIndicatorValues = new int[] {
                    int.Parse(suppliedIndicatorKey.ToString().Substring(1, 1)),
                    int.Parse(suppliedIndicatorKey.ToString().Substring(2, 1))
                };
                // return last value
                return int.Parse(suppliedIndicatorKey.ToString().Substring(3, 1));
            }
            // 3 assessments
            else if (suppliedIndicatorKey < 99999)
            {
                suppliedIndicatorValues = new int[] {
                    int.Parse(suppliedIndicatorKey.ToString().Substring(1, 1)),
                    int.Parse(suppliedIndicatorKey.ToString().Substring(2, 1)),
                    int.Parse(suppliedIndicatorKey.ToString().Substring(3, 1))
                };
                // return last value
                return int.Parse(suppliedIndicatorKey.ToString().Substring(4, 1));
            }
            // 4 assessments
            else if (suppliedIndicatorKey < 999999)
            {
                suppliedIndicatorValues = new int[] {
                    int.Parse(suppliedIndicatorKey.ToString().Substring(1, 1)),
                    int.Parse(suppliedIndicatorKey.ToString().Substring(2, 1)),
                    int.Parse(suppliedIndicatorKey.ToString().Substring(3, 1)),
                    int.Parse(suppliedIndicatorKey.ToString().Substring(4, 1))
                };
                // return last value
                return int.Parse(suppliedIndicatorKey.ToString().Substring(5, 1));
            }
            // 5 assessments
            else if (suppliedIndicatorKey < 9999999)
            {
                suppliedIndicatorValues = new int[] {
                    int.Parse(suppliedIndicatorKey.ToString().Substring(1, 1)),
                    int.Parse(suppliedIndicatorKey.ToString().Substring(2, 1)),
                    int.Parse(suppliedIndicatorKey.ToString().Substring(3, 1)),
                    int.Parse(suppliedIndicatorKey.ToString().Substring(4, 1)),
                    int.Parse(suppliedIndicatorKey.ToString().Substring(5, 1))
                };
                // return last value
                return int.Parse(suppliedIndicatorKey.ToString().Substring(6, 1));
            }


            // default return value
            return 0;

        }

    }

    [TestFixture] // 9 - 1 - 6
    // 1 standard : state of 1 : result state of 6
    // 9 is meaningless prefix
    public class When_calling_StudentObjectiveGradeStandardServiceFixture_should_return_objective_indicator_1_1_1_916 : StudentObjectiveGradeStandardServiceFixtureBaseIndicator
    {

        // condition standards indicator to be parsed
        protected override void EstablishContext()
        {
            suppliedIndicatorKey = 916;
            suppliedStandardCount = 1;
            base.EstablishContext();
        }

        [Test]
        public void When_calling_StudentObjectiveGradeStandardServiceFixture_should_return_objective_indicator_1_1_1_916_supplied()
        {

            // assertions
            // model exists
            Assert.That(actualModel, Is.Not.Null);
            // objective count
            Assert.That(actualModel.Count, Is.EqualTo(1));
            // standards configuration
            Assert.That(actualModel[0].Grades[0].Standards.Count, Is.EqualTo(suppliedStandardCount));
            Assert.That(actualModel[0].Grades[0].Standards[0].LearningStandard, Is.EqualTo("1.1.1"));
            Assert.That(actualModel[0].Grades[0].Standards[0].Assessments.Count, Is.EqualTo(1));
            // grade (objective) indicators
            //Assert.That(actualModel[0].Grades[0].MetricStateTypeId.GetValueOrDefault(), Is.EqualTo(suppliedIndicatorResult));
            // change to value presentation
            Assert.That(actualModel[0].Grades[0].Value, Is.EqualTo("1 of 1"));
        }

    }

    [TestFixture] // 9 - 6 - 6
    // 1 standard : state of 6 : result state of 6
    public class StudentObjectiveGradeStandardServiceFixture_Should_return_objective_indicator_1_1_1_966 : StudentObjectiveGradeStandardServiceFixtureBaseIndicator
    {

        // condition standards indicator to be parsed
        protected override void EstablishContext()
        {
            suppliedIndicatorKey = 966;
            suppliedStandardCount = 1;
            base.EstablishContext();
        }

        [Test]
        public void When_calling_StudentObjectiveGradeStandardServiceFixture_should_return_objective_indicator_1_1_1_966_supplied()
        {
            // check for correct grade (objective) indicator value
            Assert.That(actualModel[0].Grades[0].Standards.Count, Is.EqualTo(suppliedStandardCount));
            //Assert.That(actualModel[0].Grades[0].MetricStateTypeId.GetValueOrDefault(), Is.EqualTo(suppliedIndicatorResult));
            // change to value presentation
            Assert.That(actualModel[0].Grades[0].Value, Is.EqualTo("1 of 1"));
        }

    }

    [TestFixture] // 9 - 3 - 3
    // 1 standard : state of 3 : result state of 3
    public class StudentObjectiveGradeStandardServiceFixture_Should_return_objective_indicator_1_1_1_933 : StudentObjectiveGradeStandardServiceFixtureBaseIndicator
    {

        // condition standards indicator to be parsed
        protected override void EstablishContext()
        {
            suppliedIndicatorKey = 933;
            suppliedStandardCount = 1;
            base.EstablishContext();
        }

        [Test]
        public void When_calling_StudentObjectiveGradeStandardServiceFixture_should_return_objective_indicator_1_1_1_933_supplied()
        {
            // check for correct grade (objective) indicator value
            Assert.That(actualModel[0].Grades[0].Standards.Count, Is.EqualTo(suppliedStandardCount));
            //Assert.That(actualModel[0].Grades[0].MetricStateTypeId.GetValueOrDefault(), Is.EqualTo(suppliedIndicatorResult));
            Assert.That(actualModel[0].Grades[0].Value, Is.EqualTo("0 of 1"));
        }

    }

    [TestFixture] // 9 - 11 - 6
    // 2 standarda : state of 1 | 1 : result state of 6
    public class StudentObjectiveGradeStandardServiceFixture_Should_return_objective_indicator_1_1_2_9116 : StudentObjectiveGradeStandardServiceFixtureBaseIndicator
    {

        // condition standards indicator to be parsed
        protected override void EstablishContext()
        {
            suppliedIndicatorKey = 9116;
            suppliedStandardCount = 2;
            base.EstablishContext();
        }

        [Test]
        public void When_calling_StudentObjectiveGradeStandardServiceFixture_should_return_objective_indicator_1_1_2_9116_supplied()
        {
            // check for correct grade (objective) indicator value
            Assert.That(actualModel[0].Grades[0].Standards.Count, Is.EqualTo(suppliedStandardCount));
            //Assert.That(actualModel[0].Grades[0].MetricStateTypeId.GetValueOrDefault(), Is.EqualTo(suppliedIndicatorResult));
            Assert.That(actualModel[0].Grades[0].Value, Is.EqualTo("2 of 2"));
        }

    }

    [TestFixture] // 9 - 61 - 6
    // 2 standarda : state of 6 | 1 : result state of 6
    public class StudentObjectiveGradeStandardServiceFixture_Should_return_objective_indicator_1_1_2_9616 : StudentObjectiveGradeStandardServiceFixtureBaseIndicator
    {

        // condition standards indicator to be parsed
        protected override void EstablishContext()
        {
            suppliedIndicatorKey = 9616;
            suppliedStandardCount = 2;
            base.EstablishContext();
        }

        [Test]
        public void When_calling_StudentObjectiveGradeStandardServiceFixture_should_return_objective_indicator_1_1_2_9616_supplied()
        {
            // check for correct grade (objective) indicator value
            Assert.That(actualModel[0].Grades[0].Standards.Count, Is.EqualTo(suppliedStandardCount));
            //Assert.That(actualModel[0].Grades[0].MetricStateTypeId.GetValueOrDefault(), Is.EqualTo(suppliedIndicatorResult));
            Assert.That(actualModel[0].Grades[0].Value, Is.EqualTo("2 of 2"));
        }

    }

    [TestFixture] // 9 - 31 - 3
    // 2 standarda : state of 3 | 1 : result state of 3
    public class StudentObjectiveGradeStandardServiceFixture_Should_return_objective_indicator_1_1_2_9313 : StudentObjectiveGradeStandardServiceFixtureBaseIndicator
    {

        // condition standards indicator to be parsed
        protected override void EstablishContext()
        {
            suppliedIndicatorKey = 9313;
            suppliedStandardCount = 2;
            base.EstablishContext();
        }

        [Test]
        public void When_calling_StudentObjectiveGradeStandardServiceFixture_should_return_objective_indicator_1_1_2_9313_supplied()
        {
            // check for correct grade (objective) indicator value
            Assert.That(actualModel[0].Grades[0].Standards.Count, Is.EqualTo(suppliedStandardCount));
            //Assert.That(actualModel[0].Grades[0].MetricStateTypeId.GetValueOrDefault(), Is.EqualTo(suppliedIndicatorResult));
            Assert.That(actualModel[0].Grades[0].Value, Is.EqualTo("1 of 2"));
        }

    }

    [TestFixture] // 9 - 33 - 3
    // 2 standarda : state of 3 | 3 : result state of 3
    public class StudentObjectiveGradeStandardServiceFixture_Should_return_objective_indicator_1_1_2_9333 : StudentObjectiveGradeStandardServiceFixtureBaseIndicator
    {

        // condition standards indicator to be parsed
        protected override void EstablishContext()
        {
            suppliedIndicatorKey = 9333;
            suppliedStandardCount = 2;
            base.EstablishContext();
        }

        [Test]
        public void When_calling_StudentObjectiveGradeStandardServiceFixture_should_return_objective_indicator_1_1_2_9333_supplied()
        {
            // check for correct grade (objective) indicator value
            Assert.That(actualModel[0].Grades[0].Standards.Count, Is.EqualTo(suppliedStandardCount));
            //Assert.That(actualModel[0].Grades[0].MetricStateTypeId.GetValueOrDefault(), Is.EqualTo(suppliedIndicatorResult));
            Assert.That(actualModel[0].Grades[0].Value, Is.EqualTo("0 of 2"));
        }

    }

    [TestFixture] // 9 - 111 - 6
    // 3 standarda : state of 1 | 1 | 1 : result state of 6
    public class StudentObjectiveGradeStandardServiceFixture_Should_return_objective_indicator_1_1_3_91116 : StudentObjectiveGradeStandardServiceFixtureBaseIndicator
    {

        // condition standards indicator to be parsed
        protected override void EstablishContext()
        {
            suppliedIndicatorKey = 91116;
            suppliedStandardCount = 3;
            base.EstablishContext();
        }

        [Test]
        public void When_calling_StudentObjectiveGradeStandardServiceFixture_should_return_objective_indicator_1_1_3_91116_supplied()
        {
            // check for correct grade (objective) indicator value
            Assert.That(actualModel[0].Grades[0].Standards.Count, Is.EqualTo(suppliedStandardCount));
            //Assert.That(actualModel[0].Grades[0].MetricStateTypeId.GetValueOrDefault(), Is.EqualTo(suppliedIndicatorResult));
            Assert.That(actualModel[0].Grades[0].Value, Is.EqualTo("3 of 3"));
        }

    }

    [TestFixture] // 9 - 113 - 3
    // 3 standarda : state of 1 | 1 | 3 : result state of 3
    public class StudentObjectiveGradeStandardServiceFixture_Should_return_objective_indicator_1_1_3_91133 : StudentObjectiveGradeStandardServiceFixtureBaseIndicator
    {

        // condition standards indicator to be parsed
        protected override void EstablishContext()
        {
            suppliedIndicatorKey = 91133;
            suppliedStandardCount = 3;
            base.EstablishContext();
        }

        [Test]
        public void When_calling_StudentObjectiveGradeStandardServiceFixture_should_return_objective_indicator_1_1_3_91133_supplied()
        {
            // check for correct grade (objective) indicator value
            Assert.That(actualModel[0].Grades[0].Standards.Count, Is.EqualTo(suppliedStandardCount));
            //Assert.That(actualModel[0].Grades[0].MetricStateTypeId.GetValueOrDefault(), Is.EqualTo(suppliedIndicatorResult));
            Assert.That(actualModel[0].Grades[0].Value, Is.EqualTo("2 of 3"));
        }

    }

    [TestFixture] // 9 - 163 - 3
    // 3 standarda : state of 1 | 6 | 3 : result state of 3
    public class StudentObjectiveGradeStandardServiceFixture_Should_return_objective_indicator_1_1_3_91633 : StudentObjectiveGradeStandardServiceFixtureBaseIndicator
    {

        // condition standards indicator to be parsed
        protected override void EstablishContext()
        {
            suppliedIndicatorKey = 91633;
            suppliedStandardCount = 3;
            base.EstablishContext();
        }

        [Test]
        public void When_calling_StudentObjectiveGradeStandardServiceFixture_should_return_objective_indicator_1_1_3_91633_supplied()
        {
            // check for correct grade (objective) indicator value
            Assert.That(actualModel[0].Grades[0].Standards.Count, Is.EqualTo(suppliedStandardCount));
            //Assert.That(actualModel[0].Grades[0].MetricStateTypeId.GetValueOrDefault(), Is.EqualTo(suppliedIndicatorResult));
            Assert.That(actualModel[0].Grades[0].Value, Is.EqualTo("2 of 3"));
        }

    }

    [TestFixture] // 9 - 1111 - 6
    // 4 standarda : state of 1 | 1 | 1 | 1 : result state of 6
    public class StudentObjectiveGradeStandardServiceFixture_Should_return_objective_indicator_1_1_4_911116 : StudentObjectiveGradeStandardServiceFixtureBaseIndicator
    {

        // condition standards indicator to be parsed
        protected override void EstablishContext()
        {
            suppliedIndicatorKey = 911116;
            suppliedStandardCount = 4;
            base.EstablishContext();
        }

        [Test]
        public void When_calling_StudentObjectiveGradeStandardServiceFixture_should_return_objective_indicator_1_1_4_911116_supplied()
        {
            // check for correct grade (objective) indicator value
            Assert.That(actualModel[0].Grades[0].Standards.Count, Is.EqualTo(suppliedStandardCount));
            //Assert.That(actualModel[0].Grades[0].MetricStateTypeId.GetValueOrDefault(), Is.EqualTo(suppliedIndicatorResult));
            Assert.That(actualModel[0].Grades[0].Value, Is.EqualTo("4 of 4"));
        }

    }

    [TestFixture] // 9 - 1113 - 1
    // 4 standarda : state of 1 | 1 | 1 | 3 : result state of 1
    public class StudentObjectiveGradeStandardServiceFixture_Should_return_objective_indicator_1_1_4_911131 : StudentObjectiveGradeStandardServiceFixtureBaseIndicator
    {

        // condition standards indicator to be parsed
        protected override void EstablishContext()
        {
            suppliedIndicatorKey = 911131;
            suppliedStandardCount = 4;
            base.EstablishContext();
        }

        [Test]
        public void When_calling_StudentObjectiveGradeStandardServiceFixture_should_return_objective_indicator_1_1_4_911131_supplied()
        {
            // check for correct grade (objective) indicator value
            Assert.That(actualModel[0].Grades[0].Standards.Count, Is.EqualTo(suppliedStandardCount));
            //Assert.That(actualModel[0].Grades[0].MetricStateTypeId.GetValueOrDefault(), Is.EqualTo(suppliedIndicatorResult));
            Assert.That(actualModel[0].Grades[0].Value, Is.EqualTo("3 of 4"));
        }

    }

    [TestFixture] // 9 - 1133 - 3
    // 4 standarda : state of 1 | 1 | 3 | 3 : result state of 3
    public class StudentObjectiveGradeStandardServiceFixture_Should_return_objective_indicator_1_1_4_911333 : StudentObjectiveGradeStandardServiceFixtureBaseIndicator
    {

        // condition standards indicator to be parsed
        protected override void EstablishContext()
        {
            suppliedIndicatorKey = 911333;
            suppliedStandardCount = 4;
            base.EstablishContext();
        }

        [Test]
        public void When_calling_StudentObjectiveGradeStandardServiceFixture_should_return_objective_indicator_1_1_4_911333_supplied()
        {
            // check for correct grade (objective) indicator value
            Assert.That(actualModel[0].Grades[0].Standards.Count, Is.EqualTo(suppliedStandardCount));
            //Assert.That(actualModel[0].Grades[0].MetricStateTypeId.GetValueOrDefault(), Is.EqualTo(suppliedIndicatorResult));
            Assert.That(actualModel[0].Grades[0].Value, Is.EqualTo("2 of 4"));
        }

    }

    [TestFixture] // 9 - 11111 - 6
    // 5 standarda : state of 1 | 1 | 1 | 1 | 1 : result state of 6
    public class StudentObjectiveGradeStandardServiceFixture_Should_return_objective_indicator_1_1_5_9111116 : StudentObjectiveGradeStandardServiceFixtureBaseIndicator
    {

        // condition standards indicator to be parsed
        protected override void EstablishContext()
        {
            suppliedIndicatorKey = 9111116;
            suppliedStandardCount = 5;
            base.EstablishContext();
        }

        [Test]
        public void When_calling_StudentObjectiveGradeStandardServiceFixture_should_return_objective_indicator_1_1_5_9111116_supplied()
        {
            // check for correct grade (objective) indicator value
            Assert.That(actualModel[0].Grades[0].Standards.Count, Is.EqualTo(suppliedStandardCount));
            //Assert.That(actualModel[0].Grades[0].MetricStateTypeId.GetValueOrDefault(), Is.EqualTo(suppliedIndicatorResult));
            Assert.That(actualModel[0].Grades[0].Value, Is.EqualTo("5 of 5"));
        }

    }

    [TestFixture] // 9 - 11131 - 1
    // 5 standarda : state of 1 | 1 | 1 | 3 | 1 : result state of 1
    public class StudentObjectiveGradeStandardServiceFixture_Should_return_objective_indicator_1_1_5_9111311 : StudentObjectiveGradeStandardServiceFixtureBaseIndicator
    {

        // condition standards indicator to be parsed
        protected override void EstablishContext()
        {
            suppliedIndicatorKey = 9111311;
            suppliedStandardCount = 5;
            base.EstablishContext();
        }

        [Test]
        public void When_calling_StudentObjectiveGradeStandardServiceFixture_should_return_objective_indicator_1_1_5_9111311_supplied()
        {
            // check for correct grade (objective) indicator value
            Assert.That(actualModel[0].Grades[0].Standards.Count, Is.EqualTo(suppliedStandardCount));
            //Assert.That(actualModel[0].Grades[0].MetricStateTypeId.GetValueOrDefault(), Is.EqualTo(suppliedIndicatorResult));
            Assert.That(actualModel[0].Grades[0].Value, Is.EqualTo("4 of 5"));
        }

    }

    [TestFixture] // 9 - 11133 - 3
    // 5 standarda : state of 1 | 1 | 1 | 3 | 3 : result state of 3
    public class StudentObjectiveGradeStandardServiceFixture_Should_return_objective_indicator_1_1_5_9111333 : StudentObjectiveGradeStandardServiceFixtureBaseIndicator
    {

        // condition standards indicator to be parsed
        protected override void EstablishContext()
        {
            suppliedIndicatorKey = 9111333;
            suppliedStandardCount = 5;
            base.EstablishContext();
        }

        [Test]
        public void When_calling_StudentObjectiveGradeStandardServiceFixture_should_return_objective_indicator_1_1_5_9111333_supplied()
        {
            // check for correct grade (objective) indicator value
            Assert.That(actualModel[0].Grades[0].Standards.Count, Is.EqualTo(suppliedStandardCount));
            //Assert.That(actualModel[0].Grades[0].MetricStateTypeId.GetValueOrDefault(), Is.EqualTo(suppliedIndicatorResult));
            Assert.That(actualModel[0].Grades[0].Value, Is.EqualTo("3 of 5"));
        }

    }

    [TestFixture]
    // test of grade window revision (only return current and previous two grades regardless of meta data configuration)
    public class StudentObjectiveGradeStandardServiceFixture_Should_return_model_5_5_3_1 : StudentObjectiveGradeStandardServiceFixtureBase
    {

        // configure test parameters
        protected override void EstablishContext()
        {
            suppliedObjectiveCount = 5;
            suppliedGradeCount = 5;
            suppliedStandardCount = 5;
            suppliedAssessmentCount = 1;
            suppliedStudentGradeCurrent = 7;
            base.EstablishContext();
        }

        // configure objective meta data for more than grade window
        protected override List<StudentMetricLearningStandardMetaDataModel> GetObjectives()
        {

            // create new list
            suppliedObjectives = new List<StudentMetricLearningStandardMetaDataModel>();

            // add 5 objectives
            for (int i = 0; i < 5; i++)
            {
                var objective = new StudentMetricLearningStandardMetaDataModel(suppliedMetricVariantId)
                {
                    LearningObjective = suppliedObjectivePrefix + (i + 1)
                };
                // add 5 grades to middle 3 objectives
                if (i >= 1 && i <= 3)
                {
                    for (int j = 3; j < 8; j++)
                    {
                        var grade = new StudentMetricLearningStandardMetaDataModel.GradeModel()
                        {
                            GradeLevel = Utilities.GetGradeLevelFromSort(j + 1),
                            GradeSort = j + 1
                        };
                        // add standards
                        for (int k = 0; k < suppliedStandardCount; k++)
                        {
                            var standard = new StudentMetricLearningStandardMetaDataModel.StandardModel()
                            {
                                LearningStandard = (i + 1) + "." + (j + 1) + "." + (k + 1)
                            };
                            grade.Standards.Add(standard);
                        }
                        objective.Grades.Add(grade);
                    }
                }
                suppliedObjectives.Add(objective);
            }

            // first and last objectives will only have grade 4 | 8 which should not come through
            suppliedObjectives[0].Grades.Add(new StudentMetricLearningStandardMetaDataModel.GradeModel() { GradeLevel = Utilities.GetGradeLevelFromSort(4), GradeSort = 4 });
            suppliedObjectives[4].Grades.Add(new StudentMetricLearningStandardMetaDataModel.GradeModel() { GradeLevel = Utilities.GetGradeLevelFromSort(8), GradeSort = 8 });

            // return list
            return suppliedObjectives;

        }

        // condition test data for learning for grade window
        protected override IQueryable<StudentMetricLearningStandard> GetStandardsCurrent()
        {

            // create new standard list
            var standards = new List<StudentMetricLearningStandard>();

            // for middle three objectives
            for (int i = 1; i < 4; i++)
            {
                // for each grade
                for (int j = 3; j < 7; j++)
                {
                    // for each standard
                    for (int k = 0; k < suppliedStandardCount; k++)
                    {
                        // set standard key
                        string standardKey = (i + 1) + "." + (j + 1) + "." + (k + 1);
                        // add assessments
                        for (int l = 0; l < suppliedAssessmentCount; l++)
                        {
                            standards.Add(new StudentMetricLearningStandard()
                            {
                                StudentUSI = suppliedStudentUSI,
                                MetricId = suppliedMetricVariantId,
                                SchoolId = suppliedSchoolId,
                                GradeLevel = Utilities.GetGradeLevelFromSort(j + 1),
                                AssessmentTitle = "(" + standardKey + ") SY 2013,District: 35, Science Grade 7 Genetics...",
                                Version = suppliedSchoolYearCurrent,
                                LearningObjective = suppliedObjectivePrefix + (i + 1),
                                LearningStandard = standardKey,
                                Description = "(" + standardKey + ") using stuff in science...",
                                DateAdministration = suppliedAssessmentDate1.AddMonths(l),
                                MetricStateTypeId = 3,
                                Value = "1 of 2"
                            });
                        }
                    }
                }
            }

            // return list
            suppliedStandardsCurrent = standards.AsQueryable();
            return suppliedStandardsCurrent;

        }

        [Test]
        public void When_calling_StudentObjectiveGradeStandardServiceFixture_should_return_model_5_5_3_1_supplied()
        {

            // model exists
            Assert.That(actualModel, Is.Not.Null);
            // objective count should be 3 due to first and last objectives only having grades outside window
            Assert.That(suppliedObjectives.Count, Is.EqualTo(5));
            Assert.That(actualModel.Count, Is.EqualTo(3));
            // model collections have correct counts
            for (int i = 0; i < 3; i++)
            {
                // grades
                Assert.That(suppliedObjectives[i + 1].Grades.Count, Is.EqualTo(5));
                Assert.That(actualModel[i].Grades.Count, Is.EqualTo(3));
                for (int j = 0; j < 3; j++)
                {
                    // grade level is correct
                    Assert.That(actualModel[i].Grades[j].GradeLevel, Is.EqualTo(Utilities.GetGradeLevelFromSort(j + 5)));
                    // standards count is correct
                    Assert.That(actualModel[i].Grades[j].Standards.Count, Is.EqualTo(suppliedStandardCount));
                    // standards
                    for (int k = 0; k < suppliedStandardCount; k++)
                    {
                        // learning standard is correct
                        Assert.That(actualModel[i].Grades[j].Standards[k].LearningStandard, Is.EqualTo((i + 2) + "." + (j + 5) + "." + (k + 1)));
                        // assessment count is correct
                        Assert.That(actualModel[i].Grades[j].Standards[k].Assessments.Count, Is.EqualTo(suppliedAssessmentCount));
                    }
                }
            }

        }

    }

    [TestFixture]
    // test of benchmark history absence of data
    public class StudentObjectiveGradeStandardServiceFixture_Should_return_model_no_benchmark_history : StudentObjectiveGradeStandardServiceFixtureBase
    {

        // configure test parameters
        protected override void EstablishContext()
        {
            suppliedObjectiveCount = 3;
            suppliedGradeStart = 1;
            suppliedGradeCount = 3;
            suppliedGradeAhead = 0;
            suppliedStandardCount = 3;
            suppliedAssessmentCount = 3;
            suppliedStudentGradeCurrent = 3;
            base.EstablishContext();
        }

        // configure benchmark history for no records
        protected override IQueryable<StudentMetricBenchmarkAssessmentHistorical> GetBenchmarksHistory()
        {
            return new List<StudentMetricBenchmarkAssessmentHistorical>().AsQueryable();
        }

        // condition test data for learning standard
        protected override IQueryable<StudentMetricLearningStandard> GetStandardsCurrent()
        {

            // create new standard list
            var standards = new List<StudentMetricLearningStandard>();

            // add assessment
            standards.Add(new StudentMetricLearningStandard()
            {
                StudentUSI = suppliedStudentUSI,
                MetricId = suppliedMetricVariantId,
                SchoolId = suppliedSchoolId,
                GradeLevel = Utilities.GetGradeLevelFromSort(2),
                AssessmentTitle = "Assessment Title",
                Version = suppliedSchoolYearCurrent,
                LearningObjective = suppliedObjectivePrefix + "2",
                LearningStandard = "2.2.2",
                Description = "using stuff in science...",
                DateAdministration = suppliedAssessmentDate1.AddMonths(0),
                MetricStateTypeId = 3,
                Value = "1 of 2"
            });

            // return list
            return standards.AsQueryable();

        }

        [Test]
        public void When_calling_StudentObjectiveGradeStandardServiceFixture_should_return_model_no_benchmark_history_supplied()
        {

            // assertions
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.Count, Is.EqualTo(suppliedObjectiveCount));

        }

    }

    [TestFixture]
    // test insertion of historical assessments not in current meta data   
    public class StudentObjectiveGradeStandardServiceFixture_Should_return_model_insert_history : StudentObjectiveGradeStandardServiceFixtureBaseDimensions
    {

        // configure test parameters
        protected override void EstablishContext()
        {
            suppliedObjectiveCount = 2;
            suppliedGradeStart = 6;
            suppliedGradeCount = 2;
            suppliedGradeAhead = 0;
            suppliedStandardCount = 2;
            suppliedAssessmentCount = 2;
            suppliedStudentGradeCurrent = 7;
            base.EstablishContext();
        }

        // condition history data for anomalous standards
        protected override IQueryable<StudentMetricLearningStandardHistorical> GetStandardsHistory()
        {

            // create new standard list
            var standards = new List<StudentMetricLearningStandardHistorical>();

            // add standard within existing objective / grade that does not exist in meta data
            string standardKey = "2.6.0";
            standards.Add(new StudentMetricLearningStandardHistorical()
            {
                StudentUSI = suppliedStudentUSI,
                MetricId = suppliedMetricVariantId,
                SchoolId = suppliedSchoolId,
                SchoolYear = (short)(suppliedSchoolYearCurrent - 1),
                GradeLevel = Utilities.GetGradeLevelFromSort(suppliedStudentGradeCurrent - 1),
                AssessmentTitle = "(" + standardKey + ") SY 2012,District: 35, Science Grade 6 Genetics...",
                Version = (short)(suppliedSchoolYearCurrent - 1),
                LearningObjective = suppliedObjectivePrefix + "2",
                LearningStandard = standardKey,
                Description = "(" + standardKey + ") using stuff in science...",
                DateAdministration = suppliedAssessmentDate1.AddYears(-1),
                MetricStateTypeId = 3,
                Value = "1 of 2"
            });


            // add standard within existing objecive for grade that does not exist
            standardKey = "2.5.3";
            standards.Add(new StudentMetricLearningStandardHistorical()
            {
                StudentUSI = suppliedStudentUSI,
                MetricId = suppliedMetricVariantId,
                SchoolId = suppliedSchoolId,
                SchoolYear = (short)(suppliedSchoolYearCurrent - 2),
                GradeLevel = Utilities.GetGradeLevelFromSort(suppliedStudentGradeCurrent - 2),
                AssessmentTitle = "(" + standardKey + ") SY 2011,District: 35, Science Grade 5 Genetics...",
                Version = (short)(suppliedSchoolYearCurrent - 2),
                LearningObjective = suppliedObjectivePrefix + "2",
                LearningStandard = standardKey,
                Description = "(" + standardKey + ") using stuff in science...",
                DateAdministration = suppliedAssessmentDate1.AddYears(-2),
                MetricStateTypeId = 3,
                Value = "1 of 2"
            });

            // add standard for objective that does not exist
            standardKey = "0.5.1";
            standards.Add(new StudentMetricLearningStandardHistorical()
            {
                StudentUSI = suppliedStudentUSI,
                MetricId = suppliedMetricVariantId,
                SchoolId = suppliedSchoolId,
                SchoolYear = (short)(suppliedSchoolYearCurrent - 2),
                GradeLevel = Utilities.GetGradeLevelFromSort(suppliedStudentGradeCurrent - 2),
                AssessmentTitle = "(" + standardKey + ") SY 2011,District: 35, Science Grade 5 Genetics...",
                Version = (short)(suppliedSchoolYearCurrent - 2),
                LearningObjective = suppliedObjectivePrefix + "0",
                LearningStandard = standardKey,
                Description = "(" + standardKey + ") using stuff in science...",
                DateAdministration = suppliedAssessmentDate1.AddYears(-2),
                MetricStateTypeId = 3,
                Value = "1 of 2"
            });


            // return list
            suppliedStandardsHistory = standards.AsQueryable();
            return suppliedStandardsHistory;

        }

        [Test]
        public void When_calling_StudentObjectiveGradeStandardServiceFixture_should_return_model_insert_history()
        {
            // model count
            Assert.That(actualModel, Is.Not.Null);
            // service should have added one objective to meta data
            Assert.That(actualModel.Count, Is.EqualTo(suppliedObjectiveCount + 1));
            // service should have ordered objectives putting historical insertion first
            Assert.That(actualModel[0].ObjectiveDescription, Is.EqualTo(suppliedObjectivePrefix + "0"));
            Assert.That(actualModel[1].ObjectiveDescription, Is.EqualTo(suppliedObjectivePrefix + "1"));
            Assert.That(actualModel[2].ObjectiveDescription, Is.EqualTo(suppliedObjectivePrefix + "2"));
            // within existing objectives from meta data...
            // history should have added an additional grade to objective 2
            Assert.That(actualModel[2].Grades.Count, Is.EqualTo(suppliedGradeCount + 1));
            // two from meta data + third from history, sorted by service
            Assert.That(actualModel[2].Grades[0].GradeLevel, Is.EqualTo(Utilities.GetGradeLevelFromSort(suppliedStudentGradeCurrent - 2)));
            Assert.That(actualModel[2].Grades[1].GradeLevel, Is.EqualTo(Utilities.GetGradeLevelFromSort(suppliedStudentGradeCurrent - 1)));
            Assert.That(actualModel[2].Grades[2].GradeLevel, Is.EqualTo(Utilities.GetGradeLevelFromSort(suppliedStudentGradeCurrent)));
            // current grade has 2 standards
            Assert.That(actualModel[2].Grades[2].Standards.Count, Is.EqualTo(suppliedStandardCount));
            // previous grade has additional standard
            Assert.That(actualModel[2].Grades[1].Standards.Count, Is.EqualTo(suppliedStandardCount + 1));
            // two grades ago has one standard inserted into model
            Assert.That(actualModel[2].Grades[0].Standards.Count, Is.EqualTo(1));
            // standards inserted and sorted
            Assert.That(actualModel[2].Grades[1].Standards[0].LearningStandard, Is.EqualTo("2.6.0"));
            Assert.That(actualModel[2].Grades[1].Standards[1].LearningStandard, Is.EqualTo("2." + (suppliedStudentGradeCurrent - 1) + ".1"));
            Assert.That(actualModel[2].Grades[1].Standards[2].LearningStandard, Is.EqualTo("2." + (suppliedStudentGradeCurrent - 1) + ".2"));
            Assert.That(actualModel[2].Grades[0].Standards[0].LearningStandard, Is.EqualTo("2.5.3"));
            // new objective added by history
            Assert.That(actualModel[0].Grades.Count, Is.EqualTo(1));
            Assert.That(actualModel[0].Grades[0].GradeLevel, Is.EqualTo(Utilities.GetGradeLevelFromSort(5)));
            Assert.That(actualModel[0].Grades[0].Standards.Count, Is.EqualTo(1));
            Assert.That(actualModel[0].Grades[0].Standards[0].LearningStandard, Is.EqualTo("0.5.1"));

        }

    }

    [TestFixture]
    // test provider check on turning history off   
    public class StudentObjectiveGradeStandardServiceFixture_Should_return_model_no_history : StudentObjectiveGradeStandardServiceFixtureBaseDimensions
    {

        // configure test parameters
        protected override void EstablishContext()
        {
            suppliedObjectiveCount = 2;
            suppliedGradeStart = 6;
            suppliedGradeCount = 2;
            suppliedStandardCount = 2;
            suppliedAssessmentCount = 2;
            suppliedStudentGradeCurrent = 7;
            suppliedWarehouseAvailable = false;
            base.EstablishContext();
        }

        [Test]
        public void When_calling_StudentObjectiveGradeStandardServiceFixture_should_return_model_no_history()
        {
            // assertions
            // model count
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.Count, Is.EqualTo(suppliedObjectiveCount));
            // service should have ordered objectives putting historical insertion first
            Assert.That(actualModel[0].ObjectiveDescription, Is.EqualTo(suppliedObjectivePrefix + "1"));
            Assert.That(actualModel[1].ObjectiveDescription, Is.EqualTo(suppliedObjectivePrefix + "2"));
            // within existing objectives from meta data...
            Assert.That(actualModel[1].Grades.Count, Is.EqualTo(suppliedGradeCount));
            Assert.That(actualModel[1].Grades[0].GradeLevel, Is.EqualTo(Utilities.GetGradeLevelFromSort(suppliedStudentGradeCurrent - 1)));
            Assert.That(actualModel[1].Grades[1].GradeLevel, Is.EqualTo(Utilities.GetGradeLevelFromSort(suppliedStudentGradeCurrent)));
            // current grade has 2 standards
            Assert.That(actualModel[1].Grades[1].Standards.Count, Is.EqualTo(suppliedStandardCount));
            Assert.That(actualModel[1].Grades[0].Standards.Count, Is.EqualTo(suppliedStandardCount));
            // standards sorted
            Assert.That(actualModel[1].Grades[1].Standards[0].LearningStandard, Is.EqualTo("2." + suppliedStudentGradeCurrent + ".1"));
            Assert.That(actualModel[1].Grades[0].Standards[1].LearningStandard, Is.EqualTo("2." + (suppliedStudentGradeCurrent - 1) + ".2"));
            // no historical assessments
            Assert.That(actualModel[0].Grades[0].Standards[0].Assessments.Count, Is.EqualTo(suppliedAssessmentCount));
            Assert.That(actualModel[1].Grades[0].Standards[0].Assessments.Count, Is.EqualTo(suppliedAssessmentCount));
        }

    }

}

