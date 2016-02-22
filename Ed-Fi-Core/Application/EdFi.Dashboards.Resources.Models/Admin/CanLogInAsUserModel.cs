// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;

namespace EdFi.Dashboards.Resources.Models.Admin
{
    [Serializable]
    public class CanLogInAsUserModel
    {
        public bool CanLogIn { get; set; }
        public string Email { get; set; }
        public long StaffUSI { get; set; }
    }
}
