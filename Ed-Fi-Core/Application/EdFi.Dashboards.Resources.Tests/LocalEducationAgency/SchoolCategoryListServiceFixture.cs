// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Navigation.Support;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;
using IIdNameService = EdFi.Dashboards.Resources.School.IIdNameService;
using MockRepository = Rhino.Mocks.MockRepository;

namespace EdFi.Dashboards.Resources.Tests.LocalEducationAgency
{

    [TestFixture]
    public class When_building_school_category_model_list : TestFixtureBase
    {
        //The Injected Dependencies.
        private IRepository<SchoolInformation> repository;
        private IRepository<SchoolGradePopulation> schoolGradePopulationRepository;
        private IUniqueListIdProvider uniqueListIdProvider;
        protected ISchoolCategoryProvider schoolCategoryProvider;

        private IQueryable<SchoolInformation> suppliedSchoolsInformation;
        private IQueryable<SchoolGradePopulation> suppliedSchoolGradePopulation;
        private const int suppliedLocalEducationAgencyId = 1;
        private const string suppliedUniqueListId = "myList";

        private IList<SchoolCategoryModel> actualModel;
        private SchoolAreaLinksFake schoolAreaLinksFake = new SchoolAreaLinksFake();

        protected override void EstablishContext()
        {
            repository = mocks.StrictMock<IRepository<SchoolInformation>>();
            schoolGradePopulationRepository = mocks.StrictMock<IRepository<SchoolGradePopulation>>();
            uniqueListIdProvider = mocks.StrictMock<IUniqueListIdProvider>();
            schoolCategoryProvider = mocks.StrictMock<ISchoolCategoryProvider>();

            suppliedSchoolsInformation = GetSuppliedSchoolInformation();
            suppliedSchoolGradePopulation = GetSuppliedSchoolGradePopulation();

            Expect.Call(schoolCategoryProvider.GetSchoolCategoryPriorityForSorting("High School")).Repeat.Any().Return(1);
            Expect.Call(schoolCategoryProvider.GetSchoolCategoryPriorityForSorting("Junior High School")).Repeat.Any().Return(2);
            Expect.Call(schoolCategoryProvider.GetSchoolCategoryPriorityForSorting("Middle School")).Repeat.Any().Return(3);
            Expect.Call(schoolCategoryProvider.GetSchoolCategoryPriorityForSorting("Elementary School")).Repeat.Any().Return(4);
            Expect.Call(schoolCategoryProvider.GetSchoolCategoryPriorityForSorting("Ungraded")).Repeat.Any().Return(6);
            Expect.Call(repository.GetAll()).Return(suppliedSchoolsInformation);
            Expect.Call(schoolGradePopulationRepository.GetAll()).Repeat.Times(7).Return(suppliedSchoolGradePopulation);
            Expect.Call(uniqueListIdProvider.GetUniqueId()).Return(suppliedUniqueListId);
        }

        protected IQueryable<SchoolInformation> GetSuppliedSchoolInformation()
        {
            var list = new List<SchoolInformation>
                        {
                            new SchoolInformation { SchoolId = 5, LocalEducationAgencyId = suppliedLocalEducationAgencyId, Name = "School 5", SchoolCategory="Ungraded"},
                            new SchoolInformation { SchoolId = 4, LocalEducationAgencyId = suppliedLocalEducationAgencyId, Name = "School 4", SchoolCategory="Elementary School"},
                            new SchoolInformation { SchoolId = 1, LocalEducationAgencyId = suppliedLocalEducationAgencyId, Name = "School Yak", SchoolCategory="High School"},
                            new SchoolInformation { SchoolId = 2, LocalEducationAgencyId = suppliedLocalEducationAgencyId, Name = "School Apple", SchoolCategory="High School"},
                            new SchoolInformation { SchoolId = 6, LocalEducationAgencyId = suppliedLocalEducationAgencyId, Name = "School Banana", SchoolCategory="High School"},
                            new SchoolInformation { SchoolId = 3, LocalEducationAgencyId = suppliedLocalEducationAgencyId, Name = "School 3", SchoolCategory="Middle School"},
                            new SchoolInformation { SchoolId = 7, LocalEducationAgencyId = suppliedLocalEducationAgencyId, Name = "School 7", SchoolCategory="No Students"},
                            new SchoolInformation { SchoolId = 8, LocalEducationAgencyId = suppliedLocalEducationAgencyId + 1, Name = "Wrong Data", SchoolCategory="Wrong Local Education Agency"},
                        };
            return list.AsQueryable();
        }

