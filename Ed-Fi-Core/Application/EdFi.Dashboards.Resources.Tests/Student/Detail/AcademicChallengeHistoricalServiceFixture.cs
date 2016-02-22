// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.Student.Detail;
using EdFi.Dashboards.Resources.Models.Student.Detail.AssessmentHistory;
using EdFi.Dashboards.Resources.StudentSchool.Detail;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Student.Detail
{
    [TestFixture]
    public abstract class When_getting_advanced_assessment_history<TRequest, TResponse, TService, TSubjectArea, TAssessment> : TestFixtureBase
        where TRequest : AcademicChallengeHistoricalListRequest, new ()
        where TResponse : List<Assessment>, new()
        where TSubjectArea : SubjectArea, new()
        where TAssessment : Assessment, new()
        where TService : AcademicChallengeHistoricalListServiceBase<TRequest, TResponse, TSubjectArea, TAssessment>, new()
    {
        private IRepository<StudentRecordAssessmentHistory> repository;

        private const int suppliedStudentUSI = 1;
        private const string suppliedSubjectArea = "Mathematics";

        private TResponse actualModel;
        private IQueryable<StudentRecordAssessmentHistory> suppliedData;

        protected override void EstablishContext()
        {
            suppliedData = GetSuppliedData();

            repository = mocks.StrictMock<IRepository<StudentRecordAssessmentHistory>>();
            Expect.Call(repository.GetAll()).Repeat.Any().Return(suppliedData);
        }

        protected IQueryable<StudentRecordAssessmentHistory> GetSuppliedData()
        {
            var list = new List<StudentRecordAssessmentHistory>
        	       	{
        	       		new StudentRecordAssessmentHistory { StudentUSI = suppliedStudentUSI, AcademicSubject = suppliedSubjectArea, SchoolYear = 2007, AdministrationDate = DateTime.Now.AddYears(-3), GradeLevel = "9", AssessmentTitle = "State Assessment", AssessmentCategory = "State Assessment", Accommodations = true, Score = "1900", MetStandardScore = 2100, CommendedScore = 2400},
						new StudentRecordAssessmentHistory { StudentUSI = suppliedStudentUSI, AcademicSubject = suppliedSubjectArea, SchoolYear = 2008, AdministrationDate = new DateTime(2011, 5, 1), GradeLevel = "10", AssessmentTitle = "AP - Calculus BC", AssessmentCategory = "Advanced Placement", Accommodations = false, Score = "5", MetStandardScore = -1},
						new StudentRecordAssessmentHistory { StudentUSI = suppliedStudentUSI, AcademicSubject = suppliedSubjectArea, SchoolYear = 2008, AdministrationDate = new DateTime(2011, 5, 1), GradeLevel = "10", AssessmentTitle = "AP - Statistics", AssessmentCategory = "Advanced Placement", Accommodations = false, Score = "5", MetStandardScore = -1},
						new StudentRecordAssessmentHistory { StudentUSI = suppliedStudentUSI, AcademicSubject = suppliedSubjectArea, SchoolYear = 2008, AdministrationDate = new DateTime(2011, 5, 1), GradeLevel = "10", AssessmentTitle = "AP - Calculus AB", AssessmentCategory = "Advanced Placement", Accommodations = false, Score = "5", MetStandardScore = -1},
						new StudentRecordAssessmentHistory { StudentUSI = suppliedStudentUSI, AcademicSubject = suppliedSubjectArea, SchoolYear = 2009, AdministrationDate = new DateTime(2009, 5, 1), GradeLevel = "11", AssessmentTitle = "IB - Math", AssessmentCategory = "International Baccalaureate", Accommodations = false, Score = "2", MetStandardScore = -1},
						new StudentRecordAssessmentHistory { StudentUSI = suppliedStudentUSI, AcademicSubject = suppliedSubjectArea, SchoolYear = 2009, AdministrationDate = new DateTime(2010, 5, 1), GradeLevel = "11", AssessmentTitle = "IB - Math", AssessmentCategory = "International Baccalaureate", Accommodations = false, Score = "3", MetStandardScore = -1},
						new StudentRecordAssessmentHistory { StudentUSI = suppliedStudentUSI, AcademicSubject = suppliedSubjectArea, SchoolYear = 2009, AdministrationDate = new DateTime(2011, 5, 1), GradeLevel = "11", AssessmentTitle = "IB - Math", AssessmentCategory = "International Baccalaureate", Accommodations = false, Score = "6", MetStandardScore = -1},
                        new StudentRecordAssessmentHistory { StudentUSI = suppliedStudentUSI+2, AcademicSubject = suppliedSubjectArea, SchoolYear = 2007, AdministrationDate = DateTime.Now.AddYears(-3), GradeLevel = "9", AssessmentTitle = "State Assessment", AssessmentCategory = "State Assessment", Accommodations = true, Score = "1900", MetStandardScore = 2100, CommendedScore = 2400},
						new StudentRecordAssessmentHistory { StudentUSI = suppliedStudentUSI+2, AcademicSubject = suppliedSubjectArea, SchoolYear = 2008, AdministrationDate = DateTime.Now.AddYears(-2), GradeLevel = "10", AssessmentTitle = "AP - Calculus AB", Accommodations = false, Score = "5", MetStandardScore = -1},
						new StudentRecordAssessmentHistory { StudentUSI = suppliedStudentUSI+2, AcademicSubject = suppliedSubjectArea, SchoolYear = 2009, AdministrationDate = DateTime.Now.AddYears(-1), GradeLevel = "11", AssessmentTitle = "IB - Math", AssessmentCategory = "International Baccalaureate", Accommodations = false, Score = "6", MetStandardScore = -1},

						new StudentRecordAssessmentHistory { StudentUSI = suppliedStudentUSI, AcademicSubject = "Science", SchoolYear = 2007, AdministrationDate = DateTime.Now.AddYears(-3), GradeLevel = "9", AssessmentTitle = "State Assessment", AssessmentCategory = "State Assessment", Accommodations = true, Score = "1953", MetStandardScore = 2100, CommendedScore = 2400},
						new StudentRecordAssessmentHistory { StudentUSI = suppliedStudentUSI, AcademicSubject = "Science", SchoolYear = 2008, AdministrationDate = DateTime.Now.AddYears(-2), GradeLevel = "10", AssessmentTitle = "AP - Physics", AssessmentCategory = "Advanced Placement", Accommodations = false, Score = "4", MetStandardScore = -1},						
						new StudentRecordAssessmentHistory { StudentUSI = suppliedStudentUSI, AcademicSubject = "Science", SchoolYear = 2008, AdministrationDate = DateTime.Now.AddYears(-2), GradeLevel = "10", AssessmentTitle = "IB - Physics", AssessmentCategory = "International Baccalaureate", Accommodations = false, Score = "5", MetStandardScore = -1},						
						new StudentRecordAssessmentHistory { StudentUSI = suppliedStudentUSI+2, AcademicSubject = "Science", SchoolYear = 2007, AdministrationDate = DateTime.Now.AddYears(-3), GradeLevel = "9", AssessmentTitle = "State Assessment", AssessmentCategory = "State Assessment", Accommodations = true, Score = "1953", MetStandardScore = 2100, CommendedScore = 2400},
						new StudentRecordAssessmentHistory { StudentUSI = suppliedStudentUSI+2, AcademicSubject = "Science", SchoolYear = 2008, AdministrationDate = DateTime.Now.AddYears(-2), GradeLevel = "10", AssessmentTitle = "AP - Physics", AssessmentCategory = "Advanced Placement", Accommodations = false, Score = "4", MetStandardScore = -1},						
						new StudentRecordAssessmentHistory { StudentUSI = suppliedStudentUSI+2, AcademicSubject = "Science", SchoolYear = 2008, AdministrationDate = DateTime.Now.AddYears(-2), GradeLevel = "10", AssessmentTitle = "IB - Physics", AssessmentCategory = "International Baccalaureate", Accommodations = false, Score = "5", MetStandardScore = -1},						
					};
            return list.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new TService { StudentRecordAssessmentHistoryRepository = repository };
            var request = new TRequest {StudentUSI = suppliedStudentUSI, SubjectArea = suppliedSubjectArea};
            actualModel = service.Get(request);
        }

        [Test]
        public void Should_contain_data()
        {
            Assert.That(actualModel.Count(), Is.EqualTo(6));
        }

        [Test]
        public void Should_bind_assessment_values_correctly()
        {
            var assessmentHistory = from rows in suppliedData
                                    where rows.StudentUSI == suppliedStudentUSI && rows.AcademicSubject == suppliedSubjectArea && (rows.AssessmentTitle.StartsWith("AP") || rows.AssessmentTitle.StartsWith("IB"))
                                    orderby rows.AdministrationDate, rows.AssessmentTitle
                                    select rows;

            int indexAssessment = 0;
            foreach(var actualAssessment in actualModel)
            {
                var test = assessmentHistory.ElementAt(indexAssessment);
                Assert.That(actualAssessment.SchoolYear, Is.EqualTo(test.SchoolYear));
                Assert.That(actualAssessment.DateTaken, Is.EqualTo(test.AdministrationDate));
                Assert.That(actualAssessment.GradeLevel, Is.EqualTo(test.GradeLevel));
                Assert.That(actualAssessment.Accommodations, test.Accommodations ? Is.EqualTo("Yes") : Is.EqualTo(String.Empty));

                Assert.That(actualAssessment.Score, Is.EqualTo(test.Score));

                if (!test.MetStandardScore.HasValue)
                    Assert.That(actualAssessment.MetStandardScore, Is.EqualTo(String.Empty));
                else if (test.MetStandardScore.Value != 0)
                    Assert.That(actualAssessment.MetStandardScore, Is.EqualTo("Yes"));
                else
                    Assert.That(actualAssessment.MetStandardScore, Is.EqualTo("No"));

                if (!test.CommendedScore.HasValue)
                    Assert.That(actualAssessment.CommendedScore, Is.EqualTo(String.Empty));
                else if (test.CommendedScore.Value != 0)
                    Assert.That(actualAssessment.CommendedScore, Is.EqualTo("Yes"));
                else
                    Assert.That(actualAssessment.CommendedScore, Is.EqualTo("No"));

                indexAssessment++;
            }
        }

        [Test]
        public void Should_sort_assessment_history_correctly()
        {
            var previousDateTaken = DateTime.MinValue;
            var previousAssessmentTitle = String.Empty;
            foreach (var assessment in actualModel)
            {
                Assert.That(assessment.DateTaken, Is.GreaterThanOrEqualTo(previousDateTaken));
                if (assessment.DateTaken == previousDateTaken)
                    Assert.That(assessment.AssessmentTitle, Is.GreaterThanOrEqualTo(previousAssessmentTitle));

                previousDateTaken = assessment.DateTaken;
                previousAssessmentTitle = assessment.AssessmentTitle;
            }
        }

        [Test]
        public void Should_have_no_unassigned_values_on_presentation_model()
        {
            //In the case of Accommodations they could be null or false that is the default value. This is OK.
            //REfere to the supplied data.
            actualModel[0].EnsureNoDefaultValues("Assessment.ScoreState.StateText",
                "Assessment.ScoreState.MaxValue",
                "Assessment.ScoreState.IsMaxValueInclusive",
                "Assessment.ScoreState.MinValue",
                "Assessment.ScoreState.IsMinValueInclusive",
                "Assessment.ScoreState.Format");
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }

        [Test]
        public virtual void Should_have_all_autoMapper_mappings_valid()
        {
            AutoMapper.Mapper.AssertConfigurationIsValid();
        }
    }

    public class When_getting_advanced_assessment_history_from_service : When_getting_advanced_assessment_history<AcademicChallengeHistoricalListRequest, List<Assessment>, AcademicChallengeHistoricalListService, SubjectArea, Assessment>
    { }
}
