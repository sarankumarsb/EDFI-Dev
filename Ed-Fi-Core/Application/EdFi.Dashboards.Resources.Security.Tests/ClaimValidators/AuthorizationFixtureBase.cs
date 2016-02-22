// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using EdFi.Dashboards.Common;
using System.IO;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Security.ClaimValidators;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Security.Implementations;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Security.Tests.ClaimValidators
{
    public abstract class AuthorizationFixtureBase : ClaimValidatorFixturesBase
    {

        #region readonly User Information 
        public static readonly UserInformation.LocalEducationAgency CURRENT_USER_HAS_LOCALEDUCATIONAGENCY_LEVEL_CLAIMS_ON_LOCALEDUCATIONAGENCY_01 =
            new UserInformation.LocalEducationAgency(LOCALEDUCATIONAGENCY_ID_01)
                {
                    Name = LOCALEDUCATIONAGENCY_NAME_01,
                    ClaimTypes = new List<String>
                                     {
                                         EdFiClaimTypes.AccessOrganization,
                                         EdFiClaimTypes.AdministerDashboard,
                                         EdFiClaimTypes.ManageGoals,
                                         EdFiClaimTypes.ViewAllMetrics,
                                         EdFiClaimTypes.ViewAllStudents,
                                         EdFiClaimTypes.ViewMyMetrics,
                                         EdFiClaimTypes.ViewMyStudents,
                                         EdFiClaimTypes.ViewOperationalDashboard,
                                         EdFiClaimTypes.ViewAllTeachers
                                     }
                };

        public static readonly UserInformation.LocalEducationAgency CURRENT_USER_HAS_LOCALEDUCATIONAGENCY_LEVEL_CLAIMS_ON_LOCALEDUCATIONAGENCY_02 =
            new UserInformation.LocalEducationAgency(LOCALEDUCATIONAGENCY_ID_02)
                {
                    Name = LOCALEDUCATIONAGENCY_NAME_02,
                    ClaimTypes = new List<String>
                                     {
                                         EdFiClaimTypes.AccessOrganization,
                                         EdFiClaimTypes.AdministerDashboard,
                                         EdFiClaimTypes.ManageGoals,
                                         EdFiClaimTypes.ViewAllMetrics,
                                         EdFiClaimTypes.ViewAllStudents,
                                         EdFiClaimTypes.ViewMyMetrics,
                                         EdFiClaimTypes.ViewMyStudents,
                                         EdFiClaimTypes.ViewOperationalDashboard,
                                         EdFiClaimTypes.ViewAllTeachers
                                     }
                };

        public static readonly UserInformation.School CURRENT_USER_HAS_SCHOOL_LEVEL_CLAIMS_ON_SCHOOL_01_01 =
            new UserInformation.School(LOCALEDUCATIONAGENCY_ID_01, SCH_ID_01_01)
                {
                    Name = SCH_NAME_01_01,
                    ClaimTypes = new List<String>
                                     {
                                         EdFiClaimTypes.AccessOrganization,
                                         EdFiClaimTypes.AdministerDashboard,
                                         EdFiClaimTypes.ManageGoals,
                                         EdFiClaimTypes.ViewAllMetrics,
                                         EdFiClaimTypes.ViewAllStudents,
                                         EdFiClaimTypes.ViewMyMetrics,
                                         EdFiClaimTypes.ViewMyStudents,
                                         EdFiClaimTypes.ViewOperationalDashboard,
                                         EdFiClaimTypes.ViewAllTeachers
                                     }
                };

        public static readonly UserInformation.School CURRENT_USER_HAS_SCHOOL_LEVEL_CLAIMS_ON_SCHOOL_01_02 =
            new UserInformation.School(LOCALEDUCATIONAGENCY_ID_01, SCH_ID_01_02)
                {
                    Name = SCH_NAME_01_02,
                    ClaimTypes = new List<String>
                                     {
                                         EdFiClaimTypes.AccessOrganization,
                                         EdFiClaimTypes.AdministerDashboard,
                                         EdFiClaimTypes.ManageGoals,
                                         EdFiClaimTypes.ViewAllMetrics,
                                         EdFiClaimTypes.ViewAllStudents,
                                         EdFiClaimTypes.ViewMyMetrics,
                                         EdFiClaimTypes.ViewMyStudents,
                                         EdFiClaimTypes.ViewOperationalDashboard,
                                         EdFiClaimTypes.ViewAllTeachers
                                     }
                };

        public static readonly UserInformation.School CURRENT_USER_HAS_NO_CLAIMS_ON_SCHOOL_01_01 =
            new UserInformation.School(LOCALEDUCATIONAGENCY_ID_01, SCH_ID_01_01)
                {
                    Name = SCH_NAME_01_01,
                    ClaimTypes = new List<String>()
                };

        public static readonly UserInformation.School CURRENT_USER_HAS_NO_CLAIMS_ON_SCHOOL_01_02 =
            new UserInformation.School(LOCALEDUCATIONAGENCY_ID_01, SCH_ID_01_02)
                {
                    Name = SCH_NAME_01_02,
                    ClaimTypes = new List<String>()
                };

        public static readonly UserInformation CURRENT_USER_HAS_LOCALEDUCATIONAGENCY_LEVEL_CLAIMS_USER_INFO =
            new UserInformation
                {
                    StaffUSI = CURRENT_USER_HAS_LOCALEDUCATIONAGENCY_LEVEL_CLAIMS_USI,
                    FullName = CURRENT_USER_HAS_LOCALEDUCATIONAGENCY_LEVEL_CLAIMS_NAME,
                    AssociatedOrganizations = new List<UserInformation.EducationOrganization>
                                                  {
                                                      CURRENT_USER_HAS_LOCALEDUCATIONAGENCY_LEVEL_CLAIMS_ON_LOCALEDUCATIONAGENCY_01,
                                                      CURRENT_USER_HAS_LOCALEDUCATIONAGENCY_LEVEL_CLAIMS_ON_LOCALEDUCATIONAGENCY_02
                                                  },
                };

        public static readonly UserInformation CURRENT_USER_HAS_SCHOOL_LEVEL_CLAIMS_USER_INFO =
            new UserInformation
                {
                    StaffUSI = CURRENT_USER_HAS_SCHOOL_LEVEL_CLAIMS_USI,
                    FullName = CURRENT_USER_HAS_SCHOOL_LEVEL_CLAIMS_NAME,
                    AssociatedOrganizations = new List<UserInformation.EducationOrganization>
                                                  {
                                                      CURRENT_USER_HAS_SCHOOL_LEVEL_CLAIMS_ON_SCHOOL_01_01,
                                                      CURRENT_USER_HAS_SCHOOL_LEVEL_CLAIMS_ON_SCHOOL_01_02
                                                  },
                };

        public static readonly UserInformation CURRENT_USER_HAS_NO_CLAIMS_USER_INFO =
            new UserInformation
                {
                    StaffUSI = CURRENT_USER_HAS_NO_CLAIMS_USI,
                    FullName = CURRENT_USER_HAS_NO_CLAIMS_NAME,
                    AssociatedOrganizations = new List<UserInformation.EducationOrganization>
                                                  {
                                                      CURRENT_USER_HAS_NO_CLAIMS_ON_SCHOOL_01_01,
                                                      CURRENT_USER_HAS_NO_CLAIMS_ON_SCHOOL_01_02,
                                                  },
                };
#endregion

        protected ClaimValidatorBase myClaimValidator;
        protected IAuthorizationDelegate myAuthorizationDelegate;
        protected SecurityAssertionProvider mySecurityAssertionProvider;
        protected IAuthorizationInformationProvider myAuthorizationInformationProvider;
        protected IIdNameService _schoolIdNameService;
        protected ICurrentUserClaimInterrogator currentUserClaimInterrogator;
        protected Exception myException;


        public ClaimValidatorRequest GetClaimValidatorRequest()
        {
            var result = new ClaimValidatorRequest {Parameters = GetSuppliedParameters()};
            return result;
        }

        protected void LoginCurrentUserHasLocalEducationAgencyLevelClaims()
        {
            Thread.CurrentPrincipal =
                CURRENT_USER_HAS_LOCALEDUCATIONAGENCY_LEVEL_CLAIMS_USER_INFO.ToClaimsPrincipal(CURRENT_USER_HAS_LOCALEDUCATIONAGENCY_LEVEL_CLAIMS_NAME,
                                                                              new[] {"Superintendent"});
        }

        protected void LoginCurrentUserHasSchoolLevelClaims()
        {
            Thread.CurrentPrincipal =
                CURRENT_USER_HAS_SCHOOL_LEVEL_CLAIMS_USER_INFO.ToClaimsPrincipal(
                    CURRENT_USER_HAS_SCHOOL_LEVEL_CLAIMS_NAME, new[] { "Superintendent" });
        }

        protected void LoginCurrentUserHasNoClaims()
        {
            Thread.CurrentPrincipal =
                CURRENT_USER_HAS_NO_CLAIMS_USER_INFO.ToClaimsPrincipal(CURRENT_USER_HAS_NO_CLAIMS_NAME,
                                                                       new[] { "Superintendent" });
        }

        protected abstract ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider);

        // The name of the class should contain the name of the validator.
        // This is a simple test to ensure that there is a match.
        protected void CheckValidatorName()
        {
            var typeName = GetType().Name;
            var validator = CreateValidator(mySecurityAssertionProvider);

            if (null == validator) return;

            var validatorName = validator.GetType().Name.Replace("Validator", "");

            if (!typeName.Contains(validatorName)) throw new ArgumentException( typeName + " !=" + validatorName);
        }

        protected override void EstablishContext()
        {
            CheckValidatorName();
            CreateSecurityMocks(mocks);
        }

        protected override void ExecuteTest()
        {
            throw new NotImplementedException();
        }

        protected virtual IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield break;
        }

        // Do not call this method directly
        private ParameterInstance GetParameterInstance(object value)
        {
            var trace = new StackTrace();
            var parmInfo = trace.GetFrame(1).GetMethod().GetParameters()[0];

            return new ParameterInstance
                       {
                           Name = parmInfo.Name,
                           ParameterInfo = parmInfo,
                           Value = value,
                       };
        }

        protected ParameterInstance GetFilterParameter(string filter)
        {
            return GetParameterInstance(filter);
        }

        protected ParameterInstance GetTextToFindParameter(string textToFind)
        {
            return GetParameterInstance(textToFind);
        }

        protected ParameterInstance GetRowCountToReturnParameter(int rowCountToReturn)
        {
            return GetParameterInstance(rowCountToReturn);
        }

        protected ParameterInstance GetMatchContainsParameter(bool matchContains)
        {
            return GetParameterInstance(matchContains);
        }

        protected ParameterInstance GetLocalEducationAgencyIdNullableParameter(int? localEducationAgencyId)
        {
            return GetParameterInstance(localEducationAgencyId);
        }

        protected ParameterInstance GetSchoolIdNullableParameter(int? schoolId)
        {
            return GetParameterInstance(schoolId);
        }

        protected ParameterInstance GetStaffUSINullableParameter(long? staffUSI)
        {
            return GetParameterInstance(staffUSI);
        }

        protected ParameterInstance GetLocalEducationAgencyIdParameter(int localEducationAgencyId)
        {
            return GetParameterInstance(localEducationAgencyId);
        }

        protected ParameterInstance GetSchoolIdParameter(int schoolId)
        {
            return GetParameterInstance(schoolId);
        }

        protected ParameterInstance GetSchoolIdsParameter(int[] schoolIds)
        {
            return GetParameterInstance(schoolIds);
        }

        protected ParameterInstance GetSectionIdParameter(long teacherSectionId)
        {
            return GetParameterInstance(teacherSectionId);
        }

        protected ParameterInstance GetCohortIdParameter(long staffCohortId)
        {
            return GetParameterInstance(staffCohortId);
        }

        protected ParameterInstance GetSectionOrCohortIdParameter(long sectionOrCohortId)
        {
            return GetParameterInstance(sectionOrCohortId);
        }

        protected ParameterInstance GetStudentUSIParameter(long studentUSI)
        {
            return GetParameterInstance(studentUSI);
        }

        protected ParameterInstance GetStudentUSIsParameter(long[] studentUSIs)
        {
            return GetParameterInstance(studentUSIs);
        }

        protected ParameterInstance GetStaffUSIParameter(long staffUSI)
        {
            return GetParameterInstance(staffUSI);
        }

        protected ParameterInstance GetSchoolSpecialistIdParameter(long staffUSI)
        {
            return GetParameterInstance(staffUSI);
        }
        
        protected ParameterInstance GetStudentListTypeParameter(string studentListType)
        {
            return GetParameterInstance(studentListType);
        }

        protected ParameterInstance GetCustomStudentListIdNullableParameter(int? customStudentListId)
        {
            return GetParameterInstance(customStudentListId);
        }

        protected ParameterInstance GetMetricIdParameter(int metricId)
        {
            return GetParameterInstance(metricId);
        }

        protected ParameterInstance GetClaimSetParameter(string claimSet)
        {
            return GetParameterInstance(claimSet);
        }

        protected ParameterInstance GetClaimSetMapsParameter(IDictionary<string, string> claimSetMaps)
        {
            return GetParameterInstance(claimSetMaps);
        }

        protected ParameterInstance GetCurrentOperationParameter(string currentOperation)
        {
            return GetParameterInstance(currentOperation);
        }

        protected ParameterInstance GetEdOrgPositionTitlesParameter(IDictionary<string, string> edOrgPositionTitles)
        {
            return GetParameterInstance(edOrgPositionTitles);
        }

        protected ParameterInstance GetFileInputStreamParameter(Stream fileInputStream)
        {
            return GetParameterInstance(fileInputStream);
        }

        protected ParameterInstance GetFileNameParameter(string fileName)
        {
            return GetParameterInstance(fileName);
        }

        protected ParameterInstance GetIsPostParameter(bool isPost)
        {
            return GetParameterInstance(isPost);
        }

        protected ParameterInstance GetIsSuccessParameter(bool isSuccess)
        {
            return GetParameterInstance(isSuccess);
        }

        protected ParameterInstance GetLinksParameter(IEnumerable<Link> links)
        {
            return GetParameterInstance(links);
        }

        protected ParameterInstance GetMessagesParameter(IList<string> messages)
        {
            return GetParameterInstance(messages);
        }

        protected ParameterInstance GetPositionTitleParameter(string positionTitle)
        {
            return GetParameterInstance(positionTitle);
        }

        protected ParameterInstance GetPossibleClaimSetsParameter(IDictionary<string, string> possibleClaimSets)
        {
            return GetParameterInstance(possibleClaimSets);
        }

        protected ParameterInstance GetResourceUrlParameter(string resourceUrl)
        {
            return GetParameterInstance(resourceUrl);
        }

        protected ParameterInstance GetUrlParameter(string url)
        {
            return GetParameterInstance(url);
        }

        public void ExpectValidationPass()
        {
            Assert.That(myException, Is.Null);
        }

        public void ExpectValidationFail(Type type)
        {
            Assert.That(myException, Is.InstanceOf(type));
        }

        public void ExpectException(Type type, string message)
        {
            Assert.That(myException, Is.InstanceOf(type));
            Assert.That(myException.Message, Is.EqualTo(message));
        }

        public void ExpectValidationFailNoCohortPermission()
        {
            ExpectException(typeof(UserAccessDeniedException),
                            SecurityAssertionProvider.NoCohortPermissionErrorMessage);
        }
        
        public void ExpectValidationFailNoCustomStudentListPermission()
        {
            ExpectException(typeof(UserAccessDeniedException),
                            SecurityAssertionProvider.NoCustomStudentListPermissionErrorMessage);
        }

        public void ExpectValidationFailNoEducationOrganizationPermission()
        {
            ExpectException(typeof (UserAccessDeniedException),
                            SecurityAssertionProvider.NoEducationOrganizationPermissionErrorMessage);
        }

        public void ExpectValidationFailNoStaffPermission()
        {
            ExpectException(typeof (UserAccessDeniedException),
                            SecurityAssertionProvider.NoStaffPermissionErrorMessage);
        }
        
        public void ExpectValidationFailNoStudentPermission()
        {
            ExpectException(typeof (UserAccessDeniedException),
                            SecurityAssertionProvider.NoStudentPermissionErrorMessage);
        }

        public void ExpectValidationFailNoSectionPermission()
        {
            ExpectException(typeof (UserAccessDeniedException),
                            SecurityAssertionProvider.NoSectionPermissionErrorMessage);
        }

        public void ExpectValidationFailNoSchoolPermission()
        {
            ExpectException(typeof(UserAccessDeniedException),
                            SecurityAssertionProvider.NoSchoolPermissionErrorMessage);
        }

        public void ExpectValidationFailInvalidParameter()
        {
            ExpectException(typeof(UserAccessDeniedException),
                            ParameterAuthorizationBase.InvalidParameterErrorMessage);
        }

        public void ExpectValidationFailInvalidId()
        {
            ExpectException(typeof(ArgumentException),
                            "Unable to validate claims against an uninitialized LocalEducationAgencyId value.");
        }

        protected void CreateSecurityMocks(MockRepository mocks)
        {
            myAuthorizationInformationProvider = mocks.StrictMock<IAuthorizationInformationProvider>();
            _schoolIdNameService = mocks.StrictMock<IIdNameService>();
            if (currentUserClaimInterrogator == null)
            {
                currentUserClaimInterrogator = mocks.StrictMock<ICurrentUserClaimInterrogator>();
        }
            mySecurityAssertionProvider = new SecurityAssertionProvider(myAuthorizationInformationProvider,currentUserClaimInterrogator);
            mySecurityAssertionProvider.SetSchoolIdNameService(_schoolIdNameService);
        }

        protected void ExpectGetSchoolIdName()
        {
            Expect.Call(_schoolIdNameService.Get(new IdNameRequest { SchoolId = SCH_ID_01_01 })).IgnoreArguments()
                .Return(new IdNameModel { LocalEducationAgencyId = LOCALEDUCATIONAGENCY_ID_01 });
        }

        public void ExpectGetLocalEducationOrganizationHierarchyAny()
        {
            SetupResult.For(currentUserClaimInterrogator.GetEducationOrganizationHierarchy(0)).Constraints(
                new FuncConstraint<int>(r => r == LOCALEDUCATIONAGENCY_ID_00))
                .Return(new List<int>{LOCALEDUCATIONAGENCY_ID_00});

            SetupResult.For(currentUserClaimInterrogator.GetEducationOrganizationHierarchy(0)).Constraints(
                new FuncConstraint<int>(r => r == LOCALEDUCATIONAGENCY_ID_01))
                .Return(new List<int>{LOCALEDUCATIONAGENCY_ID_01});

            SetupResult.For(currentUserClaimInterrogator.GetEducationOrganizationHierarchy(0)).Constraints(
                new FuncConstraint<int>(r => r == LOCALEDUCATIONAGENCY_ID_02))
                .Return(new List<int>{LOCALEDUCATIONAGENCY_ID_02});

            // The IdNameService is invoke by the GetEducationOrganizationHierarchy, which is invoked
            // for each EducationOrganizationId: SchoolId, or LEAId.  Set up the expectations for all
            // permutations.
            //
            SetupResult.For(currentUserClaimInterrogator.GetEducationOrganizationHierarchy(0)).Constraints(
                new FuncConstraint<int>(r => r == SCH_ID_00_00))
                .Return(new List<int>{SCH_ID_00_00, LOCALEDUCATIONAGENCY_ID_00});

            SetupResult.For(currentUserClaimInterrogator.GetEducationOrganizationHierarchy(0)).Constraints(
                new FuncConstraint<int>(r => r == SCH_ID_00_01))
                .Return(new List<int>{ SCH_ID_00_01, LOCALEDUCATIONAGENCY_ID_00 });

            SetupResult.For(currentUserClaimInterrogator.GetEducationOrganizationHierarchy(0)).Constraints(
                new FuncConstraint<int>(r => r == SCH_ID_00_02))
                .Return(new List<int>{ SCH_ID_00_02, LOCALEDUCATIONAGENCY_ID_00 });

            SetupResult.For(currentUserClaimInterrogator.GetEducationOrganizationHierarchy(0)).Constraints(
                new FuncConstraint<int>(r => r == SCH_ID_01_01))
                .Return(new List<int> { SCH_ID_01_01, LOCALEDUCATIONAGENCY_ID_01 });

            SetupResult.For(currentUserClaimInterrogator.GetEducationOrganizationHierarchy(0)).Constraints(
                new FuncConstraint<int>(r => r == SCH_ID_01_02))
                .Return(new List<int>{ SCH_ID_01_02, LOCALEDUCATIONAGENCY_ID_01 });

            SetupResult.For(currentUserClaimInterrogator.GetEducationOrganizationHierarchy(0)).Constraints(
                new FuncConstraint<int>(r => r == SCH_ID_02_01))
                .Return(new List<int>{ SCH_ID_02_01, LOCALEDUCATIONAGENCY_ID_02 });

            SetupResult.For(currentUserClaimInterrogator.GetEducationOrganizationHierarchy(0)).Constraints(
                new FuncConstraint<int>(r => r == SCH_ID_02_02))
                .Return(new List<int>{ SCH_ID_02_02, LOCALEDUCATIONAGENCY_ID_02 });
        }

        public void ExpectGetAssociatedEducationOrganizationsTeacherNone()
        {
            Expect.Call(myAuthorizationInformationProvider.GetAssociatedEducationOrganizations(TEACHER_USI_00)).Return(
                new List<int>());
        }
        
        /// <summary>
        /// The Teacher is associated with all the schools of LocalEducationAgency 01
        /// </summary>
        public void ExpectGetAssociatedEducationOrganizationsValidValid(string claimType)
        {
            Expect.Call(myAuthorizationInformationProvider.GetAssociatedEducationOrganizations(TEACHER_USI_01)).Return(
                new List<int> {SCH_ID_01_01, SCH_ID_01_02});
            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(claimType, SCH_ID_01_01)).Return(true);
        }

        public void ExpectGetAssociatedEducationOrganizationsValidValidLea(string claimType)
        {
            Expect.Call(myAuthorizationInformationProvider.GetAssociatedEducationOrganizations(TEACHER_USI_01)).Return(
                new List<int> { LOCALEDUCATIONAGENCY_ID_01, SCH_ID_01_01 });
            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(claimType, LOCALEDUCATIONAGENCY_ID_01)).Return(true).Repeat.Any();
        }

        public void ExpectHasClaimWithinEducationOrganizationHierarchyValid(int educationOrganizationId, string claimType)
        {
            Expect.Call(
                currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                claimType, educationOrganizationId)).Return(true).Repeat.Any();
        }

        public void ExpectGetAssociatedEducationOrganizationsValid(long staffUSI)
        {
            Expect.Call(myAuthorizationInformationProvider.GetAssociatedEducationOrganizations(staffUSI)).Return(
                new List<int> { SCH_ID_01_01, SCH_ID_01_02 });
        }

        public void ExpectGetAssociatedEducationOrganizationsInvalidInvalid(string claimType)
        {
            Expect.Call(myAuthorizationInformationProvider.GetAssociatedEducationOrganizations(TEACHER_USI_01)).Return(
                new List<int> {SCH_ID_02_01, SCH_ID_02_02});
            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(claimType, SCH_ID_02_01)).Return(false);
            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(claimType, SCH_ID_02_02)).Return(false);
        }

        public void ExpectGetAssociatedEducationOrganizationsValidInvalid(string claimType)
        {
            Expect.Call(myAuthorizationInformationProvider.GetAssociatedEducationOrganizations(TEACHER_USI_01)).Return(
                new List<int> {SCH_ID_01_01, SCH_ID_02_01});
            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(claimType, SCH_ID_01_01)).Return(true);
        }

        public void ExpectGetAssociatedEducationOrganizationsInvalidValid(string claimType)
        {
            Expect.Call(myAuthorizationInformationProvider.GetAssociatedEducationOrganizations(TEACHER_USI_01)).Return(
                new List<int> {SCH_ID_02_02, SCH_ID_01_02});
            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(claimType, SCH_ID_02_02)).Return(false);
            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(claimType, SCH_ID_01_02)).Return(true);
        }

        public void ExpectGetStaffCohortsValid()
        {
            Expect.Call(myAuthorizationInformationProvider.GetStaffCohortIds(TEACHER_USI_01)).Return(
                new List<long> { COHORT_01_01, COHORT_01_02 });
        }
        
        public void ExpectGetStaffCustomStudentListsValid()
        {
            Expect.Call(myAuthorizationInformationProvider.GetStaffCustomStudentListIds(TEACHER_USI_01)).Return(
                new List<long> { CUSTOMSTUDENTLIST_01_01, CUSTOMSTUDENTLIST_01_02 });
        }

        public void ExpectGetStaffCustomMetricsBasedWatchListIds()
        {
            Expect.Call(myAuthorizationInformationProvider.GetStaffCustomMetricsBasedWatchListIds(TEACHER_USI_01)).Return(
                new List<long> { METRICSBASEDWATCHLIST_01_01, METRICSBASEDWATCHLIST_01_02 } );
        }

        public void ExpectGetEducationOrganizationCustomMetricsBasedWatchListIds(int educationOrganizationId)
        {
            Expect.Call(myAuthorizationInformationProvider.GetEducationOrganizationCustomMetricsBasedWatchListIds(educationOrganizationId)).Return(
                new List<long> { METRICSBASEDWATCHLIST_01_01, METRICSBASEDWATCHLIST_01_02 });
        }

        public void ExpectGetStaffCohortsInvalid()
        {
            Expect.Call(myAuthorizationInformationProvider.GetStaffCohortIds(TEACHER_USI_01)).Return(
                new List<long> { COHORT_00_01 });
        }

        public void ExpectGetStaffCustomStudentListsInvalid()
        {
            Expect.Call(myAuthorizationInformationProvider.GetStaffCustomStudentListIds(TEACHER_USI_01)).Return(
                new List<long> { CUSTOMSTUDENTLIST_00_01 });
        }
        
        public void ExpectGetEducationOrganizationCohortInvalid()
        {
            ExpectGetEducationOrganizationCohortInvalid(SCH_ID_01_01);
        }

        public void ExpectGetEducationOrganizationCohortInvalid(int educationOrganizationId)
        {
            Expect.Call(myAuthorizationInformationProvider.GetEducationOrganizationCohortIds(educationOrganizationId)).Return(
                new List<long>());
        }

        public void ExpectGetEducationOrganizationCustomStudentListInvalid()
        {
            ExpectGetEducationOrganizationCustomStudentListInvalid(SCH_ID_01_01);
        }

        public void ExpectGetEducationOrganizationCustomStudentListInvalid(int educationOrganizationId)
        {
            Expect.Call(myAuthorizationInformationProvider.GetEducationOrganizationCustomStudentListIds(educationOrganizationId)).Return(
                new List<long>());
        }

        public void ExpectGetEducationOrganizationCohortValid()
        {
            ExpectGetEducationOrganizationCohortValid(SCH_ID_01_01);
        }

        public void ExpectGetEducationOrganizationCohortValid(int educationOrganizationId)
        {
            Expect.Call(myAuthorizationInformationProvider.GetEducationOrganizationCohortIds(educationOrganizationId)).Return(
                new List<long> { COHORT_01_01 });
        }

        public void ExpectGetEducationOrganizationCustomStudentListValid()
        {
            ExpectGetEducationOrganizationCustomStudentListValid(SCH_ID_01_01);
        }

        public void ExpectGetEducationOrganizationCustomStudentListValid(int educationOrganizationId)
        {
            Expect.Call(myAuthorizationInformationProvider.GetEducationOrganizationCustomStudentListIds(educationOrganizationId)).Return(
                new List<long> { CUSTOMSTUDENTLIST_01_01 });
        }

        public void ExpectGetSchoolSectionInvalid()
        {
            Expect.Call(myAuthorizationInformationProvider.GetSchoolSectionIds(SCH_ID_01_01)).Return(
                new List<long>());
        }

        public void ExpectGetSchoolSectionValid()
        {
            Expect.Call(myAuthorizationInformationProvider.GetSchoolSectionIds(SCH_ID_01_01)).Return(
                new List<long> { SECTION_01_01 });
        }

        public void ExpectGetSchoolStaffInvalid()
        {
            //TODO: Remove this once the CustomStudentListService is refactored to work like the MetricService with different Request objects.
            //Refer to the AuthoriztionFixtureBase.cs -> ExpectCurrentUserClaimInterrogatorHasClaimWithinEducationOrganizationHierarchy method.
            ExpectCurrentUserClaimInterrogatorHasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.AccessOrganization, SCH_ID_01_01, false);

            Expect.Call(myAuthorizationInformationProvider.GetSchoolStaffUSIs(SCH_ID_01_01)).Return(
                new List<long> { TEACHER_USI_00 });
        }

        public void ExpectGetSchoolStaffValid()
        {
            Expect.Call(myAuthorizationInformationProvider.GetSchoolStaffUSIs(SCH_ID_01_01)).Return(
                new List<long> { TEACHER_USI_01 });
        }

        public void ExpectGetLocalEducationAgencyStaffInvalid()
        {
            //TODO: Remove this once the CustomStudentListService is refactored to work like the MetricService with different Request objects.
            //Refer to the AuthoriztionFixtureBase.cs -> ExpectCurrentUserClaimInterrogatorHasClaimWithinEducationOrganizationHierarchy method.
            ExpectCurrentUserClaimInterrogatorHasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.AccessOrganization, LOCALEDUCATIONAGENCY_ID_01, false);

            Expect.Call(myAuthorizationInformationProvider.GetLocalEducationAgencyStaffUSIs(LOCALEDUCATIONAGENCY_ID_01)).Return(
                new List<long> { TEACHER_USI_00 });
        }

        public void ExpectGetLocalEducationAgencyStaffValid()
        {
            Expect.Call(myAuthorizationInformationProvider.GetLocalEducationAgencyStaffUSIs(LOCALEDUCATIONAGENCY_ID_01)).Return(
                new List<long> { TEACHER_USI_01 });
        }

        public void ExpectGetSchoolStudentsInvalid()
        {
            Expect.Call(myAuthorizationInformationProvider.IsStudentAssociatedWithSchool(STUDENT_01_01, SCH_ID_01_01))
                .Return(false);
        }

        public void ExpectGetSchoolStudentsValid()
        {
            Expect.Call(myAuthorizationInformationProvider.IsStudentAssociatedWithSchool(STUDENT_01_01, SCH_ID_01_01)).Return(
                true);
        }

        public void ExpectGetTeacherSectionValid()
        {
            Expect.Call(myAuthorizationInformationProvider.GetTeacherSectionIds(TEACHER_USI_01)).Return(
                new List<long> { SECTION_01_01 });
        }

        public void ExpectGetTeacherSectionInvalid()
        {
            Expect.Call(myAuthorizationInformationProvider.GetTeacherSectionIds(TEACHER_USI_01)).Return(
                new List<long> { SECTION_00_01 });
        }

        public void ExpectGetStaffSectionStudentsValid()
        {
            Expect.Call(myAuthorizationInformationProvider.GetTeacherStudentUSIs(TEACHER_USI_01))
                .Return(new List<long> { STUDENT_01_01, STUDENT_01_02 });
        }

        public void ExpectGetStaffSectionStudentsInvalid()
        {
            Expect.Call(myAuthorizationInformationProvider.GetTeacherStudentUSIs(TEACHER_USI_01))
                .Return(new List<long> { STUDENT_00_01, STUDENT_00_02 });
        }

        public void ExpectGetStaffCohortStudentsValid()
        {
            Expect.Call(myAuthorizationInformationProvider.GetStaffCohortStudentUSIs(TEACHER_USI_01))
                .Return(new List<long> { STUDENT_01_01, STUDENT_01_02 });
        }

        public void ExpectGetStaffCohortStudentsInvalid()
        {
            Expect.Call(myAuthorizationInformationProvider.GetStaffCohortStudentUSIs(TEACHER_USI_01))
                .Return(new List<long> { STUDENT_00_01, STUDENT_00_02 });
        }

        public void ExpectGetStudentSchoolIdsValid(string claimType, bool value)
        {
            Expect.Call(myAuthorizationInformationProvider.GetStudentSchoolIds(STUDENT_01_01))
                .Return(new List<int> {SCH_ID_01_01});
            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(claimType,
                                                                                                  SCH_ID_01_01)).Return(
                                                                                                      value);
        }

        public void ExpectGetStudentSchoolIdsInvalid()
        {
            Expect.Call(myAuthorizationInformationProvider.GetStudentSchoolIds(STUDENT_01_01))
                .Return(new List<int> { SCH_ID_00_00 });
        }

        public void ExpectGetStudentSchoolIdsInvalidValid()
        {
            Expect.Call(myAuthorizationInformationProvider.GetStudentSchoolIds(STUDENT_01_01))
                .Return(new List<int> { SCH_ID_00_00, SCH_ID_01_01 });
        }

        public void ExpectGetStudentSchoolIdsValidValid()
        {
            Expect.Call(myAuthorizationInformationProvider.GetStudentSchoolIds(STUDENT_01_01))
                .Return(new List<int> { SCH_ID_01_01, SCH_ID_01_02 });
            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                    EdFiClaimTypes.ViewMyStudents, SCH_ID_01_01)).Return(true);
            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                EdFiClaimTypes.ViewMyStudents, SCH_ID_01_02)).Return(true);
        }

        //TODO: Remove this once the CustomStudentListService is refactored to work like the MetricService with different Request objects.
        public void ExpectCurrentUserClaimInterrogatorHasClaimWithinEducationOrganizationHierarchy(string claimType, int educationOrganizationId, bool returnValue)
        {
            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(claimType, educationOrganizationId)).Return(returnValue);
        }
    }
}
