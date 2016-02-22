using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Application.Resources.Models.Staff;
using EdFi.Dashboards.Application.Resources.Staff;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Testing;
using EdFi.Dashboards.Testing.NBuilder;
using EdFi.Dashboards.Resources.Models.Common;
using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;

namespace EdFi.Dashboards.Application.Resources.Tests.Staff
{
    public abstract class CustomStudentListServiceFixtureBase : TestFixtureBase
    {
        protected Mock<IPersistingRepository<StaffCustomStudentList>> staffCustomStudentListRepository;
        protected Mock<IPersistingRepository<StaffCustomStudentListStudent>> staffCustomStudentListStudentRepository;
        protected Mock<ICacheProvider> cacheProvider;
        protected ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks;
        protected IStaffAreaLinks staffAreaLinks;

        protected IQueryable<StaffCustomStudentList> suppliedCustomStudentListData;
        protected IQueryable<StaffCustomStudentListStudent> suppliedCustomStudentListStudentData;
        protected int suppliedLocalEducationAgencyId = 100;
        protected int? suppliedSchoolId = 200;
        protected int suppliedStaffUSI = 300;

        protected override void EstablishContext()
        {
            staffCustomStudentListRepository = new Mock<IPersistingRepository<StaffCustomStudentList>>();
            staffCustomStudentListStudentRepository = new Mock<IPersistingRepository<StaffCustomStudentListStudent>>();
            cacheProvider = new Mock<ICacheProvider>();
            localEducationAgencyAreaLinks = new LocalEducationAgencyAreaLinksFake();
            staffAreaLinks = new StaffAreaLinksFake();

            suppliedCustomStudentListData = GetSuppliedCustomStudentListData();
            suppliedCustomStudentListStudentData = GetSuppliedCustomStudentListStudentData();

            staffCustomStudentListRepository.Setup(x => x.GetAll()).Returns(suppliedCustomStudentListData);
            staffCustomStudentListStudentRepository.Setup(x => x.GetAll()).Returns(suppliedCustomStudentListStudentData);
        }

        protected virtual IQueryable<StaffCustomStudentList> GetSuppliedCustomStudentListData()
        {
            var list = new List<StaffCustomStudentList>();
            return list.AsQueryable();
        }

        protected virtual IQueryable<StaffCustomStudentListStudent> GetSuppliedCustomStudentListStudentData()
        {
            var list = new List<StaffCustomStudentListStudent>();
            return list.AsQueryable();
        }
    }

    public abstract class CustomStudentListServiceGetFixtureBase : CustomStudentListServiceFixtureBase
    {
        protected IEnumerable<CustomStudentListModel> actualModel;

        protected override void ExecuteTest()
        {
            var service = new CustomStudentListService(staffCustomStudentListRepository.Object, staffCustomStudentListStudentRepository.Object, cacheProvider.Object, localEducationAgencyAreaLinks, staffAreaLinks);
            actualModel = service.Get(CustomStudentListGetRequest.Create(suppliedLocalEducationAgencyId, suppliedSchoolId, suppliedStaffUSI));
        }
    }

