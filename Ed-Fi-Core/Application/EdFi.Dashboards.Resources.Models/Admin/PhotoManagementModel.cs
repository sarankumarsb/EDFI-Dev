// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resources.Models.School;

namespace EdFi.Dashboards.Resources.Models.Admin
{
    [Serializable]
    public class PhotoManagementModel
    {
        public List<SchoolModel> Schools { get; set; } 
        public int? TotalRecords { get; set; }
        public int? SuccessfullyProcessedPhotos { get; set; }
        public List<string> ErrorMessages { get; set; }

        public PhotoManagementModel()
        {
            Schools = new List<SchoolModel>();
        }
    }
}
