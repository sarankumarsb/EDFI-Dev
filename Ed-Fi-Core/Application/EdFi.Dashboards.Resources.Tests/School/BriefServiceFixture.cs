// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using NUnit.Framework;
using Rhino.Mocks;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Testing;

namespace EdFi.Dashboards.Resources.Tests.School
{
    [TestFixture]
    public abstract class When_calling_the_school_brief_service : TestFixtureBase
    {
        //The Injected Dependencies.
        protected IRepository<SchoolInformation> schoolInformationRepository;
        protected ISchoolAreaLinks schoolAreaLinks = new SchoolAreaLinksFake();

        //The Actual Model.
        protected BriefModel actualModel;
        protected BriefService service;

        //The supplied Data models.
        protected const int suppliedLocalEducationAgency = 1000;
        protected int suppliedSchoolId = 1;
        
        protected IQueryable<SchoolInformation> suppliedSchoolInformation;
        protected string suppliedImageName;

        protected override void EstablishContext()
        {
            //Set up the mocks
            schoolInformationRepository = mocks.StrictMock<IRepository<SchoolInformation>>();
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }

    public class When_calling_the_school_brief_service_with_a_school_that_exists : When_calling_the_school_brief_service
    {
        protected override void EstablishContext()
        {
            //Call the base.
            base.EstablishContext();

            //Prepare supplied data collections
            suppliedSchoolInformation = GetSuppliedLocalEducationAgencyData();

            //Set expectations
            Expect.Call(schoolInformationRepository.GetAll()).Return(suppliedSchoolInformation);
        }

        protected MetricMetadataNode GetSuppliedRootNode()
        {
            return new MetricMetadataNode(new TestMetricMetadataTree()) { MetricNodeId = 1 };
        }

        protected IQueryable<SchoolInformation> GetSuppliedLocalEducationAgencyData()
        {
            return (new List<SchoolInformation>
                        {
                            new SchoolInformation{ SchoolId = suppliedSchoolId, Name = "My School", LocalEducationAgencyId = suppliedLocalEducationAgency}
                        }).AsQueryable();
        }

        protected override void ExecuteTest()
        {
            service = new BriefService(schoolInformationRepository, schoolAreaLinks);
            actualModel = service.Get(new BriefRequest {SchoolId = suppliedSchoolId});
        }

        [Test]
        public void Should_return_model_that_is_not_null()
        {
            Assert.That(actualModel, Is.Not.Null);
        }

        [Test]
        public void Should_return_model_that_mapped_correctly()
        {
            var suppliedSchool = suppliedSchoolInformation.Single();

            Assert.That(actualModel.Name, Is.EqualTo(suppliedSchool.Name));
            Assert.That(actualModel.SchoolId, Is.EqualTo(suppliedSchool.SchoolId));
            Assert.That(actualModel.ProfileThumbnail, Is.EqualTo(schoolAreaLinks.ProfileThumbnail(suppliedSchoolId)));
            Assert.That(actualModel.Url, Is.EqualTo(schoolAreaLinks.Default(suppliedSchoolId, suppliedSchool.Name)));
            Assert.That(actualModel.Links.Count(), Is.EqualTo(0));//Today we don't have any additional links on this resource.
        }

        [Test]
        public void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel.EnsureNoDefaultValues("BriefModel.ResourceUrl", "BriefModel.Links");
        }
    }

    public class When_calling_the_school_brief_service_with_a_school_that_does_not_exists : When_calling_the_school_brief_service
    {

        protected override void EstablishContext()
        {
            suppliedSchoolId = -1;

            base.EstablishContext();

            schoolInformationRepository = mocks.StrictMock<IRepository<SchoolInformation>>();

            suppliedSchoolInformation = GetSuppliedLocalEducationAgencyData();

            Expect.Call(schoolInformationRepository.GetAll()).Return(suppliedSchoolInformation);
        }

        protected MetricMetadataNode GetSuppliedRootNode()
        {
            return new MetricMetadataNode(new TestMetricMetadataTree()) { MetricNodeId = 1 };
        }

        protected IQueryable<SchoolInformation> GetSuppliedLocalEducationAgencyData()
        {
            return (new List<SchoolInformation>()).AsQueryable();
        }

        protected override void ExecuteTest()
        {
            service = new BriefService(schoolInformationRepository, schoolAreaLinks);
            actualModel = service.Get(new BriefRequest { SchoolId = suppliedSchoolId });
        }

        [Test]
        public void Should_return_model_that_is_not_null()
        {
            Assert.That(actualModel, Is.Not.Null);
        }

        [Test]
        public void Should_return_a_default_model()
        {
            var suppliedModel = new BriefModel
                                    {
                                        SchoolId = suppliedSchoolId,
                                        Name = "No school found.",
                                        ProfileThumbnail = schoolAreaLinks.ProfileThumbnail(suppliedSchoolId)
                                    };

            Assert.That(actualModel.Name, Is.EqualTo(suppliedModel.Name));
            Assert.That(actualModel.SchoolId, Is.EqualTo(suppliedModel.SchoolId));
            Assert.That(actualModel.ProfileThumbnail, Is.EqualTo(suppliedModel.ProfileThumbnail));
        }
    }
}
