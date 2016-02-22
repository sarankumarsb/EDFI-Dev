// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Resources.Security.ClaimValidators;
using EdFi.Dashboards.Resources.Security.Implementations;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security.AuthorizationDelegates
{
    /// <summary>
    /// Grants the current user access to LEA information if the user has any claim on any organization
    /// in the LEA hierarchy.
    /// </summary>
    public class OrganizationContextAuthorizationDelegate : ParameterAuthorizationBase, IAuthorizationDelegate
    {
        public OrganizationContextAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider) :
            base(securityAssertionProvider, null)
        {
        }

        private void CheckLea(IEnumerable<ParameterInstance> parameters)
        {
            var lea = (int?)GetParameterByName(parameters, ClaimValidatorRequest.LocalEducationAgencyParameterName).Value;
            if (null == lea) return;

            var userInfo = UserInformation.Current;
            //Look at LEA or higher.
            var claimValidatorLeaEdorgs = AssertionProvider.GetEducationOrganizationHierarchy((int) lea);
            if (userInfo.AssociatedOrganizations.Any(
                n => ((!HasNothingClaimType(n.ClaimTypes)) &&
                    (claimValidatorLeaEdorgs.Contains(n.EducationOrganizationId) || //The user has an explict claim for the LEA or State level
                    (AssertionProvider.GetEducationOrganizationHierarchy(n.EducationOrganizationId).Contains((int)lea)))))) //The user has the lea within the hierarchy of one of thier claims.
                    //Note: Can not look higher than LEA in this last check because the State agency ID always comes back in the hierarchy.
                return;

            throw new UserAccessDeniedException(SecurityAssertionProvider.NoEducationOrganizationPermissionErrorMessage);
        }

        public void AuthorizeRequest(IEnumerable<ParameterInstance> parameters)
        {
            CheckLea(parameters);
        }
    }
}
