using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
    public class LiteralValueStaffInformationLookupKeyProvider : IStaffInformationLookupKeyProvider
    {
        private readonly string _staffInformationLookupKey;

        public LiteralValueStaffInformationLookupKeyProvider(string staffInformationLookupKey)
        {
            _staffInformationLookupKey = staffInformationLookupKey;
        }

        public string GetStaffInformationLookupKey()
        {
            return _staffInformationLookupKey;
        }
    }
}