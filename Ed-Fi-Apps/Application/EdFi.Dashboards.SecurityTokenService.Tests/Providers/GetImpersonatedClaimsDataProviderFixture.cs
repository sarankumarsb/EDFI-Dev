using System.Security;
using System.Xml;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.SecurityTokenService.Authentication.Implementations;
using EdFi.Dashboards.SecurityTokenService.Web.Providers;
using EdFi.Dashboards.Testing;
using Microsoft.IdentityModel.Claims;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Mocks;
using Newtonsoft.Json;

namespace EdFi.Dashboards.SecurityTokenService.Tests.Providers
{
    public abstract class GetImpersonatedClaimsDataProviderFixture : TestFixtureBase
    {
        protected IWimpProvider wimpProvider;
        protected IStaffInformationProvider staffInformationProvider;
        protected IAuthenticationProvider authenticationProvider;
        protected IUserClaimsProvider userClaimsProvider;
        protected GetImpersonatedClaimsDataProvider getImpersonatedClaimsDataProvider;
        protected readonly string staffUSIToImpersonate = "250001";
        protected readonly string targetedUsername = "targetedUserName";
        protected readonly int administerStateId = 1;
        protected readonly int administerLeaId = 2;
        protected readonly string administerStateName = "StateName";
        protected readonly string administerLeaName = "LeaName";
        protected readonly string stateAgencyClaimJsonFormatString = "{{\"stateAgencyId\":\"{0}\",\"educationOrganizationName\":\"{1}\"}}";
        protected readonly string localEducationAgencyIdClaimJsonFormatString = "{{\"localEducationAgencyId\":\"{0}\",\"educationOrganizationName\":\"{1}\"}}";
        
        protected override void EstablishContext()
        {
            wimpProvider = mocks.StrictMock<IWimpProvider>();
            staffInformationProvider = mocks.Stub<IStaffInformationProvider>();
            authenticationProvider = mocks.Stub<IAuthenticationProvider>();
            userClaimsProvider = mocks.Stub<IUserClaimsProvider>();
        }

        protected virtual bool IncludeAdministerStateId()
        {
            return true;
        }

        protected virtual bool IncludeAdministerLeaID()
        {
            return true;
        }

        protected virtual IEnumerable<Claim> GetImpersonatorClaims()
        {
            var result = new List<Claim>();

            if( IncludeAdministerStateId())
                result.Add(new Claim(EdFiClaimTypes.AdministerDashboard, string.Format(stateAgencyClaimJsonFormatString, administerStateId, administerStateName)));

            if (IncludeAdministerLeaID())
                result.Add(new Claim(EdFiClaimTypes.AdministerDashboard, string.Format(localEducationAgencyIdClaimJsonFormatString, administerLeaId, administerLeaName)));

            return result.AsEnumerable();
        }

        protected virtual bool IncludeTargetedUserLEAIds()
        {
            return true;
        }

        protected virtual IEnumerable<Claim> GetApplicationSpecificClaims()
        {
            var result = new List<Claim>();

            if( IncludeTargetedUserLEAIds())
                result.Add(new Claim(EdFiClaimTypes._OrgClaimNamespace, string.Format(localEducationAgencyIdClaimJsonFormatString, administerLeaId, administerLeaName)));

            return result.AsEnumerable();
        }

        protected override void ExecuteTest()
        {
            getImpersonatedClaimsDataProvider = new GetImpersonatedClaimsDataProvider(wimpProvider, 
                staffInformationProvider, authenticationProvider, userClaimsProvider);
        }

        protected virtual void VerifyUserClaimsData(UserClaimsData userClaimData)
        {
            Assert.That(userClaimData.Username, Is.EqualTo(targetedUsername));
            Assert.That(userClaimData.StaffUSI, Is.EqualTo(Convert.ToInt64( staffUSIToImpersonate)));
            var testThisOut1 = userClaimData.Claims.Select(x => x.ToString()).ToList();
            var testThisOut2 = GetApplicationSpecificClaims().Select(x => x.ToString()).ToList();
            CollectionAssert.IsSubsetOf(testThisOut2, testThisOut1);
        }
    }

