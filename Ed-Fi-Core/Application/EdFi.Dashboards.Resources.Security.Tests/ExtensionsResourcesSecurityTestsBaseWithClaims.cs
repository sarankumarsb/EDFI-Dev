using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Testing;
using Microsoft.IdentityModel.Claims;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Security.Tests
{
    public abstract class ExtensionsResourcesSecurityTestsBaseWithClaims : TestFixtureBase
    {
        public static readonly string TEST_ISSUER = "https://dlpsaas.accesscontrol.windows.net/";
        public static readonly string STS_URL = "https://sts.windows.net/{0}/";
        public static readonly string FIRST_NAME = "Kurt";
        public static readonly string LAST_NAME = "McCarthy";
        public static readonly string EMAIL = "KurtMcCarthy@dlpsaasdemo.onmicrosoft.com";
        public static readonly Guid TENANT_ID = Guid.NewGuid();
        public static readonly int LEA = 255901;
        public static readonly string CODE = "code";

        protected IClaimsPrincipal principal;
        protected IClaimsIdentity claimsIdentity;
        protected ClaimCollection suppliedClaimsCollect;
        protected ClaimsIdentityCollection identityCollection;
        protected List<Claim> suppliedClaims;

        protected override void EstablishContext()
        {
            claimsIdentity = mocks.Stub<IClaimsIdentity>();
            
            suppliedClaims = GetDefaultClaims(IncludeLeaClaim(), IncludeLeaCodeClaim());
            suppliedClaimsCollect = new ClaimCollection(claimsIdentity);
            suppliedClaimsCollect.AddRange(suppliedClaims);

            identityCollection = new ClaimsIdentityCollection(Enumerable.Repeat(claimsIdentity, 1));

            Expect.Call(claimsIdentity.Claims).Return(suppliedClaimsCollect);
            principal = mocks.Stub<IClaimsPrincipal>();

            Expect.Call(principal.Identity).Return(claimsIdentity);
            Expect.Call(principal.Identities).Return(identityCollection);
        }

        protected virtual bool IncludeLeaClaim()
        {
            return true;
        }

        protected virtual bool SubstituteLeaIdString(out string toSubWith)
        {
            toSubWith = string.Empty;
            return false;
        }

        protected virtual bool IncludeLeaCodeClaim()
        {
            return true;
        }

        protected List<Claim> GetDefaultClaims(bool includeLea, bool includeLeaCodeClaim)
        {
            List<Claim> claims = new List<Claim>();

            if (includeLea)
            {
                var lea = LEA.ToString();
                string toReplaceWith;

                if (SubstituteLeaIdString(out toReplaceWith))
                    claims.Add(new Claim(CustomDashboardClaimType.LocalEducationAgencyId, toReplaceWith, ClaimValueTypes.String, TEST_ISSUER));
                else
                    claims.Add(new Claim(CustomDashboardClaimType.LocalEducationAgencyId, LEA.ToString(), ClaimValueTypes.String, TEST_ISSUER));
            }

            if (includeLeaCodeClaim)
                claims.Add(new Claim(CustomDashboardClaimType.LocalEducationAgencyCode, CODE, ClaimValueTypes.String, TEST_ISSUER));

            claims.Add(new Claim(ClaimTypes.GivenName, FIRST_NAME, ClaimValueTypes.String, TEST_ISSUER));
            claims.Add(new Claim(ClaimTypes.Name, EMAIL, ClaimValueTypes.String, TEST_ISSUER));
            claims.Add(new Claim(ClaimTypes.Surname, LAST_NAME, ClaimValueTypes.String, TEST_ISSUER));
            claims.Add(new Claim(ClaimTypes.Email, EMAIL, ClaimValueTypes.String, TEST_ISSUER));
            return claims;
        }

        public List<Claim> GetApplicationClaims()
        {
            var toReturn = new List<Claim>()
            {
                new Claim(EdFiClaimTypes.AccessOrganization, "accessOrg"),
                new Claim(EdFiClaimTypes.AdministerDashboard, "administerDashboard"),
                new Claim(EdFiClaimTypes.FullName, "fullName"),
                new Claim(EdFiClaimTypes.Impersonating, "impersonating"),
                new Claim(EdFiClaimTypes.LocalEducationAgencyId, "localEducationAgency"),
                new Claim(EdFiClaimTypes.ManageGoals, "manageGoals")
            };
            return toReturn;
        }
    }
}
