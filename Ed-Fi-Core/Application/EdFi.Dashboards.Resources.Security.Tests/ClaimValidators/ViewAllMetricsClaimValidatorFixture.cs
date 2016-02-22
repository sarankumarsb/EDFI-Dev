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
    #region abstract Classes ViewAllMetricsClaimValidatorFixture
    public abstract class ViewAllMetricsClaimValidatorFixture : AuthorizationFixtureBase
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
    public abstract class WhenCurrentUserHasLocalEducationAgencyLevelViewAllMetricsClaim : ViewAllMetricsClaimValidatorFixture
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
                        EdFiClaimTypes.ViewAllMetrics, educationOrganization)).Return(true).Repeat.Any();
            }
            educationOrganizationParamaterInstance = context.FirstOrDefault(p =>
                    p.Name.Equals(ClaimValidatorRequest.LocalEducationAgencyParameterName,
                                  StringComparison.OrdinalIgnoreCase));

            if (educationOrganizationParamaterInstance != null && educationOrganizationParamaterInstance.Value != null)
            {
                var educationOrganization = (int)educationOrganizationParamaterInstance.Value;

                Expect.Call(
                    currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                        EdFiClaimTypes.ViewAllMetrics, educationOrganization)).Return(true).Repeat.Any();
            }
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }

    public abstract class WhenCurrentUserHasSchoolLevelViewAllMetricsClaim : ViewAllMetricsClaimValidatorFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasSchoolLevelClaims();
            base.EstablishContext();
            IEnumerable<ParameterInstance> context = GetSuppliedParameters();
            var educationOrganizationParamaterInstance = (context.FirstOrDefault(p => p.Name.Equals(ClaimValidatorRequest.SchoolParameterName, StringComparison.OrdinalIgnoreCase)));
            if (educationOrganizationParamaterInstance != null && educationOrganizationParamaterInstance.Value != null)
            {
                var educationOrganization = (int)educationOrganizationParamaterInstance.Value;

                Expect.Call(
                    currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                        EdFiClaimTypes.ViewAllMetrics, educationOrganization)).Return(true).Repeat.Any();
            }
            educationOrganizationParamaterInstance = context.FirstOrDefault(p =>
                    p.Name.Equals(ClaimValidatorRequest.LocalEducationAgencyParameterName,
                                  StringComparison.OrdinalIgnoreCase));

            if (educationOrganizationParamaterInstance != null && educationOrganizationParamaterInstance.Value != null)
            {
                var educationOrganization = (int)educationOrganizationParamaterInstance.Value;

                Expect.Call(
                    currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                        EdFiClaimTypes.ViewAllMetrics, educationOrganization)).Return(false).Repeat.Any();
            }
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }

    public abstract class WhenCurrentUserHasNoViewAllMetricsClaim : ViewAllMetricsClaimValidatorFixture
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
                        EdFiClaimTypes.ViewAllMetrics, educationOrganization)).Return(false).Repeat.Any();
            }
            educationOrganizationParamaterInstance = context.FirstOrDefault(p =>
                    p.Name.Equals(ClaimValidatorRequest.LocalEducationAgencyParameterName,
                                  StringComparison.OrdinalIgnoreCase));

            if (educationOrganizationParamaterInstance != null && educationOrganizationParamaterInstance.Value != null)
            {
                var educationOrganization = (int)educationOrganizationParamaterInstance.Value;

                Expect.Call(
                    currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                        EdFiClaimTypes.ViewAllMetrics, educationOrganization)).Return(false).Repeat.Any();
            }
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }
    #endregion

    #region  Current User Requests View All Teachers by LocalEducationAgency
    public class WhenCurrentUserRequestsViewAllMetricsClaimLocalEducationAgencyHasLocalEducationAgencyClaim : WhenCurrentUserHasLocalEducationAgencyLevelViewAllMetricsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllMetricsClaimValidatorLocalEducationAgency(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllMetricsClaimLocalEducationAgencyHasSchoolClaim : WhenCurrentUserHasSchoolLevelViewAllMetricsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllMetricsClaimValidatorLocalEducationAgency(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllMetricsClaimLocalEducationAgencyHasNoClaim : WhenCurrentUserHasNoViewAllMetricsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllMetricsClaimValidatorLocalEducationAgency(securityAssertionProvider, null);
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
    public class WhenCurrentUserRequestsViewAllMetricsClaimSchoolHasLocalEducationAgencyClaim : WhenCurrentUserHasLocalEducationAgencyLevelViewAllMetricsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllMetricsClaimValidatorSchool(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllMetricsClaimSchoolHasSchoolClaim : WhenCurrentUserHasSchoolLevelViewAllMetricsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllMetricsClaimValidatorSchool(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewAllMetricsClaimSchoolHasNoClaim : WhenCurrentUserHasNoViewAllMetricsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewAllMetricsClaimValidatorSchool(securityAssertionProvider, null);
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
