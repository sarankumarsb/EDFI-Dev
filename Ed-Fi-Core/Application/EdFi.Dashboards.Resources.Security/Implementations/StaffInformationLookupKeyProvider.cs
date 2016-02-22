using System.Linq;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
    public class StaffInformationLookupKeyProvider : IStaffInformationLookupKeyProvider
    {
        private const string StaffInformationEmailProperty = "EmailAddress";
        private readonly ILocalEducationAgencyContextProvider _leaContextProvider;
        private readonly IRepository<Dashboards.Application.Data.Entities.LocalEducationAgency> _localEducationAgencyRepository;
        private readonly IRepository<LocalEducationAgencyAuthentication> _localEducationAgencyAuthenticationRepository;

        public StaffInformationLookupKeyProvider(ILocalEducationAgencyContextProvider localEducationAgencyContextProvider,
            IRepository<Dashboards.Application.Data.Entities.LocalEducationAgency> localEducationAgencyRepository,
            IRepository<LocalEducationAgencyAuthentication> localEducationAgencyAuthenticationRepository)
        {
            _leaContextProvider = localEducationAgencyContextProvider;
            _localEducationAgencyRepository = localEducationAgencyRepository;
            _localEducationAgencyAuthenticationRepository = localEducationAgencyAuthenticationRepository;
        }

        public string GetStaffInformationLookupKey()
        {
            var leaCode = _leaContextProvider.GetCurrentLocalEducationAgencyCode();

            var staffInfoLookupKey =
                (from lea in _localEducationAgencyRepository.GetAll()
                     .Where(x => x.Code == leaCode)
                 from leaAuth in _localEducationAgencyAuthenticationRepository.GetAll()
                     .Where(x => x.LocalEducationAgencyId == lea.LocalEducationAgencyId)
                     .DefaultIfEmpty()
                 select leaAuth.StaffInformationLookupKey).SingleOrDefault();

            return !string.IsNullOrWhiteSpace(staffInfoLookupKey) ? staffInfoLookupKey : StaffInformationEmailProperty;
        }
    }
}
