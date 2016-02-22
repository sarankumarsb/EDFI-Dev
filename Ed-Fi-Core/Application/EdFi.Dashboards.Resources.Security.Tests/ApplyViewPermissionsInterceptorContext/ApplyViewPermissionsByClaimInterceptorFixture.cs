// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Security;
using System.Threading;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Security.AuthorizationDelegates;
using EdFi.Dashboards.Resources.Security.ClaimValidators;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Security.Implementations;
using EdFi.Dashboards.Resources.Security.Tests.ClaimValidators;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;
using IdNameRequest = EdFi.Dashboards.Resources.School.IdNameRequest;
using IIdNameService = EdFi.Dashboards.Resources.School.IIdNameService;
using InformationRequest = EdFi.Dashboards.Resources.StudentSchool.InformationRequest;
using OverviewRequest = EdFi.Dashboards.Resources.StudentSchool.OverviewRequest;

namespace EdFi.Dashboards.Resources.Security.Tests.ApplyViewPermissionsInterceptorContext
{
    #region Test Services
    public interface ITestServiceBase
    {
        bool Validate_LocalEducationAgency(int localEducationAgencyId);

        bool Validate_SchoolStaff(int schoolId, long staffUSI);

        bool Validate_StudentInformationRequest(InformationRequest requestObject);

        bool Validate_StudentOverviewRequest(OverviewRequest requestObject);

        bool CheckParameterMarkedIgnoreValid([AuthenticationIgnore("This is a test fixture.")] long markedIgnore);

        bool CheckParameterMarkedIgnoreInvalid([AuthenticationIgnore("This is a test fixture.")] long studentUSI);

        bool Validate_RequestObjectValid(RequestObjectValid requestObject);

        bool Validate_RequestObjectInvalid(RequestObjectInvalid requestObject);

        // DJWhite: 30 Jan 2012
        // This tests validation by CanBeAuthorizedBy on the request object class
        //
        bool Validate_RequestObjectValidViewMyStudents(RequestObjectValidViewMyStudents requestObject);
    }

    public enum RunTestServiceCase
    {
        None,
        InheritAllMy,
        OverrideAllMy,
        OverrideMy,
        OverrideAll
    }

    public class RequestObjectValid
    {
        public int SchoolId { get; set; }
        public long StudentUSI { get; set; }
        [AuthenticationIgnore("This is a test fixture.")]
        public int UnknownParameter { get; set; }
    }

    public class MetricRequestObjectValid : RequestObjectValid
    {
        public int MetricVariantId { get; set; }
    }

    public class RequestObjectInvalid
    {
        public int SchoolId { get; set; }
        public long StudentUSI { get; set; }
        public int UnknownParameter { get; set; }
    }

    public class CanNotIgnoreRestrictedPropertyRequestObjectInvalid : RequestObjectInvalid
    {
        [AuthenticationIgnore("This is a test fixture.")]
        public new int SchoolId { get; set; }
        [AuthenticationIgnore("This is a test fixture.")]
        public new long StudentUSI { get; set; }
        [AuthenticationIgnore("This is a test fixture.")]
        public new int UnknownParameter { get; set; }
    }

    [CanBeAuthorizedBy(EdFiClaimTypes.ViewMyStudents)]
    public class RequestObjectValidViewMyStudents : RequestObjectValid
    {}

    public abstract class TestServiceBase : ITestServiceBase
    {
        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public virtual bool Validate_LocalEducationAgency(int localEducationAgencyId)
        {
            return true;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public virtual bool Validate_SchoolStaff(int schoolId, long staffUSI)
        {
            return true;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewMyStudents)]
        public virtual bool Validate_StudentInformationRequest(InformationRequest requestObject)
        {
            return true;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewMyStudents)]
        public virtual bool Validate_StudentOverviewRequest(OverviewRequest requestObject)
        {
            return true;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewMyStudents)]
        public virtual bool CheckParameterMarkedIgnoreValid(long markedIgnore)
        {
            return true;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewMyStudents)]
        public virtual bool CheckParameterMarkedIgnoreInvalid(long studentUSI)
        {
            return true;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewMyStudents)]
        public virtual bool Validate_RequestObjectValid(RequestObjectValid requestObject)
        {
            return true;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewMyStudents)]
        public virtual bool Validate_RequestObjectInvalid(RequestObjectInvalid requestObject)
        {
            return true;
        }

        public bool Validate_RequestObjectValidViewMyStudents(RequestObjectValidViewMyStudents requestObject)
        {
            return true;
        }
    }

    public class TestServiceInheritAllMy : TestServiceBase
    {
        public override bool Validate_LocalEducationAgency(int localEducationAgencyId)
        {
            return true;
        }

        public override bool Validate_SchoolStaff(int schoolId, long staffUSI)
        {
            return true;
        }

        public override bool Validate_StudentInformationRequest(InformationRequest requestObject)
        {
            return true;
        }

        public override bool Validate_StudentOverviewRequest(OverviewRequest requestObject)
        {
            return true;
        }
    }

    public class TestServiceOverrideAllMy : TestServiceBase
    {
        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public override bool Validate_LocalEducationAgency(int localEducationAgencyId)
        {
            return true;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public override bool Validate_SchoolStaff(int schoolId, long staffUSI)
        {
            return true;
        }
    }

    public class TestServiceOverrideMy : TestServiceBase
    {
        [CanBeAuthorizedBy(EdFiClaimTypes.ViewMyStudents)]
        public override bool Validate_LocalEducationAgency(int localEducationAgencyId)
        {
            return true;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewMyStudents)]
        public override bool Validate_SchoolStaff(int schoolId, long staffUSI)
        {
            return true;
        }
    }

    public class TestServiceOverrideAll : TestServiceBase
    {
        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents)]
        public override bool Validate_LocalEducationAgency(int localEducationAgencyId)
        {
            return true;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents)]
        public override bool Validate_SchoolStaff(int schoolId, long staffUSI)
        {
            return true;
        }
    }
