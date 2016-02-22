using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.LocalEducationAgency
{
    public class When_getting_student_school_category_menu : TestFixtureBase
    {
        protected IRepository<SchoolGradePopulation> schoolGradePopulationRepository;
        protected IRepository<SchoolInformation> schoolInformationRepository;
        protected ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks;
        protected ISchoolCategoryProvider schoolCategoryProvider;
		protected IMetricsBasedWatchListMenuService watchListMenuService;

        private const int suppliedLocalEducationAgencyId = 500;
        private const int suppliedSchoolId1 = 1000;
        private const int suppliedSchoolId2 = 1001;
        private const int suppliedSchoolId3 = 1002;
        private const int suppliedSchoolId4 = 1003;
        private const string suppliedSchoolCategory1 = "AA School Category 1";
        private const string suppliedSchoolCategory2 = "Ungraded";
        private const string suppliedSchoolCategory3 = "School Category 3";
        private StudentSchoolCategoryMenuModel actualModel;
		private StudentListMenuModel menuModel1;
		private StudentListMenuModel menuModel2;

        protected override void EstablishContext()
        {
	        var watchlistRequest = new MetricsBasedWatchListMenuRequest();

            localEducationAgencyAreaLinks = new LocalEducationAgencyAreaLinksFake();
            schoolInformationRepository = mocks.StrictMock<IRepository<SchoolInformation>>();
            schoolGradePopulationRepository = mocks.StrictMock<IRepository<SchoolGradePopulation>>();
            schoolCategoryProvider = mocks.StrictMock<ISchoolCategoryProvider>();
	        watchListMenuService = mocks.StrictMock<IMetricsBasedWatchListMenuService>();

            Expect.Call(schoolInformationRepository.GetAll()).Return(GetSchoolInformation());
            Expect.Call(schoolGradePopulationRepository.GetAll()).Return(GetSchoolGradePopulation());
            Expect.Call(schoolCategoryProvider.GetSchoolCategoryPriorityForSorting(suppliedSchoolCategory1)).Repeat.Any().Return(2);
            Expect.Call(schoolCategoryProvider.GetSchoolCategoryPriorityForSorting(suppliedSchoolCategory2)).Repeat.Any().Return(1);
			
	        menuModel1 = new StudentListMenuModel
	        {
		        Description = "Dynamic List 1",
		        Href = "List1Url",
		        MenuType = "PageSchoolCategory",
		        SectionId = 1,
		        ListType = StudentListType.MetricsBasedWatchList,
		        Selected = false
	        };

			menuModel2 = new StudentListMenuModel
			{
				Description = "Dynamic List 2",
				Href = "List2Url",
				MenuType = "PageSchoolCategory",
				SectionId = 2,
				ListType = StudentListType.MetricsBasedWatchList,
				Selected = false
			};

	        Expect.Call(watchListMenuService.Get(watchlistRequest)).IgnoreArguments().Repeat.Any().Return(new List<StudentListMenuModel> {menuModel1, menuModel2});
            
            base.EstablishContext();
        }

        private IQueryable<SchoolInformation> GetSchoolInformation()
        {
            var list = new List<SchoolInformation>
                           {
                               new SchoolInformation {LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId1, SchoolCategory = suppliedSchoolCategory1},
                               new SchoolInformation {LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId2, SchoolCategory = suppliedSchoolCategory2},
                               new SchoolInformation {LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId3, SchoolCategory = suppliedSchoolCategory1},
                               new SchoolInformation {LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId4, SchoolCategory = suppliedSchoolCategory3},
                               new SchoolInformation {LocalEducationAgencyId = suppliedLocalEducationAgencyId + 1, SchoolId = 999, SchoolCategory = "wrong data"},
                               new SchoolInformation {LocalEducationAgencyId = suppliedLocalEducationAgencyId + 1, SchoolId = 998, SchoolCategory = "wrong data2"},
                           };
            return list.AsQueryable();
        }

        private IQueryable<SchoolGradePopulation> GetSchoolGradePopulation()
        {
            var list = new List<SchoolGradePopulation>
                           {
                               new SchoolGradePopulation { SchoolId = suppliedSchoolId1 },
                               new SchoolGradePopulation { SchoolId = suppliedSchoolId2 },
                               new SchoolGradePopulation { SchoolId = suppliedSchoolId3 },
                           };
            return list.AsQueryable();
        }

        protected override void ExecuteTest()
        {
			var service = new StudentSchoolCategoryMenuService(schoolInformationRepository, schoolGradePopulationRepository, schoolCategoryProvider, localEducationAgencyAreaLinks, watchListMenuService);
            actualModel = service.Get(StudentSchoolCategoryMenuRequest.Create(suppliedLocalEducationAgencyId, long.MinValue));
        }

        [Test]
        public void Should_load_school_categories()
        {
            Assert.That(actualModel.SchoolCategories.Count, Is.EqualTo(2));
			TestAttributeAndUrl(actualModel.SchoolCategories.ElementAt(1), suppliedSchoolCategory1, suppliedSchoolCategory1, localEducationAgencyAreaLinks.StudentSchoolCategoryList(suppliedLocalEducationAgencyId, suppliedSchoolCategory1));
        }

		[Test]
		public void Should_load_dynamic_lists()
		{
			Assert.That(actualModel.DynamicWatchLists.Count, Is.EqualTo(2));
			TestAttributeAndUrl(actualModel.DynamicWatchLists.ElementAt(1), menuModel2.Description, menuModel2.SectionId.ToString(), menuModel2.Href);
		}

        private void TestAttributeAndUrl(AttributeItemWithUrl<string> model, string attribute, string value, string url)
        {
            Assert.That(model.Attribute, Is.EqualTo(attribute));
            Assert.That(model.Value, Is.EqualTo(value));
            Assert.That(model.Url, Is.EqualTo(url));
        }
    }
}
