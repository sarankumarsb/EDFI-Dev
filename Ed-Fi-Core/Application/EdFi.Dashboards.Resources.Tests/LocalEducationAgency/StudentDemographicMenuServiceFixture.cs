using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.LocalEducationAgency
{
    public class When_getting_student_demographic_menu : TestFixtureBase
    {
        private IRepository<LocalEducationAgencyStudentDemographic> LocalEducationAgencyStudentDemographicRepository;
        private IRepository<LocalEducationAgencyProgramPopulation> LocalEducationAgencyProgramPopulationRepository;
        private IRepository<LocalEducationAgencyIndicatorPopulation> LocalEducationAgencyIndicatorPopulationRepository;
        private ILocalEducationAgencyAreaLinks LocalEducationAgencyAreaLinks;
		private IMetricsBasedWatchListMenuService watchListMenuService;

        private const int suppliedLocalEducationAgencyId = 1000;
        private StudentDemographicMenuModel actualModel;
		private StudentListMenuModel menuModel1;
		private StudentListMenuModel menuModel2;

        protected override void EstablishContext()
        {
			var watchlistRequest = new MetricsBasedWatchListMenuRequest();

            LocalEducationAgencyAreaLinks = new LocalEducationAgencyAreaLinksFake();
            LocalEducationAgencyStudentDemographicRepository = mocks.StrictMock<IRepository<LocalEducationAgencyStudentDemographic>>();
            LocalEducationAgencyProgramPopulationRepository = mocks.StrictMock<IRepository<LocalEducationAgencyProgramPopulation>>();
            LocalEducationAgencyIndicatorPopulationRepository = mocks.StrictMock<IRepository<LocalEducationAgencyIndicatorPopulation>>();
	        watchListMenuService = mocks.StrictMock<IMetricsBasedWatchListMenuService>();

            Expect.Call(LocalEducationAgencyStudentDemographicRepository.GetAll()).Return(GetLocalEducationAgencyStudentDemographic());
            Expect.Call(LocalEducationAgencyProgramPopulationRepository.GetAll()).Return(GetLocalEducationAgencyProgramPopulation());
            Expect.Call(LocalEducationAgencyIndicatorPopulationRepository.GetAll()).Return(GetLocalEducationAgencyIndicatorPopulation());

			menuModel1 = new StudentListMenuModel
			{
				Description = "Dynamic List 1",
				Href = "List1Url",
				MenuType = "PageDemographic",
				SectionId = 1,
				ListType = StudentListType.MetricsBasedWatchList,
				Selected = false
			};

			menuModel2 = new StudentListMenuModel
			{
				Description = "Dynamic List 2",
				Href = "List2Url",
				MenuType = "PageDemographic",
				SectionId = 2,
				ListType = StudentListType.MetricsBasedWatchList,
				Selected = false
			};

			Expect.Call(watchListMenuService.Get(watchlistRequest)).IgnoreArguments().Repeat.Any().Return(new List<StudentListMenuModel> { menuModel1, menuModel2 });

            base.EstablishContext();
        }

        private IQueryable<LocalEducationAgencyStudentDemographic> GetLocalEducationAgencyStudentDemographic()
        {
            var list = new List<LocalEducationAgencyStudentDemographic>
                           {
                               new LocalEducationAgencyStudentDemographic { Attribute = "Demographic999", Value = .4m, LocalEducationAgencyId = suppliedLocalEducationAgencyId + 1, DisplayOrder = 1},
                               new LocalEducationAgencyStudentDemographic { Attribute = "Female", Value = .4m, LocalEducationAgencyId = suppliedLocalEducationAgencyId, DisplayOrder = 1},
                               new LocalEducationAgencyStudentDemographic { Attribute = "Male", Value = .5m, LocalEducationAgencyId = suppliedLocalEducationAgencyId, DisplayOrder = 2},
                               new LocalEducationAgencyStudentDemographic { Attribute = "Hispanic/Latino", Value = .2m, LocalEducationAgencyId = suppliedLocalEducationAgencyId, DisplayOrder = 3},
                               new LocalEducationAgencyStudentDemographic { Attribute = "Demographic3", Value = .2m, LocalEducationAgencyId = suppliedLocalEducationAgencyId, DisplayOrder = 6},
                               new LocalEducationAgencyStudentDemographic { Attribute = "Demographic1", Value = null, LocalEducationAgencyId = suppliedLocalEducationAgencyId, DisplayOrder = 4},
                               new LocalEducationAgencyStudentDemographic { Attribute = "Demographic2", Value = .2m, LocalEducationAgencyId = suppliedLocalEducationAgencyId, DisplayOrder = 5},
                           };
            return list.AsQueryable();
        }

        private IQueryable<LocalEducationAgencyProgramPopulation> GetLocalEducationAgencyProgramPopulation()
        {
            var list = new List<LocalEducationAgencyProgramPopulation>
                           {
                               new LocalEducationAgencyProgramPopulation { Attribute = "Program999", Value = .4m, LocalEducationAgencyId = suppliedLocalEducationAgencyId + 1, DisplayOrder = 1},
                               new LocalEducationAgencyProgramPopulation { Attribute = "Program3", Value = .4m, LocalEducationAgencyId = suppliedLocalEducationAgencyId, DisplayOrder = 3},
                               new LocalEducationAgencyProgramPopulation { Attribute = "Program1", Value = null, LocalEducationAgencyId = suppliedLocalEducationAgencyId, DisplayOrder = 1},
                               new LocalEducationAgencyProgramPopulation { Attribute = "Program2", Value = .4m, LocalEducationAgencyId = suppliedLocalEducationAgencyId, DisplayOrder = 2},
                           };
            return list.AsQueryable();
        }

        private IQueryable<LocalEducationAgencyIndicatorPopulation> GetLocalEducationAgencyIndicatorPopulation()
        {
            var list = new List<LocalEducationAgencyIndicatorPopulation>
                           {
                               new LocalEducationAgencyIndicatorPopulation { Attribute = "Indicator999", Value = .4m, LocalEducationAgencyId = suppliedLocalEducationAgencyId + 1, DisplayOrder = 1},
                               new LocalEducationAgencyIndicatorPopulation { Attribute = "Indicator3", Value = .4m, LocalEducationAgencyId = suppliedLocalEducationAgencyId, DisplayOrder = 3},
                               new LocalEducationAgencyIndicatorPopulation { Attribute = "Indicator1", Value = null, LocalEducationAgencyId = suppliedLocalEducationAgencyId, DisplayOrder = 1},
                               new LocalEducationAgencyIndicatorPopulation { Attribute = "Indicator2", Value = .4m, LocalEducationAgencyId = suppliedLocalEducationAgencyId, DisplayOrder = 2},
                           };
            return list.AsQueryable();
        }

        protected override void ExecuteTest()
        {
			var service = new StudentDemographicMenuService(LocalEducationAgencyStudentDemographicRepository, LocalEducationAgencyProgramPopulationRepository, LocalEducationAgencyIndicatorPopulationRepository, LocalEducationAgencyAreaLinks, watchListMenuService);
            actualModel = service.Get(StudentDemographicMenuRequest.Create(suppliedLocalEducationAgencyId, long.MinValue));
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

		[Test]
		public void Should_load_dynamic_lists()
		{
			Assert.That(actualModel.WatchLists.Count, Is.EqualTo(2));
			TestAttributeAndUrl(actualModel.WatchLists.ElementAt(0), menuModel1.Description, menuModel1.Href);
			TestAttributeAndUrl(actualModel.WatchLists.ElementAt(1), menuModel2.Description, menuModel2.Href);
		}

	    private void TestAttributeAndUrl(AttributeItemWithUrl<string> model, string attribute, string url)
	    {
			Assert.That(model.Attribute, Is.EqualTo(attribute));
			Assert.That(model.Url, Is.EqualTo(url));
	    }

	    private void TestAttributeAndUrl(AttributeItemWithUrl<decimal> model, string attribute)
        {
            Assert.That(model.Attribute, Is.EqualTo(attribute));
            Assert.That(model.Url, Is.EqualTo(LocalEducationAgencyAreaLinks.StudentDemographicList(suppliedLocalEducationAgencyId, attribute)));
            
        }
    }
}
