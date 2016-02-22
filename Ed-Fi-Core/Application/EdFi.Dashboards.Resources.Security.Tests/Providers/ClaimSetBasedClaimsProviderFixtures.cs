// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Resources.Security.Implementations;
using EdFi.Dashboards.Resources.Security.Tests.ClaimValidators;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using Microsoft.IdentityModel.Claims;
using Newtonsoft.Json;
using NUnit.Framework;

namespace EdFi.Dashboards.Resources.Security.Tests.Providers
{
    /// <summary>
    /// Summary description for RoleBasedClaimsProviderFixtures
    /// </summary>
    public abstract class ClaimSetBasedClaimsProviderFixturesBase : ClaimValidatorFixturesBase
    {
        protected ClaimSetBasedClaimsProvider currentProvider = new ClaimSetBasedClaimsProvider();
        protected Exception exceptionThrown;

        protected IEnumerable<Claim> actualClaims;

        protected ClaimsSet currentClaimsSet;
        protected EducationOrganizationIdentifier currentEducationOrganizationIdentifier;

        protected virtual void InitializeParameters()
        {
            currentClaimsSet = ClaimsSet.Specialist;
            currentEducationOrganizationIdentifier = null;
        }

        protected override void ExecuteTest()
        {
            try
            {
                InitializeParameters();
                actualClaims =
                    currentProvider.GetClaims(currentClaimsSet, currentEducationOrganizationIdentifier).ToList();

            }
            catch (Exception ex)
            {
                exceptionThrown = ex;
            }
        }

        public void ExpectClaims(IEnumerable<Claim> expectedClaims)
        {
            Assert.That(expectedClaims.Count() == actualClaims.Count(),
                        string.Format("Expected {0} claims, Got {1}", expectedClaims.Count(), actualClaims.Count()));

            // Make sure that each expected claim is in the current claims.
            foreach (var expectedClaim in expectedClaims)
            {
                ExpectClaim(expectedClaim, actualClaims);
            }

            // And make sure that each current claim is present in the expectedClaims.
            foreach (var currentClaim in actualClaims)
            {
                ExpectClaim( currentClaim, expectedClaims);
            }
        }

        public void ExpectClaim(Claim expectedClaim, IEnumerable<Claim> currentClaims)
        {
            Assert.That(currentClaims != null);

            var expectedClaimType = expectedClaim.ClaimType;
            var expectedProperties = JsonConvert.DeserializeObject<Dictionary<string, string>>(expectedClaim.Value);
            var expectedLeaId = expectedProperties[EdFiClaimProperties.LocalEducationAgencyId];
            var expectedSchId = expectedProperties[EdFiClaimProperties.SchoolId];

            var match =
                (from claim in currentClaims
                 let claimProperties = JsonConvert.DeserializeObject<Dictionary<string,string>>(claim.Value)
                 let claimLeaId = claimProperties[EdFiClaimProperties.LocalEducationAgencyId]
                 let claimSchId = claimProperties[EdFiClaimProperties.SchoolId]
                 where (expectedClaimType == claim.ClaimType && expectedLeaId == claimLeaId && expectedSchId == claimSchId)
                 select claim).FirstOrDefault();
            Assert.That(match, Is.Not.Null,
                        string.Format("Did not match Claim: ClaimType {0} LocalEducationAgencyId {1} SchoolId {2}",
                                      expectedClaimType, expectedLeaId, expectedSchId));
        }
    }

    public class When_system_administrator_gets_claims_without_school : ClaimSetBasedClaimsProviderFixturesBase
    {
        protected override void InitializeParameters()
        {
            currentClaimsSet = ClaimsSet.SystemAdministrator;
            currentEducationOrganizationIdentifier = EDU_ORG_ID_1_0;
        }

