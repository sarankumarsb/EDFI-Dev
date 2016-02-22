// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources.Admin;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using NUnit.Framework;
using Rhino.Mocks;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.Admin;
using EdFi.Dashboards.Testing;
using Is = NUnit.Framework.Is;

namespace EdFi.Dashboards.Resources.Tests.Admin
{
    [TestFixture]
    public abstract class BaseConfigurationServiceFixture : TestFixtureBase
    {
        protected int suppliedLocalEducationAgencyId = 1;
        protected string suppliedLocalEducationAgencyName = "test LocalEducationAgency";
        protected string providedSystemMessage = "this is the system message";
        protected IQueryable<LocalEducationAgencyAdministration> suppliedAdministration;
        protected IQueryable<LocalEducationAgencyInformation> suppliedLocalEducationAgencyInformation;
        protected IPersistingRepository<LocalEducationAgencyAdministration> administrationRepository;
        protected IRepository<LocalEducationAgencyInformation> localEducationAgencyInformationRepository;
        protected IAdminAreaLinks adminAreaLinks = new AdminAreaLinksFake();

        protected ISystemMessageProvider systemMessageProvider;
        protected ISiteAvailableProvider siteAvailableProvider;
        protected ConfigurationModel actualModel;

        protected override void EstablishContext()
        {
            suppliedAdministration = GetSuppliedAdministration();

            administrationRepository = mocks.StrictMock<IPersistingRepository<LocalEducationAgencyAdministration>>();
            Expect.Call(administrationRepository.GetAll()).Return(suppliedAdministration);

            suppliedLocalEducationAgencyInformation = GetSuppliedLocalEducationAgencyInformation();
            localEducationAgencyInformationRepository = mocks.StrictMock<IRepository<LocalEducationAgencyInformation>>();
            Expect.Call(localEducationAgencyInformationRepository.GetAll()).Return(suppliedLocalEducationAgencyInformation);

            systemMessageProvider = mocks.StrictMock<ISystemMessageProvider>();
            siteAvailableProvider = mocks.StrictMock<ISiteAvailableProvider>();

            base.EstablishContext();
        }

        protected virtual IQueryable<LocalEducationAgencyAdministration> GetSuppliedAdministration()
        {
            var adminConfigs = new List<LocalEducationAgencyAdministration>();
            return adminConfigs.AsQueryable();
        }

        protected virtual IQueryable<LocalEducationAgencyInformation> GetSuppliedLocalEducationAgencyInformation()
        {
            var results = new List<LocalEducationAgencyInformation>
                                                                  {
                                                                      new LocalEducationAgencyInformation{LocalEducationAgencyId = 100, Name = "not correct"},
                                                                      new LocalEducationAgencyInformation{LocalEducationAgencyId = suppliedLocalEducationAgencyId, Name = suppliedLocalEducationAgencyName},
                                                                      new LocalEducationAgencyInformation{LocalEducationAgencyId = 200, Name = "still not correct"}
                                                                  };
            return results.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var adminService = new ConfigurationService(administrationRepository, localEducationAgencyInformationRepository, adminAreaLinks, new HashtableCacheProvider(), siteAvailableProvider, systemMessageProvider);
            actualModel = adminService.Get(new ConfigurationRequest() { LocalEducationAgencyId = suppliedLocalEducationAgencyId });
        }

