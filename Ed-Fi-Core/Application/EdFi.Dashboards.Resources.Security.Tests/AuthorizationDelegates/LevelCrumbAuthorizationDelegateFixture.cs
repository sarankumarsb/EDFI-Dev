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
    #region abstract Classes ViewNavigationClaimValidatorFixture
    public abstract class LevelCrumbAuthorizationDelegateFixture : AuthorizationFixtureBase
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

    public abstract class WhenCurrentUserLevelCrumbAuthorizationDelegateLocalEducationAgency : LevelCrumbAuthorizationDelegateFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasLocalEducationAgencyLevelClaims();
            base.EstablishContext();
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }

    public abstract class WhenCurrentUserLevelCrumbAuthorizationDelegateSchool : LevelCrumbAuthorizationDelegateFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasSchoolLevelClaims();
            base.EstablishContext();
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }

    public abstract class WhenCurrentUserLevelCrumbAuthorizationDelegateNone : LevelCrumbAuthorizationDelegateFixture
    {
        protected override void EstablishContext()
        {
            LoginCurrentUserHasNoClaims();
            base.EstablishContext();
            ExpectGetLocalEducationOrganizationHierarchyAny();
        }
    }
    #endregion

    #region Valid LocalEducationAgency, Null School, Null Staff.
    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffValidLocalEducationAgencyWithClaimLocalEducationAgency : WhenCurrentUserLevelCrumbAuthorizationDelegateLocalEducationAgency
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetStaffUSINullableParameter(null);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffValidLocalEducationAgencyWithClaimSchool : WhenCurrentUserLevelCrumbAuthorizationDelegateSchool
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetStaffUSINullableParameter(null);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffValidLocalEducationAgencyWithClaimNone : WhenCurrentUserLevelCrumbAuthorizationDelegateNone
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetStaffUSINullableParameter(null);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }
    #endregion

    #region Valid LocalEducationAgency, Null School, Zero Staff USI.
    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffValidLocalEducationAgencyZeroStaffWithClaimLocalEducationAgency : WhenCurrentUserLevelCrumbAuthorizationDelegateLocalEducationAgency
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetStaffUSINullableParameter(0);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffValidLocalEducationAgencyZeroStaffWithClaimSchool : WhenCurrentUserLevelCrumbAuthorizationDelegateSchool
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetStaffUSINullableParameter(0);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffValidLocalEducationAgencyZeroStaffWithClaimNone : WhenCurrentUserLevelCrumbAuthorizationDelegateNone
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetStaffUSINullableParameter(0);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }
    #endregion

    #region Valid LocalEducationAgency, Valid School, Null Staff
    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffValidLocalEducationAgencySchoolWithClaimLocalEducationAgency : WhenCurrentUserLevelCrumbAuthorizationDelegateLocalEducationAgency
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(SCH_ID_01_01);
            yield return GetStaffUSINullableParameter(null);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffValidLocalEducationAgencySchoolWithClaimSchool : WhenCurrentUserLevelCrumbAuthorizationDelegateSchool
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(SCH_ID_01_01);
            yield return GetStaffUSINullableParameter(null);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffValidLocalEducationAgencySchoolWithClaimNone : WhenCurrentUserLevelCrumbAuthorizationDelegateNone
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(SCH_ID_01_01);
            yield return GetStaffUSINullableParameter(null);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }
    #endregion

    #region Valid LocalEducationAgency, Valid School, Valid Staff.
    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffValidLocalEducationAgencyValidSchoolValidStaffWithClaimLocalEducationAgency : WhenCurrentUserLevelCrumbAuthorizationDelegateLocalEducationAgency
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolStaffValid();
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(SCH_ID_01_01);
            yield return GetStaffUSINullableParameter(TEACHER_USI_01);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffValidLocalEducationAgencyValidSchoolValidStaffWithClaimSchool : WhenCurrentUserLevelCrumbAuthorizationDelegateSchool
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(SCH_ID_01_01);
            yield return GetStaffUSINullableParameter(TEACHER_USI_01);
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

    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffValidLocalEducationAgencyValidSchoolValidStaffWithClaimNone : WhenCurrentUserLevelCrumbAuthorizationDelegateNone
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(SCH_ID_01_01);
            yield return GetStaffUSINullableParameter(TEACHER_USI_01);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }
    #endregion

    #region Valid LocalEducationAgency, Valid School, Invalid Staff.
    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffValidLocalEducationAgencyValidSchoolInvalidStaffWithClaimLocalEducationAgency : WhenCurrentUserLevelCrumbAuthorizationDelegateLocalEducationAgency
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolStaffInvalid();
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(SCH_ID_01_01);
            yield return GetStaffUSINullableParameter(TEACHER_USI_01);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoStaffPermission();
        }
    }

    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffValidLocalEducationAgencyValidSchoolInvalidStaffWithClaimSchool : WhenCurrentUserLevelCrumbAuthorizationDelegateSchool
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            ExpectGetSchoolStaffInvalid();
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(SCH_ID_01_01);
            yield return GetStaffUSINullableParameter(TEACHER_USI_01);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoStaffPermission();
        }
    }

    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffValidLocalEducationAgencyValidSchoolInvalidStaffWithClaimNone : WhenCurrentUserLevelCrumbAuthorizationDelegateNone
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(SCH_ID_01_01);
            yield return GetStaffUSINullableParameter(TEACHER_USI_01);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }
    #endregion

    #region Null LocalEducationAgency, Valid School, Null Staff
    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffNullLocalEducationAgencyValidSchoolWithClaimLocalEducationAgency : WhenCurrentUserLevelCrumbAuthorizationDelegateLocalEducationAgency
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(null);
            yield return GetSchoolIdNullableParameter(SCH_ID_01_01);
            yield return GetStaffUSINullableParameter(null);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailInvalidParameter();
        }
    }

    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffNullLocalEducationAgencyValidSchoolWithClaimSchool : WhenCurrentUserLevelCrumbAuthorizationDelegateSchool
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(null);
            yield return GetSchoolIdNullableParameter(SCH_ID_01_01);
            yield return GetStaffUSINullableParameter(null);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailInvalidParameter();
        }
    }

    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffNullLocalEducationAgencyValidSchoolWithClaimNone : WhenCurrentUserLevelCrumbAuthorizationDelegateNone
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(SCH_ID_01_01);
            yield return GetStaffUSINullableParameter(null);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }
    #endregion

    #region Valid LocalEducationAgency, Null School, Valid Staff.
    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffValidLocalEducationAgencyNullSchoolValidStaffWithClaimLocalEducationAgency : WhenCurrentUserLevelCrumbAuthorizationDelegateLocalEducationAgency
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetStaffUSINullableParameter(TEACHER_USI_01);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffValidLocalEducationAgencyNullSchoolValidStaffWithClaimSchool : WhenCurrentUserLevelCrumbAuthorizationDelegateSchool
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetStaffUSINullableParameter(TEACHER_USI_01);
        }

        [Test]
        public void ShouldPass()
        {
            ExpectValidationPass();
        }
    }

    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffValidLocalEducationAgencyNullSchoolValidStaffWithClaimNone : WhenCurrentUserLevelCrumbAuthorizationDelegateNone
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetStaffUSINullableParameter(TEACHER_USI_01);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }
    #endregion

    #region Valid LocalEducationAgency, Invalid School, Null Staff.
    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffValidLocalEducationAgencyInvalidSchoolWithClaimLocalEducationAgency : WhenCurrentUserLevelCrumbAuthorizationDelegateLocalEducationAgency
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(SCH_ID_02_01);
            yield return GetStaffUSINullableParameter(null);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailInvalidParameter();
        }
    }

    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffValidLocalEducationAgencyInvalidSchoolWithClaimSchool : WhenCurrentUserLevelCrumbAuthorizationDelegateSchool
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(SCH_ID_02_01);
            yield return GetStaffUSINullableParameter(null);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailInvalidParameter();
        }
    }

    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffValidLocalEducationAgencyInvalidSchoolWithClaimNone : WhenCurrentUserLevelCrumbAuthorizationDelegateNone
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_01);
            yield return GetSchoolIdNullableParameter(SCH_ID_02_01);
            yield return GetStaffUSINullableParameter(null);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }
    #endregion

    #region Invalid LocalEducationAgency, Null School, Null Staff.
    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffInvalidLocalEducationAgencyWithClaimLocalEducationAgency : WhenCurrentUserLevelCrumbAuthorizationDelegateLocalEducationAgency
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_00);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetStaffUSINullableParameter(null);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }

    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffInvalidLocalEducationAgencyWithClaimSchool : WhenCurrentUserLevelCrumbAuthorizationDelegateSchool
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_00);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetStaffUSINullableParameter(null);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }

    public class WhenCurrentUserRequestsLevelCrumbAuthorizationDelegateLocalEducationAgencySchoolStaffInvalidLocalEducationAgencyWithClaimNone : WhenCurrentUserLevelCrumbAuthorizationDelegateNone
    {
        protected override IAuthorizationDelegate CreateAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider)
        {
            return new LevelCrumbAuthorizationDelegate(securityAssertionProvider);
        }

        protected override IEnumerable<ParameterInstance> GetSuppliedParameters()
        {
            yield return GetLocalEducationAgencyIdNullableParameter(LOCALEDUCATIONAGENCY_ID_00);
            yield return GetSchoolIdNullableParameter(null);
            yield return GetStaffUSINullableParameter(null);
        }

        [Test]
        public void ShouldFail()
        {
            ExpectValidationFailNoEducationOrganizationPermission();
        }
    }
    #endregion

}
