using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace EdFi.Dashboards.Application.Resources.Models.Admin
{
    [Serializable]
    public class TitleClaimSetModel : PostResultsModel
    {
        public string CurrentOperation { get; set; }
        public int LocalEducationAgencyId { get; set; }

        public string PositionTitle { get; set; }
        public IDictionary<string, string> EdOrgPositionTitles { get; set; }

        public IDictionary<string, string> ClaimSetMaps { get; set; }

        public string ClaimSet { get; set; }
        public IDictionary<string, string> PossibleClaimSets { get; set; }

        public string FileName { get; set; }
        
        [IgnoreDataMember]
        public Stream FileInputStream { get; set; }
    }
}
