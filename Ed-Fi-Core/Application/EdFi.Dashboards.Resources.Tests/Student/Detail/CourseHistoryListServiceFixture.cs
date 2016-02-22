// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Resources.Models.Student.Detail.CourseHistory;
using EdFi.Dashboards.Resources.StudentSchool.Detail;
using NUnit.Framework;
using Rhino.Mocks;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Testing;

namespace EdFi.Dashboards.Resources.Tests.Student.Detail
{
    [TestFixture]
    public abstract class When_getting_course_history<TRequest, TResponse, TService, TSubjectArea, TCourse, TSemester, TGrade> : TestFixtureBase
        where TRequest : CourseHistoryListRequest, new()
        where TResponse : CourseHistoryModel, new()
        where TSubjectArea : SubjectArea, new()
        where TSemester : Semester, new()
        where TCourse : Course, new()
        where TGrade : Grade, new()
        where TService : CourseHistoryListServiceBase<TRequest, TResponse, TSubjectArea, TCourse, TSemester, TGrade>, new()
    {
        private IRepository<StudentRecordCourseHistory> repository;

        private TResponse actualModel;

        private const int suppliedStudentUSI = 1;
        private IQueryable<StudentRecordCourseHistory> suppliedData;

        protected override void EstablishContext()
        {
            suppliedData = GetSuppliedData();

            repository = mocks.StrictMock<IRepository<StudentRecordCourseHistory>>();
            Expect.Call(repository.GetAll()).Return(suppliedData);
        }

        protected IQueryable<StudentRecordCourseHistory> GetSuppliedData()
        {
            var l = new List<StudentRecordCourseHistory>
			       	{
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = "Social Studies", Instructor = "J", LocalCourseCode = "MALR11", CourseTitle = "Algebra I (1 Unit)", TermType = "1", GradeLevel = "12", SchoolYear=2010, FinalNumericGradeEarned = 34, CreditsEarned = Convert.ToDecimal(0.5)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = "Social Studies", Instructor = "K", LocalCourseCode = "MALR11", CourseTitle = "Algebra I (1 Unit)", TermType = "1", GradeLevel = "12", SchoolYear=2010, FinalNumericGradeEarned = 65, CreditsEarned = Convert.ToDecimal(0.42)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = "Social Studies", Instructor = "L", LocalCourseCode = "MALR11", CourseTitle = "Algebra I (1 Unit)", TermType = "1", GradeLevel = "12", SchoolYear=2010, FinalNumericGradeEarned = 33, CreditsEarned = Convert.ToDecimal(0.511)},

			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = "All Other Subjects", Instructor = "M", LocalCourseCode = "MALR11", CourseTitle = "Algebra I (1 Unit)", TermType = "Spring", GradeLevel = "12", SchoolYear=2010, FinalNumericGradeEarned = 77, CreditsEarned = Convert.ToDecimal(0.5)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = "All Other Subjects", Instructor = "N", LocalCourseCode = "MALR11", CourseTitle = "BBBBBBB", TermType = "Spring", GradeLevel = "12", SchoolYear=2010, FinalNumericGradeEarned = 77, CreditsEarned = Convert.ToDecimal(0.5)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = "All Other Subjects", Instructor = "O", LocalCourseCode = "MALR11", CourseTitle = "Algebra I (1 Unit)", TermType = "Spring", GradeLevel = "12", SchoolYear=2010, FinalNumericGradeEarned = 77, CreditsEarned = Convert.ToDecimal(0.54)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI+2, SubjectArea = "All Other Subjects", Instructor = "Ox", LocalCourseCode = "MALR11", CourseTitle = "Algebra I (1 Unit)", TermType = "Spring", GradeLevel = "12", SchoolYear=2010, FinalNumericGradeEarned = 77, CreditsEarned = Convert.ToDecimal(0.54)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = "All Other Subjects", Instructor = "P", LocalCourseCode = "MALR11", CourseTitle = "Algebra I (1 Unit)", TermType = "Fall", GradeLevel = "09", SchoolYear=2007, FinalNumericGradeEarned = 77, CreditsEarned = Convert.ToDecimal(0.5)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI+3, SubjectArea = "All Other Subjects", Instructor = "Paper", LocalCourseCode = "MALR11", CourseTitle = "velocity", TermType = "Fall", GradeLevel = "09", SchoolYear=2007, FinalNumericGradeEarned = 77, CreditsEarned = Convert.ToDecimal(0.5)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = "All Other Subjects", Instructor = "Q", LocalCourseCode = "MALR11", CourseTitle = "Algebra I (1 Unit)", TermType = "Spring", GradeLevel = "11", SchoolYear=2009, FinalNumericGradeEarned = 77, CreditsEarned = Convert.ToDecimal(0.5)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = "All Other Subjects", Instructor = "R", LocalCourseCode = "MALR11", CourseTitle = "Algebra I (1 Unit)", TermType = "Fall", GradeLevel = "12", SchoolYear=2010, FinalNumericGradeEarned = 77, CreditsEarned = Convert.ToDecimal(1.7)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = "All Other Subjects", Instructor = "S", LocalCourseCode = "MALR11", CourseTitle = "Algebra I (1 Unit)", TermType = "Fall", GradeLevel = "11", SchoolYear=2009, FinalNumericGradeEarned = 77, CreditsEarned = Convert.ToDecimal(0.5)},
			       	
                        new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = "ELA/Reading", Instructor = "AAA", LocalCourseCode = "LENR11", CourseTitle = "English I (1 Unit)", TermType = "1", GradeLevel = "12", SchoolYear=2010, FinalLetterGradeEarned = "B", CreditsEarned = Convert.ToDecimal(0.5)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = "ELA/Reading", Instructor = "BBB", LocalCourseCode = "LENR11", CourseTitle = "English I (1 Unit)", TermType = "2", GradeLevel = "12", SchoolYear=2010, FinalLetterGradeEarned = "C+", CreditsEarned = Convert.ToDecimal(0.88)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = "ELA/Reading", Instructor = "CCC", LocalCourseCode = "LENR11", CourseTitle = "English I (1 Unit)", TermType = "3", GradeLevel = "12", SchoolYear=2010, FinalLetterGradeEarned = "A-", CreditsEarned = Convert.ToDecimal(0.5)},

			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = "Mathematics", Instructor = "DDD", LocalCourseCode = "MALR11", CourseTitle = "Algebra I (1 Unit)", TermType = "1", GradeLevel = "12", SchoolYear=2010, FinalNumericGradeEarned = 77, CreditsEarned = Convert.ToDecimal(0.5)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = "Mathematics", Instructor = "EEE", LocalCourseCode = "MALR11", CourseTitle = "Algebra I (1 Unit)", TermType = "2", GradeLevel = "12", SchoolYear=2010, FinalNumericGradeEarned = 77, CreditsEarned = Convert.ToDecimal(0.01)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = "Mathematics", Instructor = "FFF", LocalCourseCode = "MGER21", CourseTitle = "Geometry I (1 Unit)", TermType = "3", GradeLevel = "12", SchoolYear=2010, FinalNumericGradeEarned = 77, CreditsEarned = Convert.ToDecimal(0.5)},

			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = "Science", Instructor = "GGG", LocalCourseCode = "MALR11", CourseTitle = "Algebra I (1 Unit)", TermType = "1", GradeLevel = "12", SchoolYear=2010, FinalNumericGradeEarned = 77, CreditsEarned = Convert.ToDecimal(0.5)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = "Science", Instructor = "HHH", LocalCourseCode = "MALR11", CourseTitle = "Algebra I (1 Unit)", TermType = "1", GradeLevel = "12", SchoolYear=2010, FinalNumericGradeEarned = 77, CreditsEarned = Convert.ToDecimal(0.3)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = "Science", Instructor = "III", LocalCourseCode = "MALR11", CourseTitle = "Algebra I (1 Unit)", TermType = "1", GradeLevel = "12", SchoolYear=2010, FinalNumericGradeEarned = 77, CreditsEarned = Convert.ToDecimal(0.3)},

			       	};
            return l.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new TService{ Repository = repository};

            var request = new TRequest
                              {
                                  StudentUSI = suppliedStudentUSI
                              };

            actualModel = service.Get(request);
        }

        [Test]
        public void Should_contain_data()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.SubjectAreas, Is.Not.Null);
        }

        [Test]
        public void Should_create_correct_number_of_subject_areas()
        {
            var subjectAreas = from rows in suppliedData
                               group rows by rows.SubjectArea
                                   into s
                                   select new { Name = s.Key };


            Assert.That(actualModel.SubjectAreas.Count, Is.EqualTo(subjectAreas.Count()));
        }

        [Test]
        public void Should_sort_subject_areas_correctly()
        {
            var comparer = new SubjectAreaComparer();

            string previousSubjectArea = String.Empty;
            foreach (var subjectArea in actualModel.SubjectAreas)
            {
                Assert.That(comparer.Compare(previousSubjectArea, subjectArea.Name), Is.EqualTo(-1), String.Format("{0} {1}", previousSubjectArea, subjectArea.Name));
                previousSubjectArea = subjectArea.Name;
            }
        }
    
        [Test]
        public void Should_create_correct_number_of_courses_for_each_subject_area()
        {
            foreach (var subjectArea in actualModel.SubjectAreas)
            {
                var area = subjectArea;
                var suppliedCourses = from rows in suppliedData
                                           where rows.StudentUSI == suppliedStudentUSI && rows.SubjectArea == area.Name
                                           select rows;

                Assert.That(subjectArea.Courses.Count, Is.EqualTo(suppliedCourses.Count()));
            }
        }

        [Test]
        public void Should_sort_courses_correctly()
        {
            foreach (var subjectArea in actualModel.SubjectAreas)
            {
                string previousGradeLevel = String.Empty;
                string previousActualSemester = String.Empty;
                string previousCourseTitle = String.Empty;

                foreach (var course in subjectArea.Courses)
                {
                    Assert.That(course.GradeLevel, Is.GreaterThanOrEqualTo(previousGradeLevel));
                    if (course.GradeLevel == previousGradeLevel)
                    {
                        Assert.That(course.ActualSemester.TermType, Is.GreaterThanOrEqualTo(previousActualSemester));
                        if (course.ActualSemester.TermType == previousActualSemester)
                        {
                            Assert.That(course.CourseTitle, Is.GreaterThanOrEqualTo(previousCourseTitle));
                            previousCourseTitle = course.CourseTitle;
                        }
                        else
                        {
                            previousCourseTitle = String.Empty;
                        }
                    }
                    else
                    {
                        previousActualSemester = String.Empty;
                    }
                    previousGradeLevel = course.GradeLevel;
                }

            }

        }

        [Test]
        public void Should_bind_data_correctly()
        {
            foreach (var subjectArea in actualModel.SubjectAreas)
            {
                foreach (var course in subjectArea.Courses)
                {
                    var course1 = course;
                    var suppliedCourse = suppliedData.SingleOrDefault(x => x.Instructor == course1.Instructor);
                    Assert.IsNotNull(suppliedCourse);
                    Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourse.LocalCourseCode));
                    Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourse.CourseTitle));
                    Assert.That(course.Instructor, Is.EqualTo(suppliedCourse.Instructor));
                    Assert.That(course.ActualSemester.TermType, Is.EqualTo(suppliedCourse.TermType));
                    Assert.That(course.GradeLevel, Is.EqualTo(suppliedCourse.GradeLevel));
                    Assert.That(course.CreditsEarned, Is.EqualTo(suppliedCourse.CreditsEarned));

                    Assert.That(course.FinalGrade.Value, suppliedCourse.FinalNumericGradeEarned != null ? Is.EqualTo(suppliedCourse.FinalNumericGradeEarned.ToString()) : Is.EqualTo(suppliedCourse.FinalLetterGradeEarned));
                }
            }
        }

        [Test]
        public void Should_calculate_total_credits_earned_per_subject_area_correctly()
        {
            foreach (var subjectArea in actualModel.SubjectAreas)
            {
                var area = subjectArea;
                var suppliedCourses = from rows in suppliedData
                                      where rows.SubjectArea == area.Name && rows.StudentUSI == suppliedStudentUSI
                                      select rows;

                decimal? suppliedTotalCreditsEarned = suppliedCourses.Sum(x => x.CreditsEarned);

                Assert.That(subjectArea.TotalCreditsEarned, Is.EqualTo(suppliedTotalCreditsEarned));
            }
        }

        [Test]
        public void Should_calculate_cumulative_credits_earned_correctly()
        {
            var suppliedCourses = from rows in suppliedData
                                  where rows.StudentUSI == suppliedStudentUSI
                                  select rows;

            decimal? suppliedCumulativeCreditsEarned = suppliedCourses.Sum(x => x.CreditsEarned);
            Assert.That(actualModel.CumulativeCreditsEarned, Is.EqualTo(suppliedCumulativeCreditsEarned));
        }

        [Test]
        public void Should_have_no_unassigned_values_on_presentation_model()
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

    public class When_getting_course_history_from_the_service : When_getting_course_history<CourseHistoryListRequest, CourseHistoryModel, CourseHistoryListListService, SubjectArea, Course, Semester, Grade>
    { }

    [TestFixture]
    public abstract class When_getting_course_history_by_subject_area<TRequest, TResponse, TService, TSubjectArea, TCourse, TSemester, TGrade> : TestFixtureBase
        where TRequest : CourseHistoryListRequest, new()
        where TResponse : CourseHistoryModel, new()
        where TSubjectArea : SubjectArea, new()
        where TSemester : Semester, new()
        where TCourse : Course, new()
        where TGrade : Grade, new()
        where TService : CourseHistoryListServiceBase<TRequest, TResponse, TSubjectArea, TCourse, TSemester, TGrade>, new()
    {
        private IRepository<StudentRecordCourseHistory> repository;

        private TResponse actualModel;

        private const int suppliedStudentUSI = 1;
        private const string suppliedSubjectArea = "All Other Subjects";
        private IQueryable<StudentRecordCourseHistory> suppliedData;

        protected override void EstablishContext()
        {
            suppliedData = GetSuppliedData();

            repository = mocks.StrictMock<IRepository<StudentRecordCourseHistory>>();
            Expect.Call(repository.GetAll()).Return(suppliedData);
        }

        protected IQueryable<StudentRecordCourseHistory> GetSuppliedData()
        {
            var l = new List<StudentRecordCourseHistory>
			       	{
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = "Social Studies", Instructor = "J", LocalCourseCode = "MALR11", CourseTitle = "Algebra I (1 Unit)", TermType = "1", GradeLevel = "12", SchoolYear=2010, FinalNumericGradeEarned = 34, CreditsEarned = Convert.ToDecimal(0.5)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = "Social Studies", Instructor = "K", LocalCourseCode = "MALR11", CourseTitle = "Algebra I (1 Unit)", TermType = "1", GradeLevel = "12", SchoolYear=2010, FinalNumericGradeEarned = 65, CreditsEarned = Convert.ToDecimal(0.42)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = "Social Studies", Instructor = "L", LocalCourseCode = "MALR11", CourseTitle = "Algebra I (1 Unit)", TermType = "1", GradeLevel = "12", SchoolYear=2010, FinalNumericGradeEarned = 33, CreditsEarned = Convert.ToDecimal(0.511)},

			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = suppliedSubjectArea, Instructor = "M", LocalCourseCode = "MALR11", CourseTitle = "Algebra I (1 Unit)", TermType = "Spring", GradeLevel = "12", SchoolYear=2010, FinalNumericGradeEarned = 77, CreditsEarned = Convert.ToDecimal(0.5)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = suppliedSubjectArea, Instructor = "N", LocalCourseCode = "MALR11", CourseTitle = "BBBBBBB", TermType = "Spring", GradeLevel = "12", SchoolYear=2010, FinalNumericGradeEarned = 77, CreditsEarned = Convert.ToDecimal(0.5)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = suppliedSubjectArea, Instructor = "O", LocalCourseCode = "MALR11", CourseTitle = "Algebra I (1 Unit)", TermType = "Spring", GradeLevel = "12", SchoolYear=2010, FinalNumericGradeEarned = 77, CreditsEarned = Convert.ToDecimal(0.54)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI+2, SubjectArea = suppliedSubjectArea, Instructor = "Ox", LocalCourseCode = "MALR11", CourseTitle = "Algebra I (1 Unit)", TermType = "Spring", GradeLevel = "12", SchoolYear=2010, FinalNumericGradeEarned = 77, CreditsEarned = Convert.ToDecimal(0.54)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = suppliedSubjectArea, Instructor = "P", LocalCourseCode = "MALR11", CourseTitle = "Algebra I (1 Unit)", TermType = "Fall", GradeLevel = "09", SchoolYear=2007, FinalNumericGradeEarned = 77, CreditsEarned = Convert.ToDecimal(0.5)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI+3, SubjectArea = suppliedSubjectArea, Instructor = "Paper", LocalCourseCode = "MALR11", CourseTitle = "velocity", TermType = "Fall", GradeLevel = "09", SchoolYear=2007, FinalNumericGradeEarned = 77, CreditsEarned = Convert.ToDecimal(0.5)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = suppliedSubjectArea, Instructor = "Q", LocalCourseCode = "MALR11", CourseTitle = "Algebra I (1 Unit)", TermType = "Spring", GradeLevel = "11", SchoolYear=2009, FinalNumericGradeEarned = 77, CreditsEarned = Convert.ToDecimal(0.5)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = suppliedSubjectArea, Instructor = "R", LocalCourseCode = "MALR11", CourseTitle = "Algebra I (1 Unit)", TermType = "Fall", GradeLevel = "12", SchoolYear=2010, FinalNumericGradeEarned = 77, CreditsEarned = Convert.ToDecimal(1.7)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = suppliedSubjectArea, Instructor = "S", LocalCourseCode = "MALR11", CourseTitle = "Algebra I (1 Unit)", TermType = "Fall", GradeLevel = "11", SchoolYear=2009, FinalNumericGradeEarned = 77, CreditsEarned = Convert.ToDecimal(0.5)},
			       	
                        new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = "ELA/Reading", Instructor = "AAA", LocalCourseCode = "LENR11", CourseTitle = "English I (1 Unit)", TermType = "1", GradeLevel = "12", SchoolYear=2010, FinalLetterGradeEarned = "B", CreditsEarned = Convert.ToDecimal(0.5)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = "ELA/Reading", Instructor = "BBB", LocalCourseCode = "LENR11", CourseTitle = "English I (1 Unit)", TermType = "2", GradeLevel = "12", SchoolYear=2010, FinalLetterGradeEarned = "C+", CreditsEarned = Convert.ToDecimal(0.88)},
			       		new StudentRecordCourseHistory{StudentUSI = suppliedStudentUSI, SubjectArea = "ELA/Reading", Instructor = "CCC", LocalCourseCode = "LENR11", CourseTitle = "English I (1 Unit)", TermType = "3", GradeLevel = "12", SchoolYear=2010, FinalLetterGradeEarned = "A-", CreditsEarned = Convert.ToDecimal(0.5)},

			       	};
            return l.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new TService{ Repository = repository };
            
            var request = new TRequest
                              {
                                  StudentUSI = suppliedStudentUSI,
                                  SubjectArea = suppliedSubjectArea
                              };

            actualModel = service.Get(request);
        }

        [Test]
        public void Should_contain_data()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.SubjectAreas, Is.Not.Null);
            Assert.That(actualModel.SubjectAreas.Count, Is.EqualTo(1));
            Assert.That(actualModel.SubjectAreas[0].Name, Is.EqualTo(suppliedSubjectArea));
        }

        [Test]
        public void Should_create_correct_number_of_courses_for_each_subject_area()
        {
                var suppliedCourses = from rows in suppliedData
                                           where rows.StudentUSI == suppliedStudentUSI && rows.SubjectArea == suppliedSubjectArea
                                           select rows;

                Assert.That(actualModel.SubjectAreas[0].Courses.Count, Is.EqualTo(suppliedCourses.Count()));
        }

        [Test]
        public void Should_sort_courses_correctly()
        {
            string previousGradeLevel = String.Empty;
            string previousActualSemester = String.Empty;
            string previousCourseTitle = String.Empty;

            foreach (var course in actualModel.SubjectAreas[0].Courses)
            {
                Assert.That(course.GradeLevel, Is.GreaterThanOrEqualTo(previousGradeLevel));
                if (course.GradeLevel == previousGradeLevel)
                {
                    Assert.That(course.ActualSemester.TermType, Is.GreaterThanOrEqualTo(previousActualSemester));
                    if (course.ActualSemester.TermType == previousActualSemester)
                    {
                        Assert.That(course.CourseTitle, Is.GreaterThanOrEqualTo(previousCourseTitle));
                        previousCourseTitle = course.CourseTitle;
                    }
                    else
                    {
                        previousCourseTitle = String.Empty;
                    }
                }
                else
                {
                    previousActualSemester = String.Empty;
                }
                previousGradeLevel = course.GradeLevel;
            }

        }

        [Test]
        public void Should_bind_data_correctly()
        {
            foreach (var course in actualModel.SubjectAreas[0].Courses)
            {
                var course1 = course;
                var suppliedCourse = suppliedData.SingleOrDefault(x => x.Instructor == course1.Instructor);
                Assert.IsNotNull(suppliedCourse);
                Assert.That(course.LocalCourseCode, Is.EqualTo(suppliedCourse.LocalCourseCode));
                Assert.That(course.CourseTitle, Is.EqualTo(suppliedCourse.CourseTitle));
                Assert.That(course.Instructor, Is.EqualTo(suppliedCourse.Instructor));
                Assert.That(course.ActualSemester.TermType, Is.EqualTo(suppliedCourse.TermType));
                Assert.That(course.GradeLevel, Is.EqualTo(suppliedCourse.GradeLevel));
                Assert.That(course.CreditsEarned, Is.EqualTo(suppliedCourse.CreditsEarned));

                Assert.That(course.FinalGrade.Value, suppliedCourse.FinalNumericGradeEarned != null ? Is.EqualTo(suppliedCourse.FinalNumericGradeEarned.ToString()) : Is.EqualTo(suppliedCourse.FinalLetterGradeEarned));
            }
        }

        [Test]
        public void Should_calculate_total_credits_earned_per_subject_area_correctly()
        {
                var suppliedCourses = from rows in suppliedData
                                      where rows.SubjectArea == suppliedSubjectArea && rows.StudentUSI == suppliedStudentUSI
                                      select rows;

                decimal? suppliedTotalCreditsEarned = suppliedCourses.Sum(x => x.CreditsEarned);

                Assert.That(actualModel.SubjectAreas[0].TotalCreditsEarned, Is.EqualTo(suppliedTotalCreditsEarned));
        }

        [Test]
        public void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel.EnsureNoDefaultValues("CourseHistoryModel.CumulativeCreditsEarned");
        }
    }

    public class When_getting_course_history_by_subject_area_from_the_service : When_getting_course_history_by_subject_area<CourseHistoryListRequest, CourseHistoryModel, CourseHistoryListListService, SubjectArea, Course, Semester, Grade>
    { }
}
