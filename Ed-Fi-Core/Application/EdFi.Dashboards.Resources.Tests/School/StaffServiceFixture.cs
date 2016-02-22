// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Images;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Mvc;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.School
{
    [TestFixture]
    public class When_invoking_staff_list_service_get : TestFixtureBase
    {
        //The Injected Dependencies.
        private IRepository<StaffInformation> staffInformationRepository;
        private IRepository<StaffEducationOrgInformation> staffEducationOrgInformationRepository;

        //The Actual Model.
        protected StaffModel actualModel;

        //The supplied Data models.
        protected const int suppliedSchoolId1 = 1;
        private IQueryable<StaffInformation> suppliedStaffInformationData;
        private IQueryable<StaffEducationOrgInformation> suppliedStaffEducationOrgInformationData;

        private IStaffAreaLinks staffAreaLinksFake = new StaffAreaLinksFake();

        protected override void EstablishContext()
        {
            //Prepare supplied data collections            
            suppliedStaffEducationOrgInformationData = GetSuppliedStaffEducationOrgInformation();
            suppliedStaffInformationData = GetSuppliedStaffInformation();

            //Set up the mocks
            staffInformationRepository = mocks.StrictMock<IRepository<StaffInformation>>();
            staffEducationOrgInformationRepository = mocks.StrictMock<IRepository<StaffEducationOrgInformation>>();

            //Set expectations
            Expect.Call(staffInformationRepository.GetAll()).Return(suppliedStaffInformationData);
            Expect.Call(staffEducationOrgInformationRepository.GetAll()).Return(suppliedStaffEducationOrgInformationData);
        }

        protected IQueryable<StaffInformation> GetSuppliedStaffInformation()
        {
            return (new List<StaffInformation>
                        {
                            new StaffInformation{StaffUSI = 1, DateOfBirth = DateTime.Today, EmailAddress = "a@aaa.com", FirstName = "John", LastSurname = "Doe", Gender = "Male", ProfileThumbnail = "image.jpg", YearsOfPriorProfessionalExperience = 1, HighestLevelOfEducationCompleted = "Masters", HighlyQualifiedTeacher = true},
                            new StaffInformation{StaffUSI = 2, DateOfBirth = DateTime.Today.AddDays(-1), EmailAddress = "b@aaa.com", FirstName = "Jane", LastSurname = "Doee", Gender = "Female", ProfileThumbnail = "image2.jpg", YearsOfPriorProfessionalExperience = 2, HighestLevelOfEducationCompleted = "PHD", HighlyQualifiedTeacher = false},
                            new StaffInformation{StaffUSI = 3, DateOfBirth = DateTime.Today.AddDays(-2), EmailAddress = "c@aaa.com", FirstName = "Doug", LastSurname = "Loyo", Gender = "Male", ProfileThumbnail = "cool.jpg", YearsOfPriorProfessionalExperience = 9, HighestLevelOfEducationCompleted = "Masters", HighlyQualifiedTeacher = null},
                            new StaffInformation{StaffUSI = 4}, //Should be filterd out by the join..

                        }).AsQueryable();
        }

        protected IQueryable<StaffEducationOrgInformation> GetSuppliedStaffEducationOrgInformation()
        {
            return (new List<StaffEducationOrgInformation>
                        {
                            new StaffEducationOrgInformation{StaffUSI = 1, StaffCategory = "Teacher", EducationOrganizationId = suppliedSchoolId1},
                            new StaffEducationOrgInformation{StaffUSI = 2, StaffCategory = "Educational Aide", EducationOrganizationId = suppliedSchoolId1},
                            new StaffEducationOrgInformation{StaffUSI = 3, StaffCategory = "SomethingElse", EducationOrganizationId = suppliedSchoolId1},
                            new StaffEducationOrgInformation{StaffUSI = 5, EducationOrganizationId = 9999}, //Should be filterd out by the join..
                        }).AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new StaffService(staffInformationRepository, staffEducationOrgInformationRepository, staffAreaLinksFake);
            actualModel = service.Get(StaffRequest.Create(suppliedSchoolId1));
        }

        [Test]
        public void Should_return_model_that_is_not_null()
        {
            Assert.That(actualModel, Is.Not.Null);
        }

        [Test]
        public void Should_return_model_with_staff()
        {
            var data = from s in suppliedStaffInformationData
                       join o in suppliedStaffEducationOrgInformationData on s.StaffUSI equals o.StaffUSI
                       where o.EducationOrganizationId == suppliedSchoolId1
                       select new {s, o};

            Assert.That(actualModel.Staff.Count(), Is.EqualTo(data.Count()), "Count does not match.");


            var suppliedStaff = data.GetEnumerator();
            var actualStaff = actualModel.Staff.GetEnumerator();
            while (suppliedStaff.MoveNext() && actualStaff.MoveNext())
            {
                var suppliedImage = staffAreaLinksFake.ListImage(suppliedSchoolId1, suppliedStaff.Current.s.StaffUSI, suppliedStaff.Current.s.Gender);

                Assert.That(actualStaff.Current.StaffUSI, Is.EqualTo(suppliedStaff.Current.s.StaffUSI));
                Assert.That(actualStaff.Current.ProfileThumbnail, Is.EqualTo(suppliedImage));
                Assert.That(actualStaff.Current.Name, Is.EqualTo(String.Format("{0}, {1}", suppliedStaff.Current.s.LastSurname, suppliedStaff.Current.s.FirstName)));
                Assert.That(actualStaff.Current.EmailAddress, Is.EqualTo(suppliedStaff.Current.s.EmailAddress));
                Assert.That(actualStaff.Current.DateOfBirth, Is.EqualTo(suppliedStaff.Current.s.DateOfBirth));
                Assert.That(actualStaff.Current.Gender, Is.EqualTo(suppliedStaff.Current.s.Gender));
                Assert.That(actualStaff.Current.YearsOfPriorProfessionalExperience, Is.EqualTo(suppliedStaff.Current.s.YearsOfPriorProfessionalExperience));
                Assert.That(actualStaff.Current.HighestLevelOfEducationCompleted, Is.EqualTo(suppliedStaff.Current.s.HighestLevelOfEducationCompleted));
                Assert.That(actualStaff.Current.HighlyQualifiedTeacher, Is.EqualTo((suppliedStaff.Current.s.HighlyQualifiedTeacher == null) ? "" : (suppliedStaff.Current.s.HighlyQualifiedTeacher.Value) ? "Yes" : "No"));
                Assert.That(actualStaff.Current.StaffCategory, Is.EqualTo(suppliedStaff.Current.o.StaffCategory));
            }
            
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }
}
