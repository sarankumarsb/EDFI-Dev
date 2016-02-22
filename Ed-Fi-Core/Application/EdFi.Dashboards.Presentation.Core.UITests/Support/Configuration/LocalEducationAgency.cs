using System.Collections.Generic;

namespace EdFi.Dashboards.Presentation.Core.UITests.Support.Configuration
{
    /// <summary>
    /// Provides details about a specific Local Education Agency for use in testing scenarios.
    /// </summary>
    public class LocalEducationAgency
    {
        /// <summary>
        /// Gets or sets the code used in the Dashboard URLs to represent the Local Education Agency.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the user profiles for testing the Dashboards with the Local Education Agency.
        /// </summary>
        public List<UserProfile> UserProfiles { get; set; }
    }
}
