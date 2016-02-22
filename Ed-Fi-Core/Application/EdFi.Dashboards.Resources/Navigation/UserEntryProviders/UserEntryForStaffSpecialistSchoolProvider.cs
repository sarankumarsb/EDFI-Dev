// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities.CastleWindsorInstallers;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Navigation.UserEntryProviders
{
    [ChainOfResponsibilityOrder(500)]
    public class UserEntryForStaffSpecialistSchoolProvider : ChainOfResponsibilityBase<IUserEntryProvider, EntryRequest, string>, IUserEntryProvider
    {
        private readonly ISchoolAreaLinks _schoolLinks;

        public UserEntryForStaffSpecialistSchoolProvider(IUserEntryProvider next, ISchoolAreaLinks schoolLinks)
            : base(next)
        {
            _schoolLinks = schoolLinks;
        }

        public string GetUserEntry(EntryRequest request)
        {
            return base.ProcessRequest(request);
        }

        protected override bool CanHandleRequest(EntryRequest request)
        {
            if (UserInformation.Current.UserType == 1) // EDFIDB-139
            {
                var associatedSchools = UserEntryProviderHelper.GetAssociatedSchoolsWithClaim(UserInformation.Current, EdFiClaimTypes.ViewMyMetrics, request);

                return UserEntryProviderHelper.GetValidAssociatedSchools(associatedSchools, request.SchoolId).Any();
            }
            else { return false; }
        }

        protected override string HandleRequest(EntryRequest request)
        {
            var associatedSchools = UserEntryProviderHelper.GetAssociatedSchoolsWithClaim(UserInformation.Current, EdFiClaimTypes.ViewMyMetrics, request);
            var associatedSchool = UserEntryProviderHelper.GetValidAssociatedSchools(associatedSchools, request.SchoolId).First();

            return _schoolLinks.Overview(associatedSchool.SchoolId, associatedSchool.SchoolName);
        }
    }
}