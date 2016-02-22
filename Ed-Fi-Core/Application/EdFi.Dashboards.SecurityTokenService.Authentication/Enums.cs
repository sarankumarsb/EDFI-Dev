// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;

namespace EdFi.Dashboards.SecurityTokenService.Authentication
{
    public enum ClaimsSet
    {
        SystemAdministrator,
        Superintendent,
        Principal,
        Administration,
        Leader,
        Specialist,
        Staff,
        Student, // VINSTUDLOGIN
        // this is used so that the System Administrator can log in as another user while the site is deactivated for the Local Education Agency
        // it should not affect security filtering or anything else
        Impersonation
    }
}
