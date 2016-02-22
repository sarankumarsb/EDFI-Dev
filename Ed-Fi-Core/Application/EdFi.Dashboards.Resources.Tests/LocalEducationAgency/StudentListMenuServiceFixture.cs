using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.LocalEducationAgency
{
    public class StudentListMenuServiceFixtureBase : TestFixtureBase
    {
        protected IRepository<StaffCohort> staffCohortRepository;
        protected IRepository<StaffCustomStudentList> staffCustomStudentListRepository;
        protected ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks;

        protected int suppliedLocalEducationAgency = 1000;
        protected int suppliedStaffUSI = 2000;
        protected int suppliedSectionOrCohortId = 3000;
        protected string suppliedStudentListType;

        protected IEnumerable<StudentListMenuModel> actualModel;

        protected override void EstablishContext()
        {
            staffCohortRepository = mocks.StrictMock<IRepository<StaffCohort>>();
            staffCustomStudentListRepository = mocks.StrictMock<IRepository<StaffCustomStudentList>>();
            localEducationAgencyAreaLinks = new LocalEducationAgencyAreaLinksFake();

            Expect.Call(staffCohortRepository.GetAll()).Return(GetSuppliedStaffCohortRepository());
            Expect.Call(staffCustomStudentListRepository.GetAll()).Return(GetSuppliedCustomStudentListRepository());
        }

        protected virtual IQueryable<StaffCohort> GetSuppliedStaffCohortRepository()
        {
            var list = new List<StaffCohort>();
            return list.AsQueryable();
        }

        protected virtual IQueryable<StaffCustomStudentList> GetSuppliedCustomStudentListRepository()
        {
            var list = new List<StaffCustomStudentList>();
            return list.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new StudentListMenuService(staffCohortRepository, staffCustomStudentListRepository, localEducationAgencyAreaLinks);
            actualModel = service.Get(StudentListMenuRequest.Create(suppliedLocalEducationAgency, suppliedStaffUSI, suppliedSectionOrCohortId, suppliedStudentListType));
        }
    }

    public class When_getting_student_list_menu_with_no_cohorts_or_student_lists : StudentListMenuServiceFixtureBase
    {
        [Test]
        public void Should_get_basic_menu()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.Count(), Is.EqualTo(1));
            var menuItem = actualModel.ElementAt(0);
            Assert.That(menuItem.Description, Is.EqualTo("All Students"));
            Assert.That(menuItem.ListType, Is.EqualTo(StudentListType.All));
            Assert.That(menuItem.SectionId, Is.EqualTo(0));
            Assert.That(menuItem.Selected, Is.True);
            Assert.That(menuItem.Href, Is.EqualTo(localEducationAgencyAreaLinks.StudentList(suppliedLocalEducationAgency, suppliedStaffUSI, null, null, StudentListType.All.ToString())));
        }
    }

    public class When_getting_student_list_menu_with_cohorts : StudentListMenuServiceFixtureBase
    {
        protected override IQueryable<StaffCohort> GetSuppliedStaffCohortRepository()
        {
            var list = new List<StaffCohort>
                           {
                               new StaffCohort{ EducationOrganizationId = 999, StaffUSI = suppliedStaffUSI, StaffCohortId = 4, CohortDescription = "wrong data"},
                               new StaffCohort{ EducationOrganizationId = suppliedLocalEducationAgency, StaffUSI = 999, StaffCohortId = 5, CohortDescription = "wrong data"},
                               new StaffCohort{ EducationOrganizationId = suppliedLocalEducationAgency, StaffUSI = suppliedStaffUSI, StaffCohortId = 1, CohortDescription = "zebra"},
                               new StaffCohort{ EducationOrganizationId = suppliedLocalEducationAgency, StaffUSI = suppliedStaffUSI, StaffCohortId = 2, CohortDescription = "apple"},
                               new StaffCohort{ EducationOrganizationId = suppliedLocalEducationAgency, StaffUSI = suppliedStaffUSI, StaffCohortId = 3, CohortDescription = "orange"},
                           };
            return list.AsQueryable();
        }

        [Test]
        public void Should_get_cohorts_for_menu()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.Count(), Is.EqualTo(4));

            var menuItem = actualModel.ElementAt(0);
            Assert.That(menuItem.ListType, Is.EqualTo(StudentListType.All));

            menuItem = actualModel.ElementAt(1);
            Assert.That(menuItem.SectionId, Is.EqualTo(2));
            Assert.That(menuItem.Selected, Is.True);
            Assert.That(menuItem.ListType, Is.EqualTo(StudentListType.Cohort));
            Assert.That(menuItem.Href, Is.EqualTo(localEducationAgencyAreaLinks.StudentList(suppliedLocalEducationAgency, suppliedStaffUSI, null, 2, StudentListType.Cohort.ToString())));
            Assert.That(menuItem.Description, Is.EqualTo("apple"));

            menuItem = actualModel.ElementAt(2);
            Assert.That(menuItem.SectionId, Is.EqualTo(3));

            menuItem = actualModel.ElementAt(3);
            Assert.That(menuItem.SectionId, Is.EqualTo(1));
        }
    }

    public class When_getting_student_list_menu_with_custom_student_lists : StudentListMenuServiceFixtureBase
    {
        protected override IQueryable<StaffCustomStudentList>  GetSuppliedCustomStudentListRepository()
        {
            var list = new List<StaffCustomStudentList>
                           {
                               new StaffCustomStudentList{ EducationOrganizationId = 999, StaffUSI = suppliedStaffUSI, StaffCustomStudentListId = 4, CustomStudentListIdentifier = "wrong data"},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedLocalEducationAgency, StaffUSI = 999, StaffCustomStudentListId = 5, CustomStudentListIdentifier = "wrong data"},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedLocalEducationAgency, StaffUSI = suppliedStaffUSI, StaffCustomStudentListId = 1, CustomStudentListIdentifier = "zebra"},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedLocalEducationAgency, StaffUSI = suppliedStaffUSI, StaffCustomStudentListId = 2, CustomStudentListIdentifier = "apple"},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedLocalEducationAgency, StaffUSI = suppliedStaffUSI, StaffCustomStudentListId = 3, CustomStudentListIdentifier = "orange"},
                           };
            return list.AsQueryable();
        }

        [Test]
        public void Should_get_student_lists_for_menu()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.Count(), Is.EqualTo(4));

            var menuItem = actualModel.ElementAt(0);
            Assert.That(menuItem.ListType, Is.EqualTo(StudentListType.All));

            menuItem = actualModel.ElementAt(1);
            Assert.That(menuItem.SectionId, Is.EqualTo(2));
            Assert.That(menuItem.Selected, Is.True);
            Assert.That(menuItem.ListType, Is.EqualTo(StudentListType.CustomStudentList));
            Assert.That(menuItem.Href, Is.EqualTo(localEducationAgencyAreaLinks.StudentList(suppliedLocalEducationAgency, suppliedStaffUSI, null, 2, StudentListType.CustomStudentList.ToString())));
            Assert.That(menuItem.Description, Is.EqualTo("apple"));

            menuItem = actualModel.ElementAt(2);
            Assert.That(menuItem.SectionId, Is.EqualTo(3));

            menuItem = actualModel.ElementAt(3);
            Assert.That(menuItem.SectionId, Is.EqualTo(1));
        }
    }

    public class When_getting_student_list_menu : StudentListMenuServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedSectionOrCohortId = 3;
            suppliedStudentListType = StudentListType.CustomStudentList.ToString();

            base.EstablishContext();
        }

        protected override IQueryable<StaffCohort> GetSuppliedStaffCohortRepository()
        {
            var list = new List<StaffCohort>
                           {
                               new StaffCohort{ EducationOrganizationId = 999, StaffUSI = suppliedStaffUSI, StaffCohortId = 4, CohortDescription = "wrong data"},
                               new StaffCohort{ EducationOrganizationId = suppliedLocalEducationAgency, StaffUSI = 999, StaffCohortId = 5, CohortDescription = "wrong data"},
                               new StaffCohort{ EducationOrganizationId = suppliedLocalEducationAgency, StaffUSI = suppliedStaffUSI, StaffCohortId = 1, CohortDescription = "zebra"},
                               new StaffCohort{ EducationOrganizationId = suppliedLocalEducationAgency, StaffUSI = suppliedStaffUSI, StaffCohortId = 2, CohortDescription = "apple"},
                               new StaffCohort{ EducationOrganizationId = suppliedLocalEducationAgency, StaffUSI = suppliedStaffUSI, StaffCohortId = 3, CohortDescription = "orange"},
                           };
            return list.AsQueryable();
        }

        protected override IQueryable<StaffCustomStudentList> GetSuppliedCustomStudentListRepository()
        {
            var list = new List<StaffCustomStudentList>
                           {
                               new StaffCustomStudentList{ EducationOrganizationId = 999, StaffUSI = suppliedStaffUSI, StaffCustomStudentListId = 4, CustomStudentListIdentifier = "wrong data"},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedLocalEducationAgency, StaffUSI = 999, StaffCustomStudentListId = 5, CustomStudentListIdentifier = "wrong data"},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedLocalEducationAgency, StaffUSI = suppliedStaffUSI, StaffCustomStudentListId = 1, CustomStudentListIdentifier = "zebra"},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedLocalEducationAgency, StaffUSI = suppliedStaffUSI, StaffCustomStudentListId = 2, CustomStudentListIdentifier = "apple"},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedLocalEducationAgency, StaffUSI = suppliedStaffUSI, StaffCustomStudentListId = 3, CustomStudentListIdentifier = "orange"},
                           };
            return list.AsQueryable();
        }

        [Test]
        public void Should_get_student_lists_for_menu()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.Count(), Is.EqualTo(7));

            var menuItem = actualModel.ElementAt(0);
            Assert.That(menuItem.ListType, Is.EqualTo(StudentListType.All));
            Assert.That(menuItem.Selected, Is.False);

            menuItem = actualModel.ElementAt(1);
            Assert.That(menuItem.SectionId, Is.EqualTo(2));
            Assert.That(menuItem.Selected, Is.False);
            Assert.That(menuItem.ListType, Is.EqualTo(StudentListType.Cohort));
            Assert.That(menuItem.Href, Is.EqualTo(localEducationAgencyAreaLinks.StudentList(suppliedLocalEducationAgency, suppliedStaffUSI, null, 2, StudentListType.Cohort.ToString())));
            Assert.That(menuItem.Description, Is.EqualTo("apple"));

            menuItem = actualModel.ElementAt(2);
            Assert.That(menuItem.SectionId, Is.EqualTo(3));
            Assert.That(menuItem.ListType, Is.EqualTo(StudentListType.Cohort));
            Assert.That(menuItem.Selected, Is.False);

            menuItem = actualModel.ElementAt(3);
            Assert.That(menuItem.SectionId, Is.EqualTo(1));
            Assert.That(menuItem.ListType, Is.EqualTo(StudentListType.Cohort));
            Assert.That(menuItem.Selected, Is.False);

            menuItem = actualModel.ElementAt(4);
            Assert.That(menuItem.SectionId, Is.EqualTo(2));
            Assert.That(menuItem.ListType, Is.EqualTo(StudentListType.CustomStudentList));
            Assert.That(menuItem.Selected, Is.False);

            menuItem = actualModel.ElementAt(5);
            Assert.That(menuItem.SectionId, Is.EqualTo(3));
            Assert.That(menuItem.ListType, Is.EqualTo(StudentListType.CustomStudentList));
            Assert.That(menuItem.Selected, Is.True);

            menuItem = actualModel.ElementAt(6);
            Assert.That(menuItem.SectionId, Is.EqualTo(1));
            Assert.That(menuItem.ListType, Is.EqualTo(StudentListType.CustomStudentList));
            Assert.That(menuItem.Selected, Is.False);
        }
    }
}