        [Test]
        public void ShouldPass()
        {
            var expected = new List<Claim>
                               {
                                   CLAIM_ACCESS_ORGANIZATION_1_0,
                                   CLAIM_ADMIN_1_0,
                                   CLAIM_ALL_STUDENTS_1_0,
                                   CLAIM_ALL_METRICS_1_0,
                                   CLAIM_MANAGE_GOALS_1_0,
                                   CLAIM_OPERATIONS_1_0,
                                   CLAIM_TEACHERS_1_0,
                                   CLAIM_WATCH_LIST_1_0
                               };

            ExpectClaims(expected);
        }
    }
    
    public class When_system_administrator_gets_claims_with_school : ClaimSetBasedClaimsProviderFixturesBase
    {
        protected override void InitializeParameters()
        {
            currentClaimsSet = ClaimsSet.SystemAdministrator;
            currentEducationOrganizationIdentifier = EDU_ORG_ID_1_1;
        }

        [Test]
        public void ShouldPass()
        {
            var expected = new List<Claim>
                               {
                                   CLAIM_ACCESS_ORGANIZATION_1_0,
                                   CLAIM_ADMIN_1_0,
                                   CLAIM_ALL_STUDENTS_1_0,
                                   CLAIM_ALL_METRICS_1_0,
                                   CLAIM_MANAGE_GOALS_1_0,
                                   CLAIM_OPERATIONS_1_0,
                                   CLAIM_TEACHERS_1_0,
                                   CLAIM_WATCH_LIST_1_0
                               };

            ExpectClaims(expected);
        }
    }
    
    public class When_system_administrator_gets_claims_without_lea : ClaimSetBasedClaimsProviderFixturesBase
    {
        protected override void InitializeParameters()
        {
            currentClaimsSet = ClaimsSet.SystemAdministrator;
            currentEducationOrganizationIdentifier = EDU_ORG_ID_0_1;
        }

        [Test]
        public void ShouldPass()
        {
            var expected = new List<Claim>
                               {
                                   CLAIM_ACCESS_ORGANIZATION_0_1,
                                   CLAIM_ADMIN_0_1,
                                   CLAIM_ALL_STUDENTS_0_1,
                                   CLAIM_ALL_METRICS_0_1,
                                   CLAIM_MANAGE_GOALS_0_1,
                                   CLAIM_OPERATIONS_0_1,
                                   CLAIM_TEACHERS_0_1,
                                   CLAIM_WATCH_LIST_0_1
                               };

            ExpectClaims(expected);
        }
    }
    
    public class When_superintendent_gets_claims_without_school : ClaimSetBasedClaimsProviderFixturesBase
    {
        protected override void InitializeParameters()
        {
            currentClaimsSet = ClaimsSet.Superintendent; 
            currentEducationOrganizationIdentifier = EDU_ORG_ID_1_0;
        }
       
        [Test]
        public void ShouldPass()
        {
            var expected = new List<Claim>
                               {
                                   CLAIM_ACCESS_ORGANIZATION_1_0,
                                   CLAIM_ALL_METRICS_1_0,
                                   CLAIM_ALL_STUDENTS_1_0,
                                   CLAIM_TEACHERS_1_0,
                                   CLAIM_OPERATIONS_1_0,
                                   CLAIM_MANAGE_GOALS_1_0,
                                   CLAIM_WATCH_LIST_1_0
                               };

            ExpectClaims(expected);
        }
    }

    public class When_superintendent_gets_claims_with_school : ClaimSetBasedClaimsProviderFixturesBase
    {
        protected override void InitializeParameters()
        {
            currentClaimsSet = ClaimsSet.Superintendent;
            currentEducationOrganizationIdentifier = EDU_ORG_ID_1_1;
        }

        [Test]
        public void ShouldPass()
        {
            var expected = new List<Claim>
                               {
                                   CLAIM_ACCESS_ORGANIZATION_1_1,
                                   CLAIM_ALL_METRICS_1_1,
                                   CLAIM_ALL_STUDENTS_1_1,
                                   CLAIM_TEACHERS_1_1,
                                   CLAIM_OPERATIONS_1_1,
                                   CLAIM_MANAGE_GOALS_1_1,
                                   CLAIM_WATCH_LIST_1_1
                               };

            ExpectClaims(expected);
        }
    }

