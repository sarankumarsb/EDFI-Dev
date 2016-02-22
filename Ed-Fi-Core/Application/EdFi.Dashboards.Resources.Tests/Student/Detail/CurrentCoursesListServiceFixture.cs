// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.Student.Detail;
using EdFi.Dashboards.Resources.Models.Student.Detail.CurrentCourses;
using EdFi.Dashboards.Resources.StudentSchool.Detail;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;
using EdFi.Dashboards.Resources.Models.Student;

namespace EdFi.Dashboards.Resources.Tests.Student.Detail
{
    [TestFixture]
    public abstract class When_getting_current_courses<TRequest, TResponse, TService, TSemester, TCourse, TGrade> : TestFixtureBase
        where TRequest : CurrentCoursesListRequest, new()
        where TResponse : CurrentCoursesModel, new()
        where TSemester : Semester, new()
        where TCourse : Course, new()
        where TGrade : Grade, new()
        where TService : CurrentCoursesListServiceBase<TRequest, TResponse, TSemester, TCourse, TGrade>, new()
    {
        protected const int schoolYear = 2011;
        protected const string leaCode = "testISD";
        protected const int school0 = 1000000;
        protected const int school1 = school0 + 1;

        protected const int student0 = 7000;
        protected const int student1 = student0 + 1;
        protected const int student2 = student0 + 2;
        protected const int student3 = student0 + 3;

        protected const string fallSemester = "Fall Semester";
        protected const string springSemester = "Spring Semester";
        protected const string summerSemester = "Summer Semester";
        protected const string yearRound = "Year Round";

        protected const int gradingPeriod1 = 1;
        protected const int gradingPeriod2 = 2;
        protected const int gradingPeriod3 = 3;
        protected const int gradingPeriod4 = 4;
        protected const int gradingPeriod5 = 5;
        protected const int gradingPeriod6 = 6;
        protected const int finalGradePeriod = 999;

        protected const string wrongCourseCode = "WRONG";
        protected const string wrongCourseTitle = "WRONG TITLE";

        protected const string suppliedCourseCode1 = "AAA002";
        protected const string suppliedCourseTitle1 = "Class AAA";
        protected const string suppliedSubjectArea1 = "subject area 1";
        protected decimal? suppliedCreditsToBeEarned1 = .34m;
        protected const string suppliedInstructor1 = "instructor 1";
        protected const string suppliedGradeLevel1 = "11th Grade";
        protected const string suppliedLetterGrade1Period1 = "B";
        protected const string suppliedLetterGrade1Period2 = "C";
        protected const int suppliedNumericGrade1Period3 = 88;
        protected const string suppliedLetterGrade1Period4 = "D";
        protected const string suppliedLetterGrade1Period5 = "E";
        protected const int suppliedNumericGrade1Period6 = 87;
        protected const int suppliedNumericGrade1Final = 33;

        protected const string suppliedCourseCode2 = "AAA003";
        protected const string suppliedCourseTitle2 = "Class BBB";
        protected const string suppliedSubjectArea2 = "subject area 2";
        protected decimal? suppliedCreditsToBeEarned2 = .78m;
        protected const string suppliedInstructor2 = "instructor 2";
        protected const string suppliedGradeLevel2 = "12th Grade";
        protected const int suppliedNumericGrade2Period1 = 94;
        protected const int suppliedNumericGrade2Period2 = 93;
        protected const string suppliedLetterGrade2Period3 = "A";
        protected const int suppliedNumericGrade2Period4 = 92;
        protected const int suppliedNumericGrade2Period5 = 91;
        protected const string suppliedLetterGrade2Period6 = "A";
        protected const string suppliedLetterGrade2Final = "A1";

        protected const string suppliedCourseCode3 = "AAA001";
        protected const string suppliedCourseTitle3 = "Class CCC";
        protected const string suppliedSubjectArea3 = "subject area 3";
        protected decimal? suppliedCreditsToBeEarned3 = .8m;
        protected const string suppliedInstructor3 = "instructor 3";
        protected const string suppliedGradeLevel3 = "10th Grade";
        protected const int suppliedNumericGrade3Period1 = 10;
        protected const int suppliedNumericGrade3Period2 = 11;
        protected const int suppliedNumericGrade3Period3 = 12;
        protected const int suppliedNumericGrade3Period4 = 13;
        protected const int suppliedNumericGrade3Period5 = 14;
        protected const int suppliedNumericGrade3Period6 = 15;
        protected const int suppliedNumericGrade3Final = 16;

        protected const string suppliedCourseCode4 = "AAA004";
        protected const string suppliedCourseTitle4 = "Class DDD";
        protected const string suppliedSubjectArea4 = "subject area 4";
        protected decimal? suppliedCreditsToBeEarned4 = .2m;
        protected const string suppliedInstructor4 = "instructor 4";
        protected const string suppliedGradeLevel4 = "9th Grade";
        protected const int suppliedNumericGrade4Period1 = 20;
        protected const int suppliedNumericGrade4Period2 = 21;
        protected const int suppliedNumericGrade4Period3 = 22;
        protected const int suppliedNumericGrade4Period4 = 23;
        protected const int suppliedNumericGrade4Period5 = 24;
        protected const int suppliedNumericGrade4Period6 = 25;
        protected const int suppliedNumericGrade4Final = 26;

        protected const string suppliedCourseCode5 = "AAA005";
        protected const string suppliedCourseTitle5 = "Class EEE";
        protected const string suppliedSubjectArea5 = "subject area 5";
        protected decimal? suppliedCreditsToBeEarned5 = .3m;
        protected const string suppliedInstructor5 = "instructor 5";
        protected const string suppliedGradeLevel5 = "8th Grade";
        protected const int suppliedNumericGrade5Period1 = 30;
        protected const int suppliedNumericGrade5Period2 = 31;
        protected const int suppliedNumericGrade5Period3 = 32;
        protected const int suppliedNumericGrade5Period4 = 33;
        protected const int suppliedNumericGrade5Period5 = 34;
        protected const int suppliedNumericGrade5Period6 = 35;
        protected const int suppliedNumericGrade5Final = 36;

        protected IRepository<StudentRecordCurrentCourse> repository;
        protected IQueryable<StudentRecordCurrentCourse> suppliedData;

        protected TResponse actualModel;

        protected override void EstablishContext()
        {
            suppliedData = GetData();
            repository = mocks.StrictMock<IRepository<StudentRecordCurrentCourse>>();
            Expect.Call(repository.GetAll()).Return(suppliedData);
        }

        protected override void ExecuteTest()
        {
            var service = new TService
                              {
                                  Repository = repository
                              };

            var request = new TRequest
                              {
                                  SchoolId = school0,
                                  StudentUSI = student0
                              };

            actualModel = service.Get(request);
        }

        protected abstract IQueryable<StudentRecordCurrentCourse> GetData();

        [Test]
        public virtual void Should_return_a_model_that_is_not_null()
        {
            Assert.That(actualModel, !Is.EqualTo(null));
            Assert.That(actualModel.StudentUSI, Is.EqualTo(student0));
        }

        [Test]
        public virtual void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel.EnsureNoDefaultValues();
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

    public abstract class When_getting_current_courses_with_a_spring_term<TRequest, TResponse, TService, TSemester, TCourse, TGrade> : When_getting_current_courses<TRequest, TResponse, TService, TSemester, TCourse, TGrade>
        where TRequest : CurrentCoursesListRequest, new()
        where TResponse : CurrentCoursesModel, new()
        where TSemester : Semester, new()
        where TCourse : Course, new()
        where TGrade : Grade, new()
        where TService : CurrentCoursesListServiceBase<TRequest, TResponse, TSemester, TCourse, TGrade>, new()
    {
        protected override IQueryable<StudentRecordCurrentCourse> GetData()
        {
            var data = new List<StudentRecordCurrentCourse>
                           {
                               new StudentRecordCurrentCourse { StudentUSI = student1, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = wrongCourseCode },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school1, TermTypeId = 2, TermType = springSemester, LocalCourseCode = wrongCourseCode },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod6, LetterGradeEarned = suppliedLetterGrade2Period3  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod4, LetterGradeEarned = suppliedLetterGrade1Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod5, LetterGradeEarned = suppliedLetterGrade1Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod5, NumericGradeEarned = suppliedNumericGrade2Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod4, NumericGradeEarned = suppliedNumericGrade2Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod6, NumericGradeEarned = suppliedNumericGrade1Period3  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = finalGradePeriod, LetterGradeEarned = suppliedLetterGrade2Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade1Final  },
                           };
            return data.AsQueryable();
        }

