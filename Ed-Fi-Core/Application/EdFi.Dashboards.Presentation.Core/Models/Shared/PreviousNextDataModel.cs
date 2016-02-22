// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using EdFi.Dashboards.Resources.Models.Common;
using System.Collections.Generic;

namespace EdFi.Dashboards.Presentation.Core.Models.Shared
{
    public class PreviousNextDataModel
    {
        public string ListUrl { get; set; }
        public string TableId { get; set; }
        public string MetricId { get; set; }
        /// <summary>
        /// The type of list we have. Values are usually StudentList, StaffList, etc...
        /// </summary>
        public string ListType { get; set; }
        public string[] ParameterNames { get; set; } //["StudentUSI", "SchoolID"]
        public long[][] EntityIdArray { get; set; } // [[34,444],[35,444]]
        public string ListPersistenceUniqueId { get; set; }
        public bool FromSearch { get; set; }
        public int? SortColumn { get; set; }
        public string SortDirection { get; set; }

        /// <summary>
        /// Gets or sets the student watch list data. This will be used to
        /// reload the watch list when the return to list link is clicked.
        /// </summary>
        /// <value>
        /// The student watch list data.
        /// </value>
        public List<NameValuesType> StudentWatchListData { get; set; }
    }
}
