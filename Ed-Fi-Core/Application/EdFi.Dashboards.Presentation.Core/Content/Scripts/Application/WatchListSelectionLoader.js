// single selection items (checkboxes and radio buttons) are selected based
// upon the IsSelected property but the double drop down and drop down textbox
// use an observable list to contain the selections so the IsSelected property
// will need to be checked and these objects will need to be loaded
var WatchListSelectionLoader = function(watchListModel) {
    // The main function call that will load the selections into the model
    this.LoadSelectedList = function () {
        loadTabs(watchListModel.Tabs);
    };

    function loadTabs(tabs) {
        for (var tabCount = 0; tabCount < tabs.length; tabCount++) {
            loadColumns(tabs[tabCount].Columns());
        }
    }

    function loadColumns(columns) {
        for (var columnCount = 0; columnCount < columns.length; columnCount++) {
            loadTemplates(columns[columnCount].Templates());
        }
    }

    function loadTemplates(templates) {
        for (var templateCount = 0; templateCount < templates.length; templateCount++) {
            if (templates[templateCount].IsViewModelArray || templates[templateCount].ViewModel.TemplateName !== undefined) {
                // if the ViewModel property is an array then process the
                // ViewModel recursively
                loadTemplates(templates[templateCount].ViewModel);
            } else {
                var comparisonCount;
                var valueCount = 0;

                switch (templates[templateCount].TemplateName) {
                    case 'metricRadioButtonTemplate':
                        valueCount = templates[templateCount].ViewModel.Values().length;
                        for (var radioButtonCount = 0; radioButtonCount < valueCount; radioButtonCount++) {
                            if (templates[templateCount].ViewModel.Values()[radioButtonCount].IsSelected() === true) {
                                var selectedRadioValue = templates[templateCount].ViewModel.Values()[radioButtonCount].Name;

                                templates[templateCount].ViewModel.SelectedValue(selectedRadioValue);
                            }
                        }
                        break;
                    case 'metricDropDownTemplate':
                        comparisonCount = templates[templateCount].ViewModel.Comparisons().length;
                        for (var dropDownComparisonCount = 0; dropDownComparisonCount < comparisonCount; dropDownComparisonCount++) {
                            if (templates[templateCount].ViewModel.Comparisons()[dropDownComparisonCount].IsSelected() === true) {
                                var selectedDropDownComparisonValue = templates[templateCount].ViewModel.Comparisons()[dropDownComparisonCount].Name;

                                templates[templateCount].ViewModel.SelectedComparison(selectedDropDownComparisonValue);
                                break;
                            }
                        }
                        break;
                    case 'metricDropDownTextboxTemplate':
                        // first we need to check and see if one of the
                        // comparisons has IsSelected set to true and if it
                        // does set the SelectedComparison object to the value
                        // of that item
                        comparisonCount = templates[templateCount].ViewModel.Comparisons().length;
                        for (var dropTextComparisonCount = 0; dropTextComparisonCount < comparisonCount; dropTextComparisonCount++) {
                            if (templates[templateCount].ViewModel.Comparisons()[dropTextComparisonCount].IsSelected() === true) {
                                var selectedDropTextComparisonValue = templates[templateCount].ViewModel.Comparisons()[dropTextComparisonCount].Name;
                                var selectedDropTextValue = templates[templateCount].ViewModel.SelectionValue;
                                
                                templates[templateCount].ViewModel.SelectedComparison(selectedDropTextComparisonValue);
                                templates[templateCount].ViewModel.TextboxValue(selectedDropTextValue);
                                break;
                            }
                        }
                        break;
                }
            }
        }
    }
};