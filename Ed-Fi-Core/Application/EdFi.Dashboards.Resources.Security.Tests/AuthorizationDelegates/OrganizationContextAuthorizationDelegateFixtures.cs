// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resources.Security.AuthorizationDelegates;
using EdFi.Dashboards.Resources.Security.ClaimValidators;
using EdFi.Dashboards.Resources.Security.Tests.ClaimValidators;
using NUnit.Framework;

namespace EdFi.Dashboards.Resources.Security.Tests.AuthorizationDelegates
{
    #region abstract Classes 
    public abstract class OrganizationContextAuthorizationDelegateFixtures : AuthorizationFixtureBase
    {
        protected virtual IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return null;
        }

        protected override ClaimValidatorBase CreateValidator(ISecurityAssertionProvider securityAssertionProvider)
        {
            return null;
        }

        protected override void ExecuteTest()
        {
            try
            {
                myAuthorizationDelegate = CreateAuthorizationDelegate(mySecurityAssertionProvider);
                myAuthorizationDelegate.AuthorizeRequest(GetSuppliedParameters());
            }
            catch (Exception ex)
            {
                myException = ex;
            }
        }
    }

    public abstract class WhenCurrentUserOrganizationContextAuthorizationDelegateLocalEducationAgency : OrganizationContextAuthorizationDelegateFixtures
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasLocalEducationAgencyLevelClaims();
            base.EstablishContext();
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }

    public abstract class WhenCurrentUserOrganizationContextAuthorizationDelegateSchool : OrganizationContextAuthorizationDelegateFixtures
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasSchoolLevelClaims();
            base.EstablishContext();
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }

    public abstract class WhenCurrentUserOrganizationContextAuthorizationDelegateNone : OrganizationContextAuthorizationDelegateFixtures
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasNoClaims();
            base.EstablishContext();
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }
    #endregion

    #region LocalEducationAgency Valid
    public class WhenCurrentUserOrganizationContextAuthorizationDelegateLocalEducationAgencyValidWithClaimLocalEducationAgency : WhenCurrentUserOrganizationContextAuthorizationDelegateLocalEducationAgency
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new OrganizationContextAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserOrganizationContextAuthorizationDelegateLocalEducationAgencyValidWithClaimSchool : WhenCurrentUserOrganizationContextAuthorizationDelegateSchool
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new OrganizationContextAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserOrganizationContextAuthorizationDelegateLocalEducationAgencyValidWithClaimNone : WhenCurrentUserOrganizationContextAuthorizationDelegateNone
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new OrganizationContextAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }
    #endregion

    #region LocalEducationAgency Invalid
    public class WhenCurrentUserOrganizationContextAuthorizationDelegateLocalEducationAgencyInvalidWithClaimLocalEducationAgency : WhenCurrentUserOrganizationContextAuthorizationDelegateLocalEducationAgency
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new OrganizationContextAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_00);
        }


        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }

    public class WhenCurrentUserOrganizationContextAuthorizationDelegateLocalEducationAgencyInvalidWithClaimSchool : WhenCurrentUserOrganizationContextAuthorizationDelegateSchool
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new OrganizationContextAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_00);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }

    public class WhenCurrentUserOrganizationContextAuthorizationDelegateLocalEducationAgencyInvalidWithClaimNone : WhenCurrentUserOrganizationContextAuthorizationDelegateNone
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new OrganizationContextAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_00);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }
    #endregion
}