    public class When_retrieving_custom_student_lists_for_staff_at_local_education_agency : CustomStudentListServiceGetFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedSchoolId = null;
            base.EstablishContext();
        }

        protected override IQueryable<StaffCustomStudentList> GetSuppliedCustomStudentListData()
        {
            var list = new List<StaffCustomStudentList>
                           {
                               new StaffCustomStudentList{ EducationOrganizationId = 999, StaffUSI = suppliedStaffUSI, CustomStudentListIdentifier = "wrong data 1", StaffCustomStudentListId = 1},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedLocalEducationAgencyId, StaffUSI = suppliedStaffUSI, CustomStudentListIdentifier = "custom student list 2", StaffCustomStudentListId = 2},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedLocalEducationAgencyId, StaffUSI = suppliedStaffUSI, CustomStudentListIdentifier = "custom student list 3", StaffCustomStudentListId = 3},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedLocalEducationAgencyId, StaffUSI = suppliedStaffUSI, CustomStudentListIdentifier = "custom student list 4", StaffCustomStudentListId = 4},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedLocalEducationAgencyId, StaffUSI = 999, CustomStudentListIdentifier = "wrong data 5", StaffCustomStudentListId = 5},
                           };
            return list.AsQueryable();
        }

        [Test]
        public void Should_return_correct_custom_student_lists()
        {
            Assert.That(actualModel.Count(), Is.EqualTo(3));
            var customStudentListModel = actualModel.ElementAt(0);
            Assert.That(customStudentListModel.CustomStudentListId, Is.EqualTo(2));
            Assert.That(customStudentListModel.Description, Is.EqualTo("custom student list 2"));
            Assert.That(customStudentListModel.EducationOrganizationId, Is.EqualTo(suppliedLocalEducationAgencyId));
            Assert.That(customStudentListModel.Href, Is.EqualTo(localEducationAgencyAreaLinks.StudentList(suppliedLocalEducationAgencyId, suppliedStaffUSI, null, 2, StudentListType.CustomStudentList.ToString())));
            Assert.That(customStudentListModel.StaffUSI, Is.EqualTo(suppliedStaffUSI));
        }
    }

    public class When_retrieving_custom_student_lists_for_staff_at_school : CustomStudentListServiceGetFixtureBase
    {
        protected override IQueryable<StaffCustomStudentList> GetSuppliedCustomStudentListData()
        {
            var list = new List<StaffCustomStudentList>
                           {
                               new StaffCustomStudentList{ EducationOrganizationId = 999, StaffUSI = suppliedStaffUSI, CustomStudentListIdentifier = "wrong data 1", StaffCustomStudentListId = 1},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedLocalEducationAgencyId, StaffUSI = suppliedStaffUSI, CustomStudentListIdentifier = "custom student list 2", StaffCustomStudentListId = 2},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedLocalEducationAgencyId, StaffUSI = suppliedStaffUSI, CustomStudentListIdentifier = "custom student list 3", StaffCustomStudentListId = 3},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedLocalEducationAgencyId, StaffUSI = suppliedStaffUSI, CustomStudentListIdentifier = "custom student list 4", StaffCustomStudentListId = 4},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedLocalEducationAgencyId, StaffUSI = 999, CustomStudentListIdentifier = "wrong data 5", StaffCustomStudentListId = 5},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedSchoolId.Value, StaffUSI = suppliedStaffUSI, CustomStudentListIdentifier = "custom student list 6", StaffCustomStudentListId = 6},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedSchoolId.Value, StaffUSI = 999, CustomStudentListIdentifier = "custom student list 7", StaffCustomStudentListId = 7},
                           };
            return list.AsQueryable();
        }

        [Test]
        public void Should_return_correct_custom_student_lists()
        {
            Assert.That(actualModel.Count(), Is.EqualTo(4));
            var customStudentListModel = actualModel.ElementAt(0);
            Assert.That(customStudentListModel.CustomStudentListId, Is.EqualTo(2));
            Assert.That(customStudentListModel.Description, Is.EqualTo("custom student list 2"));
            Assert.That(customStudentListModel.EducationOrganizationId, Is.EqualTo(suppliedLocalEducationAgencyId));
            Assert.That(customStudentListModel.Href, Is.EqualTo(localEducationAgencyAreaLinks.StudentList(suppliedLocalEducationAgencyId, suppliedStaffUSI, null, 2, StudentListType.CustomStudentList.ToString())));
            Assert.That(customStudentListModel.StaffUSI, Is.EqualTo(suppliedStaffUSI));

            customStudentListModel = actualModel.ElementAt(3);
            Assert.That(customStudentListModel.CustomStudentListId, Is.EqualTo(6));
            Assert.That(customStudentListModel.Description, Is.EqualTo("custom student list 6"));
            Assert.That(customStudentListModel.EducationOrganizationId, Is.EqualTo(suppliedSchoolId.Value));
            Assert.That(customStudentListModel.Href, Is.EqualTo(staffAreaLinks.Default(suppliedSchoolId.Value, suppliedStaffUSI, null, 6, StudentListType.CustomStudentList.ToString())));
            Assert.That(customStudentListModel.StaffUSI, Is.EqualTo(suppliedStaffUSI));
        }
    }

    public class When_retrieving_custom_student_lists_for_staff_at_school_with_no_local_education_agency : CustomStudentListServiceGetFixtureBase
    {
        protected override IQueryable<StaffCustomStudentList> GetSuppliedCustomStudentListData()
        {
            var list = new List<StaffCustomStudentList>
                           {
                               new StaffCustomStudentList{ EducationOrganizationId = 999, StaffUSI = suppliedStaffUSI, CustomStudentListIdentifier = "wrong data 1", StaffCustomStudentListId = 1},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedSchoolId.Value, StaffUSI = suppliedStaffUSI, CustomStudentListIdentifier = "custom student list 2", StaffCustomStudentListId = 2},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedSchoolId.Value, StaffUSI = suppliedStaffUSI, CustomStudentListIdentifier = "custom student list 3", StaffCustomStudentListId = 3},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedSchoolId.Value, StaffUSI = suppliedStaffUSI, CustomStudentListIdentifier = "custom student list 4", StaffCustomStudentListId = 4},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedSchoolId.Value, StaffUSI = 999, CustomStudentListIdentifier = "wrong data 5", StaffCustomStudentListId = 5},
                           };
            return list.AsQueryable();
        }

        [Test]
        public void Should_return_correct_custom_student_lists()
        {
            Assert.That(actualModel.Count(), Is.EqualTo(3));
            var customStudentListModel = actualModel.ElementAt(0);
            Assert.That(customStudentListModel.CustomStudentListId, Is.EqualTo(2));
            Assert.That(customStudentListModel.Description, Is.EqualTo("custom student list 2"));
            Assert.That(customStudentListModel.EducationOrganizationId, Is.EqualTo(suppliedSchoolId.Value));
            Assert.That(customStudentListModel.Href, Is.EqualTo(staffAreaLinks.Default(suppliedSchoolId.Value, suppliedStaffUSI, null, 2, StudentListType.CustomStudentList.ToString())));
            Assert.That(customStudentListModel.StaffUSI, Is.EqualTo(suppliedStaffUSI));
        }
    }

    public abstract class CustomStudentListServicePostFixtureBase : CustomStudentListServiceFixtureBase
    {
        protected int suppliedStaffCustomStudentListId = 500;
        protected int suppliedStaffCustomStudentListStudentId = 600;
        protected int? suppliedCustomStudentListId = 400;
        protected int suppliedStudentUSI1 = 700;
        protected int suppliedStudentUSI2 = 701;
        protected int suppliedStudentUSI3 = 702;
        protected int suppliedStudentUSI4 = 703;
        protected PostAction suppliedPostAction;
        protected string suppliedDescription;
        protected IEnumerable<int> suppliedStudentUSIs;

        protected List<StaffCustomStudentListStudent> savedStaffCustomStudentListStudent;
        protected List<StaffCustomStudentList> savedStaffCustomStudentList;
        protected List<Expression<Func<StaffCustomStudentListStudent, bool>>> deletedStaffCustomStudentListStudent;
        protected List<Expression<Func<StaffCustomStudentList, bool>>> deletedStaffCustomStudentList;

        protected string expectedModel;

        protected string actualModel;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            savedStaffCustomStudentList = new List<StaffCustomStudentList>();
            savedStaffCustomStudentListStudent = new List<StaffCustomStudentListStudent>();
            deletedStaffCustomStudentList = new List<Expression<Func<StaffCustomStudentList, bool>>>();
            deletedStaffCustomStudentListStudent = new List<Expression<Func<StaffCustomStudentListStudent, bool>>>();


            staffCustomStudentListRepository.Setup(x => x.Save(It.IsAny<StaffCustomStudentList>()))
                .Callback<StaffCustomStudentList>(x =>
                {
                    // Set the identity on the object being inserted
                    x.StaffCustomStudentListId = suppliedStaffCustomStudentListId;
                    savedStaffCustomStudentList.Add(x);
                });

            staffCustomStudentListStudentRepository.Setup(x => x.Save(It.IsAny<StaffCustomStudentListStudent>()))
                .Callback<StaffCustomStudentListStudent>(x =>
                {
                    // Set the identity on the object being inserted
                    x.StaffCustomStudentListStudentId = suppliedStaffCustomStudentListStudentId;
                    savedStaffCustomStudentListStudent.Add(x);
                });

            staffCustomStudentListRepository.Setup(x => x.Delete(It.IsAny<Expression<Func<StaffCustomStudentList, bool>>>()))
                .Callback<Expression<Func<StaffCustomStudentList, bool>>>(x => deletedStaffCustomStudentList.Add(x));

            staffCustomStudentListStudentRepository.Setup(x => x.Delete(It.IsAny<Expression<Func<StaffCustomStudentListStudent, bool>>>()))
                .Callback<Expression<Func<StaffCustomStudentListStudent, bool>>>(x => deletedStaffCustomStudentListStudent.Add(x));
        }

        protected override void ExecuteTest()
        {
            var service = new CustomStudentListService(staffCustomStudentListRepository.Object, staffCustomStudentListStudentRepository.Object, cacheProvider.Object, localEducationAgencyAreaLinks, staffAreaLinks);
            actualModel = service.Post(CustomStudentListPostRequest.Create(suppliedLocalEducationAgencyId, suppliedSchoolId, suppliedStaffUSI, suppliedCustomStudentListId, suppliedPostAction, suppliedDescription, suppliedStudentUSIs));
        }
    }

    public class When_creating_a_new_custom_student_list_for_staff_at_local_education_agency : CustomStudentListServicePostFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedPostAction = PostAction.Add;
            suppliedDescription = "new custom list";
            suppliedCustomStudentListId = null;
            suppliedStudentUSIs = new List<int> { suppliedStudentUSI1, suppliedStudentUSI2, suppliedStudentUSI3 };
            suppliedSchoolId = null;

            base.EstablishContext();
        }

        protected override IQueryable<StaffCustomStudentList> GetSuppliedCustomStudentListData()
        {
            var list = new List<StaffCustomStudentList>
                           {
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedLocalEducationAgencyId, StaffUSI = suppliedStaffUSI, StaffCustomStudentListId = suppliedStaffCustomStudentListId}
                           };
            return list.AsQueryable();
        }

        [Test]
        public void Should_save_new_custom_list()
        {
            Assert.That(savedStaffCustomStudentList.Count, Is.EqualTo(1));
            var newList = savedStaffCustomStudentList[0];
            Assert.That(newList.StaffCustomStudentListId, Is.EqualTo(suppliedStaffCustomStudentListId));
            Assert.That(newList.StaffUSI, Is.EqualTo(suppliedStaffUSI));
            Assert.That(newList.EducationOrganizationId, Is.EqualTo(suppliedLocalEducationAgencyId));
            Assert.That(newList.CustomStudentListIdentifier, Is.EqualTo(suppliedDescription));
        }

        [Test]
        public void Should_save_students_to_new_custom_list()
        {
            Assert.That(savedStaffCustomStudentListStudent.Count, Is.EqualTo(3));
            Assert.That(savedStaffCustomStudentListStudent.AsQueryable().Count(x => x.StudentUSI == suppliedStudentUSI1 && x.StaffCustomStudentListId == suppliedStaffCustomStudentListId), Is.EqualTo(1));
            Assert.That(savedStaffCustomStudentListStudent.AsQueryable().Count(x => x.StudentUSI == suppliedStudentUSI2 && x.StaffCustomStudentListId == suppliedStaffCustomStudentListId), Is.EqualTo(1));
            Assert.That(savedStaffCustomStudentListStudent.AsQueryable().Count(x => x.StudentUSI == suppliedStudentUSI3 && x.StaffCustomStudentListId == suppliedStaffCustomStudentListId), Is.EqualTo(1));
        }

        [Test]
        public void Should_not_delete_any_custom_lists()
        {
            Assert.That(deletedStaffCustomStudentList.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_not_delete_any_students_from_custom_lists()
        {
            Assert.That(deletedStaffCustomStudentListStudent.Count, Is.EqualTo(0));
        }
    }

    public class When_creating_a_new_custom_student_list_for_staff_at_school : CustomStudentListServicePostFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedPostAction = PostAction.Add;
            suppliedDescription = "new custom list";
            suppliedCustomStudentListId = null;
            suppliedStudentUSIs = new List<int> { suppliedStudentUSI1, suppliedStudentUSI2, suppliedStudentUSI3 };

            base.EstablishContext();
        }

        protected override IQueryable<StaffCustomStudentList> GetSuppliedCustomStudentListData()
        {
            var list = new List<StaffCustomStudentList>
                           {
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedSchoolId.Value, StaffUSI = suppliedStaffUSI, StaffCustomStudentListId = suppliedStaffCustomStudentListId}
                           };
            return list.AsQueryable();
        }

        [Test]
        public void Should_save_new_custom_list()
        {
            Assert.That(savedStaffCustomStudentList.Count, Is.EqualTo(1));
            var newList = savedStaffCustomStudentList[0];
            Assert.That(newList.StaffCustomStudentListId, Is.EqualTo(suppliedStaffCustomStudentListId));
            Assert.That(newList.StaffUSI, Is.EqualTo(suppliedStaffUSI));
            Assert.That(newList.EducationOrganizationId, Is.EqualTo(suppliedSchoolId));
            Assert.That(newList.CustomStudentListIdentifier, Is.EqualTo(suppliedDescription));
        }

        [Test]
        public void Should_save_students_to_new_custom_list()
        {
            Assert.That(savedStaffCustomStudentListStudent.Count, Is.EqualTo(3));
            Assert.That(savedStaffCustomStudentListStudent.AsQueryable().Count(x => x.StudentUSI == suppliedStudentUSI1 && x.StaffCustomStudentListId == suppliedStaffCustomStudentListId), Is.EqualTo(1));
            Assert.That(savedStaffCustomStudentListStudent.AsQueryable().Count(x => x.StudentUSI == suppliedStudentUSI2 && x.StaffCustomStudentListId == suppliedStaffCustomStudentListId), Is.EqualTo(1));
            Assert.That(savedStaffCustomStudentListStudent.AsQueryable().Count(x => x.StudentUSI == suppliedStudentUSI3 && x.StaffCustomStudentListId == suppliedStaffCustomStudentListId), Is.EqualTo(1));
        }

        [Test]
        public void Should_not_delete_any_custom_lists()
        {
            Assert.That(deletedStaffCustomStudentList.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_not_delete_any_students_from_custom_lists()
        {
            Assert.That(deletedStaffCustomStudentListStudent.Count, Is.EqualTo(0));
        }
    }

    public class When_adding_students_to_a_custom_student_list_for_staff_at_local_education_agency : CustomStudentListServicePostFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedPostAction = PostAction.Add;
            suppliedDescription = "new custom list";
            suppliedStudentUSIs = new List<int> { suppliedStudentUSI1, suppliedStudentUSI2, suppliedStudentUSI3 };
            suppliedSchoolId = null;

            base.EstablishContext();
        }

        protected override IQueryable<StaffCustomStudentList> GetSuppliedCustomStudentListData()
        {
            var list = new List<StaffCustomStudentList>
                           {
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedLocalEducationAgencyId, StaffUSI = suppliedStaffUSI, StaffCustomStudentListId = suppliedCustomStudentListId.Value}
                           };
            return list.AsQueryable();
        }

        protected override IQueryable<StaffCustomStudentListStudent> GetSuppliedCustomStudentListStudentData()
        {
            var list = new List<StaffCustomStudentListStudent>
                           {
                               new StaffCustomStudentListStudent{ StaffCustomStudentListId = suppliedCustomStudentListId.Value, StudentUSI = suppliedStudentUSI3},
                               new StaffCustomStudentListStudent{ StaffCustomStudentListId = 999, StudentUSI = suppliedStudentUSI1},
                           };
            return list.AsQueryable();
        }

        [Test]
        public void Should_not_save_any_custom_lists()
        {
            Assert.That(savedStaffCustomStudentList.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_save_new_students_to_custom_list()
        {
            Assert.That(savedStaffCustomStudentListStudent.Count, Is.EqualTo(2));
            Assert.That(savedStaffCustomStudentListStudent.AsQueryable().Count(x => x.StudentUSI == suppliedStudentUSI1 && x.StaffCustomStudentListId == suppliedCustomStudentListId), Is.EqualTo(1));
            Assert.That(savedStaffCustomStudentListStudent.AsQueryable().Count(x => x.StudentUSI == suppliedStudentUSI2 && x.StaffCustomStudentListId == suppliedCustomStudentListId), Is.EqualTo(1));
        }

        [Test]
        public void Should_not_delete_any_custom_lists()
        {
            Assert.That(deletedStaffCustomStudentList.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_not_delete_any_students_from_custom_lists()
        {
            Assert.That(deletedStaffCustomStudentListStudent.Count, Is.EqualTo(0));
        }
    }

    public class When_adding_students_to_a_custom_student_list_for_staff_at_school : CustomStudentListServicePostFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedPostAction = PostAction.Add;
            suppliedDescription = "new custom list";
            suppliedStudentUSIs = new List<int> { suppliedStudentUSI1, suppliedStudentUSI2, suppliedStudentUSI3 };

            base.EstablishContext();
        }

        protected override IQueryable<StaffCustomStudentList> GetSuppliedCustomStudentListData()
        {
            var list = new List<StaffCustomStudentList>
                           {
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedSchoolId.Value, StaffUSI = suppliedStaffUSI, StaffCustomStudentListId = suppliedCustomStudentListId.Value}
                           };
            return list.AsQueryable();
        }

        protected override IQueryable<StaffCustomStudentListStudent> GetSuppliedCustomStudentListStudentData()
        {
            var list = new List<StaffCustomStudentListStudent>
                           {
                               new StaffCustomStudentListStudent{ StaffCustomStudentListId = suppliedCustomStudentListId.Value, StudentUSI = suppliedStudentUSI3},
                               new StaffCustomStudentListStudent{ StaffCustomStudentListId = 999, StudentUSI = suppliedStudentUSI1},
                           };
            return list.AsQueryable();
        }

        [Test]
        public void Should_not_save_any_custom_lists()
        {
            Assert.That(savedStaffCustomStudentList.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_save_new_students_to_custom_list()
        {
            Assert.That(savedStaffCustomStudentListStudent.Count, Is.EqualTo(2));
            Assert.That(savedStaffCustomStudentListStudent.AsQueryable().Count(x => x.StudentUSI == suppliedStudentUSI1 && x.StaffCustomStudentListId == suppliedCustomStudentListId), Is.EqualTo(1));
            Assert.That(savedStaffCustomStudentListStudent.AsQueryable().Count(x => x.StudentUSI == suppliedStudentUSI2 && x.StaffCustomStudentListId == suppliedCustomStudentListId), Is.EqualTo(1));
        }

        [Test]
        public void Should_not_delete_any_custom_lists()
        {
            Assert.That(deletedStaffCustomStudentList.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_not_delete_any_students_from_custom_lists()
        {
            Assert.That(deletedStaffCustomStudentListStudent.Count, Is.EqualTo(0));
        }
    }

    public class When_deleting_a_custom_student_list : CustomStudentListServicePostFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedPostAction = PostAction.Delete;

            base.EstablishContext();
        }

        protected override IQueryable<StaffCustomStudentList> GetSuppliedCustomStudentListData()
        {
            var list = new List<StaffCustomStudentList>
                           {
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedSchoolId.Value, StaffUSI = suppliedStaffUSI, StaffCustomStudentListId = suppliedCustomStudentListId.Value}
                           };
            return list.AsQueryable();
        }

        protected override IQueryable<StaffCustomStudentListStudent> GetSuppliedCustomStudentListStudentData()
        {
            var list = new List<StaffCustomStudentListStudent>
                           {
                               new StaffCustomStudentListStudent{ StaffCustomStudentListId = suppliedCustomStudentListId.Value, StudentUSI = suppliedStudentUSI1},
                               new StaffCustomStudentListStudent{ StaffCustomStudentListId = suppliedCustomStudentListId.Value, StudentUSI = suppliedStudentUSI2},
                               new StaffCustomStudentListStudent{ StaffCustomStudentListId = suppliedCustomStudentListId.Value, StudentUSI = suppliedStudentUSI3},
                               new StaffCustomStudentListStudent{ StaffCustomStudentListId = 999, StudentUSI = suppliedStudentUSI1},
                           };
            return list.AsQueryable();
        }

        [Test]
        public void Should_not_save_any_custom_lists()
        {
            Assert.That(savedStaffCustomStudentList.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_not_save_any_students_to_custom_list()
        {
            Assert.That(savedStaffCustomStudentListStudent.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_delete_specified_custom_list()
        {
            Assert.That(deletedStaffCustomStudentList.Count, Is.EqualTo(1));
            Assert.That(deletedStaffCustomStudentList[0].Compile().Invoke(new StaffCustomStudentList { StaffCustomStudentListId = suppliedCustomStudentListId.Value}), Is.True);
        }

        [Test]
        public void Should_delete_students_from_specified_custom_list()
        {
            Assert.That(deletedStaffCustomStudentListStudent.Count, Is.EqualTo(1));
            Assert.That(deletedStaffCustomStudentListStudent[0].Compile().Invoke(new StaffCustomStudentListStudent { StaffCustomStudentListId = suppliedCustomStudentListId.Value }), Is.True);
        }
    }

    public class When_deleting_a_custom_student_list_that_does_not_exist : CustomStudentListServicePostFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedPostAction = PostAction.Delete;
            suppliedStudentUSIs = new List<int> { suppliedStudentUSI1, suppliedStudentUSI2, suppliedStudentUSI3 };

            base.EstablishContext();
        }

        protected override IQueryable<StaffCustomStudentList> GetSuppliedCustomStudentListData()
        {
            var list = new List<StaffCustomStudentList>
                           {
                               new StaffCustomStudentList{ EducationOrganizationId = 999, StaffUSI = suppliedStaffUSI, StaffCustomStudentListId = suppliedCustomStudentListId.Value}
                           };
            return list.AsQueryable();
        }

        protected override IQueryable<StaffCustomStudentListStudent> GetSuppliedCustomStudentListStudentData()
        {
            var list = new List<StaffCustomStudentListStudent>
                           {
                               new StaffCustomStudentListStudent{ StaffCustomStudentListId = 999, StudentUSI = suppliedStudentUSI1},
                           };
            return list.AsQueryable();
        }

        [Test]
        public void Should_not_save_any_custom_lists()
        {
            Assert.That(savedStaffCustomStudentList.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_not_save_any_students_to_custom_list()
        {
            Assert.That(savedStaffCustomStudentListStudent.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_not_delete_any_custom_lists()
        {
            Assert.That(deletedStaffCustomStudentList.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_not_delete_any_students_from_custom_lists()
        {
            Assert.That(deletedStaffCustomStudentListStudent.Count, Is.EqualTo(0));
        }
    }

    public class When_removing_students_from_a_custom_student_list : CustomStudentListServicePostFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedPostAction = PostAction.Remove;
            suppliedStudentUSIs = new List<int> { suppliedStudentUSI1, suppliedStudentUSI2, suppliedStudentUSI3 };

            base.EstablishContext();
        }

        protected override IQueryable<StaffCustomStudentList> GetSuppliedCustomStudentListData()
        {
            var list = new List<StaffCustomStudentList>
                           {
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedSchoolId.Value, StaffUSI = suppliedStaffUSI, StaffCustomStudentListId = suppliedCustomStudentListId.Value}
                           };
            return list.AsQueryable();
        }

        protected override IQueryable<StaffCustomStudentListStudent> GetSuppliedCustomStudentListStudentData()
        {
            var list = new List<StaffCustomStudentListStudent>
                           {
                               new StaffCustomStudentListStudent{ StaffCustomStudentListId = suppliedCustomStudentListId.Value, StudentUSI = suppliedStudentUSI1},
                               new StaffCustomStudentListStudent{ StaffCustomStudentListId = suppliedCustomStudentListId.Value, StudentUSI = suppliedStudentUSI2},
                               new StaffCustomStudentListStudent{ StaffCustomStudentListId = suppliedCustomStudentListId.Value, StudentUSI = suppliedStudentUSI3},
                               new StaffCustomStudentListStudent{ StaffCustomStudentListId = suppliedCustomStudentListId.Value, StudentUSI = suppliedStudentUSI4},
                               new StaffCustomStudentListStudent{ StaffCustomStudentListId = 999, StudentUSI = suppliedStudentUSI1},
                           };
            return list.AsQueryable();
        }

        [Test]
        public void Should_not_save_any_custom_lists()
        {
            Assert.That(savedStaffCustomStudentList.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_not_save_any_students_to_custom_list()
        {
            Assert.That(savedStaffCustomStudentListStudent.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_not_delete_any_custom_lists()
        {
            Assert.That(deletedStaffCustomStudentList.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_delete_students_from_specified_custom_list()
        {
            Assert.That(deletedStaffCustomStudentListStudent.Count, Is.EqualTo(3));

            var swls1 = new StaffCustomStudentListStudent { StaffCustomStudentListId = suppliedCustomStudentListId.Value, StudentUSI = suppliedStudentUSI1 };
            var swls2 = new StaffCustomStudentListStudent { StaffCustomStudentListId = suppliedCustomStudentListId.Value, StudentUSI = suppliedStudentUSI2 };
            var swls3 = new StaffCustomStudentListStudent { StaffCustomStudentListId = suppliedCustomStudentListId.Value, StudentUSI = suppliedStudentUSI3 };
            var swls4 = new StaffCustomStudentListStudent { StaffCustomStudentListId = suppliedCustomStudentListId.Value, StudentUSI = suppliedStudentUSI4 };

            var func = deletedStaffCustomStudentListStudent[0].Compile();
            Assert.That(func.Invoke(swls1) || func.Invoke(swls2) || func.Invoke(swls3), Is.True);
            Assert.That(func.Invoke(swls4), Is.False);

            func = deletedStaffCustomStudentListStudent[1].Compile();
            Assert.That(func.Invoke(swls1) || func.Invoke(swls2) || func.Invoke(swls3), Is.True);
            Assert.That(func.Invoke(swls4), Is.False);

            func = deletedStaffCustomStudentListStudent[2].Compile();
            Assert.That(func.Invoke(swls1) || func.Invoke(swls2) || func.Invoke(swls3), Is.True);
            Assert.That(func.Invoke(swls4), Is.False);
        }
    }

    public class When_renaming_a_custom_student_list : CustomStudentListServicePostFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedPostAction = PostAction.Set;
            suppliedDescription = "the new description";
            base.EstablishContext();
        }

        protected override IQueryable<StaffCustomStudentList> GetSuppliedCustomStudentListData()
        {
            var list = new List<StaffCustomStudentList>
                           {
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedSchoolId.Value, StaffUSI = suppliedStaffUSI, StaffCustomStudentListId = suppliedCustomStudentListId.Value, CustomStudentListIdentifier = "old list name"}
                           };
            return list.AsQueryable();
        }

        protected override IQueryable<StaffCustomStudentListStudent> GetSuppliedCustomStudentListStudentData()
        {
            var list = new List<StaffCustomStudentListStudent>
                           {
                               new StaffCustomStudentListStudent{ StaffCustomStudentListId = suppliedCustomStudentListId.Value, StudentUSI = suppliedStudentUSI3},
                               new StaffCustomStudentListStudent{ StaffCustomStudentListId = 999, StudentUSI = suppliedStudentUSI1},
                           };
            return list.AsQueryable();
        }

        [Test]
        public void Should_save_custom_list()
        {
            Assert.That(savedStaffCustomStudentList.Count, Is.EqualTo(1));
            var newList = savedStaffCustomStudentList[0];
            Assert.That(newList.StaffCustomStudentListId, Is.EqualTo(suppliedStaffCustomStudentListId));
            Assert.That(newList.StaffUSI, Is.EqualTo(suppliedStaffUSI));
            Assert.That(newList.EducationOrganizationId, Is.EqualTo(suppliedSchoolId));
            Assert.That(newList.CustomStudentListIdentifier, Is.EqualTo(suppliedDescription));
        }
        
        [Test]
        public void Should_not_save_any_students_to_custom_list()
        {
            Assert.That(savedStaffCustomStudentListStudent.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_not_delete_any_custom_lists()
        {
            Assert.That(deletedStaffCustomStudentList.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_not_delete_any_students_from_custom_lists()
        {
            Assert.That(deletedStaffCustomStudentListStudent.Count, Is.EqualTo(0));
        }
    }
    
    public class When_modifying_existing_custom_student_list_at_local_education_agency : CustomStudentListServicePostFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedPostAction = PostAction.Add;
            suppliedStudentUSIs = new List<int> { suppliedStudentUSI1, suppliedStudentUSI2, suppliedStudentUSI3 };
            suppliedSchoolId = null;

            base.EstablishContext();
        }

        protected override IQueryable<StaffCustomStudentList> GetSuppliedCustomStudentListData()
        {
            var list = new List<StaffCustomStudentList>
                           {
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedLocalEducationAgencyId, StaffUSI = 999, StaffCustomStudentListId = suppliedCustomStudentListId.Value},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedLocalEducationAgencyId, StaffUSI = suppliedStaffUSI, StaffCustomStudentListId = 999},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedLocalEducationAgencyId, StaffUSI = suppliedStaffUSI, StaffCustomStudentListId = suppliedCustomStudentListId.Value},
                           };
            return list.AsQueryable();
        }

        [Test]
        public void Should_return_correct_url()
        {
            Assert.That(actualModel, Is.EqualTo(localEducationAgencyAreaLinks.StudentList(suppliedLocalEducationAgencyId, suppliedStaffUSI, null, suppliedCustomStudentListId, StudentListType.CustomStudentList.ToString())));
        }
    }

    public class When_modifying_existing_custom_student_list_at_school : CustomStudentListServicePostFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedPostAction = PostAction.Add;
            suppliedStudentUSIs = new List<int> { suppliedStudentUSI1, suppliedStudentUSI2, suppliedStudentUSI3 };

            base.EstablishContext();
        }

        protected override IQueryable<StaffCustomStudentList> GetSuppliedCustomStudentListData()
        {
            var list = new List<StaffCustomStudentList>
                           {
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedSchoolId.Value, StaffUSI = 999, StaffCustomStudentListId = suppliedCustomStudentListId.Value},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedSchoolId.Value, StaffUSI = suppliedStaffUSI, StaffCustomStudentListId = 999},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedSchoolId.Value, StaffUSI = suppliedStaffUSI, StaffCustomStudentListId = suppliedCustomStudentListId.Value},
                           };
            return list.AsQueryable();
        }

        [Test]
        public void Should_return_correct_url()
        {
            Assert.That(actualModel, Is.EqualTo(staffAreaLinks.Default(suppliedSchoolId.Value, suppliedStaffUSI, null, suppliedCustomStudentListId, StudentListType.CustomStudentList.ToString())));
        }
    }

    public class When_modifying_no_longer_existing_custom_student_list_at_local_education_agency : CustomStudentListServicePostFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedPostAction = PostAction.Remove;
            suppliedStudentUSIs = new List<int> { suppliedStudentUSI1, suppliedStudentUSI2, suppliedStudentUSI3 };
            suppliedSchoolId = null;

            base.EstablishContext();
        }

        protected override IQueryable<StaffCustomStudentList> GetSuppliedCustomStudentListData()
        {
            var list = new List<StaffCustomStudentList>
                           {
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedLocalEducationAgencyId, StaffUSI = 999, StaffCustomStudentListId = suppliedCustomStudentListId.Value},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedLocalEducationAgencyId, StaffUSI = suppliedStaffUSI, StaffCustomStudentListId = 999},
                           };
            return list.AsQueryable();
        }

        [Test]
        public void Should_return_correct_url()
        {
            Assert.That(actualModel, Is.EqualTo(localEducationAgencyAreaLinks.StudentList(suppliedLocalEducationAgencyId, suppliedStaffUSI)));
        }
    }

    public class When_modifying_no_longer_existing_custom_student_list_at_school : CustomStudentListServicePostFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedPostAction = PostAction.Add;
            suppliedStudentUSIs = new List<int> { suppliedStudentUSI1, suppliedStudentUSI2, suppliedStudentUSI3 };

            base.EstablishContext();
        }

        protected override IQueryable<StaffCustomStudentList> GetSuppliedCustomStudentListData()
        {
            var list = new List<StaffCustomStudentList>
                           {
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedSchoolId.Value, StaffUSI = 999, StaffCustomStudentListId = suppliedCustomStudentListId.Value},
                               new StaffCustomStudentList{ EducationOrganizationId = suppliedSchoolId.Value, StaffUSI = suppliedStaffUSI, StaffCustomStudentListId = 999},
                           };
            return list.AsQueryable();
        }

        [Test]
        public void Should_return_correct_url()
        {
            Assert.That(actualModel, Is.EqualTo(staffAreaLinks.Default(suppliedSchoolId.Value, suppliedStaffUSI)));
        }
    }
}
