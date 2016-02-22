using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Testing;
using EdFi.Dashboards.SecurityTokenService.Web.Providers;
using Microsoft.IdentityModel.Claims;
using Rhino.Mocks;
using NUnit.Framework;
using EdFi.Dashboards.Resources.Security;

namespace EdFi.Dashboards.SecurityTokenService.Tests.Providers
{
    public class IdentityClaimsGetOutputClaimsIdentityProviderFixture : TestFixtureBase
    {
        protected IStaffInformationProvider staffInformationProvider;
        protected IAuthenticationProvider authenticationProvider;
        protected IDashboardUserClaimsInformationProvider<EdFiUserSecurityDetails> dashboardUserClaimsInformationProvider;
        protected IHttpRequestProvider httpRequestProvider;
        protected IdentityClaimsGetOutputClaimsIdentityProvider identityClaimsGetOutputClaimsIdentityProvider;
        protected readonly string userName = "UserName";
        protected IClaimsIdentity identity;
        protected IClaimsPrincipal principal;
        protected readonly int staffUSI = 1;
        protected readonly string firstName = "firstName";
        protected readonly string lastName = "lastName";
        protected readonly string fullName = "fullName";
        protected readonly string email = "emal";

        protected override void EstablishContext()
        {
            staffInformationProvider = mocks.Stub<IStaffInformationProvider>();
            authenticationProvider = mocks.Stub<IAuthenticationProvider>();
            dashboardUserClaimsInformationProvider = mocks.Stub<IDashboardUserClaimsInformationProvider<EdFiUserSecurityDetails>>();
            httpRequestProvider = mocks.Stub<IHttpRequestProvider>();

            identity = mocks.Stub<IClaimsIdentity>();
            principal = mocks.Stub<IClaimsPrincipal>();

            Expect.Call(identity.Name).Return(userName);
            Expect.Call(principal.Identity).Return(identity);
            Expect.Call(staffInformationProvider.ResolveStaffUSI(authenticationProvider, userName)).Return(staffUSI);
        }

        protected virtual DashboardUserClaimsInformation<EdFiUserSecurityDetails> GetSuppliedDashboardUserClaimsInformation()
        {
            return new DashboardUserClaimsInformation<EdFiUserSecurityDetails>
            {
                StaffUSI = staffUSI,
                FirstName = firstName,
                LastName = lastName,
                FullName = fullName,
                Email = email
            };
        }

        protected override void ExecuteTest()
        {
            identityClaimsGetOutputClaimsIdentityProvider = new IdentityClaimsGetOutputClaimsIdentityProvider(staffInformationProvider, 
                authenticationProvider, dashboardUserClaimsInformationProvider, httpRequestProvider);
        }
    }

    public class WhenCallingIdentityClaimsGetOutputClaimsIdentityProviderFixtureWithNullPrincipal :
        IdentityClaimsGetOutputClaimsIdentityProviderFixture
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_Throw_Exception()
        {
            identityClaimsGetOutputClaimsIdentityProvider.GetOutputClaimsIdentity(null, null, null);
        }
    }

    public class WhenCallingIdentityClaimsGetOutputClaimsIdentityProviderFixtureNoUserInformation :
        IdentityClaimsGetOutputClaimsIdentityProviderFixture
    {
        [Test]
        [ExpectedException(typeof(StaffSchoolClassAssociationException))]
        public void Should_Throw_Exception()
        {
            identityClaimsGetOutputClaimsIdentityProvider.GetOutputClaimsIdentity(principal, null, null);
        }
    }

    public class WhenCallingIdentityClaimsGetOutputClaimsIdentityProviderFixture :
        IdentityClaimsGetOutputClaimsIdentityProviderFixture
    {
        protected readonly string lea = "250001";

        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(dashboardUserClaimsInformationProvider.GetClaimsInformation(userName, staffUSI))
                .Return(GetSuppliedDashboardUserClaimsInformation());
            Expect.Call(httpRequestProvider.GetValue("lea")).Return(lea);
        }

        public bool ClaimExists(IClaimsIdentity claimsIdentity, Func<Claim, bool> predicate)
        {
            var result = claimsIdentity.Claims.FirstOrDefault(predicate);
            return result != null;
        }

        [Test]
        public void Should_Return_UserInfo_Claims()
        {
            var claims = identityClaimsGetOutputClaimsIdentityProvider.GetOutputClaimsIdentity(principal, null, null);
            Assert.That(ClaimExists(claims, claim => claim.ClaimType == EdFiClaimTypes.FullName && claim.Value == fullName));
            Assert.That(ClaimExists(claims, claim => claim.ClaimType == ClaimTypes.GivenName && claim.Value == firstName));
            Assert.That(ClaimExists(claims, claim => claim.ClaimType == ClaimTypes.Surname && claim.Value == lastName));
            Assert.That(ClaimExists(claims, claim => claim.ClaimType == CustomDashboardClaimType.LocalEducationAgencyCode && claim.Value == lea));
            Assert.That(ClaimExists(claims, claim => claim.ClaimType == ClaimTypes.Email && claim.Value == email));
            Assert.That(ClaimExists(claims, claim => claim.ClaimType == EdFiClaimTypes.StaffUSI && claim.Value == staffUSI.ToString()));
            Assert.That(ClaimExists(claims, claim => claim.ClaimType == ClaimTypes.Name && claim.Value == userName));
        }
    }
}
