// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
    public class StaffCategoryUserClaimSetsProvider : ChainOfResponsibilityUserClaimSetsProviderBase<ClaimsSet, EdFiUserSecurityDetails>
    {
        public StaffCategoryUserClaimSetsProvider(IUserClaimSetsProvider<ClaimsSet, EdFiUserSecurityDetails> next)
        {
            this.Next = next;
        }

        protected override bool CanGetUserClaimSets(EdFiUserSecurityDetails userSecurityDetails)
        {
            return DoGetUserClaimSets(userSecurityDetails).Any();
        }

        protected override IEnumerable<ClaimsSet> DoGetUserClaimSets(EdFiUserSecurityDetails userSecurityDetails)
        {
            var claimsSets = new List<ClaimsSet>();

            switch (userSecurityDetails.StaffCategory)
            {
                case "LEA System Administrator":
                case "Local Education Agency System Administrator":
                    claimsSets.Add(ClaimsSet.SystemAdministrator);
                    break;

                case "Superintendent":
                    claimsSets.Add(ClaimsSet.Superintendent);
                    break;                  

                case "Principal":
                    claimsSets.Add(ClaimsSet.Principal);
                    break;

                case "Assistant Superintendent":
                case "Assistant Principal":
                case "Counselor":
                case "School Leader":
                    claimsSets.Add(ClaimsSet.Administration);
                    break;

                case "School Administrative Support Staff":
                case "LEA Administrative Support Staff":
                case "Local Education Agency Administrative Support Staff":
                    claimsSets.Add(ClaimsSet.Leader);
                    break;

                case "LEA Specialist":
                case "Local Education Agency Specialist":
                case "Teacher":
                case "School Nurse":
                case "High School Counselor":
                case "School Specialist":
                case "Specialist/Consultant":
                    claimsSets.Add(ClaimsSet.Specialist);
                    break;

                case "Local Education Agency":
                case "LEA Administrator":
                case "Local Education Agency Administrator":
                case "Clerk":
                case "School Administrator":
                    claimsSets.Add(ClaimsSet.Staff);
                    break;
                case "Student":
                    claimsSets.Add(ClaimsSet.Student); // VINSTUDLOGIN
                    break;

            }

            return claimsSets;
        }
    }
}