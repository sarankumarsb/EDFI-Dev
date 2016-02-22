using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Models.Application
{
    [Serializable]
    public class HomeModel
    {        
        /// <summary>
        /// Gets or sets the collection of local education agencies available in the system.
        /// </summary>
        public IEnumerable<LocalEducationAgency> LocalEducationAgencies { get; set; }

        /// <summary>
        /// Provides basic information required for creating routes to the configured local education agencies.
        /// </summary>
        [Serializable]
        public class LocalEducationAgency
        {
            /// <summary>
            /// Gets or sets the code used for the district routes.
            /// </summary>
            public string LocalEducationAgencyCode { get; set; }

            /// <summary>
            /// Gets or sets the name of the local education agency.
            /// </summary>
            public string LocalEducationAgencyName { get; set; }

            /// <summary>
            /// Gets or sets the URL for the local education agency's dashboards home page.
            /// </summary>
            public string HomeUrl { get; set; }
        }
    }
}