    public class When_superintendent_gets_claims_without_lea : ClaimSetBasedClaimsProviderFixturesBase
    {
        protected override void InitializeParameters()
        {
            currentClaimsSet = ClaimsSet.Superintendent;
            currentEducationOrganizationIdentifier = EDU_ORG_ID_0_1;
        }

        [Test]
        public void ShouldPass()
        {
            var expected = new List<Claim>
                               {
                                   CLAIM_ACCESS_ORGANIZATION_0_1,
                                   CLAIM_ALL_METRICS_0_1,
                                   CLAIM_ALL_STUDENTS_0_1,
                                   CLAIM_TEACHERS_0_1,
                                   CLAIM_OPERATIONS_0_1,
                                   CLAIM_MANAGE_GOALS_0_1,
                                   CLAIM_WATCH_LIST_0_1
                               };

            ExpectClaims(expected);
        }
    }

    public class When_principal_gets_claims_without_school : ClaimSetBasedClaimsProviderFixturesBase
    {
        protected override void InitializeParameters()
        {
            currentClaimsSet = ClaimsSet.Principal;
            currentEducationOrganizationIdentifier = EDU_ORG_ID_1_0;
        }

        [Test]
        public void ShouldPass()
        {
            var expected = new List<Claim>
                               {
                                   CLAIM_ACCESS_ORGANIZATION_1_0,
                                   CLAIM_ALL_STUDENTS_1_0,
                                   CLAIM_ALL_METRICS_1_0,
                                   CLAIM_MY_METRICS_1_0,
                                   CLAIM_TEACHERS_1_0,
                                   CLAIM_OPERATIONS_1_0,
                                   CLAIM_WATCH_LIST_1_0
                               };

            ExpectClaims(expected);
        }
    }

    public class When_principal_gets_claims_with_school : ClaimSetBasedClaimsProviderFixturesBase
    {
        protected override void InitializeParameters()
        {
            currentClaimsSet = ClaimsSet.Principal;
            currentEducationOrganizationIdentifier = EDU_ORG_ID_1_1;
        }

        [Test]
        public void ShouldPass()
        {
            var expected = new List<Claim>
                               {
                                   CLAIM_ACCESS_ORGANIZATION_1_1,
                                   CLAIM_ALL_STUDENTS_1_1,
                                   CLAIM_ALL_METRICS_1_1,
                                   CLAIM_MY_METRICS_1_0,
                                   CLAIM_TEACHERS_1_1,
                                   CLAIM_OPERATIONS_1_1,
                                   CLAIM_WATCH_LIST_1_0
                               };

            ExpectClaims(expected);
        }
    }

    public class When_principal_gets_claims_without_lea : ClaimSetBasedClaimsProviderFixturesBase
    {
        protected override void InitializeParameters()
        {
            currentClaimsSet = ClaimsSet.Principal;
            currentEducationOrganizationIdentifier = EDU_ORG_ID_0_1;
        }

        [Test]
        public void ShouldPass()
        {
            var expected = new List<Claim>
                               {
                                   CLAIM_ACCESS_ORGANIZATION_0_1,
                                   CLAIM_ALL_STUDENTS_0_1,
                                   CLAIM_ALL_METRICS_0_1,
                                   CLAIM_MY_METRICS_0_1,
                                   CLAIM_TEACHERS_0_1,
                                   CLAIM_OPERATIONS_0_1,
                                   CLAIM_WATCH_LIST_0_1
                               };

            ExpectClaims(expected);
        }
    }

    public class When_administration_gets_claims_without_school : ClaimSetBasedClaimsProviderFixturesBase
    {
        protected override void InitializeParameters()
        {
            currentClaimsSet = ClaimsSet.Administration;
            currentEducationOrganizationIdentifier = EDU_ORG_ID_1_0;
        }

