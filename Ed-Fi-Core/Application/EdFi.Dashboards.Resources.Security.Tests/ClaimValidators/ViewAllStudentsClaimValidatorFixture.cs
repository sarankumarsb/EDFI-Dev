// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Security.ClaimValidators;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using NUnit.Framework;
using Rhino.Mocks;
using BriefModel = EdFi.Dashboards.Resources.Models.Student.BriefModel;

namespace EdFi.Dashboards.Resources.Security.Tests.ClaimValidators
{
    #region abstract Classes ViewAllStudentsClaimValidatorFixture
    public abstract class ViewAllStudentsClaimValidatorFixture : AuthorizationFixtureBase
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

    public abstract class WhenCurrentUserRequestsViewAllStudentsClaimSchool : ViewAllStudentsClaimValidatorFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasSchoolLevelClaims();
            base.EstablishContext();
            IEnumerable<ParameterInstance> context = GetSuppliedParameters();
            var educationOrganizationParamaterInstance = (context.FirstOrDefault(p => p.Name.Equals(ClaimValidatorRequest.SchoolParameterName, StringComparison.OrdinalIgnoreCase)));

            if (educationOrganizationParamaterInstance != null &&
                educationOrganizationParamaterInstance.Value != null)
            {
                var educationOrganization = (int)educationOrganizationParamaterInstance.Value;
                Expect.Call(
                    currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                        EdFiClaimTypes.ViewAllStudents, educationOrganization)).Return(true).Repeat.Any();
            }
            educationOrganizationParamaterInstance = context.FirstOrDefault(p =>
                    p.Name.Equals(ClaimValidatorRequest.LocalEducationAgencyParameterName,
                                  StringComparison.OrdinalIgnoreCase));

            if (educationOrganizationParamaterInstance != null &&
                educationOrganizationParamaterInstance.Value != null)
            {
                var educationOrganization = (int)educationOrganizationParamaterInstance.Value;
                Expect.Call(
                    currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                        EdFiClaimTypes.ViewAllStudents, educationOrganization)).Return(true).Repeat.Any();
            }
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }

    public abstract class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgency : ViewAllStudentsClaimValidatorFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasLocalEducationAgencyLevelClaims();
            base.EstablishContext();
            IEnumerable<ParameterInstance> context = GetSuppliedParameters();
            var educationOrganizationParamaterInstance = (context.FirstOrDefault(p => p.Name.Equals(ClaimValidatorRequest.SchoolParameterName, StringComparison.OrdinalIgnoreCase)));
            if (educationOrganizationParamaterInstance != null && educationOrganizationParamaterInstance.Value != null)
            {
                var educationOrganization = (int)educationOrganizationParamaterInstance.Value;

                Expect.Call(
                    currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                        EdFiClaimTypes.ViewAllStudents, educationOrganization)).Return(true).Repeat.Any();
            }
            educationOrganizationParamaterInstance = context.FirstOrDefault(p =>
                    p.Name.Equals(ClaimValidatorRequest.LocalEducationAgencyParameterName,
                                  StringComparison.OrdinalIgnoreCase));

            if (educationOrganizationParamaterInstance != null && educationOrganizationParamaterInstance.Value != null)
            {
                var educationOrganization = (int)educationOrganizationParamaterInstance.Value;

                Expect.Call(
                    currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                        EdFiClaimTypes.ViewAllStudents, educationOrganization)).Return(true).Repeat.Any();
            }
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }

    public abstract class WhenCurrentUserRequestsViewAllStudentsClaimNone : ViewAllStudentsClaimValidatorFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasNoClaims();
            base.EstablishContext();
            IEnumerable<ParameterInstance> context = GetSuppliedParameters();
            var educationOrganizationParamaterInstance = (context.FirstOrDefault(p => p.Name.Equals(ClaimValidatorRequest.SchoolParameterName, StringComparison.OrdinalIgnoreCase)));
            if (educationOrganizationParamaterInstance != null && educationOrganizationParamaterInstance.Value != null)
            {
                var educationOrganization = (int)educationOrganizationParamaterInstance.Value;

                Expect.Call(
                    currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                        EdFiClaimTypes.ViewAllStudents, educationOrganization)).Return(false).Repeat.Any();
            }
            educationOrganizationParamaterInstance = context.FirstOrDefault(p =>
                    p.Name.Equals(ClaimValidatorRequest.LocalEducationAgencyParameterName,
                                  StringComparison.OrdinalIgnoreCase));

            if (educationOrganizationParamaterInstance != null && educationOrganizationParamaterInstance.Value != null)
            {
                var educationOrganization = (int)educationOrganizationParamaterInstance.Value;

                Expect.Call(
                    currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                        EdFiClaimTypes.ViewAllStudents, educationOrganization)).Return(false).Repeat.Any();
            }
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }
    #endregion

    #region  Current User Requests View All Students by Local Education Agency
    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencyClaimSchool : WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgency(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencyClaimNone : WhenCurrentUserRequestsViewAllStudentsClaimNone
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgency(securityAssertionProvider, null);
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

    #region  Current User Requests View All Students by School
    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolClaimSchool : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchool(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolClaimNone : WhenCurrentUserRequestsViewAllStudentsClaimNone
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchool(securityAssertionProvider, null);
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

    #region  Current User Requests View All Students by School Cohort
    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolCohortValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolCohort(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetCohortIdParameter(COHORT_01_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetEducationOrganizationCohortValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolCohortNoClaim : WhenCurrentUserRequestsViewAllStudentsClaimNone
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolCohort(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetCohortIdParameter(COHORT_01_01);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }

    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolCohortInvalidCohort : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolCohort(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetCohortIdParameter(COHORT_01_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetEducationOrganizationCohortInvalid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoCohortPermission();
        }
    }
    #endregion

    #region  Current User Requests View All Students by School Staff
    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolStaffValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStaff(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolStaffNoClaim : WhenCurrentUserRequestsViewAllStudentsClaimNone
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStaff(securityAssertionProvider, null);
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
    #endregion

    #region  Current User Requests View All Students by School Staff Cohort
    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolStaffCohortValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStaffCohort(securityAssertionProvider, null);
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
            ExpectGetEducationOrganizationCohortValid();
            ExpectGetStaffCohortsValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolStaffCohortNoClaim : WhenCurrentUserRequestsViewAllStudentsClaimNone
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStaffCohort(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolStaffCohortInvalidStaff : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStaffCohort(securityAssertionProvider, null);
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
            ExpectGetStaffCohortsInvalid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoCohortPermission();
        }
    }

    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolStaffCohortInvalidCohort : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStaffCohort(securityAssertionProvider, null);
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
    #endregion

    #region  Current User Requests View All Students by School Staff Section
    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolStaffSectionValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStaffSection(securityAssertionProvider, null);
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
            ExpectGetSchoolSectionValid();
            ExpectGetTeacherSectionValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolStaffSectionNoClaim : WhenCurrentUserRequestsViewAllStudentsClaimNone
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStaffSection(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolStaffSectionInvalidTeacherSection : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStaffSection(securityAssertionProvider, null);
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
            ExpectGetTeacherSectionInvalid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoSectionPermission();
        }
    }

    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolStaffSectionInvalidSchoolSection : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStaffSection(securityAssertionProvider, null);
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
            ExpectGetTeacherSectionValid();
            ExpectGetSchoolSectionInvalid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoSectionPermission();
        }
    }
    #endregion

    #region Current User Requests View All Students by School Staff StudentList

    // DJWhite 4 Jan 2012: Default null or empty to NONE which is equivalent to ALL.
    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolStaffStudentListAllValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolStaffStudentListMetricsBasedWatchListValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolStaffStudentListNullValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolStaffStudentListEmptyValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
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

    public class WhenCurrentUserHasRequestsViewAllStudentsClaimSchoolStaffStudentListAllNoClaim : WhenCurrentUserRequestsViewAllStudentsClaimNone
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolStaffStudentListAllInvalidStaff : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
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
            ExpectGetSchoolStaffInvalid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoStaffPermission();
        }
    }

    // The Section Case:
    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolStaffStudentListSectionValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolStaffStudentListSectionTeacherSectionInvalid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
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
            //ExpectGetSchoolSectionValid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoSectionPermission();
        }
    }

    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolStaffStudentListSectionSchoolSectionInvalid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
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
    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolStaffStudentListCohortValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolStaffStudentListCohortStaffCohortInvalid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolStaffStudentListCohortSchoolCohortInvalid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
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

    // The None Case:
    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolStaffStudentListNoneValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStaffStudentList(securityAssertionProvider, null);
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
    #endregion

    #region Current User Requests View All Students by LocalEducationAgency School Staff StudentList

    #region With LEA, School, and Staff

    // DJWhite 4 Jan 2012: Default null or empty to NONE which is equivalent to ALL.
    public class WhenCurrentUserWithSchoolRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListListAllValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
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
            ExpectGetSchoolStaffValid();
            ExpectGetSchoolIdName();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserWithSchoolRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListListMetricsBasedWatchListValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.MetricsBasedWatchList.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolStaffValid();
            ExpectGetSchoolIdName();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserWithSchoolRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListNullValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
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
            ExpectGetSchoolStaffValid();
            ExpectGetSchoolIdName();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserWithSchoolRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListEmptyValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
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
            ExpectGetSchoolStaffValid();
            ExpectGetSchoolIdName();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserWithSchoolHasRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListAllNoClaim : WhenCurrentUserRequestsViewAllStudentsClaimNone
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
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

    public class WhenCurrentUserWithSchoolRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListAllInvalidStaff : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
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
            ExpectGetSchoolStaffInvalid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoStaffPermission();
        }
    }

    // The Section Case:
    public class WhenCurrentUserWithSchoolRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListSectionValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
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
            ExpectGetSchoolIdName();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserWithSchoolRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListSectionTeacherSectionInvalid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.Section.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetTeacherSectionInvalid();
            //ExpectGetSchoolSectionValid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoSectionPermission();
        }
    }

    public class WhenCurrentUserWithSchoolRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListSectionSchoolSectionInvalid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
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
    public class WhenCurrentUserWithSchoolRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListCohortValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
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
            ExpectGetStaffCohortsValid();
            ExpectGetEducationOrganizationCohortValid();
            ExpectGetSchoolIdName();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserWithSchoolRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListCohortStaffCohortInvalid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_00);
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

    public class WhenCurrentUserWithSchoolRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListSchoolCohortInvalid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
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
    public class WhenCurrentUserWithSchoolRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListNoneValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
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
            ExpectGetSchoolStaffValid();
            ExpectGetSchoolIdName();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    #endregion

    #region With LEA and Staff

    // DJWhite 4 Jan 2012: Default null or empty to NONE which is equivalent to ALL.
    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListListAllValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListListMetricsBasedWatchListValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListNullValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListEmptyValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
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

    public class WhenCurrentUserHasRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListAllNoClaim : WhenCurrentUserRequestsViewAllStudentsClaimNone
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListAllInvalidStaff : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
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

    // The Section Case:
    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListSectionValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
            yield return GetStudentListTypeParameter(StudentListType.Section.ToString());
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationFail(typeof(UserAccessDeniedException));
        }
    }

    // The Cohort Case:
    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListCohortValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListCohortStaffCohortInvalid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_00);
            yield return GetSchoolIdNullableParameter(null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListSchoolCohortInvalid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
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
    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListNoneValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
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

    #region With LEA and School

    // DJWhite 4 Jan 2012: Default null or empty to NONE which is equivalent to ALL.
    public class WhenCurrentUserWithSchoolWithStaffNullRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListListAllValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSINullableParameter(null);
            yield return GetStudentListTypeParameter(StudentListType.All.ToString());
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserWithSchoolWithStaffNullRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListListMetricsBasedWatchListValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSINullableParameter(null);
            yield return GetStudentListTypeParameter(StudentListType.MetricsBasedWatchList.ToString());
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserWithSchoolWithStaffNullRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListNullValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSINullableParameter(null);
            yield return GetStudentListTypeParameter(null);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserWithSchoolWithStaffNullRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListEmptyValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSINullableParameter(null);
            yield return GetStudentListTypeParameter("");
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserWithSchoolWithStaffNullHasRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListAllNoClaim : WhenCurrentUserRequestsViewAllStudentsClaimNone
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSINullableParameter(null);
            yield return GetStudentListTypeParameter(StudentListType.All.ToString());
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }

    // The Section Case:
    public class WhenCurrentUserWithSchoolWithStaffNullRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListSectionValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSINullableParameter(null);
            yield return GetStudentListTypeParameter(StudentListType.Section.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolSectionValid();
            ExpectGetSchoolIdName();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserWithSchoolWithStaffNullRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListSectionSchoolSectionInvalid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSINullableParameter(null);
            yield return GetStudentListTypeParameter(StudentListType.Section.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolSectionInvalid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoSectionPermission();
        }
    }

    // The Cohort Case:
    public class WhenCurrentUserWithSchoolWithStaffNullRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListCohortValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
            yield return GetStaffUSINullableParameter(null);
            yield return GetStudentListTypeParameter(StudentListType.Cohort.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetEducationOrganizationCohortValid();
            ExpectGetSchoolIdName();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserWithSchoolWithStaffNullRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListSchoolCohortInvalid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
            yield return GetStaffUSINullableParameter(null);
            yield return GetStudentListTypeParameter(StudentListType.Cohort.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetEducationOrganizationCohortInvalid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoCohortPermission();
        }
    }

    // The None Case:
    public class WhenCurrentUserWithSchoolWithStaffNullRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListNoneValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSINullableParameter(null);
            yield return GetStudentListTypeParameter(StudentListType.None.ToString());
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    #endregion

    #region With LEA Only

    // DJWhite 4 Jan 2012: Default null or empty to NONE which is equivalent to ALL.
    public class WhenCurrentUserWithStaffNullRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListListMetricsBasedWatchListValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSINullableParameter(null);
            yield return GetStudentListTypeParameter(StudentListType.MetricsBasedWatchList.ToString());
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserWithStaffNullRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListListAllValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSINullableParameter(null);
            yield return GetStudentListTypeParameter(StudentListType.All.ToString());
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserWithStaffNullRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListNullValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSINullableParameter(null);
            yield return GetStudentListTypeParameter(null);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserWithStaffNullRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListEmptyValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSINullableParameter(null);
            yield return GetStudentListTypeParameter("");
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserWithStaffNullHasRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListAllNoClaim : WhenCurrentUserRequestsViewAllStudentsClaimNone
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSINullableParameter(null);
            yield return GetStudentListTypeParameter(StudentListType.All.ToString());
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }

    // The Section Case:
    public class WhenCurrentUserWithStaffNullRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListSectionValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSINullableParameter(null);
            yield return GetStudentListTypeParameter(StudentListType.Section.ToString());
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationFail(typeof(UserAccessDeniedException));
        }
    }

    // The Cohort Case:
    public class WhenCurrentUserWithStaffNullRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListCohortValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
            yield return GetStaffUSINullableParameter(null);
            yield return GetStudentListTypeParameter(StudentListType.Cohort.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetEducationOrganizationCohortValid(LOCALEDUCATIONAGENCY_ID_01);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserWithStaffNullRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListSchoolCohortInvalid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetSectionOrCohortIdParameter(COHORT_01_01);
            yield return GetStaffUSINullableParameter(null);
            yield return GetStudentListTypeParameter(StudentListType.Cohort.ToString());
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetEducationOrganizationCohortInvalid(LOCALEDUCATIONAGENCY_ID_01);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoCohortPermission();
        }
    }

    // The None Case:
    public class WhenCurrentUserWithStaffNullRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListNoneValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSINullableParameter(null);
            yield return GetStudentListTypeParameter(StudentListType.None.ToString());
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    #endregion

    #region With Nothing

    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffSectionStudentListListNoLocalEducationAgencyNorSchool : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(0);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetSectionOrCohortIdParameter(SECTION_01_01);
            yield return GetStaffUSINullableParameter(null);
            yield return GetStudentListTypeParameter(StudentListType.All.ToString());
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailInvalidId();
        }
    }


    #endregion

    #endregion

    #region  Current User Requests View All Students by School Section
    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolSectionValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolSection(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionIdParameter(SECTION_01_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolSectionValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolSectionNoClaim : WhenCurrentUserRequestsViewAllStudentsClaimNone
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolSection(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionIdParameter(SECTION_01_01);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }

    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolSectionInvalidSection : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolSection(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetSectionIdParameter(SECTION_01_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolSectionInvalid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoSectionPermission();
        }
    }
    #endregion

    #region  Current User Requests View All Students by School Student
    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolStudentValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStudent(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStudentUSIParameter(STUDENT_01_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolStudentsValid();
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolStudentNoClaim : WhenCurrentUserRequestsViewAllStudentsClaimNone
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStudent(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimSchoolStudentInvalidStudents : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorSchoolStudent(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStudentUSIParameter(STUDENT_01_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolStudentsInvalid();
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoStudentPermission();
        }
    }
    #endregion

    #region  Current User Requests View All Students by Staff
    public class WhenCurrentUserRequestsViewAllStudentsClaimStaffValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorStaff(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetStaffUSIParameter(TEACHER_USI_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            //Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllStudents, SCH_ID_01_01)).Return(true);
            ExpectGetAssociatedEducationOrganizationsValidValid(EdFiClaimTypes.ViewAllStudents);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewAllStudentsClaimStaffNoClaim : WhenCurrentUserRequestsViewAllStudentsClaimNone
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorStaff(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetStaffUSIParameter(TEACHER_USI_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetAssociatedEducationOrganizationsInvalidInvalid(EdFiClaimTypes.ViewAllStudents);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoStaffPermission();
        }
    }

    public class WhenCurrentUserRequestsViewAllStudentsClaimStaffInvalidOrganization : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorStaff(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetStaffUSIParameter(TEACHER_USI_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            /*Expect.Call(
                currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                    EdFiClaimTypes.ViewAllStudents, SCH_ID_02_01)).Return(false);
            Expect.Call(
                currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                    EdFiClaimTypes.ViewAllStudents, SCH_ID_02_02)).Return(false);*/
            ExpectGetAssociatedEducationOrganizationsInvalidInvalid(EdFiClaimTypes.ViewAllStudents);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoStaffPermission();
        }
    }
    #endregion

    #region  Current User Requests View All Students by LocalEducationAgency School Student
    // This is only going to test a Valid LocalEducationAgency, and an Invalid LocalEducationAgency.  All other cases are already tested elsewhere.
    //
    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencySchoolStudentLocalEducationAgencyValid : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStudent(securityAssertionProvider, null);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(_schoolIdNameService.Get(null)).IgnoreArguments().Return(new IdNameModel { LocalEducationAgencyId = LOCALEDUCATIONAGENCY_ID_01, SchoolId = SCH_ID_01_01 });
            mySecurityAssertionProvider.SetSchoolIdNameService(_schoolIdNameService);
            ExpectGetSchoolStudentsValid();
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStudentUSIParameter(STUDENT_01_01);
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencySchoolStudentLocalEducationAgencyNoClaim : WhenCurrentUserRequestsViewAllStudentsClaimNone
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStudent(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencySchoolStudentLocalEducationAgencyInvalidSchoolStudents : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStudent(securityAssertionProvider, null);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolStudentsInvalid();
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
            ExpectValidationFailNoStudentPermission();
        }
    }
    #endregion

    #region Current User Requests View All Students by Local Education Agency Staff StudentList

    // DJWhite 4 Jan 2012: Default null or empty to NONE which is equivalent to ALL.
    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencyStaffStudentListAllValid : WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencyStaffStudentList(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencyStaffStudentListNullValid : WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencyStaffStudentList(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencyStaffStudentListEmptyValid : WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencyStaffStudentList(securityAssertionProvider, null);
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

    public class WhenCurrentUserHasRequestsViewAllStudentsClaimLocalEducationAgencyStaffStudentListAllNoClaim : WhenCurrentUserRequestsViewAllStudentsClaimNone
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencyStaffStudentList(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencyStaffStudentListAllInvalidStaff : WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencyStaffStudentList(securityAssertionProvider, null);
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
    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencyStaffStudentListCohortValid : WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencyStaffStudentList(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencyStaffStudentListCohortStaffCohortInvalid : WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencyStaffStudentList(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencyStaffStudentListCohortSchoolCohortInvalid : WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencyStaffStudentList(securityAssertionProvider, null);
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
    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencyStaffStudentListNoneValid : WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencyStaffStudentList(securityAssertionProvider, null);
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

    #region  Current User Requests View All Students by Local Education Agency School Staff
    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffClaimLocalEducationAgency : WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaff(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffClaimNone : WhenCurrentUserRequestsViewAllStudentsClaimNone
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaff(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffClaimSchool : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaff(securityAssertionProvider, null);
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
            Expect.Call(_schoolIdNameService.Get(null)).IgnoreArguments().Return(new IdNameModel {LocalEducationAgencyId = LOCALEDUCATIONAGENCY_ID_01, SchoolId = SCH_ID_01_01});
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffClaimNoneForSchool : WhenCurrentUserRequestsViewAllStudentsClaimNone
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaff(securityAssertionProvider, null);
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

    #region  Current User Requests View All Students by Local Education Agency School Staff CustomStudentList

    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffCustomStudentListClaimLocalEducationAgency : WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffCustomStudentList(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffCustomStudentListClaimInvalidStudentListLocalEducationAgency : WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgency
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffCustomStudentList(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffCustomStudentListClaimNone : WhenCurrentUserRequestsViewAllStudentsClaimNone
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffCustomStudentList(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffCustomStudentListClaimSchool : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffCustomStudentList(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffCustomStudentListClaimInvalidStudentListSchool : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffCustomStudentList(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllStudentsClaimLocalEducationAgencySchoolStaffCustomStudentListClaimNoneForSchool : WhenCurrentUserRequestsViewAllStudentsClaimNone
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffCustomStudentList(securityAssertionProvider, null);
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

    public class GetSignatures : WhenCurrentUserRequestsViewAllStudentsClaimSchool
    {

        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllStudentsClaimValidatorLocalEducationAgency(securityAssertionProvider, null);
        }


        protected override void EstablishContext()
        {
            //CheckValidatorName();
            CreateSecurityMocks(mocks);
            LoginCurrentUserHasSchoolLevelClaims();
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetCohortIdParameter(COHORT_01_01);
        }

        protected void WriteClaims()
        {
            Console.WriteLine("\n\nFields:");
            var claims = GetSupportedClaimNames();
            foreach (var claim in claims)
            {
                Console.WriteLine(claim);
            }

            Console.WriteLine("\n\n");
        }

        public void WriteClaimChains()
        {

            var assemblyForSecurity = Assembly.GetAssembly(typeof(ClaimValidatorBase));
            var allTypesForSecurity = assemblyForSecurity.GetTypes();

            var validatorTypes = from t in allTypesForSecurity
                                 where (!t.IsAbstract)
                                 select t;

            var parmValidatorKeysByClaimName = GetValidatorTypeChainByClaim(validatorTypes);

            var keys = parmValidatorKeysByClaimName.Keys.OrderBy(n => n);
            foreach (var vals in keys.Select(key => parmValidatorKeysByClaimName[key]))
            {
                Console.WriteLine(vals);
            }
        }

        private static Dictionary<string, IEnumerable<Type>> GetValidatorTypeChainByClaim(IEnumerable<Type> securityTypes)
        {
            var claimNames = GetSupportedClaimNames();
            var validatorTypeChainByClaim = new Dictionary<string, IEnumerable<Type>>();

            // Register decorator chains for each claim, based on conventions
            claimNames.Each(claimName =>
            {
                // Get all the claim parameter validators for this claim, by convention
                var decoratorTypes =
                    (from t in securityTypes
                     where t.Name.StartsWith(claimName + "ClaimValidator")
                     select t)
                        .ToList();

                var sortedValidators = decoratorTypes.OrderBy(x => x.Name);
                validatorTypeChainByClaim[claimName] = sortedValidators;
            });
            return validatorTypeChainByClaim;
        }

        protected static IEnumerable<string> GetSupportedClaimNames()
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

        protected override void ExecuteTest()
        {
            myClaimValidator = CreateValidator(mySecurityAssertionProvider);

            WriteClaims();
            WriteClaimChains();

            var allTypes = Assembly.GetAssembly(typeof(IService<BriefRequest, BriefModel>)).GetTypes();
            var keys = new HashSet<string>();

            var serviceTypes =
                from serviceType in allTypes
                where serviceType.Name.StartsWith("I") && serviceType.Name.EndsWith("Service")
                select serviceType;

            var omitted = new HashSet<string>
                              {
                                  "metricId",
                                  "periodId",
                                  "subject",
                                  "subjectArea",
                                  "configurationModel",
                                  "schoolMetricId",
                                  "localEducationAgencyMetricId",
                                  "viewType"
                              };

            foreach (var serviceType in serviceTypes)
            {
                foreach (var methodInfo in serviceType.GetMethods())
                {
                    var foo = (methodInfo.GetParameters().Select(p => p.Name)
                        .Where(name => (!omitted.Contains(name)))
                        .OrderBy(name => name)).ToList();
                    if (foo.Count <= 0) continue;

                    if (foo.Contains("matchContains")) continue;

                    //if ((foo.Contains("studentMetricId")) ||
                    //    (foo.Contains("schoolMetricId")))
                    //{
                    //    Console.Out.WriteLine(serviceType.Name + ", " + methodInfo.Name);
                    //    foreach (var temp in foo)
                    //    {
                    //        Console.Out.WriteLine("\t" + temp);
                    //    }
                    //}

                    keys.Add(string.Join("|", foo));
                }
            }

            Console.Out.WriteLine("\nSignatures:");
            foreach (var key in keys.ToList().OrderBy(s => s))
            {
                Console.Out.WriteLine("\t" + key);
            }
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

}
