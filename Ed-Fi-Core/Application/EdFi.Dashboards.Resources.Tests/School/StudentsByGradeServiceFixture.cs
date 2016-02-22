// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.School
{
    [TestFixture]
    public class When_loading_students_by_grade_list : TestFixtureBase
    {
        //The Injected Dependencies.
        private IRepository<StudentInformation> studentInformationRepository;
        private IRepository<StudentSchoolInformation> studentSchoolInformationRepository;
        private IUniqueListIdProvider uniqueListIdProvider;
        protected IRootMetricNodeResolver rootMetricNodeResolver;
        private IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;

        //The Actual Model.
        protected StudentsByGradeModel actualModel;

        //The supplied Data models.
        protected const int suppliedSchoolId1 = 1;
        protected const int suppliedMetricId = 8;
        private IQueryable<StudentInformation> suppliedStudentInformationData;
        private IQueryable<StudentSchoolInformation> suppliedStudentSchoolInformationData;
        private string suppliedUniqueListIdProvider = "studentlist1";
        private StudentSchoolAreaLinksFake studentSchoolAreaLinksFake = new StudentSchoolAreaLinksFake();

        protected override void EstablishContext()
        {
            //Prepare supplied data collections            
            suppliedStudentInformationData = GetSuppliedStudentInformation();
            suppliedStudentSchoolInformationData = GetSuppliedStudentSchoolInformation();

            //Set up the mocks
            studentInformationRepository = mocks.StrictMock<IRepository<StudentInformation>>();
            studentSchoolInformationRepository = mocks.StrictMock<IRepository<StudentSchoolInformation>>();
            uniqueListIdProvider = mocks.StrictMock<IUniqueListIdProvider>();
            rootMetricNodeResolver = mocks.StrictMock<IRootMetricNodeResolver>();
            gradeLevelUtilitiesProvider = mocks.StrictMock<IGradeLevelUtilitiesProvider>();

            //Set expectations
            Expect.Call(studentInformationRepository.GetAll()).Return(suppliedStudentInformationData);
            Expect.Call(studentSchoolInformationRepository.GetAll()).Return(suppliedStudentSchoolInformationData);
            Expect.Call(uniqueListIdProvider.GetUniqueId()).Return(suppliedUniqueListIdProvider);
            Expect.Call(rootMetricNodeResolver.GetRootMetricNodeForStudent(suppliedSchoolId1)).Repeat.Any().Return(GetStudentRootOverviewNode());
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForSorting("1")).IgnoreArguments().Repeat.AtLeastOnce().Return(1);

        }

        private IQueryable<StudentInformation> GetSuppliedStudentInformation()
        {
            return (new List<StudentInformation>
                        {
                            new StudentInformation{StudentUSI = 1, FirstName = "John", MiddleName = "T", LastSurname = "Doe", FullName = "John T DoeX"},
                            new StudentInformation{StudentUSI = 2, FirstName = "Mary", MiddleName = "K", LastSurname = "Ferguson", FullName = "Mary K FergusonX"},
                            new StudentInformation{StudentUSI = 3, FirstName = "Xavier", MiddleName = "", LastSurname = "Adams", FullName = "Xavier AdamsX"},
                            new StudentInformation{StudentUSI = 4, FirstName = "Stacey", MiddleName = "W", LastSurname = "Martins", FullName = "Stacey W MartinsX"},
                            new StudentInformation{StudentUSI = 5, FirstName = "Anthony", MiddleName = "", LastSurname = "Adams", FullName = "Anthony AdamsX"},
                            new StudentInformation{StudentUSI = 9999}, //Will be filtered out because he belongs to a different School...
                        }).AsQueryable();
        }

        private IQueryable<StudentSchoolInformation> GetSuppliedStudentSchoolInformation()
        {
            return (new List<StudentSchoolInformation>
                        {
                            new StudentSchoolInformation{StudentUSI = 1, SchoolId = suppliedSchoolId1, GradeLevel = "3rd Grade"},
                            new StudentSchoolInformation{StudentUSI = 2, SchoolId = suppliedSchoolId1, GradeLevel = "1st Grade"},
                            new StudentSchoolInformation{StudentUSI = 3, SchoolId = suppliedSchoolId1, GradeLevel = "1st Grade"},
                            new StudentSchoolInformation{StudentUSI = 4, SchoolId = suppliedSchoolId1, GradeLevel = "2nd Grade"},
                            new StudentSchoolInformation{StudentUSI = 5, SchoolId = suppliedSchoolId1, GradeLevel = "1st Grade"},
                            new StudentSchoolInformation{StudentUSI = 9999, SchoolId = 9999}, //Will be filtered out because he belongs to a different School...
                        }).AsQueryable();
        }

        private MetricMetadataNode GetStudentRootOverviewNode()
        {
            var tree = new TestMetricMetadataTree();

            return new MetricMetadataNode(tree)
            {
                MetricId = suppliedMetricId,
                Name = "Student's Overview",
                MetricNodeId = 7,
                Children = new List<MetricMetadataNode>
                            {
                                new MetricMetadataNode(tree){MetricId=21, MetricNodeId = 71, Name = "Student's Attendance and Discipline",
                                Children = new List<MetricMetadataNode>
                                                {
                                                    new MetricMetadataNode(tree){MetricId=211, MetricNodeId = 711, Name = "Attendance"},
                                                    new MetricMetadataNode(tree){MetricId=212, Name = "Discipline"} 
                                                }
                                },
                                new MetricMetadataNode(tree){MetricId=22, MetricNodeId = 72, Name = "School's Other Metric"},
                            }
            };
        }

        protected override void ExecuteTest()
        {
            var service = new StudentsByGradeService(studentInformationRepository, studentSchoolInformationRepository, uniqueListIdProvider, studentSchoolAreaLinksFake, gradeLevelUtilitiesProvider);
            actualModel = service.Get(StudentsByGradeRequest.Create(suppliedSchoolId1));
        }

        [Test]
        public void Should_return_model_that_is_not_null()
        {
            Assert.That(actualModel, Is.Not.Null);
        }

        [Test]
        public void Should_return_model_with_schoolId_bound()
        {
            Assert.That(actualModel.SchoolId, Is.EqualTo(suppliedSchoolId1));
        }

        [Test]
        public void Should_return_model_with_data_bound_correctly()
        {
            var data = from s in suppliedStudentInformationData
                       join ssi in suppliedStudentSchoolInformationData on s.StudentUSI equals ssi.StudentUSI
                       where ssi.SchoolId == suppliedSchoolId1
                       select new
                       {
                           s.StudentUSI,
                           s.FirstName,
                           s.MiddleName,
                           s.LastSurname,
                           ssi.GradeLevel,
                           s.FullName,
                       };

            var dataGroupedAndOrderedByGrade = (from s in data
                                                 group s by s.GradeLevel
                                                     into g
                                                     select new
                                                                {
                                                                    gradeLevel = g.Key,
                                                                    students = from s in g
                                                                                 orderby s.LastSurname, s.FirstName
                                                                                 select new
                                                                                            {
                                                                                                s.StudentUSI,
                                                                                                fullName = (s.LastSurname + ", " + s.FirstName + ((String.IsNullOrEmpty(s.MiddleName)) ? "" : " " + s.MiddleName + ".")),
                                                                                                fullNameForHref = s.FullName,
                                                                                            }
                                                                }).OrderBy(x => x.gradeLevel, new StudentsByGradeService.SchoolGradeComparer(gradeLevelUtilitiesProvider));


            //Should have right number of grades
            Assert.That(actualModel.Grades.Count(), Is.EqualTo(dataGroupedAndOrderedByGrade.Count()));

            //Should have the data bound correctly.
            var i = 0;
            foreach (var suppliedGrade in dataGroupedAndOrderedByGrade)
            {

                Assert.That(actualModel.Grades.ElementAt(i).GradeLevel,Is.EqualTo(suppliedGrade.gradeLevel), "Grade Level Text does not Match");
                Assert.That(actualModel.Grades.ElementAt(i).Students.Count(), Is.EqualTo(suppliedGrade.students.Count()), "Student count in this GradeLevel do not match.");

                //Lets test 2 things here. The ordering of students and the data bound.
                var j = 0;
                foreach (var suppliedStudent in suppliedGrade.students)
                {
                    Assert.That(actualModel.Grades.ElementAt(i).Students.ElementAt(j).StudentUSI, Is.EqualTo(suppliedStudent.StudentUSI));
                    Assert.That(actualModel.Grades.ElementAt(i).Students.ElementAt(j).FullName, Is.EqualTo(suppliedStudent.fullName));
                    Assert.That(actualModel.Grades.ElementAt(i).Students.ElementAt(j).Url, Is.EqualTo(studentSchoolAreaLinksFake.Overview(suppliedSchoolId1, suppliedStudent.StudentUSI, suppliedStudent.fullNameForHref, new { listContext = suppliedUniqueListIdProvider + suppliedSchoolId1 }))); // suppliedMetricId, 

                    j++;
                }

                i++;
            }
        }

        [Test]
        public void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel.EnsureNoDefaultValues(
                "StudentsByGradeModel.Grades[0].Students[0].Links",
                "StudentsByGradeModel.Grades[0].Students[0].ResourceUrl",
                "StudentsByGradeModel.Grades[0].Students[1].ResourceUrl",
                "StudentsByGradeModel.Grades[0].Students[2].ResourceUrl");
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }
}
