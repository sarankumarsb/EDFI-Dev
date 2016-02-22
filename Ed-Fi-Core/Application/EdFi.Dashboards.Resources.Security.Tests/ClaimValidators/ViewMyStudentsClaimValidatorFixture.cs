// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Security.ClaimValidators;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Security.Tests.ClaimValidators
{
    #region abstract Classes ViewMyStudentsClaimValidatorFixture
    public abstract class ViewMyStudentsClaimValidatorFixture : AuthorizationFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            IEnumerable<ParameterInstance> context = GetSuppliedParameters();
            var educationOrganizationParamaterInstance = (context.FirstOrDefault(p => p.Name.Equals(ClaimValidatorRequest.SchoolParameterName, StringComparison.OrdinalIgnoreCase)));
            if (educationOrganizationParamaterInstance != null && educationOrganizationParamaterInstance.Value != null)
            {
                var educationOrganization = (int)educationOrganizationParamaterInstance.Value;
                if (GetType().Name.IndexOf("NoClaim", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    Expect.Call(
                        currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                            EdFiClaimTypes.ViewMyStudents, educationOrganization)).Return(false).Repeat.Any();
                }
                else
                {
                    Expect.Call(
                        currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                            EdFiClaimTypes.ViewMyStudents, educationOrganization)).Return(true).Repeat.Any();
                }
            }
            educationOrganizationParamaterInstance = context.FirstOrDefault(p =>
                    p.Name.Equals(ClaimValidatorRequest.LocalEducationAgencyParameterName,
                                  StringComparison.OrdinalIgnoreCase));

            if (educationOrganizationParamaterInstance != null && educationOrganizationParamaterInstance.Value != null)
            {
                var educationOrganization = (int)educationOrganizationParamaterInstance.Value;
                if (GetType().Name.IndexOf("NoClaim", StringComparison.OrdinalIgnoreCase) > 0 || GetType().Name.IndexOf("LocalEducationAgencyHasSchoolClaim", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    Expect.Call(
                        currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                            EdFiClaimTypes.ViewMyStudents, educationOrganization)).Return(false).Repeat.Any();
                }
                else
                {
                    Expect.Call(
                        currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                            EdFiClaimTypes.ViewMyStudents, educationOrganization)).Return(true).Repeat.Any();
                }
            }
        }

        protected override void ExecuteTest()
        {
            try
            {
                myClaimValidator = CreateValidator(mySecurityAssertionProvider);
                myClaimValidator.ProcessRequest(GetClaimValidatorRequest());
            }
            catch (Exception ex)
            {
                myException = ex;
            }
        }
    }

    public abstract class WhenCurrentUserHasViewMyStudentsClaimSchool : ViewMyStudentsClaimValidatorFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasSchoolLevelClaims();
            base.EstablishContext();
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }

    public abstract class WhenCurrentUserHasViewMyStudentsClaimLocalEducationAgency : ViewMyStudentsClaimValidatorFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasLocalEducationAgencyLevelClaims();
            base.EstablishContext();
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }

    public abstract class WhenCurrentUserHasViewMyStudentsNoClaim : ViewMyStudentsClaimValidatorFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasNoClaims();
            base.EstablishContext();
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }
    #endregion

    #region  Current User Requests View My Students by School Staff
    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffValid : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaff(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolStaffValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffNoClaim : WhenCurrentUserHasViewMyStudentsNoClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaff(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffInvalidUser : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaff(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_00);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoStaffPermission();
        }
    }
    #endregion

    #region  Current User Requests View My Students by School Staff Cohort
    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffCohortValid : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffCohort(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetCohortIdParameter(COHORT_01_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            //ExpectGetSchoolStaffValid();
            ExpectGetEducationOrganizationCohortValid();
            ExpectGetStaffCohortsValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffCohortNoClaim : WhenCurrentUserHasViewMyStudentsNoClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffCohort(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetCohortIdParameter(COHORT_01_01);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffCohortInvalidCohort : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffCohort(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetCohortIdParameter(COHORT_01_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetEducationOrganizationCohortInvalid();
            ExpectGetStaffCohortsValid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoCohortPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffCohortStaffInvalidUser : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffCohort(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_00);
            yield return GetCohortIdParameter(COHORT_01_01);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoStaffPermission();
        }
    }
    #endregion

    #region  Current User Requests View My Students by School Staff Section
    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffSectionValid : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffSection(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetSectionIdParameter(SECTION_01_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            //ExpectGetSchoolStaffValid();
            ExpectGetSchoolSectionValid();
            ExpectGetTeacherSectionValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffSectionNoClaim : WhenCurrentUserHasViewMyStudentsNoClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffSection(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetSectionIdParameter(SECTION_01_01);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffSectionInvalidSection : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffSection(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetSectionIdParameter(SECTION_01_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolSectionInvalid();
            ExpectGetTeacherSectionValid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoSectionPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffSectionStaffNotCurrentUser : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffSection(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_00);
            yield return GetSectionIdParameter(SECTION_01_01);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoStaffPermission();
        }
    }
    #endregion

    #region Current User Requests View My Students by School Staff Student List
    // <claimName>BY<signature><listType><Valid|NoClaim|InvalidUser|[InvalidSection|InvalidCohort]

    // The All Case:
    // ViewMyStudentsClaimSchoolStaffStudentListAll is equivalent to ViewMyStudentsClaimSchoolStaff
    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffStudentListAllValid : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.All.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolStaffValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffStudentListAllNoClaim : WhenCurrentUserHasViewMyStudentsNoClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.All.ToString());
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffStudentListAllInvalidUser : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_00);
            yield return GetStudentListTypeParameter(StudentListType.All.ToString());
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoStaffPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffStudentListMetricsBasedWatchListValid : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.MetricsBasedWatchList.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolStaffValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    // The Section Case:
    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffStudentListSectionValid : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.Section.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetTeacherSectionValid();
            ExpectGetSchoolSectionValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffStudentListSectionNoClaim : WhenCurrentUserHasViewMyStudentsNoClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.Section.ToString());
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffStudentListSectionInvalidUser : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_00);
            yield return GetStudentListTypeParameter(StudentListType.Section.ToString());
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoStaffPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffStudentListSectionInvalidTeacherSection : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.Section.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetTeacherSectionInvalid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoSectionPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffStudentListSectionInvalidSchoolSection : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.Section.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetTeacherSectionValid();
            ExpectGetSchoolSectionInvalid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoSectionPermission();
        }
    }

    // The Cohort Case:
    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffStudentListCohortValid : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.Cohort.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetStaffCohortsValid();
            ExpectGetEducationOrganizationCohortValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    // The CustomStudentList Case:
    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffStudentListCustomStudentListValid : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(CUSTOMSTUDENTLIST_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.CustomStudentList.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetStaffCustomStudentListsValid();
            ExpectGetEducationOrganizationCustomStudentListValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffStudentListCohortNoClaim : WhenCurrentUserHasViewMyStudentsNoClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.Cohort.ToString());
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffStudentListCustomStudentListNoClaim : WhenCurrentUserHasViewMyStudentsNoClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.CustomStudentList.ToString());
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffStudentListCohortInvalidUser : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_00);
            yield return GetStudentListTypeParameter(StudentListType.Cohort.ToString());
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoStaffPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffStudentListCustomStudentListInvalidUser : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_00);
            yield return GetStudentListTypeParameter(StudentListType.Cohort.ToString());
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoStaffPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffStudentListCohortInvalidStaffCohort : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.Cohort.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetStaffCohortsInvalid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoCohortPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffStudentListCustomStudentListInvalidStaffCustomStudentList : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.CustomStudentList.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetStaffCustomStudentListsInvalid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoCustomStudentListPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffStudentListCohortInvalidSchoolCohort : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.Cohort.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetStaffCohortsValid();
            ExpectGetEducationOrganizationCohortInvalid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoCohortPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffStudentListCustomStudentListInvalidSchoolCustomStudentList : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.Cohort.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            //ExpectGetStaffCustomStudentListsValid();
            //ExpectGetStaffCustomStudentListsInvalid();
            //ExpectGetEducationOrganizationCustomStudentListInvalid();
            ExpectGetStaffCohortsValid();
            ExpectGetEducationOrganizationCohortInvalid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoCohortPermission();
        }
    }

    // The None Case:
    // DJWhite 4 Jan 2012: Default null or empty to NONE which is equivalent to ALL.
    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffStudentListNoneValid : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.None.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolStaffValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffStudentListNullValid : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(null);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolStaffValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStaffStudentListEmptyValid : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter("");
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolStaffValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }
    #endregion

    #region Current User Requests View My Students by School Student
    // This is equivalent to SchoolStaffStudent with the staff member being the current user.
    // Validation can occur via Section or Cohort.
    //
    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStudentValid : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStudent(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStudentUSIParameter(STUDENT_01_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetStaffSectionStudentsValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStudentNoClaim : WhenCurrentUserHasViewMyStudentsNoClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStudent(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStudentUSIParameter(STUDENT_01_01);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStudentInvalidSectionValidCohort : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStudent(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStudentUSIParameter(STUDENT_01_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetStaffSectionStudentsInvalid();
            ExpectGetStaffCohortStudentsValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolStudentInvalidSectionInvalidCohort : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchoolStudent(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStudentUSIParameter(STUDENT_01_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetStaffSectionStudentsInvalid();
            ExpectGetStaffCohortStudentsInvalid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoStudentPermission();
        }
    }
    #endregion

    #region Current User Requests View My Students by School
    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolValid : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchool(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimSchoolNoClaim : WhenCurrentUserHasViewMyStudentsNoClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorSchool(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }
    #endregion

    #region Current User Requests View My Students by Student
    public class WhenCurrentUserRequestsViewMyStudentsClaimStudentNoClaim : WhenCurrentUserHasViewMyStudentsNoClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorStudent(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetStudentUSIParameter(STUDENT_01_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetStudentSchoolIdsValid(EdFiClaimTypes.ViewMyStudents, false);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoStudentPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimStudentSchoolOneValid : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorStudent(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetStudentUSIParameter(STUDENT_01_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetStudentSchoolIdsValid(EdFiClaimTypes.ViewMyStudents, true);

            Expect.Call(myAuthorizationInformationProvider.IsStudentAssociatedWithStaffAtSchool(STUDENT_01_01, TEACHER_USI_01, SCH_ID_01_01))
                .Return(true);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimStudentSchoolOneInvalid : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorStudent(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetStudentUSIParameter(STUDENT_01_01);
        }
        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetStudentSchoolIdsValid(EdFiClaimTypes.ViewMyStudents, true);

            Expect.Call(myAuthorizationInformationProvider.IsStudentAssociatedWithStaffAtSchool(STUDENT_01_01, TEACHER_USI_01, SCH_ID_01_01))
                .Return(false);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoStudentPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimStudentSchoolTwoValid : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorStudent(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetStudentUSIParameter(STUDENT_01_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetStudentSchoolIdsValidValid();

            // The student is associated with the staff at the second school, but not the first.
            Expect.Call(myAuthorizationInformationProvider.IsStudentAssociatedWithStaffAtSchool(STUDENT_01_01, TEACHER_USI_01, SCH_ID_01_01))
                .Return(false);
            Expect.Call(myAuthorizationInformationProvider.IsStudentAssociatedWithStaffAtSchool(STUDENT_01_01, TEACHER_USI_01, SCH_ID_01_02))
                .Return(true);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimStudentSchoolTwoInvalid : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorStudent(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetStudentUSIParameter(STUDENT_01_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetStudentSchoolIdsValidValid();

            // The student is associated with the staff at the second school, but not the first.
            Expect.Call(myAuthorizationInformationProvider.IsStudentAssociatedWithStaffAtSchool(STUDENT_01_01, TEACHER_USI_01, SCH_ID_01_01))
                .Return(false);
            Expect.Call(myAuthorizationInformationProvider.IsStudentAssociatedWithStaffAtSchool(STUDENT_01_01, TEACHER_USI_01, SCH_ID_01_02))
                .Return(false);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoStudentPermission();
        }
    }
    #endregion

    #region Current User Requests View My Students by LocalEducationAgency School Student
    // This is only going to test a Valid LocalEducationAgency, and an Invalid LocalEducationAgency.  All other cases are already tested elsewhere.
    //
    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStudentLocalEducationAgencyValid : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStudent(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStudentUSIParameter(STUDENT_01_01);
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(_schoolIdNameService.Get(null)).IgnoreArguments().Return(new IdNameModel
            {
                LocalEducationAgencyId =
                    LOCALEDUCATIONAGENCY_ID_01,
                SchoolId = SCH_ID_01_01
            });
            mySecurityAssertionProvider.SetSchoolIdNameService(_schoolIdNameService);
            ExpectGetStaffSectionStudentsValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStudentLocalEducationAgencyNoClaim : WhenCurrentUserHasViewMyStudentsNoClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStudent(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStudentUSIParameter(STUDENT_01_01);
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }


    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStudentLocalEducationAgencyInvalid : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStudent(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStudentUSIParameter(STUDENT_01_01);
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_00);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(_schoolIdNameService.Get(null)).IgnoreArguments().Return(new IdNameModel
                                                                                     {
                                                                                         LocalEducationAgencyId =
                                                                                             LOCALEDUCATIONAGENCY_ID_01,
                                                                                         SchoolId = SCH_ID_01_01
                                                                                     });
            mySecurityAssertionProvider.SetSchoolIdNameService(_schoolIdNameService);
            ExpectGetStaffSectionStudentsValid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoSchoolPermission();
        }
    }
    #endregion

    #region Current User Requests View My Students by Local Education Agency Staff StudentList

    // DJWhite 4 Jan 2012: Default null or empty to NONE which is equivalent to ALL.
    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencyStaffStudentListAllValid : WhenCurrentUserHasViewMyStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencyStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.All.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetLocalEducationAgencyStaffValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencyStaffStudentListMetricsBasedWatchListValid : WhenCurrentUserHasViewMyStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencyStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.MetricsBasedWatchList.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetLocalEducationAgencyStaffValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencyStaffStudentListNullValid : WhenCurrentUserHasViewMyStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencyStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(null);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetLocalEducationAgencyStaffValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencyStaffStudentListEmptyValid : WhenCurrentUserHasViewMyStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencyStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter("");
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetLocalEducationAgencyStaffValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserHasRequestsViewMyStudentsClaimLocalEducationAgencyStaffStudentListAllNoClaim : WhenCurrentUserHasViewMyStudentsNoClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencyStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.All.ToString());
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencyStaffStudentListAllInvalidStaff : WhenCurrentUserHasViewMyStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencyStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.All.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetLocalEducationAgencyStaffInvalid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoStaffPermission();
        }
    }

    // The Cohort Case:
    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencyStaffStudentListCohortValid : WhenCurrentUserHasViewMyStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencyStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.Cohort.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetStaffCohortsValid();
            ExpectGetEducationOrganizationCohortValid(LOCALEDUCATIONAGENCY_ID_01);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencyStaffStudentListCohortStaffCohortInvalid : WhenCurrentUserHasViewMyStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencyStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.Cohort.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetStaffCohortsInvalid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoCohortPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencyStaffStudentListCohortSchoolCohortInvalid : WhenCurrentUserHasViewMyStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencyStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.Cohort.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetStaffCohortsValid();
            ExpectGetEducationOrganizationCohortInvalid(LOCALEDUCATIONAGENCY_ID_01);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoCohortPermission();
        }
    }

    // The None Case:
    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencyStaffStudentListNoneValid : WhenCurrentUserHasViewMyStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencyStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.None.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetLocalEducationAgencyStaffValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }
    #endregion

    #region Current User Requests View My Students by Local Education Agency Staff StudentList

    // Default null or empty to NONE which is equivalent to ALL.
    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListAllValid : WhenCurrentUserHasViewMyStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.All.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolIdName();
            ExpectGetSchoolIdName();
            ExpectGetSchoolStaffValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListNullValid : WhenCurrentUserHasViewMyStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(null);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolIdName();
            ExpectGetSchoolIdName();
            ExpectGetSchoolStaffValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListEmptyValid : WhenCurrentUserHasViewMyStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter("");
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolIdName();
            ExpectGetSchoolIdName();
            ExpectGetSchoolStaffValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserHasRequestsViewMyStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListAllNoClaim : WhenCurrentUserHasViewMyStudentsNoClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.All.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolIdName();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListAllInvalidStaff : WhenCurrentUserHasViewMyStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.All.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolIdName();
            ExpectGetSchoolStaffInvalid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoStaffPermission();
        }
    }

    // The Cohort Case:
    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListCohortValid : WhenCurrentUserHasViewMyStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.Cohort.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolIdName();
            ExpectGetStaffCohortsValid();
            ExpectGetEducationOrganizationCohortValid(LOCALEDUCATIONAGENCY_ID_01);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListCohortStaffCohortInvalid : WhenCurrentUserHasViewMyStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.Cohort.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolIdName();
            ExpectGetStaffCohortsInvalid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoCohortPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListCohortSchoolCohortInvalid : WhenCurrentUserHasViewMyStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.Cohort.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolIdName();
            ExpectGetStaffCohortsValid();
            ExpectGetEducationOrganizationCohortInvalid(LOCALEDUCATIONAGENCY_ID_01);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoCohortPermission();
        }
    }

    // The CustomStudentList Case:
    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListCustomStudentListNoClaimSchool : WhenCurrentUserHasViewMyStudentsNoClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(CUSTOMSTUDENTLIST_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.CustomStudentList.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolIdName();
        }

        [Test]
        public void ShouldFailNoLocalEducationAgencyPermission()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListCustomStudentListNoClaimLocalEducationAgency : WhenCurrentUserHasViewMyStudentsNoClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetSectionOrCohortIdParameter(CUSTOMSTUDENTLIST_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.CustomStudentList.ToString());
        }

        [Test]
        public void ShouldFailNoLocalEducationAgencyPermission()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListCustomStudentListClaimSchool : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(CUSTOMSTUDENTLIST_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.CustomStudentList.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetEducationOrganizationCustomStudentListValid();
            ExpectGetStaffCustomStudentListsValid();
            Expect.Call(_schoolIdNameService.Get(null)).IgnoreArguments().Repeat.Any().Return(new IdNameModel { LocalEducationAgencyId = LOCALEDUCATIONAGENCY_ID_01, SchoolId = SCH_ID_01_01 });
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListCustomStudentListClaimSchoolInvalidStudentList : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(CUSTOMSTUDENTLIST_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.CustomStudentList.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolIdName();
            ExpectGetStaffCustomStudentListsInvalid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoCustomStudentListPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListCustomStudentListClaimLocalEducationAgency : WhenCurrentUserHasViewMyStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetSectionOrCohortIdParameter(CUSTOMSTUDENTLIST_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.CustomStudentList.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetEducationOrganizationCustomStudentListValid(LOCALEDUCATIONAGENCY_ID_01);
            ExpectGetStaffCustomStudentListsValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListCustomStudentListClaimLocalEducationAgencyInvalidStudentList : WhenCurrentUserHasViewMyStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetSectionOrCohortIdParameter(CUSTOMSTUDENTLIST_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.CustomStudentList.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetStaffCustomStudentListsValid();
            ExpectGetEducationOrganizationCustomStudentListInvalid(LOCALEDUCATIONAGENCY_ID_01);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoCustomStudentListPermission();
        }
    }

    // The None Case:
    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListNoneValid : WhenCurrentUserHasViewMyStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.None.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolIdName();
            ExpectGetSchoolIdName();
            ExpectGetSchoolStaffValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }
    #endregion

    #region  Current User Requests View My Students by Local Education Agency School Staff
    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStaffClaimLocalEducationAgency : WhenCurrentUserHasViewMyStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaff(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetLocalEducationAgencyStaffValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStaffNoClaim : WhenCurrentUserHasViewMyStudentsNoClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaff(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
        }

        [Test]
        public void ShouldFailNoLocalEducationAgencyPermission()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStaffClaimSchool : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaff(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolStaffValid();
            Expect.Call(_schoolIdNameService.Get(null)).IgnoreArguments().Return(new IdNameModel { LocalEducationAgencyId = LOCALEDUCATIONAGENCY_ID_01, SchoolId = SCH_ID_01_01 });
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStaffNoClaimForSchool : WhenCurrentUserHasViewMyStudentsNoClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaff(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }
    #endregion

    #region  Current User Requests View My Students by Local Education Agency School Staff CustomStudentList

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStaffCustomStudentListClaimLocalEducationAgency : WhenCurrentUserHasViewMyStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaffCustomStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetCustomStudentListIdNullableParameter(CUSTOMSTUDENTLIST_01_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetLocalEducationAgencyStaffValid();
            ExpectGetStaffCustomStudentListsValid();
            ExpectGetEducationOrganizationCustomStudentListValid(LOCALEDUCATIONAGENCY_ID_01);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStaffCustomStudentListClaimInvalidStudentListLocalEducationAgency : WhenCurrentUserHasViewMyStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaffCustomStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetCustomStudentListIdNullableParameter(CUSTOMSTUDENTLIST_01_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetLocalEducationAgencyStaffValid();
            ExpectGetStaffCustomStudentListsValid();
            ExpectGetEducationOrganizationCustomStudentListInvalid(LOCALEDUCATIONAGENCY_ID_01);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoCustomStudentListPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStaffCustomStudentListNoClaim : WhenCurrentUserHasViewMyStudentsNoClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaffCustomStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetCustomStudentListIdNullableParameter(CUSTOMSTUDENTLIST_01_01);
        }

        [Test]
        public void ShouldFailNoLocalEducationAgencyPermission()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStaffCustomStudentListClaimSchool : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaffCustomStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetCustomStudentListIdNullableParameter(CUSTOMSTUDENTLIST_01_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolStaffValid();
            ExpectGetEducationOrganizationCustomStudentListValid();
            ExpectGetStaffCustomStudentListsValid();
            Expect.Call(_schoolIdNameService.Get(null)).IgnoreArguments().Repeat.Any().Return(new IdNameModel { LocalEducationAgencyId = LOCALEDUCATIONAGENCY_ID_01, SchoolId = SCH_ID_01_01 });
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStaffCustomStudentListClaimInvalidStudentListSchool : WhenCurrentUserHasViewMyStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaffCustomStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetCustomStudentListIdNullableParameter(CUSTOMSTUDENTLIST_01_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolStaffValid();
            ExpectGetStaffCustomStudentListsValid();
            ExpectGetEducationOrganizationCustomStudentListInvalid();
            ExpectGetLocalEducationAgencyStaffValid();
            ExpectGetStaffCustomStudentListsValid();
            ExpectGetEducationOrganizationCustomStudentListInvalid(LOCALEDUCATIONAGENCY_ID_01);
            Expect.Call(_schoolIdNameService.Get(null)).IgnoreArguments().Repeat.Any().Return(new IdNameModel { LocalEducationAgencyId = LOCALEDUCATIONAGENCY_ID_01, SchoolId = SCH_ID_01_01 });
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoCustomStudentListPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyStudentsClaimLocalEducationAgencySchoolStaffCustomStudentListNoClaimForSchool : WhenCurrentUserHasViewMyStudentsNoClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaffCustomStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetCustomStudentListIdNullableParameter(CUSTOMSTUDENTLIST_01_01);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }
    #endregion

}
