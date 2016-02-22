// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency.Information;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using NUnit.Framework;
using Rhino.Mocks;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Testing;
using InformationRequest = EdFi.Dashboards.Resources.LocalEducationAgency.InformationRequest;
using InformationService = EdFi.Dashboards.Resources.LocalEducationAgency.InformationService;

namespace EdFi.Dashboards.Resources.Tests.LocalEducationAgency
{
    [TestFixture]
    public abstract class When_building_local_education_agency_information_model<TRequest, TResponse, TService, TAdministrator, TAccountability, TAccountabilityRatings, TIndicatorPopulation, TProgramPopulation, TStudentDemographics> : TestFixtureBase
        where TRequest : InformationRequest, new()
        where TResponse : InformationModel, new()
        where TAdministrator : Administrator, new()
        where TAccountability : AttributeItem<string>, new()
        where TAccountabilityRatings : AttributeItem<string>, new()
        where TIndicatorPopulation : AttributeItemWithUrl<decimal?>, new()
        where TProgramPopulation : AttributeItemWithUrl<decimal?>, new()
        where TStudentDemographics : AttributeItemWithTrend<decimal?>, new()
        where TService : InformationServiceBase<TRequest, TResponse, TAdministrator, TAccountability, TAccountabilityRatings, TIndicatorPopulation, TProgramPopulation, TStudentDemographics>, new()
    {
        protected IRepository<LocalEducationAgencyInformation> localEducationAgencyInformationRepository;
        protected IRepository<LocalEducationAgencyStudentDemographic> studentDemographicsRepository;
        protected IRepository<LocalEducationAgencyIndicatorPopulation> indicatorPopulationRepository;
        protected IRepository<LocalEducationAgencyProgramPopulation> programPopulationRepository;
        protected IRepository<LocalEducationAgencyAccountabilityByCategory> accountabilityByCategoryRepository;
        protected IRepository<LocalEducationAgencyAccountabilityInformation> accountabilityRepository;
        protected IRepository<LocalEducationAgencyAdministrationInformation> administrationRepository;
        protected IRepository<LocalEducationAgencyCharacteristicsInformation> characteristicRepository;
        protected ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks;
        protected ISchoolCategoryProvider schoolCategoryProvider;
        protected ICurrentUserClaimInterrogator currentUserClaimInterrogator;

        protected TResponse actualModel;

        protected const int suppliedLocalEducationAgencyId = 2332;
        protected const decimal suppliedHispanicValue = .011m;
        protected const decimal suppliedFemaleValue = .502m;
        protected const decimal suppliedMaleValue = .499m;

        protected IQueryable<LocalEducationAgencyInformation> suppliedLocalEducationAgencyInfo;
        protected IQueryable<LocalEducationAgencyStudentDemographic> suppliedStudentDemographics;
        protected IQueryable<LocalEducationAgencyIndicatorPopulation> suppliedIndicatorPopulation;
        protected IQueryable<LocalEducationAgencyProgramPopulation> suppliedProgramPopulation;
        protected IQueryable<LocalEducationAgencyAccountabilityByCategory> suppliedAccountabilityByCategory;
        protected IQueryable<LocalEducationAgencyAccountabilityInformation> suppliedAccountability;
        protected IQueryable<LocalEducationAgencyAdministrationInformation> suppliedAdministration;
        protected IQueryable<LocalEducationAgencyCharacteristicsInformation> suppliedCharacteristic;
        protected LocalEducationAgencyInformation selectedLocalEducationAgencyInformation;

        protected override void EstablishContext()
        {
            localEducationAgencyInformationRepository = mocks.StrictMock<IRepository<LocalEducationAgencyInformation>>();
            studentDemographicsRepository = mocks.StrictMock<IRepository<LocalEducationAgencyStudentDemographic>>();
            indicatorPopulationRepository = mocks.StrictMock<IRepository<LocalEducationAgencyIndicatorPopulation>>();
            programPopulationRepository = mocks.StrictMock<IRepository<LocalEducationAgencyProgramPopulation>>();
            accountabilityByCategoryRepository = mocks.StrictMock<IRepository<LocalEducationAgencyAccountabilityByCategory>>();
            accountabilityRepository = mocks.StrictMock<IRepository<LocalEducationAgencyAccountabilityInformation>>();
            administrationRepository = mocks.StrictMock<IRepository<LocalEducationAgencyAdministrationInformation>>();
            characteristicRepository = mocks.StrictMock<IRepository<LocalEducationAgencyCharacteristicsInformation>>();
            localEducationAgencyAreaLinks = new LocalEducationAgencyAreaLinksFake();
            schoolCategoryProvider = mocks.StrictMock<ISchoolCategoryProvider>();
            currentUserClaimInterrogator = mocks.StrictMock<ICurrentUserClaimInterrogator>();

            suppliedLocalEducationAgencyInfo = GetSuppliedLocalEducationAgencyInfo();
            suppliedStudentDemographics = GetSuppliedStudentDemographics();
            suppliedIndicatorPopulation = GetSuppliedIndicatorPopulation();
            suppliedProgramPopulation = GetSuppliedProgramPopulation();
            suppliedAccountabilityByCategory = GetSuppliedAccountabilityByCategory();
            suppliedAccountability = GetSuppliedAccountabilityInformation();
            suppliedAdministration = GetSuppliedAdministrationInformation();
            suppliedCharacteristic = GetSuppliedCharacteristicsInformation();
            selectedLocalEducationAgencyInformation = suppliedLocalEducationAgencyInfo.ElementAt(1);

            Expect.Call(localEducationAgencyInformationRepository.GetAll()).Return(suppliedLocalEducationAgencyInfo);
            Expect.Call(studentDemographicsRepository.GetAll()).Return(suppliedStudentDemographics);
            Expect.Call(indicatorPopulationRepository.GetAll()).Return(suppliedIndicatorPopulation);
            Expect.Call(programPopulationRepository.GetAll()).Return(suppliedProgramPopulation);
            Expect.Call(accountabilityByCategoryRepository.GetAll()).Return(suppliedAccountabilityByCategory);
            Expect.Call(accountabilityRepository.GetAll()).Return(suppliedAccountability);
            Expect.Call(administrationRepository.GetAll()).Return(suppliedAdministration);
            Expect.Call(characteristicRepository.GetAll()).Return(suppliedCharacteristic);

            Expect.Call(schoolCategoryProvider.GetSchoolCategoryPriorityForSorting("High School")).Repeat.Any().Return(1);
            Expect.Call(schoolCategoryProvider.GetSchoolCategoryPriorityForSorting("Junior High School")).Repeat.Any().Return(2);
            Expect.Call(schoolCategoryProvider.GetSchoolCategoryPriorityForSorting("Middle School")).Repeat.Any().Return(3);
            Expect.Call(schoolCategoryProvider.GetSchoolCategoryPriorityForSorting("Elementary School")).Repeat.Any().Return(4);
            Expect.Call(schoolCategoryProvider.GetSchoolCategoryPriorityForSorting("Ungraded")).Repeat.Any().Return(6);
        }

        protected IQueryable<LocalEducationAgencyInformation> GetSuppliedLocalEducationAgencyInfo()
        {
            var list = new List<LocalEducationAgencyInformation> { 
                            new LocalEducationAgencyInformation { LocalEducationAgencyId = 2, Name = "My LocalEducationAgency"},
                            new LocalEducationAgencyInformation { LocalEducationAgencyId = suppliedLocalEducationAgencyId, Name = "My LocalEducationAgency", AddressLine1 = "123 Fake St.", AddressLine2 = "Building Z", AddressLine3 = "Suite 99", City = "Anywhere", State = "ZZ", ZipCode = "99999", TelephoneNumber = "(123) 456-7890", FaxNumber = "(234) 567-8901", WebSite = @"http://myisd.edu"},
                            new LocalEducationAgencyInformation { LocalEducationAgencyId = 3, Name = "wrong data"},
                        };
            return list.AsQueryable();
        }

        protected IQueryable<LocalEducationAgencyStudentDemographic> GetSuppliedStudentDemographics()
        {
            var list = new List<LocalEducationAgencyStudentDemographic> {
                new LocalEducationAgencyStudentDemographic{ LocalEducationAgencyId = 2, Attribute = "wrong data", Value = .066m},
                new LocalEducationAgencyStudentDemographic{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "Hispanic/Latino", Value = suppliedHispanicValue, TrendDirection = 1, DisplayOrder = 1},
                new LocalEducationAgencyStudentDemographic{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "Female", Value = suppliedFemaleValue, TrendDirection = 0, DisplayOrder = 2},
                new LocalEducationAgencyStudentDemographic{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "Male", Value = suppliedMaleValue, TrendDirection = -1, DisplayOrder = 3},
                new LocalEducationAgencyStudentDemographic{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "Expected 4.8", Value = .8m, TrendDirection = 1, DisplayOrder = 7},
                new LocalEducationAgencyStudentDemographic{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "Expected 2.8", Value = .81m, TrendDirection = 1, DisplayOrder = 4},
                new LocalEducationAgencyStudentDemographic{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "Expected 3.8", Value = .82m, TrendDirection = 1, DisplayOrder = 5},
                new LocalEducationAgencyStudentDemographic{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "Expected 1.8", Value = .83m, TrendDirection = 1, DisplayOrder = 6},
            };
            return list.AsQueryable();
        }

        protected IQueryable<LocalEducationAgencyIndicatorPopulation> GetSuppliedIndicatorPopulation()
        {
            var list = new List<LocalEducationAgencyIndicatorPopulation>  {
                new LocalEducationAgencyIndicatorPopulation{ LocalEducationAgencyId = 2, Attribute = "wrong data", Value = .03m},
                new LocalEducationAgencyIndicatorPopulation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "Expected 4.7", Value = .7m, DisplayOrder = 3},
                new LocalEducationAgencyIndicatorPopulation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "Expected 2.7", Value = .71m, DisplayOrder = 1},
                new LocalEducationAgencyIndicatorPopulation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "Expected 3.7", Value = .72m, DisplayOrder = 2},
                new LocalEducationAgencyIndicatorPopulation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "Expected 1.7", Value = .73m, DisplayOrder = 4},
            };
            return list.AsQueryable();
        }

        protected IQueryable<LocalEducationAgencyProgramPopulation> GetSuppliedProgramPopulation()
        {
            var list = new List<LocalEducationAgencyProgramPopulation>  {
                new LocalEducationAgencyProgramPopulation{ LocalEducationAgencyId = 2, Attribute = "wrong data", Value = .05m},
                new LocalEducationAgencyProgramPopulation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "Expected 4.6", Value = .6m, DisplayOrder = 6},
                new LocalEducationAgencyProgramPopulation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "Expected 2.6", Value = .61m, DisplayOrder = 2},
                new LocalEducationAgencyProgramPopulation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "Expected 3.6", Value = .62m, DisplayOrder = 9},
                new LocalEducationAgencyProgramPopulation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "Expected 1.6", Value = .63m, DisplayOrder = 1},
            };
            return list.AsQueryable();
        }

        protected IQueryable<LocalEducationAgencyAccountabilityByCategory> GetSuppliedAccountabilityByCategory()
        {
            var list = new List<LocalEducationAgencyAccountabilityByCategory>
                            {
                                new LocalEducationAgencyAccountabilityByCategory{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "Category 3", Value = "b3", DisplayOrder = 3},
                                new LocalEducationAgencyAccountabilityByCategory{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "Category 1", Value = "b1", DisplayOrder = 1},
                                new LocalEducationAgencyAccountabilityByCategory{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "Category 2", Value = "b2", DisplayOrder = 2},
                                new LocalEducationAgencyAccountabilityByCategory{ LocalEducationAgencyId = suppliedLocalEducationAgencyId + 1, Attribute = "Category 4", Value = "b4", DisplayOrder = 1},
                            };
            return list.AsQueryable();
        }

        protected IQueryable<LocalEducationAgencyAccountabilityInformation> GetSuppliedAccountabilityInformation()
        {
            var list = new List<LocalEducationAgencyAccountabilityInformation>
                            {
                                new LocalEducationAgencyAccountabilityInformation { LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "att3", Value = "v1.3", DisplayOrder = 3 },
                                new LocalEducationAgencyAccountabilityInformation { LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "att1", Value = "v1.1", DisplayOrder = 1 },
                                new LocalEducationAgencyAccountabilityInformation { LocalEducationAgencyId = suppliedLocalEducationAgencyId + 1, Attribute = "att1", Value = "v2", DisplayOrder = 1 },
                            };
            return list.AsQueryable();
        }

        protected IQueryable<LocalEducationAgencyAdministrationInformation> GetSuppliedAdministrationInformation()
        {
            var list = new List<LocalEducationAgencyAdministrationInformation>
                                                {
                                                    new LocalEducationAgencyAdministrationInformation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Role = "Role 1", Name = "name 2", DisplayOrder = 3},
                                                    new LocalEducationAgencyAdministrationInformation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Role = "Role 2", Name = "name 3", DisplayOrder = 1},
                                                    new LocalEducationAgencyAdministrationInformation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Role = "Role 2", Name = "name 4", DisplayOrder = 2},
                                                    new LocalEducationAgencyAdministrationInformation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId + 1, Role = "Role 2", Name = "name 5", DisplayOrder =  4},
                                                    new LocalEducationAgencyAdministrationInformation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId + 1, Role = "Role 3", Name = "name 6", DisplayOrder = 5},
                                                };
            return list.AsQueryable();
        }

        protected IQueryable<LocalEducationAgencyCharacteristicsInformation> GetSuppliedCharacteristicsInformation()
        {
            var list = new List<LocalEducationAgencyCharacteristicsInformation>
                                                {
                                                    new LocalEducationAgencyCharacteristicsInformation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "DE", Value = "234", CharacteristicsId = 1},
                                                    new LocalEducationAgencyCharacteristicsInformation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "High School", Value = "10", CharacteristicsId = 2},
                                                    new LocalEducationAgencyCharacteristicsInformation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "Middle School", Value = "20", CharacteristicsId = 2},
                                                    new LocalEducationAgencyCharacteristicsInformation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "Junior High School", Value = "15", CharacteristicsId = 2},
                                                    new LocalEducationAgencyCharacteristicsInformation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "Ungraded", Value = "10", CharacteristicsId = 2},
                                                    new LocalEducationAgencyCharacteristicsInformation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "TIC", Value = "10", CharacteristicsId = 3},
                                                    new LocalEducationAgencyCharacteristicsInformation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "LES", Value = ".23", CharacteristicsId = 4},
                                                    new LocalEducationAgencyCharacteristicsInformation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "High School", Value = "10:3", CharacteristicsId = 5},
                                                    new LocalEducationAgencyCharacteristicsInformation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "Elementary School", Value = "15:2", CharacteristicsId = 5},
                                                    new LocalEducationAgencyCharacteristicsInformation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, Attribute = "Ungraded", Value = "100:2", CharacteristicsId = 5},
                                                };
            return list.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new TService
                              {
                                  LocalEducationAgencyInformationRepository = localEducationAgencyInformationRepository,
                                  StudentDemographicsRepository = studentDemographicsRepository,
                                  IndicatorPopulationRepository = indicatorPopulationRepository,
                                  ProgramPopulationRepository = programPopulationRepository,
                                  AccountabilityByCategoryRepository = accountabilityByCategoryRepository,
                                  AccountabilityRepository = accountabilityRepository,
                                  AdministrationRepository = administrationRepository,
                                  CharacteristicRepository = characteristicRepository,
                                  LocalEducationAgencyAreaLinks = localEducationAgencyAreaLinks,
                                  SchoolCategoryProvider = schoolCategoryProvider,
                                  CurrentUserClaimInterrogator = currentUserClaimInterrogator
                              };

            actualModel = service.Get(new TRequest { LocalEducationAgencyId = suppliedLocalEducationAgencyId });
        }

        [Test]
        public virtual void Should_create_model_correctly()
        {
            Assert.That(actualModel.Name, Is.EqualTo(selectedLocalEducationAgencyInformation.Name));
            Assert.That(actualModel.LocalEducationAgencyId, Is.EqualTo(selectedLocalEducationAgencyInformation.LocalEducationAgencyId));
            Assert.That(actualModel.AddressLines[0], Is.EqualTo(selectedLocalEducationAgencyInformation.AddressLine1));
            Assert.That(actualModel.AddressLines[1], Is.EqualTo(selectedLocalEducationAgencyInformation.AddressLine2));
            Assert.That(actualModel.AddressLines[2], Is.EqualTo(selectedLocalEducationAgencyInformation.AddressLine3));
            Assert.That(actualModel.City, Is.EqualTo(selectedLocalEducationAgencyInformation.City));
            Assert.That(actualModel.State, Is.EqualTo(selectedLocalEducationAgencyInformation.State));
            Assert.That(actualModel.ZipCode, Is.EqualTo(selectedLocalEducationAgencyInformation.ZipCode));
            Assert.That(actualModel.TelephoneNumbers[0].Type, Is.EqualTo("mainline"));
            Assert.That(actualModel.TelephoneNumbers[1].Type, Is.EqualTo("fax"));
            Assert.That(actualModel.TelephoneNumbers[0].Number, Is.EqualTo(selectedLocalEducationAgencyInformation.TelephoneNumber));
            Assert.That(actualModel.TelephoneNumbers[1].Number, Is.EqualTo(selectedLocalEducationAgencyInformation.FaxNumber));
            Assert.That(actualModel.WebSite, Is.EqualTo(selectedLocalEducationAgencyInformation.WebSite));
            Assert.That(actualModel.ProfileThumbnail, Is.EqualTo(localEducationAgencyAreaLinks.Image(suppliedLocalEducationAgencyId)));
            Assert.That(actualModel.Url, Is.EqualTo(localEducationAgencyAreaLinks.Information(suppliedLocalEducationAgencyId)));
        }

        [Test]
        public virtual void Should_load_demographics_correctly()
        {
            Assert.That(actualModel.StudentDemographics, Is.Not.Null);
            Assert.That(actualModel.StudentDemographics.Female.Value, Is.EqualTo(suppliedFemaleValue));
            Assert.That(actualModel.StudentDemographics.Female.Trend, Is.EqualTo(TrendDirection.Unchanged));
            Assert.That(actualModel.StudentDemographics.Male.Value, Is.EqualTo(suppliedMaleValue));
            Assert.That(actualModel.StudentDemographics.Male.Trend, Is.EqualTo(TrendDirection.Decreasing));
            Assert.That(actualModel.StudentDemographics.ByEthnicity, Is.Not.Null);
            Assert.That(actualModel.StudentDemographics.ByEthnicity.Count(), Is.EqualTo(1));
            Assert.That(actualModel.StudentDemographics.ByEthnicity[0].Attribute, Is.EqualTo("Hispanic/Latino"));
            Assert.That(actualModel.StudentDemographics.ByEthnicity[0].Value, Is.EqualTo(suppliedHispanicValue));
            Assert.That(actualModel.StudentDemographics.ByEthnicity[0].Trend, Is.EqualTo(TrendDirection.Increasing));
            Assert.That(actualModel.StudentDemographics.ByRace, Is.Not.Null);
            Assert.That(actualModel.StudentDemographics.ByRace.Count(), Is.EqualTo(4));

            var suppliedRaceList = GetSuppliedStudentDemographics().Where(x => x.Attribute.StartsWith("Expected ")).OrderBy(x => x.DisplayOrder).ToList();
            int suppliedRacePosition = 0;
            foreach (var race in actualModel.StudentDemographics.ByRace)
            {
                var suppliedRace = suppliedRaceList[suppliedRacePosition++];
                Assert.That(race.Attribute, Is.EqualTo(suppliedRace.Attribute), "Sort order is not correct.");
                Assert.That(race.Value, Is.EqualTo(suppliedRace.Value));
            }
        }

        [Test]
        public virtual void Should_load_indicator_population_correctly()
        {
            Assert.That(actualModel.StudentIndicatorPopulation, Is.Not.Null);
            Assert.That(actualModel.StudentIndicatorPopulation.Count(), Is.EqualTo(4));

            var suppliedList = GetSuppliedIndicatorPopulation().Where(x => x.Attribute.StartsWith("Expected ")).OrderBy(x => x.DisplayOrder).ToList();
            int suppliedPosition = 0;
            foreach (var race in actualModel.StudentIndicatorPopulation)
            {
                var supplied = suppliedList[suppliedPosition++];
                Assert.That(race.Attribute, Is.EqualTo(supplied.Attribute), "Sort order is not correct.");
                Assert.That(race.Value, Is.EqualTo(supplied.Value));
            }
        }

        [Test]
        public virtual void Should_load_program_population_correctly()
        {
            Assert.That(actualModel.StudentsByProgram, Is.Not.Null);
            Assert.That(actualModel.StudentsByProgram.Count(), Is.EqualTo(4));

            var suppliedList = GetSuppliedProgramPopulation().Where(x => x.Attribute.StartsWith("Expected ")).OrderBy(x => x.DisplayOrder).ToList();
            int suppliedPosition = 0;
            foreach (var race in actualModel.StudentsByProgram)
            {
                var supplied = suppliedList[suppliedPosition++];
                Assert.That(race.Attribute, Is.EqualTo(supplied.Attribute), "Sort order is not correct.");
                Assert.That(race.Value, Is.EqualTo(supplied.Value));
            }
        }

        [Test]
        public virtual void Should_load_local_education_agency_administration_correctly()
        {
            Assert.That(actualModel.Administrators.Count, Is.EqualTo(3));

            Assert.That(actualModel.Administrators[0].Role, Is.EqualTo("Role 2"));
            Assert.That(actualModel.Administrators[0].Name, Is.EqualTo("name 3"));

            Assert.That(actualModel.Administrators[1].Role, Is.EqualTo("Role 2"));
            Assert.That(actualModel.Administrators[1].Name, Is.EqualTo("name 4"));

            Assert.That(actualModel.Administrators[2].Role, Is.EqualTo("Role 1"));
            Assert.That(actualModel.Administrators[2].Name, Is.EqualTo("name 2"));
        }

        [Test]
        public virtual void Should_load_local_education_agency_accountability_correctly()
        {
            Assert.That(actualModel.Accountability.Count, Is.EqualTo(2));
            Assert.That(actualModel.Accountability[0].Attribute, Is.EqualTo("att1"));
            Assert.That(actualModel.Accountability[0].Value, Is.EqualTo("v1.1"));

            Assert.That(actualModel.Accountability[1].Attribute, Is.EqualTo("att3"));
            Assert.That(actualModel.Accountability[1].Value, Is.EqualTo("v1.3"));
        }

        [Test]
        public virtual void Should_load_local_education_agency_accountability_by_category_correctly()
        {
            Assert.That(actualModel.SchoolAccountabilityRatings.Count, Is.EqualTo(3));
            Assert.That(actualModel.SchoolAccountabilityRatings[0].Attribute, Is.EqualTo("Category 1"));
            Assert.That(actualModel.SchoolAccountabilityRatings[0].Value, Is.EqualTo("b1"));

            Assert.That(actualModel.SchoolAccountabilityRatings[1].Attribute, Is.EqualTo("Category 2"));
            Assert.That(actualModel.SchoolAccountabilityRatings[1].Value, Is.EqualTo("b2"));

            Assert.That(actualModel.SchoolAccountabilityRatings[2].Attribute, Is.EqualTo("Category 3"));
            Assert.That(actualModel.SchoolAccountabilityRatings[2].Value, Is.EqualTo("b3"));
        }

        [Test]
        public virtual void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel.EnsureNoDefaultValues(new[] { "InformationModel.ResourceUrl", "InformationModel.StudentDemographics.Female.Trend" });
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

    public abstract class LocalEducationAgencyInformationResourceBase : When_building_local_education_agency_information_model<InformationRequest, InformationModel, InformationService, Administrator, AttributeItem<string>, AttributeItem<string>, AttributeItemWithUrl<decimal?>, AttributeItemWithUrl<decimal?>, AttributeItemWithTrend<decimal?>>
    { }

    public class When_getting_local_education_agency_information_resource_as_administrator : LocalEducationAgencyInformationResourceBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewOperationalDashboard, suppliedLocalEducationAgencyId)).Repeat.Any().Return(true);
            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllMetrics, suppliedLocalEducationAgencyId)).Repeat.Any().Return(true);
        }

        [Test]
        public void Should_have_urls_on_demographic_attributes()
        {
            Assert.That(actualModel.StudentDemographics.Female.Url, Is.EqualTo(localEducationAgencyAreaLinks.StudentDemographicList(suppliedLocalEducationAgencyId, "Female")));
            Assert.That(actualModel.StudentDemographics.Male.Url, Is.EqualTo(localEducationAgencyAreaLinks.StudentDemographicList(suppliedLocalEducationAgencyId, "Male")));
            foreach (var race in actualModel.StudentDemographics.ByRace)
            {
                Assert.That(race.Url, Is.EqualTo(localEducationAgencyAreaLinks.StudentDemographicList(suppliedLocalEducationAgencyId, race.Attribute)));
            }
            foreach (var ethnicity in actualModel.StudentDemographics.ByEthnicity)
            {
                Assert.That(ethnicity.Url, Is.EqualTo(localEducationAgencyAreaLinks.StudentDemographicList(suppliedLocalEducationAgencyId, ethnicity.Attribute)));
            }
            foreach (var program in actualModel.StudentsByProgram)
            {
                Assert.That(program.Url, Is.EqualTo(localEducationAgencyAreaLinks.StudentDemographicList(suppliedLocalEducationAgencyId, program.Attribute)));
            }
            foreach (var indicator in actualModel.StudentIndicatorPopulation)
            {
                Assert.That(indicator.Url, Is.EqualTo(localEducationAgencyAreaLinks.StudentDemographicList(suppliedLocalEducationAgencyId, indicator.Attribute)));
            }
        }
    }

    public class When_getting_local_education_agency_information_resource_as_staff : LocalEducationAgencyInformationResourceBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewOperationalDashboard, suppliedLocalEducationAgencyId)).Repeat.Any().Return(false);
            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllMetrics, suppliedLocalEducationAgencyId)).Repeat.Any().Return(false);
        }

        [Test]
        public override void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel.EnsureNoDefaultValues(new[] { "InformationModel.ResourceUrl", 
                                                        "InformationModel.StudentDemographics.Female.Trend" ,
                                                        "InformationModel.StudentDemographics.Female.Url",
                                                        "InformationModel.StudentDemographics.Male.Url",
                                                        "InformationModel.StudentDemographics.ByRace[0].Url",
                                                        "InformationModel.StudentDemographics.ByRace[1].Url",
                                                        "InformationModel.StudentDemographics.ByRace[2].Url",
                                                        "InformationModel.StudentDemographics.ByRace[3].Url",
                                                        "InformationModel.StudentDemographics.ByEthnicity[0].Url",
                                                        "InformationModel.StudentsByProgram[0].Url",
                                                        "InformationModel.StudentsByProgram[1].Url",
                                                        "InformationModel.StudentsByProgram[2].Url",
                                                        "InformationModel.StudentsByProgram[3].Url",
                                                        "InformationModel.StudentIndicatorPopulation[0].Url",
                                                        "InformationModel.StudentIndicatorPopulation[1].Url",
                                                        "InformationModel.StudentIndicatorPopulation[2].Url",
                                                        "InformationModel.StudentIndicatorPopulation[3].Url",
                                                        "InformationModel.LateEnrollmentStudents.Url"});
        }        
    }
}
