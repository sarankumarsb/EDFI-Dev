// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Models.Student.Detail.AssessmentHistory;
using EdFi.Dashboards.Resources.StudentSchool.Detail;
using NUnit.Framework;
using Rhino.Mocks;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Resources.Models.Student.Detail;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Testing;

namespace EdFi.Dashboards.Resources.Tests.Student.Detail
{
    [TestFixture]
    public abstract class When_getting_assessment_history<TRequest, TResponse, TService, TSubjectArea, TAssessment> : TestFixtureBase
        where TRequest : AssessmentHistoryRequest, new()
        where TResponse : AssessmentHistoryModel, new()
        where TSubjectArea : SubjectArea, new()
        where TAssessment : Assessment, new()
        where TService : AssessmentHistoryServiceBase<TRequest, TResponse, TSubjectArea, TAssessment>, new()
    {
        private IRepository<StudentRecordAssessmentHistory> repository;

        private const int suppliedStudentUSI = 1;
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
            var now = DateTime.Now;

            var list = new List<StudentRecordAssessmentHistory>
        	       	{
                       	new StudentRecordAssessmentHistory { StudentUSI=suppliedStudentUSI, AcademicSubject = "Social Studies", SchoolYear = 2009, AdministrationDate = now.AddYears(-1), GradeLevel = "11", AssessmentTitle = "State Assessment", Accommodations = false, Score = "2162", MetMinimum = 0, MetStandardScore = 0, CommendedScore = 0},
                       	new StudentRecordAssessmentHistory { StudentUSI=suppliedStudentUSI, AcademicSubject = "Social Studies", SchoolYear = 2008, AdministrationDate = now.AddYears(-2), GradeLevel = "10", AssessmentTitle = "State Assessment", Accommodations = false, Score = "2162", MetMinimum = 2100, MetStandardScore = 2100, CommendedScore = 2400},
						new StudentRecordAssessmentHistory { StudentUSI=suppliedStudentUSI, AcademicSubject = "Social Studies", SchoolYear = 2009, AdministrationDate = now.AddYears(-1), GradeLevel = "11", AssessmentTitle = "State Assessment", Accommodations = true, Score = "2180", MetStandardScore = 2100, CommendedScore = 2400},
			       	    new StudentRecordAssessmentHistory { StudentUSI=suppliedStudentUSI, AcademicSubject = "Social Studies", SchoolYear = 2009, AdministrationDate = now.AddYears(-1), GradeLevel = "11", AssessmentTitle = "State Assessment", Accommodations = true, Score = "2180", MetMinimum = null, MetStandardScore = null, CommendedScore = null},
			       	
                        new StudentRecordAssessmentHistory { StudentUSI=suppliedStudentUSI +2, AcademicSubject = "Test Subject Area", SchoolYear = 2009, AdministrationDate = now.AddYears(-1), GradeLevel = "11", AssessmentTitle = "State Assessment", Accommodations = true, Score = "2180", MetStandardScore = 2100, CommendedScore = 2400},
			        
        	       		new StudentRecordAssessmentHistory { StudentUSI=suppliedStudentUSI, AcademicSubject = "ELA / Reading", SchoolYear = 2008, AdministrationDate = now.AddYears(-2), GradeLevel = "10", AssessmentTitle = "State Assessment", Accommodations = false, Score = "2117", MetStandardScore = 2100, CommendedScore = 2400},
						new StudentRecordAssessmentHistory { StudentUSI=suppliedStudentUSI, AcademicSubject = "ELA / Reading", SchoolYear = 2009, AdministrationDate = now.AddYears(-1), GradeLevel = "11", AssessmentTitle = "State Assessment", Accommodations = true, Score = "2500", MetStandardScore = 2100, CommendedScore = null},
                        new StudentRecordAssessmentHistory { StudentUSI=suppliedStudentUSI, AcademicSubject = "ELA / Reading", SchoolYear = 2007, AdministrationDate = now.AddYears(-3), GradeLevel = "9", AssessmentTitle = "State Assessment", Accommodations = true, Score = "2059", MetStandardScore = null, CommendedScore = 2400},
						new StudentRecordAssessmentHistory { StudentUSI=suppliedStudentUSI+2, AcademicSubject = "ELA / Reading", SchoolYear = 2009, AdministrationDate = now.AddYears(-1), GradeLevel = "11", AssessmentTitle = "State Assessment", Accommodations = true, Score = "2500", MetStandardScore = 2100, CommendedScore = null},

						
						new StudentRecordAssessmentHistory { StudentUSI=suppliedStudentUSI, AcademicSubject = "Science", SchoolYear = 2007, AdministrationDate = now.AddYears(-3), GradeLevel = "9", AssessmentTitle = "State Assessment", Accommodations = true, Score = "1953", MetStandardScore = 2100, CommendedScore = 2400},
						new StudentRecordAssessmentHistory { StudentUSI=suppliedStudentUSI, AcademicSubject = "Science", SchoolYear = 2008, AdministrationDate = now.AddYears(-2), GradeLevel = "10", AssessmentTitle = "State Assessment", Accommodations = false, Score = "2068", MetStandardScore = 2100, CommendedScore = 2400},						
						
                        new StudentRecordAssessmentHistory { StudentUSI=suppliedStudentUSI, AcademicSubject = "Mathematics", SchoolYear = 2007, AdministrationDate = now.AddYears(-3), GradeLevel = "9", AssessmentTitle = "State Assessment", Accommodations = true, Score = "1900", MetStandardScore = 2100, CommendedScore = 2400},
						new StudentRecordAssessmentHistory { StudentUSI=suppliedStudentUSI, AcademicSubject = "Mathematics", SchoolYear = 2008, AdministrationDate = now.AddYears(-2), GradeLevel = "10", AssessmentTitle = "State Assessment", Accommodations = false, Score = "2300", MetStandardScore = 2100, CommendedScore = 2400},
						new StudentRecordAssessmentHistory { StudentUSI=suppliedStudentUSI, AcademicSubject = "Mathematics", SchoolYear = 2009, AdministrationDate = now.AddYears(-1), GradeLevel = "11", AssessmentTitle = "State Assessment", Accommodations = true, Score = "2500", MetStandardScore = 2100, CommendedScore = 2400},

					};
            return list.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new TService
                              {
                                  StudentRecordAssessmentHistoryRepository = repository
                              };
            var request = new TRequest { StudentUSI = suppliedStudentUSI};

            actualModel = service.Get(request);
        }

