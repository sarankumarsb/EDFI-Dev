// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using NUnit.Framework;
using Rhino.Mocks;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Testing;

namespace EdFi.Dashboards.Resources.Tests.Student
{
    [TestFixture]
    public class When_calling_the_selected_student_service_with_a_student_that_exists : TestFixtureBase
    {
        protected BriefModel actualModel;
        protected BriefService service;

        protected const long studentUSI0 = 1;
        protected const int schoolId0 = 2;
        protected const int localEducationAgencyId0 = 2;
        protected const string suppliedGender = "Male";
        protected IQueryable<StudentInformation> suppliedStudentInformation;
        protected IQueryable<StudentSchoolInformation> suppliedStudentSchoolInformation;
        protected List<Accommodations> suppliedAccommodations;
        protected IStudentSchoolAreaLinks studentSchoolAreaLinks;

        protected IRepository<StudentSchoolInformation> studentSchoolInformationRepository;
        protected IRepository<StudentInformation> studentInformationRepository;
        protected IAccommodationProvider accommodationProvider;

        protected override void EstablishContext()
        {
            studentSchoolInformationRepository = mocks.StrictMock<IRepository<StudentSchoolInformation>>();
            studentInformationRepository = mocks.StrictMock<IRepository<StudentInformation>>();
            accommodationProvider =mocks.StrictMock<IAccommodationProvider>();
            studentSchoolAreaLinks = new StudentSchoolAreaLinksFake();

            suppliedStudentInformation = GetSuppliedStudentData();
            suppliedStudentSchoolInformation = GetSuppliedStudentSchoolData();
            suppliedAccommodations = GetSuppliedAccommodations();


            //Expected calls.
            Expect.Call(studentInformationRepository.GetAll()).Return(suppliedStudentInformation);
            Expect.Call(studentSchoolInformationRepository.GetAll()).Return(suppliedStudentSchoolInformation);            
            Expect.Call(accommodationProvider.GetAccommodations(studentUSI0, schoolId0)).Return(suppliedAccommodations);
        }

        protected IQueryable<StudentInformation> GetSuppliedStudentData()
        {
            return (new List<StudentInformation>
                        {
                            new StudentInformation{StudentUSI = studentUSI0, FullName = "John Doe III", Gender = suppliedGender, Race = "Whatever"}
                        }).AsQueryable();
        }

        protected IQueryable<StudentSchoolInformation> GetSuppliedStudentSchoolData()
        {
            return (new List<StudentSchoolInformation>
                        {
                            new StudentSchoolInformation{StudentUSI = studentUSI0, SchoolId=schoolId0, GradeLevel = "1st Grade", Homeroom = "My Homeroom"}
                        }).AsQueryable();
        }

        protected List<Accommodations> GetSuppliedAccommodations()
        {
            return new List<Accommodations> { Accommodations.Overage, Accommodations.ESLAndLEP };
        }


        protected override void ExecuteTest()
        {
            service = new BriefService(studentInformationRepository, studentSchoolInformationRepository, accommodationProvider, studentSchoolAreaLinks);
            
            var request = new BriefRequest
                              {
                                  StudentUSI = studentUSI0,
                                  SchoolId = schoolId0,
                              };

            actualModel = service.Get(request);
        }

        [Test]
        public void Should_return_model_that_is_not_null()
        {
            Assert.That(actualModel, Is.Not.Null);
        }

        [Test]
        public void Should_return_model_with_student_properties_correctly_mapped()
        {
            var suppliedStudent = suppliedStudentInformation.Single();
            
            Assert.That(actualModel.FullName, Is.EqualTo(suppliedStudent.FullName));
            Assert.That(actualModel.Gender, Is.EqualTo(suppliedStudent.Gender));
            Assert.That(actualModel.Race, Is.EqualTo(suppliedStudent.Race));
            Assert.That(actualModel.ProfileThumbnail, Is.EqualTo(studentSchoolAreaLinks.ProfileThumbnail(schoolId0, studentUSI0, suppliedGender, suppliedStudent.FullName)));
            Assert.That(actualModel.Accommodations, Is.EqualTo(suppliedAccommodations));

        }