        [Test]
        public void Should_have_semesters_built_correctly()
        {
            Assert.That(actualModel.Semesters.Count, Is.EqualTo(1));
            Assert.That(actualModel.Semesters[0].Term, Is.EqualTo(springSemester));
            Assert.That(actualModel.Semesters[0].StudentUSI, Is.EqualTo(student0));
            Assert.That(actualModel.Semesters[0].AvailablePeriods.Count, Is.EqualTo(4));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[0], Is.EqualTo(GradingPeriod.Four));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[1], Is.EqualTo(GradingPeriod.Five));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[2], Is.EqualTo(GradingPeriod.Six));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[3], Is.EqualTo(GradingPeriod.FinalGrade));
            Assert.That(actualModel.Semesters[0].Courses.Count, Is.EqualTo(2));
        }

        [Test]
        public void Should_have_courses_built_and_bound_correctly()
        {
            var course = actualModel.Semesters[0].Courses[0];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode1));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle1));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea1));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor1));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned1));

            course = actualModel.Semesters[0].Courses[1];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode2));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle2));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea2));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor2));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel2));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned2));
        }

        [Test]
        public void Should_have_grades_built_and_bound_correctly()
        {
            Assert.That(actualModel.Semesters[0].Courses[0].Grades.Count, Is.EqualTo(4));

            var grade = actualModel.Semesters[0].Courses[0].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period1));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Four));

            grade = actualModel.Semesters[0].Courses[0].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period2));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Five));

            grade = actualModel.Semesters[0].Courses[0].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade1Period3.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Six));

            grade = actualModel.Semesters[0].Courses[0].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade1Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[0].Courses[1].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[0].Courses[1].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period1.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Four));

            grade = actualModel.Semesters[0].Courses[1].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period2.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Five));

            grade = actualModel.Semesters[0].Courses[1].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade2Period3));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Six));

            grade = actualModel.Semesters[0].Courses[1].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade2Final));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));
        }
        
    }

    
    public class When_getting_current_courses_with_a_spring_term_form_the_service : When_getting_current_courses_with_a_spring_term<CurrentCoursesListRequest, CurrentCoursesModel, CurrentCoursesListService, Semester, Course, Grade>
    {
        
    }

    public abstract class When_getting_current_courses_with_a_fall_term<TRequest, TResponse, TService, TSemester, TCourse, TGrade> : When_getting_current_courses<TRequest, TResponse, TService, TSemester, TCourse, TGrade>
        where TRequest : CurrentCoursesListRequest, new()
        where TResponse : CurrentCoursesModel, new()
        where TSemester : Semester, new()
        where TCourse : Course, new()
        where TGrade : Grade, new()
        where TService : CurrentCoursesListServiceBase<TRequest, TResponse, TSemester, TCourse, TGrade>, new()
    {
        protected override IQueryable<StudentRecordCurrentCourse> GetData()
        {
            var data = new List<StudentRecordCurrentCourse>
                           {
                               new StudentRecordCurrentCourse { StudentUSI = student1, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = wrongCourseCode },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school1, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = wrongCourseCode },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod3, LetterGradeEarned = suppliedLetterGrade2Period3  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod1, LetterGradeEarned = suppliedLetterGrade1Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod2, LetterGradeEarned = suppliedLetterGrade1Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod2, NumericGradeEarned = suppliedNumericGrade2Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod1, NumericGradeEarned = suppliedNumericGrade2Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod3, NumericGradeEarned = suppliedNumericGrade1Period3  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = finalGradePeriod, LetterGradeEarned = suppliedLetterGrade2Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade1Final  },
                           };
            return data.AsQueryable();
        }

        [Test]
        public void Should_have_semesters_built_correctly()
        {
            Assert.That(actualModel.Semesters.Count, Is.EqualTo(1));
            Assert.That(actualModel.Semesters[0].Term, Is.EqualTo(fallSemester));
            Assert.That(actualModel.Semesters[0].StudentUSI, Is.EqualTo(student0));
            Assert.That(actualModel.Semesters[0].AvailablePeriods.Count, Is.EqualTo(4));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[0], Is.EqualTo(GradingPeriod.One));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[1], Is.EqualTo(GradingPeriod.Two));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[2], Is.EqualTo(GradingPeriod.Three));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[3], Is.EqualTo(GradingPeriod.FinalGrade));
            Assert.That(actualModel.Semesters[0].Courses.Count, Is.EqualTo(2));
        }

        [Test]
        public void Should_have_courses_built_and_bound_correctly()
        {
            var course = actualModel.Semesters[0].Courses[0];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode1));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle1));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea1));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor1));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned1));

            course = actualModel.Semesters[0].Courses[1];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode2));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle2));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea2));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor2));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel2));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned2));
        }

        [Test]
        public void Should_have_grades_built_and_bound_correctly()
        {
            Assert.That(actualModel.Semesters[0].Courses[0].Grades.Count, Is.EqualTo(4));

            var grade = actualModel.Semesters[0].Courses[0].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period1));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[0].Courses[0].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period2));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[0].Courses[0].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade1Period3.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));

            grade = actualModel.Semesters[0].Courses[0].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade1Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[0].Courses[1].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[0].Courses[1].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period1.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[0].Courses[1].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period2.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[0].Courses[1].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade2Period3));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));

            grade = actualModel.Semesters[0].Courses[1].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade2Final));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));
        }
    }

    public class When_getting_current_courses_with_a_fall_term_form_the_service : When_getting_current_courses_with_a_fall_term<CurrentCoursesListRequest, CurrentCoursesModel, CurrentCoursesListService, Semester, Course, Grade>
    {

    }

    public abstract class When_getting_current_courses_with_a_fall_and_spring_term<TRequest, TResponse, TService, TSemester, TCourse, TGrade> : When_getting_current_courses<TRequest, TResponse, TService, TSemester, TCourse, TGrade>
        where TRequest : CurrentCoursesListRequest, new()
        where TResponse : CurrentCoursesModel, new()
        where TSemester : Semester, new()
        where TCourse : Course, new()
        where TGrade : Grade, new()
        where TService : CurrentCoursesListServiceBase<TRequest, TResponse, TSemester, TCourse, TGrade>, new()
    {
        protected override IQueryable<StudentRecordCurrentCourse> GetData()
        {
            var data = new List<StudentRecordCurrentCourse>
                           {
                               new StudentRecordCurrentCourse { StudentUSI = student1, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = wrongCourseCode },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school1, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = wrongCourseCode },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = gradingPeriod6, NumericGradeEarned = suppliedNumericGrade3Period3  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod4, LetterGradeEarned = suppliedLetterGrade1Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod5, LetterGradeEarned = suppliedLetterGrade1Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = gradingPeriod5, NumericGradeEarned = suppliedNumericGrade3Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod3, LetterGradeEarned = suppliedLetterGrade2Period3  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod1, LetterGradeEarned = suppliedLetterGrade1Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod2, LetterGradeEarned = suppliedLetterGrade1Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod2, NumericGradeEarned = suppliedNumericGrade2Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod1, NumericGradeEarned = suppliedNumericGrade2Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod3, NumericGradeEarned = suppliedNumericGrade1Period3  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = gradingPeriod4, NumericGradeEarned = suppliedNumericGrade3Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod6, NumericGradeEarned = suppliedNumericGrade1Period3  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = finalGradePeriod, LetterGradeEarned = suppliedLetterGrade2Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade3Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade1Final  },
                           };
            return data.AsQueryable();
        }            

        [Test]
        public void Should_have_semesters_built_correctly()
        {
            Assert.That(actualModel.Semesters.Count, Is.EqualTo(2));
            Assert.That(actualModel.Semesters[0].Term, Is.EqualTo(fallSemester));
            Assert.That(actualModel.Semesters[0].StudentUSI, Is.EqualTo(student0));
            Assert.That(actualModel.Semesters[0].AvailablePeriods.Count, Is.EqualTo(4));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[0], Is.EqualTo(GradingPeriod.One));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[1], Is.EqualTo(GradingPeriod.Two));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[2], Is.EqualTo(GradingPeriod.Three));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[3], Is.EqualTo(GradingPeriod.FinalGrade));
            Assert.That(actualModel.Semesters[0].Courses.Count, Is.EqualTo(2));

            Assert.That(actualModel.Semesters[1].Term, Is.EqualTo(springSemester));
            Assert.That(actualModel.Semesters[1].StudentUSI, Is.EqualTo(student0));
            Assert.That(actualModel.Semesters[1].AvailablePeriods.Count, Is.EqualTo(4));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[0], Is.EqualTo(GradingPeriod.Four));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[1], Is.EqualTo(GradingPeriod.Five));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[2], Is.EqualTo(GradingPeriod.Six));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[3], Is.EqualTo(GradingPeriod.FinalGrade));
            Assert.That(actualModel.Semesters[1].Courses.Count, Is.EqualTo(2));
        }

        [Test]
        public void Should_have_courses_built_and_bound_correctly()
        {
            var course = actualModel.Semesters[0].Courses[0];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode1));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle1));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea1));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor1));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned1));

            course = actualModel.Semesters[0].Courses[1];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode2));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle2));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea2));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor2));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel2));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned2));

            course = actualModel.Semesters[1].Courses[0];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode1));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle1));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea1));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor1));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned1));

            course = actualModel.Semesters[1].Courses[1];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode3));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle3));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea3));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor3));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel3));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned3));
        }

        [Test]
        public void Should_have_grades_built_and_bound_correctly()
        {
            Assert.That(actualModel.Semesters[0].Courses[0].Grades.Count, Is.EqualTo(4));

            var grade = actualModel.Semesters[0].Courses[0].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period1));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[0].Courses[0].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period2));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[0].Courses[0].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade1Period3.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));

            grade = actualModel.Semesters[0].Courses[0].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[0].Courses[1].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[0].Courses[1].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period1.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[0].Courses[1].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period2.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[0].Courses[1].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade2Period3));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));

            grade = actualModel.Semesters[0].Courses[1].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade2Final));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));

            Assert.That(actualModel.Semesters[1].Courses[0].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[1].Courses[0].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period1));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Four));

            grade = actualModel.Semesters[1].Courses[0].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period2));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Five));

            grade = actualModel.Semesters[1].Courses[0].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade1Period3.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Six));

            grade = actualModel.Semesters[1].Courses[0].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade1Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[1].Courses[1].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[1].Courses[1].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Period1.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Four));

            grade = actualModel.Semesters[1].Courses[1].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Period2.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Five));

            grade = actualModel.Semesters[1].Courses[1].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Period3.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Six));

            grade = actualModel.Semesters[1].Courses[1].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));
        }

        [Test]
        public override void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel.EnsureNoDefaultValues("CurrentCoursesModel.Semesters[0].Courses[0].Grades[3].Value");
        }
    }

    public class When_getting_current_courses_with_a_fall_and_spring_term_form_the_service : When_getting_current_courses_with_a_fall_and_spring_term<CurrentCoursesListRequest, CurrentCoursesModel, CurrentCoursesListService, Semester, Course, Grade>
    {

    }

    public abstract class When_getting_current_courses_with_a_year_round_term<TRequest, TResponse, TService, TSemester, TCourse, TGrade> : When_getting_current_courses<TRequest, TResponse, TService, TSemester, TCourse, TGrade>
        where TRequest : CurrentCoursesListRequest, new()
        where TResponse : CurrentCoursesModel, new()
        where TSemester : Semester, new()
        where TCourse : Course, new()
        where TGrade : Grade, new()
        where TService : CurrentCoursesListServiceBase<TRequest, TResponse, TSemester, TCourse, TGrade>, new()
    {
        protected override IQueryable<StudentRecordCurrentCourse> GetData()
        {
            var data = new List<StudentRecordCurrentCourse>
                           {
                               new StudentRecordCurrentCourse { StudentUSI = student1, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = wrongCourseCode },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school1, TermTypeId = 1, TermType = yearRound, LocalCourseCode = wrongCourseCode },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = finalGradePeriod, LetterGradeEarned = suppliedLetterGrade2Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade1Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod6, LetterGradeEarned = suppliedLetterGrade2Period6  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod4, LetterGradeEarned = suppliedLetterGrade1Period4  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod5, LetterGradeEarned = suppliedLetterGrade1Period5  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod5, NumericGradeEarned = suppliedNumericGrade2Period5  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod3, LetterGradeEarned = suppliedLetterGrade2Period3  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod1, LetterGradeEarned = suppliedLetterGrade1Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod2, LetterGradeEarned = suppliedLetterGrade1Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod2, NumericGradeEarned = suppliedNumericGrade2Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod1, NumericGradeEarned = suppliedNumericGrade2Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod3, NumericGradeEarned = suppliedNumericGrade1Period3  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod4, NumericGradeEarned = suppliedNumericGrade2Period4  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod6, NumericGradeEarned = suppliedNumericGrade1Period6  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = gradingPeriod6, NumericGradeEarned = suppliedNumericGrade3Period6  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = gradingPeriod5, NumericGradeEarned = suppliedNumericGrade3Period5  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = gradingPeriod4, NumericGradeEarned = suppliedNumericGrade3Period4  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade3Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode4, CourseTitle = suppliedCourseTitle4, SubjectArea = suppliedSubjectArea4, CreditsToBeEarned = suppliedCreditsToBeEarned4, Instructor = suppliedInstructor4, GradeLevel = suppliedGradeLevel4, GradingPeriod = gradingPeriod1, NumericGradeEarned = suppliedNumericGrade4Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode4, CourseTitle = suppliedCourseTitle4, SubjectArea = suppliedSubjectArea4, CreditsToBeEarned = suppliedCreditsToBeEarned4, Instructor = suppliedInstructor4, GradeLevel = suppliedGradeLevel4, GradingPeriod = gradingPeriod2, NumericGradeEarned = suppliedNumericGrade4Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode4, CourseTitle = suppliedCourseTitle4, SubjectArea = suppliedSubjectArea4, CreditsToBeEarned = suppliedCreditsToBeEarned4, Instructor = suppliedInstructor4, GradeLevel = suppliedGradeLevel4, GradingPeriod = gradingPeriod3, NumericGradeEarned = suppliedNumericGrade4Period3  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode4, CourseTitle = suppliedCourseTitle4, SubjectArea = suppliedSubjectArea4, CreditsToBeEarned = suppliedCreditsToBeEarned4, Instructor = suppliedInstructor4, GradeLevel = suppliedGradeLevel4, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade4Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode5, CourseTitle = suppliedCourseTitle5, SubjectArea = suppliedSubjectArea5, CreditsToBeEarned = suppliedCreditsToBeEarned5, Instructor = suppliedInstructor5, GradeLevel = suppliedGradeLevel5, GradingPeriod = gradingPeriod6, NumericGradeEarned = suppliedNumericGrade5Period6  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode5, CourseTitle = suppliedCourseTitle5, SubjectArea = suppliedSubjectArea5, CreditsToBeEarned = suppliedCreditsToBeEarned5, Instructor = suppliedInstructor5, GradeLevel = suppliedGradeLevel5, GradingPeriod = gradingPeriod5, NumericGradeEarned = suppliedNumericGrade5Period5  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode5, CourseTitle = suppliedCourseTitle5, SubjectArea = suppliedSubjectArea5, CreditsToBeEarned = suppliedCreditsToBeEarned5, Instructor = suppliedInstructor5, GradeLevel = suppliedGradeLevel5, GradingPeriod = gradingPeriod4, NumericGradeEarned = suppliedNumericGrade5Period4  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode5, CourseTitle = suppliedCourseTitle5, SubjectArea = suppliedSubjectArea5, CreditsToBeEarned = suppliedCreditsToBeEarned5, Instructor = suppliedInstructor5, GradeLevel = suppliedGradeLevel5, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade5Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = gradingPeriod3, NumericGradeEarned = suppliedNumericGrade3Period3  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = gradingPeriod2, NumericGradeEarned = suppliedNumericGrade3Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = gradingPeriod1, NumericGradeEarned = suppliedNumericGrade3Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade3Final  },
                           };
            return data.AsQueryable();
        }

        [Test]
        public void Should_have_semesters_built_correctly()
        {
            Assert.That(actualModel.Semesters.Count, Is.EqualTo(2));
            Assert.That(actualModel.Semesters[0].Term, Is.EqualTo(fallSemester));
            Assert.That(actualModel.Semesters[0].StudentUSI, Is.EqualTo(student0));
            Assert.That(actualModel.Semesters[0].AvailablePeriods.Count, Is.EqualTo(4));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[0], Is.EqualTo(GradingPeriod.One));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[1], Is.EqualTo(GradingPeriod.Two));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[2], Is.EqualTo(GradingPeriod.Three));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[3], Is.EqualTo(GradingPeriod.FinalGrade));
            Assert.That(actualModel.Semesters[0].Courses.Count, Is.EqualTo(4));

            Assert.That(actualModel.Semesters[1].Term, Is.EqualTo(springSemester));
            Assert.That(actualModel.Semesters[1].StudentUSI, Is.EqualTo(student0));
            Assert.That(actualModel.Semesters[1].AvailablePeriods.Count, Is.EqualTo(4));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[0], Is.EqualTo(GradingPeriod.Four));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[1], Is.EqualTo(GradingPeriod.Five));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[2], Is.EqualTo(GradingPeriod.Six));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[3], Is.EqualTo(GradingPeriod.FinalGrade));
            Assert.That(actualModel.Semesters[1].Courses.Count, Is.EqualTo(4));
        }

        [Test]
        public void Should_have_courses_built_and_bound_correctly()
        {
            var course = actualModel.Semesters[0].Courses[0];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode1));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle1));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea1));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor1));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned1));

            course = actualModel.Semesters[0].Courses[1];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode2));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle2));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea2));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor2));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel2));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned2));

            course = actualModel.Semesters[0].Courses[2];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode3));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle3));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea3));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor3));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel3));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned3));

            course = actualModel.Semesters[0].Courses[3];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode4));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle4));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea4));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor4));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel4));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned4));

            course = actualModel.Semesters[1].Courses[0];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode1));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle1));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea1));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor1));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned1));

            course = actualModel.Semesters[1].Courses[1];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode2));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle2));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea2));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor2));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel2));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned2));

            course = actualModel.Semesters[1].Courses[2];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode3));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle3));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea3));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor3));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel3));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned3));

            course = actualModel.Semesters[1].Courses[3];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode5));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle5));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea5));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor5));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel5));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned5));
        }

        [Test]
        public void Should_have_grades_built_and_bound_correctly()
        {
            Assert.That(actualModel.Semesters[0].Courses[0].Grades.Count, Is.EqualTo(4));

            var grade = actualModel.Semesters[0].Courses[0].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period1));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[0].Courses[0].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period2));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[0].Courses[0].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade1Period3.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));

            grade = actualModel.Semesters[0].Courses[0].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[0].Courses[1].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[0].Courses[1].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period1.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[0].Courses[1].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period2.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[0].Courses[1].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade2Period3));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));

            grade = actualModel.Semesters[0].Courses[1].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[0].Courses[2].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[0].Courses[2].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Period1.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[0].Courses[2].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Period2.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[0].Courses[2].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Period3.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));

            grade = actualModel.Semesters[0].Courses[2].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[0].Courses[3].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[0].Courses[3].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade4Period1.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[0].Courses[3].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade4Period2.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[0].Courses[3].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade4Period3.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));

            grade = actualModel.Semesters[0].Courses[3].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade4Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));

            Assert.That(actualModel.Semesters[1].Courses[0].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[1].Courses[0].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period4));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Four));

            grade = actualModel.Semesters[1].Courses[0].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period5));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Five));

            grade = actualModel.Semesters[1].Courses[0].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade1Period6.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Six));

            grade = actualModel.Semesters[1].Courses[0].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade1Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[1].Courses[1].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[1].Courses[1].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period4.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Four));

            grade = actualModel.Semesters[1].Courses[1].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period5.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Five));

            grade = actualModel.Semesters[1].Courses[1].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade2Period6));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Six));

            grade = actualModel.Semesters[1].Courses[1].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade2Final));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[1].Courses[2].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[1].Courses[2].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Period4.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Four));

            grade = actualModel.Semesters[1].Courses[2].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Period5.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Five));

            grade = actualModel.Semesters[1].Courses[2].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Period6.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Six));

            grade = actualModel.Semesters[1].Courses[2].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[1].Courses[3].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[1].Courses[3].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade5Period4.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Four));

            grade = actualModel.Semesters[1].Courses[3].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade5Period5.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Five));

            grade = actualModel.Semesters[1].Courses[3].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade5Period6.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Six));

            grade = actualModel.Semesters[1].Courses[3].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade5Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));
        }

        [Test]
        public override void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel.EnsureNoDefaultValues("CurrentCoursesModel.Semesters[0].Courses[0].Grades[3].Value");
        }
    }

    public class When_getting_current_courses_with_a_year_round_term_form_the_service : When_getting_current_courses_with_a_year_round_term<CurrentCoursesListRequest, CurrentCoursesModel, CurrentCoursesListService, Semester, Course, Grade>
    {

    }

    public abstract class When_getting_current_courses_with_a_year_round_term_and_no_fall_term<TRequest, TResponse, TService, TSemester, TCourse, TGrade> : When_getting_current_courses<TRequest, TResponse, TService, TSemester, TCourse, TGrade>
        where TRequest : CurrentCoursesListRequest, new()
        where TResponse : CurrentCoursesModel, new()
        where TSemester : Semester, new()
        where TCourse : Course, new()
        where TGrade : Grade, new()
        where TService : CurrentCoursesListServiceBase<TRequest, TResponse, TSemester, TCourse, TGrade>, new()
    {
        protected override IQueryable<StudentRecordCurrentCourse> GetData()
        {
            var data = new List<StudentRecordCurrentCourse>
                           {
                               new StudentRecordCurrentCourse { StudentUSI = student1, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = wrongCourseCode },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school1, TermTypeId = 1, TermType = yearRound, LocalCourseCode = wrongCourseCode },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod6, LetterGradeEarned = suppliedLetterGrade2Period6  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod4, LetterGradeEarned = suppliedLetterGrade1Period4  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod5, LetterGradeEarned = suppliedLetterGrade1Period5  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod5, NumericGradeEarned = suppliedNumericGrade2Period5  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod3, LetterGradeEarned = suppliedLetterGrade2Period3  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod1, LetterGradeEarned = suppliedLetterGrade1Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod2, LetterGradeEarned = suppliedLetterGrade1Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod2, NumericGradeEarned = suppliedNumericGrade2Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod1, NumericGradeEarned = suppliedNumericGrade2Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod3, NumericGradeEarned = suppliedNumericGrade1Period3  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod4, NumericGradeEarned = suppliedNumericGrade2Period4  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod6, NumericGradeEarned = suppliedNumericGrade1Period6  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = finalGradePeriod, LetterGradeEarned = suppliedLetterGrade2Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade1Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = gradingPeriod6, NumericGradeEarned = suppliedNumericGrade3Period6  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = gradingPeriod5, NumericGradeEarned = suppliedNumericGrade3Period5  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade3Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = gradingPeriod4, NumericGradeEarned = suppliedNumericGrade3Period4  },
                           };
            return data.AsQueryable();
        }

        [Test]
        public void Should_have_semesters_built_correctly()
        {
            Assert.That(actualModel.Semesters.Count, Is.EqualTo(2));
            Assert.That(actualModel.Semesters[0].Term, Is.EqualTo(fallSemester));
            Assert.That(actualModel.Semesters[0].StudentUSI, Is.EqualTo(student0));
            Assert.That(actualModel.Semesters[0].AvailablePeriods.Count, Is.EqualTo(3));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[0], Is.EqualTo(GradingPeriod.One));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[1], Is.EqualTo(GradingPeriod.Two));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[2], Is.EqualTo(GradingPeriod.Three));
            Assert.That(actualModel.Semesters[0].Courses.Count, Is.EqualTo(2));

            Assert.That(actualModel.Semesters[1].Term, Is.EqualTo(springSemester));
            Assert.That(actualModel.Semesters[1].StudentUSI, Is.EqualTo(student0));
            Assert.That(actualModel.Semesters[1].AvailablePeriods.Count, Is.EqualTo(4));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[0], Is.EqualTo(GradingPeriod.Four));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[1], Is.EqualTo(GradingPeriod.Five));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[2], Is.EqualTo(GradingPeriod.Six));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[3], Is.EqualTo(GradingPeriod.FinalGrade));
            Assert.That(actualModel.Semesters[1].Courses.Count, Is.EqualTo(3));
        }

        [Test]
        public void Should_have_courses_built_and_bound_correctly()
        {
            var course = actualModel.Semesters[0].Courses[0];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode1));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle1));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea1));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor1));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned1));

            course = actualModel.Semesters[0].Courses[1];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode2));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle2));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea2));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor2));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel2));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned2));

            course = actualModel.Semesters[1].Courses[0];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode1));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle1));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea1));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor1));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned1));

            course = actualModel.Semesters[1].Courses[1];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode2));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle2));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea2));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor2));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel2));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned2));

            course = actualModel.Semesters[1].Courses[2];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode3));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle3));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea3));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor3));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel3));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned3));
        }

        [Test]
        public void Should_have_grades_built_and_bound_correctly()
        {
            Assert.That(actualModel.Semesters[0].Courses[0].Grades.Count, Is.EqualTo(3));

            var grade = actualModel.Semesters[0].Courses[0].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period1));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[0].Courses[0].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period2));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[0].Courses[0].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade1Period3.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));


            Assert.That(actualModel.Semesters[0].Courses[1].Grades.Count, Is.EqualTo(3));

            grade = actualModel.Semesters[0].Courses[1].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period1.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[0].Courses[1].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period2.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[0].Courses[1].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade2Period3));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));

            Assert.That(actualModel.Semesters[1].Courses[0].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[1].Courses[0].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period4));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Four));

            grade = actualModel.Semesters[1].Courses[0].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period5));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Five));

            grade = actualModel.Semesters[1].Courses[0].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade1Period6.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Six));

            grade = actualModel.Semesters[1].Courses[0].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade1Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[1].Courses[1].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[1].Courses[1].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period4.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Four));

            grade = actualModel.Semesters[1].Courses[1].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period5.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Five));

            grade = actualModel.Semesters[1].Courses[1].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade2Period6));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Six));

            grade = actualModel.Semesters[1].Courses[1].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade2Final));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[1].Courses[2].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[1].Courses[2].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Period4.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Four));

            grade = actualModel.Semesters[1].Courses[2].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Period5.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Five));

            grade = actualModel.Semesters[1].Courses[2].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Period6.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Six));

            grade = actualModel.Semesters[1].Courses[2].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));
        }
    }

    public class When_getting_current_courses_with_a_year_round_term_and_no_fall_term_form_the_service : When_getting_current_courses_with_a_year_round_term_and_no_fall_term<CurrentCoursesListRequest, CurrentCoursesModel, CurrentCoursesListService, Semester, Course, Grade>
    {

    }

    public abstract class When_getting_current_courses_with_a_year_round_term_and_no_spring_term<TRequest, TResponse, TService, TSemester, TCourse, TGrade> : When_getting_current_courses<TRequest, TResponse, TService, TSemester, TCourse, TGrade>
        where TRequest : CurrentCoursesListRequest, new()
        where TResponse : CurrentCoursesModel, new()
        where TSemester : Semester, new()
        where TCourse : Course, new()
        where TGrade : Grade, new()
        where TService : CurrentCoursesListServiceBase<TRequest, TResponse, TSemester, TCourse, TGrade>, new()
    {
        protected override IQueryable<StudentRecordCurrentCourse> GetData()
        {
            var data = new List<StudentRecordCurrentCourse>
                           {
                               new StudentRecordCurrentCourse { StudentUSI = student1, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = wrongCourseCode },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school1, TermTypeId = 1, TermType = yearRound, LocalCourseCode = wrongCourseCode },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod6, LetterGradeEarned = suppliedLetterGrade2Period6  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod4, LetterGradeEarned = suppliedLetterGrade1Period4  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod5, LetterGradeEarned = suppliedLetterGrade1Period5  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod5, NumericGradeEarned = suppliedNumericGrade2Period5  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod3, LetterGradeEarned = suppliedLetterGrade2Period3  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod1, LetterGradeEarned = suppliedLetterGrade1Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod2, LetterGradeEarned = suppliedLetterGrade1Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod2, NumericGradeEarned = suppliedNumericGrade2Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod1, NumericGradeEarned = suppliedNumericGrade2Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod3, NumericGradeEarned = suppliedNumericGrade1Period3  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod4, NumericGradeEarned = suppliedNumericGrade2Period4  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod6, NumericGradeEarned = suppliedNumericGrade1Period6  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = finalGradePeriod, LetterGradeEarned = suppliedLetterGrade2Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade1Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = gradingPeriod3, NumericGradeEarned = suppliedNumericGrade3Period3  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = gradingPeriod2, NumericGradeEarned = suppliedNumericGrade3Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = gradingPeriod1, NumericGradeEarned = suppliedNumericGrade3Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade3Final  },
                           };
            return data.AsQueryable();
        }

        [Test]
        public void Should_have_semesters_built_correctly()
        {
            Assert.That(actualModel.Semesters.Count, Is.EqualTo(2));
            Assert.That(actualModel.Semesters[0].Term, Is.EqualTo(fallSemester));
            Assert.That(actualModel.Semesters[0].StudentUSI, Is.EqualTo(student0));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[0], Is.EqualTo(GradingPeriod.One));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[1], Is.EqualTo(GradingPeriod.Two));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[2], Is.EqualTo(GradingPeriod.Three));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[3], Is.EqualTo(GradingPeriod.FinalGrade));
            Assert.That(actualModel.Semesters[0].Courses.Count, Is.EqualTo(3));

            Assert.That(actualModel.Semesters[1].Term, Is.EqualTo(springSemester));
            Assert.That(actualModel.Semesters[1].StudentUSI, Is.EqualTo(student0));
            Assert.That(actualModel.Semesters[1].AvailablePeriods.Count, Is.EqualTo(4));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[0], Is.EqualTo(GradingPeriod.Four));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[1], Is.EqualTo(GradingPeriod.Five));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[2], Is.EqualTo(GradingPeriod.Six));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[3], Is.EqualTo(GradingPeriod.FinalGrade));
            Assert.That(actualModel.Semesters[1].Courses.Count, Is.EqualTo(2));
        }

        [Test]
        public void Should_have_courses_built_and_bound_correctly()
        {
            var course = actualModel.Semesters[0].Courses[0];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode1));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle1));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea1));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor1));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned1));

            course = actualModel.Semesters[0].Courses[1];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode2));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle2));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea2));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor2));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel2));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned2));

            course = actualModel.Semesters[0].Courses[2];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode3));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle3));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea3));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor3));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel3));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned3));

            course = actualModel.Semesters[1].Courses[0];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode1));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle1));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea1));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor1));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned1));

            course = actualModel.Semesters[1].Courses[1];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode2));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle2));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea2));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor2));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel2));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned2));
        }

        [Test]
        public void Should_have_grades_built_and_bound_correctly()
        {
            Assert.That(actualModel.Semesters[0].Courses[0].Grades.Count, Is.EqualTo(4));

            var grade = actualModel.Semesters[0].Courses[0].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period1));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[0].Courses[0].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period2));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[0].Courses[0].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade1Period3.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));

            grade = actualModel.Semesters[0].Courses[0].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[0].Courses[1].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[0].Courses[1].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period1.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[0].Courses[1].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period2.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[0].Courses[1].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade2Period3));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));

            grade = actualModel.Semesters[0].Courses[1].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[0].Courses[2].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[0].Courses[2].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Period1.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[0].Courses[2].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Period2.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[0].Courses[2].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Period3.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));

            grade = actualModel.Semesters[0].Courses[2].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));

            Assert.That(actualModel.Semesters[1].Courses[0].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[1].Courses[0].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period4));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Four));

            grade = actualModel.Semesters[1].Courses[0].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period5));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Five));

            grade = actualModel.Semesters[1].Courses[0].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade1Period6.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Six));

            grade = actualModel.Semesters[1].Courses[0].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade1Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[1].Courses[1].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[1].Courses[1].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period4.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Four));

            grade = actualModel.Semesters[1].Courses[1].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period5.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Five));

            grade = actualModel.Semesters[1].Courses[1].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade2Period6));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Six));

            grade = actualModel.Semesters[1].Courses[1].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade2Final));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));
        }

        [Test]
        public override void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel.EnsureNoDefaultValues("CurrentCoursesModel.Semesters[0].Courses[0].Grades[3].Value");
        }
    }

    public class When_getting_current_courses_with_a_year_round_term_and_no_spring_term_form_the_service : When_getting_current_courses_with_a_year_round_term_and_no_spring_term<CurrentCoursesListRequest, CurrentCoursesModel, CurrentCoursesListService, Semester, Course, Grade>
    {

    }

    public abstract class When_getting_current_courses_with_a_year_round_term_and_no_fall_or_spring_terms<TRequest, TResponse, TService, TSemester, TCourse, TGrade> : When_getting_current_courses<TRequest, TResponse, TService, TSemester, TCourse, TGrade>
        where TRequest : CurrentCoursesListRequest, new()
        where TResponse : CurrentCoursesModel, new()
        where TSemester : Semester, new()
        where TCourse : Course, new()
        where TGrade : Grade, new()
        where TService : CurrentCoursesListServiceBase<TRequest, TResponse, TSemester, TCourse, TGrade>, new()
    {
        protected override IQueryable<StudentRecordCurrentCourse> GetData()
        {
            var data = new List<StudentRecordCurrentCourse>
                           {
                               new StudentRecordCurrentCourse { StudentUSI = student1, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = wrongCourseCode },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school1, TermTypeId = 1, TermType = yearRound, LocalCourseCode = wrongCourseCode },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod6, LetterGradeEarned = suppliedLetterGrade2Period6  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod4, LetterGradeEarned = suppliedLetterGrade1Period4  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod5, LetterGradeEarned = suppliedLetterGrade1Period5  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod5, NumericGradeEarned = suppliedNumericGrade2Period5  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod3, LetterGradeEarned = suppliedLetterGrade2Period3  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod1, LetterGradeEarned = suppliedLetterGrade1Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod2, LetterGradeEarned = suppliedLetterGrade1Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod2, NumericGradeEarned = suppliedNumericGrade2Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod1, NumericGradeEarned = suppliedNumericGrade2Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod3, NumericGradeEarned = suppliedNumericGrade1Period3  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod4, NumericGradeEarned = suppliedNumericGrade2Period4  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod6, NumericGradeEarned = suppliedNumericGrade1Period6  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = finalGradePeriod, LetterGradeEarned = suppliedLetterGrade2Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade1Final  },
                           };
            return data.AsQueryable();
        }

        [Test]
        public void Should_have_semesters_built_correctly()
        {
            Assert.That(actualModel.Semesters.Count, Is.EqualTo(2));
            Assert.That(actualModel.Semesters[0].Term, Is.EqualTo(fallSemester));
            Assert.That(actualModel.Semesters[0].StudentUSI, Is.EqualTo(student0));
            Assert.That(actualModel.Semesters[0].AvailablePeriods.Count, Is.EqualTo(3));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[0], Is.EqualTo(GradingPeriod.One));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[1], Is.EqualTo(GradingPeriod.Two));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[2], Is.EqualTo(GradingPeriod.Three));
            Assert.That(actualModel.Semesters[0].Courses.Count, Is.EqualTo(2));

            Assert.That(actualModel.Semesters[1].Term, Is.EqualTo(springSemester));
            Assert.That(actualModel.Semesters[1].StudentUSI, Is.EqualTo(student0));
            Assert.That(actualModel.Semesters[1].AvailablePeriods.Count, Is.EqualTo(4));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[0], Is.EqualTo(GradingPeriod.Four));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[1], Is.EqualTo(GradingPeriod.Five));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[2], Is.EqualTo(GradingPeriod.Six));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[3], Is.EqualTo(GradingPeriod.FinalGrade));
            Assert.That(actualModel.Semesters[1].Courses.Count, Is.EqualTo(2));
        }

        [Test]
        public void Should_have_courses_built_and_bound_correctly()
        {
            var course = actualModel.Semesters[0].Courses[0];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode1));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle1));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea1));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor1));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned1));

            course = actualModel.Semesters[0].Courses[1];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode2));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle2));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea2));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor2));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel2));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned2));

            course = actualModel.Semesters[1].Courses[0];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode1));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle1));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea1));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor1));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned1));

            course = actualModel.Semesters[1].Courses[1];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode2));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle2));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea2));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor2));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel2));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned2));
        }

        [Test]
        public void Should_have_grades_built_and_bound_correctly()
        {
            Assert.That(actualModel.Semesters[0].Courses[0].Grades.Count, Is.EqualTo(3));

            var grade = actualModel.Semesters[0].Courses[0].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period1));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[0].Courses[0].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period2));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[0].Courses[0].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade1Period3.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));


            Assert.That(actualModel.Semesters[0].Courses[1].Grades.Count, Is.EqualTo(3));

            grade = actualModel.Semesters[0].Courses[1].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period1.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[0].Courses[1].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period2.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[0].Courses[1].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade2Period3));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));

            Assert.That(actualModel.Semesters[1].Courses[0].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[1].Courses[0].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period4));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Four));

            grade = actualModel.Semesters[1].Courses[0].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period5));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Five));

            grade = actualModel.Semesters[1].Courses[0].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade1Period6.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Six));

            grade = actualModel.Semesters[1].Courses[0].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade1Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[1].Courses[1].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[1].Courses[1].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period4.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Four));

            grade = actualModel.Semesters[1].Courses[1].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period5.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Five));

            grade = actualModel.Semesters[1].Courses[1].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade2Period6));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Six));

            grade = actualModel.Semesters[1].Courses[1].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade2Final));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));
        }
    }

    public class When_getting_current_courses_with_a_year_round_term_and_no_fall_or_spring_terms_form_the_service : When_getting_current_courses_with_a_year_round_term_and_no_fall_or_spring_terms<CurrentCoursesListRequest, CurrentCoursesModel, CurrentCoursesListService, Semester, Course, Grade>
    {

    }

    public abstract class When_getting_current_courses_with_a_year_round_term_with_no_fall_grades<TRequest, TResponse, TService, TSemester, TCourse, TGrade> : When_getting_current_courses<TRequest, TResponse, TService, TSemester, TCourse, TGrade>
        where TRequest : CurrentCoursesListRequest, new()
        where TResponse : CurrentCoursesModel, new()
        where TSemester : Semester, new()
        where TCourse : Course, new()
        where TGrade : Grade, new()
        where TService : CurrentCoursesListServiceBase<TRequest, TResponse, TSemester, TCourse, TGrade>, new()
    {
        protected override IQueryable<StudentRecordCurrentCourse> GetData()
        {
            var data = new List<StudentRecordCurrentCourse>
                           {
                               new StudentRecordCurrentCourse { StudentUSI = student1, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = wrongCourseCode },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school1, TermTypeId = 1, TermType = yearRound, LocalCourseCode = wrongCourseCode },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod6, LetterGradeEarned = suppliedLetterGrade2Period6  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod4, LetterGradeEarned = suppliedLetterGrade1Period4  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod5, LetterGradeEarned = suppliedLetterGrade1Period5  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod4, NumericGradeEarned = suppliedNumericGrade2Period4  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade1Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = finalGradePeriod, LetterGradeEarned = suppliedLetterGrade2Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod1, LetterGradeEarned = null  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod2, LetterGradeEarned = null  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod3, LetterGradeEarned = null  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod5, LetterGradeEarned = null  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod1, LetterGradeEarned = null  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod2, LetterGradeEarned = null  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod3, LetterGradeEarned = null  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod6, LetterGradeEarned = null  },
                           };
            return data.AsQueryable();
        }

        [Test]
        public void Should_have_semesters_built_correctly()
        {
            Assert.That(actualModel.Semesters.Count, Is.EqualTo(2));
            Assert.That(actualModel.Semesters[0].Term, Is.EqualTo(fallSemester));
            Assert.That(actualModel.Semesters[0].StudentUSI, Is.EqualTo(student0));
            Assert.That(actualModel.Semesters[0].AvailablePeriods.Count, Is.EqualTo(3));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[0], Is.EqualTo(GradingPeriod.One));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[1], Is.EqualTo(GradingPeriod.Two));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[2], Is.EqualTo(GradingPeriod.Three));
            Assert.That(actualModel.Semesters[0].Courses.Count, Is.EqualTo(2));

            Assert.That(actualModel.Semesters[1].Term, Is.EqualTo(springSemester));
            Assert.That(actualModel.Semesters[1].StudentUSI, Is.EqualTo(student0));
            Assert.That(actualModel.Semesters[1].AvailablePeriods.Count, Is.EqualTo(4));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[0], Is.EqualTo(GradingPeriod.Four));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[1], Is.EqualTo(GradingPeriod.Five));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[2], Is.EqualTo(GradingPeriod.Six));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[3], Is.EqualTo(GradingPeriod.FinalGrade));
            Assert.That(actualModel.Semesters[1].Courses.Count, Is.EqualTo(2));
        }

        [Test]
        public void Should_have_courses_built_and_bound_correctly()
        {
            var course = actualModel.Semesters[0].Courses[0];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode1));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle1));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea1));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor1));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned1));

            course = actualModel.Semesters[0].Courses[1];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode2));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle2));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea2));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor2));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel2));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned2));

            course = actualModel.Semesters[1].Courses[0];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode1));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle1));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea1));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor1));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned1));

            course = actualModel.Semesters[1].Courses[1];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode2));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle2));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea2));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor2));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel2));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned2));
        }

        [Test]
        public void Should_have_grades_built_and_bound_correctly()
        {
            Assert.That(actualModel.Semesters[0].Courses[0].Grades.Count, Is.EqualTo(3));

            var grade = actualModel.Semesters[0].Courses[0].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[0].Courses[0].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[0].Courses[0].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));


            Assert.That(actualModel.Semesters[0].Courses[1].Grades.Count, Is.EqualTo(3));

            grade = actualModel.Semesters[0].Courses[1].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[0].Courses[1].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[0].Courses[1].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));

            Assert.That(actualModel.Semesters[1].Courses[0].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[1].Courses[0].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period4));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Four));

            grade = actualModel.Semesters[1].Courses[0].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period5));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Five));

            grade = actualModel.Semesters[1].Courses[0].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Six));

            grade = actualModel.Semesters[1].Courses[0].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade1Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[1].Courses[1].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[1].Courses[1].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period4.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Four));

            grade = actualModel.Semesters[1].Courses[1].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Five));

            grade = actualModel.Semesters[1].Courses[1].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade2Period6));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Six));

            grade = actualModel.Semesters[1].Courses[1].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade2Final));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));
        }

        [Test]
        public override void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel.EnsureNoDefaultValues("CurrentCoursesModel.Semesters[1].Courses[0].Grades[2].Value",
                                              "CurrentCoursesModel.Semesters[1].Courses[1].Grades[1].Value",
                                              "CurrentCoursesModel.Semesters[0].Courses[0].Grades[0].Value",
                                              "CurrentCoursesModel.Semesters[0].Courses[0].Grades[1].Value",
                                              "CurrentCoursesModel.Semesters[0].Courses[0].Grades[2].Value");
        }

    }

    public class When_getting_current_courses_with_a_year_round_term_with_no_fall_grades_form_the_service : When_getting_current_courses_with_a_year_round_term_with_no_fall_grades<CurrentCoursesListRequest, CurrentCoursesModel, CurrentCoursesListService, Semester, Course, Grade>
    {

    }

    public abstract class When_getting_current_courses_with_a_year_round_term_with_no_spring_grades<TRequest, TResponse, TService, TSemester, TCourse, TGrade> : When_getting_current_courses<TRequest, TResponse, TService, TSemester, TCourse, TGrade>
        where TRequest : CurrentCoursesListRequest, new()
        where TResponse : CurrentCoursesModel, new()
        where TSemester : Semester, new()
        where TCourse : Course, new()
        where TGrade : Grade, new()
        where TService : CurrentCoursesListServiceBase<TRequest, TResponse, TSemester, TCourse, TGrade>, new()
    {
        protected override IQueryable<StudentRecordCurrentCourse> GetData()
        {
            var data = new List<StudentRecordCurrentCourse>
                           {
                               new StudentRecordCurrentCourse { StudentUSI = student1, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = wrongCourseCode },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school1, TermTypeId = 1, TermType = yearRound, LocalCourseCode = wrongCourseCode },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod1, LetterGradeEarned = suppliedLetterGrade1Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod2, LetterGradeEarned = suppliedLetterGrade1Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod1, NumericGradeEarned = suppliedNumericGrade2Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod3, NumericGradeEarned = null  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod4, NumericGradeEarned = null  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod5, NumericGradeEarned = null  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod6, NumericGradeEarned = null  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = finalGradePeriod, NumericGradeEarned = null  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod2, NumericGradeEarned = null  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod3, LetterGradeEarned = null  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod4, LetterGradeEarned = null  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod5, LetterGradeEarned = null  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod6, LetterGradeEarned = null  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = finalGradePeriod, LetterGradeEarned = null  },
                           };
            return data.AsQueryable();
        }

        [Test]
        public void Should_have_semesters_built_correctly()
        {
            Assert.That(actualModel.Semesters.Count, Is.EqualTo(2));
            Assert.That(actualModel.Semesters[0].Term, Is.EqualTo(fallSemester));
            Assert.That(actualModel.Semesters[0].StudentUSI, Is.EqualTo(student0));
            Assert.That(actualModel.Semesters[0].AvailablePeriods.Count, Is.EqualTo(3));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[0], Is.EqualTo(GradingPeriod.One));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[1], Is.EqualTo(GradingPeriod.Two));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[2], Is.EqualTo(GradingPeriod.Three));
            Assert.That(actualModel.Semesters[0].Courses.Count, Is.EqualTo(2));

            Assert.That(actualModel.Semesters[1].Term, Is.EqualTo(springSemester));
            Assert.That(actualModel.Semesters[1].StudentUSI, Is.EqualTo(student0));
            Assert.That(actualModel.Semesters[1].AvailablePeriods.Count, Is.EqualTo(4));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[0], Is.EqualTo(GradingPeriod.Four));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[1], Is.EqualTo(GradingPeriod.Five));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[2], Is.EqualTo(GradingPeriod.Six));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[3], Is.EqualTo(GradingPeriod.FinalGrade));
            Assert.That(actualModel.Semesters[1].Courses.Count, Is.EqualTo(2));
        }

        [Test]
        public void Should_have_courses_built_and_bound_correctly()
        {
            var course = actualModel.Semesters[0].Courses[0];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode1));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle1));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea1));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor1));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned1));

            course = actualModel.Semesters[0].Courses[1];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode2));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle2));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea2));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor2));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel2));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned2));

            course = actualModel.Semesters[1].Courses[0];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode1));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle1));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea1));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor1));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned1));

            course = actualModel.Semesters[1].Courses[1];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode2));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle2));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea2));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor2));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel2));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned2));
        }

        [Test]
        public void Should_have_grades_built_and_bound_correctly()
        {
            Assert.That(actualModel.Semesters[0].Courses[0].Grades.Count, Is.EqualTo(3));

            var grade = actualModel.Semesters[0].Courses[0].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period1));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[0].Courses[0].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period2));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[0].Courses[0].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));


            Assert.That(actualModel.Semesters[0].Courses[1].Grades.Count, Is.EqualTo(3));

            grade = actualModel.Semesters[0].Courses[1].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period1.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[0].Courses[1].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[0].Courses[1].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));

            Assert.That(actualModel.Semesters[1].Courses[0].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[1].Courses[0].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Four));

            grade = actualModel.Semesters[1].Courses[0].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Five));

            grade = actualModel.Semesters[1].Courses[0].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Six));

            grade = actualModel.Semesters[1].Courses[0].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[1].Courses[1].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[1].Courses[1].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Four));

            grade = actualModel.Semesters[1].Courses[1].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Five));

            grade = actualModel.Semesters[1].Courses[1].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Six));

            grade = actualModel.Semesters[1].Courses[1].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));
        }

        [Test]
        public override void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel.EnsureNoDefaultValues("CurrentCoursesModel.Semesters[0].Courses[0].Grades[2].Value", "CurrentCoursesModel.Semesters[0].Courses[1].Grades[1].Value");
        }

    }

    public class When_getting_current_courses_with_a_year_round_term_with_no_spring_grades_form_the_service : When_getting_current_courses_with_a_year_round_term_with_no_spring_grades<CurrentCoursesListRequest, CurrentCoursesModel, CurrentCoursesListService, Semester, Course, Grade>
    {

    }

    public abstract class When_getting_current_courses_with_a_summer_term<TRequest, TResponse, TService, TSemester, TCourse, TGrade> : When_getting_current_courses<TRequest, TResponse, TService, TSemester, TCourse, TGrade>
        where TRequest : CurrentCoursesListRequest, new()
        where TResponse : CurrentCoursesModel, new()
        where TSemester : Semester, new()
        where TCourse : Course, new()
        where TGrade : Grade, new()
        where TService : CurrentCoursesListServiceBase<TRequest, TResponse, TSemester, TCourse, TGrade>, new()
    {
        // since we don't know what grading periods look like for summer terms
        // we'll just label our summer grading periods 1, 2, 3
        protected override IQueryable<StudentRecordCurrentCourse> GetData()
        {
            var data = new List<StudentRecordCurrentCourse>
                           {
                               new StudentRecordCurrentCourse { StudentUSI = student1, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = wrongCourseCode },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school1, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = wrongCourseCode },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = gradingPeriod6, NumericGradeEarned = suppliedNumericGrade3Period3  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod4, LetterGradeEarned = suppliedLetterGrade1Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod5, LetterGradeEarned = suppliedLetterGrade1Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = gradingPeriod5, NumericGradeEarned = suppliedNumericGrade3Period2  },
                               
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = finalGradePeriod, LetterGradeEarned = suppliedLetterGrade2Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade1Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod3, LetterGradeEarned = suppliedLetterGrade2Period3  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod1, LetterGradeEarned = suppliedLetterGrade1Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod2, LetterGradeEarned = suppliedLetterGrade1Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod2, NumericGradeEarned = suppliedNumericGrade2Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod1, NumericGradeEarned = suppliedNumericGrade2Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod3, NumericGradeEarned = suppliedNumericGrade1Period3  },
                               
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = gradingPeriod4, NumericGradeEarned = suppliedNumericGrade3Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod6, NumericGradeEarned = suppliedNumericGrade1Period3  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade3Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade1Final  },
                              
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 3, TermType = summerSemester, LocalCourseCode = suppliedCourseCode4, CourseTitle = suppliedCourseTitle4, SubjectArea = suppliedSubjectArea4, CreditsToBeEarned = suppliedCreditsToBeEarned4, Instructor = suppliedInstructor4, GradeLevel = suppliedGradeLevel4, GradingPeriod = gradingPeriod1, NumericGradeEarned = suppliedNumericGrade4Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 3, TermType = summerSemester, LocalCourseCode = suppliedCourseCode4, CourseTitle = suppliedCourseTitle4, SubjectArea = suppliedSubjectArea4, CreditsToBeEarned = suppliedCreditsToBeEarned4, Instructor = suppliedInstructor4, GradeLevel = suppliedGradeLevel4, GradingPeriod = gradingPeriod2, NumericGradeEarned = suppliedNumericGrade4Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 3, TermType = summerSemester, LocalCourseCode = suppliedCourseCode4, CourseTitle = suppliedCourseTitle4, SubjectArea = suppliedSubjectArea4, CreditsToBeEarned = suppliedCreditsToBeEarned4, Instructor = suppliedInstructor4, GradeLevel = suppliedGradeLevel4, GradingPeriod = gradingPeriod3, NumericGradeEarned = suppliedNumericGrade4Period3  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 3, TermType = summerSemester, LocalCourseCode = suppliedCourseCode5, CourseTitle = suppliedCourseTitle5, SubjectArea = suppliedSubjectArea5, CreditsToBeEarned = suppliedCreditsToBeEarned5, Instructor = suppliedInstructor5, GradeLevel = suppliedGradeLevel5, GradingPeriod = gradingPeriod3, NumericGradeEarned = suppliedNumericGrade5Period3  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 3, TermType = summerSemester, LocalCourseCode = suppliedCourseCode5, CourseTitle = suppliedCourseTitle5, SubjectArea = suppliedSubjectArea5, CreditsToBeEarned = suppliedCreditsToBeEarned5, Instructor = suppliedInstructor5, GradeLevel = suppliedGradeLevel5, GradingPeriod = gradingPeriod2, NumericGradeEarned = suppliedNumericGrade5Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 3, TermType = summerSemester, LocalCourseCode = suppliedCourseCode5, CourseTitle = suppliedCourseTitle5, SubjectArea = suppliedSubjectArea5, CreditsToBeEarned = suppliedCreditsToBeEarned5, Instructor = suppliedInstructor5, GradeLevel = suppliedGradeLevel5, GradingPeriod = gradingPeriod1, NumericGradeEarned = suppliedNumericGrade5Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 3, TermType = summerSemester, LocalCourseCode = suppliedCourseCode4, CourseTitle = suppliedCourseTitle4, SubjectArea = suppliedSubjectArea4, CreditsToBeEarned = suppliedCreditsToBeEarned4, Instructor = suppliedInstructor4, GradeLevel = suppliedGradeLevel4, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade4Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 3, TermType = summerSemester, LocalCourseCode = suppliedCourseCode5, CourseTitle = suppliedCourseTitle5, SubjectArea = suppliedSubjectArea5, CreditsToBeEarned = suppliedCreditsToBeEarned5, Instructor = suppliedInstructor5, GradeLevel = suppliedGradeLevel5, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade5Final  },
                               
                           };
            return data.AsQueryable();
        }

        [Test]
        public void Should_have_semesters_built_correctly()
        {
            Assert.That(actualModel.Semesters.Count, Is.EqualTo(3));
            Assert.That(actualModel.Semesters[0].Term, Is.EqualTo(fallSemester));
            Assert.That(actualModel.Semesters[0].StudentUSI, Is.EqualTo(student0));
            Assert.That(actualModel.Semesters[0].AvailablePeriods.Count, Is.EqualTo(4));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[0], Is.EqualTo(GradingPeriod.One));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[1], Is.EqualTo(GradingPeriod.Two));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[2], Is.EqualTo(GradingPeriod.Three));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[3], Is.EqualTo(GradingPeriod.FinalGrade));
            Assert.That(actualModel.Semesters[0].Courses.Count, Is.EqualTo(2));

            Assert.That(actualModel.Semesters[1].Term, Is.EqualTo(springSemester));
            Assert.That(actualModel.Semesters[1].StudentUSI, Is.EqualTo(student0));
            Assert.That(actualModel.Semesters[1].AvailablePeriods.Count, Is.EqualTo(4));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[0], Is.EqualTo(GradingPeriod.Four));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[1], Is.EqualTo(GradingPeriod.Five));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[2], Is.EqualTo(GradingPeriod.Six));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[3], Is.EqualTo(GradingPeriod.FinalGrade));
            Assert.That(actualModel.Semesters[1].Courses.Count, Is.EqualTo(2));

            Assert.That(actualModel.Semesters[2].Term, Is.EqualTo(summerSemester));
            Assert.That(actualModel.Semesters[2].StudentUSI, Is.EqualTo(student0));
            Assert.That(actualModel.Semesters[2].AvailablePeriods.Count, Is.EqualTo(4));
            Assert.That(actualModel.Semesters[2].AvailablePeriods[0], Is.EqualTo(GradingPeriod.One));
            Assert.That(actualModel.Semesters[2].AvailablePeriods[1], Is.EqualTo(GradingPeriod.Two));
            Assert.That(actualModel.Semesters[2].AvailablePeriods[2], Is.EqualTo(GradingPeriod.Three));
            Assert.That(actualModel.Semesters[2].AvailablePeriods[3], Is.EqualTo(GradingPeriod.FinalGrade));
            Assert.That(actualModel.Semesters[2].Courses.Count, Is.EqualTo(2));
        }

        [Test]
        public void Should_have_courses_built_and_bound_correctly()
        {
            var course = actualModel.Semesters[0].Courses[0];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode1));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle1));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea1));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor1));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned1));

            course = actualModel.Semesters[0].Courses[1];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode2));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle2));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea2));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor2));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel2));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned2));

            course = actualModel.Semesters[1].Courses[0];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode1));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle1));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea1));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor1));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned1));

            course = actualModel.Semesters[1].Courses[1];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode3));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle3));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea3));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor3));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel3));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned3));

            course = actualModel.Semesters[2].Courses[0];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode4));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle4));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea4));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor4));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel4));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned4));

            course = actualModel.Semesters[2].Courses[1];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode5));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle5));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea5));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor5));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel5));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned5));
        }

        [Test]
        public void Should_have_grades_built_and_bound_correctly()
        {
            Assert.That(actualModel.Semesters[0].Courses[0].Grades.Count, Is.EqualTo(4));

            var grade = actualModel.Semesters[0].Courses[0].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period1));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[0].Courses[0].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period2));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[0].Courses[0].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade1Period3.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));

            grade = actualModel.Semesters[0].Courses[0].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade1Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[0].Courses[1].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[0].Courses[1].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period1.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[0].Courses[1].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period2.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[0].Courses[1].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade2Period3));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));

            grade = actualModel.Semesters[0].Courses[1].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade2Final));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));

            Assert.That(actualModel.Semesters[1].Courses[0].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[1].Courses[0].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period1));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Four));

            grade = actualModel.Semesters[1].Courses[0].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period2));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Five));

            grade = actualModel.Semesters[1].Courses[0].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade1Period3.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Six));

            grade = actualModel.Semesters[1].Courses[0].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade1Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[1].Courses[1].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[1].Courses[1].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Period1.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Four));

            grade = actualModel.Semesters[1].Courses[1].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Period2.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Five));

            grade = actualModel.Semesters[1].Courses[1].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Period3.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Six));

            grade = actualModel.Semesters[1].Courses[1].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[2].Courses[1].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[2].Courses[0].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade4Period1.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[2].Courses[0].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade4Period2.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[2].Courses[0].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade4Period3.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));

            grade = actualModel.Semesters[2].Courses[0].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade4Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[2].Courses[1].Grades.Count, Is.EqualTo(4));

            grade = actualModel.Semesters[2].Courses[1].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade5Period1.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[2].Courses[1].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade5Period2.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[2].Courses[1].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade5Period3.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));

            grade = actualModel.Semesters[2].Courses[1].Grades[3];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade5Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));
        }
    }

    public class When_getting_current_courses_with_a_summer_term_form_the_service : When_getting_current_courses_with_a_summer_term<CurrentCoursesListRequest, CurrentCoursesModel, CurrentCoursesListService, Semester, Course, Grade>
    {

    }

    public abstract class When_getting_current_courses_with_a_nine_week_grading_period<TRequest, TResponse, TService, TSemester, TCourse, TGrade> : When_getting_current_courses<TRequest, TResponse, TService, TSemester, TCourse, TGrade>
        where TRequest : CurrentCoursesListRequest, new()
        where TResponse : CurrentCoursesModel, new()
        where TSemester : Semester, new()
        where TCourse : Course, new()
        where TGrade : Grade, new()
        where TService : CurrentCoursesListServiceBase<TRequest, TResponse, TSemester, TCourse, TGrade>, new()
    {
        protected override IQueryable<StudentRecordCurrentCourse> GetData()
        {
            var data = new List<StudentRecordCurrentCourse>
                           {
                               new StudentRecordCurrentCourse { StudentUSI = student1, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = wrongCourseCode },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school1, TermTypeId = 1, TermType = yearRound, LocalCourseCode = wrongCourseCode },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod3, LetterGradeEarned = suppliedLetterGrade1Period4  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod4, LetterGradeEarned = suppliedLetterGrade1Period5  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod4, NumericGradeEarned = suppliedNumericGrade2Period5  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod1, LetterGradeEarned = suppliedLetterGrade1Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = gradingPeriod2, LetterGradeEarned = suppliedLetterGrade1Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod2, NumericGradeEarned = suppliedNumericGrade2Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod1, NumericGradeEarned = suppliedNumericGrade2Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = gradingPeriod3, NumericGradeEarned = suppliedNumericGrade2Period4  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode1, CourseTitle = suppliedCourseTitle1, SubjectArea = suppliedSubjectArea1, CreditsToBeEarned = suppliedCreditsToBeEarned1, Instructor = suppliedInstructor1, GradeLevel = suppliedGradeLevel1, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade1Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = yearRound, LocalCourseCode = suppliedCourseCode2, CourseTitle = suppliedCourseTitle2, SubjectArea = suppliedSubjectArea2, CreditsToBeEarned = suppliedCreditsToBeEarned2, Instructor = suppliedInstructor2, GradeLevel = suppliedGradeLevel2, GradingPeriod = finalGradePeriod, LetterGradeEarned = suppliedLetterGrade2Final  },
                               
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = gradingPeriod4, NumericGradeEarned = suppliedNumericGrade3Period5  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade3Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = gradingPeriod3, NumericGradeEarned = suppliedNumericGrade3Period4  },
                               
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode4, CourseTitle = suppliedCourseTitle4, SubjectArea = suppliedSubjectArea4, CreditsToBeEarned = suppliedCreditsToBeEarned4, Instructor = suppliedInstructor4, GradeLevel = suppliedGradeLevel4, GradingPeriod = gradingPeriod1, NumericGradeEarned = suppliedNumericGrade4Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode4, CourseTitle = suppliedCourseTitle4, SubjectArea = suppliedSubjectArea4, CreditsToBeEarned = suppliedCreditsToBeEarned4, Instructor = suppliedInstructor4, GradeLevel = suppliedGradeLevel4, GradingPeriod = gradingPeriod2, NumericGradeEarned = suppliedNumericGrade4Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode4, CourseTitle = suppliedCourseTitle4, SubjectArea = suppliedSubjectArea4, CreditsToBeEarned = suppliedCreditsToBeEarned4, Instructor = suppliedInstructor4, GradeLevel = suppliedGradeLevel4, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade4Final  },
                               
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode5, CourseTitle = suppliedCourseTitle5, SubjectArea = suppliedSubjectArea5, CreditsToBeEarned = suppliedCreditsToBeEarned5, Instructor = suppliedInstructor5, GradeLevel = suppliedGradeLevel5, GradingPeriod = gradingPeriod4, NumericGradeEarned = suppliedNumericGrade5Period5  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode5, CourseTitle = suppliedCourseTitle5, SubjectArea = suppliedSubjectArea5, CreditsToBeEarned = suppliedCreditsToBeEarned5, Instructor = suppliedInstructor5, GradeLevel = suppliedGradeLevel5, GradingPeriod = gradingPeriod3, NumericGradeEarned = suppliedNumericGrade5Period4  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 2, TermType = springSemester, LocalCourseCode = suppliedCourseCode5, CourseTitle = suppliedCourseTitle5, SubjectArea = suppliedSubjectArea5, CreditsToBeEarned = suppliedCreditsToBeEarned5, Instructor = suppliedInstructor5, GradeLevel = suppliedGradeLevel5, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade5Final  },
                               
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = gradingPeriod2, NumericGradeEarned = suppliedNumericGrade3Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade3Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 1, TermType = fallSemester, LocalCourseCode = suppliedCourseCode3, CourseTitle = suppliedCourseTitle3, SubjectArea = suppliedSubjectArea3, CreditsToBeEarned = suppliedCreditsToBeEarned3, Instructor = suppliedInstructor3, GradeLevel = suppliedGradeLevel3, GradingPeriod = gradingPeriod1, NumericGradeEarned = suppliedNumericGrade3Period1  },
                               
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 3, TermType = summerSemester, LocalCourseCode = suppliedCourseCode4, CourseTitle = suppliedCourseTitle4, SubjectArea = suppliedSubjectArea4, CreditsToBeEarned = suppliedCreditsToBeEarned4, Instructor = suppliedInstructor4, GradeLevel = suppliedGradeLevel4, GradingPeriod = gradingPeriod1, NumericGradeEarned = suppliedNumericGrade4Period1  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 3, TermType = summerSemester, LocalCourseCode = suppliedCourseCode4, CourseTitle = suppliedCourseTitle4, SubjectArea = suppliedSubjectArea4, CreditsToBeEarned = suppliedCreditsToBeEarned4, Instructor = suppliedInstructor4, GradeLevel = suppliedGradeLevel4, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade4Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 3, TermType = summerSemester, LocalCourseCode = suppliedCourseCode5, CourseTitle = suppliedCourseTitle5, SubjectArea = suppliedSubjectArea5, CreditsToBeEarned = suppliedCreditsToBeEarned5, Instructor = suppliedInstructor5, GradeLevel = suppliedGradeLevel5, GradingPeriod = gradingPeriod2, NumericGradeEarned = suppliedNumericGrade5Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 3, TermType = summerSemester, LocalCourseCode = suppliedCourseCode4, CourseTitle = suppliedCourseTitle4, SubjectArea = suppliedSubjectArea4, CreditsToBeEarned = suppliedCreditsToBeEarned4, Instructor = suppliedInstructor4, GradeLevel = suppliedGradeLevel4, GradingPeriod = gradingPeriod2, NumericGradeEarned = suppliedNumericGrade4Period2  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 3, TermType = summerSemester, LocalCourseCode = suppliedCourseCode5, CourseTitle = suppliedCourseTitle5, SubjectArea = suppliedSubjectArea5, CreditsToBeEarned = suppliedCreditsToBeEarned5, Instructor = suppliedInstructor5, GradeLevel = suppliedGradeLevel5, GradingPeriod = finalGradePeriod, NumericGradeEarned = suppliedNumericGrade5Final  },
                               new StudentRecordCurrentCourse { StudentUSI = student0, SchoolId = school0, TermTypeId = 3, TermType = summerSemester, LocalCourseCode = suppliedCourseCode5, CourseTitle = suppliedCourseTitle5, SubjectArea = suppliedSubjectArea5, CreditsToBeEarned = suppliedCreditsToBeEarned5, Instructor = suppliedInstructor5, GradeLevel = suppliedGradeLevel5, GradingPeriod = gradingPeriod1, NumericGradeEarned = suppliedNumericGrade5Period1  },
                               };
            return data.AsQueryable();
        }

        [Test]
        public void Should_have_semesters_built_correctly()
        {
            Assert.That(actualModel.Semesters.Count, Is.EqualTo(3));
            Assert.That(actualModel.Semesters[0].Term, Is.EqualTo(fallSemester));
            Assert.That(actualModel.Semesters[0].StudentUSI, Is.EqualTo(student0));
            Assert.That(actualModel.Semesters[0].AvailablePeriods.Count, Is.EqualTo(3));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[0], Is.EqualTo(GradingPeriod.One));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[1], Is.EqualTo(GradingPeriod.Two));
            Assert.That(actualModel.Semesters[0].AvailablePeriods[2], Is.EqualTo(GradingPeriod.FinalGrade));
            Assert.That(actualModel.Semesters[0].Courses.Count, Is.EqualTo(4));

            Assert.That(actualModel.Semesters[1].Term, Is.EqualTo(springSemester));
            Assert.That(actualModel.Semesters[1].StudentUSI, Is.EqualTo(student0));
            Assert.That(actualModel.Semesters[1].AvailablePeriods.Count, Is.EqualTo(3));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[0], Is.EqualTo(GradingPeriod.Three));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[1], Is.EqualTo(GradingPeriod.Four));
            Assert.That(actualModel.Semesters[1].AvailablePeriods[2], Is.EqualTo(GradingPeriod.FinalGrade));
            Assert.That(actualModel.Semesters[1].Courses.Count, Is.EqualTo(4));

            Assert.That(actualModel.Semesters[2].Term, Is.EqualTo(summerSemester));
            Assert.That(actualModel.Semesters[2].StudentUSI, Is.EqualTo(student0));
            Assert.That(actualModel.Semesters[2].AvailablePeriods.Count, Is.EqualTo(3));
            Assert.That(actualModel.Semesters[2].AvailablePeriods[0], Is.EqualTo(GradingPeriod.One));
            Assert.That(actualModel.Semesters[2].AvailablePeriods[1], Is.EqualTo(GradingPeriod.Two));
            Assert.That(actualModel.Semesters[2].AvailablePeriods[2], Is.EqualTo(GradingPeriod.FinalGrade));
            Assert.That(actualModel.Semesters[2].Courses.Count, Is.EqualTo(2));
        }

        [Test]
        public void Should_have_courses_built_and_bound_correctly()
        {
            var course = actualModel.Semesters[0].Courses[0];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode1));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle1));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea1));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor1));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned1));

            course = actualModel.Semesters[0].Courses[1];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode2));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle2));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea2));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor2));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel2));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned2));

            course = actualModel.Semesters[0].Courses[2];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode3));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle3));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea3));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor3));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel3));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned3));

            course = actualModel.Semesters[0].Courses[3];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode4));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle4));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea4));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor4));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel4));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned4));

            course = actualModel.Semesters[1].Courses[0];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode1));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle1));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea1));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor1));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel1));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned1));

            course = actualModel.Semesters[1].Courses[1];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode2));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle2));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea2));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor2));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel2));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned2));

            course = actualModel.Semesters[1].Courses[2];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode3));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle3));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea3));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor3));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel3));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned3));

            course = actualModel.Semesters[1].Courses[3];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode5));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle5));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea5));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor5));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel5));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned5));

            course = actualModel.Semesters[2].Courses[0];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode4));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle4));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea4));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor4));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel4));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned4));

            course = actualModel.Semesters[2].Courses[1];
            Assert.That(course.StudentUSI, Is.EqualTo(student0));
            Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourseCode5));
            Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourseTitle5));
            Assert.That(course.SubjectArea, Is.EqualTo(suppliedSubjectArea5));
            Assert.That(course.Instructor, Is.EqualTo(suppliedInstructor5));
            Assert.That(course.GradeLevel, Is.EqualTo(suppliedGradeLevel5));
            Assert.That(course.CreditsToBeEarned, Is.EqualTo(suppliedCreditsToBeEarned5));
        }

        [Test]
        public void Should_have_grades_built_and_bound_correctly()
        {
            Assert.That(actualModel.Semesters[0].Courses[0].Grades.Count, Is.EqualTo(3));

            var grade = actualModel.Semesters[0].Courses[0].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period1));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[0].Courses[0].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period2));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[0].Courses[0].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[0].Courses[1].Grades.Count, Is.EqualTo(3));

            grade = actualModel.Semesters[0].Courses[1].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period1.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[0].Courses[1].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period2.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[0].Courses[1].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.Null);
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));

            Assert.That(actualModel.Semesters[0].Courses[2].Grades.Count, Is.EqualTo(3));

            grade = actualModel.Semesters[0].Courses[2].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Period1.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[0].Courses[2].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Period2.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[0].Courses[2].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[0].Courses[3].Grades.Count, Is.EqualTo(3));

            grade = actualModel.Semesters[0].Courses[3].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade4Period1.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[0].Courses[3].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade4Period2.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[0].Courses[3].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade4Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));

            Assert.That(actualModel.Semesters[1].Courses[0].Grades.Count, Is.EqualTo(3));

            grade = actualModel.Semesters[1].Courses[0].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period4));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));

            grade = actualModel.Semesters[1].Courses[0].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade1Period5));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Four)); 

            grade = actualModel.Semesters[1].Courses[0].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade1Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[1].Courses[1].Grades.Count, Is.EqualTo(3));

            grade = actualModel.Semesters[1].Courses[1].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period4.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));

            grade = actualModel.Semesters[1].Courses[1].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade2Period5.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Four));

            grade = actualModel.Semesters[1].Courses[1].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedLetterGrade2Final));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[1].Courses[2].Grades.Count, Is.EqualTo(3));

            grade = actualModel.Semesters[1].Courses[2].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Period4.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));

            grade = actualModel.Semesters[1].Courses[2].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Period5.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Four));

            grade = actualModel.Semesters[1].Courses[2].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade3Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[1].Courses[3].Grades.Count, Is.EqualTo(3));

            grade = actualModel.Semesters[1].Courses[3].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade5Period4.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Three));

            grade = actualModel.Semesters[1].Courses[3].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade5Period5.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Four));

            grade = actualModel.Semesters[1].Courses[3].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade5Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[2].Courses[1].Grades.Count, Is.EqualTo(3));

            grade = actualModel.Semesters[2].Courses[0].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade4Period1.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[2].Courses[0].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade4Period2.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[2].Courses[0].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade4Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));


            Assert.That(actualModel.Semesters[2].Courses[1].Grades.Count, Is.EqualTo(3));

            grade = actualModel.Semesters[2].Courses[1].Grades[0];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade5Period1.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.One));

            grade = actualModel.Semesters[2].Courses[1].Grades[1];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade5Period2.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.Two));

            grade = actualModel.Semesters[2].Courses[1].Grades[2];
            Assert.That(grade.StudentUSI, Is.EqualTo(student0));
            Assert.That(grade.Value, Is.EqualTo(suppliedNumericGrade5Final.ToString()));
            Assert.That(grade.GradePeriod, Is.EqualTo(GradingPeriod.FinalGrade));
        }
        
        [Test]
        public override void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel.EnsureNoDefaultValues("CurrentCoursesModel.Semesters[0].Courses[0].Grades[2].Value");
        }
    }

    public class When_getting_current_courses_with_a_nine_week_grading_period_form_the_service : When_getting_current_courses_with_a_nine_week_grading_period<CurrentCoursesListRequest, CurrentCoursesModel, CurrentCoursesListService, Semester, Course, Grade>
    {

    }
}