        [Test]
        public void ShouldPass()
        {
            var expected = new List<Claim>
                               {
                                   CLAIM_ACCESS_ORGANIZATION_1_0,
                                   CLAIM_ALL_METRICS_1_0,
                                   CLAIM_MY_METRICS_1_0,
                                   CLAIM_ALL_STUDENTS_1_0,
                                   CLAIM_TEACHERS_1_0,
                                   CLAIM_OPERATIONS_1_0,
                                   CLAIM_WATCH_LIST_1_0
                               };

            ExpectClaims(expected);
        }
    }

    public class When_administration_gets_claims_with_school : ClaimSetBasedClaimsProviderFixturesBase
    {
        protected override void InitializeParameters()
        {
            currentClaimsSet = ClaimsSet.Administration;
            currentEducationOrganizationIdentifier = EDU_ORG_ID_1_1;
        }

        [Test]
        public void ShouldPass()
        {
            var expected = new List<Claim>
                               {
                                   CLAIM_ACCESS_ORGANIZATION_1_1,
                                   CLAIM_ALL_METRICS_1_1,
                                   CLAIM_MY_METRICS_1_0,
                                   CLAIM_ALL_STUDENTS_1_1,
                                   CLAIM_TEACHERS_1_1,
                                   CLAIM_OPERATIONS_1_1,
                                   CLAIM_WATCH_LIST_1_0
                               };

            ExpectClaims(expected);
        }
    }

    public class When_administration_gets_claims_without_lea : ClaimSetBasedClaimsProviderFixturesBase
    {
        protected override void InitializeParameters()
        {
            currentClaimsSet = ClaimsSet.Administration;
            currentEducationOrganizationIdentifier = EDU_ORG_ID_0_1;
        }

        [Test]
        public void ShouldPass()
        {
            var expected = new List<Claim>
                               {
                                   CLAIM_ACCESS_ORGANIZATION_0_1,
                                   CLAIM_ALL_METRICS_0_1,
                                   CLAIM_MY_METRICS_0_1,
                                   CLAIM_ALL_STUDENTS_0_1,
                                   CLAIM_TEACHERS_0_1,
                                   CLAIM_OPERATIONS_0_1,
                                   CLAIM_WATCH_LIST_0_1
                               };

            ExpectClaims(expected);
        }
    }

    public class When_leader_gets_claims_without_school : ClaimSetBasedClaimsProviderFixturesBase
    {
        protected override void InitializeParameters()
        {
            currentClaimsSet = ClaimsSet.Leader;
            currentEducationOrganizationIdentifier = EDU_ORG_ID_1_0;
        }

        [Test]
        public void ShouldPass()
        {
            var expected = new List<Claim>
                               {
                                   CLAIM_ACCESS_ORGANIZATION_1_0,
                                   CLAIM_ALL_METRICS_1_0,
                                   CLAIM_MY_METRICS_1_0,
                                   CLAIM_ALL_STUDENTS_1_0,
                                   CLAIM_TEACHERS_1_0
                               };

            ExpectClaims(expected);
        }
    }

    public class When_leader_gets_claims_with_school : ClaimSetBasedClaimsProviderFixturesBase
    {
        protected override void InitializeParameters()
        {
            currentClaimsSet = ClaimsSet.Leader;
            currentEducationOrganizationIdentifier = EDU_ORG_ID_1_1;
        }

        [Test]
        public void ShouldPass()
        {
            var expected = new List<Claim>
                               {
                                   CLAIM_ACCESS_ORGANIZATION_1_1,
                                   CLAIM_ALL_METRICS_1_1,
                                   CLAIM_MY_METRICS_1_0,
                                   CLAIM_ALL_STUDENTS_1_1,
                                   CLAIM_TEACHERS_1_1
                               };

            ExpectClaims(expected);
        }
    }

    public class When_leader_gets_claims_without_lea : ClaimSetBasedClaimsProviderFixturesBase
    {
        protected override void InitializeParameters()
        {
            currentClaimsSet = ClaimsSet.Leader;
            currentEducationOrganizationIdentifier = EDU_ORG_ID_0_1;
        }

