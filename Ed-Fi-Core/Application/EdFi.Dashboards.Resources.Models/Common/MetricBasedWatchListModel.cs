using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Resources.Models.Common
{
    [Serializable]
    public class MetricBasedWatchListModel
    {
        /// <summary>
        /// Gets or sets the metric based watch list identifier.
        /// </summary>
        /// <value>
        /// The metric based watch list identifier.
        /// </value>
        public int MetricBasedWatchListId { get; set; }

        /// <summary>
        /// Gets or sets the staff usi.
        /// </summary>
        /// <value>
        /// The staff usi.
        /// </value>
        public long StaffUSI { get; set; }

        /// <summary>
        /// Gets or sets the education organization identifier.
        /// </summary>
        /// <value>
        /// The education organization identifier.
        /// </value>
        public int EducationOrganizationId { get; set; }

        /// <summary>
        /// Gets or sets the name of the watch list.
        /// </summary>
        /// <value>
        /// The name of the watch list.
        /// </value>
        public string WatchListName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the watch list is shared.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the watch list is shared; otherwise, <c>false</c>.
        /// </value>
        public bool IsWatchListShared { get; set; }

        /// <summary>
        /// Gets or sets the watch list description.
        /// </summary>
        /// <value>
        /// The watch list description.
        /// </value>
        public string WatchListDescription { get; set; }
    }
}
