// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Resources.StudentSchool;
using NUnit.Framework;
using Rhino.Mocks;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Testing;

namespace EdFi.Dashboards.Resources.Tests.Student
{
    [TestFixture]
    public class When_calculating_student_accommodations_for_an_individual_student : TestFixtureBase
    {
        private const long studentUSI0 = 1;
        private const int schoolId0 = 2;
        private const int localEducationAgencyId0 = 3;

        private IRepository<StudentIndicator> studentIndicatorRepository;
        private IRepository<StudentSchoolInformation> studentSchoolInformationRepository;
        private Resources.School.IIdNameService idNameService;
        
        private IAccommodationProvider accommodationProvider;

        private IQueryable<StudentIndicator> suppliedData;
        private List<Accommodations> actualModel = new List<Accommodations>();
        
        protected override void EstablishContext()
        {
            studentIndicatorRepository = mocks.StrictMock<IRepository<StudentIndicator>>();
            studentSchoolInformationRepository = mocks.StrictMock<IRepository<StudentSchoolInformation>>();
            idNameService = mocks.StrictMock<Resources.School.IIdNameService>();
            
            suppliedData = ReturnSuppliedData();

            Expect.Call(studentIndicatorRepository.GetAll()).Return(suppliedData.AsQueryable());
            Expect.Call(studentSchoolInformationRepository.GetAll()).Return(GetSuppliedStudentSchoolInformation());
            
            Expect.Call(idNameService.Get(null))
                .Constraints(new ActionConstraint<Resources.School.IdNameRequest>(x => Assert.That(x.SchoolId, Is.EqualTo(schoolId0))))
                .Return(GetSuppliedSchoolIdNameModel());
        }

        private Models.School.IdNameModel GetSuppliedSchoolIdNameModel()
        {
            return new Models.School.IdNameModel { SchoolId = schoolId0, LocalEducationAgencyId = localEducationAgencyId0 };
        }

        private IQueryable<StudentIndicator> ReturnSuppliedData()
        {
            return (new List<StudentIndicator> { 
                //for G&T
                new StudentIndicator{  StudentUSI = studentUSI0, Name="Gifted/Talented",                EducationOrganizationId = localEducationAgencyId0,  Status = true},
                new StudentIndicator{  StudentUSI = studentUSI0, Name="Gifted/Talented",                EducationOrganizationId = localEducationAgencyId0,  Status = true},
                
                //If any of these then Special Services
                new StudentIndicator{  StudentUSI = studentUSI0, Name="Special Education",              EducationOrganizationId = localEducationAgencyId0, Status = true},
                new StudentIndicator{  StudentUSI = studentUSI0, Name="Other Services",                 EducationOrganizationId = localEducationAgencyId0, Status = true},
                new StudentIndicator{  StudentUSI = studentUSI0, Name="Primary Instructional Setting",  EducationOrganizationId = localEducationAgencyId0, Status = true},
                new StudentIndicator{  StudentUSI = studentUSI0, Name="Special Education Services",     EducationOrganizationId = localEducationAgencyId0, Status = true},
                new StudentIndicator{  StudentUSI = studentUSI0, Name="504 Designation",                EducationOrganizationId = localEducationAgencyId0, Status = true},

                //If any of these then Language
                new StudentIndicator{  StudentUSI = studentUSI0, Name="Limited English Proficiency",             EducationOrganizationId = localEducationAgencyId0, Status = true},
                new StudentIndicator{  StudentUSI = studentUSI0, Name="English as Second Language",     EducationOrganizationId = localEducationAgencyId0, Status = true},
                new StudentIndicator{  StudentUSI = studentUSI0, Name="Bilingual Program",              EducationOrganizationId = localEducationAgencyId0, Status = true},

                new StudentIndicator{  StudentUSI = studentUSI0, Name="Limited English Proficiency Monitored 1", EducationOrganizationId = localEducationAgencyId0, Status = true},
                new StudentIndicator{  StudentUSI = studentUSI0, Name="Limited English Proficiency Monitored 2", EducationOrganizationId = localEducationAgencyId0, Status = true},

                new StudentIndicator{  StudentUSI = studentUSI0, Name="Repeater",                       EducationOrganizationId = localEducationAgencyId0, Status = true},
                new StudentIndicator{  StudentUSI = studentUSI0, Name="Late Enrollment",                EducationOrganizationId = localEducationAgencyId0, Status = true},
                new StudentIndicator{  StudentUSI = studentUSI0, Name="Partial Transcript",             EducationOrganizationId = localEducationAgencyId0, Status = true},
                new StudentIndicator{  StudentUSI = studentUSI0, Name="Test Accommodation",             EducationOrganizationId = localEducationAgencyId0, Status = true},
                new StudentIndicator{  StudentUSI = studentUSI0, Name="Over Age",                        EducationOrganizationId = localEducationAgencyId0, Status = true},
            }).AsQueryable();
        }

