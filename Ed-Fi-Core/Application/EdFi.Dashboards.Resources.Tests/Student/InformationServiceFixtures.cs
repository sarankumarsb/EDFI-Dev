// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Resources.Models.Student.Information;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Testing.NBuilder;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Implementation;
using NUnit.Framework;
using Rhino.Mocks;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Testing;

namespace EdFi.Dashboards.Resources.Tests.Student
{
    //This is abstract for the extensions to be able to leverage these tests.
    public abstract class When_requesting_student_information<TRequest, TResponse, TService, TSchoolInformation, TParentInformation, TStudentProgramParticipation, TOtherStudentInformation, TSpecialService> : TestFixtureBase
        where TRequest : InformationRequest, new()
        where TResponse : InformationModel, new()
        where TSchoolInformation : SchoolInformationDetail, new()
        where TParentInformation : ParentInformation, new()
        where TStudentProgramParticipation : StudentProgramParticipation, new()
        where TOtherStudentInformation : OtherInformation, new()
        where TSpecialService : SpecialService, new()
        where TService : InformationServiceBase<TRequest, TResponse, TSchoolInformation, TParentInformation, TStudentProgramParticipation, TOtherStudentInformation, TSpecialService>, new()
    {
        //The Actual Model.
        protected TResponse actualModel;

        //The Actual Service.
        protected TService service;

        //The supplied Data models.
        protected const long studentUSI0 = 1;
        protected const int schoolId0 = 2;
        protected const int localEducationAgencyId0 = 3;
        protected StudentInformation suppliedStudentInfo;
        protected IQueryable<StudentParentInformation> suppliedStudentParentInfo;
        protected IQueryable<StudentIndicator> suppliedStudentIndicators;
        protected IQueryable<StudentSchoolInformation> suppliedStudentSchoolInfo;
        protected IStudentSchoolAreaLinks studentSchoolAreaLinks = new StudentSchoolAreaLinksFake();

        protected override void EstablishContext()
        {
            // Initialize the builder
            BuilderSetup.SetDefaultPropertyNamer(
                new NonDefaultNonRepeatingPropertyNamer(
                    new ReflectionUtil()));

            // Build a Data.StudentInformationModel instance, with hard-coded image reference
            suppliedStudentInfo = Builder<StudentInformation>.CreateNew().Build();
            suppliedStudentInfo.StudentUSI = studentUSI0;
            suppliedStudentInfo.EnsureNoDefaultValues();

            suppliedStudentSchoolInfo = GetSuppliedSchoolInformation();

            suppliedStudentParentInfo = GetSuppliedStudentParentInfo();

            suppliedStudentIndicators = GetSuppliedStudentIndicators();

            // Set up the mocks
            service = new TService
            {
                SchoolCategoryProvider = mocks.StrictMock<Resources.School.ISchoolCategoryProvider>(),
                StudentInformationRepository = mocks.StrictMock<IRepository<StudentInformation>>(),
                StudentSchoolInformationRepository = mocks.StrictMock<IRepository<StudentSchoolInformation>>(),
                StudentParentInformationRepository = mocks.StrictMock<IRepository<StudentParentInformation>>(),
                StudentIndicatorRepository = mocks.StrictMock<IRepository<StudentIndicator>>(),
                StudentSchoolAreaLinks = studentSchoolAreaLinks
            };

            Expect.Call(service.StudentInformationRepository.GetAll()).Return((new List<StudentInformation> { suppliedStudentInfo }).AsQueryable());
            Expect.Call(service.StudentSchoolInformationRepository.GetAll()).Return(suppliedStudentSchoolInfo);
            Expect.Call(service.StudentParentInformationRepository.GetAll()).Return(suppliedStudentParentInfo.AsQueryable());
            Expect.Call(service.StudentIndicatorRepository.GetAll()).Repeat.Any().Return(suppliedStudentIndicators.AsQueryable());

            //Tested in its own single responsibility test.
            Expect.Call(service.SchoolCategoryProvider.GetSchoolCategoryType(schoolId0)).Return(SchoolCategory.HighSchool);
        }

        #region ForSuppliedInfo

        protected virtual IQueryable<StudentParentInformation> GetSuppliedStudentParentInfo()
        {
            // Build two Data.StudentParentInformation instances
            var suppliedData = Builder<StudentParentInformation>.CreateListOfSize(2).Build().ToList();
            foreach (var p in suppliedData)
            {
                p.StudentUSI = studentUSI0;
                p.EnsureNoDefaultValues();
            }

            return suppliedData.AsQueryable();
        }

        protected virtual IQueryable<StudentSchoolInformation> GetSuppliedSchoolInformation()
        {
            // Build a Data.StudentSchoolInformation instance
            var suppliedData = Builder<StudentSchoolInformation>.CreateNew().Build();
            suppliedData.StudentUSI = studentUSI0;
            suppliedData.SchoolId = schoolId0;
            suppliedData.EnsureNoDefaultValues();

            return new []{suppliedData}.AsQueryable();
        }

        protected virtual List<StudentIndicator> GetSuppliedSpecialServices()
        {
            var res = new List<StudentIndicator>{
                        new StudentIndicator{ StudentUSI = 1, EducationOrganizationId = 2, Name = "P1", ParentName = null, Status = true, Type = "Special"},
                        new StudentIndicator{ StudentUSI = 1, EducationOrganizationId = 2, Name = "SP1", ParentName = "P1", Status = true, Type = "Special"},
                        new StudentIndicator{ StudentUSI = 1, EducationOrganizationId = 2, Name = "P2", ParentName = null, Status = false, Type = "Special"},
                    };
            return res;
        }

        protected virtual IQueryable<StudentIndicator> GetSuppliedStudentIndicators()
        {
            var res = new List<StudentIndicator>{
                        new StudentIndicator{ StudentUSI = studentUSI0, EducationOrganizationId = schoolId0, Name = "apple", ParentName = null, Status = true, Type = StudentIndicatorType.Special.ToString(), DisplayOrder = 2},
                        new StudentIndicator{ StudentUSI = studentUSI0, EducationOrganizationId = schoolId0, Name = "SP1", ParentName = "apple", Status = true, Type = StudentIndicatorType.Special.ToString()},
                        new StudentIndicator{ StudentUSI = studentUSI0, EducationOrganizationId = schoolId0, Name = "banana", ParentName = null, Status = false, Type = StudentIndicatorType.Special.ToString(), DisplayOrder = 1},

                        new StudentIndicator{ StudentUSI = studentUSI0, EducationOrganizationId = schoolId0, Name = "Prog1", ParentName = null, Status = true, Type = StudentIndicatorType.Program.ToString()},
                        new StudentIndicator{ StudentUSI = studentUSI0, EducationOrganizationId = schoolId0, Name = "Prog2", ParentName = null, Status = true, Type = StudentIndicatorType.Program.ToString()},
                        new StudentIndicator{ StudentUSI = studentUSI0, EducationOrganizationId = schoolId0, Name = "Prog3", ParentName = "apple", Status = true, Type = StudentIndicatorType.Program.ToString()},

                        new StudentIndicator{ StudentUSI = studentUSI0, EducationOrganizationId = schoolId0, Name = "Other1", ParentName = null, Status = true, Type = StudentIndicatorType.Other.ToString()},
                        new StudentIndicator{ StudentUSI = studentUSI0, EducationOrganizationId = schoolId0, Name = "Other2", ParentName = "apple", Status = true, Type = StudentIndicatorType.Other.ToString()},
                        new StudentIndicator{ StudentUSI = studentUSI0, EducationOrganizationId = schoolId0, Name = "Other Child For 1", ParentName = "Other1", Status = true, Type = StudentIndicatorType.Other.ToString()},
                        new StudentIndicator{ StudentUSI = studentUSI0, EducationOrganizationId = localEducationAgencyId0, Name = "Other3", ParentName = "apple", Status = true, Type = StudentIndicatorType.Other.ToString()},
                    };

            return res.AsQueryable();
        }

        #endregion

        protected override void ExecuteTest()
        {
            var suppliedRequest = new TRequest
            {
                LocalEducationAgencyId = localEducationAgencyId0,
                SchoolId = schoolId0,
                StudentUSI = studentUSI0
            };

            actualModel = service.Get(suppliedRequest);
        }

        [Test]
        public virtual void Should_assign_all_properties_on_student_information_presentation_model_correctly()
        {
            //Asserting the Student's Info
            Assert.That(actualModel.StudentUSI, Is.EqualTo(suppliedStudentInfo.StudentUSI));
            Assert.That(actualModel.FullName, Is.EqualTo(suppliedStudentInfo.FullName));
            Assert.That(actualModel.EmailAddress, Is.EqualTo(suppliedStudentInfo.EmailAddress));

            Assert.That(actualModel.AddressLines.ElementAt(0), Is.EqualTo(suppliedStudentInfo.AddressLine1));
            Assert.That(actualModel.AddressLines.ElementAt(1), Is.EqualTo(suppliedStudentInfo.AddressLine2));
            Assert.That(actualModel.AddressLines.ElementAt(2), Is.EqualTo(suppliedStudentInfo.AddressLine3));

            Assert.That(actualModel.City, Is.EqualTo(suppliedStudentInfo.City));
            Assert.That(actualModel.State, Is.EqualTo(suppliedStudentInfo.State));
            Assert.That(actualModel.ZipCode, Is.EqualTo(suppliedStudentInfo.ZipCode));
            Assert.That(actualModel.TelephoneNumber, Is.EqualTo(suppliedStudentInfo.TelephoneNumber));
            Assert.That(actualModel.DateOfBirth, Is.EqualTo(suppliedStudentInfo.DateOfBirth));
            Assert.That(actualModel.PlaceOfBirth, Is.EqualTo(suppliedStudentInfo.PlaceOfBirth));
            Assert.That(actualModel.Gender, Is.EqualTo(suppliedStudentInfo.Gender));
            Assert.That(actualModel.OldEthnicity, Is.EqualTo(suppliedStudentInfo.OldEthnicity));
            Assert.That(actualModel.HispanicLatinoEthnicity, Is.EqualTo(suppliedStudentInfo.HispanicLatinoEthnicity));
            Assert.That(actualModel.HomeLanguage, Is.EqualTo(suppliedStudentInfo.HomeLanguage));
            Assert.That(actualModel.Language, Is.EqualTo(suppliedStudentInfo.Language));
            Assert.That(actualModel.Race, Is.EqualTo(suppliedStudentInfo.Race));

            Assert.That(actualModel.CurrentAge, Is.EqualTo(suppliedStudentInfo.CurrentAge));
            Assert.That(actualModel.CohortYear, Is.EqualTo(suppliedStudentInfo.CohortYear));
            Assert.That(actualModel.SingleParentPregnantTeen, Is.EqualTo(suppliedStudentInfo.SingleParentPregnantTeen));
            Assert.That(actualModel.ParentMilitary, Is.EqualTo(suppliedStudentInfo.ParentMilitary));

            Assert.That(actualModel.ProfileThumbnail, Is.EqualTo(studentSchoolAreaLinks.Image(schoolId0, studentUSI0, suppliedStudentInfo.Gender, suppliedStudentInfo.FullName)));
        }

        [Test]
        public virtual void Should_assign_all_properties_on_school_info_presentation_model_correctly()
        {
            var suppliedItem = suppliedStudentSchoolInfo.First();
            //Asserting the School Info
            Assert.That(actualModel.SchoolInformation.DateOfEntry, Is.EqualTo(suppliedItem.DateOfEntry));
            Assert.That(actualModel.SchoolInformation.DateOfWithdrawal, Is.EqualTo(suppliedItem.DateOfWithdrawal));
            Assert.That(actualModel.SchoolInformation.ExpectedGraduationYear, Is.EqualTo(suppliedItem.ExpectedGraduationYear));
            Assert.That(actualModel.SchoolInformation.GradeLevel, Is.EqualTo(suppliedItem.GradeLevel));
            Assert.That(actualModel.SchoolInformation.Homeroom, Is.EqualTo(suppliedItem.Homeroom));
        }

        [Test]
        public virtual void Should_assign_all_properties_on_feeder_schools_presentation_model_correctly()
        {
            var suppliedItem = suppliedStudentSchoolInfo.First();
            Assert.That(actualModel.SchoolInformation.FeederSchools[0], Is.EqualTo(suppliedItem.FeederSchools));
            //Assert.That(actualModel.SchoolInformation.FeederSchools[1], Is.EqualTo(suppliedStudentSchoolInfo.FeederSchools[1]));
        }

        [Test]
        public virtual void Should_assign_all_properties_on_program_status_presentation_model_correctly()
        {
            var suppliedStudentProgramInfo = GetSuppliedStudentIndicators().Where(x => x.Type == StudentIndicatorType.Program.ToString()).ToList();
            //Asserting the Program Status
            for (int i = 0; i < 2; i++)
            {
                Assert.That(actualModel.Programs.ElementAt(i).Status, Is.EqualTo(suppliedStudentProgramInfo[i].Status));
                Assert.That(actualModel.Programs.ElementAt(i).Name, Is.EqualTo(suppliedStudentProgramInfo[i].Name));
            }
        }

        [Test]
        public virtual void Should_construct_special_services_count_correctly()
        {
            //Two roots.
            Assert.That(actualModel.SpecialServices.Count(), Is.EqualTo(2));

            //One child in the second one because of the sorting.
            Assert.That(actualModel.SpecialServices.ElementAt(1).Children.Count(), Is.EqualTo(1));
        }

        [Test]
        public virtual void Should_construct_special_services_hierarchy_correctly()
        {
            var suppliedSpecialSeervices = GetSuppliedStudentIndicators().Where(x => x.Type == StudentIndicatorType.Special.ToString()).OrderBy(x => x.DisplayOrder).ToList();

            Assert.That(actualModel.SpecialServices.ElementAt(1).Name, Is.EqualTo(suppliedSpecialSeervices[2].Name));
            Assert.That(actualModel.SpecialServices.ElementAt(1).Status, Is.EqualTo(suppliedSpecialSeervices[2].Status));

            Assert.That(actualModel.SpecialServices.ElementAt(1).Children.ElementAt(0).Name, Is.EqualTo(suppliedSpecialSeervices[0].Name));
            Assert.That(actualModel.SpecialServices.ElementAt(1).Children.ElementAt(0).Status, Is.EqualTo(suppliedSpecialSeervices[0].Status));

            Assert.That(actualModel.SpecialServices.ElementAt(0).Name, Is.EqualTo(suppliedSpecialSeervices[1].Name));
            Assert.That(actualModel.SpecialServices.ElementAt(0).Status, Is.EqualTo(suppliedSpecialSeervices[1].Status));
        }

        [Test]
        public virtual void Should_assign_all_properties_on_other_student_information_presentation_model_correctly()
        {
            var suppliedOtherStudentInfo = GetSuppliedStudentIndicators().Where(x => x.Type == StudentIndicatorType.Other.ToString() && x.ParentName == null).ToList();

            Assert.That(actualModel.OtherStudentInformation.Count(), Is.EqualTo(suppliedOtherStudentInfo.Count), "Count is wrong.");

            //Asserting the OtherStudentInformation Name and Status
            for (int i = 0; i < suppliedOtherStudentInfo.Count; i++)
            {
                Assert.That(actualModel.OtherStudentInformation.ElementAt(i).Status, Is.EqualTo(suppliedOtherStudentInfo[i].Status), "Status does not match.");
                Assert.That(actualModel.OtherStudentInformation.ElementAt(i).Name, Is.EqualTo(suppliedOtherStudentInfo[i].Name));

                var suppliedChildren = (from s in GetSuppliedStudentIndicators()
                                        where s.ParentName == suppliedOtherStudentInfo[i].Name
                                        select s).ToList();

                //If children assert model has them.
                Assert.That(actualModel.OtherStudentInformation.ElementAt(i).Children.Count(), Is.EqualTo(suppliedChildren.Count()));
                for (int j = 0; j < suppliedChildren.Count(); j++)
                {
                    Assert.That(actualModel.OtherStudentInformation.ElementAt(i).Children.ElementAt(j).Status, Is.EqualTo(suppliedChildren[j].Status), "Status does not match.");
                    Assert.That(actualModel.OtherStudentInformation.ElementAt(i).Children.ElementAt(j).Name, Is.EqualTo(suppliedChildren[j].Name));
                }

            }


        }

        [Test]
        public virtual void Should_assign_all_properties_on_parent_information_presentation_model_correctly()
        {
            var suppliedStudentParentInfoList = suppliedStudentParentInfo.ToList();

            Assert.That(actualModel.Parents.Count(), Is.GreaterThan(0));
            //Asserting the Parents Info
            for (int i = 0; i < 2; i++)
            {
                Assert.That(actualModel.Parents.ElementAt(i).ParentUSI, Is.EqualTo(suppliedStudentParentInfoList[i].ParentUSI));
                Assert.That(actualModel.Parents.ElementAt(i).FullName, Is.EqualTo(suppliedStudentParentInfoList[i].FullName));
                Assert.That(actualModel.Parents.ElementAt(i).Relation, Is.EqualTo(suppliedStudentParentInfoList[i].Relation));
                Assert.That(actualModel.Parents.ElementAt(i).AddressLines[0], Is.EqualTo(suppliedStudentParentInfoList[i].AddressLine1));
                Assert.That(actualModel.Parents.ElementAt(i).AddressLines[1], Is.EqualTo(suppliedStudentParentInfoList[i].AddressLine2));
                Assert.That(actualModel.Parents.ElementAt(i).AddressLines[2], Is.EqualTo(suppliedStudentParentInfoList[i].AddressLine3));
                Assert.That(actualModel.Parents.ElementAt(i).City, Is.EqualTo(suppliedStudentParentInfoList[i].City));
                Assert.That(actualModel.Parents.ElementAt(i).State, Is.EqualTo(suppliedStudentParentInfoList[i].State));
                Assert.That(actualModel.Parents.ElementAt(i).ZipCode, Is.EqualTo(suppliedStudentParentInfoList[i].ZipCode));
                Assert.That(actualModel.Parents.ElementAt(i).TelephoneNumber, Is.EqualTo(suppliedStudentParentInfoList[i].TelephoneNumber));
                Assert.That(actualModel.Parents.ElementAt(i).EmailAddress, Is.EqualTo(suppliedStudentParentInfoList[i].EmailAddress));
            }
        }

        [Test]
        public virtual void Should_have_no_unassigned_values_on_presentation_model()
        {
            // TODO: Deferred - Actually map those properties when data is available.
            actualModel.EnsureNoDefaultValues(
                                    "InformationModel.Programs",							//No Data
                                    "InformationModel.SchoolInformation.FeederSchools",	//No Data
                                    "InformationModel.SpecialServices[1].Children[0].Children",	//No Data on the second child
                                    "InformationModel.SpecialServices[0].Status",		//I'm testing for false
                                    "InformationModel.SpecialServices[0].Children",	//No Data no child on this one.
                                    "InformationModel.Programs[0].Children"	//No Data no child on this one.
                                    );
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

    [TestFixture]
    public class When_requesting_student_information_from_the_EdFi_Student_Information_Service : When_requesting_student_information<InformationRequest, InformationModel, InformationService, SchoolInformationDetail, ParentInformation, StudentProgramParticipation, OtherInformation, SpecialService>
    {

    }
}

