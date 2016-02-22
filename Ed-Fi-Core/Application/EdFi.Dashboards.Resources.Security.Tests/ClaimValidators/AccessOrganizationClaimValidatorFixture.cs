// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Security.ClaimValidators;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Security.Tests.ClaimValidators
{
    #region abstract Classes
    public abstract class AccessOrganizationClaimValidatorFixture : AuthorizationFixtureBase
    {
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

    public abstract class WhenCurrentUserHasLocalEducationAgencyLevelAccessOrganizationClaim : AccessOrganizationClaimValidatorFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasLocalEducationAgencyLevelClaims();
            base.EstablishContext();
            IEnumerable<ParameterInstance> context = GetSuppliedParameters();
            var educationOrganizationParamaterInstance = (context.FirstOrDefault(p => p.Name.Equals(ClaimValidatorRequest.SchoolParameterName, StringComparison.OrdinalIgnoreCase))
            ?? context.FirstOrDefault(p => p.Name.Equals(ClaimValidatorRequest.LocalEducationAgencyParameterName, StringComparison.OrdinalIgnoreCase)));
            if (educationOrganizationParamaterInstance != null && educationOrganizationParamaterInstance.Value != null)
            {
                var educationOrganization = (int)educationOrganizationParamaterInstance.Value;

                Expect.Call(
                    currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                        EdFiClaimTypes.AccessOrganization, educationOrganization)).Return(true).Repeat.Any();
            }
            //else{Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.AccessOrganization, null)).Return(true);}
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }

    public abstract class WhenCurrentUserHasSchoolLevelAccessOrganizationClaim : AccessOrganizationClaimValidatorFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasSchoolLevelClaims();
            base.EstablishContext();
            IEnumerable<ParameterInstance> context = GetSuppliedParameters();
            var educationOrganizationParamaterInstance = (context.FirstOrDefault(p => p.Name.Equals(ClaimValidatorRequest.SchoolParameterName, StringComparison.OrdinalIgnoreCase))
            ?? context.FirstOrDefault(p => p.Name.Equals(ClaimValidatorRequest.LocalEducationAgencyParameterName, StringComparison.OrdinalIgnoreCase)));
            if (educationOrganizationParamaterInstance != null && educationOrganizationParamaterInstance.Value != null)
            {
                var educationOrganization = (int)educationOrganizationParamaterInstance.Value;

                Expect.Call(
                    currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                        EdFiClaimTypes.AccessOrganization, educationOrganization)).Return(true).Repeat.Any();
            }
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }

    public abstract class WhenCurrentUserHasNoAccessOrganizationClaim : AccessOrganizationClaimValidatorFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasNoClaims();
            base.EstablishContext();
            IEnumerable<ParameterInstance> context = GetSuppliedParameters();
            var educationOrganizationParamaterInstance = (context.FirstOrDefault(p => p.Name.Equals(ClaimValidatorRequest.SchoolParameterName, StringComparison.OrdinalIgnoreCase))
            ?? context.FirstOrDefault(p => p.Name.Equals(ClaimValidatorRequest.LocalEducationAgencyParameterName, StringComparison.OrdinalIgnoreCase)));
            if (educationOrganizationParamaterInstance != null && educationOrganizationParamaterInstance.Value != null)
            {
                var educationOrganization = (int)educationOrganizationParamaterInstance.Value;

                Expect.Call(
                    currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                        EdFiClaimTypes.AccessOrganization, educationOrganization)).Return(false).Repeat.Any();
            }
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }
    #endregion

    #region  Current User Requests Access Organizations by LocalEducationAgency
    public class WhenCurrentUserRequestsAccessOrganizationClaimLocalEducationAgencyHasLocalEducationAgencyClaim : WhenCurrentUserHasLocalEducationAgencyLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorLocalEducationAgency(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    // N.B.  The logic of this claim is reversed from the norm.  If the CurrentUser has a claim on the school level,
    // Then the CurrentUser should be able to validate against the LocalEducationAgency
    //
    public class WhenCurrentUserRequestsAccessOrganizationClaimLocalEducationAgencyHasSchoolClaim : WhenCurrentUserHasSchoolLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorLocalEducationAgency(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsAccessOrganizationClaimLocalEducationAgencyHasNoClaim : WhenCurrentUserHasNoAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorLocalEducationAgency(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
        }

        [Test]
        public void ShouldFailNoLocalEducationAgencyPermission()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }
    #endregion

    #region  Current User Requests Access Organizations by School
    public class WhenCurrentUserRequestsAccessOrganizationClaimSchoolHasLocalEducationAgencyClaim : WhenCurrentUserHasLocalEducationAgencyLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorSchool(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsAccessOrganizationClaimSchoolHasSchoolClaim : WhenCurrentUserHasSchoolLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorSchool(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsAccessOrganizationClaimSchoolHasNoClaim : WhenCurrentUserHasNoAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorSchool(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
        }

        [Test]
        public void ShouldFailNoLocalEducationAgencyPermission()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }
    #endregion

    #region  Current User Requests Access Organizations by School Staff
    public class WhenCurrentUserRequestsAccessOrganizationClaimSchoolStaffHasLocalEducationAgencyClaim : WhenCurrentUserHasLocalEducationAgencyLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorSchoolStaff(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            // since we'll be only do validation with current claims, there will be no call to the database
            // ExpectGetSchoolStaffValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsAccessOrganizationClaimSchoolStaffHasSchoolClaim : WhenCurrentUserHasSchoolLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorSchoolStaff(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            // since we'll be only do validation with current claims, there will be no call to the database
            // ExpectGetSchoolStaffValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsAccessOrganizationClaimSchoolStaffHasNoClaim : WhenCurrentUserHasNoAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorSchoolStaff(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
        }

        [Test]
        public void ShouldFailNoLocalEducationAgencyPermission()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }
    #endregion

    #region  Current User Requests Access Organizations by School Staff StudentList
    public class WhenCurrentUserRequestsAccessOrganizationClaimSchoolStaffStudentListHasLocalEducationAgencyClaimSection : WhenCurrentUserHasLocalEducationAgencyLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
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

    public class WhenCurrentUserRequestsAccessOrganizationClaimSchoolStaffStudentListHasSchoolClaimSection : WhenCurrentUserHasSchoolLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
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

    public class WhenCurrentUserRequestsAccessOrganizationClaimSchoolStaffStudentListHasLocalEducationAgencyClaimCohort : WhenCurrentUserHasLocalEducationAgencyLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
            yield return GetStudentListTypeParameter(StudentListType.Cohort.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetStaffCohortsValid();
            ExpectGetEducationOrganizationCohortValid(SCH_ID_01_01);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsAccessOrganizationClaimSchoolStaffStudentListHasSchoolClaimCohort : WhenCurrentUserHasSchoolLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
            yield return GetStudentListTypeParameter(StudentListType.Cohort.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetStaffCohortsValid();
            ExpectGetEducationOrganizationCohortValid(SCH_ID_01_01);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsAccessOrganizationClaimSchoolStaffStudentListHasLocalEducationAgencyClaimCustomStudentList : WhenCurrentUserHasLocalEducationAgencyLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
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

    public class WhenCurrentUserRequestsAccessOrganizationClaimSchoolStaffStudentListHasSchoolClaimCustomStudentList : WhenCurrentUserHasSchoolLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
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

    public class WhenCurrentUserRequestsAccessOrganizationClaimSchoolStaffStudentListHasLocalEducationAgencyClaimMetricsBasedWatchList : WhenCurrentUserHasLocalEducationAgencyLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
            yield return GetStudentListTypeParameter(StudentListType.MetricsBasedWatchList.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetStaffCustomMetricsBasedWatchListIds();
            ExpectGetEducationOrganizationCustomMetricsBasedWatchListIds(SCH_ID_01_01);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsAccessOrganizationClaimSchoolStaffStudentListHasSchoolClaimMetricsBasedWatchList : WhenCurrentUserHasSchoolLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
            yield return GetStudentListTypeParameter(StudentListType.MetricsBasedWatchList.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetStaffCustomMetricsBasedWatchListIds();
            ExpectGetEducationOrganizationCustomMetricsBasedWatchListIds(SCH_ID_01_01);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsAccessOrganizationClaimSchoolStaffStudentListHasLocalEducationAgencyClaimAll : WhenCurrentUserHasLocalEducationAgencyLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
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

    public class WhenCurrentUserRequestsAccessOrganizationClaimSchoolStaffStudentListHasSchoolClaimAll : WhenCurrentUserHasSchoolLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
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

    public class WhenCurrentUserRequestsAccessOrganizationClaimSchoolStaffStudentListClaimBadType :
        WhenCurrentUserHasSchoolLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStudentListTypeParameter("foo");
        }

        [Test]
        public void ShouldNotPass()
        {
            ExpectException(typeof(UserAccessDeniedException), ClaimValidatorRequest.InvalidParameterErrorMessage);
        }
    }

    public class WhenCurrentUserRequestsAccessOrganizationClaimSchoolStaffStudentListHasNoClaim : WhenCurrentUserHasNoAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStudentListTypeParameter(StudentListType.Section.ToString());
        }

        [Test]
        public void ShouldFailNoLocalEducationAgencyPermission()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }
    #endregion

    #region  Current User Requests Access Organizations by LocalEducationAgency School
    // N.B. The AccessOrganizationClaimValidatorLocalEducationAgencySchool ignores the LocalEducationAgency, as access
    // to the LocalEducationAgency is implicit in access to the school.
    // The LocalEducationAgency and School parameters for the LevelCrumbService are nullable, so that needs to be tested as well.
    //
    public class WhenCurrentUserRequestsAccessOrganizationClaimLocalEducationAgencySchoolHasLocalEducationAgencyClaim : WhenCurrentUserHasLocalEducationAgencyLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorLocalEducationAgencySchool(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsAccessOrganizationClaimLocalEducationAgencySchoolLocalEducationAgencyNullHasLocalEducationAgencyClaim : WhenCurrentUserHasLocalEducationAgencyLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorLocalEducationAgencySchool(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(null);
            yield return GetSchoolIdNullableParameter(SCH_ID_01_01);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsAccessOrganizationClaimLocalEducationAgencySchoolSchoolNullHasLocalEducationAgencyClaim : WhenCurrentUserHasLocalEducationAgencyLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorLocalEducationAgencySchool(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsAccessOrganizationClaimLocalEducationAgencySchoolLocalEducationAgencyNullSchoolNullHasLocalEducationAgencyClaim : WhenCurrentUserHasLocalEducationAgencyLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorLocalEducationAgencySchool(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(null);
            yield return GetSchoolIdNullableParameter(null);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFail(typeof(ArgumentException));
        }
    }

    public class WhenCurrentUserRequestsAccessOrganizationClaimLocalEducationAgencySchoolHasSchoolClaim : WhenCurrentUserHasSchoolLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorLocalEducationAgencySchool(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsAccessOrganizationClaimLocalEducationAgencySchoolLocalEducationAgencyNullHasSchoolClaim : WhenCurrentUserHasSchoolLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorLocalEducationAgencySchool(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(null);
            yield return GetSchoolIdNullableParameter(SCH_ID_01_01);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsAccessOrganizationClaimLocalEducationAgencySchoolSchoolNullHasSchoolClaim : WhenCurrentUserHasSchoolLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorLocalEducationAgencySchool(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsAccessOrganizationClaimLocalEducationAgencySchoolLocalEducationAgencyNullSchoolNullHasSchoolClaim : WhenCurrentUserHasSchoolLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorLocalEducationAgencySchool(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(null);
            yield return GetSchoolIdNullableParameter(null);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFail(typeof(ArgumentException));
        }
    }

    public class WhenCurrentUserRequestsAccessOrganizationClaimLocalEducationAgencySchoolHasNoClaim : WhenCurrentUserHasNoAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorLocalEducationAgencySchool(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
        }

        [Test]
        public void ShouldFailNoLocalEducationAgencyPermission()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }
    #endregion

    #region  Current User Requests Access Organizations by Local Education Agency Staff
    public class WhenCurrentUserRequestsAccessOrganizationClaimLocalEducationAgencyStaffHasLocalEducationAgencyClaim : WhenCurrentUserHasLocalEducationAgencyLevelAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorLocalEducationAgencyStaff(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
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

    public class WhenCurrentUserRequestsAccessOrganizationClaimLocalEducationAgencyStaffHasNoClaim : WhenCurrentUserHasNoAccessOrganizationClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AccessOrganizationClaimValidatorLocalEducationAgencyStaff(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
        }

        [Test]
        public void ShouldFailNoLocalEducationAgencyPermission()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }
    #endregion
}
