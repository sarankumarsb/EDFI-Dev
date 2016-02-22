// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.School.Information;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.School
{
    [TestFixture]
    public abstract class When_getting_school_information<TRequest, TResponse, TService, TAccountability, TSchoolAdministration, TGradePopulation, TGradePopulationIndicators, TStudentDemographics, TProgramPopulation, TFeederSchools, TGraduationPlan> : TestFixtureBase
        where TRequest : InformationRequest, new()
        where TResponse : InformationModel, new()
        where TAccountability : AttributeItem<string>, new()
        where TSchoolAdministration : Administrator, new()
        where TGradePopulation : AttributeItemWithTrend<decimal?>, new()
        where TGradePopulationIndicators : AttributeItemWithTrend<decimal?>, new()
        where TStudentDemographics : AttributeItemWithTrend<decimal?>, new()
        where TProgramPopulation : AttributeItemWithTrend<decimal?>, new()
        where TFeederSchools : AttributeItemWithTrend<decimal?>, new()
        where TGraduationPlan : AttributeItemWithTrend<decimal?>, new()
        where TService : InformationServiceBase<TRequest, TResponse, TAccountability, TSchoolAdministration, TGradePopulation, TGradePopulationIndicators, TStudentDemographics, TProgramPopulation, TFeederSchools, TGraduationPlan>, new()
    {
        //The Injected Dependencies.
        protected IRepository<SchoolInformation> schoolInformationRepository;
        protected IRepository<SchoolAdministrationInformation> schoolAdministrationInformationRepository;
        protected IRepository<SchoolGradePopulation> schoolGradePopulationRepository;
        protected IRepository<SchoolIndicatorPopulation> schoolIndicatorPopulationRepository;
        protected IRepository<SchoolAccountabilityInformation> schoolAccountabilityInformationRepository;
        protected IRepository<SchoolProgramPopulation> schoolProgramPopulationRepository;
        protected IRepository<SchoolStudentDemographic> schoolStudentDemographicRepository;
        protected IRepository<SchoolFeederSchool> schoolFeederSchoolRepository;
        protected ISchoolCategoryProvider schoolCategoryProvider;
        protected IDomainSpecificMetricNodeResolver domainSpecificMetricNodeResolver;
        protected IDomainMetricService<SchoolMetricInstanceSetRequest> domainMetricService;
        protected ISchoolAreaLinks schoolAreaLinks;
        protected ICurrentUserClaimInterrogator currentUserClaimInterrogator;

        //The Actual Model.
        protected TResponse actualModel;
        protected const int suppliedSchoolId1 = 1;
        protected const int suppliedHighSchoolGraduationMetricVariantId = 999;
        protected const decimal suppliedHispanicValue = .011m;
        protected const decimal suppliedFemaleValue = .502m;
        protected const decimal suppliedMaleValue = .499m;
        protected const SchoolCategory suppliedSchoolCategory = SchoolCategory.HighSchool;
        protected Guid suppliedMetricInstanceSetKey = new Guid("11111111-1111-1111-1111-111111111111");
        protected SchoolMetricInstanceSetRequest suppliedRequest = new SchoolMetricInstanceSetRequest { SchoolId = suppliedSchoolId1, MetricVariantId = suppliedHighSchoolGraduationMetricVariantId };

        //The supplied Data models.
        protected IQueryable<SchoolInformation> suppliedSchoolInformationData;
        protected IQueryable<SchoolAdministrationInformation> suppliedSchoolAdministrationInformationData;
        protected IQueryable<SchoolAccountabilityInformation> suppliedSchoolAccountabilityInformationData;
        protected IQueryable<SchoolGradePopulation> suppliedSchoolGradePopulationData;
        protected IQueryable<SchoolIndicatorPopulation> suppliedSchoolIndicatorPopulationData;
        protected IQueryable<SchoolStudentDemographic> suppliedSchoolStudentDemographicData;
        protected IQueryable<SchoolProgramPopulation> suppliedSchoolProgramPopulationData;
        protected IQueryable<SchoolFeederSchool> suppliedSchoolFeederSchoolData;
        protected IQueryable<SchoolStudentDemographic> suppliedStudentDemographics;
        protected ContainerMetric suppliedHighSchoolGraduationPlanMetric;
        protected TService service;


        protected override void EstablishContext()
        {
            //Prepare supplied data collections
            schoolAreaLinks = new SchoolAreaLinksFake();
            suppliedSchoolInformationData = GetSuppliedSchoolInformation();
            suppliedSchoolAdministrationInformationData = GetSuppliedSchoolAdministration();
            suppliedSchoolAccountabilityInformationData = GetSuppliedSchoolAccountability();
            suppliedSchoolGradePopulationData = GetSuppliedSchoolGradePopulation();
            suppliedSchoolIndicatorPopulationData = GetSuppliedSchoolIndicatorPopulation();
            suppliedSchoolStudentDemographicData = GetSuppliedStudentDemographics();
            suppliedSchoolProgramPopulationData = GetSuppliedSchoolProgramPopulation();
            suppliedSchoolFeederSchoolData = GetSuppliedSchoolFeederSchool();
            suppliedHighSchoolGraduationPlanMetric = GetGraduationPlanContainerMetric();


            //Set up the mocks
            schoolInformationRepository = mocks.StrictMock<IRepository<SchoolInformation>>();
            schoolAdministrationInformationRepository = mocks.StrictMock<IRepository<SchoolAdministrationInformation>>();
            schoolGradePopulationRepository = mocks.StrictMock<IRepository<SchoolGradePopulation>>();
            schoolIndicatorPopulationRepository = mocks.StrictMock<IRepository<SchoolIndicatorPopulation>>();
            schoolAccountabilityInformationRepository = mocks.StrictMock<IRepository<SchoolAccountabilityInformation>>();
            schoolProgramPopulationRepository = mocks.StrictMock<IRepository<SchoolProgramPopulation>>();
            schoolStudentDemographicRepository = mocks.StrictMock<IRepository<SchoolStudentDemographic>>();
            schoolFeederSchoolRepository = mocks.StrictMock<IRepository<SchoolFeederSchool>>();
            schoolCategoryProvider = mocks.StrictMock<ISchoolCategoryProvider>();
            domainSpecificMetricNodeResolver = mocks.StrictMock<IDomainSpecificMetricNodeResolver>();
            domainMetricService = mocks.StrictMock<IDomainMetricService<SchoolMetricInstanceSetRequest>>();
            currentUserClaimInterrogator = mocks.StrictMock<ICurrentUserClaimInterrogator>();

            //Set expectations
            Expect.Call(schoolInformationRepository.GetAll()).Return(suppliedSchoolInformationData);
            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId1)).Return(suppliedSchoolCategory);
            Expect.Call(schoolAdministrationInformationRepository.GetAll()).Return(suppliedSchoolAdministrationInformationData);
            Expect.Call(schoolAccountabilityInformationRepository.GetAll()).Return(suppliedSchoolAccountabilityInformationData);
            Expect.Call(schoolGradePopulationRepository.GetAll()).Return(suppliedSchoolGradePopulationData);
            Expect.Call(schoolIndicatorPopulationRepository.GetAll()).Return(suppliedSchoolIndicatorPopulationData);
            Expect.Call(schoolStudentDemographicRepository.GetAll()).Return(suppliedSchoolStudentDemographicData);
            Expect.Call(schoolProgramPopulationRepository.GetAll()).Return(suppliedSchoolProgramPopulationData);
            Expect.Call(schoolFeederSchoolRepository.GetAll()).Return(suppliedSchoolFeederSchoolData);

            Expect.Call(domainSpecificMetricNodeResolver.GetSchoolHighSchoolGraduationPlan()).Return(new MetricMetadataNode(new TestMetricMetadataTree()) { MetricId = 1, MetricVariantId = suppliedHighSchoolGraduationMetricVariantId });

            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllStudents, suppliedSchoolId1)).Repeat.Any().Return(true);

            Expect.Call(domainMetricService.Get(null))
                .Constraints(
                    new ActionConstraint<SchoolMetricInstanceSetRequest>(x =>
                    {
                        Assert.That(x.SchoolId == suppliedSchoolId1);
                        Assert.That(x.MetricVariantId == suppliedHighSchoolGraduationMetricVariantId);
                    })
                ).Return(new MetricTree(suppliedHighSchoolGraduationPlanMetric));

            service = new TService
            {
                SchoolInformationRepository = schoolInformationRepository,
                SchoolAdministrationInformationRepository = schoolAdministrationInformationRepository,
                SchoolGradePopulationRepository = schoolGradePopulationRepository,
                SchoolIndicatorPopulationRepository = schoolIndicatorPopulationRepository,
                SchoolAccountabilityInformationRepository = schoolAccountabilityInformationRepository,
                SchoolProgramPopulationRepository = schoolProgramPopulationRepository,
                SchoolStudentDemographicRepository = schoolStudentDemographicRepository,
                SchoolFeederSchoolRepository = schoolFeederSchoolRepository,
                DomainSpecificMetricNodeResolver = domainSpecificMetricNodeResolver,
                DomainMetricService = domainMetricService,
                SchoolCategoryProvider = schoolCategoryProvider,
                SchoolAreaLinks = schoolAreaLinks,
                CurrentUserClaimInterrogator = currentUserClaimInterrogator
            };
        }

        #region Supplied Data Construction
        private IQueryable<SchoolInformation> GetSuppliedSchoolInformation()
        {
            return (new List<SchoolInformation>
                        {
                            new SchoolInformation{
                                SchoolId = suppliedSchoolId1,
                                AddressLine1 = "AddressLine1", 
                                AddressLine2 = "AddressLine2", 
                                AddressLine3 = "AddressLine3", 
                                City = "City", 
                                State = "State",
                                ZipCode = "ZipCode",
                                LocalEducationAgencyId = 1, 
                                Name = "Name", 
                                ProfileThumbnail = "ProfileThumbnail", 
                                SchoolCategory = "High School",  
                                TelephoneNumber = "111-333-4444",
                                FaxNumber = "2223334444",
                                WebSite = "www.w.com"
                            }, 
                            new SchoolInformation{
                                SchoolId = 9999 //this school should be filtered out.
                            },
                        }
                ).AsQueryable();
        }

        private IQueryable<SchoolAdministrationInformation> GetSuppliedSchoolAdministration()
        {
            return (new List<SchoolAdministrationInformation>
                        {
                            new SchoolAdministrationInformation{ SchoolId = suppliedSchoolId1, Name = "John Doe", Role = "Principal", DisplayOrder = 1},
                            new SchoolAdministrationInformation{ SchoolId = suppliedSchoolId1, Name = "John Martins", Role = "Teacher", DisplayOrder = 3},
                            new SchoolAdministrationInformation{ SchoolId = suppliedSchoolId1, Name = "Jane Doe", Role = "Other", DisplayOrder = 2},
                            new SchoolAdministrationInformation{ SchoolId = 9999},//Should be filtered out.
                        }).AsQueryable();
        }

        private IQueryable<SchoolAccountabilityInformation> GetSuppliedSchoolAccountability()
        {
            return (new List<SchoolAccountabilityInformation>
                        {
                            new SchoolAccountabilityInformation { SchoolId = suppliedSchoolId1, Attribute = "att3", Value = "v1", DisplayOrder = 3 },
                            new SchoolAccountabilityInformation { SchoolId = suppliedSchoolId1, Attribute = "att1", Value = "v1", DisplayOrder = 1 },
                            new SchoolAccountabilityInformation { SchoolId = suppliedSchoolId1, Attribute = "att2", Value = "v1", DisplayOrder = 2 },
                            new SchoolAccountabilityInformation{SchoolId = 9999},
                        }).AsQueryable();
        }

        private IQueryable<SchoolGradePopulation> GetSuppliedSchoolGradePopulation()
        {
            return (new List<SchoolGradePopulation>
                        {
                            new SchoolGradePopulation { SchoolId = suppliedSchoolId1, Attribute = "att3", Value = 22, TrendDirection = 1, DisplayOrder = 3 },
                            new SchoolGradePopulation { SchoolId = suppliedSchoolId1, Attribute = "att1", Value = 55, TrendDirection = 0, DisplayOrder = 1, LateEnrollment = 11},
                            new SchoolGradePopulation { SchoolId = suppliedSchoolId1, Attribute = "att2", Value = 33, TrendDirection = -1, DisplayOrder = 2 },
                            new SchoolGradePopulation{SchoolId = 9999},
                        }).AsQueryable();
        }

        private IQueryable<SchoolIndicatorPopulation> GetSuppliedSchoolIndicatorPopulation()
        {
            return (new List<SchoolIndicatorPopulation>
                        {
                            new SchoolIndicatorPopulation { SchoolId = suppliedSchoolId1, Attribute = "att3", Value = 22, TrendDirection = 1, DisplayOrder = 3 },
                            new SchoolIndicatorPopulation { SchoolId = suppliedSchoolId1, Attribute = "att1", Value = 55, TrendDirection = 0, DisplayOrder = 1},
                            new SchoolIndicatorPopulation { SchoolId = suppliedSchoolId1, Attribute = "att2", Value = 33, TrendDirection = -1, DisplayOrder = 2 },
                            new SchoolIndicatorPopulation{SchoolId = 9999},
                        }).AsQueryable();
        }

        protected virtual IQueryable<SchoolStudentDemographic> GetSuppliedStudentDemographics()
        {
            var list = new List<SchoolStudentDemographic> {
                new SchoolStudentDemographic{ SchoolId = 9999, Attribute = "This 1 Will be filtered out.", Value = .034m, TrendDirection=1, DisplayOrder=1},
                new SchoolStudentDemographic{ SchoolId = 9999, Attribute = "This 2 Will be filtered out.", Value = .066m, TrendDirection=1, DisplayOrder=1},
                new SchoolStudentDemographic{ SchoolId = suppliedSchoolId1,  Attribute = "Hispanic/Latino", Value = suppliedHispanicValue, TrendDirection=1, DisplayOrder=3},
                new SchoolStudentDemographic{ SchoolId = suppliedSchoolId1,  Attribute = "Female", Value = suppliedFemaleValue, TrendDirection=1, DisplayOrder=1},
                new SchoolStudentDemographic{ SchoolId = suppliedSchoolId1,  Attribute = "Male", Value = suppliedMaleValue, TrendDirection=1, DisplayOrder=2},
                new SchoolStudentDemographic{ SchoolId = suppliedSchoolId1,  Attribute = "Expected 4.8", Value = .8m, TrendDirection=1, DisplayOrder=7},
                new SchoolStudentDemographic{ SchoolId = suppliedSchoolId1,  Attribute = "Expected 2.8", Value = .81m, TrendDirection=1, DisplayOrder=6},
                new SchoolStudentDemographic{ SchoolId = suppliedSchoolId1,  Attribute = "Expected 3.8", Value = .82m, TrendDirection=1, DisplayOrder=5},
                new SchoolStudentDemographic{ SchoolId = suppliedSchoolId1,  Attribute = "Expected 1.8", Value = .83m, TrendDirection=1, DisplayOrder=4},
            };
            return list.AsQueryable();
        }

        private IQueryable<SchoolProgramPopulation> GetSuppliedSchoolProgramPopulation()
        {
            return (new List<SchoolProgramPopulation>
                        {
                            new SchoolProgramPopulation { SchoolId = suppliedSchoolId1, Attribute = "att3", Value = 22, TrendDirection = 1, DisplayOrder = 3 },
                            new SchoolProgramPopulation { SchoolId = suppliedSchoolId1, Attribute = "att1", Value = 55, TrendDirection = 0, DisplayOrder = 1},
                            new SchoolProgramPopulation { SchoolId = suppliedSchoolId1, Attribute = "att2", Value = 33, TrendDirection = -1, DisplayOrder = 2 },
                            new SchoolProgramPopulation{SchoolId = 9999},
                        }).AsQueryable();
        }

        private IQueryable<SchoolFeederSchool> GetSuppliedSchoolFeederSchool()
        {
            return (new List<SchoolFeederSchool>
                        {
                            new SchoolFeederSchool { SchoolId = suppliedSchoolId1, Attribute = "School 3", Value = 33, TrendDirection = 1, DisplayOrder = 3 },
                            new SchoolFeederSchool { SchoolId = suppliedSchoolId1, Attribute = "School 1", Value = 11, TrendDirection = 0, DisplayOrder = 1},
                            new SchoolFeederSchool { SchoolId = suppliedSchoolId1, Attribute = "School 2", Value = 22, TrendDirection = -1, DisplayOrder = 2 },
                            new SchoolFeederSchool{SchoolId = 9999},
                        }).AsQueryable();
        }

        private ContainerMetric GetGraduationPlanContainerMetric()
        {
            const int metricIdA = 100;
            const int metricIdB = 101;
            const int metricIdC = 102;
            return new ContainerMetric
                       {
                           Name = "High School Graduation Plan",
                           Children = new List<MetricBase>
                                          {
                                              new GranularMetric<double> {MetricId = metricIdA, Name="A", Value = .45, TrendDirection = 1, Url = schoolAreaLinks.Metrics(suppliedSchoolId1, metricIdA)},
                                              new GranularMetric<double> {MetricId = metricIdB, Name="C", Value = .15, TrendDirection = 0, Url = schoolAreaLinks.Metrics(suppliedSchoolId1, metricIdB)},
                                              new GranularMetric<double> {MetricId = metricIdC, Name="B", Value = .99, TrendDirection = -1, Url = schoolAreaLinks.Metrics(suppliedSchoolId1, metricIdC)},
                                              new GranularMetric<string> {Name="D", Value = "This should not be picked", TrendDirection = -1},
                                          }

                       };
        }

        #endregion

        protected override void ExecuteTest()
        {
            var request = new TRequest { SchoolId = suppliedSchoolId1};

            actualModel = service.Get(request);
        }

        [Test]
        public void Should_assign_all_properties_on_school_information_presentation_model_correctly()
        {
            var suppliedSchoolData = suppliedSchoolInformationData.Single(x => x.SchoolId == suppliedSchoolId1);

            //Asserting the School Info
            Assert.That(actualModel.SchoolId, Is.EqualTo(suppliedSchoolData.SchoolId), "SchoolId does not match.");
            Assert.That(actualModel.SchoolCategory, Is.EqualTo(suppliedSchoolCategory), "SchoolCategory does not match.");
            Assert.That(actualModel.Name, Is.EqualTo(suppliedSchoolData.Name), "Name does not match.");
            //Assert.That(actualModel.ProfileThumbnail, Is.EqualTo(suppliedSchoolData.ProfileThumbnail));

            Assert.That(actualModel.AddressLines[0], Is.EqualTo(suppliedSchoolData.AddressLine1), "Addresslines[0] does not match.");
            Assert.That(actualModel.AddressLines[1], Is.EqualTo(suppliedSchoolData.AddressLine2), "Addresslines[1] does not match.");
            Assert.That(actualModel.AddressLines[2], Is.EqualTo(suppliedSchoolData.AddressLine3), "Addresslines[2] does not match.");

            Assert.That(actualModel.City, Is.EqualTo(suppliedSchoolData.City), "City does not match.");
            Assert.That(actualModel.State, Is.EqualTo(suppliedSchoolData.State), "State does not match.");
            Assert.That(actualModel.ZipCode, Is.EqualTo(suppliedSchoolData.ZipCode), "ZipCode does not match.");
            Assert.That(actualModel.WebSite, Is.EqualTo(suppliedSchoolData.WebSite), "WebSite does not match.");
        }

        [Test]
        public void Should_format_telephone_numbers_correctly()
        {
            Assert.That(actualModel.TelephoneNumbers[0].Number, Is.EqualTo("(111) 333-4444"));
            Assert.That(actualModel.TelephoneNumbers[1].Number, Is.EqualTo("(222) 333-4444"));
        }

        [Test]
        public void Should_assign_all_properties_on_school_administration_presentation_model_correctly()
        {
            var suppliedAdminData = suppliedSchoolAdministrationInformationData.Where(x => x.SchoolId == suppliedSchoolId1).OrderBy(x => x.DisplayOrder).ToList();

            Assert.That(actualModel.Administration.Count, Is.EqualTo(suppliedAdminData.Count), "The count is not equal.");

            //Asserting the School Administration
            for (int i = 0; i < suppliedAdminData.Count(); i++)
            {
                Assert.That(actualModel.Administration[i].Role, Is.EqualTo(suppliedAdminData[i].Role));
                Assert.That(actualModel.Administration[i].Name, Is.EqualTo(suppliedAdminData[i].Name));
            }
        }

        [Test]
        public void Should_assign_all_properties_on_school_accountability_presentation_model_correctly()
        {
            var data = suppliedSchoolAccountabilityInformationData.Where(x => x.SchoolId == suppliedSchoolId1).OrderBy(x => x.DisplayOrder).ToList();

            Assert.That(actualModel.Accountability.Count, Is.EqualTo(data.Count), "The count is not equal.");

            //Asserting the School Administration
            for (int i = 0; i < data.Count(); i++)
            {
                Assert.That(actualModel.Accountability[i].Attribute, Is.EqualTo(data[i].Attribute));
                Assert.That(actualModel.Accountability[i].Value, Is.EqualTo(data[i].Value));
            }
        }

        [Test]
        public void Should_assign_all_properties_on_school_grade_population_presentation_model_correctly()
        {
            var data = suppliedSchoolGradePopulationData.Where(x => x.SchoolId == suppliedSchoolId1).OrderBy(x => x.DisplayOrder).ToList();

            //Asserting the GradePopulation
            //The first row should be assigned the TotalNumberOfStudents
            Assert.That(actualModel.GradePopulation.TotalNumberOfStudents.Attribute, Is.EqualTo(data[0].Attribute));
            Assert.That(actualModel.GradePopulation.TotalNumberOfStudents.Value, Is.EqualTo(data[0].Value));
            Assert.That(actualModel.GradePopulation.TotalNumberOfStudents.Trend, Is.EqualTo(TrendDirection.Unchanged));

            //The rest are grouped by Grade
            for (int i = 1; i < data.Count; i++)
            {
                Assert.That(actualModel.GradePopulation.TotalNumberOfStudentsByGrade[i - 1].Attribute, Is.EqualTo(data[i].Attribute));
                Assert.That(actualModel.GradePopulation.TotalNumberOfStudentsByGrade[i - 1].Value, Is.EqualTo(data[i].Value));
            }
            Assert.That(actualModel.GradePopulation.TotalNumberOfStudentsByGrade[0].Trend, Is.EqualTo(TrendDirection.Decreasing));
            Assert.That(actualModel.GradePopulation.TotalNumberOfStudentsByGrade[1].Trend, Is.EqualTo(TrendDirection.Increasing));
        }

        [Test]
        public void Should_assign_all_properties_on_school_indicator_population_presentation_model_correctly()
        {
            var data = suppliedSchoolIndicatorPopulationData.Where(x => x.SchoolId == suppliedSchoolId1).OrderBy(x => x.DisplayOrder).ToList();

            //Asserting the SchoolIndicatorPopulation
            for (int i = 0; i < 2; i++)
            {
                Assert.That(actualModel.GradePopulation.Indicators[i].Attribute, Is.EqualTo(data[i].Attribute));
                Assert.That(actualModel.GradePopulation.Indicators[i].Value, Is.EqualTo(data[i].Value));
            }
            Assert.That(actualModel.GradePopulation.Indicators[0].Trend, Is.EqualTo(TrendDirection.Unchanged));
            Assert.That(actualModel.GradePopulation.Indicators[1].Trend, Is.EqualTo(TrendDirection.Decreasing));
            Assert.That(actualModel.GradePopulation.Indicators[2].Trend, Is.EqualTo(TrendDirection.Increasing));

        }

        [Test]
        public virtual void Should_assign_all_properties_on_school_student_demographics_presentation_model_correctly()
        {
            Assert.That(actualModel.StudentDemographics, Is.Not.Null);
            Assert.That(actualModel.StudentDemographics.Female.Value, Is.EqualTo(suppliedFemaleValue));
            Assert.That(actualModel.StudentDemographics.Male.Value, Is.EqualTo(suppliedMaleValue));
            Assert.That(actualModel.StudentDemographics.ByEthnicity, Is.Not.Null);
            Assert.That(actualModel.StudentDemographics.ByEthnicity.Count(), Is.EqualTo(1));
            Assert.That(actualModel.StudentDemographics.ByEthnicity[0].Attribute, Is.EqualTo("Hispanic/Latino"));
            Assert.That(actualModel.StudentDemographics.ByEthnicity[0].Value, Is.EqualTo(suppliedHispanicValue));
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
        public void Should_assign_all_properties_on_school_program_population_presentation_model_correctly()
        {
            var data = suppliedSchoolProgramPopulationData.Where(x => x.SchoolId == suppliedSchoolId1).OrderBy(x => x.DisplayOrder).ToList();

            //Asserting the SchoolProgramPopulation
            for (int i = 0; i < data.Count; i++)
            {
                Assert.That(actualModel.StudentsByProgram[i].Attribute, Is.EqualTo(data[i].Attribute));
                Assert.That(actualModel.StudentsByProgram[i].Value, Is.EqualTo(data[i].Value));
            }
            Assert.That(actualModel.StudentsByProgram[0].Trend, Is.EqualTo(TrendDirection.Unchanged));
            Assert.That(actualModel.StudentsByProgram[1].Trend, Is.EqualTo(TrendDirection.Decreasing));
            Assert.That(actualModel.StudentsByProgram[2].Trend, Is.EqualTo(TrendDirection.Increasing));
        }

        [Test]
        public void Should_assign_all_properties_on_school_feeder_schools_presentation_model_correctly()
        {
            var data = suppliedSchoolFeederSchoolData.Where(x => x.SchoolId == suppliedSchoolId1).OrderBy(x => x.DisplayOrder).ToList();

            //Asserting the FeederSchools
            for (int i = 0; i < data.Count; i++)
            {
                Assert.That(actualModel.FeederSchoolDistribution[i].Attribute, Is.EqualTo(data[i].Attribute));
                Assert.That(actualModel.FeederSchoolDistribution[i].Value, Is.EqualTo(data[i].Value));
            }

            Assert.That(actualModel.StudentsByProgram[0].Trend, Is.EqualTo(TrendDirection.Unchanged));
            Assert.That(actualModel.StudentsByProgram[1].Trend, Is.EqualTo(TrendDirection.Decreasing));
            Assert.That(actualModel.StudentsByProgram[2].Trend, Is.EqualTo(TrendDirection.Increasing));
        }

        [Test]
        public void Should_assign_all_properties_on_school_high_school_graduation_plan_presentation_model_correctly()
        {
            var suppliedContainer = suppliedHighSchoolGraduationPlanMetric as ContainerMetric;

            Assert.That(actualModel.HighSchoolGraduationPlan.Count, Is.EqualTo(suppliedContainer.Children.Count() - 1));

            int i = 0;
            foreach (MetricBase mb in suppliedContainer.Children)
            {
                var gm = mb as GranularMetric<double>;
                if (gm == null)
                    continue;
                Assert.That(actualModel.HighSchoolGraduationPlan[i].Attribute, Is.EqualTo(gm.Name));
                Assert.That(actualModel.HighSchoolGraduationPlan[i].Value, Is.EqualTo(gm.Value));
                Assert.That(actualModel.HighSchoolGraduationPlan[i].Trend, Is.EqualTo(TrendFromDirection(gm.TrendDirection)));
                i++;
            }
        }

        private TrendDirection TrendFromDirection(int? direction)
        {
            switch (direction)
            {
                case 1:
                    return TrendDirection.Increasing;
                case 0:
                    return TrendDirection.Unchanged;
                case -1:
                    return TrendDirection.Decreasing;
                default:
                    return TrendDirection.None;
            }
        }

        [Test]
        public virtual void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel.EnsureNoDefaultValues("InformationModel.GradePopulation.TotalNumberOfStudents.Trend",//These trends are to test no change which values is 0 and default for int.
                                                "InformationModel.GradePopulation.Indicators[0].Trend",
                                                "InformationModel.StudentsByProgram[0].Trend",
                                                "InformationModel.FeederSchoolDistribution[0].Trend",
                                                "InformationModel.FeederSchoolDistribution[0].Url",
                                                "InformationModel.FeederSchoolDistribution[1].Url",
                                                "InformationModel.FeederSchoolDistribution[2].Url",
                                                "InformationModel.HighSchoolGraduationPlan[1].Trend"
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

    public abstract class SchoolInformationResourceBase : When_getting_school_information<InformationRequest, InformationModel, InformationService, AttributeItem<string>, Administrator, AttributeItemWithTrend<decimal?>, AttributeItemWithTrend<decimal?>, AttributeItemWithTrend<decimal?>, AttributeItemWithTrend<decimal?>, AttributeItemWithTrend<decimal?>, AttributeItemWithTrend<decimal?>>
    {}

    public class When_getting_local_education_agency_information_resource_as_administrator : SchoolInformationResourceBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewOperationalDashboard, suppliedSchoolId1)).Repeat.Any().Return(true);
            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllMetrics, suppliedSchoolId1)).Repeat.Any().Return(true);
        }

        [Test]
        public void Should_have_urls_on_demographic_attributes()
        {
            Assert.That(actualModel.StudentDemographics.Female.Url, Is.EqualTo(schoolAreaLinks.StudentDemographicList(suppliedSchoolId1, "Female")));
            Assert.That(actualModel.StudentDemographics.Male.Url, Is.EqualTo(schoolAreaLinks.StudentDemographicList(suppliedSchoolId1, "Male")));
            foreach (var race in actualModel.StudentDemographics.ByRace)
            {
                Assert.That(race.Url, Is.EqualTo(schoolAreaLinks.StudentDemographicList(suppliedSchoolId1, race.Attribute)));
            }
            foreach (var ethnicity in actualModel.StudentDemographics.ByEthnicity)
            {
                Assert.That(ethnicity.Url, Is.EqualTo(schoolAreaLinks.StudentDemographicList(suppliedSchoolId1, ethnicity.Attribute)));
            }
            foreach (var program in actualModel.StudentsByProgram)
            {
                Assert.That(program.Url, Is.EqualTo(schoolAreaLinks.StudentDemographicList(suppliedSchoolId1, program.Attribute)));
            }
            foreach (var indicator in actualModel.GradePopulation.Indicators)
            {
                Assert.That(indicator.Url, Is.EqualTo(schoolAreaLinks.StudentDemographicList(suppliedSchoolId1, indicator.Attribute)));
            }
        }
    }

    public class When_getting_local_education_agency_information_resource_as_staff : SchoolInformationResourceBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewOperationalDashboard, suppliedSchoolId1)).Repeat.Any().Return(false);
            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllMetrics, suppliedSchoolId1)).Repeat.Any().Return(false);
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
                                                        "InformationModel.GradePopulation.SchoolLateEnrollment.Url",
                                                        "InformationModel.GradePopulation.Indicators[0].Url",
                                                        "InformationModel.GradePopulation.Indicators[1].Url",
                                                        "InformationModel.GradePopulation.Indicators[2].Url",
                                                        "InformationModel.GradePopulation.Indicators[3].Url",
                                                        "InformationModel.GradePopulation.TotalNumberOfStudents.Trend",//These trends are to test no change which values is 0 and default for int.
                                                        "InformationModel.GradePopulation.Indicators[0].Trend",
                                                        "InformationModel.StudentsByProgram[0].Trend",
                                                        "InformationModel.FeederSchoolDistribution[0].Trend",
                                                        "InformationModel.FeederSchoolDistribution[0].Url",
                                                        "InformationModel.FeederSchoolDistribution[1].Url",
                                                        "InformationModel.FeederSchoolDistribution[2].Url",
                                                        "InformationModel.HighSchoolGraduationPlan[1].Trend"});
        }
    }
    
    [TestFixture]
    public abstract class When_getting_school_information_with_missing_demographics<TRequest, TResponse, TService, TAccountability, TSchoolAdministration, TGradePopulation, TGradePopulationIndicators, TStudentDemographics, TProgramPopulation, TFeederSchools, TGraduationPlan> : When_getting_school_information<TRequest, TResponse, TService, TAccountability, TSchoolAdministration, TGradePopulation, TGradePopulationIndicators, TStudentDemographics, TProgramPopulation, TFeederSchools, TGraduationPlan>
        where TRequest : InformationRequest, new()
        where TResponse : InformationModel, new()
        where TAccountability : AttributeItem<string>, new()
        where TSchoolAdministration : Administrator, new()
        where TGradePopulation : AttributeItemWithTrend<decimal?>, new()
        where TGradePopulationIndicators : AttributeItemWithTrend<decimal?>, new()
        where TStudentDemographics : AttributeItemWithTrend<decimal?>, new()
        where TProgramPopulation : AttributeItemWithTrend<decimal?>, new()
        where TFeederSchools : AttributeItemWithTrend<decimal?>, new()
        where TGraduationPlan : AttributeItemWithTrend<decimal?>, new()
        where TService : InformationServiceBase<TRequest, TResponse, TAccountability, TSchoolAdministration, TGradePopulation, TGradePopulationIndicators, TStudentDemographics, TProgramPopulation, TFeederSchools, TGraduationPlan>, new()
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewOperationalDashboard, suppliedSchoolId1)).Repeat.Any().Return(true);
            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllMetrics, suppliedSchoolId1)).Repeat.Any().Return(true);
        }

        protected override IQueryable<SchoolStudentDemographic> GetSuppliedStudentDemographics()
        {
            var list = new List<SchoolStudentDemographic> {
                new SchoolStudentDemographic{ SchoolId = 9999, Attribute = "Foo 1", Value = .034m, TrendDirection=1, DisplayOrder=1},
                new SchoolStudentDemographic{ SchoolId = 9999, Attribute = "Foo 2", Value = .066m, TrendDirection=1, DisplayOrder=1},
                //Ethnicity should be cero...
                //new SchoolStudentDemographic{ SchoolId = suppliedSchoolInformationData.SchoolId, Attribute = "Hispanic/Latino", Value = suppliedHispanicValue, TrendDirection=1, DisplayOrder=1},
                //Female should be missing...
                //new SchoolStudentDemographic{ SchoolId = suppliedSchoolInformationData.SchoolId, Attribute = "Female", Value = suppliedFemaleValue + .1m, TrendDirection=1, DisplayOrder=1},
                new SchoolStudentDemographic{ SchoolId = suppliedSchoolId1, Attribute = "Male", Value = suppliedMaleValue, TrendDirection=1, DisplayOrder=1},
                new SchoolStudentDemographic{ SchoolId = suppliedSchoolId1, Attribute = "Expected 4.8", Value = .8m, TrendDirection=1, DisplayOrder=1},
                new SchoolStudentDemographic{ SchoolId = suppliedSchoolId1, Attribute = "Expected 2.8", Value = .81m, TrendDirection=1, DisplayOrder=1},
                new SchoolStudentDemographic{ SchoolId = suppliedSchoolId1, Attribute = "Expected 3.8", Value = .82m, TrendDirection=1, DisplayOrder=1},
                new SchoolStudentDemographic{ SchoolId = suppliedSchoolId1, Attribute = "Expected 1.8", Value = .83m, TrendDirection=1, DisplayOrder=1},
            };
            return list.AsQueryable();
        }

        [Test]
        public override void Should_assign_all_properties_on_school_student_demographics_presentation_model_correctly()
        {

            Assert.That(actualModel.StudentDemographics, Is.Not.Null);
            Assert.That(actualModel.StudentDemographics.Female.Value, Is.EqualTo(0));
            Assert.That(actualModel.StudentDemographics.Male.Value, Is.EqualTo(suppliedMaleValue));
            Assert.That(actualModel.StudentDemographics.ByEthnicity, Is.Not.Null);
            Assert.That(actualModel.StudentDemographics.ByEthnicity.Count(), Is.EqualTo(1));
            Assert.That(actualModel.StudentDemographics.ByEthnicity[0].Attribute, Is.EqualTo("Hispanic/Latino"));
            Assert.That(actualModel.StudentDemographics.ByEthnicity[0].Value, Is.EqualTo(0));
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
        public override void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel.EnsureNoDefaultValues(  "InformationModel.GradePopulation.TotalNumberOfStudents.Trend",
                                                "InformationModel.GradePopulation.Indicators[0].Trend",
                                                "InformationModel.StudentDemographics.Female.Value",
                                                "InformationModel.StudentDemographics.Female.Trend",
                                                "InformationModel.StudentDemographics.ByEthnicity[0].Value",
                                                "InformationModel.StudentDemographics.ByEthnicity[0].Trend",
                                                "InformationModel.StudentsByProgram[0].Trend",
                                                "InformationModel.FeederSchoolDistribution[0].Trend",
                                                "InformationModel.FeederSchoolDistribution[0].Url",
                                                "InformationModel.FeederSchoolDistribution[1].Url",
                                                "InformationModel.FeederSchoolDistribution[2].Url",
                                                "InformationModel.HighSchoolGraduationPlan[1].Trend"
                                                );

        }

    }

    public class When_getting_school_information_with_missing_demographics_from_service : When_getting_school_information_with_missing_demographics<InformationRequest, InformationModel, InformationService, AttributeItem<string>, Administrator, AttributeItemWithTrend<decimal?>, AttributeItemWithTrend<decimal?>, AttributeItemWithTrend<decimal?>, AttributeItemWithTrend<decimal?>, AttributeItemWithTrend<decimal?>, AttributeItemWithTrend<decimal?>>
    {}
}
