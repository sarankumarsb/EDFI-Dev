using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Resources.StudentMetrics;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.LocalEducationAgency
{
    public abstract class StudentListServiceFixtureBase : TestFixtureBase
    {
        protected IRepository<StaffCohort> staffCohortRepository;
        protected IRepository<StaffCustomStudentListStudent> staffCustomStudentListStudentRepository;
        protected IRepository<StaffCustomStudentList> staffCustomStudentListRepository;
        protected IUniqueListIdProvider uniqueListProvider;
        protected IMetadataListIdResolver metadataListIdResolver;
        protected IListMetadataProvider listMetadataProvider;
        protected IClassroomMetricsProvider classroomMetricsProvider;
        protected IStudentSchoolAreaLinks studentSchoolLinks;
        protected IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;

        protected int suppliedLocalEducationAgencyId = 1000;
        protected int suppliedSchoolId1 = 2001;
        protected int suppliedStaffUSI = 5000;
        protected int suppliedSectionOrCohortId;
        protected string suppliedStudentListType;
        protected string suppliedUniqueListId = "suppliedUniqueListId";
        protected int suppliedMetadataListId = 6;
        protected List<MetadataColumnGroup> suppliedMetadataColumnGroupList = new List<MetadataColumnGroup> { new MetadataColumnGroup { Title = "Supplied Metadata" } };
        protected List<StudentWithMetrics.Metric> suppliedAdditionalMetrics = new List<StudentWithMetrics.Metric> { new StudentWithMetrics.Metric(123) { DisplayValue = "Supplied Additional Metric" } };
        protected List<long> expectedStudentUSI;

        protected StudentListModel actualModel;
        private IStudentMetricsProvider studentListWithMetricsProvider;

        protected override void EstablishContext()
        {
            studentSchoolLinks = new StudentSchoolAreaLinksFake();
            staffCohortRepository = mocks.StrictMock<IRepository<StaffCohort>>();
            staffCustomStudentListStudentRepository = mocks.StrictMock<IRepository<StaffCustomStudentListStudent>>();
            staffCustomStudentListRepository = mocks.StrictMock<IRepository<StaffCustomStudentList>>();
            uniqueListProvider = mocks.StrictMock<IUniqueListIdProvider>();
            metadataListIdResolver = mocks.StrictMock<IMetadataListIdResolver>();
            listMetadataProvider = mocks.StrictMock<IListMetadataProvider>();
            classroomMetricsProvider = mocks.StrictMock<IClassroomMetricsProvider>();
            gradeLevelUtilitiesProvider = mocks.StrictMock<IGradeLevelUtilitiesProvider>();
            studentListWithMetricsProvider = mocks.StrictMock<IStudentMetricsProvider>();

            Expect.Call(uniqueListProvider.GetUniqueId()).Return("StudentList" + suppliedStaffUSI);
            Expect.Call(staffCohortRepository.GetAll()).Repeat.Any().Return(GetStaffCohorts());
            Expect.Call(staffCustomStudentListRepository.GetAll()).Repeat.Any().Return(GetStaffCustomStudentLists());
            Expect.Call(staffCustomStudentListStudentRepository.GetAll()).Repeat.Any().Return(GetStaffCustomStudentListStudents());

            Expect.Call(metadataListIdResolver.GetListId(ListType.StudentDemographic, SchoolCategory.HighSchool)).Return(suppliedMetadataListId);
            Expect.Call(listMetadataProvider.GetListMetadata(suppliedMetadataListId)).Return(suppliedMetadataColumnGroupList);

            Expect.Call(studentListWithMetricsProvider.GetOrderedStudentList(null))
                  .Repeat.Any()
                  .IgnoreArguments()
                  .Return(GetStudentList());

            Expect.Call(studentListWithMetricsProvider.GetStudentsWithMetrics(null))
                  .Repeat.Any()
                  .IgnoreArguments()
                  .Return(new List<StudentMetric>().AsQueryable());

            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForSorting("1")).IgnoreArguments().Repeat.Any().Return(1);
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).IgnoreArguments().Repeat.Any().Return("1");

            Expect.Call(classroomMetricsProvider.GetAdditionalMetrics(null, null)).IgnoreArguments().Repeat.Any().Return(suppliedAdditionalMetrics);

            base.EstablishContext();
        }

        protected IQueryable<StaffCohort> GetStaffCohorts()
        {
            var list = new List<StaffCohort>
                           {
                               new StaffCohort{ EducationOrganizationId = 999, StaffUSI = suppliedStaffUSI, StaffCohortId = 2 },
                               new StaffCohort{ EducationOrganizationId = suppliedLocalEducationAgencyId, StaffUSI = 999, StaffCohortId = 3 },
                               new StaffCohort{ EducationOrganizationId = suppliedLocalEducationAgencyId, StaffUSI = suppliedStaffUSI, StaffCohortId = 2 },
                               new StaffCohort{ EducationOrganizationId = suppliedLocalEducationAgencyId, StaffUSI = suppliedStaffUSI, StaffCohortId = 1 },
                           };
            return list.AsQueryable();
        }

        protected IQueryable<StaffStudentCohort> GetStaffStudentCohorts()
        {
            var list = new List<StaffStudentCohort>
                           {
                           };
            return list.AsQueryable();
        }

        protected IQueryable<StaffCustomStudentListStudent> GetStaffCustomStudentListStudents()
        {
            var list = new List<StaffCustomStudentListStudent>
                           {
                               new StaffCustomStudentListStudent { StaffCustomStudentListId = 1, StudentUSI = 1, StaffCustomStudentListStudentId = 1},
                               new StaffCustomStudentListStudent { StaffCustomStudentListId = 1, StudentUSI = 6, StaffCustomStudentListStudentId = 1},
                               new StaffCustomStudentListStudent { StaffCustomStudentListId = 1, StudentUSI = 7, StaffCustomStudentListStudentId = 1},
                               new StaffCustomStudentListStudent { StaffCustomStudentListId = 1, StudentUSI = 8, StaffCustomStudentListStudentId = 1},
                               new StaffCustomStudentListStudent { StaffCustomStudentListId = 999, StudentUSI = 9, StaffCustomStudentListStudentId = 1},
                               new StaffCustomStudentListStudent { StaffCustomStudentListId = 2, StudentUSI = 6 + 9990, StaffCustomStudentListStudentId = 1},
                               new StaffCustomStudentListStudent { StaffCustomStudentListId = 2, StudentUSI = 7 + 9990, StaffCustomStudentListStudentId = 1},
                               new StaffCustomStudentListStudent { StaffCustomStudentListId = 2, StudentUSI = 8 + 9990, StaffCustomStudentListStudentId = 1},
                           };
            return list.AsQueryable();
        }

        protected IQueryable<StaffCustomStudentList> GetStaffCustomStudentLists()
        {
            var list = new List<StaffCustomStudentList>
                           {
                               new StaffCustomStudentList { EducationOrganizationId = 999, StaffUSI = suppliedStaffUSI, StaffCustomStudentListId = 2 },
                               new StaffCustomStudentList { EducationOrganizationId = suppliedLocalEducationAgencyId, StaffUSI = 999, StaffCustomStudentListId = 3 },
                               new StaffCustomStudentList { EducationOrganizationId = suppliedLocalEducationAgencyId, StaffUSI = suppliedStaffUSI, StaffCustomStudentListId = 2 },
                               new StaffCustomStudentList { EducationOrganizationId = suppliedLocalEducationAgencyId, StaffUSI = suppliedStaffUSI, StaffCustomStudentListId = 1 },
                           };
            return list.AsQueryable();
        }

        protected IQueryable<EnhancedStudentInformation> GetStudentList()
        {
            var list = new List<EnhancedStudentInformation>
                           {
                               new EnhancedStudentInformation{ StudentUSI = 1, SchoolId = suppliedSchoolId1, FirstName = "first1", LastSurname = "Last1", MiddleName = "Middle1", GradeLevel = "1"},
                               new EnhancedStudentInformation{ StudentUSI = 2, SchoolId = suppliedSchoolId1, FirstName = "first2", LastSurname = "Last2", MiddleName = "Middle2", GradeLevel = "1"},
                               new EnhancedStudentInformation{ StudentUSI = 3, SchoolId = suppliedSchoolId1, FirstName = "first3", LastSurname = "Last3", MiddleName = "Middle3", GradeLevel = "1"},
                               new EnhancedStudentInformation{ StudentUSI = 4, SchoolId = 0, FirstName = "first4", LastSurname = "Last4", MiddleName = "Middle4", GradeLevel = "1"},
                               new EnhancedStudentInformation{ StudentUSI = 5, SchoolId = suppliedSchoolId1, FirstName = "first5", LastSurname = "Last5", MiddleName = "Middle5", GradeLevel = "1"},
                               new EnhancedStudentInformation{ StudentUSI = 6, SchoolId = suppliedSchoolId1, FirstName = "first6", LastSurname = "Last6", MiddleName = "Middle6", GradeLevel = "1"},
                               new EnhancedStudentInformation{ StudentUSI = 7, SchoolId = suppliedSchoolId1, FirstName = "first7", LastSurname = "Last7", MiddleName = "Middle7", GradeLevel = "1"},
                               new EnhancedStudentInformation{ StudentUSI = 8, SchoolId = 0, FirstName = "first8", LastSurname = "Last8", MiddleName = "Middle8", GradeLevel = "1"},
                               new EnhancedStudentInformation{ StudentUSI = 9, SchoolId = suppliedSchoolId1, FirstName = "first9", LastSurname = "Last9", MiddleName = "Middle9", GradeLevel = "1"},
                               new EnhancedStudentInformation{ StudentUSI = 1 + 9990, SchoolId = 999},
                               new EnhancedStudentInformation{ StudentUSI = 2 + 9990, SchoolId = 999},
                               new EnhancedStudentInformation{ StudentUSI = 3 + 9990, SchoolId = 999},
                               new EnhancedStudentInformation{ StudentUSI = 4 + 9990, SchoolId = 999},
                               new EnhancedStudentInformation{ StudentUSI = 5 + 9990, SchoolId = 999},
                               new EnhancedStudentInformation{ StudentUSI = 6 + 9990, SchoolId = 999},
                               new EnhancedStudentInformation{ StudentUSI = 7 + 9990, SchoolId = 999},
                           };
            return list.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new StudentListService(staffCohortRepository, staffCustomStudentListRepository, staffCustomStudentListStudentRepository, uniqueListProvider, metadataListIdResolver, listMetadataProvider, classroomMetricsProvider, studentSchoolLinks, studentListWithMetricsProvider, gradeLevelUtilitiesProvider);
            actualModel = service.Get(StudentListRequest.Create(suppliedLocalEducationAgencyId, suppliedStaffUSI, suppliedSectionOrCohortId, suppliedStudentListType, 1, 100, null, null));
        }

        [Test]
        public virtual void Should_correctly_set_model()
        {
            //Assert.That(actualModel.UniqueListId, Is.EqualTo(suppliedUniqueListId));
            Assert.That(actualModel.UniqueListId, Is.EqualTo("StudentList" + suppliedStaffUSI));
            Assert.That(actualModel.SchoolCategory, Is.EqualTo(SchoolCategory.HighSchool));
            Assert.That(actualModel.LocalEducationAgencyId, Is.EqualTo(suppliedLocalEducationAgencyId));
        }

        [Test]
        public virtual void Should_correctly_set_metadata()
        {
            Assert.That(actualModel.ListMetadata, Is.EqualTo(suppliedMetadataColumnGroupList));
        }

        [Test]
        public virtual void Should_correctly_select_students()
        {
            Assert.That(actualModel.Students.Count, Is.EqualTo(expectedStudentUSI.Count));

            Assert.That(expectedStudentUSI.Contains(actualModel.Students.ElementAt(0).StudentUSI), Is.True);
            var expectedStudentListData = GetStudentList().Single(x => x.StudentUSI == actualModel.Students.ElementAt(0).StudentUSI);

            Assert.That(actualModel.Students.ElementAt(0).StudentUSI, Is.EqualTo(expectedStudentListData.StudentUSI));
            Assert.That(actualModel.Students.ElementAt(0).SchoolId, Is.EqualTo(suppliedSchoolId1));
            Assert.That(actualModel.Students.ElementAt(0).Name, Is.EqualTo(Utilities.FormatPersonNameByLastName(expectedStudentListData.FirstName, expectedStudentListData.MiddleName, expectedStudentListData.LastSurname)));
            //Assert.That(actualModel.Students.ElementAt(0).Href.Href, Is.EqualTo(studentSchoolLinks.Overview(suppliedSchoolId1, expectedStudentListData.StudentUSI, expectedStudentListData.FullName, new { listContext = suppliedUniqueListId })));
            Assert.That(actualModel.Students.ElementAt(0).Href.Href, Is.EqualTo(studentSchoolLinks.Overview(suppliedSchoolId1, expectedStudentListData.StudentUSI, expectedStudentListData.FullName, new { listContext = "StudentList" + suppliedStaffUSI })));
            Assert.That(actualModel.Students.ElementAt(0).Metrics, Is.EqualTo(suppliedAdditionalMetrics));
        }
    }

    public class When_loading_student_list_cohort : StudentListServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.Cohort.ToString();
            suppliedSectionOrCohortId = 1;
            expectedStudentUSI = new List<long> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 9991, 9992, 9993, 9994, 9995, 9996, 9997 };

            base.EstablishContext();
        }
    }
}