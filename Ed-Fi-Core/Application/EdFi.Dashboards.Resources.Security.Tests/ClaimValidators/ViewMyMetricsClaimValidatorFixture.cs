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
    #region abstract Classes ViewMyMetricsClaimValidatorFixture
    public abstract class ViewMyMetricsClaimValidatorFixture : AuthorizationFixtureBase
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

    public abstract class WhenCurrentUserHasLocalEducationAgencyLevelViewMyMetricsClaim : ViewMyMetricsClaimValidatorFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasLocalEducationAgencyLevelClaims();
            base.EstablishContext();
            IEnumerable<ParameterInstance> context = GetSuppliedParameters();
            var educationOrganizationParamaterInstance = (context.FirstOrDefault(p => p.Name.Equals(ClaimValidatorRequest.SchoolParameterName, StringComparison.OrdinalIgnoreCase)));

            if (educationOrganizationParamaterInstance != null &&
                educationOrganizationParamaterInstance.Value != null)
            {
                var educationOrganization = (int)educationOrganizationParamaterInstance.Value;
                Expect.Call(
                    currentUserClaimInterrogator.HasClaimOnEducationOrganization(
                        EdFiClaimTypes.ViewMyMetrics, educationOrganization)).Return(false).Repeat.Any();
            }
            educationOrganizationParamaterInstance = context.FirstOrDefault(p =>
                    p.Name.Equals(ClaimValidatorRequest.LocalEducationAgencyParameterName,
                                  StringComparison.OrdinalIgnoreCase));

            if (educationOrganizationParamaterInstance != null &&educationOrganizationParamaterInstance.Value != null)
            {
                var educationOrganization = (int)educationOrganizationParamaterInstance.Value;
                Expect.Call(currentUserClaimInterrogator.HasClaimOnEducationOrganization(
                        EdFiClaimTypes.ViewMyMetrics, educationOrganization)).Return(true).Repeat.Any();
            }
        }
    }

    public abstract class WhenCurrentUserHasSchoolLevelViewMyMetricsClaim : ViewMyMetricsClaimValidatorFixture
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
                    currentUserClaimInterrogator.HasClaimOnEducationOrganization(
                        EdFiClaimTypes.ViewMyMetrics, educationOrganization)).Return(true).Repeat.Any();
            }
            educationOrganizationParamaterInstance = context.FirstOrDefault(p =>
                    p.Name.Equals(ClaimValidatorRequest.LocalEducationAgencyParameterName,
                                  StringComparison.OrdinalIgnoreCase));

            if (educationOrganizationParamaterInstance != null && educationOrganizationParamaterInstance.Value != null)
            {
                var educationOrganization = (int)educationOrganizationParamaterInstance.Value;
                Expect.Call(currentUserClaimInterrogator.HasClaimOnEducationOrganization(
                        EdFiClaimTypes.ViewMyMetrics, educationOrganization)).Return(false).Repeat.Any();
            }
        }
    }

    public abstract class WhenCurrentUserHasNoViewMyMetricsClaim : ViewMyMetricsClaimValidatorFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasNoClaims();
            base.EstablishContext();
            IEnumerable<ParameterInstance> context = GetSuppliedParameters();
            var educationOrganizationParamaterInstance = (context.FirstOrDefault(p => p.Name.Equals(ClaimValidatorRequest.SchoolParameterName, StringComparison.OrdinalIgnoreCase)));

            if (educationOrganizationParamaterInstance != null &&
                educationOrganizationParamaterInstance.Value != null)
            {
                var educationOrganization = (int)educationOrganizationParamaterInstance.Value;
                Expect.Call(
                    currentUserClaimInterrogator.HasClaimOnEducationOrganization(
                        EdFiClaimTypes.ViewMyMetrics, educationOrganization)).Return(false).Repeat.Any();
            }
            educationOrganizationParamaterInstance = context.FirstOrDefault(p =>
                    p.Name.Equals(ClaimValidatorRequest.LocalEducationAgencyParameterName,
                                  StringComparison.OrdinalIgnoreCase));

            if (educationOrganizationParamaterInstance != null && educationOrganizationParamaterInstance.Value != null)
            {
                var educationOrganization = (int)educationOrganizationParamaterInstance.Value;
                Expect.Call(currentUserClaimInterrogator.HasClaimOnEducationOrganization(
                        EdFiClaimTypes.ViewMyMetrics, educationOrganization)).Return(false).Repeat.Any();
            }
        }
    }
    #endregion

    #region  Current User Requests View My Teachers by LocalEducationAgency
    public class WhenCurrentUserRequestsViewMyMetricsClaimLocalEducationAgencyHasLocalEducationAgencyClaim : WhenCurrentUserHasLocalEducationAgencyLevelViewMyMetricsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyMetricsClaimValidatorLocalEducationAgency(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewMyMetricsClaimLocalEducationAgencyHasSchoolClaim : WhenCurrentUserHasSchoolLevelViewMyMetricsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyMetricsClaimValidatorLocalEducationAgency(securityAssertionProvider, null);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdParameter(LOCALEDUCATIONAGENCY_ID_01);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }

    public class WhenCurrentUserRequestsViewMyMetricsClaimLocalEducationAgencyHasNoClaim : WhenCurrentUserHasNoViewMyMetricsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyMetricsClaimValidatorLocalEducationAgency(securityAssertionProvider, null);
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

    #region  Current User Requests View My Teachers by School
    public class WhenCurrentUserRequestsViewMyMetricsClaimSchoolHasLocalEducationAgencyClaim : WhenCurrentUserHasLocalEducationAgencyLevelViewMyMetricsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyMetricsClaimValidatorSchool(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewMyMetricsClaimSchoolHasSchoolClaim : WhenCurrentUserHasSchoolLevelViewMyMetricsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyMetricsClaimValidatorSchool(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsViewMyMetricsClaimSchoolHasNoClaim : WhenCurrentUserHasNoViewMyMetricsClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ViewMyMetricsClaimValidatorSchool(securityAssertionProvider, null);
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