    public class When_Calling_GetImpersonatedClaimsDataProviderFixture_Claims_Null :
        GetImpersonatedClaimsDataProviderFixture
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_ThrowException()
        {
            var claims = getImpersonatedClaimsDataProvider.GetImpersonatedClaimsData(null);
        }
    }

    public class When_Calling_GetImpersonatedClaimsDataProviderFixture_Missing_GetStaffUSIToImpersonate :
        GetImpersonatedClaimsDataProviderFixture
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(wimpProvider.GetWimp()).Return(null);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_ThrowException()
        {
            var claims = getImpersonatedClaimsDataProvider.GetImpersonatedClaimsData(GetImpersonatorClaims());
        }
    }

    public class When_Calling_GetImpersonatedClaimsDataProviderFixture_Missing_AdministerDashboard :
        GetImpersonatedClaimsDataProviderFixture
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(wimpProvider.GetWimp()).Return(staffUSIToImpersonate);
        }

        protected override bool IncludeAdministerStateId()
        {
            return false;
        }

        protected override bool IncludeAdministerLeaID()
        {
            return false;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "You don't have permissions to impersonate.  This action has been logged.")]
        public void Should_ThrowException()
        {
            var claims = getImpersonatedClaimsDataProvider.GetImpersonatedClaimsData(GetImpersonatorClaims());
        }
    }

    public class When_Calling_GetImpersonatedClaimsDataProviderFixture_Missing_TargetedUserName :
        GetImpersonatedClaimsDataProviderFixture
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(wimpProvider.GetWimp()).Return(staffUSIToImpersonate);
        }

        [Test]
		[ExpectedException(typeof(InvalidOperationException))]
        public void Should_ThrowException()
        {
            var claims = getImpersonatedClaimsDataProvider.GetImpersonatedClaimsData(GetImpersonatorClaims());
        }
    }

    public class When_Calling_GetImpersonatedClaimsDataProviderFixture_DAE :
        GetImpersonatedClaimsDataProviderFixture
    {
        

        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(wimpProvider.GetWimp()).Return(staffUSIToImpersonate);
            Expect.Call(staffInformationProvider.ResolveUsername(authenticationProvider, staffUSIToImpersonate)).Return(targetedUsername);
            Expect.Call(userClaimsProvider.GetApplicationSpecificClaims(targetedUsername, Convert.ToInt32(staffUSIToImpersonate))).Throw(new DashboardsAuthenticationException());
        }

        [Test]
        [ExpectedException(typeof(DashboardsAuthenticationException))]
        public void Should_ThrowException()
        {
            var claims = getImpersonatedClaimsDataProvider.GetImpersonatedClaimsData(GetImpersonatorClaims());
        }
    }

    public class When_Calling_GetImpersonatedClaimsDataProviderFixture_No_StateId_NoLeaOverlap :
        GetImpersonatedClaimsDataProviderFixture
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(wimpProvider.GetWimp()).Return(staffUSIToImpersonate);
            Expect.Call(staffInformationProvider.ResolveUsername(authenticationProvider, staffUSIToImpersonate)).Return(targetedUsername);
            Expect.Call(userClaimsProvider.GetApplicationSpecificClaims(targetedUsername, Convert.ToInt32(staffUSIToImpersonate))).Return(this.GetApplicationSpecificClaims());
        }

        protected override bool IncludeAdministerStateId()
        {
            return false;
        }

        protected override bool IncludeAdministerLeaID()
        {
            return false;
        }

        protected virtual bool IncldueTargetedUserLEAIds()
        {
            return false;
        }

        [Test]
		[ExpectedException(typeof(InvalidOperationException))]
        public void Should_ThrowException()
        {
            var claims = getImpersonatedClaimsDataProvider.GetImpersonatedClaimsData(GetImpersonatorClaims());
        }
    }

    public class When_Calling_GetImpersonatedClaimsDataProviderFixture_With_StateId_NoLeaOverlap :
        GetImpersonatedClaimsDataProviderFixture
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(wimpProvider.GetWimp()).Return(staffUSIToImpersonate);
            Expect.Call(staffInformationProvider.ResolveUsername(authenticationProvider, staffUSIToImpersonate)).Return(targetedUsername);
            Expect.Call(userClaimsProvider.GetApplicationSpecificClaims(targetedUsername, Convert.ToInt32(staffUSIToImpersonate))).Return(this.GetApplicationSpecificClaims());
        }

        protected override bool IncludeAdministerStateId()
        {
            return true;
        }

        protected override bool IncludeAdministerLeaID()
        {
            return false;
        }

        protected virtual bool IncldueTargetedUserLEAIds()
        {
            return false;
        }

        [Test]
        public void Should_Return_ImpersonatedClaims()
        {
            var claims = getImpersonatedClaimsDataProvider.GetImpersonatedClaimsData(GetImpersonatorClaims());
            VerifyUserClaimsData(claims);
        }
    }

    public class When_Calling_GetImpersonatedClaimsDataProviderFixture_No_StateId_WithLeaOverlap :
        GetImpersonatedClaimsDataProviderFixture
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(wimpProvider.GetWimp()).Return(staffUSIToImpersonate);
            Expect.Call(staffInformationProvider.ResolveUsername(authenticationProvider, staffUSIToImpersonate)).Return(targetedUsername);
            Expect.Call(userClaimsProvider.GetApplicationSpecificClaims(targetedUsername, Convert.ToInt32(staffUSIToImpersonate))).Return(this.GetApplicationSpecificClaims());
        }

        protected override bool IncludeAdministerStateId()
        {
            return false;
        }

        protected override bool IncludeAdministerLeaID()
        {
            return true;
        }

        protected virtual bool IncldueTargetedUserLEAIds()
        {
            return true;
        }

        [Test]
        public void Should_Return_ImpersonatedClaims()
        {
            var claims = getImpersonatedClaimsDataProvider.GetImpersonatedClaimsData(GetImpersonatorClaims());
            VerifyUserClaimsData(claims);
        }
    }

    public class When_Calling_GetImpersonatedClaimsDataProviderFixture_With_StateId_WithLeaOverlap :
        GetImpersonatedClaimsDataProviderFixture
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(wimpProvider.GetWimp()).Return(staffUSIToImpersonate);
            Expect.Call(staffInformationProvider.ResolveUsername(authenticationProvider, staffUSIToImpersonate)).Return(targetedUsername);
            Expect.Call(userClaimsProvider.GetApplicationSpecificClaims(targetedUsername, Convert.ToInt32(staffUSIToImpersonate))).Return(this.GetApplicationSpecificClaims());
        }

        protected override bool IncludeAdministerStateId()
        {
            return true;
        }

        protected override bool IncludeAdministerLeaID()
        {
            return true;
        }

        protected virtual bool IncldueTargetedUserLEAIds()
        {
            return true;
        }

        [Test]
        public void Should_Return_ImpersonatedClaims()
        {
            var userClaimData = getImpersonatedClaimsDataProvider.GetImpersonatedClaimsData(GetImpersonatorClaims());
            VerifyUserClaimsData(userClaimData);
        }
    }
}
