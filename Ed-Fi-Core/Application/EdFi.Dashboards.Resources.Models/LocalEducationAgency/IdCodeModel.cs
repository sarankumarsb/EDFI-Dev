// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;

namespace EdFi.Dashboards.Resources.Models.LocalEducationAgency
{
    [Serializable]
    public class IdCodeModel
    {
        public int LocalEducationAgencyId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
