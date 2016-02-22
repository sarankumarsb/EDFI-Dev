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
    public abstract class ManageGoalsClaimValidatorFixture : AuthorizationFixtureBase
    {
        protected override void ExecuteTest()
        {
            try
            {
                myClaimValidator = CreateValidator(mySecurityAssertionProvider);
                myClaimValidator.ProcessRequest( GetClaimValidatorRequest());
            }
            catch (Exception ex)
            {
                myException = ex;
            }
        }
    }

    public abstract class WhenCurrentUserHasLocalEducationAgencyLevelManageGoalsClaim : ManageGoalsClaimValidatorFixture
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
                        EdFiClaimTypes.ManageGoals, educationOrganization)).Return(true).Repeat.Any();
            }
            educationOrganizationParamaterInstance = context.FirstOrDefault(p =>
                    p.Name.Equals(ClaimValidatorRequest.LocalEducationAgencyParameterName,
                                  StringComparison.OrdinalIgnoreCase));

            if (educationOrganizationParamaterInstance != null && educationOrganizationParamaterInstance.Value != null)
            {
                var educationOrganization = (int)educationOrganizationParamaterInstance.Value;

                Expect.Call(
                    currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                        EdFiClaimTypes.ManageGoals, educationOrganization)).Return(true).Repeat.Any();
            }
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }

    public abstract class WhenCurrentUserHasNoManageGoalsClaim : ManageGoalsClaimValidatorFixture
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

                Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                        EdFiClaimTypes.ManageGoals, educationOrganization)).Return(false).Repeat.Any();
            }
            educationOrganizationParamaterInstance = context.FirstOrDefault(p =>
                    p.Name.Equals(ClaimValidatorRequest.LocalEducationAgencyParameterName,
                                  StringComparison.OrdinalIgnoreCase));

            if (educationOrganizationParamaterInstance != null && educationOrganizationParamaterInstance.Value != null)
            {
                var educationOrganization = (int)educationOrganizationParamaterInstance.Value;

                Expect.Call(
                    currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                        EdFiClaimTypes.ManageGoals, educationOrganization)).Return(false).Repeat.Any();
            }
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }

    #endregion

    #region  Current User Requests Manage Goals by LocalEducationAgency
    public class WhenCurrentUserRequestsManageGoalsClaimLocalEducationAgencyHasLocalEducationAgencyClaim : WhenCurrentUserHasLocalEducationAgencyLevelManageGoalsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ManageGoalsClaimValidatorLocalEducationAgency(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsManageGoalsClaimLocalEducationAgencyHasNoClaim : WhenCurrentUserHasNoManageGoalsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ManageGoalsClaimValidatorLocalEducationAgency(securityAssertionProvider, null);
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

    #region  Current User Requests Manage Goals by LocalEducationAgencyMetric
    public class WhenCurrentUserRequestsManageGoalsClaimLocalEducationAgencyMetricHasLocalEducationAgencyClaim : WhenCurrentUserHasLocalEducationAgencyLevelManageGoalsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ManageGoalsClaimValidatorLocalEducationAgencyMetric(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetMetricIdParameter(0);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsManageGoalsClaimLocalEducationAgencyMetricHasNoClaim : WhenCurrentUserHasNoManageGoalsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ManageGoalsClaimValidatorLocalEducationAgencyMetric(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetMetricIdParameter(0);
        }

        [Test]
        public void ShouldFailNoLocalEducationAgencyPermission()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }
    #endregion

    #region  Current User Requests Manage Goals by School
    public class WhenCurrentUserRequestsManageGoalsClaimSchoolHasLocalEducationAgencyClaim : WhenCurrentUserHasLocalEducationAgencyLevelManageGoalsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ManageGoalsClaimValidatorSchool(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsManageGoalsClaimSchoolHasNoClaim : WhenCurrentUserHasNoManageGoalsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ManageGoalsClaimValidatorSchool(securityAssertionProvider, null);
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