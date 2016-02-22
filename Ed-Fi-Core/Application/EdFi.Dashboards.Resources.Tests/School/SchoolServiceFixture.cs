// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Testing.NBuilder;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Implementation;
using NUnit.Framework;
using Rhino.Mocks;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Testing;

namespace EdFi.Dashboards.Resources.Tests.School
{
    [TestFixture]
    public class When_loading_school_model : TestFixtureBase
    {
        private SchoolModel actualModel;
        private SchoolService service;

        private SchoolInformation suppliedSchoolInformationData;
        private const int schoolId0 = 1;

        private IRepository<SchoolInformation> schoolInformationRepository;

        protected override void EstablishContext()
        {
            schoolInformationRepository = mocks.StrictMock<IRepository<SchoolInformation>>();

            // Initialize the builder
            BuilderSetup.SetDefaultPropertyNamer(
                new NonDefaultNonRepeatingPropertyNamer(
                    new ReflectionUtil()));

            // Build a Data.StudentInformationModel instance, with hard-coded SchoolId
            suppliedSchoolInformationData = Builder<SchoolInformation>.CreateNew().Build();
            suppliedSchoolInformationData.SchoolId = schoolId0;

            Expect.Call(schoolInformationRepository.GetAll()).Return((new List<SchoolInformation>{suppliedSchoolInformationData}).AsQueryable());
        }

        protected IQueryable<SchoolInformation> GetSuppliedSchoolInformation()
        {
            return (new List<SchoolInformation>
                        {
                            new SchoolInformation()
                        }).AsQueryable();
        }

        protected override void ExecuteTest()
        {
            service = new SchoolService(schoolInformationRepository);
            actualModel = service.Get(new SchoolRequest() { SchoolId = schoolId0 });
        }

        [Test]
        public void Should_return_model_that_is_mapped_correctly()
        {
            Assert.That(actualModel.SchoolId, Is.EqualTo(suppliedSchoolInformationData.SchoolId));
            Assert.That(actualModel.Name, Is.EqualTo(suppliedSchoolInformationData.Name));
            Assert.That(actualModel.SchoolCategory, Is.EqualTo(suppliedSchoolInformationData.SchoolCategory));
            Assert.That(actualModel.AddressLine1, Is.EqualTo(suppliedSchoolInformationData.AddressLine1));
            Assert.That(actualModel.AddressLine2, Is.EqualTo(suppliedSchoolInformationData.AddressLine2));
            Assert.That(actualModel.AddressLine3, Is.EqualTo(suppliedSchoolInformationData.AddressLine3));
            Assert.That(actualModel.City, Is.EqualTo(suppliedSchoolInformationData.City));
            Assert.That(actualModel.State, Is.EqualTo(suppliedSchoolInformationData.State));
            Assert.That(actualModel.ZipCode, Is.EqualTo(suppliedSchoolInformationData.ZipCode));
            Assert.That(actualModel.TelephoneNumber, Is.EqualTo(suppliedSchoolInformationData.TelephoneNumber));
            Assert.That(actualModel.FaxNumber, Is.EqualTo(suppliedSchoolInformationData.FaxNumber));
            Assert.That(actualModel.ProfileThumbnail, Is.EqualTo(suppliedSchoolInformationData.ProfileThumbnail));
            Assert.That(actualModel.WebSite, Is.EqualTo(suppliedSchoolInformationData.WebSite));
        }

        [Test]
        public void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel.EnsureNoDefaultValues();
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }
}