        protected IQueryable<SchoolGradePopulation> GetSuppliedSchoolGradePopulation()
        {
            var list = new List<SchoolGradePopulation>
                        {
                            new SchoolGradePopulation { SchoolId = 5},
                            new SchoolGradePopulation { SchoolId = 4},
                            new SchoolGradePopulation { SchoolId = 1},
                            new SchoolGradePopulation { SchoolId = 2},
                            new SchoolGradePopulation { SchoolId = 6},
                            new SchoolGradePopulation { SchoolId = 6},
                            new SchoolGradePopulation { SchoolId = 6},
                            new SchoolGradePopulation { SchoolId = 3},
                            new SchoolGradePopulation { SchoolId = 8}
                        };
            return list.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new SchoolCategoryListService(repository, schoolGradePopulationRepository, schoolAreaLinksFake, uniqueListIdProvider, schoolCategoryProvider);
            actualModel = service.Get(SchoolCategoryListRequest.Create(suppliedLocalEducationAgencyId));
        }

        [Test]
        public void Should_load_school_categories_correctly()
        {
            Assert.That(actualModel.Count(), Is.EqualTo(suppliedSchoolsInformation.GroupBy(x => x.SchoolCategory).Count() - 2));

            var comparer = new SchoolCategoryComparer(schoolCategoryProvider);

            string previousSchoolCategoryName = String.Empty;

            foreach (var schoolCategoryModel in actualModel)
            {
                var schoolCategory = schoolCategoryModel.Category;

                if (!string.IsNullOrEmpty(previousSchoolCategoryName))
                    Assert.That(comparer.Compare(schoolCategory, previousSchoolCategoryName), Is.EqualTo(1));

                previousSchoolCategoryName = schoolCategory;

                Assert.That(schoolCategoryModel.Schools.Count(), Is.EqualTo(suppliedSchoolsInformation.Count(x => x.SchoolCategory == schoolCategory)), "schoolCategory.Name = " + schoolCategoryModel.Category);
            }
        }

        [Test]
        public void Should_load_schools_correctly()
        {
            foreach (var schoolCategoryModel in actualModel)
            {
                var schoolCategory = schoolCategoryModel.Category;
                string previousSchoolCategoryName = String.Empty;

                foreach (var school in schoolCategoryModel.Schools)
                {
                    if (!string.IsNullOrEmpty(previousSchoolCategoryName))
                        Assert.That(school.Name, Is.GreaterThan(previousSchoolCategoryName));

                    previousSchoolCategoryName = school.Name;

                    SchoolCategoryModel.School school1 = school;
                    var suppliedSchoolInformation = suppliedSchoolsInformation.Single(x => x.SchoolId == school1.SchoolId);

                    Assert.That(schoolCategory, Is.EqualTo(suppliedSchoolInformation.SchoolCategory));
                    Assert.That(school.SchoolId, Is.EqualTo(suppliedSchoolInformation.SchoolId));
                    Assert.That(school.Name, Is.EqualTo(suppliedSchoolInformation.Name));
                    Assert.That(school.Url, Is.EqualTo(schoolAreaLinksFake.Default(suppliedSchoolInformation.SchoolId, suppliedSchoolInformation.Name).AppendParameters("listContext=" + suppliedUniqueListId)));
                    Assert.That(school.Links.Count(), Is.EqualTo(2));

                    var link = school.Links.SingleOrDefault(x => x.Rel == "Students");
                    Assert.IsNotNull(link);
                    Assert.That(link.Href, Is.EqualTo(schoolAreaLinksFake.StudentGradeList(suppliedSchoolInformation.SchoolId, suppliedSchoolInformation.Name).AppendParameters("listContext=" + suppliedUniqueListId)));

                    link = school.Links.SingleOrDefault(x => x.Rel == "Teachers");
                    Assert.IsNotNull(link);
                    Assert.That(link.Href, Is.EqualTo(schoolAreaLinksFake.Teachers(suppliedSchoolInformation.SchoolId, suppliedSchoolInformation.Name).AppendParameters("listContext=" + suppliedUniqueListId)));
                }
            }
        }

