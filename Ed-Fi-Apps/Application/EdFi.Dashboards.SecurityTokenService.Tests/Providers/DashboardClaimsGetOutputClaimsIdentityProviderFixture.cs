using System.Security.Cryptography.X509Certificates;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.SecurityTokenService.Authentication.Analytics;
using EdFi.Dashboards.SecurityTokenService.Web.Providers;
using EdFi.Dashboards.Testing;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IdentityModel.Claims;
using Rhino.Mocks;
using NUnit.Framework;
using System;

namespace EdFi.Dashboards.SecurityTokenService.Tests.Providers
{
    public class DashboardClaimsGetOutputClaimsIdentityProviderFixture : TestFixtureBase
    {
        protected IStaffInformationProvider staffInformationProvider;
        protected IAuthenticationProvider authenticationProvider;
        protected IUserClaimsProvider userClaimsProvider;
        protected IClaimsIssuedTrackingEventProvider ClaimsIssuedTrackingEventProvider;
        protected IGetImpersonatedClaimsDataProvider getImpersonatedClaimsDataProvider;
        protected DashboardClaimsGetOutputClaimsIdentityProvider DashboardClaimsGetOutputClaimsIdentityProvider;
        protected readonly string userName = "UserName";
        protected IClaimsIdentity identity;
        protected IClaimsPrincipal principal;
        protected readonly int staffUSI = 1;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            staffInformationProvider = mocks.Stub<IStaffInformationProvider>();
            authenticationProvider = mocks.Stub<IAuthenticationProvider>();
            userClaimsProvider = mocks.DynamicMock<IUserClaimsProvider>();
            
            identity = mocks.Stub<IClaimsIdentity>();
            principal = mocks.Stub<IClaimsPrincipal>();

            Expect.Call(identity.Name).Return(userName);
            Expect.Call(principal.Identity).Return(identity);
            Expect.Call(staffInformationProvider.ResolveStaffUSI(authenticationProvider, userName)).Return(staffUSI);

            
        }

        protected virtual IClaimsPrincipal GetPrincipal()
        {
            return principal;
        }

        protected virtual IEnumerable<Claim> GetSuppliedAppSpecificClaims()
        {
            return new List<Claim>().AsEnumerable();
        }

        protected virtual UserClaimsData GetSuppliedImpersonated()
        {
            return new UserClaimsData {Username = userName, Claims = GetSuppliedImpersonatedClaims()};
        }

        protected virtual IEnumerable<Claim> GetSuppliedImpersonatedClaims()
        {
            return new List<Claim>().AsEnumerable();
        }

        protected virtual bool IsImpersonating()
        {
            return true;
        }

        protected override void ExecuteTest()
        {
            DashboardClaimsGetOutputClaimsIdentityProvider = new DashboardClaimsGetOutputClaimsIdentityProvider(staffInformationProvider, authenticationProvider,
                userClaimsProvider, ClaimsIssuedTrackingEventProvider, getImpersonatedClaimsDataProvider);
        }
    }

    public class WhenCallingDashboardClaimsGetOutputClaimsIdentityProviderWithNullPrincipal :
        DashboardClaimsGetOutputClaimsIdentityProviderFixture
    {
        protected override IClaimsPrincipal GetPrincipal()
        {
            return null;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_Throw_Exception()
        {
            var claims = DashboardClaimsGetOutputClaimsIdentityProvider.GetOutputClaimsIdentity(GetPrincipal(), null, null);
        }
    }

    public class WhenCallingDashboardClaimsGetOutputClaimsIdentityProviderAndImpersonating :
        DashboardClaimsGetOutputClaimsIdentityProviderFixture
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            userClaimsProvider = mocks.StrictMock<IUserClaimsProvider>();
            getImpersonatedClaimsDataProvider = mocks.DynamicMock<IGetImpersonatedClaimsDataProvider>();
            ClaimsIssuedTrackingEventProvider = mocks.StrictMock<IClaimsIssuedTrackingEventProvider>();

            Expect.Call(userClaimsProvider.GetApplicationSpecificClaims(userName, staffUSI)).Return(GetSuppliedAppSpecificClaims());
            Expect.Call(getImpersonatedClaimsDataProvider.IsImpersonating()).Return(IsImpersonating());
            Expect.Call(getImpersonatedClaimsDataProvider.GetImpersonatedClaimsData(GetSuppliedAppSpecificClaims())).Return(GetSuppliedImpersonated());
            ClaimsIssuedTrackingEventProvider.Expect(x => x.Track(userName, staffUSI, true, GetSuppliedAppSpecificClaims()));
        }

        [Test]
        public void Should_Return_ImpersonatedClaims()
        {
            var claims = DashboardClaimsGetOutputClaimsIdentityProvider.GetOutputClaimsIdentity(GetPrincipal(), null, null);
        }
    }
}
