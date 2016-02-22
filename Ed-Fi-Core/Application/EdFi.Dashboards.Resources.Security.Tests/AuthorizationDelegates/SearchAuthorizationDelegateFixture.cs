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
    #region abstract Classes SearchAuthorizationDelegateFixture
    public abstract class SearchAuthorizationDelegateFixture : AuthorizationFixtureBase
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

    public abstract class WhenCurrentUserViewSearchClaimLocalEducationAgency : SearchAuthorizationDelegateFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasLocalEducationAgencyLevelClaims();
            base.EstablishContext();
        }
    }

    public abstract class WhenCurrentUserViewSearchClaimSchool : SearchAuthorizationDelegateFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasSchoolLevelClaims();
            base.EstablishContext();
        }
    }

    public abstract class WhenCurrentUserViewSearchClaimNone : SearchAuthorizationDelegateFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasNoClaims();
            base.EstablishContext();
        }
    }
    #endregion

    #region matchContains rowCountToReturn, textToFind.
    public class WhenCurrentUserRequestsViewSearchClaimMatchRowTextWithClaimLocalEducationAgency : WhenCurrentUserViewSearchClaimLocalEducationAgency
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new SearchAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetTextToFindParameter("FOO");
            yield return GetRowCountToReturnParameter(100);
            yield return GetMatchContainsParameter(true);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewSearchClaimMatchRowTextWithClaimSchool : WhenCurrentUserViewSearchClaimSchool
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new SearchAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetTextToFindParameter("FOO");
            yield return GetRowCountToReturnParameter(100);
            yield return GetMatchContainsParameter(true);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewSearchClaimMatchRowTextWithClaimNone : WhenCurrentUserViewSearchClaimNone
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new SearchAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetTextToFindParameter("FOO");
            yield return GetRowCountToReturnParameter(100);
            yield return GetMatchContainsParameter(true);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }
    #endregion
    //(string textToFind, int rowCountToReturn, bool matchContains, string filter)
    #region ISearchService.Get(filter, matchContains, rowCountToReturn, textToFind):
    public class WhenCurrentUserRequestsViewSearchClaimFilterMatchRowTextWithClaimLocalEducationAgency : WhenCurrentUserViewSearchClaimLocalEducationAgency
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new SearchAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetFilterParameter("BAR");
            yield return GetTextToFindParameter("FOO");
            yield return GetRowCountToReturnParameter(100);
            yield return GetMatchContainsParameter(true);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewSearchClaimFilterMatchRowTextWithClaimSchool : WhenCurrentUserViewSearchClaimSchool
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new SearchAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetFilterParameter("BAR");
            yield return GetTextToFindParameter("FOO");
            yield return GetRowCountToReturnParameter(100);
            yield return GetMatchContainsParameter(true);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsViewSearchClaimFilterMatchRowTextWithClaimNone : WhenCurrentUserViewSearchClaimNone
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new SearchAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetFilterParameter("BAR");
            yield return GetTextToFindParameter("FOO");
            yield return GetRowCountToReturnParameter(100);
            yield return GetMatchContainsParameter(true);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }
    #endregion

}
