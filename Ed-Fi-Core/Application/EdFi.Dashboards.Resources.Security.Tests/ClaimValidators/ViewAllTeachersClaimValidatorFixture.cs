// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Resources.Security.ClaimValidators;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Security.Tests.ClaimValidators
{
    #region abstract Classes ViewAllTeachersClaimValidatorFixture
    public abstract class ViewAllTeachersClaimValidatorFixture : AuthorizationFixtureBase
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
                            EdFiClaimTypes.ViewAllTeachers, educationOrganization)).Return(false).Repeat.Any();
                }
                else
                {
                    Expect.Call(
                        currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                            EdFiClaimTypes.ViewAllTeachers, educationOrganization)).Return(true).Repeat.Any();
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
                            EdFiClaimTypes.ViewAllTeachers, educationOrganization)).Return(false).Repeat.Any();
                }
                else
                {
                    Expect.Call(
                        currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                            EdFiClaimTypes.ViewAllTeachers, educationOrganization)).Return(true).Repeat.Any();
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

    public abstract class WhenCurrentUserHasLocalEducationAgencyLevelViewAllTeachersClaim : ViewAllTeachersClaimValidatorFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasLocalEducationAgencyLevelClaims();
            base.EstablishContext();
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }

    public abstract class WhenCurrentUserHasSchoolLevelViewAllTeachersClaim : ViewAllTeachersClaimValidatorFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasSchoolLevelClaims();
            base.EstablishContext();
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }

    public abstract class WhenCurrentUserHasNoViewAllTeachersClaim : ViewAllTeachersClaimValidatorFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasNoClaims();
            base.EstablishContext();
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }
    #endregion

    #region  Current User Requests View All Teachers by LocalEducationAgency
    public class WhenCurrentUserRequestsViewAllTeachersClaimLocalEducationAgencyHasLocalEducationAgencyClaim : WhenCurrentUserHasLocalEducationAgencyLevelViewAllTeachersClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllTeachersClaimValidatorLocalEducationAgency(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllTeachersClaimLocalEducationAgencyHasSchoolClaim : WhenCurrentUserHasSchoolLevelViewAllTeachersClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllTeachersClaimValidatorLocalEducationAgency(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllTeachersClaimLocalEducationAgencyHasNoClaim : WhenCurrentUserHasNoViewAllTeachersClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllTeachersClaimValidatorLocalEducationAgency(securityAssertionProvider, null);
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

    #region  Current User Requests View All Teachers by School
    public class WhenCurrentUserRequestsViewAllTeachersClaimSchoolHasLocalEducationAgencyClaim : WhenCurrentUserHasLocalEducationAgencyLevelViewAllTeachersClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllTeachersClaimValidatorSchool(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllTeachersClaimSchoolHasSchoolClaim : WhenCurrentUserHasSchoolLevelViewAllTeachersClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllTeachersClaimValidatorSchool(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllTeachersClaimSchoolHasNoClaim : WhenCurrentUserHasNoViewAllTeachersClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllTeachersClaimValidatorSchool(securityAssertionProvider, null);
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

    #region  Current User Requests View All Teachers by StaffUsi
    public class WhenCurrentUserRequestsViewAllTeachersClaimStaffUSITwoInvalidInvalidHasSchoolClaim : WhenCurrentUserHasSchoolLevelViewAllTeachersClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllTeachersClaimValidatorStaff(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetStaffUSIParameter(TEACHER_USI_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetAssociatedEducationOrganizationsInvalidInvalid(EdFiClaimTypes.ViewAllTeachers);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoStaffPermission();
        }
    }

    public class WhenCurrentUserRequestsViewAllTeachersClaimStaffUSITwoInvalidValidHasSchoolClaim : WhenCurrentUserHasSchoolLevelViewAllTeachersClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllTeachersClaimValidatorStaff(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetStaffUSIParameter(TEACHER_USI_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetAssociatedEducationOrganizationsInvalidValid(EdFiClaimTypes.ViewAllTeachers);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewAllTeachersClaimStaffUSITwoValidInvalidHasSchoolClaim : WhenCurrentUserHasSchoolLevelViewAllTeachersClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllTeachersClaimValidatorStaff(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetStaffUSIParameter(TEACHER_USI_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetAssociatedEducationOrganizationsValidInvalid(EdFiClaimTypes.ViewAllTeachers);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewAllTeachersClaimStaffUSITwoValidValidHasSchoolClaim : WhenCurrentUserHasSchoolLevelViewAllTeachersClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllTeachersClaimValidatorStaff(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetStaffUSIParameter(TEACHER_USI_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetAssociatedEducationOrganizationsValidValid(EdFiClaimTypes.ViewAllTeachers);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    #endregion

    #region  Current User Requests View All Teachers by School StaffUsi
    public class WhenCurrentUserRequestsViewAllTeachersClaimSchoolStaffNoClaim : WhenCurrentUserHasNoViewAllTeachersClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllTeachersClaimValidatorSchoolStaff(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllTeachersClaimSchoolStaffValid : WhenCurrentUserHasSchoolLevelViewAllTeachersClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllTeachersClaimValidatorSchoolStaff(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllTeachersClaimSchoolStaffInvalid : WhenCurrentUserHasSchoolLevelViewAllTeachersClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllTeachersClaimValidatorSchoolStaff(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetSchoolIdParameter(SCH_ID_01_01);
            yield return GetStaffUSIParameter(TEACHER_USI_01);
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
    #endregion
}
