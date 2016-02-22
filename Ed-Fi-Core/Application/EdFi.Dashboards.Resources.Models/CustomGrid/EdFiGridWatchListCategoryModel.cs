using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Models.CustomGrid
{
    /// <summary>
    /// Represents the entire grid watch list.
    /// </summary>
    [Serializable]
    public class EdFiGridWatchListModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdFiGridWatchListModel"/> class.
        /// </summary>
        public EdFiGridWatchListModel()
        {
            Tabs = new List<EdFiGridWatchListTabModel>();
        }

        /// <summary>
        /// Gets or sets the tabs.
        /// </summary>
        /// <value>
        /// The tabs.
        /// </value>
        public List<EdFiGridWatchListTabModel> Tabs { get; set; }

        /// <summary>
        /// Gets or sets the name of the watch list.
        /// </summary>
        /// <value>
        /// The name of the watch list.
        /// </value>
        public string WatchListName { get; set; }

        /// <summary>
        /// Gets or sets the watch list description.
        /// </summary>
        /// <value>
        /// The watch list description.
        /// </value>
        public string WatchListDescription { get; set; }

        /// <summary>
        /// Gets or sets the watch list URL.
        /// </summary>
        /// <value>
        /// The watch list URL.
        /// </value>
        public string WatchListUrl { get; set; }

        /// <summary>
        /// Gets or sets the watch list search URL.
        /// </summary>
        /// <value>
        /// The watch list search URL.
        /// </value>
        public string WatchListSearchUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the watch list is changed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the watch list is changed; otherwise, <c>false</c>.
        /// </value>
        public bool IsWatchListChanged { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the watch list is shared.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the watch list is shared; otherwise, <c>false</c>.
        /// </value>
        public bool IsWatchListShared { get; set; }
    }

    /// <summary>
    /// Represents a tab in the watch list feature.
    /// </summary>
    [Serializable]
    public class EdFiGridWatchListTabModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdFiGridWatchListTabModel"/> class.
        /// </summary>
        public EdFiGridWatchListTabModel()
        {
            Columns = new List<EdFiGridWatchListColumnModel>();
        }

        /// <summary>
        /// Gets or sets the display text.
        /// </summary>
        /// <value>
        /// The display text.
        /// </value>
        public string DisplayText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is the active tab.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this is the active tab; otherwise, <c>false</c>.
        /// </value>
        public bool IsActiveTab { get; set; }

        /// <summary>
        /// Gets or sets the name of the tab.
        /// </summary>
        /// <value>
        /// The tab name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the columns for a tab page.
        /// </summary>
        /// <value>
        /// The tab page columns.
        /// </value>
        public List<EdFiGridWatchListColumnModel> Columns { get; set; }
    }

    /// <summary>
    /// Represents a column within a tab page.
    /// </summary>
    [Serializable]
    public class EdFiGridWatchListColumnModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdFiGridWatchListColumnModel"/> class.
        /// </summary>
        public EdFiGridWatchListColumnModel()
        {
            Templates = new List<EdFiGridWatchListTemplateModel>();
        }

        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        /// <value>
        /// The categories.
        /// </value>
        public List<EdFiGridWatchListTemplateModel> Templates { get; set; }
    }

    /// <summary>
    /// Rekpresents a call to one of the custom partial views.
    /// </summary>
    [Serializable]
    public class EdFiGridWatchListTemplateModel
    {
        /// <summary>
        /// Gets or sets the group display value.
        /// </summary>
        /// <value>
        /// The group display value.
        /// </value>
        public string GroupDisplayValue { get; set; }

        /// <summary>
        /// Gets or sets the partial name to call.
        /// </summary>
        /// <value>
        /// The partial name.
        /// </value>
        public string TemplateName { get; set; }

        /// <summary>
        /// Gets or sets the view model to pass to the partial.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        public object ViewModel { get; set; }
    }

    /// <summary>
    /// Contains the common fields contained in each selection model object.
    /// </summary>
    [Serializable]
    public class EdFiGridWatchListBaseSelectionModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display value.
        /// </summary>
        /// <value>
        /// The display value.
        /// </value>
        public string DisplayValue { get; set; }

        /// <summary>
        /// Gets or sets the selection value; this is used when the display
        /// value should be blank but a display value is needed for the
        /// "Select Values" section.
        /// </summary>
        /// <value>
        /// The selection value.
        /// </value>
        public string SelectionValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this item is shown in the filter list.
        /// </summary>
        /// <value>
        /// <c>true</c> if this is shown in the filter list; otherwise, <c>false</c>.
        /// </value>
        public bool IsShownInFilterList { get; set; }
    }

    /// <summary>
    /// Represents objects that allow for a single selection; checkboxes use
    /// this model since each one can represent a single selection.
    /// </summary>
    [Serializable]
    public class EdFiGridWatchListSingleSelectionModel : EdFiGridWatchListBaseSelectionModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdFiGridWatchListSingleSelectionModel"/> class.
        /// </summary>
        public EdFiGridWatchListSingleSelectionModel()
        {
            Values = new List<EdFiGridWatchListSelectionItemModel>();
        }

        /// <summary>
        /// Gets or sets the key/value pair that will be used to create the
        /// controls.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public List<EdFiGridWatchListSelectionItemModel> Values { get; set; }
    }

    /// <summary>
    /// This model is used to load data for controls where child controls are
    /// loaded based upon a parent control selection.
    /// </summary>
    [Serializable]
    public class EdFiGridWatchListDoubleSelectionModel : EdFiGridWatchListBaseSelectionModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdFiGridWatchListDoubleSelectionModel"/> class.
        /// </summary>
        public EdFiGridWatchListDoubleSelectionModel()
        {
            Comparisons = new List<EdFiGridWatchListSelectionItemModel>();
            Values = new List<EdFiGridWatchListSelectionItemModel>();
        }

        /// <summary>
        /// Gets or sets the values that will be loaded into the first drop
        /// down.
        /// </summary>
        /// <value>
        /// The comparisons.
        /// </value>
        public List<EdFiGridWatchListSelectionItemModel> Comparisons { get; set; }

        /// <summary>
        /// Gets or sets the values that will be loaded into the second drop
        /// down based upon the first drop down.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public List<EdFiGridWatchListSelectionItemModel> Values { get; set; }
    }

    [Serializable]
    public class EdFiGridWatchListDoubleSelectionTextboxModel : EdFiGridWatchListBaseSelectionModel
    {
        public const string TextboxRangeFormat = "range";
        public const string TextboxPercentFormat = "percent";

        /// <summary>
        /// Initializes a new instance of the <see cref="EdFiGridWatchListDoubleSelectionTextboxModel"/> class.
        /// </summary>
        public EdFiGridWatchListDoubleSelectionTextboxModel()
        {
            Comparisons = new List<EdFiGridWatchListSelectionItemModel>();
        }

        /// <summary>
        /// Gets or sets the values that will be loaded into the drop down.
        /// </summary>
        /// <value>
        /// The comparisons.
        /// </value>
        public List<EdFiGridWatchListSelectionItemModel> Comparisons { get; set; }

        /// <summary>
        /// Gets or sets the textbox format.
        /// </summary>
        /// <value>
        /// The textbox format.
        /// </value>
        public string TextboxFormat { get; set; }

        /// <summary>
        /// Gets or sets the textbox minimum value.
        /// </summary>
        /// <value>
        /// The textbox minimum value.
        /// </value>
        public int TextboxMinValue { get; set; }

        /// <summary>
        /// Gets or sets the textbox maximum value.
        /// </summary>
        /// <value>
        /// The textbox maximum value.
        /// </value>
        public int TextboxMaxValue { get; set; }
    }

    /// <summary>
    /// Instead of having the values be a Dictionary of type string, string
    /// it will need to be of this type so it will produce the proper Json.
    /// </summary>
    [Serializable]
    public class EdFiGridWatchListSelectionItemModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdFiGridWatchListSelectionItemModel"/> class.
        /// </summary>
        public EdFiGridWatchListSelectionItemModel()
        {
            IsSelected = false;
        }

        /// <summary>
        /// Gets or sets the name given to the item.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display value.
        /// </summary>
        /// <value>
        /// The display value.
        /// </value>
        public string DisplayValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the item is selected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the item is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected { get; set; }
    }
}
