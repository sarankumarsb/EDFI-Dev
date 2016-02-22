// this will be the object that will contain all client-side functionality
// for the metrics based watch list
var MetricsBasedWatchList = function (gridName, watchListDialogs, watchListUrl, watchListSearchUrl, watchListSettings, watchListButtons, sharingCheckbox, pageSelectionValues) {
    var self = this,
        watchListDivIdentifier = "",
        watchListMenuItemsDivIdentifier = "",
        watchListRenameLink = "",
        watchListRenameLinkIdentifier = "",
        watchListDeleteLink = "",
        watchListDeleteLinkIdentifier = "",
        watchListCancelLink = "",
        watchListCancelLinkIdentifier = "",
        watchListSearchLink = "",
        watchListSearchLinkIdentifier = "",
        watchListSaveButtonIdentifier = "",
        watchListRenameButtonIdentifier = "",
        watchListDeleteButtonIdentifier = "",
        watchListSharingCheckboxIdentifier = "",
        pageDemographicValue = "",
        pageSchoolCategoryValue = "",
        pageGrade = "",
        saveConfirmationPopup = "",
        sharingConfirmationPopup = "",
        renamePopup = "",
        renamePopupTextbox = "",
        renamePopupDescriptionTextArea = "",
        deletePopup = "",
        deleteConfirmed = false,
        formIdentifier = "",
        linkHtml = "",
        linkHtmlNoDelete = "",
        selectionLoader = null;

    // make sure the identifier isn't set to string.Empty before setting all of
    // the member variables
    if (gridName !== "") {
        watchListDivIdentifier = "#" + gridName + "EdfiGrid-watch-list";
        watchListMenuItemsDivIdentifier = "#" + gridName + "-EdFiGrid-interaction-student-watchlist";
        watchListRenameLink = gridName + "-EdFiGrid-WatchList-RenameLink";
        watchListRenameLinkIdentifier = "#" + watchListRenameLink;
        watchListDeleteLink = gridName + "-EdFiGrid-WatchList-DeleteLink";
        watchListDeleteLinkIdentifier = "#" + watchListDeleteLink;
        watchListCancelLink = gridName + "-EdFiGrid-WatchList-CancelLink";
        watchListCancelLinkIdentifier = "#" + watchListCancelLink;
        watchListSearchLink = gridName + "-EdFiGrid-search-filter-list";
        watchListSearchLinkIdentifier = "#" + watchListSearchLink;
        watchListSaveButtonIdentifier = "#" + gridName + watchListButtons.SaveButton;
        watchListDeleteButtonIdentifier = "#" + gridName + watchListButtons.DeleteButton;
        watchListRenameButtonIdentifier = "#" + gridName + watchListButtons.RenameButton;
        watchListSharingCheckboxIdentifier = "#" + gridName + sharingCheckbox;
        pageDemographicValue = pageSelectionValues.Demographic;
        pageSchoolCategoryValue = pageSelectionValues.SchoolCategory;
        pageGrade = pageSelectionValues.Grade;
        saveConfirmationPopup = "#" + gridName + watchListDialogs.SavePopup.PopupName;
        sharingConfirmationPopup = "#" + gridName + watchListDialogs.SharingPopup.PopupName;
        renamePopup = "#" + gridName + watchListDialogs.RenamePopup.PopupName;
        renamePopupTextbox = "#" + gridName + watchListDialogs.RenamePopup.TextboxName;
        renamePopupDescriptionTextArea = "#" + gridName + watchListDialogs.RenamePopup.DescriptionNameTextArea;
        deletePopup = "#" + gridName + watchListDialogs.DeletePopup.PopupName;
        formIdentifier = "#" + gridName + "EdfiGrid-watch-list-form";

        linkHtml = "<div id='" + gridName + "-WatchList-ButtonDiv' class='CustomStudentList-ButtonDiv'>" +
                   "<a href='#' id='" + watchListRenameLink + "' class='CustomStudentList-button CustomStudentList-FinishLink' style='display:none;'>CHANGE NAME</a>" +
                   "<a href='#' id='" + watchListDeleteLink + "' class='CustomStudentList-button CustomStudentList-FinishLink' style='display:none;'>DELETE DYNAMIC LIST</a>" +
                   "<a href='#' id='" + watchListCancelLink + "' class='CustomStudentList-button CustomStudentList-CancelLink' style='display:none;'>CANCEL</a>" +
                   "</div>";

        linkHtmlNoDelete = "<div id='" + gridName + "-WatchList-ButtonDiv' class='CustomStudentList-ButtonDiv'>" +
                   "<a href='#' id='" + watchListRenameLink + "' class='CustomStudentList-button CustomStudentList-FinishLink' style='display:none;'>CHANGE NAME</a>" +
                   "<a href='#' id='" + watchListCancelLink + "' class='CustomStudentList-button CustomStudentList-CancelLink' style='display:none;'>CANCEL</a>" +
                   "</div>";
    }

    this.OriginalWatchList = null;
    this.WatchListModel = null;
    this.SelectionBuilder = null;
    this.DisplayBuilder = null;
    this.FormValidator = null;
    this.IsLoading = false;

    this.loadWatchList = function (jsonToLoad) {
        // set IsLoading so other objects won't fire certain events until
        // loading is done
        self.IsLoading = true;

        self.WatchListModel = new WatchListModel(jsonToLoad);
        self.WatchListModel.IsCancelling = false;

        /**********************************************************************
         * This code block is trying to solve the issue where the user makes
         * a change to the watch list data, doesn't save the changes, then
         * selects a user to view. The previous/next controller gets the
         * proper number of selected users, but when the "back to list" link
         * is clicked the data in the grid is correct but the watch list
         * doesn't reflect the most recent changes. Currently it does not work
         * but this is considered a very rare edge case since if they redo the
         * previous selections they made the grid will still show the proper
         * data.
         *********************************************************************/
        var originalWatchListData = jsonToLoad;
        if (self.WatchListModel.IsWatchListChanged) {
            var dataToSend = {
                Id: watchListSettings.WatchListId,
                StaffUSI: watchListSettings.StaffUSI,
                SchoolId: watchListSettings.SchoolId
            };

            $.getJSON(watchListUrl + '/Get', dataToSend, function (returnData) {
                originalWatchListData = returnData;
            });

            if (originalWatchListData === '') {
                originalWatchListData = jsonToLoad;
            }
        }

        // save the original watch list
        self.OriginalWatchList = new WatchListModel(originalWatchListData);

        // the bindings have to be applied to the empty watch list model or
        // things will get screwy; this is why the IsLoading field was created
        ko.setTemplateEngine(new ko.nativeTemplateEngine());
        ko.applyBindings(self.WatchListModel);

        // setup all of the watch list loading/validating objects
        selectionLoader = new WatchListSelectionLoader(self.WatchListModel);
        self.SelectionBuilder = new WatchListSelectionBuilder(self.WatchListModel);
        self.DisplayBuilder = new WatchListDisplayBuilder(self.WatchListModel);
        self.FormValidator = new FormValidator(formIdentifier);

        // load the current watch list data if any exists
        selectionLoader.LoadSelectedList();
        self.WatchListModel.SelectedValuesJson = self.SelectionBuilder.GetSelectedValuesJson();
        self.WatchListModel.OriginalSelection = self.WatchListModel.SelectedValuesJson;
        self.DisplayBuilder.BuildDisplayList();
        self.FormValidator.initializeValidator();

        // now that loading is finished we want to start tracking changes again
        self.IsLoading = false;
    };

    // this method will setup anything that is needed for the watch list
    this.initializeEvents = function () {
        // setup the close button click event
        $("#" + gridName + "EdfiGrid-filtered-list-header button").click(function () { checkWatchListChanged(); });

        // setup the tab toggle click event
        $("#" + gridName + "EdfiGrid-watch-list-nav > li > a").click(function (event) {
            // remove the active tab class from all tabs
            $(gridName + ", .active-filter-tab").removeClass("active-filter-tab");
            // find the tab that was clicked and apply the active tab class
            $(this).parent().addClass("active-filter-tab");
            // remove the active tab page class from all tabs
            $("[id^='" + gridName + "-metric-based-filters-']").removeClass("active-filter");

            // based upon which tab was clicked display the page that corresponds
            // to that tab
            var showFilter = $(this).attr("data-tab-content");
            $("#" + showFilter).addClass("active-filter");

            // stop the event from firing up the rest of the dom
            event.preventDefault();
        });

        // disables/enables the textboxes when a value is selected in the
        // drop-down
        $("select").change(function (data) {
            var dropDownValueName = "#" + data.target.name;
            var textboxValueName = dropDownValueName + "-value";

            if ($(dropDownValueName).val() != "") {
                $(textboxValueName).prop('disabled', false);
            } else {
                $(textboxValueName).prop('disabled', true);
            }
        });

        // setup the form validation
        $(formIdentifier + " input:text").each(function() {
            var textboxIdentifier = "#" + $(this).prop("id");
            var dataMinValue = $(textboxIdentifier).attr("data-min-value");
            var dataMaxValue = $(textboxIdentifier).attr("data-max-value");
            var dataMinValueNumber = Number(dataMinValue);
            var dataMaxValueNumber = Number(dataMaxValue);

            self.FormValidator.addValidationMethod(textboxIdentifier, {
                digits: true,
                min: dataMinValueNumber,
                max: dataMaxValueNumber,
                messages: {
                    min: 'The minimum value is ' + dataMinValue,
                    max: 'The maximum value is ' + dataMaxValue,
                    digits: 'The value must be a number'
                }
            });
        });

        // call the form validation when the user tabs out of the textbox
        $(formIdentifier + " input:text").keypress(function (event) {
            if (event.which == 13) {
                event.preventDefault();
            }

            self.FormValidator.validate();

            if (!self.FormValidator.IsFormValid) {
                $(this).focus();
            }
        });

        // wire up the save button click event
        if ($(watchListSaveButtonIdentifier) !== undefined) {
            $(watchListSaveButtonIdentifier).click(function() {
                self.saveWatchList();
            });
        }

        // wire up the delete button click event
        if ($(watchListDeleteButtonIdentifier) !== undefined) {
            $(watchListDeleteButtonIdentifier).click(function() {
                self.deleteWatchList();
            });
        }

        // wire up the rename button click event
        if ($(watchListRenameButtonIdentifier) !== undefined) {
            $(watchListRenameButtonIdentifier).click(function() {
                self.renameWatchList();
            });
        }

        // wire up the sharing checkbox click event
        if ($(watchListSharingCheckboxIdentifier) !== undefined) {
            $(watchListSharingCheckboxIdentifier).click(function() {
                if ($(watchListSharingCheckboxIdentifier).prop("checked") === true) {
                    $(sharingConfirmationPopup).dialog("open");
                }
            });
        }

        // setup the save confirmation dialog
        $(saveConfirmationPopup).dialog({
            autoOpen: false,
            height: 200,
            width: 300,
            modal: true,
            buttons: {
                Yes: function () {
                    self.saveWatchList();
                    self.resetWatchListChanged();
                    $(this).dialog("close");
                },
                No: function () {
                    self.restoreOriginalWatchList();
                    $(this).dialog("close");
                }
            }
        });

        // setup the sharing confirmation dialog
        $(sharingConfirmationPopup).dialog({
            autoOpen: false,
            height: 200,
            width: 300,
            modal: true,
            buttons: {
                OK: function() {
                    $(this).dialog("close");
                }
            }
        });

        // wire up the save dialog close event
        $(saveConfirmationPopup).on("dialogclose", function () {
            self.close();
        });

        // setup the rename dialog
        $(renamePopup).dialog({
            autoOpen: false,
            height: 300,
            width: 500,
            modal: true,
            buttons: {
                OK: function () {
                    self.WatchListModel.WatchListName($(renamePopupTextbox).val());
                    self.WatchListModel.WatchListDescription($(renamePopupDescriptionTextArea).val());
                    $(this).dialog("close");
                },
                Cancel: function () {
                    $(this).dialog("close");
                }
            }
        });

        // setup the delete confirmation dialog
        $(deletePopup).dialog({
            autoOpen: false,
            height: 200,
            width: 300,
            modal: true,
            buttons: {
                Yes: function() {
                    deleteConfirmed = true;
                    self.deleteWatchList();
                },
                No: function() {
                    $(this).dialog("close");
                }
            }
        });
    };

    // can be called to close the watch list dialog
    this.close = function () {
        //window.sessionStorage.setItem('originalWatchListData', undefined);
        $(watchListDivIdentifier).slideToggle();
        $(watchListDivIdentifier).trigger('closing', $(watchListDivIdentifier));
    };

    // called to delete a watch list
    this.deleteWatchList = function () {
        if (!deleteConfirmed) {
            $(deletePopup).dialog("open");
        } else {
            $(deletePopup).dialog("close");

            if (watchListSettings !== undefined) {
                var dataToSend = createDataToSend();
                dataToSend.Action = "delete";

                $.ajax({
                    url: watchListUrl + "/Post",
                    type: 'POST',
                    contentType: "application/json",
                    data: JSON.stringify(dataToSend)
                }).done(function (returnData) {
                    NavigateToPage(returnData);
                    deleteConfirmed = false;
                });
            }
        }
    };

    // called to open the watch list dialog
    this.open = function () {
        createWatchListMenuItems();
        $(watchListDivIdentifier).slideToggle();
        $(watchListDivIdentifier).trigger('opening', $(watchListDivIdentifier));
    };

    // called to change the name of the watch list
    this.renameWatchList = function () {
        $(renamePopup).dialog("open");
    };

    // used to reset the IsWatchListChanged field
    this.resetWatchListChanged = function () {
        self.WatchListModel.IsWatchListChanged = false;
    };

    // if the user makes changes, closes the form, and decides not to save the
    // changes we need to revert back to the previously saved version of the
    // watch list
    this.restoreOriginalWatchList = function() {
        self.WatchListModel.IsCancelling = true;
        self.WatchListModel.WatchListName(self.OriginalWatchList.WatchListName());
        restoreTabs(self.WatchListModel.Tabs, self.OriginalWatchList.Tabs);
        self.resetWatchListChanged();
        self.WatchListModel.IsCancelling = false;
    };

    // called to save the watch list
    this.saveWatchList = function () {
        $.ajax({
            url: watchListUrl + "/Post",
            type: 'POST',
            contentType: "application/json",
            data: JSON.stringify(createDataToSend())
        }).done(function (returnData) {
            self.resetWatchListChanged();
            NavigateToPage(returnData);
        });
    };

    // since we have multiple methods that need to send this data using ajax
    // this method is used to construct the data object to be sent
    function createDataToSend() {
        var dataToSend = {
            LocalEducationAgencyId: watchListSettings.LocalEducationAgencyId,
            SchoolId: watchListSettings.SchoolId,
            StaffUSI: watchListSettings.StaffUSI,
            PageStaffUSI: watchListSettings.PageStaffUSI,
            MetricBasedWatchListId: watchListSettings.MetricBasedWatchListId,
            WatchListName: self.WatchListModel.WatchListName(),
            WatchListDescription: self.WatchListModel.WatchListDescription(),
            IsWatchListShared: self.WatchListModel.IsWatchListShared(),
            SelectedValuesJson: self.WatchListModel.SelectedValuesJson,
            ResourceName: watchListSettings.SavingControllerName,
            Demographic: pageDemographicValue,
            SchoolCategory: pageSchoolCategoryValue,
            Grade: pageGrade,
            Action: watchListSettings.StudentListType === 'MetricsBasedWatchList' ? 'set' : 'add'
        };

        return dataToSend;
    }

    // this method is used to create the watch list menu items that should show
    // in the customize menu
    function createWatchListMenuItems() {
        if ($(watchListMenuItemsDivIdentifier).html() === "") {
            if (watchListSettings.StudentListType !== "MetricsBasedWatchList") {
                $(linkHtmlNoDelete).appendTo(watchListMenuItemsDivIdentifier);
            } else {
                $(linkHtml).appendTo(watchListMenuItemsDivIdentifier);
            }

            $(watchListRenameLinkIdentifier).on('click', function() { self.renameWatchList(); });
            if (watchListSettings.StudentListType === "MetricsBasedWatchList") {
                $(watchListDeleteLinkIdentifier).on('click', function() { self.deleteWatchList(); });
            }
            $(watchListSearchLinkIdentifier).on('click', function () { NavigateToPage(watchListSearchUrl); });
            $(watchListCancelLinkIdentifier).on('click', function () { checkWatchListChanged(); });
        }
    }

    // this is called when a watch list is being closed to determine if any of
    // the selections have changed and if so ask the user if they wish to save
    // the changes
    function checkWatchListChanged() {
        if (self.WatchListModel.IsWatchListChanged) {
            $(saveConfirmationPopup).dialog("open");
        } else {
            self.close();
        }
    }

    /**************************************************************************
     * The methods below are used to take the saved watch list data and load it
     * back into the watch list form; this has to be done since we aren't
     * saving the JSON data directly to the database but instead saving the
     * selections only.
     *************************************************************************/

    function restoreTabs(tabs, originalTabs) {
        for (var tabCount = 0; tabCount < tabs.length; tabCount++) {
            restoreColumns(tabs[tabCount].Columns(), originalTabs[tabCount].Columns());
        }
    }

    function restoreColumns(columns, originalColumns) {
        for (var columnCount = 0; columnCount < columns.length; columnCount++) {
            restoreTemplates(columns[columnCount].Templates(), originalColumns[columnCount].Templates());
        }
    }

    function restoreTemplates(templates, originalTemplates) {
        for (var templateCount = 0; templateCount < templates.length; templateCount++) {
            if (templates[templateCount].IsViewModelArray) {
                restoreTemplates(templates[templateCount].ViewModel, originalTemplates[templateCount].ViewModel);
            } else if (templates[templateCount].ViewModel.TemplateName != undefined) {
                restoreTemplates(templates[templateCount].ViewModel, originalTemplates[templateCount].ViewModel);
            } else {
                switch (templates[templateCount].TemplateName) {
                    case 'metricRadioButtonTemplate':
                        if (templates[templateCount].ViewModel.SelectedValue() !== undefined) {
                            templates[templateCount].ViewModel.SelectedValue(originalTemplates[templateCount].ViewModel.SelectedValue());
                        }
                        break;
                    case 'metricCheckboxTemplate':
                    case 'metricCheckboxInlineTemplate':
                        for (var valueCount = 0; valueCount < templates[templateCount].ViewModel.Values().length; valueCount++) {
                            templates[templateCount].ViewModel.Values()[valueCount].IsSelected(originalTemplates[templateCount].ViewModel.Values()[valueCount].IsSelected());
                        }
                        break;
                    case 'metricDropDownTemplate':
                        if (templates[templateCount].ViewModel.SelectedComparison() !== undefined) {
                            templates[templateCount].ViewModel.SelectedComparison(originalTemplates[templateCount].ViewModel.SelectedComparison());
                        }

                        if (templates[templateCount].ViewModel.Values().length <= 0 && templates[templateCount].ViewModel.SelectedValue() !== undefined) {
                            templates[templateCount].ViewModel.SelectedValue(originalTemplates[templateCount].ViewModel.SelectedValue());
                        }
                        break;
                    case 'metricDropDownTextboxTemplate':
                        if (templates[templateCount].ViewModel.SelectedComparison() !== undefined) {
                            templates[templateCount].ViewModel.SelectedComparison(originalTemplates[templateCount].ViewModel.SelectedComparison());
                        }
                        if (templates[templateCount].ViewModel.TextboxValue() !== undefined) {
                            templates[templateCount].ViewModel.TextboxValue(originalTemplates[templateCount].ViewModel.TextboxValue());
                        }
                        break;
                }
            }
        }
    }
}