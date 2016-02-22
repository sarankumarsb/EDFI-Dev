using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.SecurityTokenService.Authentication
{
    public interface IStaffInformationProvider
    {
        int UserType { get; set; } // VIN22012016

        long ResolveStaffUSI(IAuthenticationProvider authenticationProvider, string username);
        string ResolveUsername(IAuthenticationProvider authenticationProvider, string staffUSI, int userType);
    }
}
