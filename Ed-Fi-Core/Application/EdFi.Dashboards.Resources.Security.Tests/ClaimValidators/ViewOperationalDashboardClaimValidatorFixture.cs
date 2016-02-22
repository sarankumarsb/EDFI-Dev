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
    #region abstract Classes ViewOperationalDashboardClaimValidatorFixture
    public abstract class ViewOperationalDashboardClaimValidatorFixture : AuthorizationFixtureBase
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
                            EdFiClaimTypes.ViewOperationalDashboard, educationOrganization)).Return(false).Repeat.Any();
                }
                else
                {
                    Expect.Call(
                        currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                            EdFiClaimTypes.ViewOperationalDashboard, educationOrganization)).Return(true).Repeat.Any();
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
                            EdFiClaimTypes.ViewOperationalDashboard, educationOrganization)).Return(false).Repeat.Any();
                }
                else
                {
                    Expect.Call(
                        currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                            EdFiClaimTypes.ViewOperationalDashboard, educationOrganization)).Return(true).Repeat.Any();
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
    public abstract class WhenCurrentUserHasLocalEducationAgencyLevelViewOperationalDashboardClaim : ViewOperationalDashboardClaimValidatorFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasLocalEducationAgencyLevelClaims();
            base.EstablishContext();
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }

    public abstract class WhenCurrentUserHasSchoolLevelViewOperationalDashboardClaim : ViewOperationalDashboardClaimValidatorFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasSchoolLevelClaims();
            base.EstablishContext();
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }

    public abstract class WhenCurrentUserHasNoViewOperationalDashboardClaim : ViewOperationalDashboardClaimValidatorFixture
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
    public class WhenCurrentUserRequestsViewOperationalDashboardClaimLocalEducationAgencyHasLocalEducationAgencyClaim : WhenCurrentUserHasLocalEducationAgencyLevelViewOperationalDashboardClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewOperationalDashboardClaimValidatorLocalEducationAgency(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewOperationalDashboardClaimLocalEducationAgencyHasSchoolClaim : WhenCurrentUserHasSchoolLevelViewOperationalDashboardClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewOperationalDashboardClaimValidatorLocalEducationAgency(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewOperationalDashboardClaimLocalEducationAgencyHasNoClaim : WhenCurrentUserHasNoViewOperationalDashboardClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewOperationalDashboardClaimValidatorLocalEducationAgency(securityAssertionProvider, null);
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
    public class WhenCurrentUserRequestsViewOperationalDashboardClaimSchoolHasLocalEducationAgencyClaim : WhenCurrentUserHasLocalEducationAgencyLevelViewOperationalDashboardClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewOperationalDashboardClaimValidatorSchool(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewOperationalDashboardClaimSchoolHasSchoolClaim : WhenCurrentUserHasSchoolLevelViewOperationalDashboardClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewOperationalDashboardClaimValidatorSchool(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewOperationalDashboardClaimSchoolHasNoClaim : WhenCurrentUserHasNoViewOperationalDashboardClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewOperationalDashboardClaimValidatorSchool(securityAssertionProvider, null);
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
}
