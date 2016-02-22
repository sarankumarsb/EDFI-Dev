using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.Student;

namespace EdFi.Dashboards.Resources.Models.CustomGrid
{
    [Serializable]
    public class EdFiGridModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdFiGridModel"/> class.
        /// </summary>
        public EdFiGridModel()
        {
            EntityIds = new List<StudentSchoolIdentifier>();
            GridTable = new GridTable();
            ListMetadata = new List<MetadataColumnGroup>();
            Students = new List<StudentWithMetricsAndAccommodations>();
        }

        /// <summary>
        /// Gets or sets the entity ids.
        /// </summary>
        /// <value>
        /// The entity ids.
        /// </value>
        public List<StudentSchoolIdentifier> EntityIds { get; set; }

        /// <summary>
        /// Gets or sets the grid table.
        /// </summary>
        /// <value>
        /// The grid table.
        /// </value>
        public GridTable GridTable { get; set; }

        /// <summary>
        /// Gets or sets the list metadata.
        /// </summary>
        /// <value>
        /// The list metadata.
        /// </value>
        public List<MetadataColumnGroup> ListMetadata { get; set; }

        /// <summary>
        /// Gets or sets the students.
        /// </summary>
        /// <value>
        /// The students.
        /// </value>
        public List<StudentWithMetricsAndAccommodations> Students { get; set; }

        /// <summary>
        /// Gets or sets the previous/next controllers session page.
        /// </summary>
        /// <value>
        /// The previous/next controllers session page.
        /// </value>
        public string PreviousNextSessionPage { get; set; }

        /// <summary>
        /// Gets or sets the export grid data URL.
        /// </summary>
        /// <value>
        /// The export grid data URL.
        /// </value>
        public string ExportGridDataUrl { get; set; }
    }
}