#endregion

    #region Test Interceptor classes.
    public class TestStageInterceptor : StageInterceptor
    {
        public TestStageInterceptor() : base ( new Lazy<IInterceptorStage>[] {new Lazy<IInterceptorStage>(() => new TestInterceptor())} )
        {}
    }

    public class TestInterceptor : ApplyViewPermissionsByClaimInterceptor
    {
        public TestInterceptor()
            : base(ApplyViewPermissionsByClaimFixtureBase.CurrentClaimAuthorizations, ApplyViewPermissionsByClaimFixtureBase.CurrentAuthorizationDelegates, ApplyViewPermissionsByClaimFixtureBase.SupportedClaimNamesProvider)
        {}
    }
#endregion

    /// <summary>
    /// Provides a base class for all other tests that modifies the current thread's principal
    /// to have the roles provided by the template method GetSuppliedRolesForCurrentUser, and
    /// mocks the invocation method and arguments, expecting a call to the Proceed method (by default).
    /// It also catches any exceptions thrown by the invocation into a variable called exceptionThrown
    /// for subsequent inspection.
    /// </summary>
    public abstract class ApplyViewPermissionsByClaimFixtureBase : TestFixtureBase
    {
        public static WindsorContainer container;

        public static IClaimAuthorization[] CurrentClaimAuthorizations;
        public static IAuthorizationDelegate[] CurrentAuthorizationDelegates;
        public static ISupportedClaimNamesProvider SupportedClaimNamesProvider;

        protected Exception exceptionThrown;
        protected IEnumerable<ParameterInstance> myParameters;
        protected SecurityAssertionProvider mySecurityAssertionProvider;
        protected IAuthorizationInformationProvider myAuthorizationInformationProvider;
        protected IIdNameService _schoolIdNameService;
        protected ICurrentUserClaimInterrogator currentUserClaimInterrogator;

        // The value of RunTestCase determines which Concrete Class will be registered for 
        // the ITestServiceBase interface.
        //
        public RunTestServiceCase RunTestCase = RunTestServiceCase.None;
        protected ITestServiceBase testService;

        #region Methods to instantiate ClaimValidators and ClaimAuthorization.
        public IClaimAuthorization CreateAccessOrganizationClaim()
        {
            var nullValidator = new NullClaimValidator();
            var validator = new AccessOrganizationClaimValidatorLocalEducationAgency(mySecurityAssertionProvider, nullValidator);
            var result = new AccessOrganizationClaimAuthorization(validator);

            return result;
        }

        public IClaimAuthorization CreateAdministerDashboardClaim()
        {
            var nullValidator = new NullClaimValidator();
            var validator = new AdministerDashboardClaimValidatorLocalEducationAgency(mySecurityAssertionProvider, nullValidator);
            var result = new AdministerDashboardClaimAuthorization(validator);

            return result;
        }

        public IClaimAuthorization CreateManageGoalsDashboardClaim()
        {
            var nullValidator = new NullClaimValidator();
            var validator = new ManageGoalsClaimValidatorLocalEducationAgency(mySecurityAssertionProvider, nullValidator);
            var result = new ManageGoalsClaimAuthorization(validator);

            return result;
        }

        public IClaimAuthorization CreateManageWatchListClaimAuthorization()
        {
            var nullValidator = new NullClaimValidator();
            var validator = new ManageWatchListClaimValidatorLocalEducationAgencySchoolStaff(
                mySecurityAssertionProvider, nullValidator);
            var result = new ManageWatchListClaimAuthorization(validator);

            return result;
        }

        public IClaimAuthorization CreateViewAllStudentsClaimAuthorization()
        {
            var nullValidator = new NullClaimValidator();
            var validator = new ViewAllStudentsClaimValidatorLocalEducationAgency(mySecurityAssertionProvider, nullValidator);
            var result = new ViewAllStudentsClaimAuthorization(validator);
            return result;
        }

        public IClaimAuthorization CreateViewAllTeachersClaimAuthorization()
        {
            var nullValidator = new NullClaimValidator();
            var validator = new ViewAllTeachersClaimValidatorSchool(mySecurityAssertionProvider, nullValidator);
            var result = new ViewAllTeachersClaimAuthorization(validator);
            return result;
        }

        public IClaimAuthorization CreateViewAllMetricsClaimAuthorization()
        {
            var nullValidator = new NullClaimValidator();
            var validator = new ViewAllMetricsClaimValidatorLocalEducationAgency(mySecurityAssertionProvider, nullValidator);
            var result = new ViewAllMetricsClaimAuthorization(validator);

            return result;
        }

        public IClaimAuthorization CreateViewMyMetricsClaimAuthorization()
        {
            var nullValidator = new NullClaimValidator();
            var validator = new ViewMyMetricsClaimValidatorLocalEducationAgency(mySecurityAssertionProvider, nullValidator);
            var result = new ViewMyMetricsClaimAuthorization(validator);

            return result;
        }

        public IClaimAuthorization CreateViewMyStudentsClaimAuthorization()
        {
            var nullValidator = new NullClaimValidator();
            var validatorSchoolStaff = new ViewMyStudentsClaimValidatorSchoolStaff(mySecurityAssertionProvider, nullValidator);
            var validatorSchoolStudent = new ViewMyStudentsClaimValidatorSchoolStudent(mySecurityAssertionProvider, validatorSchoolStaff);
            var validatorLEASchoolStudent = new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStudent(mySecurityAssertionProvider, validatorSchoolStudent);

            var result = new ViewMyStudentsClaimAuthorization(validatorLEASchoolStudent);
            return result;
        }

        public IClaimAuthorization CreateViewOperationalDashboardClaimAuthorization()
        {
            var nullValidator = new NullClaimValidator();
            var validator = new ViewOperationalDashboardClaimValidatorLocalEducationAgency(mySecurityAssertionProvider, nullValidator);
            var result = new ViewOperationalDashboardClaimAuthorization(validator);
            return result;
        }

        public void CreateClaimAuthorizations()
        {
            CurrentClaimAuthorizations = new[]
                             {
                                 CreateAccessOrganizationClaim(),
                                 CreateAdministerDashboardClaim(),
                                 CreateManageGoalsDashboardClaim(),
                                 CreateManageWatchListClaimAuthorization(),
                                 CreateViewAllTeachersClaimAuthorization(),
                                 CreateViewAllStudentsClaimAuthorization(),
                                 CreateViewMyStudentsClaimAuthorization(),
                                 CreateViewAllMetricsClaimAuthorization(),
                                 CreateViewMyMetricsClaimAuthorization(),
                                 CreateViewOperationalDashboardClaimAuthorization()
                             };
            CurrentAuthorizationDelegates = new[]
                                                {
                                                    new LevelCrumbAuthorizationDelegate(mySecurityAssertionProvider)
                                                };
        }

        public static IEnumerable<string> GetSupportedClaimNames()
        {
            var type = typeof(EdFiClaimTypes);
            var fields = type.GetFields();
            var result = (from field in fields
                          let fieldValue = field.GetValue(null)
                          where ((fieldValue.ToString().StartsWith(EdFiClaimTypes._OrgClaimNamespace)) &&
                                 (!fieldValue.Equals(EdFiClaimTypes._OrgClaimNamespace)))
                          select field.Name).OrderBy(s => s);

            return result;
        }
#endregion

        #region Register Services.
        public void RegisterInheritClaimClasses()
        {
            container = new WindsorContainer();
            container.Register(
                Component
                    .For<TestStageInterceptor>());
        }
#endregion

        public const int VALID_LOCALEDUCATIONAGENCY = ClaimValidatorFixturesBase.LOCALEDUCATIONAGENCY_ID_01;
        public const int INVALID_LOCALEDUCATIONAGENCY = ClaimValidatorFixturesBase.LOCALEDUCATIONAGENCY_ID_00;
        public const int DEFAULTED_LOCALEDUCATIONAGENCY = 0;
        public const long VALID_STAFF = ClaimValidatorFixturesBase.CURRENT_USER_HAS_LOCALEDUCATIONAGENCY_LEVEL_CLAIMS_USI;
        public const long INVALID_STAFF = ClaimValidatorFixturesBase.CURRENT_USER_INVALID;
        public const int VALID_SCHOOL = ClaimValidatorFixturesBase.SCH_ID_01_01;
        public const int INVALID_SCHOOL = ClaimValidatorFixturesBase.SCH_ID_00_00;

        public const long VALID_STUDENT = ClaimValidatorFixturesBase.STUDENT_01_01;
        public const long INVALID_STUDENT = ClaimValidatorFixturesBase.STUDENT_00_01;

        public OverviewRequest VALID_SOR =
            new OverviewRequest
                {
                    SchoolId = VALID_SCHOOL,
                    StudentUSI = VALID_STUDENT
                };

        public OverviewRequest INVALID_SOR_INVALID_SCHOOL =
            new OverviewRequest
            {
                SchoolId = INVALID_SCHOOL,
                StudentUSI = VALID_STUDENT
            };

        public OverviewRequest INVALID_SOR_INVALID_STUDENT =
            new OverviewRequest
            {
                SchoolId = VALID_SCHOOL,
                StudentUSI = INVALID_STUDENT
            };

        public InformationRequest VALID_SIR =
            new InformationRequest
                {
                    LocalEducationAgencyId = VALID_LOCALEDUCATIONAGENCY,
                    SchoolId = VALID_SCHOOL,
                    StudentUSI = VALID_STUDENT
                };

        public InformationRequest INVALID_SIR_LOCALEDUCATIONAGENCY_INVALID =
            new InformationRequest
            {
                LocalEducationAgencyId = INVALID_LOCALEDUCATIONAGENCY,
                SchoolId = VALID_SCHOOL,
                StudentUSI = VALID_STUDENT
            };

        public InformationRequest INVALID_SIR_SCHOOL_INVALID =
            new InformationRequest
            {
                LocalEducationAgencyId = VALID_LOCALEDUCATIONAGENCY,
                SchoolId = INVALID_SCHOOL,
                StudentUSI = VALID_STUDENT
            };

        public InformationRequest INVALID_SIR_STUDENT_INVALID =
            new InformationRequest
            {
                LocalEducationAgencyId = VALID_LOCALEDUCATIONAGENCY,
                SchoolId = VALID_SCHOOL,
                StudentUSI = INVALID_STUDENT
            };

        // The only difference between these objects is that the UnknownParameter is marked AuthIgnore
        // on the RequestObjectValid class.
        //
        public RequestObjectValid REQUEST_OBJECT_VALID =
            new RequestObjectValid
                {
                    SchoolId = VALID_SCHOOL,
                    StudentUSI = VALID_STUDENT,
                    UnknownParameter = 0
                };

        public RequestObjectValidViewMyStudents REQUEST_OBJECT_VALID_VIEW_MY_STUDENTS =
            new RequestObjectValidViewMyStudents
            {
                SchoolId = VALID_SCHOOL,
                StudentUSI = VALID_STUDENT,
                UnknownParameter = 0
            };

        public RequestObjectInvalid REQUEST_OBJECT_INVALID =
            new RequestObjectInvalid
            {
                SchoolId = VALID_SCHOOL,
                StudentUSI = VALID_STUDENT,
                UnknownParameter = 0
            };

        public CanNotIgnoreRestrictedPropertyRequestObjectInvalid IGNORE_RESTRICTED_REQUEST_OBJECT_INVALID =
            new CanNotIgnoreRestrictedPropertyRequestObjectInvalid
            {
                SchoolId = VALID_SCHOOL,
                StudentUSI = VALID_STUDENT,
                UnknownParameter = 0
            };

        
        public MetricRequestObjectValid METRIC_REQUEST_OBJECT_VALID =
            new MetricRequestObjectValid
                {
                    SchoolId = VALID_SCHOOL,
                    StudentUSI = VALID_STUDENT,
                    UnknownParameter = 0,
                    MetricVariantId = 1234
                };

        protected void LoginCurrentUserHasLocalEducationAgencyLevelClaims()

        {
            Thread.CurrentPrincipal =
                AuthorizationFixtureBase.CURRENT_USER_HAS_LOCALEDUCATIONAGENCY_LEVEL_CLAIMS_USER_INFO.ToClaimsPrincipal(ClaimValidatorFixturesBase.CURRENT_USER_HAS_LOCALEDUCATIONAGENCY_LEVEL_CLAIMS_NAME,
                                                                              new[] { "Superintendent" });
            foreach(var associatedOrg in AuthorizationFixtureBase.CURRENT_USER_HAS_LOCALEDUCATIONAGENCY_LEVEL_CLAIMS_USER_INFO.AssociatedOrganizations)
            {
                foreach(var claimType in associatedOrg.ClaimTypes)
                {
                    Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(claimType,
                                                                                                          associatedOrg.
                                                                                                              EducationOrganizationId))
                        .Repeat.Any().Return(true);
                }
            }
        }

        protected void CreateSecurityMocks(MockRepository mocks)
        {
            myAuthorizationInformationProvider = mocks.StrictMock<IAuthorizationInformationProvider>();
            _schoolIdNameService = mocks.StrictMock<IIdNameService>();
            currentUserClaimInterrogator = mocks.StrictMock<ICurrentUserClaimInterrogator>();
            mySecurityAssertionProvider = new SecurityAssertionProvider(myAuthorizationInformationProvider,currentUserClaimInterrogator);
            mySecurityAssertionProvider.SetSchoolIdNameService(_schoolIdNameService);
            SupportedClaimNamesProvider = mocks.StrictMock<ISupportedClaimNamesProvider>();
        }

        public void ExceptSchoolIdNameServiceGetAny()
        {
            // The IdNameService is invoke by the GetEducationOrganizationHierarchy, which is invoked
            // for each EducationOrganizationId: SchoolId, or LEAId.  Set up the expectations for all
            // permutations.
            //
            SetupResult.For(_schoolIdNameService.Get(null)).Constraints(
                new FuncConstraint<IdNameRequest>(r => r.SchoolId == VALID_SCHOOL))
                .Return(new IdNameModel { LocalEducationAgencyId = VALID_LOCALEDUCATIONAGENCY });

            SetupResult.For(_schoolIdNameService.Get(null)).Constraints(
                new FuncConstraint<IdNameRequest>(r => r.SchoolId == INVALID_SCHOOL))
                .Return(null);

            SetupResult.For(_schoolIdNameService.Get(null)).Constraints(
                new FuncConstraint<IdNameRequest>(r => r.SchoolId == VALID_LOCALEDUCATIONAGENCY))
                .Return(null);

            SetupResult.For(_schoolIdNameService.Get(null)).Constraints(
                new FuncConstraint<IdNameRequest>(r => r.SchoolId == INVALID_LOCALEDUCATIONAGENCY))
                .Return(null);

            // and the Default value of 0
            SetupResult.For(_schoolIdNameService.Get(null)).Constraints(
                new FuncConstraint<IdNameRequest>(r => r.SchoolId == DEFAULTED_LOCALEDUCATIONAGENCY))
                .Return(null);
        }

        public void ExpectGetSchoolStaffValidRepeatAny()
        {
            Expect.Call(myAuthorizationInformationProvider.GetSchoolStaffUSIs(VALID_SCHOOL))
                .Return(new List<long> { VALID_STAFF })
                .Repeat.Any();
        }

        public void ExpectGetTeacherStudentsValidRepeatAny()
        {
            Expect.Call(myAuthorizationInformationProvider.GetTeacherStudentUSIs(VALID_STAFF))
                .Return(new List<long> { VALID_STUDENT })
                .Repeat.Any();            
        }

        public void ExpectGetStaffCohortStudentUSIsValidRepeatAny()
        {
            Expect.Call(myAuthorizationInformationProvider.GetStaffCohortStudentUSIs(VALID_STAFF))
                .Return(new List<long> { VALID_STUDENT })
                .Repeat.Any();
        }

        public void ExpectGetSupportedClaimNamesProvider()
        {
            Expect.Call(SupportedClaimNamesProvider.GetSupportedClaimNames())
                .Return(GetSupportedClaimNames());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            CreateSecurityMocks(mocks);
            LoginCurrentUserHasLocalEducationAgencyLevelClaims();
            CreateClaimAuthorizations();
            SetRunTestCase();
            // Dependencies must be registered before creating the concrete service.
            //
            RegisterInheritClaimClasses();
            CreateConcreteService();
            ExpectGetSchoolStaffValidRepeatAny();
            ExpectGetTeacherStudentsValidRepeatAny();
            ExpectGetStaffCohortStudentUSIsValidRepeatAny();
            ExceptSchoolIdNameServiceGetAny();
            ExpectGetSupportedClaimNamesProvider();

        }

        protected override void ExecuteTest()
        {
        }

        protected virtual void SetRunTestCase()
        {
            RunTestCase = RunTestServiceCase.None;
        }

        protected virtual void CreateConcreteService()
        {
            // Only instantiate a single service to avoid any possible confustion
            // over which test scenario is being run.
            //
            switch (RunTestCase)
            {
                case RunTestServiceCase.InheritAllMy:
                    container.Register(
                        Component
                            .For<ITestServiceBase>()
                            .ImplementedBy<TestServiceInheritAllMy>()
                            .Interceptors(typeof(TestStageInterceptor)));
                    testService = container.Resolve<ITestServiceBase>();
                    break;

                case RunTestServiceCase.OverrideAllMy:
                    container.Register(
                        Component
                            .For<ITestServiceBase>()
                            .ImplementedBy<TestServiceOverrideAllMy>()
                            .Interceptors(typeof(TestStageInterceptor)));
                    testService = container.Resolve<ITestServiceBase>();
                    break;

                case RunTestServiceCase.OverrideAll:
                    container.Register(
                        Component
                            .For<ITestServiceBase>()
                            .ImplementedBy<TestServiceOverrideAll>()
                            .Interceptors(typeof(TestStageInterceptor)));
                    testService = container.Resolve<ITestServiceBase>();
                    break;

                case RunTestServiceCase.OverrideMy:
                    container.Register(
                        Component
                            .For<ITestServiceBase>()
                            .ImplementedBy<TestServiceOverrideMy>()
                            .Interceptors(typeof(TestStageInterceptor)));
                    testService = container.Resolve<ITestServiceBase>();
                    break;

                default:
                    throw new UserAccessDeniedException( "Invalid RunTestCase: " + RunTestCase.ToString());
            }
            testService = container.Resolve<ITestServiceBase>();
        }

        public void Validate_LocalEducationAgency(int id)
        {
            exceptionThrown = null;

            try
            {
                testService.Validate_LocalEducationAgency(id);
            }
            catch (Exception ex)
            {
                exceptionThrown = ex;
            }
        }

        public void Validate_SchoolStaff(int school, long staff)
        {
            exceptionThrown = null;

            try
            {
                testService.Validate_SchoolStaff(school, staff);
            }
            catch (Exception ex)
            {
                exceptionThrown = ex;
            }
        }

        public void Validate_StudentInformationRequest(InformationRequest requestObject)
        {
            exceptionThrown = null;

            try
            {
                testService.Validate_StudentInformationRequest(requestObject);
            }
            catch (Exception ex)
            {
                exceptionThrown = ex;
            }
        }

        public void Validate_CheckParameterMarkedIgnoreValid(long ignore)
        {
            exceptionThrown = null;

            try
            {
                testService.CheckParameterMarkedIgnoreValid(ignore);
            }
            catch (Exception ex)
            {
                exceptionThrown = ex;
            }
        }

        public void Validate_CheckParameterMarkedIgnoreInvalid(long studentUSI)
        {
            exceptionThrown = null;

            try
            {
                testService.CheckParameterMarkedIgnoreInvalid(studentUSI);
            }
            catch (Exception ex)
            {
                exceptionThrown = ex;
            }
        }

        public void Validate_StudentOverviewRequest(OverviewRequest requestObject)
        {
            exceptionThrown = null;

            try
            {
                testService.Validate_StudentOverviewRequest(requestObject);
            }
            catch (Exception ex)
            {
                exceptionThrown = ex;
            }
        }

        public void Validate_RequestObjectValid(RequestObjectValid requestObject)
        {
            exceptionThrown = null;

            try
            {
                testService.Validate_RequestObjectValid(requestObject);
            }
            catch (Exception ex)
            {
                exceptionThrown = ex;
            }
        }
        
        public void Validate_RequestObjectValidViewMyStudents(RequestObjectValidViewMyStudents requestObject)
        {
            exceptionThrown = null;

            try
            {
                testService.Validate_RequestObjectValidViewMyStudents(requestObject);
            }
            catch (Exception ex)
            {
                exceptionThrown = ex;
            }
        }

        public void Validate_RequestObjectInvalid(RequestObjectInvalid requestObject)
        {
            exceptionThrown = null;

            try
            {
                testService.Validate_RequestObjectInvalid(requestObject);
            }
            catch (Exception ex)
            {
                exceptionThrown = ex;
            }
        }

        public void ExpectValidationPass()
        {
            Assert.That(exceptionThrown, Is.Null);
        }

        public void ExpectSecurityAccessDeniedException()
        {
            Assert.That(exceptionThrown, Is.InstanceOf(typeof(SecurityAccessDeniedException)));
        }

        public void ExpectInnerException(Type exceptionType)
        {
            Assert.That(exceptionThrown.InnerException, Is.InstanceOf(exceptionType));
        }

    }

    /**
     * LEABaseInheritAllMy: ViewAllStudents, ViewMyStudents
     * LEABaseOverrideAllMy: ViewAllStudents, ViewMyStudents
     * LEABaseOverrideMy: ViewMyStudents
     * LEABaseOverrideAll: ViewAllStudents
     * 
     * ViewAllStudentsClaimAuthorization: Has Validator: ViewAllStudentsClaimValidatorLocalEducationAgency
     * ViewMyStudentsClaimAuthorization: Has Validator: ViewMyStudentsClaimValidatorSchoolStaff
     * 
     * Scenarios:
     * LEA_VALID
     * LEA_INVALID:
     * SCHOOL_STAFF_VALID
     * SCHOOL_STAFF_SCHOOL_INVALID
     * */

    [TestFixture]
    public class WhenApplyViewPermissionByClaimInheritAllMyWithOrganizationalHierarchyClaimClaimCheckTrue : ApplyViewPermissionsByClaimFixtureBase
    {
        protected override void SetRunTestCase()
        {
            RunTestCase = RunTestServiceCase.InheritAllMy;
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewMyStudents, 1)).IgnoreArguments().Repeat.Any()
                .Return(true);
        }

        [Test]
        public void WithValidLeaShouldPass()
        {
            Validate_LocalEducationAgency(VALID_LOCALEDUCATIONAGENCY);
            ExpectValidationPass();
        }

        [Test]
        public void WithValidSchoolStaffShouldPass()
        {
            Validate_SchoolStaff(VALID_SCHOOL, VALID_STAFF);
            ExpectValidationPass();
        }

        [Test]
        public void WithInvalidSchoolStaffShouldFail()
        {
            Validate_SchoolStaff(VALID_SCHOOL, INVALID_STAFF);
            ExpectSecurityAccessDeniedException();
        }

        [Test]
        public void WithValidSORShouldPass()
        {
            Validate_StudentOverviewRequest(VALID_SOR);
            ExpectValidationPass();
        }

        [Test]
        public void With_INVALID_SOR_INVALID_STUDENT_ShouldFail()
        {
            Validate_StudentOverviewRequest(INVALID_SOR_INVALID_STUDENT);
            ExpectSecurityAccessDeniedException();
        }

        [Test]
        public void WithValidSIRShouldPass()
        {
            Validate_StudentInformationRequest(VALID_SIR);
            ExpectValidationPass();
        }

        [Test]
        public void With_INVALID_SIR_LOCALEDUCATIONAGENCY_INVALID_ShouldFail()
        {
            Validate_StudentInformationRequest(INVALID_SIR_LOCALEDUCATIONAGENCY_INVALID);
            ExpectSecurityAccessDeniedException();
        }

        [Test]
        public void With_INVALID_SIR_SCHOOL_INVALID_ShouldFail()
        {
            Validate_StudentInformationRequest(INVALID_SIR_SCHOOL_INVALID);
            ExpectSecurityAccessDeniedException();
        }

        [Test]
        public void With_INVALID_SIR_STUDENT_INVALID_ShouldFail()
        {
            Validate_StudentInformationRequest(INVALID_SIR_STUDENT_INVALID);
            ExpectSecurityAccessDeniedException();
        }

        [Test]
        public void With_CheckParameterMarkedIgnoreValid_ShouldPass()
        {
            Validate_CheckParameterMarkedIgnoreValid(VALID_STUDENT);
            ExpectValidationPass();
        }

        [Test]
        public void With_CheckParameterMarkedIgnoreInvalid_ShouldFail()
        {
            Validate_CheckParameterMarkedIgnoreInvalid(VALID_STUDENT);
            ExpectSecurityAccessDeniedException();
        }

        [Test]
        // This tests [AuthIgnore] on an unhandled property.
        public void With_Validate_RequestObjectValid_ShouldPass()
        {
            Validate_RequestObjectValid(REQUEST_OBJECT_VALID);
            ExpectValidationPass();
        }

        [Test]
        public void WithValidate_RequestObjectValidViewMyStudents_ShouldPass()
        {
            Validate_RequestObjectValidViewMyStudents(REQUEST_OBJECT_VALID_VIEW_MY_STUDENTS);
            ExpectValidationPass();
        }

        [Test]
        // This tests an unhandled property.
        public void With_Validate_RequestObjectInvalid_ShouldFail()
        {
            Validate_RequestObjectInvalid(REQUEST_OBJECT_INVALID);
            ExpectSecurityAccessDeniedException();
            ExpectInnerException(typeof (UnhandledSignatureException));
        }

        [Test]
        // This tests [AuthIgnore] on an restricted property: StudentUSI.
        public void With_Validate_CanNotIgnoreRestrictedPropertyRequestObjectInvalid_ShouldFail()
        {
            Validate_RequestObjectInvalid(IGNORE_RESTRICTED_REQUEST_OBJECT_INVALID);
            ExpectSecurityAccessDeniedException();
            ExpectInnerException(typeof(ArgumentException));
        }

        [Test]
        // Test use of a PredefinedSafe property.
        public void With_Validate_MetricRequestObjectValid_ShouldPass()
        {
            Validate_RequestObjectValid(METRIC_REQUEST_OBJECT_VALID);
            ExpectValidationPass();
        }

        [Test]
        // Test for a null request object.  The property values of the request object will be 
        // set to the default value of the property type.  This will result in an access denied
        // as the default values are not valid.
        public void With_NullRequestObjectValid_ShouldFail()
        {
            Validate_RequestObjectValid(null);
            ExpectSecurityAccessDeniedException();
            Assert.That(exceptionThrown.InnerException, Is.InstanceOf(typeof(UserAccessDeniedException)) | Is.InstanceOf(typeof(ArgumentException)));
        }
    }

    [TestFixture]
    public class WhenApplyViewPermissionByClaimInheritAllMyWithOrganizationalHierarchyClaimClaimCheckFalse : ApplyViewPermissionsByClaimFixtureBase
    {
        protected override void SetRunTestCase()
        {
            RunTestCase = RunTestServiceCase.InheritAllMy;
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewMyStudents, 1)).IgnoreArguments().Repeat.Any()
                .Return(false);
        }

        [Test]
        public void WithInvalidLeaShouldFail()
        {
            Validate_LocalEducationAgency(INVALID_LOCALEDUCATIONAGENCY);
            ExpectSecurityAccessDeniedException();
        }

        [Test]
        public void With_INVALID_SOR_INVALID_SCHOOL_ShouldFail()
        {
            Validate_StudentOverviewRequest(INVALID_SOR_INVALID_SCHOOL);
            ExpectSecurityAccessDeniedException();
        }
    }

    [TestFixture]
    public class WhenApplyViewPermissionByClaimOverrideAllMyWithOrganizationalHierarchyClaimClaimCheckTrue : ApplyViewPermissionsByClaimFixtureBase
    {
        protected override void SetRunTestCase()
        {
            RunTestCase = RunTestServiceCase.OverrideAllMy;
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewMyStudents, 1)).IgnoreArguments().Repeat.Any()
                .Return(true);
        }

        [Test]
        public void WithValidLeaShouldPass()
        {
            Validate_LocalEducationAgency(VALID_LOCALEDUCATIONAGENCY);
            ExpectValidationPass();
        }

        [Test]
        public void WithValidSchoolStaffShouldPass()
        {
            Validate_SchoolStaff(VALID_SCHOOL, VALID_STAFF);
            ExpectValidationPass();
        }

        [Test]
        public void WithInvalidSchoolStaffShouldFail()
        {
            Validate_SchoolStaff(VALID_SCHOOL, INVALID_STAFF);
            ExpectSecurityAccessDeniedException();
        }
    }

    [TestFixture]
    public class WhenApplyViewPermissionByClaimOverrideAllMyWithOrganizationalHierarchyClaimClaimCheckFalse : ApplyViewPermissionsByClaimFixtureBase
    {
        protected override void SetRunTestCase()
        {
            RunTestCase = RunTestServiceCase.OverrideAllMy;
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewMyStudents, 1)).IgnoreArguments().Repeat.Any()
                .Return(false);
        }

        [Test]
        public void WithValidLeaShouldPass()
        {
            Validate_LocalEducationAgency(VALID_LOCALEDUCATIONAGENCY);
            ExpectValidationPass();
        }

        [Test]
        public void WithInvalidLeaShouldFail()
        {
            Validate_LocalEducationAgency(INVALID_LOCALEDUCATIONAGENCY);
            ExpectSecurityAccessDeniedException();
        }

        [Test]
        public void WithInvalidSchoolStaffShouldFail()
        {
            Validate_SchoolStaff(VALID_SCHOOL, INVALID_STAFF);
            ExpectSecurityAccessDeniedException();
        }
    }

    [TestFixture]
    public class WhenApplyViewPermissionByClaimOverrideAll : ApplyViewPermissionsByClaimFixtureBase
    {
        protected override void SetRunTestCase()
        {
            RunTestCase = RunTestServiceCase.OverrideAll;
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewMyStudents, 1)).IgnoreArguments().Repeat.Any()
                .Return(false);
        }

        [Test]
        public void WithValidLeaShouldPass()
        {
            Validate_LocalEducationAgency(VALID_LOCALEDUCATIONAGENCY);
            ExpectValidationPass();
        }

        [Test]
        public void WithInvalidLeaShouldFail()
        {
            Validate_LocalEducationAgency(INVALID_LOCALEDUCATIONAGENCY);
            ExpectSecurityAccessDeniedException();
        }

        [Test]
        public void WithValidSchoolStaffShouldFail()
        {
            Validate_SchoolStaff(VALID_SCHOOL, VALID_STAFF);
            ExpectSecurityAccessDeniedException();
        }

        [Test]
        public void WithInvalidSchoolStaffShouldFail()
        {
            Validate_SchoolStaff(VALID_SCHOOL, INVALID_STAFF);
            ExpectSecurityAccessDeniedException();
        }
    }

    [TestFixture]
    public class WhenApplyViewPermissionByClaimOverrideMy : ApplyViewPermissionsByClaimFixtureBase
    {
        protected override void SetRunTestCase()
        {
            RunTestCase = RunTestServiceCase.OverrideMy;
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewMyStudents, 1)).IgnoreArguments().Repeat.Any()
                .Return(true);
        }

        [Test]
        public void WithValidLeaShouldFail()
        {
            Validate_LocalEducationAgency(VALID_LOCALEDUCATIONAGENCY);
            ExpectSecurityAccessDeniedException();
        }

        [Test]
        public void WithInvalidLeaShouldFail()
        {
            Validate_LocalEducationAgency(INVALID_LOCALEDUCATIONAGENCY);
            ExpectSecurityAccessDeniedException();
        }

        [Test]
        public void WithValidSchoolStaffShouldPass()
        {
            Validate_SchoolStaff(VALID_SCHOOL, VALID_STAFF);
            ExpectValidationPass();
        }

        [Test]
        public void WithInvalidSchoolStaffShouldFail()
        {
            Validate_SchoolStaff(VALID_SCHOOL, INVALID_STAFF);
            ExpectSecurityAccessDeniedException();
        }
    }

}
