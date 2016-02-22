// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities.CastleWindsorInstallers;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Navigation.UserEntryProviders
{
    [ChainOfResponsibilityOrder(100)]
    public class UserEntryForStateLevelProvider : ChainOfResponsibilityBase<IUserEntryProvider, EntryRequest, string>, IUserEntryProvider
    {
        private readonly ILocalEducationAgencyAreaLinks _localEducationAgencyLinks;
        private readonly IAdminAreaLinks _adminLinks;
        private readonly ICurrentUserClaimInterrogator _currentUserClaimInterrogator;

        public string GetUserEntry(EntryRequest request)
        {
            return base.ProcessRequest(request);
        }

        public UserEntryForStateLevelProvider(IUserEntryProvider next, ICurrentUserClaimInterrogator currentUserClaimInterrogator, IAdminAreaLinks adminLinks, ILocalEducationAgencyAreaLinks localEducationAgencyLinks) : base(next)
        {
            _currentUserClaimInterrogator = currentUserClaimInterrogator;
            _adminLinks = adminLinks;
            _localEducationAgencyLinks = localEducationAgencyLinks;
        }

        protected override bool CanHandleRequest(EntryRequest request)
        {
            return request.LocalEducationAgencyId.HasValue && UserInformation.Current.AssociatedStateAgencies.Any();
        }

        protected override string HandleRequest(EntryRequest request)
        {
            return GetStateLevelUserEntry(request);
        }

        private string GetStateLevelUserEntry(EntryRequest request)
        {
            if(!request.LocalEducationAgencyId.HasValue)
                throw new ArgumentNullException("request");

            var leaId = request.LocalEducationAgencyId.Value;

            return _currentUserClaimInterrogator.HasClaimForStateAgency(EdFiClaimTypes.AdministerDashboard)
                ? _adminLinks.Configuration(leaId)
                : _localEducationAgencyLinks.Overview(leaId);
        }
    }
}