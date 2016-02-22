using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
    public class QEduLoginInformationFromUsernameProvider : IStaffInformationFromEmailProvider
    {
         public IRepository<LoginDetails> LoginDetailsRepository { get; private set; }

        private const string NoStaffUSIAssociationErrorMessageFormat = "We were able to resolve username '{0}', but we were not able to find this resolved value in the StudentGPS system.  If you feel this is incorrect, please contact an administrator.";
        private const string MultipleStaffUSIAssociationErrorMessageFormat = "We were able to resolve username '{0}', but we found multiple staff users for this resolved value in the StudentGPS system.  If you feel this is incorrect, please contact an administrator.";

        public QEduLoginInformationFromUsernameProvider(IRepository<LoginDetails> loginDetailsRepository)
        {
            this.LoginDetailsRepository = loginDetailsRepository;
        }

        public long ResolveStaffUSI(string userName)
        {
            var userRecords = this.LoginDetailsRepository.GetAll().Where(x => x.LoginId == userName).ToList();

            if (userRecords.Count == 0)
                throw new DashboardsMissingStaffAuthenticationException(string.Format(NoStaffUSIAssociationErrorMessageFormat, userName)) { Name = userName };

            if (userRecords.Count > 1)
                throw new DashboardsMultipleStaffAuthenticationException(string.Format(MultipleStaffUSIAssociationErrorMessageFormat, userName)) { Name = userName };

            var userUSI = userRecords.First().USI;
            return userUSI;
        }
    }
}
