using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using System.Collections.Generic;

namespace EdFi.Dashboards.Presentation.Core.Models.Shared
{
    public class EdFiGridModel
    {
        public EdFiGridModel()
        {
            UsePersistedColumnData = true;
        }

        public string GridName { get; set; }
        public GridTable GridTable { get; set; }
        public string UniqueTableName { get; set; }
        public bool DisplayInteractionMenu { get; set; }
        public bool DisplayAddRemoveColumns { get; set; }
        public bool SizeToWindow { get; set; }
        public int FixedHeight { get; set; }
        public IList<MetricFootnote> MetricFootnotes { get; set; }
        public string NonPersistedSettings { get; set; }
        public EntityListModel EntityList { get; set; }
        public IList<string> LegendViewNames { get; set; }
        public int DefaultSortColumn { get; set; }
        public string HrefToUse { get; set; }
        public bool UsePersistedColumnData { get; set; }
        public bool UsePagination { get; set; }
        public string PaginationCallbackUrl { get; set; }
        public string ExportGridDataUrl { get; set; }
        public bool AllowMaximizeGrid { get; set; }

        /// <summary>
        /// Gets or sets the student watch list.
        /// </summary>
        /// <value>
        /// The student watch list.
        /// </value>
        public EdFiGridWatchListModel StudentWatchList { get; set; }

        /// <summary>
        /// Gets or sets the selected demographic option.
        /// </summary>
        /// <value>
        /// The selected demographic option.
        /// </value>
        public string SelectedDemographicOption { get; set; }

        /// <summary>
        /// Gets or sets the selected level option.
        /// </summary>
        /// <value>
        /// The selected level option.
        /// </value>
        public string SelectedSchoolCategoryOption { get; set; }

        /// <summary>
        /// Gets or sets the selected grade option.
        /// </summary>
        /// <value>
        /// The selected grade option.
        /// </value>
        public string SelectedGradeOption { get; set; }

        /// <summary>
        /// Gets or sets the previous/next controllers session page.
        /// </summary>
        /// <value>
        /// The previous/next controllers session page.
        /// </value>
        public string PreviousNextSessionPage { get; set; }

        /// <summary>
        /// Gets or sets the type of the list.
        /// </summary>
        /// <value>
        /// The type of the list.
        /// </value>
        public ListType ListType { get; set; }
    }

    public class EdFiGridLegendModel
    {
        public string CssClass { get; set; }
        public string Text { get; set; }
        public string Alt { get; set; }
    }

    public class EntityListModel
    {
        public int RowIndexForId { get; set; }
        public string MetricVariantId { get; set; }
        public string ListType { get; set; }
    }
}