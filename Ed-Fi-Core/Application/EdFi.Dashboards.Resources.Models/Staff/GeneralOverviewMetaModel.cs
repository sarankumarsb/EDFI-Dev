// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resources.Models.CustomGrid;

namespace EdFi.Dashboards.Resources.Models.Staff
{
    [Serializable]
    public class GeneralOverviewMetaModel
    {
        public GeneralOverviewMetaModel()
        {
            ListMetadata = new List<MetadataColumnGroup>();
        }

        public List<MetadataColumnGroup> ListMetadata { get; set; }
    }
}