using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using EdFi.Dashboards.Resources.Security.Implementations;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.SecurityTokenService.Authentication.Analytics;
using Microsoft.IdentityModel.Claims;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Security.Tests.Implementations
{
    public abstract class AzureClaimsAuthenticationManagerProviderFixture : ExtensionsResourcesSecurityTestsBaseWithClaims
    {
        protected IStaffInformationFromEmailProvider staffInformationFromEmailProvider;
        protected IClaimsIssuedTrackingEventProvider ClaimsIssuedTrackingEventProvider;
        protected IUserClaimsProvider userClaimsProvider;
        protected AzureClaimsAuthenticationManagerProvider azureClaimsAuthenticationManagerProvider;
        protected IGetImpersonatedClaimsDataProvider getImpersonatedClaimsDataProvider;

        protected readonly string actualResourceName = "resourceName";

        protected override void EstablishContext()
        {
            base.EstablishContext();

            staffInformationFromEmailProvider = mocks.StrictMock<IStaffInformationFromEmailProvider>();
            ClaimsIssuedTrackingEventProvider = mocks.StrictMock<IClaimsIssuedTrackingEventProvider>();
            userClaimsProvider = mocks.StrictMock<IUserClaimsProvider>();
            getImpersonatedClaimsDataProvider = mocks.StrictMock<IGetImpersonatedClaimsDataProvider>();
        }

        protected override void ExecuteTest()
        {
            azureClaimsAuthenticationManagerProvider = new AzureClaimsAuthenticationManagerProvider(staffInformationFromEmailProvider, 
                ClaimsIssuedTrackingEventProvider, userClaimsProvider, getImpersonatedClaimsDataProvider);
        }
    }

    public class When_Calling_AzureClaimsAuthenticationManager_With_Null_Principal : AzureClaimsAuthenticationManagerProviderFixture
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_Throw_Exception()
        {
            azureClaimsAuthenticationManagerProvider.Get(actualResourceName, null);
        }
    }

    public class When_Calling_AzureClaimsAuthenticationManager_And_Principal_Is_Not_Authenticated :
        AzureClaimsAuthenticationManagerProviderFixture
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
            azureClaimsAuthenticationManagerProvider.Get(actualResourceName, principal);
        }
    }

    public class When_Calling_AzureClaimsAuthenticationManager_Not_Impersonating :
        AzureClaimsAuthenticationManagerProviderFixture
    {
        protected string name = "name";
        protected int staffUSI = 1;


        protected override void EstablishContext()
        {
            base.EstablishContext();

            suppliedClaims = GetApplicationClaims();

            Expect.Call(claimsIdentity.IsAuthenticated).Return(true);
            Expect.Call(claimsIdentity.Name).Return(name);
            Expect.Call(staffInformationFromEmailProvider.ResolveStaffUSI(name)).Return(staffUSI);
            Expect.Call(userClaimsProvider.GetApplicationSpecificClaims(name, staffUSI)).Return(suppliedClaims);
            ClaimsIssuedTrackingEventProvider.Expect(x => x.Track(name, staffUSI, false, suppliedClaims));
            Expect.Call(getImpersonatedClaimsDataProvider.IsImpersonating()).Return(false);
        }

        [Test]
        public void Should_Return_Principal_With_Application_Claims()
        {
            var result = azureClaimsAuthenticationManagerProvider.Get(actualResourceName, principal);
            CollectionAssert.IsSubsetOf(suppliedClaims, result.Identities.First().Claims);
        }
    }

    public class When_Calling_AzureClaimsAuthenticationManager_Impersonating :
        AzureClaimsAuthenticationManagerProviderFixture
    {
        protected string name = "name";
        protected int staffUSI = 1;
        protected List<Claim> impersonatedClaims;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            suppliedClaims = GetApplicationClaims();

            Expect.Call(claimsIdentity.IsAuthenticated).Return(true);
            Expect.Call(claimsIdentity.Name).Return(name);
            Expect.Call(staffInformationFromEmailProvider.ResolveStaffUSI(name)).Return(staffUSI);
            Expect.Call(userClaimsProvider.GetApplicationSpecificClaims(name, staffUSI)).Return(suppliedClaims);

            impersonatedClaims = GetImpersonatedClaims();
            var userClaimData = new UserClaimsData() { Claims = impersonatedClaims };
            Expect.Call(getImpersonatedClaimsDataProvider.IsImpersonating()).Return(true);
            Expect.Call(getImpersonatedClaimsDataProvider.GetImpersonatedClaimsData(suppliedClaims)).Return(userClaimData);

            ClaimsIssuedTrackingEventProvider.Expect(x => x.Track(name, staffUSI, false, impersonatedClaims));

        }

        private List<Claim> GetImpersonatedClaims()
        {
            var impersonatedClaims = new List<Claim>()
            {
                new Claim(EdFiClaimTypes.AccessOrganization, "accessOrgImp"),
                new Claim(EdFiClaimTypes.AdministerDashboard, "administerDashboardImp"),
                new Claim(EdFiClaimTypes.FullName, "fullNameImp"),
                new Claim(EdFiClaimTypes.Impersonating, "impersonatingImp"),
                new Claim(EdFiClaimTypes.LocalEducationAgencyId, "localEducationAgencyImp"),
                new Claim(EdFiClaimTypes.ManageGoals, "manageGoalsImp")
            };

            return impersonatedClaims;
        }

        [Test]
        public void Should_Return_Principal_With_Application_Claims()
        {
            var result = azureClaimsAuthenticationManagerProvider.Get(actualResourceName, principal);
            CollectionAssert.IsSubsetOf(impersonatedClaims, result.Identities.First().Claims);
            CollectionAssert.AreNotEquivalent(impersonatedClaims, suppliedClaims);
        }
    }
}