        private IQueryable<StudentSchoolInformation> GetSuppliedStudentSchoolInformation()
        {
            return (new List<StudentSchoolInformation>
                        {
                            new StudentSchoolInformation{ SchoolId = schoolId0, StudentUSI = studentUSI0, LateEnrollment = "YES", IncompleteTranscript = "YES"},
                        }).AsQueryable();
        }

        protected override void ExecuteTest()
        {
            accommodationProvider = new AccommodationProvider(studentIndicatorRepository, studentSchoolInformationRepository, idNameService);
            actualModel = accommodationProvider.GetAccommodations(studentUSI0, schoolId0);
        }

        [Test]
        public void Should_have_a_gifted_and_talented_accommodation()
        {
            Assert.That(actualModel.Single(x => x == Accommodations.GiftedAndTalented), Is.EqualTo(Accommodations.GiftedAndTalented));
        }

        [Test]
        public void Should_have_a_special_services_accommodation()
        {
            Assert.That(actualModel.Single(x => x == Accommodations.SpecialEducation), Is.EqualTo(Accommodations.SpecialEducation));
        }

        [Test]
        public void Should_have_a_designation_accommodation()
        {
            Assert.That(actualModel.Single(x => x == Accommodations.Designation), Is.EqualTo(Accommodations.Designation));
        }

        [Test]
        public void Should_have_a_language_accommodation()
        {
            Assert.That(actualModel.Single(x => x == Accommodations.ESLAndLEP), Is.EqualTo(Accommodations.ESLAndLEP));
        }

        [Test]
        public void Should_have_a_language_monitored_first_accommodation()
        {
            Assert.That(actualModel.Single(x => x == Accommodations.LEPMonitoredFirst), Is.EqualTo(Accommodations.LEPMonitoredFirst));
        }

        [Test]
        public void Should_have_a_language_monitor_second_accommodation()
        {
            Assert.That(actualModel.Single(x => x == Accommodations.LEPMonitoredSecond), Is.EqualTo(Accommodations.LEPMonitoredSecond));
        }

        [Test]
        public void Should_have_a_repeater_accommodation()
        {
            Assert.That(actualModel.Single(x => x == Accommodations.Repeater), Is.EqualTo(Accommodations.Repeater));
        }

        [Test]
        public void Should_have_a_late_enrollment_accommodation()
        {
            Assert.That(actualModel.Single(x => x == Accommodations.LateEnrollment), Is.EqualTo(Accommodations.LateEnrollment));
        }

        [Test]
        public void Should_have_a_partial_transcript_accommodation()
        {
            Assert.That(actualModel.Single(x => x == Accommodations.PartialTranscript), Is.EqualTo(Accommodations.PartialTranscript));
        }

        [Test]
        public void Should_have_a_test_accommodation_accommodation()
        {
            Assert.That(actualModel.Single(x => x == Accommodations.TestAccommodation), Is.EqualTo(Accommodations.TestAccommodation));
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }

        //[Test]
        //public void Should_have_a_OverAge_Accommodation()
        //{
        //    Assert.That(actualModel.Single(x => x == Model.Accommodations.Overage), Is.EqualTo(Model.Accommodations.Overage));
        //}
    }    
}
