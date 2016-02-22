using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Resources.Models.CustomGrid;

namespace EdFi.Dashboards.Resources.Common
{
    public class WatchListDataState
    {
        /// <summary>
        /// The current tab count used to make the tab id's unique.
        /// </summary>
        public int CurrentTabIndex;

        /// <summary>
        /// This holds the section id passed into the GetEdFiGridWatchListModel
        /// method. This is needed to allow for other methods to use this value.
        /// </summary>
        public long CurrentSectionId;

        /// <summary>
        /// Gets or sets the students.
        /// </summary>
        /// <value>
        /// The students.
        /// </value>
        public virtual IQueryable<EnhancedStudentInformation> Students { get; set; }

        /// <summary>
        /// Gets or sets the metrics.
        /// </summary>
        /// <value>
        /// The metrics.
        /// </value>
        public virtual IQueryable<StudentMetric> Metrics { get; set; }

        /// <summary>
        /// Gets or sets the grades.
        /// </summary>
        /// <value>
        /// The grades.
        /// </value>
        public virtual List<EdFiGridWatchListSelectionItemModel> Grades { get; set; }

        /// <summary>
        /// Gets or sets the metrics ids.
        /// </summary>
        /// <value>
        /// The metrics ids.
        /// </value>
        public virtual List<int> MetricIds { get; set; }

        /// <summary>
        /// Gets or sets the sections.
        /// </summary>
        /// <value>
        /// The sections.
        /// </value>
        public virtual Dictionary<int, string> Sections { get; set; }
    }
}
