// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using NUnit.Framework;
using Rhino.Mocks;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Testing;

namespace EdFi.Dashboards.Resources.Tests.LocalEducationAgency
{
    [TestFixture]
    public class When_loading_local_education_agency_brief_on_existing_local_education_agency : TestFixtureBase
    {
        private BriefModel actualModel;
        private BriefService service;

        private const int suppliedLocalEducationAgencyId = 1;
        private const string supppliedLocalEducationAgencyName = "test LocalEducationAgency name";
        private IQueryable<LocalEducationAgencyInformation> suppliedLocalEducationAgencyInformation;

        private IRepository<LocalEducationAgencyInformation> repository;
        private readonly ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks = new LocalEducationAgencyAreaLinksFake();

        protected override void EstablishContext()
        {
            repository = mocks.StrictMock<IRepository<LocalEducationAgencyInformation>>();

            suppliedLocalEducationAgencyInformation = GetSuppliedLocalEducationAgencyData();

            Expect.Call(repository.GetAll()).Return(suppliedLocalEducationAgencyInformation);
        }

        protected IQueryable<LocalEducationAgencyInformation> GetSuppliedLocalEducationAgencyData()
        {
            return (new List<LocalEducationAgencyInformation>
                        {
                            new LocalEducationAgencyInformation{LocalEducationAgencyId = suppliedLocalEducationAgencyId, Name = supppliedLocalEducationAgencyName},
                            new LocalEducationAgencyInformation{LocalEducationAgencyId = suppliedLocalEducationAgencyId+1, Name = "wrong data"}
                        }).AsQueryable();
        }

        protected override void ExecuteTest()
        {
            service = new BriefService(repository, localEducationAgencyAreaLinks);
            actualModel = service.Get(new BriefRequest { LocalEducationAgencyId = suppliedLocalEducationAgencyId });
        }

        [Test]
        public void Should_return_model_that_is_not_null()
        {
            Assert.That(actualModel, Is.Not.Null);
        }

        [Test]
        public void Should_return_model_that_mapped_correctly()
        {
            Assert.That(actualModel.Name, Is.EqualTo(supppliedLocalEducationAgencyName));
            Assert.That(actualModel.LocalEducationAgencyId, Is.EqualTo(suppliedLocalEducationAgencyId));
            Assert.That(actualModel.ProfileThumbnail, Is.EqualTo(localEducationAgencyAreaLinks.ProfileThumbnail(suppliedLocalEducationAgencyId)));
            Assert.That(actualModel.Url, Is.EqualTo(localEducationAgencyAreaLinks.Default(suppliedLocalEducationAgencyId)));
            Assert.That(actualModel.Links.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel.EnsureNoDefaultValues("BriefModel.ResourceUrl", "BriefModel.Links");
        }
        
        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }

    public class When_loading_local_education_agency_brief_on_missing_local_education_agency : TestFixtureBase
    {
        private BriefModel actualModel;
        private BriefService service;

        private const int suppliedLocalEducationAgencyId = 1;
        private IQueryable<LocalEducationAgencyInformation> suppliedLocalEducationAgencyInformation;

        private IRepository<LocalEducationAgencyInformation> repository;
        private readonly ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks = new LocalEducationAgencyAreaLinksFake();

        protected override void EstablishContext()
        {
            repository = mocks.StrictMock<IRepository<LocalEducationAgencyInformation>>();

            suppliedLocalEducationAgencyInformation = GetSuppliedLocalEducationAgencyData();

            Expect.Call(repository.GetAll()).Return(suppliedLocalEducationAgencyInformation);
        }

        protected IQueryable<LocalEducationAgencyInformation> GetSuppliedLocalEducationAgencyData()
        {
            return (new List<LocalEducationAgencyInformation>
                        {
                            new LocalEducationAgencyInformation{LocalEducationAgencyId = suppliedLocalEducationAgencyId+1, Name = "wrong data"},
                            new LocalEducationAgencyInformation{LocalEducationAgencyId = suppliedLocalEducationAgencyId+2, Name = "wrong data"}
                        }).AsQueryable();
        }

        protected override void ExecuteTest()
        {
            service = new BriefService(repository, localEducationAgencyAreaLinks);
            actualModel = service.Get(new BriefRequest { LocalEducationAgencyId = suppliedLocalEducationAgencyId });
        }

        [Test]
        public void Should_return_model_that_is_not_null()
        {
            Assert.That(actualModel, Is.Not.Null);
        }

        [Test]
        public void Should_return_a_default_model()
        {
            Assert.That(actualModel.LocalEducationAgencyId, Is.EqualTo(-1));
            Assert.That(actualModel.Name, Is.EqualTo("No local education agency found."));
            Assert.That(actualModel.ProfileThumbnail, Is.EqualTo(localEducationAgencyAreaLinks.ProfileThumbnail(suppliedLocalEducationAgencyId)));
        }
    }
}
