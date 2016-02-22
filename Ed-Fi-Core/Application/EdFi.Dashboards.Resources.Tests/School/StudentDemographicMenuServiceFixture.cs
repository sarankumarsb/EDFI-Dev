using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.School
{
    public class When_getting_student_demographic_menu : TestFixtureBase
    {
        private IRepository<SchoolStudentDemographic> schoolStudentDemographicRepository;
        private IRepository<SchoolProgramPopulation> schoolProgramPopulationRepository;
        private IRepository<SchoolIndicatorPopulation> schoolIndicatorPopulationRepository;
        private ISchoolAreaLinks schoolAreaLinks;

        private const int suppliedSchoolId = 1000;
        private StudentDemographicMenuModel actualModel;

        protected override void EstablishContext()
        {
            schoolAreaLinks = new SchoolAreaLinksFake();
            schoolStudentDemographicRepository = mocks.StrictMock<IRepository<SchoolStudentDemographic>>();
            schoolProgramPopulationRepository = mocks.StrictMock<IRepository<SchoolProgramPopulation>>();
            schoolIndicatorPopulationRepository = mocks.StrictMock<IRepository<SchoolIndicatorPopulation>>();

            Expect.Call(schoolStudentDemographicRepository.GetAll()).Return(GetSchoolStudentDemographic());
            Expect.Call(schoolProgramPopulationRepository.GetAll()).Return(GetSchoolProgramPopulation());
            Expect.Call(schoolIndicatorPopulationRepository.GetAll()).Return(GetSchoolIndicatorPopulation());

            base.EstablishContext();
        }

        private IQueryable<SchoolStudentDemographic> GetSchoolStudentDemographic()
        {
            var list = new List<SchoolStudentDemographic>
                           {
                               new SchoolStudentDemographic { Attribute = "Demographic999", Value = .4m, SchoolId = suppliedSchoolId + 1, DisplayOrder = 1},
                               new SchoolStudentDemographic { Attribute = "Female", Value = .4m, SchoolId = suppliedSchoolId, DisplayOrder = 1},
                               new SchoolStudentDemographic { Attribute = "Male", Value = .5m, SchoolId = suppliedSchoolId, DisplayOrder = 2},
                               new SchoolStudentDemographic { Attribute = "Hispanic/Latino", Value = .2m, SchoolId = suppliedSchoolId, DisplayOrder = 3},
                               new SchoolStudentDemographic { Attribute = "Demographic3", Value = .2m, SchoolId = suppliedSchoolId, DisplayOrder = 6},
                               new SchoolStudentDemographic { Attribute = "Demographic1", Value = null, SchoolId = suppliedSchoolId, DisplayOrder = 4},
                               new SchoolStudentDemographic { Attribute = "Demographic2", Value = .2m, SchoolId = suppliedSchoolId, DisplayOrder = 5},
                           };
            return list.AsQueryable();
        }

        private IQueryable<SchoolProgramPopulation> GetSchoolProgramPopulation()
        {
            var list = new List<SchoolProgramPopulation>
                           {
                               new SchoolProgramPopulation { Attribute = "Program999", Value = .4m, SchoolId = suppliedSchoolId + 1, DisplayOrder = 1},
                               new SchoolProgramPopulation { Attribute = "Program3", Value = .4m, SchoolId = suppliedSchoolId, DisplayOrder = 3},
                               new SchoolProgramPopulation { Attribute = "Program1", Value = null, SchoolId = suppliedSchoolId, DisplayOrder = 1},
                               new SchoolProgramPopulation { Attribute = "Program2", Value = .4m, SchoolId = suppliedSchoolId, DisplayOrder = 2},
                           };
            return list.AsQueryable();
        }

        private IQueryable<SchoolIndicatorPopulation> GetSchoolIndicatorPopulation()
        {
            var list = new List<SchoolIndicatorPopulation>
                           {
                               new SchoolIndicatorPopulation { Attribute = "Indicator999", Value = .4m, SchoolId = suppliedSchoolId + 1, DisplayOrder = 1},
                               new SchoolIndicatorPopulation { Attribute = "Indicator3", Value = .4m, SchoolId = suppliedSchoolId, DisplayOrder = 3},
                               new SchoolIndicatorPopulation { Attribute = "Indicator1", Value = null, SchoolId = suppliedSchoolId, DisplayOrder = 1},
                               new SchoolIndicatorPopulation { Attribute = "Indicator2", Value = .4m, SchoolId = suppliedSchoolId, DisplayOrder = 2},
                           };
            return list.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new StudentDemographicMenuService(schoolStudentDemographicRepository, schoolProgramPopulationRepository, schoolIndicatorPopulationRepository, schoolAreaLinks);
            actualModel = service.Get(StudentDemographicMenuRequest.Create(suppliedSchoolId));
        }

        [Test]
        public void Should_load_gender_options()
        {
            Assert.That(actualModel.Gender.Count, Is.EqualTo(2));
            TestAttributeAndUrl(actualModel.Gender.ElementAt(0), "Female");
            TestAttributeAndUrl(actualModel.Gender.ElementAt(1), "Male");
        }

        [Test]
        public void Should_load_ethnicity_options()
        {
            Assert.That(actualModel.Ethnicity.Count, Is.EqualTo(1));
            TestAttributeAndUrl(actualModel.Ethnicity.ElementAt(0), "Hispanic/Latino");
        }

        [Test]
        public void Should_load_race_options()
        {
            Assert.That(actualModel.Race.Count, Is.EqualTo(2));
            TestAttributeAndUrl(actualModel.Race.ElementAt(0), "Demographic2");
            TestAttributeAndUrl(actualModel.Race.ElementAt(1), "Demographic3");
        }

        [Test]
        public void Should_load_program_options()
        {
            Assert.That(actualModel.Program.Count, Is.EqualTo(2));
            TestAttributeAndUrl(actualModel.Program.ElementAt(0), "Program2");
            TestAttributeAndUrl(actualModel.Program.ElementAt(1), "Program3");
        }

        [Test]
        public void Should_load_indicator_options()
        {
            Assert.That(actualModel.Indicator.Count, Is.EqualTo(3));
            TestAttributeAndUrl(actualModel.Indicator.ElementAt(0), "Indicator2");
            TestAttributeAndUrl(actualModel.Indicator.ElementAt(1), "Indicator3");
        }

        [Test]
        public void Should_load_late_enrollment_option()
        {
            TestAttributeAndUrl(actualModel.Indicator.Last(), "Late Enrollment");
        }

        private void TestAttributeAndUrl(AttributeItemWithUrl<decimal> model, string attribute)
        {
            Assert.That(model.Attribute, Is.EqualTo(attribute));
            Assert.That(model.Url, Is.EqualTo(schoolAreaLinks.StudentDemographicList(suppliedSchoolId, attribute)));
            
        }
    }
}