        [Test]
        public virtual void Should_create_model_correctly()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.LocalEducationAgencyId, Is.EqualTo(suppliedLocalEducationAgencyId));
            Assert.That(actualModel.LocalEducationAgencyName, Is.EqualTo(suppliedLocalEducationAgencyName));
        }

        [Test]
        public void Should_have_no_unassigned_values_on_presentation_model()
        {
            // Configuration.IsKillSwitchActivatedForCurrentUser can have false as a valid value, which is considered uninitialized by this test
            actualModel.EnsureNoDefaultValues(new[] { "ConfigurationModel.ResourceUrl", "ConfigurationModel.Links", "ConfigurationModel.IsKillSwitchActivated", "ConfigurationModel.SystemMessage" });
        }
        
        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }
    
    [TestFixture]
    public class When_loading_new_configuration : BaseConfigurationServiceFixture
    {
        [Test]
        public void Kill_switch_should_be_deactivated()
        {
            Assert.That(actualModel.IsKillSwitchActivated, Is.False);
        }
    }

    [TestFixture]
    public class When_loading_non_existent_local_education_agency : TestFixtureBase
    {
        protected int suppliedLocalEducationAgencyId = 1;
        protected string suppliedLocalEducationAgencyName = "test LocalEducationAgency";
        protected IQueryable<LocalEducationAgencyAdministration> suppliedAdministration;
        protected IQueryable<LocalEducationAgencyInformation> suppliedLocalEducationAgencyInformation;
        protected IPersistingRepository<LocalEducationAgencyAdministration> AdministrationRepository;
        protected IRepository<LocalEducationAgencyInformation> localEducationAgencyInformationRepository;
        protected IAdminAreaLinks adminAreaLinks = new AdminAreaLinksFake();
        protected ISystemMessageProvider systemMessageProvider;
        protected ISiteAvailableProvider siteAvailableProvider;

        protected ConfigurationModel actualModel;

        protected override void EstablishContext()
        {
            AdministrationRepository = mocks.StrictMock<IPersistingRepository<LocalEducationAgencyAdministration>>();

            suppliedLocalEducationAgencyInformation = GetSuppliedLocalEducationAgencyInformation();
            localEducationAgencyInformationRepository = mocks.StrictMock<IRepository<LocalEducationAgencyInformation>>();
            Expect.Call(localEducationAgencyInformationRepository.GetAll()).Return(suppliedLocalEducationAgencyInformation);

            systemMessageProvider = mocks.StrictMock<ISystemMessageProvider>();
            siteAvailableProvider = mocks.StrictMock<ISiteAvailableProvider>();

            base.EstablishContext();
        }

        protected IQueryable<LocalEducationAgencyInformation> GetSuppliedLocalEducationAgencyInformation()
        {
            var results = new List<LocalEducationAgencyInformation>
                                                                  {
                                                                      new LocalEducationAgencyInformation{LocalEducationAgencyId = 100, Name = "not correct"},
                                                                      new LocalEducationAgencyInformation{LocalEducationAgencyId = 200, Name = "nope"},
                                                                      new LocalEducationAgencyInformation{LocalEducationAgencyId = 300, Name = "still not correct"}
                                                                  };
            return results.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var adminService = new ConfigurationService(AdministrationRepository, localEducationAgencyInformationRepository, adminAreaLinks, new HashtableCacheProvider(), siteAvailableProvider, systemMessageProvider);
            actualModel = adminService.Get(new ConfigurationRequest() { LocalEducationAgencyId = suppliedLocalEducationAgencyId });
        }

        [Test]
        public void Should_create_model_correctly()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.LocalEducationAgencyId, Is.EqualTo(-1));
            Assert.That(actualModel.LocalEducationAgencyName, Is.EqualTo("No district found."));
        }
    }


    [TestFixture]
    public class When_loading_new_configuration_from_existing_data : When_loading_new_configuration
    {

        protected override IQueryable<LocalEducationAgencyAdministration> GetSuppliedAdministration()
        {
            var adminConfigs = new List<LocalEducationAgencyAdministration>
                                                                 {
                                                                     new LocalEducationAgencyAdministration{ LocalEducationAgencyId = 100, IsKillSwitchActivated = true},
                                                                     new LocalEducationAgencyAdministration{ LocalEducationAgencyId = 200, IsKillSwitchActivated = true},
                                                                     new LocalEducationAgencyAdministration{ LocalEducationAgencyId = 300, IsKillSwitchActivated = true},
                                                                 };
            return adminConfigs.AsQueryable();
        }
    }


    [TestFixture]
    public class When_loading_existing_configuration : BaseConfigurationServiceFixture
    {

        protected override IQueryable<LocalEducationAgencyAdministration> GetSuppliedAdministration()
        {
            var adminConfigs = new List<LocalEducationAgencyAdministration>
                                                                 {
                                                                     new LocalEducationAgencyAdministration{ LocalEducationAgencyId = 100, IsKillSwitchActivated = true},
                                                                     new LocalEducationAgencyAdministration{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, IsKillSwitchActivated = true, SystemMessage = providedSystemMessage},
                                                                     new LocalEducationAgencyAdministration{ LocalEducationAgencyId = 300, IsKillSwitchActivated = true},
                                                                 };
            return adminConfigs.AsQueryable();
        }

        [Test]
        public void Kill_switch_should_be_set_properly()
        {
            Assert.That(actualModel.IsKillSwitchActivated, Is.True);
        }
    
        [Test]
        public void System_message_should_be_set_properly()
        {
            Assert.That(actualModel.SystemMessage, Is.EqualTo(providedSystemMessage));
        }
    }

    [TestFixture]
    public class When_saving_configuration : TestFixtureBase
    {
        protected int suppliedLocalEducationAgencyId = 1;
        protected string suppliedLocalEducationAgencyName = "test LocalEducationAgency";
        protected string providedSystemMessage = "this is the system message";
        protected IQueryable<LocalEducationAgencyAdministration> suppliedAdministration;
        protected IQueryable<LocalEducationAgencyInformation> suppliedLocalEducationAgencyInformation;
        protected IPersistingRepository<LocalEducationAgencyAdministration> administrationRepository;
        protected IRepository<LocalEducationAgencyInformation> localEducationAgencyInformationRepository;
        protected IAdminAreaLinks adminAreaLinks = new AdminAreaLinksFake();
        protected ISystemMessageProvider systemMessageProvider;
        protected ISiteAvailableProvider siteAvailableProvider;

        protected ConfigurationModel suppliedModel;

        protected override void EstablishContext()
        {
            suppliedAdministration = GetSuppliedAdministration();
            suppliedLocalEducationAgencyInformation = GetSuppliedLocalEducationAgencyInformation();
            localEducationAgencyInformationRepository = mocks.StrictMock<IRepository<LocalEducationAgencyInformation>>();
            Expect.Call(localEducationAgencyInformationRepository.GetAll()).Return(suppliedLocalEducationAgencyInformation);

            suppliedModel = new ConfigurationModel {LocalEducationAgencyId = suppliedLocalEducationAgencyId, IsKillSwitchActivated = true, SystemMessage = providedSystemMessage};
            administrationRepository = mocks.StrictMock<IPersistingRepository<LocalEducationAgencyAdministration>>();
            Expect.Call(administrationRepository.GetAll()).Return(suppliedAdministration);
            Expect.Call(delegate { administrationRepository.Save(Arg<LocalEducationAgencyAdministration>.Matches(x => x.LocalEducationAgencyId == suppliedLocalEducationAgencyId && x.IsKillSwitchActivated.GetValueOrDefault() && x.SystemMessage == providedSystemMessage)); });

            systemMessageProvider = mocks.StrictMock<ISystemMessageProvider>();
            siteAvailableProvider = mocks.StrictMock<ISiteAvailableProvider>();

            base.EstablishContext();
        }

        protected IQueryable<LocalEducationAgencyAdministration> GetSuppliedAdministration()
        {
            var adminConfigs = new List<LocalEducationAgencyAdministration>
                                                                 {
                                                                     new LocalEducationAgencyAdministration{ LocalEducationAgencyId = 100, IsKillSwitchActivated = true},
                                                                     new LocalEducationAgencyAdministration{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, IsKillSwitchActivated = true, SystemMessage = providedSystemMessage},
                                                                     new LocalEducationAgencyAdministration{ LocalEducationAgencyId = 300, IsKillSwitchActivated = true},
                                                                 };
            return adminConfigs.AsQueryable();
        }

        protected IQueryable<LocalEducationAgencyInformation> GetSuppliedLocalEducationAgencyInformation()
        {
            var results = new List<LocalEducationAgencyInformation>
                                                                  {
                                                                      new LocalEducationAgencyInformation{LocalEducationAgencyId = 100, Name = "not correct"},
                                                                      new LocalEducationAgencyInformation{LocalEducationAgencyId = suppliedLocalEducationAgencyId, Name = suppliedLocalEducationAgencyName},
                                                                      new LocalEducationAgencyInformation{LocalEducationAgencyId = 200, Name = "still not correct"}
                                                                  };
            return results.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var adminService = new ConfigurationService(administrationRepository, localEducationAgencyInformationRepository, adminAreaLinks, new HashtableCacheProvider(), siteAvailableProvider, systemMessageProvider);
            adminService.Post(suppliedModel);
        }

        [Test]
        public void Should_save_properly()
        {
            // all the validation work is done in the mock setup
        }
    }


    [TestFixture]
    public class When_saving_non_existent_local_education_agency_configuration : TestFixtureBase
    {
        protected int suppliedLocalEducationAgencyId = 1;
        protected string suppliedLocalEducationAgencyName = "test LocalEducationAgency";
        protected string providedSystemMessage = "this is the system message";
        protected IQueryable<LocalEducationAgencyInformation> suppliedLocalEducationAgencyInformation;
        protected IPersistingRepository<LocalEducationAgencyAdministration> AdministrationRepository;
        protected IRepository<LocalEducationAgencyInformation> localEducationAgencyInformationRepository;
        protected IAdminAreaLinks adminAreaLinks = new AdminAreaLinksFake();
        protected ISystemMessageProvider systemMessageProvider;
        protected ISiteAvailableProvider siteAvailableProvider;

        protected ConfigurationModel suppliedModel;

        protected override void EstablishContext()
        {
            suppliedLocalEducationAgencyInformation = GetSuppliedLocalEducationAgencyInformation();
            localEducationAgencyInformationRepository = mocks.StrictMock<IRepository<LocalEducationAgencyInformation>>();
            Expect.Call(localEducationAgencyInformationRepository.GetAll()).Repeat.Any().Return(suppliedLocalEducationAgencyInformation);

            suppliedModel = new ConfigurationModel { LocalEducationAgencyId = suppliedLocalEducationAgencyId, IsKillSwitchActivated = true, SystemMessage = providedSystemMessage };

            systemMessageProvider = mocks.StrictMock<ISystemMessageProvider>();
            siteAvailableProvider = mocks.StrictMock<ISiteAvailableProvider>();

            base.EstablishContext();
        }


        protected IQueryable<LocalEducationAgencyInformation> GetSuppliedLocalEducationAgencyInformation()
        {
            var results = new List<LocalEducationAgencyInformation>
                                                                  {
                                                                      new LocalEducationAgencyInformation{LocalEducationAgencyId = 100, Name = "not correct"},
                                                                      new LocalEducationAgencyInformation{LocalEducationAgencyId = 200, Name = "nope"},
                                                                      new LocalEducationAgencyInformation{LocalEducationAgencyId = 300, Name = "still not correct"}
                                                                  };
            return results.AsQueryable();
        }

        protected override void ExecuteTest()
        {
        }

        [Test][ExpectedException(typeof(InvalidOperationException))]
        public void Should_save_properly()
        {
            var adminService = new ConfigurationService(AdministrationRepository, localEducationAgencyInformationRepository, adminAreaLinks, new HashtableCacheProvider(), siteAvailableProvider, systemMessageProvider);
            adminService.Post(suppliedModel);
        }
    }
}
