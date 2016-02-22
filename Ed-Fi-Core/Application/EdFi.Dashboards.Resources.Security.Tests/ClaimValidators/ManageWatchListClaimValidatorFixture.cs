using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Resources.Security;
using EdFi.Dashboards.Resources.Security.ClaimValidators;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using Newtonsoft.Json.Bson;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Security.Tests.ClaimValidators
{
    public abstract class ManageWatchListClaimValidatorFixture : AuthorizationFixtureBase
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

    public abstract class WhenCurrentUserHasLocalEducationAgencyLevelManageWatchListClaim :
        ManageWatchListClaimValidatorFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasLocalEducationAgencyLevelClaims();
            base.EstablishContext();

            var context = GetSuppliedParameters();
            var edOrgParameterInstance =
                context.FirstOrDefault(
                    p => p.Name.Equals(ClaimValidatorRequest.SchoolParameterName, StringComparison.OrdinalIgnoreCase)) ??
                context.FirstOrDefault(
                    p =>
                        p.Name.Equals(ClaimValidatorRequest.LocalEducationAgencyParameterName,
                            StringComparison.OrdinalIgnoreCase));

            if (edOrgParameterInstance != null && edOrgParameterInstance.Value != null)
            {
                var educationOrganization = (int)edOrgParameterInstance.Value;

                Expect.Call(
                    currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                        EdFiClaimTypes.AccessOrganization, educationOrganization)).Return(true).Repeat.Any();
            }

            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }

    public abstract class WhenCurrentUserHasSchoolLevelManageWatchListClaim : ManageWatchListClaimValidatorFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasSchoolLevelClaims();
            base.EstablishContext();
            var context = GetSuppliedParameters();
            var edOrgParameterInstance =
                context.FirstOrDefault(
                    p => p.Name.Equals(ClaimValidatorRequest.SchoolParameterName, StringComparison.OrdinalIgnoreCase)) ??
                context.FirstOrDefault(
                    p =>
                        p.Name.Equals(ClaimValidatorRequest.LocalEducationAgencyParameterName,
                            StringComparison.OrdinalIgnoreCase));

            if (edOrgParameterInstance != null && edOrgParameterInstance.Value != null)
            {
                var educationOrganization = (int)edOrgParameterInstance.Value;

                Expect.Call(
                    currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                        EdFiClaimTypes.AccessOrganization, educationOrganization)).Return(true).Repeat.Any();
            }

            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }

    public abstract class WhenCurrentUserHasNoManageWatchListClaim : ManageWatchListClaimValidatorFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasNoClaims();
            base.EstablishContext();
            var context = GetSuppliedParameters();
            var edOrgParameterInstance =
                context.FirstOrDefault(
                    p => p.Name.Equals(ClaimValidatorRequest.SchoolParameterName, StringComparison.OrdinalIgnoreCase)) ??
                context.FirstOrDefault(
                    p =>
                        p.Name.Equals(ClaimValidatorRequest.LocalEducationAgencyParameterName,
                            StringComparison.OrdinalIgnoreCase));

            if (edOrgParameterInstance != null && edOrgParameterInstance.Value != null)
            {
                var educationOrganization = (int)edOrgParameterInstance.Value;

                Expect.Call(
                    currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                        EdFiClaimTypes.AccessOrganization, educationOrganization)).Return(false).Repeat.Any();
            }

            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }

    public class WhenCurrentUserRequestsManageWatchListClaimLocalEducationAgencySchoolStaffHasLocalEducationAgencyClaim :
        WhenCurrentUserHasLocalEducationAgencyLevelManageWatchListClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ManageWatchListClaimValidatorLocalEducationAgencySchoolStaff(securityAssertionProvider, null);
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

    public class WhenCurrentUserRequestsManageWatchListClaimLocalEducationAgencySchoolStaffHasSchoolClaim :
        WhenCurrentUserHasSchoolLevelManageWatchListClaim
    {
        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new ManageWatchListClaimValidatorLocalEducationAgencySchoolStaff(securityAssertionProvider, null);
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
}
