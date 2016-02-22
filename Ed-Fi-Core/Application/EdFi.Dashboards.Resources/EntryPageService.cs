// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources.Navigation.UserEntryProviders;
using EdFi.Dashboards.Resources.Security.Common;

namespace EdFi.Dashboards.Resources
{
    public class EntryRequest
    {
        public int? LocalEducationAgencyId { get; set; }
        public int? SchoolId { get; set; }

        public static EntryRequest Create(int? localEducationAgencyId, int? schoolId)
        {
            return new EntryRequest
                       {
                           LocalEducationAgencyId = localEducationAgencyId,
                           SchoolId = schoolId
                       };
        }
    }

    public interface IEntryService
    {
        [NoCache]
        string Get(EntryRequest request);
    }

    public class EntryService : IEntryService
    {
        private readonly IUserEntryProvider _userEntryProvider;

        public EntryService(IUserEntryProvider userEntryProvider)
        {
            _userEntryProvider = userEntryProvider;
        }

        [NoCache]
        [AuthenticationIgnore(
            "This service uses the user's claims to determine a landing page URL within the application based on the" +
            "LEA/School identifiers passed in.  No information that the current user cannot see will be returned."
            )]
        public string Get(EntryRequest request)
        {
            return _userEntryProvider.GetUserEntry(request);
        }
    }
}
