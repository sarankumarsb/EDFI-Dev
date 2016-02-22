// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;

namespace EdFi.Dashboards.Resources.Models.Student
{
    [Serializable]
    public class IdNameModel : IStudent
    {
        public long StudentUSI { get; set; }
        public string FullName { get; set; }
    }
}