        [Test]
        public void ShouldPass()
        {
            var expected = new List<Claim>
                               {
                                   CLAIM_ACCESS_ORGANIZATION_0_1,
                                   CLAIM_ALL_METRICS_0_1,
                                   CLAIM_MY_METRICS_0_1,
                                   CLAIM_ALL_STUDENTS_0_1,
                                   CLAIM_TEACHERS_0_1
                               };

            ExpectClaims(expected);
        }
    }

    public class When_specialist_gets_claims_without_school : ClaimSetBasedClaimsProviderFixturesBase
    {
        protected override void InitializeParameters()
        {
            currentClaimsSet = ClaimsSet.Specialist;
            currentEducationOrganizationIdentifier = EDU_ORG_ID_1_0;
        }

        [Test]
        public void ShouldPass()
        {
            var expected = new List<Claim>
                               {
                                   CLAIM_ACCESS_ORGANIZATION_1_0,
                                   CLAIM_MY_METRICS_1_0,
                                   CLAIM_MY_STUDENTS_1_0
                               };

            ExpectClaims(expected);
        }
    }

    public class When_specialist_gets_claims_with_school : ClaimSetBasedClaimsProviderFixturesBase
    {
        protected override void InitializeParameters()
        {
            currentClaimsSet = ClaimsSet.Specialist;
            currentEducationOrganizationIdentifier = EDU_ORG_ID_1_1;
        }

        [Test]
        public void ShouldPass()
        {
            var expected = new List<Claim>
                               {
                                   CLAIM_ACCESS_ORGANIZATION_1_1,
                                   CLAIM_MY_METRICS_1_1,
                                   CLAIM_MY_STUDENTS_1_1
                               };

            ExpectClaims(expected);
        }
    }

    public class When_specialist_gets_claims_without_lea : ClaimSetBasedClaimsProviderFixturesBase
    {
        protected override void InitializeParameters()
        {
            currentClaimsSet = ClaimsSet.Specialist;
            currentEducationOrganizationIdentifier = EDU_ORG_ID_0_1;
        }

        [Test]
        public void ShouldPass()
        {
            var expected = new List<Claim>
                               {
                                   CLAIM_ACCESS_ORGANIZATION_0_1,
                                   CLAIM_MY_METRICS_0_1,
                                   CLAIM_MY_STUDENTS_0_1
                               };

            ExpectClaims(expected);
        }
    }

    public class When_staff_gets_claims_without_school : ClaimSetBasedClaimsProviderFixturesBase
    {
        protected override void InitializeParameters()
        {
            currentClaimsSet = ClaimsSet.Staff;
            currentEducationOrganizationIdentifier = EDU_ORG_ID_1_0;
        }

        [Test]
        public void ShouldPass()
        {
            var expected = new List<Claim>
                               {
                                   CLAIM_ACCESS_ORGANIZATION_1_0,
                                   CLAIM_MY_METRICS_1_0
                               };

            ExpectClaims(expected);
        }
    }

    public class When_staff_gets_claims_with_school : ClaimSetBasedClaimsProviderFixturesBase
    {
        protected override void InitializeParameters()
        {
            currentClaimsSet = ClaimsSet.Staff;
            currentEducationOrganizationIdentifier = EDU_ORG_ID_1_1;
        }

        [Test]
        public void ShouldPass()
        {
            var expected = new List<Claim>
                               {
                                   CLAIM_ACCESS_ORGANIZATION_1_1,
                                   CLAIM_MY_METRICS_1_1
                               };

            ExpectClaims(expected);
        }
    }

    public class When_staff_gets_claims_without_lea : ClaimSetBasedClaimsProviderFixturesBase
    {
        protected override void InitializeParameters()
        {
            currentClaimsSet = ClaimsSet.Staff;
            currentEducationOrganizationIdentifier = EDU_ORG_ID_0_1;
        }

        [Test]
        public void ShouldPass()
        {
            var expected = new List<Claim>
                               {
                                   CLAIM_ACCESS_ORGANIZATION_0_1,
                                   CLAIM_MY_METRICS_0_1
                               };

            ExpectClaims(expected);
        }
    }
}
