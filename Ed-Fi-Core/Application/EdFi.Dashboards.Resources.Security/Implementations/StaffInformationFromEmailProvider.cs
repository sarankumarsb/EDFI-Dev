using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
    public class StaffInformationFromEmailProvider : IStaffInformationFromEmailProvider
    {
        public IRepository<StaffInformation> StaffInformationRepository { get; private set; }

        private const string NoStaffUSIAssociationErrorMessageFormat = "We were able to resolve username '{0}', but we were not able to find this resolved value in the StudentGPS system.  If you feel this is incorrect, please contact an administrator.";
        private const string MultipleStaffUSIAssociationErrorMessageFormat = "We were able to resolve username '{0}', but we found multiple staff users for this resolved value in the StudentGPS system.  If you feel this is incorrect, please contact an administrator.";

        public StaffInformationFromEmailProvider(IRepository<StaffInformation> staffInformationRepository)
        {
            this.StaffInformationRepository = staffInformationRepository;
        }

        public long ResolveStaffUSI(string email)
        {
            var staffRecords = this.StaffInformationRepository.GetAll().Where(x => x.EmailAddress == email).ToList();

            if (staffRecords.Count == 0)
                throw new DashboardsMissingStaffAuthenticationException(string.Format(NoStaffUSIAssociationErrorMessageFormat, email)) { Name = email };

            if (staffRecords.Count > 1)
                throw new DashboardsMultipleStaffAuthenticationException(string.Format(MultipleStaffUSIAssociationErrorMessageFormat, email)) { Name = email };

            var staffUSI = staffRecords.First().StaffUSI;
            return staffUSI;
        }
    }
}
