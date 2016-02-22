using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
    /// <summary>
    /// Validate the login credentials with newly created login table.
    /// </summary>
    public class QEduLoginInformationProvider : IStaffInformationProvider
    {

        private const string NoLookupValueErrorMessageFormat = "We were able to verify your password, but were unable to resolve your lookup value from username '{0}' to field '{1}'.  If you feel this is incorrect, please contact an administrator.";
        private const string NoStaffUSIAssociationErrorMessageFormat = "We were able to resolve username '{0}', but we were not able to find this resolved value in the StudentGPS system.  If you feel this is incorrect, please contact an administrator.";
        private const string MultipleStaffUSIAssociationErrorMessageFormat = "We were able to resolve username '{0}', but we found multiple staff users for this resolved value in the StudentGPS system.  If you feel this is incorrect, please contact an administrator.";

        private readonly Dictionary<string, string> _usernamesByStaffUSI = new Dictionary<string, string>();
        private readonly IRepository<LoginDetails> _LoginInfoRepository;

        public int UserType { get; set; } // VIN22012016

        public QEduLoginInformationProvider(IRepository<LoginDetails> LoginInfoRepo)
        {
            this._LoginInfoRepository = LoginInfoRepo;
        }

        public long ResolveStaffUSI(IAuthenticationProvider authenticationProvider, string username)
        {
            // VIN22012016
            //var staffUSIs = _LoginInfoRepository.GetAll().Where(cond => cond.LoginId == username).Select(sel => sel.USI);
            var staffUSIs = _LoginInfoRepository.GetAll().Where(cond => cond.LoginId == username); //.Select(sel => sel.USI);            

            if (staffUSIs.Count() == 0)
                throw new DashboardsMissingStaffAuthenticationException(string.Format(NoStaffUSIAssociationErrorMessageFormat, username)) { Name = username };

            if (staffUSIs.Count() > 1)
                throw new DashboardsMultipleStaffAuthenticationException(string.Format(MultipleStaffUSIAssociationErrorMessageFormat, username)) { Name = username };

            long staffUSI = 0;
            foreach (var user in staffUSIs)
            {
                staffUSI = user.USI;
                UserType = user.UserType;
                break;
            }            
            return staffUSI;
        }

        public string ResolveUsername(IAuthenticationProvider authenticationProvider, string staffUSI, int userType)
        {            
            if (!_usernamesByStaffUSI.ContainsKey(staffUSI))
            {
                var staffUSIs = _LoginInfoRepository.GetAll().Where(cond => cond.USI == Convert.ToInt32(staffUSI) && cond.UserType == Convert.ToInt32(userType)).Select(sel => sel.LoginId); // VIN22012016

                if (staffUSIs.Count() == 0)
                    throw new DashboardsMissingStaffAuthenticationException(string.Format(NoStaffUSIAssociationErrorMessageFormat, staffUSI)) { Name = staffUSI };

                if (staffUSIs.Count() > 1)
                    throw new DashboardsMultipleStaffAuthenticationException(string.Format(MultipleStaffUSIAssociationErrorMessageFormat, staffUSI)) { Name = staffUSI };

                _usernamesByStaffUSI[staffUSI] = staffUSIs.First();
            }

            return _usernamesByStaffUSI[staffUSI];
        }


    }
}
