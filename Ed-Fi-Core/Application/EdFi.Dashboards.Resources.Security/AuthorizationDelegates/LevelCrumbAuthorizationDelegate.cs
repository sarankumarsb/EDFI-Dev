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
    public class LevelCrumbAuthorizationDelegate : ParameterAuthorizationBase, IAuthorizationDelegate
    {
        public LevelCrumbAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider) :
            base(securityAssertionProvider, null)
        {
        }

        private void CheckLea(IEnumerable<ParameterInstance> parameters)
        {
            var lea = (int?)GetParameterByName(parameters, ClaimValidatorRequest.LocalEducationAgencyParameterName).Value;
            if (null == lea) return;

            var userInfo = UserInformation.Current;
            //Look at LEA or higher.
            var claimValidatorLeaEdorgs = AssertionProvider.GetEducationOrganizationHierarchy((int)lea);
            if (userInfo.AssociatedOrganizations.Any(
                n => ((!HasNothingClaimType(n.ClaimTypes)) &&
                    (claimValidatorLeaEdorgs.Contains(n.EducationOrganizationId) || //The user has an explict claim for the LEA or State level
                    (AssertionProvider.GetEducationOrganizationHierarchy(n.EducationOrganizationId).Contains((int)lea)))))) //The user has the lea within the hierarchy of one of thier claims.
                //Note: Can not look higher than LEA in this last check because the State agency ID always comes back in the hierarchy.
                return;

            throw new UserAccessDeniedException(SecurityAssertionProvider.NoEducationOrganizationPermissionErrorMessage);
        }

        private void CheckSchool(IEnumerable<ParameterInstance> parameters)
        {
            var userInfo = UserInformation.Current;

            var school = (int?)GetParameterByName(parameters, SchoolParameterName).Value;
            if (null == school) return;

            var lea = (int?)GetParameterByName(parameters, ClaimValidatorRequest.LocalEducationAgencyParameterName).Value;
            if (null == lea) throw new UserAccessDeniedException(InvalidParameterErrorMessage);

            var hierarchy = AssertionProvider.GetEducationOrganizationHierarchy((int)school);
            if (!hierarchy.Contains((int)lea)) throw new UserAccessDeniedException(InvalidParameterErrorMessage);

            if (userInfo.AssociatedOrganizations.Any(
                n => (!HasNothingClaimType(n.ClaimTypes) &&
                        hierarchy.Contains(n.EducationOrganizationId)))) //The user has an explict claim for the school, it's LEA or higher
                        
                return;

            throw new UserAccessDeniedException(SecurityAssertionProvider.NoEducationOrganizationPermissionErrorMessage);
        }

        private void CheckStaff(IEnumerable<ParameterInstance> parameters)
        {
            var staffUSINullable = (long?)GetParameterByName(parameters, StaffParameterName).Value;
            if ((null == staffUSINullable) ||
                (0 == staffUSINullable))return;

            var schoolIdNullable = (int?)GetParameterByName(parameters, SchoolParameterName).Value;
            if ((null == schoolIdNullable) ||
                (0 == schoolIdNullable)) return;

            AssertionProvider.SchoolMustBeAssociatedWithStaff((int)schoolIdNullable, (int)staffUSINullable);
        }

        public void AuthorizeRequest(IEnumerable<ParameterInstance> parameters)
        {
            CheckLea(parameters);
            CheckSchool(parameters);
            CheckStaff(parameters);
        }
    }
}