        [Test]
        public void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel[0].EnsureNoDefaultValues(
                "SchoolCategoryModel.ResourceUrl",
                "SchoolCategoryModel.Links",
                "SchoolCategoryModel.Schools[0].ResourceUrl",
                "SchoolCategoryModel.Schools[1].ResourceUrl",
                "SchoolCategoryModel.Schools[2].ResourceUrl");
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }

    [TestFixture]
    public class When_sorting_school_category : TestFixtureBase
    {
        private IIdNameService idNameService;
        private ISchoolCategoryProvider schoolCategoryProvider;
        private SchoolCategoryComparer comparer;
        private const string highSchool = "High School";
        private const string juniorHighSchool = "Junior High School";
        private const string middleSchool = "Middle School";
        private const string elementary = "Elementary School";
        private const string elementarySecondarySchool = "Elementary/Secondary School";
        private const string ungraded = "Ungraded";
        private const string secondarySchool = "SecondarySchool";
        private const string adultSchool = "Adult School";
        private const string infantToddlerSchool = "Infant/toddler School";
        private const string preschoolEarlyChildhood = "Preschool/early childhood";
        private const string primarySchool = "Primary School";
        private const string intermediateSchool = "Intermediate School";
        private const string unknown = "Apple Banana Orange";

        protected override void EstablishContext()
        {
            idNameService = mocks.StrictMock<IIdNameService>();
            schoolCategoryProvider = new SchoolCategoryProvider(idNameService);
            comparer = new SchoolCategoryComparer(schoolCategoryProvider);
        }

        protected override void ExecuteTest()
        {
        }

