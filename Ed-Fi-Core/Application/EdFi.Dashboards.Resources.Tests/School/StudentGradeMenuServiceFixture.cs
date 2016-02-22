using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.Common;
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
    public class When_getting_student_grade_menu : TestFixtureBase
    {
        protected IRepository<StudentSchoolInformation> studentSchoolInformationRepository;
        protected ISchoolAreaLinks schoolAreaLinks;
        protected IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;

        private const int suppliedSchoolId = 1000;
        private const string gradeInfant = "Infant/toddler";
        private const string grade12th = "12th grade";
        private StudentGradeMenuModel actualModel;

        protected override void EstablishContext()
        {
            schoolAreaLinks = new SchoolAreaLinksFake();
            studentSchoolInformationRepository = mocks.StrictMock<IRepository<StudentSchoolInformation>>();
            Expect.Call(studentSchoolInformationRepository.GetAll()).Return(GetSchoolStudentInformation());

            gradeLevelUtilitiesProvider = mocks.StrictMock<IGradeLevelUtilitiesProvider>();
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForSorting("12th grade")).Repeat.Times(3).Return(12);
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForSorting("Infant/toddler")).Return(-2);
            base.EstablishContext();

        }

        private IQueryable<StudentSchoolInformation> GetSchoolStudentInformation()
        {
            var list = new List<StudentSchoolInformation>
                           {
                                new StudentSchoolInformation{ SchoolId = suppliedSchoolId, GradeLevel = grade12th},
                                new StudentSchoolInformation{ SchoolId = suppliedSchoolId, GradeLevel = grade12th},
                                new StudentSchoolInformation{ SchoolId = suppliedSchoolId, GradeLevel = gradeInfant},
                                new StudentSchoolInformation{ SchoolId = suppliedSchoolId + 1, GradeLevel = "wrong grade"}
                           };
            return list.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new StudentGradeMenuService(studentSchoolInformationRepository, schoolAreaLinks, gradeLevelUtilitiesProvider);
            actualModel = service.Get(StudentGradeMenuRequest.Create(suppliedSchoolId));
        }

        [Test]
        public void Should_load_grades()
        {
            Assert.That(actualModel.Grades.Count, Is.EqualTo(3));
            Assert.That(actualModel.Grades.ElementAt(0).Attribute, Is.EqualTo("All Students"));
            Assert.That(actualModel.Grades.ElementAt(0).Url, Is.EqualTo(schoolAreaLinks.StudentGradeList(suppliedSchoolId, null, "All")));
            TestAttributeAndUrl(actualModel.Grades.ElementAt(1), gradeInfant);
            TestAttributeAndUrl(actualModel.Grades.ElementAt(2), grade12th);
        }
        
        private void TestAttributeAndUrl(AttributeItemWithUrl<decimal> model, string attribute)
        {
            Assert.That(model.Attribute, Is.EqualTo(attribute));
            Assert.That(model.Url, Is.EqualTo(schoolAreaLinks.StudentGradeList(suppliedSchoolId, null, attribute)));
        }
    }
}
