// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Models.Student.Overview;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Student
{
    [TestFixture]
    public abstract class When_invoking_the_student_overview_resource_base<TRequest, TResponse, TService> : TestFixtureBase
        where TRequest : OverviewRequest, new()
        where TResponse : OverviewModel, new()
        where TService : OverviewServiceBase<TRequest,TResponse>, new()
    {
        //The Injected Dependencies.
        protected IRepository<StudentInformation> studentInformationRepository;
        protected IRepository<StudentSchoolInformation> studentSchoolInformationRepository;
        protected IRepository<StudentIndicator> studentIndicatorRepository;
        protected IStudentSchoolAreaLinks studentSchoolAreaLinks;
        protected IAccommodationProvider accommodationProvider;

        //The Actual Model.
        protected TResponse actualModel;
        protected TService service;

        //The supplied Data models.
        protected const int suppliedStudentUSI = 1;
        protected const int suppliedSchoolId = 1000;
        protected const int suppliedLocalEducationAgencyId = 10000;
        protected const string suppliedFullName = "Test Name";
        protected const string suppliedGender = "apple";
        protected const string suppliedGradeLevel = "Correct Grade Level";
        protected const string suppliedLanguage = "Correct Language";
        protected const string suppliedHomeLanguage = "Correct Home Language";
        protected DateTime suppliedDateOfBirth = new DateTime(2010, 5, 1);
        protected TRequest suppliedRequest;
        protected IQueryable<StudentInformation> suppliedStudentInformation;
        
        protected override void EstablishContext()
        {
            //Prepare supplied data collections
            suppliedStudentInformation = GetSuppliedStudentInformationData();
            suppliedRequest = new TRequest
                          {
                              SchoolId = suppliedSchoolId,
                              StudentUSI = suppliedStudentUSI,
                              LocalEducationAgencyId = suppliedLocalEducationAgencyId
                          };


            //Set up the mocks
            studentInformationRepository = mocks.StrictMock<IRepository<StudentInformation>>();
            studentSchoolInformationRepository = mocks.StrictMock<IRepository<StudentSchoolInformation>>();
            studentIndicatorRepository = mocks.StrictMock<IRepository<StudentIndicator>>();
            studentSchoolAreaLinks = new StudentSchoolAreaLinksFake();
            accommodationProvider = mocks.StrictMock<IAccommodationProvider>();

            //Set expectations
            Expect.Call(studentInformationRepository.GetAll()).Return(suppliedStudentInformation);
            Expect.Call(studentSchoolInformationRepository.GetAll()).Return(GetSuppliedStudentSchoolInformation());
            Expect.Call(accommodationProvider.GetAccommodations(suppliedStudentUSI, suppliedSchoolId)).Return(GetSuppliedAccommodations());
        }

        protected IQueryable<StudentInformation> GetSuppliedStudentInformationData()
        {
            return (new List<StudentInformation>
                        {
                            new StudentInformation
                                {
                                    StudentUSI = 55, 
                                    FullName = "Full Name 55",
                                    ProfileThumbnail = "Thumbnail 55"
                                },
                            new StudentInformation
                                {
                                    StudentUSI = suppliedStudentUSI, 
                                    FullName = suppliedFullName,                                    
                                    ProfileThumbnail = "SuppliedImage.Jpg",
                                    Gender = suppliedGender,
                                    Race = "banana",
                                    Language = suppliedLanguage,
                                    HomeLanguage = suppliedHomeLanguage,
                                    DateOfBirth = suppliedDateOfBirth
                                },
                            new StudentInformation
                                {
                                    StudentUSI = 90, 
                                    FullName = "Full Name 90",                                    
                                    ProfileThumbnail = "Thumbnail 90"

                                }
                        }).AsQueryable();
        }

        protected IQueryable<StudentSchoolInformation> GetSuppliedStudentSchoolInformation()
        {
            var list = new List<StudentSchoolInformation>
                           {
                               new StudentSchoolInformation
                                   {
                                       StudentUSI = 55,
                                       SchoolId = suppliedSchoolId,
                                       GradeLevel = "wrong 55"
                                   },
                               new StudentSchoolInformation
                                   {
                                       StudentUSI = suppliedStudentUSI,
                                       SchoolId = suppliedSchoolId,
                                       GradeLevel = suppliedGradeLevel
                                   },
                               new StudentSchoolInformation
                                   {
                                       StudentUSI = suppliedStudentUSI,
                                       SchoolId = suppliedSchoolId + 100,
                                       GradeLevel = "wrong " + suppliedStudentUSI
                                   },
                               new StudentSchoolInformation
                                   {
                                       StudentUSI = 90,
                                       SchoolId = suppliedSchoolId,
                                       GradeLevel = "wrong 90"
                                   },

                           };
            return list.AsQueryable();
        }

        protected virtual List<Accommodations> GetSuppliedAccommodations()
        {
            return new List<Accommodations>();
        }

        protected override void ExecuteTest()
        {
            service = new TService
                          {
                              StudentInformationRepository = studentInformationRepository,
                              StudentSchoolInformationRepository = studentSchoolInformationRepository,
                              StudentIndicatorRepository = studentIndicatorRepository,
                              StudentSchoolAreaLinks = studentSchoolAreaLinks,
                              AccommodationProvider = accommodationProvider
                          };

            actualModel = service.Get(suppliedRequest);
        }

        [Test]
        public virtual void Should_not_return_null()
        {
            Assert.NotNull(actualModel);
        }

        [Test]
        public virtual void Should_have_the_correct_StudentUSI()
        {
            Assert.That(actualModel.StudentUSI, Is.EqualTo(suppliedStudentUSI));
        }

        [Test]
        public virtual void Should_have_the_correct_student_information()
        {
            Assert.That(actualModel.FullName, Is.EqualTo(suppliedFullName));
            Assert.That(actualModel.DateOfBirth, Is.EqualTo(suppliedDateOfBirth));
            Assert.That(actualModel.Language, Is.EqualTo(suppliedLanguage));
            Assert.That(actualModel.HomeLanguage, Is.EqualTo(suppliedHomeLanguage));
        }

        [Test]
        public virtual void Should_have_the_correct_overview_settings()
        {
            Assert.That(actualModel.MetricVariantId, Is.EqualTo((int)StudentMetricEnum.Overview));
            Assert.That(actualModel.RenderingMode, Is.EqualTo("Overview"));
        }

        [Test]
        public virtual void Should_have_the_correct_grade_level()
        {
            Assert.That(actualModel.GradeLevel, Is.EqualTo(suppliedGradeLevel));
        }

        [Test]
        public virtual void Should_have_the_correct_profile_thumbnail()
        {
            Assert.That(actualModel.ProfileThumbnail, Is.EqualTo(studentSchoolAreaLinks.Image(suppliedSchoolId, suppliedStudentUSI, suppliedGender, actualModel.FullName)));
        }

        [Test]
        public virtual void Should_have_the_correct_parent_contact_link()
        {
            Assert.That(actualModel.ParentContactInfoLink, Is.EqualTo(studentSchoolAreaLinks.Information(suppliedSchoolId, suppliedStudentUSI, actualModel.FullName)));
        }

        [Test]
        public virtual void Should_correctly_load_accommodations()
        {
            Assert.That(actualModel.GiftedAndTalented, Is.False);
            Assert.That(actualModel.SpecialEducation, Is.False);
            Assert.That(actualModel.EnglishAsSecondLanguage, Is.False);
            Assert.That(actualModel.BilingualProgram, Is.False);
            Assert.That(actualModel.LimitedEnglishProficiency, Is.False);
            Assert.That(actualModel.LimitedEnglishMonitoredFirstProficiency, Is.False);
            Assert.That(actualModel.LimitedEnglishMonitoredSecondProficiency, Is.False);
            Assert.That(actualModel.Designation, Is.False);
        }

        [Test]
        public virtual void Should_have_all_values_assigned()
        {
            actualModel.EnsureNoDefaultValues(new string[]
                                                  {
                                                      "OverviewModel.LimitedEnglishProficiency",
                                                      "OverviewModel.LimitedEnglishMonitoredFirstProficiency",
                                                      "OverviewModel.LimitedEnglishMonitoredSecondProficiency",
                                                      "OverviewModel.BilingualProgram", 
                                                      "OverviewModel.EnglishAsSecondLanguage",
                                                      "OverviewModel.GiftedAndTalented",
                                                      "OverviewModel.SpecialEducation",
                                                      "OverviewModel.Designation",
                                                  });
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
    public class When_invoking_the_student_overview_resource_base : When_invoking_the_student_overview_resource_base<OverviewRequest, OverviewModel, OverviewService>
    { }

    [TestFixture]
    public class When_loading_student_overview_having_birthday_on_lower_boundary : When_invoking_the_student_overview_resource_base<OverviewRequest, OverviewModel, OverviewService>
    {
        protected int expectedAgeAsOfToday;

        protected override void EstablishContext()
        {
            //We are using DateTime.Today becuase the service has to use Today to be able to calculate the AgeAsOfToday.
            var today = DateTime.Today;

            //The supplied date of birth in this scenario is 10 years before plus one day.
            //Making this only one more day to for it to be his birthday.
            suppliedDateOfBirth = today.AddDays(1).AddYears(-10);


            expectedAgeAsOfToday = (new DateTime(1, 1, 1) + (today - suppliedDateOfBirth)).Year - 1;

            base.EstablishContext();
        }

        [Test]
        public virtual void Should_correctly_calculate_age_as_of_today()
        {
            Assert.That(actualModel.AgeAsOfToday, Is.EqualTo(expectedAgeAsOfToday));
        }
    }

    [TestFixture]
    public class When_loading_student_overview_having_birthday_on_upper_boundary : When_invoking_the_student_overview_resource_base<OverviewRequest, OverviewModel, OverviewService>
    {
        protected int expectedAgeAsOfToday;

        protected override void EstablishContext()
        {
            //We are using DateTime.Today becuase the service has to use Today to be able to calculate the AgeAsOfToday.
            var today = DateTime.Today;

            //Today is his birthday but he was born 10 years ago.
            suppliedDateOfBirth = today.AddYears(-10);

            expectedAgeAsOfToday = today.Year - suppliedDateOfBirth.Year;

            base.EstablishContext();
        }

        [Test]
        public virtual void Should_correctly_calculate_age_as_of_today()
        {
            Assert.That(actualModel.AgeAsOfToday, Is.EqualTo(expectedAgeAsOfToday));
        }
    }

    [TestFixture]
    public class When_loading_student_overview_with_gifted_and_talented_accommodation : When_invoking_the_student_overview_resource_base<OverviewRequest, OverviewModel, OverviewService>
    {
        protected override List<Accommodations> GetSuppliedAccommodations()
        {
            return new List<Accommodations>{Accommodations.GiftedAndTalented};
        }

        [Test]
        public override void Should_correctly_load_accommodations()
        {
            Assert.That(actualModel.GiftedAndTalented, Is.True);
            Assert.That(actualModel.SpecialEducation, Is.False);
            Assert.That(actualModel.EnglishAsSecondLanguage, Is.False);
            Assert.That(actualModel.BilingualProgram, Is.False);
            Assert.That(actualModel.LimitedEnglishProficiency, Is.False);
            Assert.That(actualModel.LimitedEnglishMonitoredFirstProficiency, Is.False);
            Assert.That(actualModel.LimitedEnglishMonitoredSecondProficiency, Is.False);
            Assert.That(actualModel.Designation, Is.False);
        }
    }

    [TestFixture]
    public class When_loading_student_overview_with_special_education_accommodation : When_invoking_the_student_overview_resource_base<OverviewRequest, OverviewModel, OverviewService>
    {
        protected override List<Accommodations> GetSuppliedAccommodations()
        {
            return new List<Accommodations> { Accommodations.SpecialEducation };
        }

        [Test]
        public override void Should_correctly_load_accommodations()
        {
            Assert.That(actualModel.GiftedAndTalented, Is.False);
            Assert.That(actualModel.SpecialEducation, Is.True);
            Assert.That(actualModel.EnglishAsSecondLanguage, Is.False);
            Assert.That(actualModel.BilingualProgram, Is.False);
            Assert.That(actualModel.LimitedEnglishProficiency, Is.False);
            Assert.That(actualModel.LimitedEnglishMonitoredFirstProficiency, Is.False);
            Assert.That(actualModel.LimitedEnglishMonitoredSecondProficiency, Is.False);
            Assert.That(actualModel.Designation, Is.False);
        }
    }

    [TestFixture]
    public class When_loading_student_overview_with_english_as_second_language_accommodation : When_invoking_the_student_overview_resource_base<OverviewRequest, OverviewModel, OverviewService>
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            Expect.Call(studentIndicatorRepository.GetAll()).Return(GetSuppliedStudentIndicator());
        }

        protected override List<Accommodations> GetSuppliedAccommodations()
        {
            return new List<Accommodations> { Accommodations.ESLAndLEP };
        }

        protected virtual IQueryable<StudentIndicator> GetSuppliedStudentIndicator()
        {
            var list = new List<StudentIndicator>
                           {
                               new StudentIndicator
                                   {
                                       StudentUSI = suppliedStudentUSI,
                                       EducationOrganizationId = suppliedLocalEducationAgencyId,
                                       Status = true,
                                       Name = "English as Second Language"
                                   }
                           };
            return list.AsQueryable();
        }

        [Test]
        public override void Should_correctly_load_accommodations()
        {
            Assert.That(actualModel.GiftedAndTalented, Is.False);
            Assert.That(actualModel.SpecialEducation, Is.False);
            Assert.That(actualModel.EnglishAsSecondLanguage, Is.True);
            Assert.That(actualModel.BilingualProgram, Is.False);
            Assert.That(actualModel.LimitedEnglishProficiency, Is.False);
            Assert.That(actualModel.LimitedEnglishMonitoredFirstProficiency, Is.False);
            Assert.That(actualModel.LimitedEnglishMonitoredSecondProficiency, Is.False);
            Assert.That(actualModel.Designation, Is.False);
        }
    }

    [TestFixture]
    public class When_loading_student_overview_with_bilingual_program_accommodation : When_invoking_the_student_overview_resource_base<OverviewRequest, OverviewModel, OverviewService>
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            Expect.Call(studentIndicatorRepository.GetAll()).Return(GetSuppliedStudentIndicator());
        }

        protected override List<Accommodations> GetSuppliedAccommodations()
        {
            return new List<Accommodations> { Accommodations.ESLAndLEP };
        }

        protected virtual IQueryable<StudentIndicator> GetSuppliedStudentIndicator()
        {
            var list = new List<StudentIndicator>
                           {
                               new StudentIndicator
                                   {
                                       StudentUSI = suppliedStudentUSI,
                                       EducationOrganizationId = suppliedLocalEducationAgencyId,
                                       Status = true,
                                       Name = "Bilingual Program"
                                   }
                           };
            return list.AsQueryable();
        }

        [Test]
        public override void Should_correctly_load_accommodations()
        {
            Assert.That(actualModel.GiftedAndTalented, Is.False);
            Assert.That(actualModel.SpecialEducation, Is.False);
            Assert.That(actualModel.EnglishAsSecondLanguage, Is.False);
            Assert.That(actualModel.BilingualProgram, Is.True);
            Assert.That(actualModel.LimitedEnglishProficiency, Is.False);
            Assert.That(actualModel.LimitedEnglishMonitoredFirstProficiency, Is.False);
            Assert.That(actualModel.LimitedEnglishMonitoredSecondProficiency, Is.False);
            Assert.That(actualModel.Designation, Is.False);
        }
    }

    [TestFixture]
    public class When_loading_student_overview_with_designation_accommodation : When_invoking_the_student_overview_resource_base<OverviewRequest, OverviewModel, OverviewService>
    {
        protected override List<Accommodations> GetSuppliedAccommodations()
        {
            return new List<Accommodations> { Accommodations.Designation };
        }

        protected virtual IQueryable<StudentIndicator> GetSuppliedStudentIndicator()
        {
            var list = new List<StudentIndicator>
                           {
                               new StudentIndicator
                                   {
                                       StudentUSI = suppliedStudentUSI,
                                       EducationOrganizationId = suppliedLocalEducationAgencyId,
                                       Status = true,
                                       Name = "504 Designation"
                                   }
                           };
            return list.AsQueryable();
        }

        [Test]
        public override void Should_correctly_load_accommodations()
        {
            Assert.That(actualModel.GiftedAndTalented, Is.False);
            Assert.That(actualModel.SpecialEducation, Is.False);
            Assert.That(actualModel.EnglishAsSecondLanguage, Is.False);
            Assert.That(actualModel.BilingualProgram, Is.False);
            Assert.That(actualModel.LimitedEnglishProficiency, Is.False);
            Assert.That(actualModel.LimitedEnglishMonitoredFirstProficiency, Is.False);
            Assert.That(actualModel.LimitedEnglishMonitoredSecondProficiency, Is.False);
            Assert.That(actualModel.Designation, Is.True);
        }
    }

    [TestFixture]
    public class When_loading_student_overview_with_limited_english_proficiency_accommodation : When_invoking_the_student_overview_resource_base<OverviewRequest, OverviewModel, OverviewService>
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            Expect.Call(studentIndicatorRepository.GetAll()).Return(GetSuppliedStudentIndicator());
        }

        protected override List<Accommodations> GetSuppliedAccommodations()
        {
            return new List<Accommodations> { Accommodations.ESLAndLEP };
        }

        protected virtual IQueryable<StudentIndicator> GetSuppliedStudentIndicator()
        {
            var list = new List<StudentIndicator>
                           {
                               new StudentIndicator
                                   {
                                       StudentUSI = suppliedStudentUSI,
                                       EducationOrganizationId = suppliedLocalEducationAgencyId,
                                       Status = true,
                                       Name = "Limited English Proficiency"
                                   }
                           };
            return list.AsQueryable();
        }

        [Test]
        public override void Should_correctly_load_accommodations()
        {
            Assert.That(actualModel.GiftedAndTalented, Is.False);
            Assert.That(actualModel.SpecialEducation, Is.False);
            Assert.That(actualModel.EnglishAsSecondLanguage, Is.False);
            Assert.That(actualModel.BilingualProgram, Is.False);
            Assert.That(actualModel.LimitedEnglishProficiency, Is.True);
            Assert.That(actualModel.LimitedEnglishMonitoredFirstProficiency, Is.False);
            Assert.That(actualModel.LimitedEnglishMonitoredSecondProficiency, Is.False);
            Assert.That(actualModel.Designation, Is.False);
        }
    }

    [TestFixture]
    public class When_loading_student_overview_with_limited_english_proficiency_monitored_first_accommodation : When_invoking_the_student_overview_resource_base<OverviewRequest, OverviewModel, OverviewService>
    {
        protected override List<Accommodations> GetSuppliedAccommodations()
        {
            return new List<Accommodations> { Accommodations.LEPMonitoredFirst };
        }

        protected virtual IQueryable<StudentIndicator> GetSuppliedStudentIndicator()
        {
            var list = new List<StudentIndicator>
                           {
                               new StudentIndicator
                                   {
                                       StudentUSI = suppliedStudentUSI,
                                       EducationOrganizationId = suppliedLocalEducationAgencyId,
                                       Status = true,
                                       Name = "Limited English Proficiency Monitored 1"
                                   }
                           };
            return list.AsQueryable();
        }

        [Test]
        public override void Should_correctly_load_accommodations()
        {
            Assert.That(actualModel.GiftedAndTalented, Is.False);
            Assert.That(actualModel.SpecialEducation, Is.False);
            Assert.That(actualModel.EnglishAsSecondLanguage, Is.False);
            Assert.That(actualModel.BilingualProgram, Is.False);
            Assert.That(actualModel.LimitedEnglishProficiency, Is.False);
            Assert.That(actualModel.LimitedEnglishMonitoredFirstProficiency, Is.True);
            Assert.That(actualModel.LimitedEnglishMonitoredSecondProficiency, Is.False);
            Assert.That(actualModel.Designation, Is.False);
        }
    }

    [TestFixture]
    public class When_loading_student_overview_with_limited_english_proficiency_monitored_second_accommodation : When_invoking_the_student_overview_resource_base<OverviewRequest, OverviewModel, OverviewService>
    {
        protected override List<Accommodations> GetSuppliedAccommodations()
        {
            return new List<Accommodations> { Accommodations.LEPMonitoredSecond };
        }

        protected virtual IQueryable<StudentIndicator> GetSuppliedStudentIndicator()
        {
            var list = new List<StudentIndicator>
                           {
                               new StudentIndicator
                                   {
                                       StudentUSI = suppliedStudentUSI,
                                       EducationOrganizationId = suppliedLocalEducationAgencyId,
                                       Status = true,
                                       Name = "Limited English Proficiency Monitored 2"
                                   }
                           };
            return list.AsQueryable();
        }

        [Test]
        public override void Should_correctly_load_accommodations()
        {
            Assert.That(actualModel.GiftedAndTalented, Is.False);
            Assert.That(actualModel.SpecialEducation, Is.False);
            Assert.That(actualModel.EnglishAsSecondLanguage, Is.False);
            Assert.That(actualModel.BilingualProgram, Is.False);
            Assert.That(actualModel.LimitedEnglishProficiency, Is.False);
            Assert.That(actualModel.LimitedEnglishMonitoredFirstProficiency, Is.False);
            Assert.That(actualModel.LimitedEnglishMonitoredSecondProficiency, Is.True);
            Assert.That(actualModel.Designation, Is.False);
        }
    }
}