        [Test]
        public void Should_return_model_with_student_school_properties_correctly_mapped()
        {
            var suppliedStudentSchoolInfo = suppliedStudentSchoolInformation.Single();
            
            Assert.That(actualModel.GradeLevel, Is.EqualTo(suppliedStudentSchoolInfo.GradeLevel));
            Assert.That(actualModel.Homeroom, Is.EqualTo(suppliedStudentSchoolInfo.Homeroom));
        }

        [Test]
        public void Should_return_model_with_student_image_correctly_mapped()
        {
            var suppliedStudent = suppliedStudentInformation.Single();
            Assert.That(actualModel.ProfileThumbnail, Is.EqualTo(studentSchoolAreaLinks.ProfileThumbnail(schoolId0, studentUSI0, suppliedGender, suppliedStudent.FullName)));
            Assert.That(actualModel.Accommodations, Is.EqualTo(suppliedAccommodations));
        }

        [Test]
        public void Should_return_model_with_student_accommodations_correctly_mapped()
        {
            Assert.That(actualModel.Accommodations, Is.EqualTo(suppliedAccommodations));
        }

        [Test]
        public void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel.EnsureNoDefaultValues(
                "BriefModel.Url",
                "BriefModel.ResourceUrl",
                "BriefModel.Links");
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }

    [TestFixture]
    public class When_calling_the_selected_student_service_with_a_student_that_does_not_exists : TestFixtureBase
    {
        protected BriefModel actualDefaultModel;
        protected BriefService service;

        protected const long studentUSI1 = 11;
        protected const int schoolId0 = 2;
        protected const int localEducationAgencyId0 = 2;

        protected IRepository<StudentSchoolInformation> studentSchoolInformationRepository;
        protected IRepository<StudentInformation> studentInformationRepository;
        protected IAccommodationProvider accommodationProvider;
        protected IStudentSchoolAreaLinks studentSchoolAreaLinks;

        protected override void EstablishContext()
        {
            studentSchoolInformationRepository = mocks.StrictMock<IRepository<StudentSchoolInformation>>();
            studentInformationRepository = mocks.StrictMock<IRepository<StudentInformation>>();
            accommodationProvider = mocks.StrictMock<IAccommodationProvider>();
            studentSchoolAreaLinks = new StudentSchoolAreaLinksFake();

            //Expected calls.
            Expect.Call(studentInformationRepository.GetAll()).Return((new List<StudentInformation>()).AsQueryable());
            Expect.Call(studentSchoolInformationRepository.GetAll()).Return((new List<StudentSchoolInformation>()).AsQueryable());            
        }


        protected override void ExecuteTest()
        {
            service = new BriefService(studentInformationRepository, studentSchoolInformationRepository, accommodationProvider, studentSchoolAreaLinks);
            
            var request = new BriefRequest
            {
                StudentUSI = studentUSI1,
                SchoolId = schoolId0,
            };

            actualDefaultModel = service.Get(request);
        }

        [Test]
        public void Should_return_model_that_is_not_null()
        {
            Assert.That(actualDefaultModel, Is.Not.Null);
        }

        [Test]
        public void Should_return_model_with_student_properties_correctly_mapped()
        {

            Assert.That(actualDefaultModel.FullName, Is.EqualTo("No student found."));
            Assert.That(actualDefaultModel.GradeLevel, Is.EqualTo(""));
            Assert.That(actualDefaultModel.ProfileThumbnail, Is.EqualTo(studentSchoolAreaLinks.ProfileThumbnail(schoolId0, studentUSI1, "Male")));
            Assert.That(actualDefaultModel.Homeroom, Is.EqualTo(""));
            Assert.That(actualDefaultModel.StudentUSI, Is.EqualTo(-1));

        }
    }
}
