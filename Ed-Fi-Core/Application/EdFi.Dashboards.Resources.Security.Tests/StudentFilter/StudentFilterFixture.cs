// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Tests;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Security.Tests.StudentFilter
{
    public abstract class When_servicing_student_data_base : TestFixtureBase
    {
        #region Data constants

        protected const int school1 = 4001;
        protected const int school2 = 4002;
        protected const int school3 = 4003;
        protected const int localEducationAgency1 = 4004;
        protected const long student1_1 = 101;
        protected const long student2_1 = 102;
        protected const long student3_1 = 103;
        protected const long student4_1 = 104;
        protected const long student1_2 = 201;
        protected const long student2_2 = 202;
        protected const long student3_2 = 203;
        protected const long student4_2 = 204;
        protected const long student1_3 = 301;
        protected const long student2_3 = 302;
        protected const long student3_3 = 303;
        protected const long student4_3 = 304;
        protected const long principal1 = 7001;
        protected const long principal2 = 7002; // principal2 is associated with school2 and school3
        protected const long principal3 = 7003;
        protected const long superintendent1 = 7004;
        protected const long staff1 = 8001;
        protected const long staff2 = 8002; // staff2 is associated with school2 and school3
        protected const long staff3 = 8003;
        protected const long staff4 = 8004;
        protected const long staff5 = 8005;
        protected const int staffCohort1_1 = 800101;
        protected const int staffCohort1_2 = 800102;
        protected const int staffCohort2_1 = 800201;
        protected const int staffCohort2_2 = 800202;
        protected const int staffCohort2_3 = 800203;
        protected const long teacher1 = 9001;
        protected const long teacher2 = 9002; // teacher2 is associated with school2 and school3
        protected const long teacher3 = 9003;
        protected const int teacherSection1_1 = 900101;
        protected const int teacherSection1_2 = 900102;
        protected const int teacherSection2_1 = 900201;
        protected const int teacherSection2_2 = 900202;
        protected const int teacherSection2_3 = 900203;

        #endregion

        protected IWindsorContainer windsorContainer;
        protected IAuthorizationInformationProvider authorizationInformationProvider;

        protected ISessionStateProvider sessionStateProvider;

        protected long providedStaffUSI;
        protected string providedUserName;
        protected string[] providedRoleNames;
        protected List<long> providedUserAllowedStudentUSIs = new List<long>();
        protected long[] allStudentUSIs = new[] {student1_1, student1_2, student1_3, student2_1, student2_2, student2_3, student3_1, student3_2, student3_3, student4_1, student4_2, student4_3};
        protected string providedFiller1 = "this is filler data";
        protected string providedFiller2 = "this is more filler data";
        protected short providedSchoolYear = 2011;
        protected bool registerAccessibleStudentsInBase = true;

        protected NoStudentData actualNoStudentData;
        protected IEnumerable<NoStudentData> actualNoStudentDataEnumerable;
        protected IEnumerable<NoStudentData> actualNoStudentDataQueryable;
        protected List<NoStudentData> actualNoStudentDataList;
        protected StudentData actualStudentData;
        protected IEnumerable<StudentData> actualStudentDataEnumerable;
        protected IEnumerable<StudentData> actualStudentDataQueryable;
        protected List<StudentData> actualStudentDataList;
        protected NestedStudentData actualNestedStudentData;
        protected IEnumerable<NestedStudentData> actualNestedStudentDataEnumerable;
        protected IEnumerable<NestedStudentData> actualNestedStudentDataQueryable;
        protected List<NestedStudentData> actualNestedStudentDataList;
        protected NestedStudentDataCollection actualNestedStudentDataCollection;
        protected IEnumerable<NestedStudentDataCollection> actualNestedStudentDataCollectionEnumerable;
        protected IEnumerable<NestedStudentDataCollection> actualNestedStudentDataCollectionQueryable;
        protected List<NestedStudentDataCollection> actualNestedStudentDataCollectionList;
        protected StudentDataList actualStudentDataTypedList;
        protected ICurrentUserClaimInterrogator currentUserClaimInterrogator;
        protected ICurrentUserAccessibleStudentsProvider currentUserAccessibleStudentsProvider;

        protected override void EstablishContext()
        {
            windsorContainer = new WindsorContainer();
            authorizationInformationProvider = mocks.StrictMock<IAuthorizationInformationProvider>();
            sessionStateProvider = mocks.StrictMock<ISessionStateProvider>();
            currentUserClaimInterrogator = mocks.StrictMock<ICurrentUserClaimInterrogator>();
            currentUserAccessibleStudentsProvider = mocks.StrictMock<ICurrentUserAccessibleStudentsProvider>();
            RegisterServices(windsorContainer);
            var userAssociatedOrgs = UserInformation.Current.AssociatedSchools;
            foreach(var associatedOrg in userAssociatedOrgs)
            {
                foreach(var claimType in associatedOrg.ClaimTypes)
                {
                    Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(claimType,associatedOrg.EducationOrganizationId
                                                                                                          )).Repeat.Any().Return(true);
                }
            }
            var userAssociatedLeaOrgs = UserInformation.Current.AssociatedLocalEducationAgencies;
            foreach (var associatedOrg in userAssociatedLeaOrgs)
            {
                foreach (var claimType in associatedOrg.ClaimTypes)
                {
                    Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(claimType, associatedOrg.EducationOrganizationId
                                                                                                          )).Repeat.Any().Return(true);
                }
            }
            Expect.Call(currentUserClaimInterrogator.HasClaimForStateAgency(string.Empty)).Repeat.Any().Return(false).IgnoreArguments();
            
            CreateEdFiDashboardContext(LoginHelper.localEducationAgencyOneId);

            if (registerAccessibleStudentsInBase)
                Expect.Call(currentUserAccessibleStudentsProvider.GetAccessibleStudents(1, false)).IgnoreArguments().Repeat.
                    Any().Return(new AccessibleStudents { CanAccessAllStudents = false, StudentUSIs = GetAllStaffStudents() });

            //Expect.Call(authorizationInformationProvider.GetAllStaffStudentUSIs(providedStaffUSI)).Repeat.Any().Return(GetAllStaffStudents());

            //Expect.Call(authorizationInformationProvider.GetPrincipalStudentUSIs(providedStaffUSI)).Repeat.Any().Return(GetPrincipalStudents());
            //Expect.Call(authorizationInformationProvider.GetStaffCohortStudentUSIs(providedStaffUSI)).Repeat.Any().Return(GetCohortStudents());
            //Expect.Call(authorizationInformationProvider.GetTeacherStudentUSIs(providedStaffUSI)).Repeat.Any().Return(GetTeacherStudents());

            base.EstablishContext();
        }

        protected void CreateEdFiDashboardContext(int localEducationAgencyId)
        {
            var dashboardContext = new EdFiDashboardContext
            {
                LocalEducationAgencyId = localEducationAgencyId
            };

            CallContext.SetData(EdFiDashboardContext.CallContextKey, dashboardContext);
        }

        private void RegisterServices(IWindsorContainer container)
        {
            container.Register(
                Component
                    .For(typeof(IAuthorizationInformationProvider))
                    .Instance(authorizationInformationProvider));
            container.Register(
                Component
                    .For(typeof(ISessionStateProvider))
                    .Instance(sessionStateProvider));

            container.Register(
                Component
                    .For(typeof(StudentInterceptor))
                    .ImplementedBy(typeof(StudentInterceptor)));

            container.Register(Component.For(typeof(StageInterceptor)).UsingFactoryMethod(CreateStageInterceptor));

            container.Register(
                Component
                    .For(typeof(ITestService))
                    .ImplementedBy(typeof(TestService))
                    .Interceptors<StageInterceptor>());

            container.Register(Component.For<ICurrentUserClaimInterrogator>().Instance(currentUserClaimInterrogator));

            container.Register(Component.For<ICurrentUserAccessibleStudentsProvider>().Instance(currentUserAccessibleStudentsProvider));
        }

        private StageInterceptor CreateStageInterceptor()
        {
            var stages = new Lazy<IInterceptorStage>[1];
            stages[0] = new Lazy<IInterceptorStage>(windsorContainer.Resolve<StudentInterceptor>);
            return new StageInterceptor(stages);
        }

        protected virtual HashSet<long> GetAllStaffStudents()
        {
            var allStaffList = new HashSet<long>();
            
            foreach (var studentUSI in GetPrincipalStudents())
                allStaffList.Add(studentUSI);
            
            foreach (var studentUSI in GetTeacherStudents())
                allStaffList.Add(studentUSI);
            
            foreach (var studentUSI in GetCohortStudents())
                allStaffList.Add(studentUSI);

            return allStaffList;
        }

        protected virtual List<long> GetPrincipalStudents()
        {
            return new List<long>();
        }

        protected virtual List<long> GetCohortStudents()
        {
            return new List<long>();
        }

        protected virtual List<long> GetTeacherStudents()
        {
            return new List<long>();
        }

        protected override void ExecuteTest()
        {
            var testService = windsorContainer.Resolve<ITestService>();

            actualNoStudentData = testService.GetNoStudentData(providedFiller1);
            actualNoStudentDataEnumerable = testService.GetNoStudentDataEnumerable(GetNoStudentDataEnumerable());
            actualNoStudentDataQueryable = testService.GetNoStudentDataQueryable(GetNoStudentDataEnumerable().AsQueryable());
            actualNoStudentDataList = testService.GetNoStudentDataList(GetNoStudentDataEnumerable());

            actualStudentData = testService.GetStudentData(student1_1, providedFiller1);
            actualStudentDataEnumerable = testService.GetStudentDataEnumerable(GetStudentDataEnumerable());
            actualStudentDataQueryable = testService.GetStudentDataQueryable(GetStudentDataEnumerable().AsQueryable());
            actualStudentDataList = testService.GetStudentDataList(GetStudentDataEnumerable());

            actualNestedStudentData = testService.GetNestedStudentData(student1_1, providedFiller1, providedFiller2);
            actualNestedStudentDataEnumerable = testService.GetNestedStudentDataEnumerable(GetNestedStudentDataEnumerable());
            actualNestedStudentDataQueryable = testService.GetNestedStudentDataQueryable(GetNestedStudentDataEnumerable().AsQueryable());
            actualNestedStudentDataList = testService.GetNestedStudentDataList(GetNestedStudentDataEnumerable());

            actualNestedStudentDataCollection = testService.GetNestedStudentDataCollection(GetStudentDataEnumerable().ToList(), providedFiller1);
            actualNestedStudentDataCollectionEnumerable = testService.GetNestedStudentDataCollectionEnumerable(GetNestedStudentDataCollectionEnumerable());
            actualNestedStudentDataCollectionQueryable = testService.GetNestedStudentDataCollectionQueryable(GetNestedStudentDataCollectionEnumerable().AsQueryable());
            actualNestedStudentDataCollectionList = testService.GetNestedStudentDataCollectionList(GetNestedStudentDataCollectionEnumerable());

            actualStudentDataTypedList = testService.GetStudentDataList(GetStudentDataList());
        }

        protected IEnumerable<NoStudentData> GetNoStudentDataEnumerable()
        {
            return allStudentUSIs.Select(studentUSI => new NoStudentData {Filler = providedFiller1});
        }

        protected IEnumerable<StudentData> GetStudentDataEnumerable()
        {
            return allStudentUSIs.Select(studentUSI => new StudentData { StudentUSI = studentUSI, Filler = providedFiller1 });
        }

        protected IEnumerable<NestedStudentData> GetNestedStudentDataEnumerable()
        {
            return allStudentUSIs.Select(studentUSI => new NestedStudentData { StudentData = new StudentData { StudentUSI = studentUSI, Filler = providedFiller1 }, Filler = providedFiller2 });
        }

        protected IEnumerable<NestedStudentDataCollection> GetNestedStudentDataCollectionEnumerable()
        {
            var collection = new List<NestedStudentDataCollection>();
            for (int i = 0; i < 5; i++)
            {
                collection.Add(new NestedStudentDataCollection {StudentData = allStudentUSIs.Select(studentUSI => new StudentData {StudentUSI = studentUSI, Filler = providedFiller1}).ToList(), Filler = providedFiller2});
            }
            return collection;
        }

        protected StudentDataList GetStudentDataList()
        {
            var list = new StudentDataList();
            list.AddRange(allStudentUSIs.Select(studentUSI => new StudentData {StudentUSI = studentUSI, Filler = providedFiller1}));
            list.Filler = providedFiller2;
            return list;
        }

        [Test]
        public void User_should_see_data_with_no_student_info()
        {
            Assert.That(actualNoStudentData, Is.Not.Null);
            Assert.That(actualNoStudentData, Is.InstanceOf<NoStudentData>());
            Assert.That(actualNoStudentData.Filler, Is.EqualTo(providedFiller1));
        }

        [Test]
        public void User_should_see_enumerable_with_no_student_info()
        {
            Assert.That(actualNoStudentDataEnumerable, Is.Not.Null);
            Assert.That(actualNoStudentDataEnumerable.Count(), Is.EqualTo(allStudentUSIs.Length));
            var i = 0;
            foreach (var data in actualNoStudentDataEnumerable)
            {
                Assert.That(data.Filler, Is.EqualTo(providedFiller1), "Item " + i);
                i++;
            }
        }

        [Test]
        public void User_should_see_queryable_with_no_student_info()
        {
            Assert.That(actualNoStudentDataQueryable, Is.Not.Null);
            Assert.That(actualNoStudentDataQueryable.Count(), Is.EqualTo(allStudentUSIs.Length));
            var i = 0;
            foreach (var data in actualNoStudentDataQueryable)
            {
                Assert.That(data.Filler, Is.EqualTo(providedFiller1), "Item " + i);
                i++;
            }
        }

        [Test]
        public void User_should_see_list_with_no_student_info()
        {
            Assert.That(actualNoStudentDataList, Is.Not.Null);
            Assert.That(actualNoStudentDataList, Is.InstanceOf<IList<NoStudentData>>());
            Assert.That(actualNoStudentDataList.Count(), Is.EqualTo(allStudentUSIs.Length));
            var i = 0;
            foreach (var data in actualNoStudentDataList)
            {
                Assert.That(data.Filler, Is.EqualTo(providedFiller1), "Item " + i);
                i++;
            }
        }

        [Test]
        public void User_should_see_limited_data_with_student_info()
        {
            if (!providedUserAllowedStudentUSIs.Contains(student1_1))
            {
                Assert.That(actualStudentData, Is.Null);
                return;
            }

            Assert.That(actualStudentData, Is.Not.Null);
            Assert.That(actualStudentData, Is.InstanceOf<StudentData>());
            Assert.That(actualStudentData.Filler, Is.EqualTo(providedFiller1));
            Assert.That(actualStudentData.StudentUSI, Is.EqualTo(student1_1));
        }

        [Test]
        public void User_should_see_limited_enumerable_data_with_student_info()
        {
            Assert.That(actualStudentDataEnumerable, Is.Not.Null);
            Assert.That(actualStudentDataEnumerable, Is.InstanceOf<IEnumerable<StudentData>>());
            Assert.That(actualStudentDataEnumerable.Count(), Is.EqualTo(providedUserAllowedStudentUSIs.Count));
            var i = 0;
            foreach (var data in actualStudentDataEnumerable)
            {
                Assert.That(data.Filler, Is.EqualTo(providedFiller1), "Item " + i);
                Assert.That(providedUserAllowedStudentUSIs.Contains(data.StudentUSI), Is.True, data.StudentUSI + " was returned, but not allowed for the user");
                i++;
            }
        }

        [Test]
        public void User_should_see_limited_queryable_data_with_student_info()
        {
            Assert.That(actualStudentDataQueryable, Is.Not.Null);
            Assert.That(actualStudentDataQueryable.Count(), Is.EqualTo(providedUserAllowedStudentUSIs.Count));
            var i = 0;
            foreach (var data in actualStudentDataQueryable)
            {
                Assert.That(data.Filler, Is.EqualTo(providedFiller1), "Item " + i);
                Assert.That(providedUserAllowedStudentUSIs.Contains(data.StudentUSI), Is.True, data.StudentUSI + " was returned, but not allowed for the user");
                i++;
            }
        }

        [Test]
        public void User_should_see_limited_list_data_with_student_info()
        {
            Assert.That(actualStudentDataList, Is.Not.Null);
            Assert.That(actualStudentDataList, Is.InstanceOf<IList<StudentData>>());
            Assert.That(actualStudentDataList.Count(), Is.EqualTo(providedUserAllowedStudentUSIs.Count));
            var i = 0;
            foreach (var data in actualStudentDataList)
            {
                Assert.That(data.Filler, Is.EqualTo(providedFiller1), "Item " + i);
                Assert.That(providedUserAllowedStudentUSIs.Contains(data.StudentUSI), Is.True, data.StudentUSI + " was returned, but not allowed for the user");
                i++;
            }
        }

        [Test]
        public void User_should_see_limited_data_with_nested_student_info()
        {
            Assert.That(actualNestedStudentData, Is.Not.Null);
            Assert.That(actualNestedStudentData, Is.InstanceOf<NestedStudentData>());
            Assert.That(actualNestedStudentData.Filler, Is.EqualTo(providedFiller2));
            if (!providedUserAllowedStudentUSIs.Contains(student1_1))
            {
                Assert.That(actualNestedStudentData.StudentData, Is.Null);
            }
            else
            {
                Assert.That(actualNestedStudentData.StudentData.StudentUSI, Is.EqualTo(student1_1));
                Assert.That(actualNestedStudentData.StudentData.Filler, Is.EqualTo(providedFiller1));
            }
        }

        [Test]
        public void User_should_see_limited_enumerable_data_with_nested_student_info()
        {
            Assert.That(actualNestedStudentDataEnumerable, Is.Not.Null);
            Assert.That(actualNestedStudentDataEnumerable, Is.InstanceOf<IEnumerable<NestedStudentData>>());
            Assert.That(actualNestedStudentDataEnumerable.Count(), Is.EqualTo(allStudentUSIs.Length));
            var i = 0;
            foreach (var data in actualNestedStudentDataEnumerable)
            {
                Assert.That(data.Filler, Is.EqualTo(providedFiller2), "Item " + i);
                i++;
            }

            foreach (var studentUSI in allStudentUSIs)
            {
                NestedStudentData studentData = actualNestedStudentDataEnumerable.FirstOrDefault(x => x.StudentData != null && x.StudentData.StudentUSI == studentUSI);
                if (!providedUserAllowedStudentUSIs.Contains(studentUSI))
                {
                    Assert.That(studentData, Is.Null, studentUSI + " was returned, but not allowed for the user");
                }
                else
                {
                    Assert.NotNull(studentData, "Missing studentUSI: " + studentUSI);
                    Assert.That(studentData.StudentData.StudentUSI, Is.EqualTo(studentUSI));
                    Assert.That(studentData.StudentData.Filler, Is.EqualTo(providedFiller1));
                }
            }
        }

        [Test]
        public void User_should_see_limited_queryable_data_with_nested_student_info()
        {
            Assert.That(actualNestedStudentDataQueryable, Is.Not.Null);
            Assert.That(actualNestedStudentDataQueryable.Count(), Is.EqualTo(allStudentUSIs.Length));
            var i = 0;
            foreach (var data in actualNestedStudentDataEnumerable)
            {
                Assert.That(data.Filler, Is.EqualTo(providedFiller2), "Item " + i);
                i++;
            }

            foreach (var studentUSI in allStudentUSIs)
            {
                NestedStudentData studentData = actualNestedStudentDataEnumerable.FirstOrDefault(x => x.StudentData != null && x.StudentData.StudentUSI == studentUSI);
                if (!providedUserAllowedStudentUSIs.Contains(studentUSI))
                {
                    Assert.That(studentData, Is.Null, studentUSI + " was returned, but not allowed for the user");
                }
                else
                {
                    Assert.NotNull(studentData, "Missing studentUSI: " + studentUSI);
                    Assert.That(studentData.StudentData.StudentUSI, Is.EqualTo(studentUSI));
                    Assert.That(studentData.StudentData.Filler, Is.EqualTo(providedFiller1));
                }
            }
        }

        [Test]
        public void User_should_see_limited_list_data_with_nested_student_info()
        {
            Assert.That(actualNestedStudentDataList, Is.Not.Null);
            Assert.That(actualNestedStudentDataList, Is.InstanceOf<IList<NestedStudentData>>());
            Assert.That(actualNestedStudentDataList.Count(), Is.EqualTo(allStudentUSIs.Length));
            var i = 0;
            foreach (var data in actualNestedStudentDataEnumerable)
            {
                Assert.That(data.Filler, Is.EqualTo(providedFiller2), "Item " + i);
                i++;
            }

            foreach (var studentUSI in allStudentUSIs)
            {
                NestedStudentData studentData = actualNestedStudentDataEnumerable.FirstOrDefault(x => x.StudentData != null && x.StudentData.StudentUSI == studentUSI);
                if (!providedUserAllowedStudentUSIs.Contains(studentUSI))
                {
                    Assert.That(studentData, Is.Null, studentUSI + " was returned, but not allowed for the user");
                }
                else
                {
                    Assert.NotNull(studentData, "Missing studentUSI: " + studentUSI);
                    Assert.That(studentData.StudentData.StudentUSI, Is.EqualTo(studentUSI));
                    Assert.That(studentData.StudentData.Filler, Is.EqualTo(providedFiller1));
                }
            }
        }

        [Test]
        public void User_should_see_limited_data_with_nested_student_info_collection()
        {
            Assert.That(actualNestedStudentDataCollection, Is.Not.Null);
            Assert.That(actualNestedStudentDataCollection, Is.InstanceOf<NestedStudentDataCollection>());
            Assert.That(actualNestedStudentDataCollection.StudentData.Count, Is.EqualTo(providedUserAllowedStudentUSIs.Count));
            Assert.That(actualNestedStudentDataCollection.Filler, Is.EqualTo(providedFiller1));
            int i = 0;
            foreach (var data in actualNestedStudentDataCollection.StudentData)
            {
                Assert.That(data.Filler, Is.EqualTo(providedFiller1), "Item " + i);
                Assert.That(providedUserAllowedStudentUSIs.Contains(data.StudentUSI), Is.True, data.StudentUSI + " was returned, but not allowed for the user");
                i++;
            }
        }

        [Test]
        public void User_should_see_limited_enumerable_data_with_nested_student_info_collection()
        {
            Assert.That(actualNestedStudentDataCollectionEnumerable, Is.Not.Null);
            Assert.That(actualNestedStudentDataCollectionEnumerable, Is.InstanceOf<IEnumerable<NestedStudentDataCollection>>());
            Assert.That(actualNestedStudentDataCollectionEnumerable.Count(), Is.EqualTo(5));
            var i = 0;
            foreach (var data in actualNestedStudentDataCollectionEnumerable)
            {
                Assert.That(data.Filler, Is.EqualTo(providedFiller2), "Item " + i);
                var j = 0;
                foreach (var student in data.StudentData)
                {
                    Assert.That(student.Filler, Is.EqualTo(providedFiller1), "Item " + j);
                    Assert.That(providedUserAllowedStudentUSIs.Contains(student.StudentUSI), Is.True, student.StudentUSI + " was returned, but not allowed for the user");
                    j++;
                }
                i++;
            }
        }

        [Test]
        public void User_should_see_limited_queryable_data_with_nested_student_info_collection()
        {
            Assert.That(actualNestedStudentDataCollectionQueryable, Is.Not.Null);
            Assert.That(actualNestedStudentDataCollectionQueryable.Count(), Is.EqualTo(5));
            var i = 0;
            foreach (var data in actualNestedStudentDataCollectionQueryable)
            {
                Assert.That(data.Filler, Is.EqualTo(providedFiller2), "Item " + i);
                var j = 0;
                foreach (var student in data.StudentData)
                {
                    Assert.That(student.Filler, Is.EqualTo(providedFiller1), "Item " + j);
                    Assert.That(providedUserAllowedStudentUSIs.Contains(student.StudentUSI), Is.True, student.StudentUSI + " was returned, but not allowed for the user");
                    j++;
                }
                i++;
            }
        }

        [Test]
        public void User_should_see_limited_list_data_with_nested_student_info_collection()
        {
            Assert.That(actualNestedStudentDataCollectionList, Is.Not.Null);
            Assert.That(actualNestedStudentDataCollectionList, Is.InstanceOf<IList<NestedStudentDataCollection>>());
            Assert.That(actualNestedStudentDataCollectionList.Count(), Is.EqualTo(5));
            var i = 0;
            foreach (var data in actualNestedStudentDataCollectionList)
            {
                Assert.That(data.Filler, Is.EqualTo(providedFiller2), "Item " + i);
                var j = 0;
                foreach (var student in data.StudentData)
                {
                    Assert.That(student.Filler, Is.EqualTo(providedFiller1), "Item " + j);
                    Assert.That(providedUserAllowedStudentUSIs.Contains(student.StudentUSI), Is.True, student.StudentUSI + " was returned, but not allowed for the user");
                    j++;
                }
                i++;
            }
        }

        [Test]
        public void User_should_see_limited_student_info_list()
        {
            Assert.That(actualStudentDataTypedList, Is.Not.Null);
            Assert.That(actualStudentDataTypedList, Is.InstanceOf<StudentDataList>());
            Assert.That(actualStudentDataTypedList.Filler, Is.EqualTo(providedFiller2));
            Assert.That(actualStudentDataTypedList.Count, Is.EqualTo(providedUserAllowedStudentUSIs.Count));
            foreach (var data in actualStudentDataTypedList)
            {
                Assert.That(data, Is.Not.Null);
                Assert.That(providedUserAllowedStudentUSIs.Contains(data.StudentUSI), Is.True, data.StudentUSI + " was returned, but not allowed for the user");
                Assert.That(data.Filler, Is.EqualTo(providedFiller1));
            }
        }
    }

    [TestFixture]
    public class When_superintendent_accesses_student_data : When_servicing_student_data_base
    {
        protected override void EstablishContext()
        {
            providedStaffUSI = LoginHelper.staffUSISuperintendent;
            LoginHelper.LoginSuperintendent();

            providedUserAllowedStudentUSIs.AddRange(allStudentUSIs);

            registerAccessibleStudentsInBase = false;

            base.EstablishContext();

            Expect.Call(currentUserAccessibleStudentsProvider.GetAccessibleStudents(1, false)).IgnoreArguments().Repeat.Any()
                .Return(new AccessibleStudents { CanAccessAllStudents = false, StudentUSIs = allStudentUSIs.ToList().ToHashSet() });
        }
    }

    [TestFixture]
    public class When_principal_accesses_student_data : When_servicing_student_data_base
    {
        protected override void EstablishContext()
        {
            LoginHelper.LoginPrincipalOneSchool();
            providedStaffUSI = LoginHelper.staffUSIPrincipalOneSchool;

            providedUserAllowedStudentUSIs.Add(student1_1);
            providedUserAllowedStudentUSIs.Add(student2_1);
            providedUserAllowedStudentUSIs.Add(student3_1);
            providedUserAllowedStudentUSIs.Add(student4_1);

            base.EstablishContext();
        }
        
        protected override List<long> GetPrincipalStudents()
        {
            return providedUserAllowedStudentUSIs;
        }
    }

    [TestFixture]
    public class When_principal_with_two_schools_accesses_student_data : When_servicing_student_data_base
    {
        protected override void EstablishContext()
        {
            LoginHelper.LoginPrincipalTwoSchool();
            providedStaffUSI = LoginHelper.staffUSIPrincipalTwoSchool;

            providedUserAllowedStudentUSIs.Add(student1_2);
            providedUserAllowedStudentUSIs.Add(student2_2);
            providedUserAllowedStudentUSIs.Add(student3_2);
            providedUserAllowedStudentUSIs.Add(student4_2);
            providedUserAllowedStudentUSIs.Add(student1_3);
            providedUserAllowedStudentUSIs.Add(student2_3);
            providedUserAllowedStudentUSIs.Add(student3_3);
            providedUserAllowedStudentUSIs.Add(student4_3);

            base.EstablishContext();
        }

        protected override List<long> GetPrincipalStudents()
        {
            return providedUserAllowedStudentUSIs;
        }
    }

    [TestFixture]
    public class When_teacher_accesses_student_data : When_servicing_student_data_base
    {
        protected override void EstablishContext()
        {
            LoginHelper.LoginTeacherOne();
            providedStaffUSI = LoginHelper.staffUSITeacherOne;

            providedUserAllowedStudentUSIs.Add(student1_1);
            providedUserAllowedStudentUSIs.Add(student2_1);
            providedUserAllowedStudentUSIs.Add(student3_1);

            base.EstablishContext();
        }

        protected override List<long> GetTeacherStudents()
        {
            return providedUserAllowedStudentUSIs;
        }
    }

    [TestFixture]
    public class When_teacher_with_two_schools_accesses_student_data : When_servicing_student_data_base
    {
        protected override void EstablishContext()
        {
            providedStaffUSI = LoginHelper.staffUSITeacherTwo;
            LoginHelper.LoginTeacherTwo();

            providedUserAllowedStudentUSIs.Add(student1_2);
            providedUserAllowedStudentUSIs.Add(student2_2);
            providedUserAllowedStudentUSIs.Add(student1_3);
            providedUserAllowedStudentUSIs.Add(student3_3);

            base.EstablishContext();
        }

        protected override List<long> GetTeacherStudents()
        {
            return providedUserAllowedStudentUSIs;
        }
    }

    [TestFixture]
    public class When_school_specialist_accesses_student_data : When_servicing_student_data_base
    {
        protected override void EstablishContext()
        {
            LoginHelper.LoginSchoolSpecialistOne();
            providedStaffUSI = LoginHelper.staffUSISchoolSpecialistOne;

            providedUserAllowedStudentUSIs.Add(student1_1);
            providedUserAllowedStudentUSIs.Add(student2_1);
            providedUserAllowedStudentUSIs.Add(student3_1);

            base.EstablishContext();
        }

        protected override List<long> GetCohortStudents()
        {
            return providedUserAllowedStudentUSIs;
        }
    }

    [TestFixture]
    public class When_school_specialist_with_two_schools_accesses_student_data : When_servicing_student_data_base
    {
        protected override void EstablishContext()
        {
            providedStaffUSI = LoginHelper.staffUSISchoolSpecialistTwo;
            LoginHelper.LoginSchoolSpecialistTwo();

            providedUserAllowedStudentUSIs.Add(student1_2);
            providedUserAllowedStudentUSIs.Add(student2_2);
            providedUserAllowedStudentUSIs.Add(student1_3);
            providedUserAllowedStudentUSIs.Add(student3_3);

            base.EstablishContext();
        }

        protected override List<long> GetCohortStudents()
        {
            return providedUserAllowedStudentUSIs;
        }
    }

    [TestFixture]
    public class When_local_education_agency_system_administrator_accesses_student_data : When_servicing_student_data_base
    {
        protected override void EstablishContext()
        {
            providedStaffUSI = LoginHelper.staffUSILocalEducationAgencyAdministrator;
            LoginHelper.LoginLocalEducationAgencySystemAdministratorOne();

            providedUserAllowedStudentUSIs.AddRange(allStudentUSIs);

            registerAccessibleStudentsInBase = false;

            base.EstablishContext();

            Expect.Call(currentUserAccessibleStudentsProvider.GetAccessibleStudents(1, false)).IgnoreArguments().Repeat.Any()
                .Return(new AccessibleStudents { CanAccessAllStudents = false, StudentUSIs = allStudentUSIs.ToList().ToHashSet() });
        }
    }

    public static class ListExtensions
    {
        public static HashSet<long> ToHashSet(this List<long> list)
        {
            var hash = new HashSet<long>();

            foreach (var entry in list)
                hash.Add(entry);

            return hash;
        }
    }

    [TestFixture]
    public class When_local_education_agency_administrator_accesses_student_data : When_servicing_student_data_base
    {
        protected override void EstablishContext()
        {
            LoginHelper.LoginLocalEducationAgencyAdministrator();

            base.EstablishContext();
        }
    }

    [TestFixture]
    public class When_school_administrator_accesses_student_data : When_servicing_student_data_base
    {
        protected override void EstablishContext()
        {
            LoginHelper.LoginSchoolAdministrator();

            base.EstablishContext();
        }
    }
}
