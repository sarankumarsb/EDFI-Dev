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
    public class SearchAuthorizationDelegate : ParameterAuthorizationBase, IAuthorizationDelegate
    {
        public SearchAuthorizationDelegate(ISecurityAssertionProvider securityAssertionProvider) :
            base(securityAssertionProvider, null)
        {
        }

        public void CurrentUserHasAnyClaim()
        {
            var userInfo = UserInformation.Current;
            if (userInfo.AssociatedOrganizations.Any(
                n => ((false == HasNothingClaimType(n.ClaimTypes)))))
                return;

            throw new UserAccessDeniedException(SecurityAssertionProvider.NoEducationOrganizationPermissionErrorMessage);
        }

        public void AuthorizeRequest(IEnumerable<ParameterInstance> parameters)
        {
            CurrentUserHasAnyClaim();
        }
    }
}
