// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities.CastleWindsorInstallers;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Navigation.UserEntryProviders
{
    [ChainOfResponsibilityOrder(200)]
    public class UserEntryForSystemAdministratorProvider : ChainOfResponsibilityBase<IUserEntryProvider, EntryRequest, string>, IUserEntryProvider
    {
        private readonly IAdminAreaLinks _adminLinks;

        public UserEntryForSystemAdministratorProvider(IUserEntryProvider next, IAdminAreaLinks adminLinks)
            : base(next)
        {
            _adminLinks = adminLinks;
        }

        public string GetUserEntry(EntryRequest request)
        {
            return base.ProcessRequest(request);
        }

        protected override bool CanHandleRequest(EntryRequest request)
        {
            var localEducationAgencyId = UserEntryProviderHelper.GetAssociatedLocalEducationAgencyWithClaim(UserInformation.Current, EdFiClaimTypes.AdministerDashboard, request);

            return (localEducationAgencyId != null &&
                    (request.LocalEducationAgencyId == null || (localEducationAgencyId == request.LocalEducationAgencyId)));
        }

        protected override string HandleRequest(EntryRequest request)
        {
            var localEducationAgencyId = UserEntryProviderHelper.GetAssociatedLocalEducationAgencyWithClaim(UserInformation.Current, EdFiClaimTypes.AdministerDashboard, request);

            return localEducationAgencyId.HasValue ? _adminLinks.Configuration(localEducationAgencyId.Value) : null;
        }
    }
}