// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using Microsoft.IdentityModel.Claims;

namespace EdFi.Dashboards.SecurityTokenService.Authentication
{
    public class UserClaimsData
    {
        public string Username { get; set; }
        //public string Email { get; set; }
        public long StaffUSI { get; set; }
        public IEnumerable<Claim> Claims { get; set; }
    }
}