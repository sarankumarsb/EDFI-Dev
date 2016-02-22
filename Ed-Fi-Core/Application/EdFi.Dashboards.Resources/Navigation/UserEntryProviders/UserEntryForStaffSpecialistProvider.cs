// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities.CastleWindsorInstallers;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Navigation.UserEntryProviders
{
    [ChainOfResponsibilityOrder(700)]
    public class UserEntryForStaffSpecialistProvider : ChainOfResponsibilityBase<IUserEntryProvider, EntryRequest, string>, IUserEntryProvider
    {
        private readonly ILocalEducationAgencyAreaLinks _localEducationAgencyLinks;

        public UserEntryForStaffSpecialistProvider(IUserEntryProvider next, ILocalEducationAgencyAreaLinks localEducationAgencyLinks)
            : base(next)
        {
            _localEducationAgencyLinks = localEducationAgencyLinks;
        }

        public string GetUserEntry(EntryRequest request)
        {
            return base.ProcessRequest(request);
        }

        protected override bool CanHandleRequest(EntryRequest request)
        {
            if (UserInformation.Current.UserType == 1) // EDFIDB-139
            {
                var localEducationAgencyId = UserEntryProviderHelper.GetAssociatedLocalEducationAgencyWithClaim(UserInformation.Current, EdFiClaimTypes.ViewMyMetrics, request);

                return (localEducationAgencyId != null &&
                        (request.LocalEducationAgencyId == null || (localEducationAgencyId == request.LocalEducationAgencyId)));
            }
            else { return false; }
        }

        protected override string HandleRequest(EntryRequest request)
        {
            var localEducationAgencyId = UserEntryProviderHelper.GetAssociatedLocalEducationAgencyWithClaim(UserInformation.Current, EdFiClaimTypes.ViewMyMetrics, request);

            return localEducationAgencyId.HasValue ? _localEducationAgencyLinks.Overview(localEducationAgencyId.Value) : null;
        }
    }
}