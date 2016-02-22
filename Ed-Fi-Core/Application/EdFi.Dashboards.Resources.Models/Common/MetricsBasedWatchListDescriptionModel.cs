using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Models.Common
{
    /// <summary>
    /// The model returned from the watch list description service.
    /// </summary>
    public class MetricsBasedWatchListDescriptionModel
    {
        /// <summary>
        /// Gets or sets the watch list description.
        /// </summary>
        /// <value>
        /// The watch list description.
        /// </value>
        public string WatchListDescription { get; set; }

        /// <summary>
        /// Gets or sets the watch list selections.
        /// </summary>
        /// <value>
        /// The watch list selections.
        /// </value>
        public string WatchListSelections { get; set; }
    }
}
