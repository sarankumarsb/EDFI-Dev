using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Application.Data.Entities;

namespace EdFi.Dashboards.Resources.Models.Common
{
    /// <summary>
    /// Used to add extra data onto the watch list so it can be viewed
    /// properly.
    /// </summary>
    [Serializable]
    public class MetricBasedWatchListViewModel
    {
        /// <summary>
        /// Gets or sets the name of the teacher.
        /// </summary>
        /// <value>
        /// The name of the teacher.
        /// </value>
        public string TeacherName { get; set; }

        /// <summary>
        /// Gets or sets the watch lists.
        /// </summary>
        /// <value>
        /// The watch lists.
        /// </value>
        public List<MetricBasedWatchListModel> WatchLists { get; set; }
    }
}
