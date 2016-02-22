using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities.CastleWindsorInstallers;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Navigation.UserEntryProviders
{
    [ChainOfResponsibilityOrder(800)]
    public class UserEntryForStudentProvider : ChainOfResponsibilityBase<IUserEntryProvider, EntryRequest, string>, IUserEntryProvider
    {
        private readonly IStudentSchoolAreaLinks _schoolLinks;

        public UserEntryForStudentProvider(IUserEntryProvider next, IStudentSchoolAreaLinks schoolLinks)
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
            var associatedSchools = UserEntryProviderHelper.GetAssociatedSchoolsWithClaim(UserInformation.Current, EdFiClaimTypes.ViewMyMetrics, request);

            /*if (UserInformation.Current.StaffUSI < 0) //VINSTUDLOGIN
                return UserEntryProviderHelper.GetValidAssociatedSchools(associatedSchools, request.SchoolId).Any();
            else
                return false;*/
            if (UserInformation.Current.UserType == 2) // EDFIDB-139
                return UserEntryProviderHelper.GetValidAssociatedSchools(associatedSchools, request.SchoolId).Any();
            else
                return false;
        }

        protected override string HandleRequest(EntryRequest request)
        {
            var associatedSchools = UserEntryProviderHelper.GetAssociatedSchoolsWithClaim(UserInformation.Current, EdFiClaimTypes.ViewMyMetrics, request);
            var associatedSchool = UserEntryProviderHelper.GetValidAssociatedSchools(associatedSchools, request.SchoolId).First();

            return _schoolLinks.Overview(associatedSchool.SchoolId, (UserInformation.Current.StaffUSI));
        }
    }
}