using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Security.Implementations;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.SecurityTokenService.Authentication.Analytics;
using Microsoft.IdentityModel.Claims;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Security.Tests.Implementations
{
    public abstract class DashaboardClaimsAuthenticationManagerProviderFixture : ExtensionsResourcesSecurityTestsBaseWithClaims
    {
        protected IStaffInformationFromEmailProvider staffInformationFromEmailProvider;
        protected IAuthenticationProvider authenticationProvider;
        protected IHttpContextItemsProvider httpContextItemsProvider;
        protected IUserClaimsProvider userClaimsProvider;
        protected IClaimsIssuedTrackingEventProvider ClaimsIssuedTrackingEventProvider;
        protected DashboardClaimsAuthenticationManagerProvider DashboardClaimsAuthenticationManagerProvider;
        protected IGetImpersonatedClaimsDataProvider getImpersonatedClaimsDataProvider;

        protected readonly string resourceName = "resourceName";
        protected readonly string userName = "userName";
        
        protected override void EstablishContext()
        {
            base.EstablishContext();

            staffInformationFromEmailProvider = mocks.StrictMock<IStaffInformationFromEmailProvider>();
            authenticationProvider = mocks.StrictMock<IAuthenticationProvider>();
            httpContextItemsProvider = mocks.StrictMock<IHttpContextItemsProvider>();
            userClaimsProvider = mocks.StrictMock<IUserClaimsProvider>();
            ClaimsIssuedTrackingEventProvider = mocks.StrictMock<IClaimsIssuedTrackingEventProvider>();
            getImpersonatedClaimsDataProvider = mocks.StrictMock<IGetImpersonatedClaimsDataProvider>();
        }

        protected override void ExecuteTest()
        {
            DashboardClaimsAuthenticationManagerProvider =
                new DashboardClaimsAuthenticationManagerProvider(staffInformationFromEmailProvider, authenticationProvider, userClaimsProvider,
                    ClaimsIssuedTrackingEventProvider, httpContextItemsProvider, getImpersonatedClaimsDataProvider);
        }
    }

    public class WhenCallingDashaboardClaimsAuthenticationManagerProviderNullPrincipal :
        DashaboardClaimsAuthenticationManagerProviderFixture
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_Throw_Exception()
        {
            var result = DashboardClaimsAuthenticationManagerProvider.Get(resourceName, null);
        }
    }

    public class WhenCallingDashaboardClaimsAuthenticationManagerProviderNotAuthenticated :
        DashaboardClaimsAuthenticationManagerProviderFixture
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(claimsIdentity.IsAuthenticated).Return(false);
        }

        [Test]
        [ExpectedException(typeof(AuthenticationException))]
        public void Should_Throw_Exception()
        {
            var result = DashboardClaimsAuthenticationManagerProvider.Get(resourceName, principal);
        }
    }

    public class WhenCallingDashaboardClaimsAuthenticationManagerProviderMissingLeaClaim :
        DashaboardClaimsAuthenticationManagerProviderFixture
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(claimsIdentity.IsAuthenticated).Return(true);
        }

        protected override bool IncludeLeaCodeClaim()
        {
            return false;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_Exception()
        {
            var result = DashboardClaimsAuthenticationManagerProvider.Get(resourceName, principal);
        }
    }

    public class WhenCallingDashaboardClaimsAuthenticationManagerProviderNotImpersonating :
        DashaboardClaimsAuthenticationManagerProviderFixture
    {
        protected readonly int staffUSI;
        protected List<Claim> suppliedApplicationClaims;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            suppliedApplicationClaims = GetApplicationClaims();

            Expect.Call(claimsIdentity.IsAuthenticated).Return(true);
            httpContextItemsProvider.Expect(x => x.Add(null, null)).IgnoreArguments();
            claimsIdentity.Expect(x => x.Name).Return(userName);
            Expect.Call(staffInformationFromEmailProvider.ResolveStaffUSI(EMAIL)).Return(staffUSI);
            Expect.Call(userClaimsProvider.GetApplicationSpecificClaims(userName, staffUSI)).Return(suppliedApplicationClaims);
            Expect.Call(getImpersonatedClaimsDataProvider.IsImpersonating()).Return(false);
            ClaimsIssuedTrackingEventProvider.Expect(x => x.Track(userName, staffUSI, false, suppliedApplicationClaims));
        }

        [Test]
        public void Should_Add_ApplicationClaims()
        {
            var result = DashboardClaimsAuthenticationManagerProvider.Get(resourceName, principal);
            CollectionAssert.IsSubsetOf(suppliedApplicationClaims, result.Identities.First().Claims);
        }
    }

    public class WhenCallingDashaboardClaimsAuthenticationManagerProviderImpersonating :
        DashaboardClaimsAuthenticationManagerProviderFixture
    {
        protected readonly int staffUSI;
        protected List<Claim> suppliedApplicationClaims;
        protected List<Claim> suppliedImpersonatedClaims;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            suppliedApplicationClaims = GetApplicationClaims();

            Expect.Call(claimsIdentity.IsAuthenticated).Return(true);
            httpContextItemsProvider.Expect(x => x.Add(null, null)).IgnoreArguments();
            claimsIdentity.Expect(x => x.Name).Return(userName);
            Expect.Call(staffInformationFromEmailProvider.ResolveStaffUSI(EMAIL)).Return(staffUSI);
            Expect.Call(userClaimsProvider.GetApplicationSpecificClaims(userName, staffUSI)).Return(suppliedApplicationClaims);
            suppliedImpersonatedClaims = GetSuppliedImpersonatedClaims();
            var userData = new UserClaimsData() { Claims = suppliedImpersonatedClaims, Username = userName };
            Expect.Call(getImpersonatedClaimsDataProvider.IsImpersonating()).Return(true);
            Expect.Call(getImpersonatedClaimsDataProvider.GetImpersonatedClaimsData(suppliedApplicationClaims)).Return(userData);
            ClaimsIssuedTrackingEventProvider.Expect(x => x.Track(userName, staffUSI, false, suppliedImpersonatedClaims));
        }

        private List<Claim> GetSuppliedImpersonatedClaims()
        {
            var toReturn = new List<Claim>()
            {
                new Claim(EdFiClaimTypes.AccessOrganization, "accessOrgImp"),
                new Claim(EdFiClaimTypes.AdministerDashboard, "administerDashboardImp"),
                new Claim(EdFiClaimTypes.FullName, "fullNameImp"),
                new Claim(EdFiClaimTypes.Impersonating, "impersonatingImp"),
                new Claim(EdFiClaimTypes.LocalEducationAgencyId, "localEducationAgencyImp"),
                new Claim(EdFiClaimTypes.ManageGoals, "manageGoalsImp")
            };
            return toReturn;
        }

        [Test]
        public void Should_Add_ApplicationClaims()
        {
            var result = DashboardClaimsAuthenticationManagerProvider.Get(resourceName, principal);
            CollectionAssert.IsSubsetOf(suppliedImpersonatedClaims, result.Identities.First().Claims);
            Assert.That(suppliedImpersonatedClaims.Intersect(suppliedApplicationClaims).Count(), Is.EqualTo(0));
        }
    }
}