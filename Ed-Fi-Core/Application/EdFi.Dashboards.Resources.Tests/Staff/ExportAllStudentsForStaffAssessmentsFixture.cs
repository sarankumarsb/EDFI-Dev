using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Staff
{
    public abstract class When_exporting_all_students_for_an_assessment : TestFixtureBase
    {
        protected int student1Id = 1;
        protected int student2Id = 2;
        protected int student3Id = 3;
        protected string student1Name = "Student 1";
        protected string student2Name = "Student 2";
        protected string student3Name = "Student 3";
        protected string student1Grade = "10th";
        protected string student2Grade = "11th";
        protected string student3Grade = "2nd";
        protected string objective1Title = "Objective One Title";
        protected string objective2Title = "Objective Two Title";
        protected string objective1TitleDescription = "Objective One Title Description";
        protected string objective2TitleDescription = "Objective Two Title Description";

        protected int suppliedColumnsCount = 0;

        protected StudentExportAllModel actualModel;

        protected IAssessmentDetailsService assessmentDetailsService;

        protected override void EstablishContext()
        {
            assessmentDetailsService = mocks.StrictMock<IAssessmentDetailsService>();

            Expect.Call(assessmentDetailsService.Get(null)).IgnoreArguments().Return(GetAssessmentDetailsModel());

            base.EstablishContext();
        }

        protected abstract AssessmentDetailsModel GetAssessmentDetailsModel();

        [Test]
        public void Should_return_model_with_first_column_entitled_Student_Name()
        {
            for (int i = 0; i < actualModel.Rows.Count(); i++)
            {
                var actualColumnTitle = actualModel.Rows.ElementAt(i).Cells.ElementAt(0).Key;
                Assert.That(actualColumnTitle, Is.EqualTo("Student Name"));
            }

        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }

        [Test]
        public virtual void Should_have_return_model_with_correct_number_of_columns()
        {
            var modelColumnsCount = actualModel.Rows.ElementAt(0).Cells.Count();

            Assert.That(modelColumnsCount, Is.EqualTo(suppliedColumnsCount));
        }
    }

    [TestFixture]
    public class When_exporting_all_students_for_an_assessment_type_of_state_standardized : When_exporting_all_students_for_an_assessment
    {
        protected override void EstablishContext()
        {
            suppliedColumnsCount = 5;

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            var service = new ExportAllStudentsForStaffAssessmentsService(assessmentDetailsService);
            actualModel = service.Get(new ExportAllStudentsForStaffAssessmentsRequest()
                                    {
                                        SchoolId = 1,
                                        StaffUSI = 1,
                                        SectionOrCohortId = 1,
                                        SubjectArea = StaffModel.SubjectArea.Mathematics.ToString(),
                                        AssessmentSubType = StaffModel.AssessmentSubType.StateStandardized.ToString()
                                    });
        }

        protected override AssessmentDetailsModel GetAssessmentDetailsModel()
        {
            return new AssessmentDetailsModel()
                       {
                           Students = GetStudents(),
                           ObjectiveTitles = GetObjectiveTitles()
                       };
        }

        private List<StudentWithMetricsAndAssessments> GetStudents()
        {
            return new List<StudentWithMetricsAndAssessments>
                       {
                           new StudentWithMetricsAndAssessments
                               {
                                   StudentUSI = student1Id,
                                   Name = student1Name,
                                   GradeLevelDisplayValue = student1Grade,
                                   Score = new StudentWithMetrics.IndicatorMetric(student1Id)
                                               {
                                                   DisplayValue = "1111"
                                               },
                                   Metrics = new List<StudentWithMetrics.Metric>
                                              {
                                                  new StudentWithMetricsAndAssessments.AssessmentMetric(student1Id)
                                                      {
                                                          ObjectiveName = objective1Title,
                                                          DisplayValue = "5/10"
                                                      },

                                                  new StudentWithMetricsAndAssessments.AssessmentMetric(student1Id)
                                                      {
                                                          ObjectiveName = objective2Title,
                                                          DisplayValue = "1/20"
                                                      }
                                              }
                               },

                               new StudentWithMetricsAndAssessments
                               {
                                   StudentUSI = student2Id,
                                   Name = student2Name,
                                   GradeLevelDisplayValue = student2Grade,
                                   Score = new StudentWithMetrics.IndicatorMetric(student2Id)
                                               {
                                                   DisplayValue = "2222"
                                               },
                                   Metrics = new List<StudentWithMetrics.Metric>
                                              {
                                                  new StudentWithMetricsAndAssessments.AssessmentMetric(student2Id)
                                                      {
                                                          ObjectiveName = objective1Title,
                                                          DisplayValue = "7/10"
                                                      },

                                                  new StudentWithMetricsAndAssessments.AssessmentMetric(student1Id)
                                                      {
                                                          ObjectiveName = objective2Title,
                                                          DisplayValue = "3/20"
                                                      }
                                              }
                               }
                       };
        }

        private List<AssessmentDetailsModel.ObjectiveTitle> GetObjectiveTitles()
        {
            return new List<AssessmentDetailsModel.ObjectiveTitle>
                       {
                           new AssessmentDetailsModel.ObjectiveTitle
                               {
                                   Title = objective1Title,
                                   Description = objective1TitleDescription,
                                   Mastery = "6 of 10"
                               },

                           new AssessmentDetailsModel.ObjectiveTitle
                               {
                                   Title = objective2Title,
                                   Description = objective2TitleDescription,
                                   Mastery = "2 of 20"
                               }
                       };
        }

        [Test]
        public virtual void Should_have_subject_area_column()
        {
            var subjectAreaColumn = actualModel.Rows.ElementAt(0).Cells.ElementAt(2).Key;
            Assert.That(subjectAreaColumn, Is.EqualTo(StaffModel.SubjectArea.Mathematics.ToString()));
        }

        [Test]
        public virtual void Should_have_subject_area_value()
        {
            var subjectAreaColumn = actualModel.Rows.ElementAt(1).Cells.ElementAt(2).Value;
            Assert.That(subjectAreaColumn, Is.EqualTo("1111"));

            subjectAreaColumn = actualModel.Rows.ElementAt(2).Cells.ElementAt(2).Value;
            Assert.That(subjectAreaColumn, Is.EqualTo("2222"));
        }

        [Test]
        public void Should_return_model_with_studentUSI_on_each_row()
        {
            for (int i = 1; i < actualModel.Rows.Count(); i++)
            {
                var actualRow = actualModel.Rows.ElementAt(i);
                Assert.That(actualRow.StudentUSI, Is.EqualTo(i));
            }
        }

        [Test]
        public void Should_return_model_with_first_column_value_containing_the_student_name()
        {
            for (int i = 1; i < actualModel.Rows.Count(); i++)
            {
                var actualStudentName = actualModel.Rows.ElementAt(i).Cells.ElementAt(0).Value;
                Assert.That(actualStudentName, Is.EqualTo(string.Format("Student {0}", i)));
            }

        }
    }

    [TestFixture]
    public class When_exporting_all_students_for_an_assessment_type_of_benchmark : When_exporting_all_students_for_an_assessment
    {
        protected override void EstablishContext()
        {
            suppliedColumnsCount = 5;

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            var service = new ExportAllStudentsForStaffAssessmentsService(assessmentDetailsService);
            actualModel = service.Get(new ExportAllStudentsForStaffAssessmentsRequest()
            {
                SchoolId = 1,
                StaffUSI = 1,
                SectionOrCohortId = 1,
                SubjectArea = StaffModel.SubjectArea.Mathematics.ToString(),
                AssessmentSubType = StaffModel.AssessmentSubType.Benchmark.ToString()
            });
        }

        protected override AssessmentDetailsModel GetAssessmentDetailsModel()
        {
            return new AssessmentDetailsModel()
            {
                Students = GetStudents(),
                ObjectiveTitles = GetObjectiveTitles()
            };
        }

        private List<StudentWithMetricsAndAssessments> GetStudents()
        {
            return new List<StudentWithMetricsAndAssessments>
                       {
                           new StudentWithMetricsAndAssessments
                               {
                                   StudentUSI = student1Id,
                                   Name = student1Name,
                                   GradeLevelDisplayValue = student1Grade,
                                   Score = new StudentWithMetrics.IndicatorMetric(student1Id)
                                               {
                                                   DisplayValue = "1111"
                                               },
                                   Metrics = new List<StudentWithMetrics.Metric>
                                              {
                                                  new StudentWithMetricsAndAssessments.AssessmentMetric(student1Id)
                                                      {
                                                          ObjectiveName = objective1Title,
                                                          DisplayValue = "5/10"
                                                      },

                                                  new StudentWithMetricsAndAssessments.AssessmentMetric(student1Id)
                                                      {
                                                          ObjectiveName = objective2Title,
                                                          DisplayValue = "1/20"
                                                      }
                                              }
                               },

                               new StudentWithMetricsAndAssessments
                               {
                                   StudentUSI = student2Id,
                                   Name = student2Name,
                                   GradeLevelDisplayValue = student2Grade,
                                   Score = new StudentWithMetrics.IndicatorMetric(student2Id)
                                               {
                                                   DisplayValue = "2222"
                                               },
                                   Metrics = new List<StudentWithMetrics.Metric>
                                              {
                                                  new StudentWithMetricsAndAssessments.AssessmentMetric(student2Id)
                                                      {
                                                          ObjectiveName = objective1Title,
                                                          DisplayValue = "7/10"
                                                      },

                                                  new StudentWithMetricsAndAssessments.AssessmentMetric(student1Id)
                                                      {
                                                          ObjectiveName = objective2Title,
                                                          DisplayValue = "3/20"
                                                      }
                                              }
                               }
                       };
        }

        private List<AssessmentDetailsModel.ObjectiveTitle> GetObjectiveTitles()
        {
            return new List<AssessmentDetailsModel.ObjectiveTitle>
                       {
                           new AssessmentDetailsModel.ObjectiveTitle
                               {
                                   Title = objective1Title,
                                   Description = objective1TitleDescription,
                                   Mastery = "6 of 10"
                               },

                           new AssessmentDetailsModel.ObjectiveTitle
                               {
                                   Title = objective2Title,
                                   Description = objective2TitleDescription,
                                   Mastery = "2 of 20"
                               }
                       };
        }

        [Test]
        public virtual void Should_have_title_description_as_column_header()
        {
            var columnTitle = actualModel.Rows.ElementAt(0).Cells.ElementAt(3).Key;
            Assert.That(columnTitle, Is.EqualTo(objective1TitleDescription));

            columnTitle = actualModel.Rows.ElementAt(0).Cells.ElementAt(4).Key;
            Assert.That(columnTitle, Is.EqualTo(objective2TitleDescription));
        }
    }

    [TestFixture]
    public class When_exporting_all_students_for_an_assessment_type_of_reading : When_exporting_all_students_for_an_assessment
    {
        protected override void EstablishContext()
        {
            suppliedColumnsCount = 4;

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            var service = new ExportAllStudentsForStaffAssessmentsService(assessmentDetailsService);
            actualModel = service.Get(new ExportAllStudentsForStaffAssessmentsRequest()
            {
                SchoolId = 1,
                StaffUSI = 1,
                SectionOrCohortId = 1,
                SubjectArea = StaffModel.SubjectArea.Mathematics.ToString(),
                AssessmentSubType = StaffModel.AssessmentSubType.Reading.ToString()
            });
        }

        protected override AssessmentDetailsModel GetAssessmentDetailsModel()
        {
            return new AssessmentDetailsModel()
            {
                Students = GetStudents(),
                ObjectiveTitles = GetObjectiveTitles()
            };
        }

        private List<StudentWithMetricsAndAssessments> GetStudents()
        {
            return new List<StudentWithMetricsAndAssessments>
                       {
                           new StudentWithMetricsAndAssessments
                               {
                                   StudentUSI = student3Id,
                                   Name = student3Name,
                                   GradeLevelDisplayValue = student3Grade,
                                   Score = new StudentWithMetrics.IndicatorMetric(student3Id)
                                               {
                                                   DisplayValue = "3333"
                                               },
                                   Metrics = new List<StudentWithMetrics.Metric>
                                              {
                                                  new StudentWithMetricsAndAssessments.AssessmentMetric(student3Id)
                                                      {
                                                          ObjectiveName = objective1Title,
                                                          DisplayValue = "D"
                                                      },

                                                  new StudentWithMetricsAndAssessments.AssessmentMetric(student3Id)
                                                      {
                                                          ObjectiveName = objective2Title,
                                                          DisplayValue = "SD"
                                                      }
                                              }
                               }
                       };
        }

        private List<AssessmentDetailsModel.ObjectiveTitle> GetObjectiveTitles()
        {
            return new List<AssessmentDetailsModel.ObjectiveTitle>
                       {
                           new AssessmentDetailsModel.ObjectiveTitle
                               {
                                   Title = objective1Title,
                                   Description = objective1TitleDescription,
                                   Mastery = "1 of 1"
                               },

                           new AssessmentDetailsModel.ObjectiveTitle
                               {
                                   Title = objective2Title,
                                   Description = objective2TitleDescription,
                                   Mastery = "0 of 1"
                               }
                       };
        }
    }
}