        [Test]
        public void Should_sort_high_school_first()
        {
            Assert.That(comparer.Compare(highSchool, highSchool), Is.EqualTo(0));
            Assert.That(comparer.Compare(highSchool, juniorHighSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(highSchool, middleSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(highSchool, elementary), Is.EqualTo(-1));
            Assert.That(comparer.Compare(highSchool, elementarySecondarySchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(highSchool, ungraded), Is.EqualTo(-1));
            Assert.That(comparer.Compare(highSchool, secondarySchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(highSchool, adultSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(highSchool, infantToddlerSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(highSchool, preschoolEarlyChildhood), Is.EqualTo(-1));
            Assert.That(comparer.Compare(highSchool, primarySchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(highSchool, intermediateSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(highSchool, unknown), Is.EqualTo(-1));
        }

        [Test]
        public void Should_sort_junior_high_school_second()
        {
            Assert.That(comparer.Compare(juniorHighSchool, highSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(juniorHighSchool, juniorHighSchool), Is.EqualTo(0));
            Assert.That(comparer.Compare(juniorHighSchool, middleSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(juniorHighSchool, elementary), Is.EqualTo(-1));
            Assert.That(comparer.Compare(juniorHighSchool, elementarySecondarySchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(juniorHighSchool, ungraded), Is.EqualTo(-1));
            Assert.That(comparer.Compare(juniorHighSchool, secondarySchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(juniorHighSchool, adultSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(juniorHighSchool, infantToddlerSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(juniorHighSchool, preschoolEarlyChildhood), Is.EqualTo(-1));
            Assert.That(comparer.Compare(juniorHighSchool, primarySchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(juniorHighSchool, intermediateSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(juniorHighSchool, unknown), Is.EqualTo(-1));
        }

        [Test]
        public void Should_sort_middle_school_third()
        {
            Assert.That(comparer.Compare(middleSchool, highSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(middleSchool, juniorHighSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(middleSchool, middleSchool), Is.EqualTo(0));
            Assert.That(comparer.Compare(middleSchool, elementary), Is.EqualTo(-1));
            Assert.That(comparer.Compare(middleSchool, elementarySecondarySchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(middleSchool, ungraded), Is.EqualTo(-1));
            Assert.That(comparer.Compare(middleSchool, secondarySchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(middleSchool, adultSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(middleSchool, infantToddlerSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(middleSchool, preschoolEarlyChildhood), Is.EqualTo(-1));
            Assert.That(comparer.Compare(middleSchool, primarySchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(middleSchool, intermediateSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(middleSchool, unknown), Is.EqualTo(-1));
        }

        [Test]
        public void Should_sort_elementary_school_fourth()
        {
            Assert.That(comparer.Compare(elementary, highSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(elementary, juniorHighSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(elementary, middleSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(elementary, elementary), Is.EqualTo(0));
            Assert.That(comparer.Compare(elementary, elementarySecondarySchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(elementary, ungraded), Is.EqualTo(-1));
            Assert.That(comparer.Compare(elementary, secondarySchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(elementary, adultSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(elementary, infantToddlerSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(elementary, preschoolEarlyChildhood), Is.EqualTo(-1));
            Assert.That(comparer.Compare(elementary, primarySchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(elementary, intermediateSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(elementary, unknown), Is.EqualTo(-1));
        }

        [Test]
        public void Should_sort_secondary_school_fifth()
        {
            Assert.That(comparer.Compare(elementarySecondarySchool, highSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(elementarySecondarySchool, juniorHighSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(elementarySecondarySchool, middleSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(elementarySecondarySchool, elementary), Is.EqualTo(1));
            Assert.That(comparer.Compare(elementarySecondarySchool, elementarySecondarySchool), Is.EqualTo(0));
            Assert.That(comparer.Compare(elementarySecondarySchool, ungraded), Is.EqualTo(-1));
            Assert.That(comparer.Compare(elementarySecondarySchool, secondarySchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(elementarySecondarySchool, adultSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(elementarySecondarySchool, infantToddlerSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(elementarySecondarySchool, preschoolEarlyChildhood), Is.EqualTo(-1));
            Assert.That(comparer.Compare(elementarySecondarySchool, primarySchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(elementarySecondarySchool, intermediateSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(elementarySecondarySchool, unknown), Is.EqualTo(-1));
        }

        [Test]
        public void Should_sort_ungraded_sixth()
        {
            Assert.That(comparer.Compare(ungraded, highSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(ungraded, juniorHighSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(ungraded, middleSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(ungraded, elementary), Is.EqualTo(1));
            Assert.That(comparer.Compare(ungraded, elementarySecondarySchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(ungraded, secondarySchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(ungraded, ungraded), Is.EqualTo(0));
            Assert.That(comparer.Compare(ungraded, adultSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(ungraded, infantToddlerSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(ungraded, preschoolEarlyChildhood), Is.EqualTo(-1));
            Assert.That(comparer.Compare(ungraded, primarySchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(ungraded, intermediateSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(ungraded, unknown), Is.EqualTo(-1));
        }

        [Test]
        public void Should_sort_SecondarySchool_seventh()
        {
            Assert.That(comparer.Compare(secondarySchool, highSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(secondarySchool, juniorHighSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(secondarySchool, middleSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(secondarySchool, elementary), Is.EqualTo(1));
            Assert.That(comparer.Compare(secondarySchool, elementarySecondarySchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(secondarySchool, secondarySchool), Is.EqualTo(0));
            Assert.That(comparer.Compare(secondarySchool, ungraded), Is.EqualTo(-1));
            Assert.That(comparer.Compare(secondarySchool, adultSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(secondarySchool, infantToddlerSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(secondarySchool, preschoolEarlyChildhood), Is.EqualTo(-1));
            Assert.That(comparer.Compare(secondarySchool, primarySchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(secondarySchool, intermediateSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(secondarySchool, unknown), Is.EqualTo(-1));
        }

        [Test]
        public void Should_sort_Adult_School_eighth()
        {
            Assert.That(comparer.Compare(adultSchool, highSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(adultSchool, juniorHighSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(adultSchool, middleSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(adultSchool, elementary), Is.EqualTo(1));
            Assert.That(comparer.Compare(adultSchool, elementarySecondarySchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(adultSchool, secondarySchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(adultSchool, ungraded), Is.EqualTo(1));
            Assert.That(comparer.Compare(adultSchool, adultSchool), Is.EqualTo(0));
            Assert.That(comparer.Compare(adultSchool, infantToddlerSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(adultSchool, preschoolEarlyChildhood), Is.EqualTo(-1));
            Assert.That(comparer.Compare(adultSchool, primarySchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(adultSchool, intermediateSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(adultSchool, unknown), Is.EqualTo(-1));
        }

        [Test]
        public void Should_sort_Infant_toddler_ninth()
        {
            Assert.That(comparer.Compare(infantToddlerSchool, highSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(infantToddlerSchool, juniorHighSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(infantToddlerSchool, middleSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(infantToddlerSchool, elementary), Is.EqualTo(1));
            Assert.That(comparer.Compare(infantToddlerSchool, elementarySecondarySchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(infantToddlerSchool, secondarySchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(infantToddlerSchool, ungraded), Is.EqualTo(1));
            Assert.That(comparer.Compare(infantToddlerSchool, adultSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(infantToddlerSchool, infantToddlerSchool), Is.EqualTo(0));
            Assert.That(comparer.Compare(infantToddlerSchool, preschoolEarlyChildhood), Is.EqualTo(-1));
            Assert.That(comparer.Compare(infantToddlerSchool, primarySchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(infantToddlerSchool, intermediateSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(infantToddlerSchool, unknown), Is.EqualTo(-1));
        }

        [Test]
        public void Should_sort_Preschool_early_childhood_tenth()
        {
            Assert.That(comparer.Compare(preschoolEarlyChildhood, highSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(preschoolEarlyChildhood, juniorHighSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(preschoolEarlyChildhood, middleSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(preschoolEarlyChildhood, elementary), Is.EqualTo(1));
            Assert.That(comparer.Compare(preschoolEarlyChildhood, elementarySecondarySchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(preschoolEarlyChildhood, secondarySchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(preschoolEarlyChildhood, ungraded), Is.EqualTo(1));
            Assert.That(comparer.Compare(preschoolEarlyChildhood, adultSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(preschoolEarlyChildhood, infantToddlerSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(preschoolEarlyChildhood, preschoolEarlyChildhood), Is.EqualTo(0));
            Assert.That(comparer.Compare(preschoolEarlyChildhood, primarySchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(preschoolEarlyChildhood, intermediateSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(preschoolEarlyChildhood, unknown), Is.EqualTo(-1));
        }

        [Test]
        public void Should_sort_Primary_school_eleventh()
        {
            Assert.That(comparer.Compare(primarySchool, highSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(primarySchool, juniorHighSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(primarySchool, middleSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(primarySchool, elementary), Is.EqualTo(1));
            Assert.That(comparer.Compare(primarySchool, elementarySecondarySchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(primarySchool, secondarySchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(primarySchool, ungraded), Is.EqualTo(1));
            Assert.That(comparer.Compare(primarySchool, adultSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(primarySchool, infantToddlerSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(primarySchool, preschoolEarlyChildhood), Is.EqualTo(1));
            Assert.That(comparer.Compare(primarySchool, primarySchool), Is.EqualTo(0));
            Assert.That(comparer.Compare(primarySchool, intermediateSchool), Is.EqualTo(-1));
            Assert.That(comparer.Compare(primarySchool, unknown), Is.EqualTo(-1));
        }

        [Test]
        public void Should_sort_Intermediate_School_twelfth()
        {
            Assert.That(comparer.Compare(intermediateSchool, highSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(intermediateSchool, juniorHighSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(intermediateSchool, middleSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(intermediateSchool, elementary), Is.EqualTo(1));
            Assert.That(comparer.Compare(intermediateSchool, elementarySecondarySchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(intermediateSchool, secondarySchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(intermediateSchool, ungraded), Is.EqualTo(1));
            Assert.That(comparer.Compare(intermediateSchool, adultSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(intermediateSchool, infantToddlerSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(intermediateSchool, preschoolEarlyChildhood), Is.EqualTo(1));
            Assert.That(comparer.Compare(intermediateSchool, primarySchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(intermediateSchool, intermediateSchool), Is.EqualTo(0));
            Assert.That(comparer.Compare(intermediateSchool, unknown), Is.EqualTo(-1));
        }

        [Test]
        public void Should_sort_unknown_last()
        {
            Assert.That(comparer.Compare(unknown, highSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(unknown, juniorHighSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(unknown, middleSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(unknown, elementary), Is.EqualTo(1));
            Assert.That(comparer.Compare(unknown, elementarySecondarySchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(unknown, secondarySchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(unknown, ungraded), Is.EqualTo(1));
            Assert.That(comparer.Compare(unknown, adultSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(unknown, infantToddlerSchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(unknown, preschoolEarlyChildhood), Is.EqualTo(1));
            Assert.That(comparer.Compare(unknown, primarySchool), Is.EqualTo(1));
            Assert.That(comparer.Compare(unknown, intermediateSchool), Is.EqualTo(1)); 
            Assert.That(comparer.Compare(unknown, unknown), Is.EqualTo(0));
        }
    }
}
