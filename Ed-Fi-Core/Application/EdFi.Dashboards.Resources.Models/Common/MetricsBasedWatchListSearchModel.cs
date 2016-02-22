using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Application.Data.Entities;

namespace EdFi.Dashboards.Resources.Models.Common
{
    [Serializable]
    public class MetricsBasedWatchListSearchModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetricsBasedWatchListSearchModel"/> class.
        /// </summary>
        public MetricsBasedWatchListSearchModel()
        {
            WatchListGroups = new List<MetricBasedWatchListViewModel>();
        }

        /// <summary>
        /// Gets or sets the watch lists.
        /// </summary>
        /// <value>
        /// The watch lists.
        /// </value>
        public List<MetricBasedWatchListViewModel> WatchListGroups { get; set; }

        /// <summary>
        /// Gets or sets the referring controller.
        /// </summary>
        /// <value>
        /// The referring controller.
        /// </value>
        public string ReferringController { get; set; }

        /// <summary>
        /// Gets or sets the description service URL.
        /// </summary>
        /// <value>
        /// The description service URL.
        /// </value>
        public string DescriptionServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets the unshare service URL.
        /// </summary>
        /// <value>
        /// The unshare service URL.
        /// </value>
        public string UnshareServiceUrl { get; set; }
    }
}
