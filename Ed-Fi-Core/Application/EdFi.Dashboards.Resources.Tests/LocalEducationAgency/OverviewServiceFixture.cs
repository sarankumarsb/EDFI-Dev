// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency.Overview;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.LocalEducationAgency
{
    [TestFixture]
    public abstract class When_calling_get_method_from_local_education_agency_overview_service<TRequest, TResponse, TService, TAccountabilityRating> : TestFixtureBase
        where TRequest : OverviewRequest, new()
        where TResponse : OverviewModel, new()
        where TAccountabilityRating : AccountabilityRating, new()
        where TService : OverviewServiceBase<TRequest,TResponse,TAccountabilityRating>, new()
    {
        protected TResponse actualModel;
        protected int localEducationAgencyId_1 = 1;
        protected IRepository<LocalEducationAgencyInformation> localEducationAgencyInformationRepository;
        protected IQueryable<LocalEducationAgencyInformation> suppliedLocalEducationAgencyInformationData;
        protected IRepository<LocalEducationAgencyAccountabilityInformation> localEducationAgencyAccountabilityInformationRepository;
        protected IQueryable<LocalEducationAgencyAccountabilityInformation> suppliedLocalEducationAgencyAccountabilityInformationData;
        protected ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks;

        protected override void EstablishContext()
        {
            suppliedLocalEducationAgencyInformationData = GetSuppliedLocalEducationAgencyInformationData();
            suppliedLocalEducationAgencyAccountabilityInformationData = GetSuppliedLocalEducationAgencyAccountabilityInformationData();
            localEducationAgencyAreaLinks = new LocalEducationAgencyAreaLinksFake();

            localEducationAgencyInformationRepository = mocks.StrictMock<IRepository<LocalEducationAgencyInformation>>();
            Expect.Call(localEducationAgencyInformationRepository.GetAll()).Return(suppliedLocalEducationAgencyInformationData);

            localEducationAgencyAccountabilityInformationRepository = mocks.StrictMock<IRepository<LocalEducationAgencyAccountabilityInformation>>();
            Expect.Call(localEducationAgencyAccountabilityInformationRepository.GetAll()).Return(suppliedLocalEducationAgencyAccountabilityInformationData);

        }

        protected IQueryable<LocalEducationAgencyInformation> GetSuppliedLocalEducationAgencyInformationData()
        {
            return (new List<LocalEducationAgencyInformation>
                        {
                            new LocalEducationAgencyInformation
                                {
                                    LocalEducationAgencyId = localEducationAgencyId_1,
                                    Name = "LEA Name 1",
                                    ProfileThumbnail = "Profile Thumbnail 1"
                                },
                            new LocalEducationAgencyInformation
                                {
                                    LocalEducationAgencyId = 2,
                                    Name = "LEA Name 2",
                                    ProfileThumbnail = "Profile Thumbnail 2"
                                }
                        }).AsQueryable();
        }

        protected IQueryable<LocalEducationAgencyAccountabilityInformation> GetSuppliedLocalEducationAgencyAccountabilityInformationData()
        {
            return (new List<LocalEducationAgencyAccountabilityInformation>
                        {
                            new LocalEducationAgencyAccountabilityInformation
                                {
                                    LocalEducationAgencyId = localEducationAgencyId_1,
                                    Attribute = "Accountability Rating",
                                    Value = "Academically Acceptable",
                                    DisplayOrder = 1
                                },
                            new LocalEducationAgencyAccountabilityInformation
                                {
                                    LocalEducationAgencyId = 2,
                                    Attribute = "Accountability Rating 2",
                                    Value = "Academically Acceptable 2",
                                    DisplayOrder = 1
                                }
                        }).AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new TService
                              {
                                  LocalEducationAgencyInformationRepository = localEducationAgencyInformationRepository,
                                  LocalEducationAgencyAccountabilityInformationRepository = localEducationAgencyAccountabilityInformationRepository,
                                  LocalEducationAgencyAreaLinks = localEducationAgencyAreaLinks
                              };

            var request = new TRequest { LocalEducationAgencyId = localEducationAgencyId_1 };

            actualModel = service.Get(request);
        }

        [Test]
        public virtual void Should_not_return_null_model()
        {
            Assert.NotNull(actualModel);
        }

        [Test]
        public virtual void Should_return_local_education_agency_name()
        {
            Assert.IsNotNullOrEmpty(actualModel.LocalEducationAgencyName);
        }

        [Test]
        public virtual void Should_return_local_education_agency_image()
        {
            Assert.IsNotNullOrEmpty(actualModel.ProfileThumbnail);
        }

        [Test]
        public virtual void Should_return_ratings_information_list_not_null_or_empty()
        {
            Assert.NotNull(actualModel.AccountabilityRatings);
            Assert.IsTrue(actualModel.AccountabilityRatings.Any());
        }
        
        [Test]
        public virtual void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel.EnsureNoDefaultValues();
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

    public class When_calling_getting_the_overview_resource_for_a_local_education_agency : When_calling_get_method_from_local_education_agency_overview_service<OverviewRequest, OverviewModel, OverviewService, AccountabilityRating>
    { }
}
