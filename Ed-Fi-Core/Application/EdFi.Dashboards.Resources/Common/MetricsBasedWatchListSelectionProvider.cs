using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;

namespace EdFi.Dashboards.Resources.Common
{
    public interface IMetricsBasedWatchListSelectionProvider
    {
        /// <summary>
        /// Creates the selection text from a loaded watch list model.
        /// </summary>
        /// <param name="request">The request used to create the selection text.</param>
        /// <returns>A string representation of the loaded watch list data.</returns>
        string CreateSelectionText(EdFiGridWatchListModel request);
    }

    /// <summary>
    /// Allows for the watch list selections to be represented as a string.
    /// </summary>
    public class MetricsBasedWatchListSelectionProvider : IMetricsBasedWatchListSelectionProvider
    {
        /// <summary>
        /// Creates the selection text from a loaded watch list model.
        /// </summary>
        /// <param name="request">The request used to create the selection text.</param>
        /// <returns>
        /// A string representation of the loaded watch list data.
        /// </returns>
        public string CreateSelectionText(EdFiGridWatchListModel request)
        {
            var selectionText = CreateModelSelections(request);
            return selectionText;
        }

        /// <summary>
        /// Creates the model selections from the watch list model data.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        protected virtual string CreateModelSelections(EdFiGridWatchListModel model)
        {
            // first get all of the typed view models
            var viewModels = new List<object>();
            foreach (var column in model.Tabs.SelectMany(tab => tab.Columns))
            {
                viewModels.AddRange(column.Templates.GetViewModelsFromTemplates());
            }

            var itemList = new List<NameValuesType>();
            foreach (var viewModel in viewModels)
            {
                switch (viewModel.GetType().Name)
                {
                    case "EdFiGridWatchListSingleSelectionModel":
                        var singleSelectionModel = viewModel as EdFiGridWatchListSingleSelectionModel;
                        if (singleSelectionModel == null)
                            continue;

                        var selectedSingleValues = singleSelectionModel.Values.Where(item => item.IsSelected).ToList();
                        if (selectedSingleValues.Any())
                        {
                            var selectedItemValues = selectedSingleValues.Select(selectedValue => selectedValue.DisplayValue).ToList();
                            itemList.Add(new NameValuesType
                            {
                                Name = singleSelectionModel.DisplayValue,
                                Values = selectedItemValues
                            });
                        }
                        break;
                    case "EdFiGridWatchListDoubleSelectionModel":
                        var doubleSelectionModel = viewModel as EdFiGridWatchListDoubleSelectionModel;
                        if (doubleSelectionModel == null)
                            continue;

                        var selectedComparisons = doubleSelectionModel.Comparisons.Where(item => item.IsSelected).ToList();
                        if (selectedComparisons.Any())
                        {
                            var dropDownComparisonValues = selectedComparisons.Select(item => item.DisplayValue).ToList();
                            // if there are defined values then check to see
                            // which one is selected
                            if (doubleSelectionModel.Values != null)
                            {
                                var selectedValues = doubleSelectionModel.Values.Where(item => item.IsSelected).ToList();

                                if (selectedValues.Any())
                                {
                                    dropDownComparisonValues.Add(
                                        selectedValues.Select(item => item.DisplayValue).SingleOrDefault());
                                }
                            }

                            itemList.Add(new NameValuesType
                            {
                                Name = doubleSelectionModel.DisplayValue,
                                Values = dropDownComparisonValues
                            });
                        }
                        break;
                    case "EdFiGridWatchListDoubleSelectionTextboxModel":
                        var doubleSelectionTextboxModel = viewModel as EdFiGridWatchListDoubleSelectionTextboxModel;
                        if (doubleSelectionTextboxModel == null)
                            continue;

                        var selectedTextboxComparisons = doubleSelectionTextboxModel.Comparisons.Where(item => item.IsSelected).ToList();
                        if (selectedTextboxComparisons.Any())
                        {
                            // for the drop down/text box items there will be
                            // one selected item and a text box value
                            var comparisonValues = selectedTextboxComparisons.Select(item => item.DisplayValue).ToList();
                            comparisonValues.Add(doubleSelectionTextboxModel.SelectionValue);

                            itemList.Add(new NameValuesType
                            {
                                Name = doubleSelectionTextboxModel.DisplayValue,
                                Values = comparisonValues
                            });
                        }
                        break;
                }
            }

            var returnString = string.Empty;
            foreach (var item in itemList)
            {
                returnString += item.Name + ": ";
                returnString = item.Values.Aggregate(returnString, (current, value) => current + (value + ", "));
            }

            return returnString.TrimEnd(' ', ',');
        }
    }
}
