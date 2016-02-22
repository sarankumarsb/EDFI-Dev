// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Images;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.School
{
    [TestFixture]
    public class When_invoking_school_teachers_service_get : TestFixtureBase
    {
        //The Injected Dependencies.
        private IRepository<StaffInformation> staffInformationRepository;
        private IRepository<StaffEducationOrgInformation> staffEducationOrgInformationRepository;
        private IRepository<StaffSectionCohortAssociation> staffSectionCohortRepository;
        private IUniqueListIdProvider uniqueListIdProvider;

        //The Actual Model.
        protected TeachersModel actualModel;

        //The supplied Data models.
        protected const int suppliedSchoolId1 = 1;
        private const string suppliedUniqueId = "UniqueId1";
        private IQueryable<StaffInformation> suppliedStaffInformationData;
        private IQueryable<StaffEducationOrgInformation> suppliedStaffEducationOrgInformationData;
        private IQueryable<StaffSectionCohortAssociation> suppliedStaffSectionCohortData;
        private StaffAreaLinksFake staffAreaLinksFake = new StaffAreaLinksFake();

        protected override void EstablishContext()
        {
            //Prepare supplied data collections            
            suppliedStaffEducationOrgInformationData = GetSuppliedStaffEducationOrgInformation();
            suppliedStaffInformationData = GetSuppliedStaffInformation();
            suppliedStaffSectionCohortData = GetSuppliedStaffSectionCohortAssociation();

            //Set up the mocks
            staffInformationRepository = mocks.StrictMock<IRepository<StaffInformation>>();
            staffEducationOrgInformationRepository = mocks.StrictMock<IRepository<StaffEducationOrgInformation>>();
            staffSectionCohortRepository = mocks.StrictMock<IRepository<StaffSectionCohortAssociation>>();
            uniqueListIdProvider = mocks.StrictMock<IUniqueListIdProvider>();

            //Set expectations
            Expect.Call(staffInformationRepository.GetAll()).Return(suppliedStaffInformationData);
            Expect.Call(staffEducationOrgInformationRepository.GetAll()).Return(suppliedStaffEducationOrgInformationData);
            Expect.Call(staffSectionCohortRepository.GetAll()).Return(suppliedStaffSectionCohortData);
            Expect.Call(uniqueListIdProvider.GetUniqueId()).Repeat.Any().Return(suppliedUniqueId);
        }

        protected IQueryable<StaffInformation> GetSuppliedStaffInformation()
        {
            return (new List<StaffInformation>
                        {
                            new StaffInformation{StaffUSI = 1, DateOfBirth = DateTime.Today, EmailAddress = "a@aaa.com", FirstName = "John", LastSurname = "Doe", Gender = "Male", ProfileThumbnail = "image.jpg", YearsOfPriorProfessionalExperience = 1, HighestLevelOfEducationCompleted = "Masters", HighlyQualifiedTeacher = true},
                            new StaffInformation{StaffUSI = 2, DateOfBirth = DateTime.Today.AddDays(-1), EmailAddress = "b@aaa.com", FirstName = "Jane", LastSurname = "Doe", Gender = "Female", ProfileThumbnail = "image2.jpg", YearsOfPriorProfessionalExperience = 2, HighestLevelOfEducationCompleted = "PHD", HighlyQualifiedTeacher = false},
                            new StaffInformation{StaffUSI = 3, DateOfBirth = DateTime.Today.AddDays(-2), EmailAddress = "c@aaa.com", FirstName = "Doug", LastSurname = "Loyo", Gender = "Male", ProfileThumbnail = "cool.jpg", YearsOfPriorProfessionalExperience = 9, HighestLevelOfEducationCompleted = "Masters", HighlyQualifiedTeacher = null},
                            new StaffInformation{StaffUSI = 6, DateOfBirth = DateTime.Today.AddDays(-3), EmailAddress = "d@aaa.com", FirstName = "Ralph", LastSurname = "Mouse", Gender = "Male", ProfileThumbnail = "cool.jpg", YearsOfPriorProfessionalExperience = null, HighestLevelOfEducationCompleted = "Masters", HighlyQualifiedTeacher = null},
                            new StaffInformation{StaffUSI = 4}, //Should be filtered out by the join..

                        }).AsQueryable();
        }

        protected IQueryable<StaffEducationOrgInformation> GetSuppliedStaffEducationOrgInformation()
        {
            return (new List<StaffEducationOrgInformation>
                        {
                            new StaffEducationOrgInformation{StaffUSI = 1, EducationOrganizationId = suppliedSchoolId1},
                            new StaffEducationOrgInformation{StaffUSI = 2, EducationOrganizationId = suppliedSchoolId1},
                            new StaffEducationOrgInformation{StaffUSI = 3, EducationOrganizationId = suppliedSchoolId1},
                            new StaffEducationOrgInformation{StaffUSI = 6, EducationOrganizationId = suppliedSchoolId1},
                            new StaffEducationOrgInformation{StaffUSI = 5, EducationOrganizationId = 9999}, //Should be filtered out by the join..
                        }).AsQueryable();
        }

        protected IQueryable<StaffSectionCohortAssociation> GetSuppliedStaffSectionCohortAssociation()
        {
            return (new List<StaffSectionCohortAssociation>
                        {
                            new StaffSectionCohortAssociation{StaffUSI = 1, EducationOrganizationId = suppliedSchoolId1, SectionIdOrCohortId = 1},
                            new StaffSectionCohortAssociation{StaffUSI = 2, EducationOrganizationId = suppliedSchoolId1, SectionIdOrCohortId = 2},  
                            new StaffSectionCohortAssociation{StaffUSI = 3, EducationOrganizationId = suppliedSchoolId1, SectionIdOrCohortId = 3},  
                            new StaffSectionCohortAssociation{StaffUSI = 6, EducationOrganizationId = suppliedSchoolId1, SectionIdOrCohortId = 4},        
                            new StaffSectionCohortAssociation{StaffUSI = 6, EducationOrganizationId = suppliedSchoolId1, SectionIdOrCohortId = 5},                             
                            new StaffSectionCohortAssociation{StaffUSI = 5, EducationOrganizationId = 9999}, //Should be filtered out by the join..
                        }).AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new TeachersService(staffInformationRepository, staffEducationOrgInformationRepository, staffSectionCohortRepository, uniqueListIdProvider, staffAreaLinksFake);
            actualModel = service.Get(TeachersRequest.Create(suppliedSchoolId1));
        }

        [Test]
        public void Should_return_model_that_is_not_null()
        {
            Assert.That(actualModel, Is.Not.Null);
        }

        [Test]
        public void Should_return_correct_teacher_count()
        {
            Assert.That(actualModel.Teachers.Count(), Is.EqualTo(4));
        }

        [Test]
        public void Should_return_model_with_teachers()
        {
            var allTeachersThatTeach = (from s in suppliedStaffInformationData
                           join o in suppliedStaffEducationOrgInformationData on s.StaffUSI equals o.StaffUSI
                           join ssca in suppliedStaffSectionCohortData on new { o.StaffUSI, o.EducationOrganizationId } equals new { ssca.StaffUSI, ssca.EducationOrganizationId}
                           where o.EducationOrganizationId == suppliedSchoolId1
                           select s).ToList();
            
            foreach (var suppliedTeacher in allTeachersThatTeach)
            {
                var actualTeacher = actualModel.Teachers.First(x => x.StaffUSI == suppliedTeacher.StaffUSI);

                Assert.That(actualTeacher.StaffUSI, Is.EqualTo(suppliedTeacher.StaffUSI), "USI don't match.");
                Assert.That(actualTeacher.ProfileThumbnail, Is.EqualTo(staffAreaLinksFake.ListImage(suppliedSchoolId1, actualTeacher.StaffUSI, actualTeacher.Gender)), "Image doesn't match");
                Assert.That(actualTeacher.Name, Is.EqualTo(String.Format("{0}, {1}", suppliedTeacher.LastSurname, suppliedTeacher.FirstName)), "Name doesn't match.");
                Assert.That(actualTeacher.EmailAddress, Is.EqualTo(suppliedTeacher.EmailAddress), "Email Doesn't match.");
                Assert.That(actualTeacher.DateOfBirth, Is.EqualTo(suppliedTeacher.DateOfBirth), "DOB Doesn't match.");
                Assert.That(actualTeacher.Gender, Is.EqualTo(suppliedTeacher.Gender), "Gender Doesn't match.");
                Assert.That(actualTeacher.YearsOfPriorProfessionalExperience, Is.EqualTo(suppliedTeacher.YearsOfPriorProfessionalExperience), "Years of Experience Doesn't match.");
                Assert.That(actualTeacher.HighestLevelOfEducationCompleted, Is.EqualTo(suppliedTeacher.HighestLevelOfEducationCompleted), "Highest Level of Education Doesn't match.");
                Assert.That(actualTeacher.HighlyQualifiedTeacher, Is.EqualTo((suppliedTeacher.HighlyQualifiedTeacher == null) ? "" : (suppliedTeacher.HighlyQualifiedTeacher.Value) ? "Yes" : "No"), "HiglyQualifiedTeacher Doesn't match.");
                Assert.That(actualTeacher.Url, Is.EqualTo(staffAreaLinksFake.Default(suppliedSchoolId1, suppliedTeacher.StaffUSI, suppliedTeacher.FullName, null as int?, null, new { listContext = suppliedUniqueId})), "Href Doesn't Match");
            }
        }

        [Test]
        public void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel.EnsureNoDefaultValues(
                    "TeachersModel.Teachers[0].ResourceUrl", "TeachersModel.Teachers[0].Links", //This is expected because model implements IresourceBase and doesn't need the LINKS
                    "TeachersModel.Teachers[1].ResourceUrl", // No resources yet.
                    "TeachersModel.Teachers[2].ResourceUrl", // No resources yet.
                    "TeachersModel.Teachers[3].ResourceUrl", // No resources yet.
                    "TeachersModel.Teachers[3].YearsOfPriorProfessionalExperience"); //Expected null because of the data.
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }
}
