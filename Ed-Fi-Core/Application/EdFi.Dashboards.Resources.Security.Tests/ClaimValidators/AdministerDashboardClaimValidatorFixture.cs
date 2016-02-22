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
    #region abstract Classes
    public abstract class AdministerDashboardsClaimValidatorFixture : AuthorizationFixtureBase
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

    public abstract class WhenCurrentUserHasLocalEducationAgencyLevelAdministerDashboardsClaim : AdministerDashboardsClaimValidatorFixture
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
                        EdFiClaimTypes.AdministerDashboard, educationOrganization)).Return(true).Repeat.Any();
            }
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }

    public abstract class WhenCurrentUserHasNoAdministerDashboardsClaim : AdministerDashboardsClaimValidatorFixture
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
                        EdFiClaimTypes.AdministerDashboard, educationOrganization)).Return(false).Repeat.Any();
            }
            educationOrganizationParamaterInstance = context.FirstOrDefault(p =>
                    p.Name.Equals(ClaimValidatorRequest.LocalEducationAgencyParameterName,
                                  StringComparison.OrdinalIgnoreCase));

            if (educationOrganizationParamaterInstance != null && educationOrganizationParamaterInstance.Value != null)
            {
                var educationOrganization = (int)educationOrganizationParamaterInstance.Value;

                Expect.Call(
                    currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                        EdFiClaimTypes.AdministerDashboard, educationOrganization)).Return(false).Repeat.Any();
            }
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }
    #endregion

    #region  Current User Requests Administer Dashboards by LocalEducationAgency
    public class WhenCurrentUserRequestsAdministerDashboardClaimLocalEducationAgencyHasLocalEducationAgencyClaim : WhenCurrentUserHasLocalEducationAgencyLevelAdministerDashboardsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AdministerDashboardClaimValidatorLocalEducationAgency(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsAdministerDashboardClaimLocalEducationAgencyHasNoClaim : WhenCurrentUserHasNoAdministerDashboardsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AdministerDashboardClaimValidatorLocalEducationAgency(securityAssertionProvider, null);
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

    #region  Current User Requests Administer Dashboards by School
    public class WhenCurrentUserRequestsAdministerDashboardClaimSchoolHasNoClaim : WhenCurrentUserHasNoAdministerDashboardsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AdministerDashboardClaimValidatorSchool(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsAdministerDashboardClaimSchoolHasLocalEducationAgencyClaim : WhenCurrentUserHasLocalEducationAgencyLevelAdministerDashboardsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AdministerDashboardClaimValidatorSchool(securityAssertionProvider, null);
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
    #endregion

    #region  Current User Requests Administer Dashboards by Staff
    public class WhenCurrentUserRequestsAdministerDashboardClaimStaffValid : WhenCurrentUserHasLocalEducationAgencyLevelAdministerDashboardsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AdministerDashboardClaimValidatorStaff(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetStaffUSIParameter(TEACHER_USI_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            //Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.AdministerDashboard, SCH_ID_01_01)).Return(true);
            //Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.AdministerDashboard, SCH_ID_01_02)).Return(true);
            ExpectGetAssociatedEducationOrganizationsValidValid(EdFiClaimTypes.AdministerDashboard);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsAdministerDashboardClaimStaffInvalid : WhenCurrentUserHasNoAdministerDashboardsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AdministerDashboardClaimValidatorStaff(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetStaffUSIParameter(TEACHER_USI_01);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            //Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.AdministerDashboard, SCH_ID_02_01)).Return(false);
            //Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.AdministerDashboard, SCH_ID_02_02)).Return(false);
            ExpectGetAssociatedEducationOrganizationsInvalidInvalid(EdFiClaimTypes.AdministerDashboard);
        }

        [Test]
        public void ShouldFailNoLocalEducationAgencyPermission()
        {
            ExpectValidationFailNoStaffPermission();
        }
    }
    #endregion

    #region  Current User Requests Administer Dashboards by TitleClaimSet
    public class WhenCurrentUserRequestsAdministerDashboardClaimTitleClaimSetHasLocalEducationAgencyClaim : WhenCurrentUserHasLocalEducationAgencyLevelAdministerDashboardsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AdministerDashboardClaimValidatorTitleClaimSet(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetClaimSetParameter(null);
            yield return GetClaimSetMapsParameter(null);
            yield return GetCurrentOperationParameter(null);
            yield return GetEdOrgPositionTitlesParameter(null);
            yield return GetFileInputStreamParameter(null);
            yield return GetFileNameParameter(null);
            yield return GetIsPostParameter(false);
            yield return GetIsSuccessParameter(false);
            yield return GetLinksParameter(null);
            yield return GetMessagesParameter(null);
            yield return GetPositionTitleParameter(null);
            yield return GetPossibleClaimSetsParameter(null);
            yield return GetResourceUrlParameter(null);
            yield return GetUrlParameter(null);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsAdministerDashboardClaimTitleClaimSetHasNoClaim : WhenCurrentUserHasNoAdministerDashboardsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new AdministerDashboardClaimValidatorTitleClaimSet(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetClaimSetParameter(null);
            yield return GetClaimSetMapsParameter(null);
            yield return GetCurrentOperationParameter(null);
            yield return GetEdOrgPositionTitlesParameter(null);
            yield return GetFileInputStreamParameter(null);
            yield return GetFileNameParameter(null);
            yield return GetIsPostParameter(false);
            yield return GetIsSuccessParameter(false);
            yield return GetLinksParameter(null);
            yield return GetMessagesParameter(null);
            yield return GetPositionTitleParameter(null);
            yield return GetPossibleClaimSetsParameter(null);
            yield return GetResourceUrlParameter(null);
            yield return GetUrlParameter(null);
        }

        [Test]
        public void ShouldFailNoLocalEducationAgencyPermission()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }
    #endregion
}