        [Test]
        public void Should_contain_data()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.SubjectAreas, Is.Not.Null);
            Assert.That(actualModel.SubjectAreas.Count, Is.GreaterThan(0));
        }

        [Test]
        public void Should_create_correct_number_of_subject_areas()
        {
            var suppliedSubjectAreas = from rows in suppliedData
                               where rows.StudentUSI == suppliedStudentUSI
                               group rows by rows.AcademicSubject
                                   into s
                                   select new { Name = s.Key };

            Assert.That(actualModel.SubjectAreas.Count, Is.EqualTo(suppliedSubjectAreas.Count()));
        }

        [Test]
        public void Should_bind_values_correctly()
        {
            var suppliedSubjectAreas = from rows in suppliedData
                               where rows.StudentUSI == suppliedStudentUSI
                               orderby rows.AcademicSubject, rows.AdministrationDate
                               group rows by rows.AcademicSubject
                                   into s
                                   select new { Name = s.Key, Assessments = s };

            int index = 0;
            foreach (var suppliedSubject in suppliedSubjectAreas)
            {
                Assert.That(actualModel.SubjectAreas[index].Name, Is.EqualTo(suppliedSubject.Name));
                Assert.That(actualModel.SubjectAreas[index].Assessments.Count(), Is.EqualTo(suppliedSubject.Assessments.Count()));

                int indexAssessment = 0;
                foreach (var suppliedTest in suppliedSubject.Assessments)
                {
                    var actualAssessment = actualModel.SubjectAreas[index].Assessments[indexAssessment];
                    Assert.That(actualAssessment.SchoolYear, Is.EqualTo(suppliedTest.SchoolYear));
                    Assert.That(actualAssessment.DateTaken, Is.EqualTo(suppliedTest.AdministrationDate));
                    Assert.That(actualAssessment.GradeLevel, Is.EqualTo(suppliedTest.GradeLevel));
                    Assert.That(actualAssessment.Accommodations, suppliedTest.Accommodations ? Is.EqualTo("Yes") : Is.EqualTo(String.Empty));

                    Assert.That(actualAssessment.Score, Is.EqualTo(suppliedTest.Score));

                    if (!suppliedTest.MetMinimum.HasValue)
                        Assert.That(actualAssessment.MetMinimumScore, Is.EqualTo(String.Empty));
                    else if (suppliedTest.MetMinimum.Value != 0)
                        Assert.That(actualAssessment.MetMinimumScore, Is.EqualTo("Yes"));
                    else
                        Assert.That(actualAssessment.MetMinimumScore, Is.EqualTo("No"));

                    if (!suppliedTest.MetStandardScore.HasValue)
                        Assert.That(actualAssessment.MetStandardScore, Is.EqualTo(String.Empty));
                    else if (suppliedTest.MetStandardScore.Value != 0)
                        Assert.That(actualAssessment.MetStandardScore, Is.EqualTo("Yes"));
                    else
                        Assert.That(actualAssessment.MetStandardScore, Is.EqualTo("No"));

                    if (!suppliedTest.CommendedScore.HasValue)
                        Assert.That(actualAssessment.CommendedScore, Is.EqualTo(String.Empty));
                    else if (suppliedTest.CommendedScore.Value != 0)
                        Assert.That(actualAssessment.CommendedScore, Is.EqualTo("Yes"));
                    else
                        Assert.That(actualAssessment.CommendedScore, Is.EqualTo("No"));

                    indexAssessment++;
                }

                index++;
            }

        }

        [Test]
        public void Should_calculate_status_correctly()
        {
            var suppliedSubjectAreas = from rows in suppliedData
                               where rows.StudentUSI == suppliedStudentUSI
                               orderby rows.AcademicSubject, rows.AdministrationDate
                               group rows by rows.AcademicSubject
                                   into s
                                   select new { Name = s.Key, Assessments = s };
            int index = 0;
            foreach (var suppliedSubject in suppliedSubjectAreas)
            {
                int indexAssessment = 0;
                foreach (var suppliedTest in suppliedSubject.Assessments)
                {
                    MetricStateType suppliedStateCalculation = calculateState(suppliedTest.MetMinimum, suppliedTest.MetStandardScore, suppliedTest.CommendedScore);
                    Assert.That(actualModel.SubjectAreas[index].Assessments[indexAssessment].ScoreState.StateType, Is.EqualTo(suppliedStateCalculation));

                    indexAssessment++;
                }
                index++;
            }

        }

        private MetricStateType calculateState(int? metMinimumScore, int? metStandardScore, int? commended)
        {
            if (commended.HasValue && commended.Value != 0)
                return MetricStateType.VeryGood;
            if (metStandardScore.HasValue && metStandardScore.Value != 0)
                return MetricStateType.Good;
            if (metStandardScore.HasValue && metStandardScore.Value == 0)
                return MetricStateType.Low;
            if (metMinimumScore.HasValue && metMinimumScore.Value == 0)
                return MetricStateType.VeryLow;
            return MetricStateType.Na;
        }

        [Test]
        public void Should_sort_assessment_history_correctly()
        {
            string previousSubject = String.Empty;
            foreach (var actualSubject in actualModel.SubjectAreas)
            {
                Assert.That(actualSubject.Name, Is.GreaterThan(previousSubject));
                previousSubject = actualSubject.Name;

                DateTime previousDateTaken = DateTime.MinValue;
                foreach (var assessment in actualSubject.Assessments)
                {
                    Assert.That(assessment.DateTaken, Is.GreaterThanOrEqualTo(previousDateTaken));
                    previousDateTaken = assessment.DateTaken;
                }
            }
        }

        [Test]
        public void Should_have_no_unassigned_values_on_presentation_model()
        {
            //In the case of Accommodations they could be null or false that is the default value. This is OK.
            //REfere to the supplied data.
            actualModel.EnsureNoDefaultValues("AssessmentHistoryModel.SubjectAreas[0].Assessments[0].ScoreState.StateText", //All these are expected because we are only expecting the StateType
                "AssessmentHistoryModel.SubjectAreas[0].Assessments[0].ScoreState.MaxValue",
                "AssessmentHistoryModel.SubjectAreas[0].Assessments[0].ScoreState.IsMaxValueInclusive",
                "AssessmentHistoryModel.SubjectAreas[0].Assessments[0].ScoreState.MinValue",
                "AssessmentHistoryModel.SubjectAreas[0].Assessments[0].ScoreState.IsMinValueInclusive",
                "AssessmentHistoryModel.SubjectAreas[0].Assessments[0].ScoreState.Format",
                "AssessmentHistoryModel.SubjectAreas[0].Assessments[1].ScoreState.StateText",
                "AssessmentHistoryModel.SubjectAreas[0].Assessments[1].ScoreState.MaxValue",
                "AssessmentHistoryModel.SubjectAreas[0].Assessments[1].ScoreState.IsMaxValueInclusive",
                "AssessmentHistoryModel.SubjectAreas[0].Assessments[1].ScoreState.MinValue",
                "AssessmentHistoryModel.SubjectAreas[0].Assessments[1].ScoreState.IsMinValueInclusive",
                "AssessmentHistoryModel.SubjectAreas[0].Assessments[1].ScoreState.Format",
                "AssessmentHistoryModel.SubjectAreas[0].Assessments[2].ScoreState.StateText",
                "AssessmentHistoryModel.SubjectAreas[0].Assessments[2].ScoreState.MaxValue",
                "AssessmentHistoryModel.SubjectAreas[0].Assessments[2].ScoreState.IsMaxValueInclusive",
                "AssessmentHistoryModel.SubjectAreas[0].Assessments[2].ScoreState.MinValue",
                "AssessmentHistoryModel.SubjectAreas[0].Assessments[2].ScoreState.IsMinValueInclusive",
                "AssessmentHistoryModel.SubjectAreas[0].Assessments[2].ScoreState.Format");
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

    public class When_getting_assessment_history_from_service : When_getting_assessment_history<AssessmentHistoryRequest, AssessmentHistoryModel, AssessmentHistoryService, SubjectArea, Assessment>
    { }
}
