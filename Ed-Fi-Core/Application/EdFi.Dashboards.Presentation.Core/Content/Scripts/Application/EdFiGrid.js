/// <reference path="../External/jquery.js" />
/// <reference path="../External/jquery.cookie.js" />
/// <reference path="../External/json2.js" />
/// <reference path="../External/BrowserDetect.js" />
/// <reference path="analytics.js" />


function InitEdFiGridSettings() {
    if (window.sessionStorage) {
        window.sessionStorage.setItem("EdFiGridMaximized", false);
    }
}

//Sorting the Json Object array
jQuery.fn.sort = function () { return this.pushStack([].sort.apply(this, arguments), []); };

var edfiGrid = function (identifier, uniqueTableName, sizeToWindow, fixedHeight) {
    "use strict";
    var _sessionDataIdentifier = "",
        _persistedDataIdentifier = "",
        _gridDataIdentifier = "",
        _identifier = "",
        _javascriptIdentifier = "",
        _javascriptFormIdentifier = "",
        _sizeToWindow = sizeToWindow,
        _fixedHeight = fixedHeight,
        _edfiGridOutermostDivIdentifier = "",
        _edfiGridDivIdentifier = "",
        _edfiGridFirstContainerDivIdentifier = "",
        _loadingDivIdentifier = "",
        _ie7DivIdentifier = "",
        _noDataDivIdentifier = "",
        _processingDivIdentifier = "",
        _processingBackgroundDivIdentifier = "",
        _gridContainerDivIdentifier = "",
        _interactionMenuDivIdentifier = "",
        _interactionSubmenuDivIdentifier = "",
        _interactionMenuPlaceholderDivIdentifier = "",
        _interactionCustomDivIdentifier = "",
        _changeColumnsLinkIdentifier = "",
        _resetColumnsLinkIdentifier = "",
        _saveColumnsLinkIdentifier = "",
        _cancelColumnsLinkIdentifier = "",
        _sizeGridDivIdentifier = "",
        _maximizeGridLinkIdentifier = "",
        _restoreGridLinkIdentifier = "",
        _gridTableContainerDivIdentifier = "",
        _fixedTopDivIdentifier = "",
        _fixedDivIdentifier = "",
        _fixedHeaderDivIdentifier = "",
        _fixedHeaderTableIdentifier = "",
        _scrollHeaderDivIdentifier = "",
        _scrollHeaderTableIdentifier = "",
        _scrollBottomDivIdentifier = "",
        _scrollableDivIdentifier = "",
        _fixedDataDivIdentifier = "",
        _fixedDataTableIdentifier = "",
        _scrollDataDivIdentifier = "",
        _scrollDataTableIdentifier = "",
        _scrollDivIdentifier = "",
        _scrollSizeDivIdentifier = "",
        _footerDivIdentifier = "",
        _footerTableIdentifier = "",
        _legendDivIdentifier = "",
        _tooltipContainerDivIdentifier = "",
        _footnotesDivIdentifier = "",
        _acendingSort = "asc",
        _decendingSort = "desc",
        _gridState = null,
        _gridSort = null,
        _gridSettings = null,
        _defaultVisibleColumns = null,
        _dataColumns = null,
        _demographicData = null,
        _schoolCategoryData = null,
        _gradeData = null,
        _previousNextSessionPage = null,
        _pageListType = null,
        _studentWatchListData = null,
        _studentWatchListRenameLinkIdentifier = "",
        _studentWatchListDeleteLinkIdentifier = "",
        _studentWatchListSearchLinkIdentifier = "",
        _studentWatchListCancelLinkIdentifier = "",
        _watchListCreateLinkIdentifier = "",
        _headerHasMultipleRows = false,
        _scrollBarSpace = 20,
        notApplicableString = "N/A",
        hiddenColumnClass = "hiddenCol",
        _isChrome = BrowserDetect.browser == "Chrome",
        _isSafari = BrowserDetect.browser == "Safari",
        _isIpad = BrowserDetect.OS == "iPad",
        _isIE = BrowserDetect.browser == "Explorer",
        _isIE7 = BrowserDetect.browser == "Explorer" && BrowserDetect.version < 8,
        _useServerSidePaging = 0,
        _paginationCallbackUrl = "",
        _exportGridDataUrl = "",
        _exportLocalEducationAgencyId = 0,
        _exportSchoolId = 0,
        _holdHasExtraCellSpace = false,
        _isGridMaximized = false,
        _holdAreaMainContentWidth = 0,
        _holdPushHeight = 0,
        _holdWindowMinHeight = "",
        _isIntializing = false,
        _entityIds = null;

    if (identifier !== "") {
        _identifier = identifier;
        _sessionDataIdentifier = uniqueTableName + identifier;
        _persistedDataIdentifier = uniqueTableName + identifier + "Persisted";

        _edfiGridDivIdentifier = "#" + _identifier + "-EdFiGrid";
        _edfiGridOutermostDivIdentifier = _edfiGridDivIdentifier + "-outermost";
        _edfiGridFirstContainerDivIdentifier = _edfiGridDivIdentifier + "-grid-first-container";
        _loadingDivIdentifier = _edfiGridDivIdentifier + "-loading";
        _ie7DivIdentifier = _edfiGridDivIdentifier + "-IE7";
        _noDataDivIdentifier = _edfiGridDivIdentifier + "-no-data";
        _processingDivIdentifier = _edfiGridDivIdentifier + "-processing";
        _processingBackgroundDivIdentifier = _edfiGridDivIdentifier + "-processing-background";
        _gridContainerDivIdentifier = _edfiGridDivIdentifier + "-container";
        _interactionMenuDivIdentifier = _edfiGridDivIdentifier + "-interaction-menu";
        _interactionSubmenuDivIdentifier = _edfiGridDivIdentifier + "-interaction-submenu";
        _interactionMenuPlaceholderDivIdentifier = _edfiGridDivIdentifier + "-interaction-menu-placeholder";
        _interactionCustomDivIdentifier = _edfiGridDivIdentifier + "-interaction-custom";
        _changeColumnsLinkIdentifier = _edfiGridDivIdentifier + "-change-columns";
        _resetColumnsLinkIdentifier = _edfiGridDivIdentifier + "-reset-columns";
        _saveColumnsLinkIdentifier = _edfiGridDivIdentifier + "-save-columns";
        _cancelColumnsLinkIdentifier = _edfiGridDivIdentifier + "-cancel-columns";
        _studentWatchListRenameLinkIdentifier = _edfiGridDivIdentifier + "-WatchList-RenameLink";
        _studentWatchListDeleteLinkIdentifier = _edfiGridDivIdentifier + "-WatchList-DeleteLink";
        _studentWatchListSearchLinkIdentifier = _edfiGridDivIdentifier + "-WatchList-SearchLink";
        _studentWatchListCancelLinkIdentifier = _edfiGridDivIdentifier + "-WatchList-CancelLink";
        _watchListCreateLinkIdentifier = "#" + _identifier + "-CustomStudentList-StartLink";
        _sizeGridDivIdentifier = _edfiGridDivIdentifier + "-interaction-size-grid";
        _maximizeGridLinkIdentifier = _edfiGridDivIdentifier + "-maximize-grid";
        _restoreGridLinkIdentifier = _edfiGridDivIdentifier + "-restore-grid";
        _gridTableContainerDivIdentifier = _edfiGridDivIdentifier + "-grid-table-container";
        _fixedTopDivIdentifier = _edfiGridDivIdentifier + "-fixed-top";
        _fixedDivIdentifier = _edfiGridDivIdentifier + "-fixed";
        _fixedHeaderDivIdentifier = _edfiGridDivIdentifier + "-fixed-header";
        _fixedHeaderTableIdentifier = _edfiGridDivIdentifier + "-fixed-header-table";
        _scrollHeaderDivIdentifier = _edfiGridDivIdentifier + "-scroll-header";
        _scrollHeaderTableIdentifier = _edfiGridDivIdentifier + "-scroll-header-table";
        _scrollBottomDivIdentifier = _edfiGridDivIdentifier + "-scroll-bottom";
        _scrollableDivIdentifier = _edfiGridDivIdentifier + "-scrollable";
        _fixedDataDivIdentifier = _edfiGridDivIdentifier + "-fixed-data";
        _fixedDataTableIdentifier = _edfiGridDivIdentifier + "-fixed-data-table";
        _scrollDataDivIdentifier = _edfiGridDivIdentifier + "-scroll-data";
        _scrollDataTableIdentifier = _edfiGridDivIdentifier + "-scroll-data-table";
        _scrollDivIdentifier = _edfiGridDivIdentifier + "-scroll";
        _scrollSizeDivIdentifier = _edfiGridDivIdentifier + "-scroll-size";
        _footerDivIdentifier = _edfiGridDivIdentifier + "-footer";
        _footerTableIdentifier = _edfiGridDivIdentifier + "-footer-table";
        _legendDivIdentifier = _edfiGridDivIdentifier + "-legend";
        _tooltipContainerDivIdentifier = _edfiGridDivIdentifier + "-tooltip-container";
        _footnotesDivIdentifier = _edfiGridDivIdentifier + "-footnotes";

        _gridDataIdentifier = _identifier + "-GridData";
        _javascriptIdentifier = _identifier + "EdFiGrid";
        _javascriptFormIdentifier = "#" + _identifier + "EdFiGridForm";
    }

    this.initialize = function (persistentOptions, nonPersistedOptions, pagingOptions, entityIdOptions, exportOptions) {
        _isIntializing = true;
        _entityIds = entityIdOptions.EntityIds;
        _studentWatchListData = entityIdOptions.StudentWatchListData;

        var holdThis = this;
        $(document).keyup(function (e) {
            if (e.keyCode == 27) {
                if (_isGridMaximized) {
                    holdThis.toggleMaximizeGrid();
                }
            }
        });

        if (pagingOptions) {
            _useServerSidePaging = pagingOptions.UseServerSidePaging;
            _paginationCallbackUrl = pagingOptions.PaginationCallbackUrl;
        }

        if (exportOptions) {
            _exportGridDataUrl = exportOptions.ExportGridDataUrl;
            _exportLocalEducationAgencyId = exportOptions.LocalEducationAgencyId;
            _exportSchoolId = exportOptions.SchoolId;
        }

        this.setupMouseWheelScroll();
        this.initializeGridState(persistentOptions, nonPersistedOptions);

        setTimeout(function () { holdThis.initializeTemplates(); }, 1);
    };

    this.gridState = function () {
        return _gridState;
    };
    /***************************************************************************************************************/
    /*                                                                                                             */
    /* Grid State Persistence                                                                                      */
    /*                                                                                                             */
    /***************************************************************************************************************/

    function saveGridStateToSessionPersistentCookie() {
        var json = JSON.stringify(_gridState);
        //Session persistent Cookie
        if (window.sessionStorage) {
            window.sessionStorage.setItem(_sessionDataIdentifier, json);
        } else {
            $.cookie(_sessionDataIdentifier, json);
        }
    }

    function saveGridStateToMachinePersistentCookie() {
        //Machine persisted Cookie
        var settingsToPersist = {
            version: _gridState.version,
            pageSize: _gridState.pageSize,
            visibleColumns: _gridState.visibleColumns
        };

        $.jStorage.set(_persistedDataIdentifier, settingsToPersist); //persisted for a year.
    }

    function saveGridStateToCookie() {
        saveGridStateToSessionPersistentCookie();
        saveGridStateToMachinePersistentCookie();
    }

    function getGridStateFromSessionPersistentCookie() {
        var json;
        if (window.sessionStorage) {
            json = window.sessionStorage.getItem(_sessionDataIdentifier);
        } else {
            json = $.cookie(_sessionDataIdentifier);
        }
        if (json !== null && json !== '') {
            return JSON.parse(json);
        }
        return null;
    }

    function getGridStateFromMachinePersistentCookie() {
        return $.jStorage.get(_persistedDataIdentifier);
    }
    
    function replaceMathematicsWithMath(columns) {
        for (var i = 0; i < columns.length; i++) {
            if (columns[i].DisplayName == "Mathematics") {
                columns[i].DisplayName = "Math";
            }
        }
    }

    this.resetGridStateCookie = function () {
        if (window.sessionStorage) {
            window.sessionStorage.removeItem(_sessionDataIdentifier);
        } else {
            $.cookie(_sessionDataIdentifier, null);
        }

        //$.jStorage.deleteKey(uniqueGridId + "Persisted");
    };

    this.processDefaultColumns = function () {
        var data = jQuery[_gridDataIdentifier],
            defaultVisibleColumns = [],
            colNum;
        replaceMathematicsWithMath(data.Columns);
        loadColumns(data);

        for (colNum = 0; colNum < _dataColumns.length; colNum++) {
            if (_dataColumns[colNum].IsVisibleByDefault) {
                defaultVisibleColumns.push(colNum);
            }
        }

        _defaultVisibleColumns = defaultVisibleColumns;
    };

    this.initializeGridState = function (persistentOptions, nonPersistedOptions) {
        var defaultSettings = {
                version: null,
                columnToSort: null,
                secondarySortColumnNumber: 1,
                sortDirection: _acendingSort,
                pageSize: 10,
                visibleColumns: [],
                usePersistedColumnData: true
            },
            defaultNonPersistedSettings = {
                customSort: null,
                templateOptions: null, //Used to pass in extra template options like {hrefToUse:"RelName"}
                pageToDisplay: 1
            },
            sessionData,
            machinePersistentData;

        if (persistentOptions) {
            // changing version will cause client browser to clear the localStorage of metrics and uses defaults.
            defaultSettings.version = persistentOptions.version;
        }

        //Adding the supplied settings
        if (persistentOptions) {
            $.extend(defaultSettings, persistentOptions);
        }

        //Adding the supplied settings
        if (nonPersistedOptions) {
            $.extend(defaultNonPersistedSettings, nonPersistedOptions);
        }

        //Initialize things:
        //Default or Supplied values
        _gridState = defaultSettings;
        _gridSort = defaultNonPersistedSettings.customSort;
        _gridSettings = defaultNonPersistedSettings;

        /**Session persisted cookie.**/
        //Cookies for state. (Needs the jquery.Cookie.js)
        //If there is a current cookie that has state for this grid then we get it else we set it.
        sessionData = getGridStateFromSessionPersistentCookie(_identifier);
        if (sessionData !== null && typeof sessionData !== 'undefined' && sessionData.version == _gridState.version) {
            $.extend(_gridState, sessionData);
        } else {
            saveGridStateToSessionPersistentCookie(_identifier);
        }

        /**Machine persisted cookie**/
        machinePersistentData = getGridStateFromMachinePersistentCookie(_identifier);
        if (machinePersistentData !== null && typeof machinePersistentData !== 'undefined' && machinePersistentData.version == _gridState.version) {
            $.extend(_gridState, machinePersistentData);
        } else {
            saveGridStateToMachinePersistentCookie(_identifier);
        }

        // we don't support more than 100 items displayed at a time
        if (defaultSettings.pageSize > 100)
            defaultSettings.pageSize = 100;
    };

    this.trackUsage = function (action, additionalData) {
        analyticsManager.trackEdFiGrid(action, _identifier, additionalData);
    };

    /***************************************************************************************************************/
    /*                                                                                                             */
    /* Grid State Persistence                                                                                      */
    /*                                                                                                             */
    /***************************************************************************************************************/

    /***************************************************************************************************************/
    /*                                                                                                             */
    /* Grid Sort                                                                                                   */
    /*                                                                                                             */
    /***************************************************************************************************************/

    function sortValue(a, b, columnNumber, secondarySortColumnNumber) {
        //Logic for the nulls
        if ((a[columnNumber].V === null) && (b[columnNumber].V === null)) {
            return ((columnNumber !== secondarySortColumnNumber) && (secondarySortColumnNumber !== null && secondarySortColumnNumber !== undefined)) ? sortValue(a, b, secondarySortColumnNumber) : 0;
        }
        if ((a[columnNumber].V === null)) {
            return 1;
        }
        if ((b[columnNumber].V === null)) {
            return -1;
        }

        if (a[columnNumber].V === b[columnNumber].V) {
            return ((columnNumber !== secondarySortColumnNumber) && (secondarySortColumnNumber !== null && secondarySortColumnNumber !== undefined)) ? sortValue(a, b, secondarySortColumnNumber) : 0;
        }

        var aType = typeof a[columnNumber].V;
        var bType = typeof b[columnNumber].V;
        if (aType !== bType) {
            return aType > bType ? 1 : -1;
        }

        return (a[columnNumber].V > b[columnNumber].V) ? 1 : -1;
    }

    this.sortValueDesc = function (a, b, grid) {
        return grid.sortValueAsc(a, b, grid) * -1;
    };

    this.sortValueAsc = function (a, b, grid) {
        return sortValue(a, b, grid.gridState().columnToSort, grid.gridState().secondarySortColumnNumber);
    };

    this.sortSchoolTypeDesc = function (a, b, grid) {
        return grid.sortSchoolTypeAsc(a, b, grid) * -1;
    };

    this.sortSchoolTypeAsc = function (a, b, grid) {
        //Logic for the nulls
        if ((a[grid.gridState().columnToSort].V === null) && (b[grid.gridState().columnToSort].V === null)) {
            return sortValue(a, b, grid.gridState().secondarySortColumnNumber);
        }
        if (a[grid.gridState().columnToSort].V === null) {
            return 1;
        }
        if (b[grid.gridState().columnToSort].V === null) {
            return -1;
        }
        if (a[grid.gridState().columnToSort].V === b[grid.gridState().columnToSort].V) {
            return sortValue(a, b, grid.gridState().secondarySortColumnNumber);
        }

        var aCategory = convertSchoolType(a[grid.gridState().columnToSort].V),
            bCategory = convertSchoolType(b[grid.gridState().columnToSort].V);
        return (aCategory > bCategory) ? 1 : -1;
    };

    function convertSchoolType(schoolCategory) {
        switch (schoolCategory) {
            case "High School":
                return 1;
            case "Junior High School":
                return 2;
            case "Middle School":
                return 3;
            case "Elementary School":
                return 4;
            case "Elementary/Secondary School":
                return 5;
            case "SecondarySchool":
                return 6;
            case "Ungraded":
                return 7;
            case "Adult School":
                return 8;
            case "Infant/toddler School":
                return 9;
            case "Preschool/early childhood":
                return 10;
            case "Primary School":
                return 11;
            case "Intermediate School":
                return 12;
        }
        return 7;
    }

    this.sortNAValueDesc = function (a, b, grid) {
        return grid.sortNAValueAsc(a, b, grid) * -1;
    };

    // this treats a N/A value like a null value
    this.sortNAValueAsc = function (a, b, grid) {
        if ((a[grid.gridState().columnToSort].V === notApplicableString) || (b[grid.gridState().columnToSort].V === notApplicableString)) {
            return sortNAValue(a, b, grid.gridState().columnToSort, grid.gridState().secondarySortColumnNumber);
        } else {
            return sortValue(a, b, grid.gridState().columnToSort, grid.gridState().secondarySortColumnNumber);
        }
    };

    function sortNAValue(a, b, columnNumber, secondarySortColumnNumber) {
        //Logic for the nulls
        if ((a[columnNumber].V === notApplicableString) && (b[columnNumber].V === notApplicableString)) {
            return (columnNumber !== secondarySortColumnNumber) ? sortValue(a, b, secondarySortColumnNumber) : 0;
        }
        if (a[columnNumber].V === notApplicableString) {
            return 1;
        }
        return -1;
    }

    this.sortDesignationsDesc = function (a, b, grid) {
        return grid.sortDesignationsAsc(a, b, grid) * -1;
    };

    this.sortDesignationsAsc = function (a, b, grid) {
        var aDesignations = a[grid.gridState().columnToSort].D,
            bDesignations = b[grid.gridState().columnToSort].D;

        //Logic for the nulls
        if ((aDesignations === null) && (bDesignations === null)) {
            return (grid.gridState().columnToSort !== grid.gridState().secondarySortColumnNumber) ? sortValue(a, b, grid.gridState().secondarySortColumnNumber) : 0;
        }
        if (aDesignations === null) {
            return 1;
        }
        if (bDesignations === null) {
            return -1;
        }

        // remove overage designation since we don't display an icon for it
        aDesignations = $.grep(aDesignations, function (value) { return value !== 4; });
        bDesignations = $.grep(bDesignations, function (value) { return value !== 4; });

        if (aDesignations.length > bDesignations.length) {
            return 1;
        }
        if (aDesignations.length < bDesignations.length) {
            return -1;
        }
        for (var i = 0; i < aDesignations.length; i++) {
            if (aDesignations[i] === bDesignations[i]) {
                continue;
            }
            return aDesignations[i] > bDesignations[i] ? 1 : -1;
        }
        return (grid.gridState().columnToSort !== grid.gridState().secondarySortColumnNumber) ? sortValue(a, b, grid.gridState().secondarySortColumnNumber) : 0;
    };

    this.sortTAKSValueDesc = function (a, b, grid) {
        return grid.sortTAKSValueAsc(a, b, grid) * -1;
    };

    this.sortTAKSValueAsc = function (a, b, grid) {
        var columnToSort = grid.gridState().columnToSort,
            secondaryColumnToSort = grid.gridState().secondarySortColumnNumber;

        if (a[columnToSort] === null) {
            return 1;
        }
        if (b[columnToSort] === null) {
            return -1;
        }

        if ((a[columnToSort].S === null) && (b[columnToSort].S === null)) {
            return sortValue(a, b, columnToSort, secondaryColumnToSort);
        }
        if (a[columnToSort].S === null) {
            return 1;
        }
        if (b[columnToSort].S === null) {
            return -1;
        }

        if (a[columnToSort].S === b[columnToSort].S) {
            return sortValue(a, b, columnToSort, secondaryColumnToSort);
        }

        var aSortValue = convertStatusToSort(a[columnToSort].S);
        var bSortValue = convertStatusToSort(b[columnToSort].S);
        return (aSortValue > bSortValue) ? 1 : -1;
    };

    this.sortMetricStateDesc = function (a, b, grid) {
        return grid.sortMetricStateAsc(a, b, grid) * -1;
    };

    this.sortMetricStateAsc = function (a, b, grid) {
        var columnToSort = grid.gridState().columnToSort,
            secondaryColumnToSort = grid.gridState().secondarySortColumnNumber;

        if (a[columnToSort] === null) {
            return 1;
        }
        if (b[columnToSort] === null) {
            return -1;
        }

        if ((a[columnToSort].ST === null) && (b[columnToSort].ST === null)) {
            return sortValue(a, b, columnToSort, secondaryColumnToSort);
        }
        if (a[columnToSort].ST === null) {
            return 1;
        }
        if (b[columnToSort].ST === null) {
            return -1;
        }

        if (a[columnToSort].ST === b[columnToSort].ST) {
            return ((columnToSort !== secondaryColumnToSort) && (secondaryColumnToSort !== null && secondaryColumnToSort !== undefined)) ? sortValue(a, b, secondaryColumnToSort) : 0;
        }

        var aSortValue = convertStatusToSort(a[columnToSort].ST);
        var bSortValue = convertStatusToSort(b[columnToSort].ST);
        return (aSortValue > bSortValue) ? 1 : -1;
    };

    function convertStatusToSort(status) {
        if (status === 6) { // Very Good
            return 20;
        }
        if (status === 1) { // good
            return 19;
        }
        if (status === 2) { //acceptable
            return 18;
        }
        if (status === 3) { //  Low
            return 17;
        }
        return status;
    }

    this.wrapSortFunction = function (sortFunction) {
        var holdThis = this;
        return function (a, b) {
            return sortFunction(a, b, holdThis);
        };
    };

    this.defineSortingColumn = function (columnName) {
        if (_gridState.columnToSort === null) {
            _gridState.columnToSort = columnName;
            _gridState.sortDirection = _acendingSort;
        } else {
            if (_gridState.columnToSort === columnName) {
                if (_gridState.sortDirection === _acendingSort) {
                    _gridState.columnToSort = columnName;
                    _gridState.sortDirection = _decendingSort;
                } else {
                    _gridState.sortDirection = _acendingSort;
                }
            } else {
                _gridState.columnToSort = columnName;
                _gridState.sortDirection = _acendingSort;
            }
        }

        this.trackUsage('sort', { columnToSort: columnName, sortDirection: _gridState.sortDirection === _acendingSort ? "asc" : "desc" });

        //Usability issue when i sort i must go to the first page.
        _gridSettings.pageToDisplay = 1;

        saveGridStateToCookie();
        this.redrawGrid();
    };

    /***************************************************************************************************************/
    /*                                                                                                             */
    /* Grid Sort                                                                                                   */
    /*                                                                                                             */
    /***************************************************************************************************************/

    /***************************************************************************************************************/
    /*                                                                                                             */
    /* Grid Draw                                                                                                   */
    /*                                                                                                             */
    /***************************************************************************************************************/

    this.initializeTemplates = function () {
        //Lets draw the grid...
        this.processDefaultColumns();
        this.drawGrid();

        _isIntializing = false;
    };

    // this will set a variable that is storing json from the watch list and
    // then call the redrawGrid method.
    this.setStudentWatchListData = function (watchListData) {
        _studentWatchListData = watchListData;
        this.redrawGrid();
    };

    // this should be called before the grid is drawn so no need to call the
    // redraw method
    this.setSelectedDemographic = function(demographicData) {
        _demographicData = demographicData;
    };

    this.setSelectedLevel = function(schoolCategoryData) {
        _schoolCategoryData = schoolCategoryData;
    }

    this.setSelectedGrade = function(gradeData) {
        _gradeData = gradeData;
    }

    this.setPreviousNextSessionPage = function(previousNextSessionPage) {
        _previousNextSessionPage = previousNextSessionPage;
    }

    this.setPageListType = function(pageListType) {
        _pageListType = pageListType;
    }

    this.sizeGridAndShow = function () {
        $(_processingDivIdentifier).width($(_edfiGridOutermostDivIdentifier).width());
        $(_processingDivIdentifier).height($(_edfiGridOutermostDivIdentifier).height());
        $(_processingBackgroundDivIdentifier).width($(_edfiGridOutermostDivIdentifier).width());
        $(_processingBackgroundDivIdentifier).height($(_edfiGridOutermostDivIdentifier).height());
        $(_processingBackgroundDivIdentifier).show();
        $(_processingDivIdentifier).show();
    };

    this.redrawGrid = function () {
        this.sizeGridAndShow();
        var grid = this;
        setTimeout(subRedrawGrid(grid), 100);
    };

    function subRedrawGrid(grid) {
        return function () {
            emptyAndRemoveTableParts(_fixedHeaderTableIdentifier);
            emptyAndRemoveTableParts(_scrollHeaderTableIdentifier);
            emptyAndRemoveTableParts(_fixedDataTableIdentifier);
            emptyAndRemoveTableParts(_scrollDataTableIdentifier);
            emptyAndRemoveTableParts(_footerTableIdentifier);
            grid.drawGrid();
        };
    }

    function emptyAndRemoveTableParts(tableIdentifier) {
        $(tableIdentifier).find('thead').empty();
        $(tableIdentifier).find('tbody').empty();
        $(tableIdentifier).find('tfoot').empty();
        /*IE 8 croaks when you user remove if its not empty...*/
        $(tableIdentifier).find('thead').remove();
        $(tableIdentifier).find('tbody').remove();
        $(tableIdentifier).find('tfoot').remove();
        $(tableIdentifier).css('width', '');
    }

    this.setGridPage = function (pageNumberToGoTo) {
        _gridSettings.pageToDisplay = pageNumberToGoTo;
        saveGridStateToCookie();
        this.trackUsage('page', { pageNumber: pageNumberToGoTo });
        
        $(_edfiGridDivIdentifier).one('afterDrawEvent', function () {
            $(_footerTableIdentifier).find('a').first().focus();
        });

        this.redrawGrid();
    };

    // this method will act similar to the drawGrid method below except it
    // will export the data
    this.exportGrid = function () {
        // first remove any input fields from the form
        $(_javascriptFormIdentifier + ' input').remove();

        // second make sure if the data in these fields are null then return an empty string
        var demographicData = _demographicData != null ? _demographicData : '';
        var schoolCategoryData = _schoolCategoryData != null ? _schoolCategoryData : '';
        var gradeData = _gradeData != null ? _gradeData : '';

        // third add the necessary fields to the form post
        $(_javascriptFormIdentifier)
            .append('<input type="hidden" name="LocalEducationAgencyId" value="' + _exportLocalEducationAgencyId + '"/>')
            .append('<input type="hidden" name="SchoolId" value="' + _exportSchoolId + '"/>')
            .append("<input type='hidden' name='StudentWatchListJson' value='" + JSON.stringify(_studentWatchListData) + "'/>")
            .append('<input type="hidden" name="SelectedDemographicOption" value="' + demographicData + '"/>')
            .append('<input type="hidden" name="SelectedSchoolCategoryOption" value="' + schoolCategoryData + '"/>')
            .append('<input type="hidden" name="SelectedGradeOption" value="' + gradeData + '"/>')
            .append('<input type="hidden" name="ListType" value="' + _pageListType + '"/>');

        // lastly submit the form
        $(_javascriptFormIdentifier).submit();
    };

    this.drawGrid = function () {
        if (_useServerSidePaging) {
            var holdThis = this;
            var dataToSend = new Object();
            dataToSend.pageNumber = _gridSettings.pageToDisplay;
            dataToSend.pageSize = _gridState.pageSize;
            dataToSend.sortColumn = _gridState.columnToSort;
            dataToSend.sortDirection = _gridState.sortDirection;
            dataToSend.visibleColumns = _gridState.visibleColumns;
            dataToSend.studentWatchListData = _studentWatchListData;
            dataToSend.studentIdList = _entityIds;
            dataToSend.selectedDemographicOption = _demographicData;
            dataToSend.selectedSchoolCategoryOption = _schoolCategoryData;
            dataToSend.selectedGradeOption = _gradeData;
            dataToSend.previousNextSessionPage = _previousNextSessionPage;
            dataToSend.listType = _pageListType;
            
            $.ajax({
                type: "POST",
                contentType: "application/json",
                url: _paginationCallbackUrl,
                data: JSON.stringify(dataToSend)
            }).done(function (pageData) {
                var dataTemp = jQuery[_gridDataIdentifier];
                dataTemp.Rows = pageData.Rows;
                dataTemp.TotalRows = pageData.TotalRows;
                jQuery[_gridDataIdentifier] = dataTemp;
                holdThis.drawGridData();
            });

            return;
        }

        this.drawGridData();
    };

    this.drawGridData = function () {
        //Data preparation sorting, paging, etc...
        var data = jQuery[_gridDataIdentifier];
        var dataCols = data.Columns;
        var dataRows = data.Rows;

        if (!dataRows || dataRows === null || dataRows.length === 0) {
            $(_edfiGridDivIdentifier).hide();
            $(_loadingDivIdentifier).hide();
            $(_processingDivIdentifier).hide();
            $(_processingBackgroundDivIdentifier).hide();
            $(_noDataDivIdentifier).show();
            return;
        }

        //Variable definition to control Grid/Data state
        if (_gridState === null) {
            _gridState = { columnToSort: null, sortDirection: _acendingSort, pageSize: 10 };
        }

        //Lets see if we have saved column visibility
        if (_gridState.visibleColumns.length === 0 || !_gridState.usePersistedColumnData) {
            loadVisibleColumns(data);
        } else {
            applySavedVisibleColumns(data);
        }

        //Selecting the rows to display based on the paging.
        var dataRowsToDisplay = [];
        if (_gridSettings.pageToDisplay === 0) {
            this.setGridPage(1);
            return;
        }

        var dataRowLength = (_useServerSidePaging) ? data.TotalRows : dataRows.length;
        var startingIndex = (_useServerSidePaging) ? 0 : (_gridSettings.pageToDisplay * _gridState.pageSize) - _gridState.pageSize;
        if (startingIndex > dataRowLength) {
            var pageCount = Math.ceil(dataRowLength / _gridState.pageSize);

            if (pageCount !== 0) {
                this.setGridPage(pageCount);
                return;
            }
        }
        var endingIndex = startingIndex + _gridState.pageSize;
        if (endingIndex > dataRowLength) {
            endingIndex = dataRowLength;
        }

        //Section for sorting the data.
        if (!_useServerSidePaging && _gridState.columnToSort !== null) {
            var sortFunction = this.sortValueAsc;
            if (_gridSort === null) {
                if (_gridState.sortDirection !== _acendingSort) {
                    sortFunction = this.sortValueDesc;
                }
            } else {
                var found = false;
                for (i = 0; i < _gridSort.length; i++) {
                    var columnSort = _gridSort[i];
                    if (columnSort.columnNumber === _gridState.columnToSort) {
                        found = true;
                        if (_gridState.sortDirection === _acendingSort) {
                            sortFunction = columnSort.sortAsc;
                        } else {
                            sortFunction = columnSort.sortDesc;
                        }
                        break;
                    }
                }
                if (!found && _gridState.sortDirection !== _acendingSort) {
                    sortFunction = this.sortValueDesc;
                }
            }
            dataRows.sort(this.wrapSortFunction(sortFunction));
        }

        for (var i = startingIndex; i < endingIndex; i++) {
            dataRowsToDisplay.push(dataRows[i]);
        }

        if (dataCols[0] === null) {
            return;
        }

        var dataTemplates = [];
        var dataClasses = [];
        var headerClasses = [];
        var overriddenStyles = [];
        var flatColumns = flattenColumns(dataCols);
        this.getColumnTemplatesAndClasses(dataRowsToDisplay[0], flatColumns, flatColumns.length != dataCols.length, dataTemplates, dataClasses, headerClasses, overriddenStyles);

        $(_loadingDivIdentifier).hide();
        $(_noDataDivIdentifier).hide();
        $(_edfiGridDivIdentifier).show();
        layoutDivs(0);

        //Draw the Header
        //alert('Building THead');
        this.drawGridHeader(headerClasses, dataClasses, overriddenStyles);
        //alert('End Building THead');

        //Draw the Body
        //alert('Building TBody');
        this.drawGridBody(dataRowsToDisplay, dataTemplates, dataClasses, overriddenStyles);
        //alert('Ended Building TBody');

        //Draw the footer with the pager, the rows per page and the total number of rows.
        //alert('Building TFooter');
        drawGridFooter(dataRowLength);
        //alert('Ended Building TFooter');

        setRowHeights(_fixedHeaderTableIdentifier, _scrollHeaderTableIdentifier);
        layoutDivs(calculateFixedTopDiv());
        this.sizeTables();

        $(_processingDivIdentifier).hide();
        $(_processingBackgroundDivIdentifier).hide();

        $(_edfiGridDivIdentifier).trigger('afterDrawEvent', $(_edfiGridDivIdentifier));
    };

    function flattenColumns(columns) {
        var flatColumns = [];
        var i, j;
        if (columns[0].Children === null || columns[0].Children.length === 0) {
            return columns;
        }

        for (i in columns) {
            for (j in columns[i].Children) {
                flatColumns.push(columns[i].Children[j]);
            }
        }
        return flatColumns;
    }

    function loadColumns(data) {
        if (_dataColumns !== null) {
            return;
        }

        _dataColumns = data.Columns;
        var c = [];

        //Sample data to know it has Grouping columns
        if (_dataColumns.length > 0 && _dataColumns[0].Children.length > 0) { //we have a double header
            _headerHasMultipleRows = true;
            for (var i = 0; i < _dataColumns.length; i++) {
                for (var j = 0; j < _dataColumns[i].Children.length; j++) {
                    c.push(_dataColumns[i].Children[j]);
                }
            }
            _dataColumns = c;
        }
    }

    function loadVisibleColumns(data) {
        var colNum;
        loadColumns(data);

        _gridState.visibleColumns = [];
        for (colNum = 0; colNum < _dataColumns.length; colNum++) {
            if (_dataColumns[colNum].U === null || _dataColumns[colNum].U === -1) {
                continue;
            }
            if (_dataColumns[colNum].IsVisibleByDefault) {
                _gridState.visibleColumns.push(_dataColumns[colNum].U);
            }
        }

        saveGridStateToCookie();
    }

    function applySavedVisibleColumns(data) {
        var colNum;
        if (_gridState.visibleColumns.length === 0) {
            return;
        }

        loadColumns(data);

        for (colNum = 0; colNum < _dataColumns.length; colNum++) {
            if (_dataColumns[colNum].U === null || _dataColumns[colNum].U === -1) {
                continue;
            }
            _dataColumns[colNum].IsVisibleByDefault = $.inArray(_dataColumns[colNum].U, _gridState.visibleColumns) !== -1;
        }
    }

    this.getColumnTemplatesAndClasses = function (dataRow, headerRow, isMultiRowHeader, dataTemplates, dataClasses, headerClasses, overriddenStyles) {
        var i, dataCell, headerCell;
        for (i in dataRow) {
            if (i == 0) {
                dataTemplates.push($.template("firstCellTemplate"));
                dataClasses.push("EdFiGrid-table-cell-first");
                headerClasses.push("mcHeadGrayRoundCornerBG EdFiGrid-table-cell-first");
                overriddenStyles.push("");
                continue;
            }

            if (!isMultiRowHeader && i == dataRow.length - 1) {
                dataTemplates.push($.template("firstCellTemplate"));
                dataClasses.push("EdFiGrid-table-cell-first");
                headerClasses.push("mcHeadGrayRoundCornerBG EdFiGrid-table-cell-first");
                overriddenStyles.push("");
                continue;
            }

            dataCell = dataRow[i];
            headerCell = headerRow[i];
            this.getColumnTemplateAndClass(dataCell, headerCell, dataTemplates, dataClasses, headerClasses, overriddenStyles);
            getColumnOverriddenStyles(headerCell, overriddenStyles);
        }
    };

    function getColumnOverriddenStyles(headerCell, overriddenStyles) {

        var newStyles = "";

        //allow css definitions to be overridden
        if (headerCell.OverriddenWidth != "") {
            newStyles = newStyles + "width:" + headerCell.OverriddenWidth + ";";
        }

        overriddenStyles.push(newStyles);
    }

    this.getColumnTemplateAndClass = function (dataCell, headerCell, dataTemplates, dataClasses, headerClasses, overriddenStyles) {
        var hidden, prop;
        if (!headerCell.IsVisibleByDefault) {
            hidden = "hiddenCol ";
        } else {
            hidden = "";
        }

        for (prop in dataCell) {
            switch (prop) {
                case "StudentUSI":
                    if (dataCell.Url == null && (dataCell.Links == null || dataCell.Links.length == 0)) {
                        dataTemplates.push($.template("studentNameNoLinkCellTemplate"));
                        dataClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-student");
                        headerClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-student");
                        return;
                    }
                    if (dataCell.I == null) {
                        dataTemplates.push($.template("studentNameAndCustomStudentListCellTemplate"));
                        dataClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-student");
                        headerClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-student");
                        return;
                    }
                    dataTemplates.push($.template("studentNameAndImageCustomStudentListCellTemplate"));
                    dataClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-student-image");
                    headerClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-student-image");
                    return;
                case "TId":
                    if (dataCell.I == null) {
                        dataTemplates.push($.template("teacherNameCellTemplate"));
                        dataClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-staff");
                        headerClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-staff");
                        return;
                    }
                    dataTemplates.push($.template("teacherNameAndImageCellTemplate"));
                    dataClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-staff-image");
                    headerClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-staff-image");
                    return;
                case "StId":
                    if (dataCell.I == null) {
                        dataTemplates.push($.template("staffNameCellTemplate"));
                        dataClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-staff");
                        headerClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-staff");
                        return;
                    }
                    dataTemplates.push($.template("staffNameAndImageCellTemplate"));
                    dataClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-staff-image");
                    headerClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-staff-image");
                    return;
                case "CId":
                    dataTemplates.push($.template("schoolCellTemplate"));
                    dataClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-school");
                    headerClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-school");
                    return;
                case "A":
                    dataTemplates.push($.template("indicatorMetricCellTemplate"));
                    dataClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-indicator");
                    headerClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-indicator");
                    return;
                case "T":
                    dataTemplates.push($.template("trendMetricCellTemplate"));
                    dataClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-trend");
                    headerClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-trend");
                    return;
                case "S":
                    dataTemplates.push($.template("metricCellTemplate"));
                    dataClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-metric");
                    headerClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-metric");
                    return;
                case "F":
                    dataTemplates.push($.template("flagCellTemplate"));
                    dataClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-flag");
                    headerClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-flag");
                    return;
                case "D":
                    dataTemplates.push($.template("designationsCellTemplate"));
                    dataClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-designation");
                    headerClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-designation");
                    return;
                case "SP":
                    dataTemplates.push($.template("spacerCellTemplate"));
                    dataClasses.push("EdFiGrid-table-cell-spacer");
                    headerClasses.push("mcHeadGrayDividerBG");
                    return;
                case "ST":
                    dataTemplates.push($.template("objectiveCellTemplate"));
                    dataClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-objective");
                    headerClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-objective");
                    return;
                case "M":
                    dataTemplates.push($.template("emailCellTemplate"));
                    dataClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-email");
                    headerClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-email");
                    return;
                case "Y":
                    dataTemplates.push($.template("yearsOfExperienceCellTemplate"));
                    dataClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-years-of-experience");
                    headerClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-years-of-experience");
                    return;
                case "E":
                    dataTemplates.push($.template("highestLevelOfEducationCellTemplate"));
                    dataClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-highest-level-of-education");
                    headerClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-highest-level-of-education");
                    return;
                case "O":
                    dataTemplates.push($.template("objectiveTextCellTemplate"));
                    dataClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-objective-text");
                    headerClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-objective-text");
                    return;
                case "STe":
                    dataTemplates.push($.template("stateTextCellTemplate"));
                    dataClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-state-text");
                    headerClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-state-text");
                    return;
                case "G":
                    dataTemplates.push($.template("goalEditCellTemplate"));
                    dataClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-goal-edit");
                    headerClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-goal-edit");
                    return;
            }
        }

        dataTemplates.push($.template("defaultCellTemplate"));
        dataClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-default");
        headerClasses.push(hidden + "EdFiGrid-table-cell EdFiGrid-table-cell-default");
    };

    this.drawGridHeader = function (headerClasses, dataClasses, overriddenStyles) {
        var i, data = jQuery[_gridDataIdentifier], fixedColumns = [], scrollColumns = [];
        for (i = 0; i < _dataColumns.length; i++) {
            if (_dataColumns[i].IsFixedColumn) {
                fixedColumns.push(i);
            } else {
                scrollColumns.push(i);
            }
        }

        var header = {
            Columns: data.Columns,
            FixedRows: data.FixedRows
        };

        var options = {
            identifier: _identifier,
            drawFixedColumn: true,
            columnsToDraw: fixedColumns,
            columnToSort: _gridState.columnToSort,
            sortDirection: _gridState.sortDirection,
            headerClasses: headerClasses,
            dataClasses: dataClasses,
            overriddenStyles: overriddenStyles
        };
        var headerTemplateName = 'theadTemplateForSingleRowTableHead';
        var tooltipTemplateName = 'tooltipTemplateForSingleHeader';
        if (_headerHasMultipleRows) {
            headerTemplateName = 'theadTemplateForTabbedTable';
            tooltipTemplateName = 'tooltipTemplateForTabbedTable';
        }

        var fh = $.tmpl(headerTemplateName, header, options);
        fh.appendTo(_fixedHeaderTableIdentifier);
        options.columnsToDraw = scrollColumns;
        options.drawFixedColumn = false;
        var sh = $.tmpl(headerTemplateName, header, options);
        sh.appendTo(_scrollHeaderTableIdentifier);
        $.tmpl(tooltipTemplateName, data, options).appendTo(_tooltipContainerDivIdentifier);
        this.wireHeaderEventHandlers(_fixedHeaderTableIdentifier);
        this.wireHeaderEventHandlers(_scrollHeaderTableIdentifier);

        //console.log("fixedHeaderTable cell count: " + $(_fixedHeaderTableIdentifier + ' > tbody > tr > td').length + " scrollHeaderTable cell count:" + $(_scrollHeaderTableIdentifier + ' > tbody > tr > td').length);
    };

    this.wireHeaderEventHandlers = function (headerId) {
        //Add the event handlers
        //Find the last TR 
        var holdThis = this;
        $(headerId + ' > tbody > tr:not(.EdFiGrid-table-fixed-data-row):last > td').each(function () {
            var td = $(this);

            if (td.html().indexOf('id="spacer"') === -1) {
                td.click(function () {
                    holdThis.defineSortingColumn(Number(td.attr('prop')));
                    $(_edfiGridDivIdentifier).one('afterDrawEvent', function () {
                        $(_edfiGridDivIdentifier).find('.sortArrow').parent('a').focus();
                    });
                });
            } else {
                // spacer cells should not have a sort link
                td.find('a').removeAttr('href');
            }
        });

        $(headerId + ' > tbody > tr:not(.EdFiGrid-table-fixed-data-row):last > td.headertooltip').each(function () {
            var td = $(this);

            td.tooltip({
                tip: '#' + td.attr('tooltipid'),
                position: 'bottom center',
                bounce: true,
                offset: [8, 50],
                relative: true
            });
        });
    };

    this.highlightRow = function () {
        $(_fixedDataTableIdentifier + '>tbody>tr').removeClass('hoverRow');
        $(_scrollDataTableIdentifier + '>tbody>tr').removeClass('hoverRow');
        var classes = $(this).attr("class").split(' ');
        for (var c in classes) {
            if (classes[c].indexOf("row") === 0) {
                $(_fixedDataTableIdentifier + '>tbody>tr.' + classes[c]).addClass('hoverRow');
                $(_scrollDataTableIdentifier + '>tbody>tr.' + classes[c]).addClass('hoverRow');
                break;
            }
        }
    };

    this.drawGridBody = function (dataRowsToDisplay, dataTemplates, dataClasses, overriddenStyles) {
        /*** Find fixed columns and scroll columns ***/
        var fixedColumns = [],
            scrollColumns = [],
            i,
            holdThis;
        for (i = 0; i < _dataColumns.length; i++) {
            if (_dataColumns[i].IsFixedColumn) {
                fixedColumns.push(i);
            } else {
                scrollColumns.push(i);
            }
        }

        /***Building the body ***/
        var options = {
            drawFixedColumn: true,
            columnsToDraw: fixedColumns,
            rows: dataRowsToDisplay,
            hrefToUse: null,
            dataTemplates: dataTemplates,
            dataClasses: dataClasses,
            overriddenStyles: overriddenStyles
        };

        if (_gridSettings.templateOptions) {
            $.extend(options, _gridSettings.templateOptions);
        }

        $.tmpl('gridRowTemplate', dataRowsToDisplay, options).appendTo(_fixedDataTableIdentifier);
        options.columnsToDraw = scrollColumns;
        options.drawFixedColumn = false;
        $.tmpl('gridRowTemplate', dataRowsToDisplay, options).appendTo(_scrollDataTableIdentifier);

        //Add highlight on mouse over.
        holdThis = this;
        $(_fixedDataTableIdentifier + '>tbody>tr').each(function () {
            $(this).hover(holdThis.highlightRow);
        });
        $(_scrollDataTableIdentifier + '>tbody>tr').each(function () {
            $(this).hover(holdThis.highlightRow);
        });

        //Hide columns
        for (i = 0; i < _dataColumns.length; i++) {
            var column = _dataColumns[i];
            //Setting visibility based on parent header column
            if (!column.IsVisibleByDefault) {
                $(_edfiGridDivIdentifier).find(".dataColumn" + i).addClass("hiddenCol");
            }
            //Setting column based css
            if (column.CssClassForColumn !== "") {
                $(_edfiGridDivIdentifier).find(".dataColumn" + i).addClass(column.CssClassForColumn);
            }
        }
    };

    function drawGridFooter(totalRowCount) {
        var footerData = {
            tsdsGridObject: _javascriptIdentifier,
            currentPage: _gridSettings.pageToDisplay,
            previousPage: 0,
            nextPage: 0,
            pageCount: 0,
            totalRowCount: totalRowCount
        };

        /***Building the footer***/
        footerData.pageCount = Math.ceil(totalRowCount / _gridState.pageSize);
        footerData.previousPage = _gridSettings.pageToDisplay - 1;
        if (footerData.previousPage < 1) {
            footerData.previousPage = 1;
        }

        footerData.nextPage = _gridSettings.pageToDisplay + 1;
        if (footerData.nextPage > footerData.pageCount) {
            footerData.nextPage = footerData.pageCount;
        }

        $.tmpl('footerTemplate', footerData).appendTo(_footerTableIdentifier);
    }

    function layoutDivs(fixedTopHeight) {
        if (fixedTopHeight === 0) {
            fixedTopHeight = 2 * 45;
        }
        var edfiGridHeight;
        if (_sizeToWindow) {
            edfiGridHeight = calculateSizeToWindowTableHeight() - 3; // this 3 keeps a scroll bar from being displayed on the main browser window
            //console.log("old: " + $(_edfiGridDivIdentifier).height() + " new: " + edfiGridHeight);
        } else {
            edfiGridHeight = calculateFixedSizeTableHeight(fixedTopHeight);
        }
        if (edfiGridHeight < 500) {
            edfiGridHeight = 500;
        }
        $(_edfiGridDivIdentifier).height(edfiGridHeight);

        var i = $(_footerDivIdentifier).height();
        if ($(_interactionMenuPlaceholderDivIdentifier).length > 0) {
            i += $(_interactionMenuPlaceholderDivIdentifier).height();
            //console.log($(_interactionMenuPlaceholderDivIdentifier).height());
        }
        if (_isChrome) {
            i += $(_legendDivIdentifier).height();
        }
        else {
            i += $(_legendDivIdentifier).height() + 20;    
        }
        i += $(_footnotesDivIdentifier).height();
        var gridTableContainerHeight = edfiGridHeight - i;
        var gridTableContainerWidth = $(_edfiGridDivIdentifier).width();

        $(_fixedTopDivIdentifier).height(fixedTopHeight);
        $(_fixedTopDivIdentifier).width(gridTableContainerWidth - _scrollBarSpace);
        $(_fixedDivIdentifier).height(fixedTopHeight);
        $(_fixedDivIdentifier).width(gridTableContainerWidth - _scrollBarSpace);
        $(_scrollBottomDivIdentifier).width(gridTableContainerWidth - _scrollBarSpace);
        $(_scrollBottomDivIdentifier).height(gridTableContainerHeight - fixedTopHeight - ((!_isChrome) ? 0 : _scrollBarSpace));

        //console.log("edfiGridDivIdentifier: " + gridTableContainerWidth + " _fixedTopDivIdentifier.width: " + (gridTableContainerWidth - _scrollBarSpace) + " _fixedDivIdentifier.width: " + (gridTableContainerWidth - _scrollBarSpace) + " _scrollBottomDivIdentifier.width: " + (gridTableContainerWidth - _scrollBarSpace));
    };

    this.resize = function () {
        layoutDivs(calculateFixedTopDiv());
        if (_isGridMaximized) {
            this.sizeTables();
        } else {
            setScrollSize();
        }
    };

    function getWindowInnerHeight() {
        if (typeof (window.innerHeight) == 'number') {
            //Non-IE
            return window.innerHeight;
        } else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight)) {
            //IE 6+ in 'standards compliant mode'
            return document.documentElement.clientHeight;
        } else if (document.body && (document.body.clientWidth || document.body.clientHeight)) {
            //IE 4 compatible
            return document.body.clientHeight;
        }
        return 0;
    }

    function calculateSiblingHeight(id) {
        var height = 0;
        //$(id).siblings(":visible").each(
        $(id).siblings(":not(script)").each(
            function () {
                var s = $(this);
                if (s.is(":visible")) {
                    height += s.height();
                }
            });
        return height;
    }

    function calculateSizeToWindowTableHeight() {
        var windowMinHeight = $("#content").css('min-height').replace('px', '');
        if (_isIpad || _isGridMaximized) {
            windowMinHeight = 0;
        }
        var windowHeight = max(getWindowInnerHeight(), windowMinHeight);
        var headerHeight = 0;
        if ($(_edfiGridDivIdentifier).is(":visible")) {
            headerHeight = $(_edfiGridDivIdentifier).position().top;
        }
        var footerHeight = 0;
        if ($("#footer").is(":visible")) {
            footerHeight = $("#footer").height();
        }

        var padding = 5;
        var bodyHeight = windowHeight - (headerHeight + footerHeight + padding);
        var siblingHeight = calculateSiblingHeight(_edfiGridOutermostDivIdentifier);
        var containerHeight = bodyHeight - siblingHeight;
        //console.log("isIpad: " + _isIpad + " windowMinHeight: " + windowMinHeight + " windowInnerHeight: " + getWindowInnerHeight() + " headerHeight: " + headerHeight + " footerHeight: " + footerHeight + " siblingHeight: " + siblingHeight);
        return containerHeight;
    }

    function calculateFixedSizeTableHeight(fixedTopHeight) {
        var data = jQuery[_gridDataIdentifier];
        var minFixedHeight = min(_fixedHeight, data.Rows.length);
        var height = minFixedHeight * 45 + fixedTopHeight;

        var siblingHeight = calculateSiblingHeight(_edfiGridFirstContainerDivIdentifier) + _scrollBarSpace;
        //console.log("sibilngHeight: " + siblingHeight);
        return height + siblingHeight;
    }

    function calculateFixedTopDiv() {
        var rows = $(_fixedHeaderTableIdentifier + " > tbody > tr:not(.EdFiGrid-table-fixed-data-row)");
        if (rows.length == 1) {
            $(_fixedHeaderDivIdentifier).css({ top: 0 });
            $(_scrollHeaderDivIdentifier).css({ top: 0 });
            return $(_fixedHeaderTableIdentifier).height();
        }

        var topRowHeight = $(rows[0]).height();
        $(_fixedHeaderDivIdentifier).css({ top: topRowHeight * -1 });
        $(_scrollHeaderDivIdentifier).css({ top: topRowHeight * -1 });
        return $(_fixedHeaderTableIdentifier).height() - topRowHeight;
    }

    this.sizeTables = function () {
        var scrollDivWidth = $(_fixedTopDivIdentifier).width() - $(_fixedHeaderTableIdentifier).width();
        if (!_useServerSidePaging) {
            var scrollTableWidth = $(_scrollHeaderTableIdentifier).width();
            if (scrollDivWidth > scrollTableWidth) {
                setColumnWidths(scrollDivWidth - scrollTableWidth);
                scrollDivWidth = $(_fixedTopDivIdentifier).width() - $(_fixedHeaderTableIdentifier).width();
            }
        }
        $(_scrollHeaderDivIdentifier).width(scrollDivWidth);
        $(_scrollDataDivIdentifier).width(scrollDivWidth);

        $(_fixedHeaderDivIdentifier).width($(_fixedHeaderTableIdentifier).width());
        $(_fixedDataDivIdentifier).width($(_fixedHeaderTableIdentifier).width());

        //console.log("pre set row height: " + $(_scrollDataTableIdentifier).height());
        setRowHeights(_fixedDataTableIdentifier, _scrollDataTableIdentifier);
        //console.log("post set row height: " + $(_scrollDataTableIdentifier).height());

        if ($(".EdFiGrid-table-cell-student-image, .EdFiGrid-table-cell-student").length > 0) {
            var columnGroups = ["Yellow", "Red", "Orange"];
            var i;
            for (i = 0; i < columnGroups.length; i++) {
                if ($(_scrollHeaderTableIdentifier + " tbody > tr > td.mcHead" + columnGroups[i] + "BG.scrollHeaderCell").not(".hiddenCol").length == 2) {
                    $(_scrollHeaderTableIdentifier + " tbody > tr > td.mcHead" + columnGroups[i] + "BG.scrollHeaderCell").not(".hiddenCol").addClass("EdFiGrid-table-cell-single");
                    $(_scrollDataTableIdentifier + " tbody > tr > td.scroll" + $(_scrollHeaderTableIdentifier + " tbody > tr > td.mcHead" + columnGroups[i] + "BG.scrollHeaderCell[prop]").not(".hiddenCol").first().attr("prop")).addClass("EdFiGrid-table-cell-single");
                } else {
                    $(_scrollHeaderTableIdentifier + " tbody > tr > td.mcHead" + columnGroups[i] + "BG.scrollHeaderCell").not(".hiddenCol").removeClass("EdFiGrid-table-cell-single");
                    $(_scrollDataTableIdentifier + " tbody > tr > td.scroll" + $(_scrollHeaderTableIdentifier + " tbody > tr > td.mcHead" + columnGroups[i] + "BG.scrollHeaderCell[prop]").not(".hiddenCol").first().attr("prop")).removeClass("EdFiGrid-table-cell-single");
                }
            }
        }

        setScrollSize();
        this.scrollData($(_scrollDivIdentifier));
    };

    function setScrollSize() {
        var dataHeight = $(_scrollDataTableIdentifier).height();
        // VIN07022016
        if (dataHeight == 0) {
            dataHeight = $(_fixedDataTableIdentifier).height();
        }
        if (!_isChrome || _sizeToWindow) {
            dataHeight += _scrollBarSpace;
        }
        var viewHeight = $(_scrollBottomDivIdentifier).height();

        var dataWidth = $(_scrollDataTableIdentifier).width();
        var viewWidth = $(_scrollDataDivIdentifier).width();

        var dataPositionTop = $(_fixedTopDivIdentifier).height();
        var dataPositionLeft = $(_fixedHeaderDivIdentifier).width();

    	//this check was put into place because the scroll bar on the Teacher List Grid in Chrome and only Chrome
    	//was off the viewable area by the amount the width of the scroll bar; however, in Chrome this on occurred
    	//ONLY on the Teacher List Grid.  I checked the differences between the other grids in Chrome and it appears
    	//when there is no data width or when the data width is small the width then the scroll bar gets pushed off the 
    	//screen.  The really awesome thing about this issue is the distance it pushes the scroll bar off is not 
        //consistant.  If I get more time I will research the issue further and come up with a less hacky solution.
        if (_isChrome && dataWidth == 2) {
	        if (_gridState.visibleColumns.length == 2)
		        dataPositionLeft -= _scrollBarSpace;
	        else if (_gridState.visibleColumns.length == 1)
		        dataPositionLeft -= (_scrollBarSpace / 2);
        }

        $(_scrollDivIdentifier).css({ left: dataPositionLeft, top: dataPositionTop });
        $(_scrollDivIdentifier).height(viewHeight + _scrollBarSpace);
        $(_scrollDivIdentifier).width(viewWidth + _scrollBarSpace);

        $(_scrollSizeDivIdentifier).height(dataHeight);

        if (viewHeight > dataHeight) {
            $(_scrollSizeDivIdentifier).width(dataWidth + _scrollBarSpace);
        } else {
            $(_scrollSizeDivIdentifier).width(dataWidth);
        }
    }

    function setColumnWidths(additionalSpace) {
        var i, width;

        var fixedHeaderCells = $(_fixedHeaderTableIdentifier + ' > tbody > tr:first-child > td');
        var scrollHeaderCells = $(_scrollHeaderTableIdentifier + ' > tbody > tr:first-child > td');

        var fixedDataCells = $(_fixedDataTableIdentifier + ' > tbody > tr:first-child > td');
        var scrollDataCells = $(_scrollDataTableIdentifier + ' > tbody > tr:first-child > td');

        var fixedCellCount = fixedHeaderCells.length;
        var scrollCellCount = scrollHeaderCells.length;

        var fixedHiddenColumnCount = $(_fixedDataTableIdentifier + ' > tbody > tr:first-child > td.hiddenCol').length;
        var scrollHiddenColumnCount = $(_scrollDataTableIdentifier + ' > tbody > tr:first-child > td.hiddenCol').length;
        var fixedSpacerColumnCount = $(_fixedDataTableIdentifier + ' > tbody > tr:first-child > td.EdFiGrid-table-cell-spacer').length;
        var scrollSpacerColumnCount = $(_scrollDataTableIdentifier + ' > tbody > tr:first-child > td.EdFiGrid-table-cell-spacer').length;
        var fixedRoundedColumnCount = $(_fixedDataTableIdentifier + ' > tbody > tr:first-child > td.EdFiGrid-table-cell-first').length;
        var scrollRoundedColumnCount = $(_scrollDataTableIdentifier + ' > tbody > tr:first-child > td.EdFiGrid-table-cell-first').length;
        var dataCellCount = (fixedCellCount + scrollCellCount) - (fixedHiddenColumnCount + scrollHiddenColumnCount + fixedSpacerColumnCount + scrollSpacerColumnCount + fixedRoundedColumnCount + scrollRoundedColumnCount);
        var additionalSpacePerCell = Math.floor(additionalSpace / dataCellCount);
        var extraCellSpace = additionalSpace - additionalSpacePerCell * dataCellCount;
        //console.log("setColumnWidths " + additionalSpace + " " + additionalSpacePerCell + " " + dataCellCount + " " + fixedCellCount + " " + scrollCellCount + " " + fixedHiddenColumnCount + " " + scrollHiddenColumnCount + " " + fixedSpacerColumnCount + " " + scrollSpacerColumnCount + " " + fixedRoundedColumnCount + " " + scrollRoundedColumnCount);
        //console.log("additionalSpace: " + additionalSpace + " additionalSpacePerCell: " + additionalSpacePerCell + " extraCellSpace: " + extraCellSpace);

        if (additionalSpacePerCell <= 0) {
            _holdHasExtraCellSpace = false;
            if (extraCellSpace > 0) {
                for (i = 0; i < scrollCellCount; i++) {
                    if ($(scrollDataCells[i]).hasClass('hiddenCol') === true || $(scrollDataCells[i]).hasClass('EdFiGrid-table-cell-spacer') === true || $(scrollDataCells[i]).hasClass('EdFiGrid-table-cell-first') === true) {
                        continue;
                    }
                    //console.log("add " + i);
                    if (_isChrome) {
                        width = $(scrollHeaderCells[i]).innerWidth() + extraCellSpace;
                    } else {
                        width = $(scrollHeaderCells[i]).width() + extraCellSpace;
                    }
                    $(scrollHeaderCells[i]).width(width);
                    $(scrollDataCells[i]).width(width);
                    return;
                }
            }
            return;
        }

        _holdHasExtraCellSpace = true;
        for (i = 0; i < fixedCellCount; i++) {
            if ($(fixedDataCells[i]).hasClass('hiddenCol') === true || $(fixedDataCells[i]).hasClass('EdFiGrid-table-cell-spacer') === true || $(fixedDataCells[i]).hasClass('EdFiGrid-table-cell-first') === true) {
                continue;
            }
            var j = $(fixedHeaderCells[i]).width();
            var k = $(fixedDataCells[i]).width();
            if (_isChrome || _isSafari) {
                width = $(fixedHeaderCells[i]).innerWidth() + additionalSpacePerCell;
            } else {
                width = $(fixedHeaderCells[i]).width() + additionalSpacePerCell;
            }
            $(fixedHeaderCells[i]).width(width);
            $(fixedDataCells[i]).width(width);
            //console.log(i + " " + j + " " + k + " " + width + " " + $(fixedHeaderCells[i]).width() + " " + $(fixedDataCells[i]).width());
        }
        for (i = 0; i < scrollCellCount; i++) {
            if ($(scrollDataCells[i]).hasClass('hiddenCol') === true || $(scrollDataCells[i]).hasClass('EdFiGrid-table-cell-spacer') === true || $(scrollDataCells[i]).hasClass('EdFiGrid-table-cell-first') === true) {
                continue;
            }
            var m = $(scrollHeaderCells[i]).width();
            var n = $(scrollDataCells[i]).width();
            if (_isChrome || _isSafari) {
                width = $(scrollHeaderCells[i]).innerWidth() + additionalSpacePerCell;
            } else {
                width = $(scrollHeaderCells[i]).width() + additionalSpacePerCell;
            }
            if (extraCellSpace > 0) {
                width += extraCellSpace;
                extraCellSpace = 0;
            }
            $(scrollHeaderCells[i]).width(width);
            $(scrollDataCells[i]).width(width);
            //console.log(i + " " + m + " " + n + " " + width + " " + $(scrollHeaderCells[i]).width() + " " + $(scrollDataCells[i]).width());
        }
    }

    function setRowHeights(id1, id2) {
        var i, maxHeight, row1Height, row2Height;
        var rows1 = $(id1 + ' > tbody > tr');
        var rows2 = $(id2 + ' > tbody > tr');
        for (i = 0; i < rows1.length; i++) {
            row1Height = $(rows1[i]).height();
            row2Height = $(rows2[i]).height();
            if (row1Height === row2Height) {
                continue;
            }
            maxHeight = max(row1Height, row2Height);
            $(rows1[i]).height(maxHeight);
            $(rows2[i]).height(maxHeight);
            $($(rows1[i]).find('td')).height(maxHeight);
            $($(rows2[i]).find('td')).height(maxHeight);
        }
        $(id1).height(0);
        $(id2).height(0);
    }

    function max(a, b) {
        if (a > b) {
            return a;
        }
        return b;
    }

    function min(a, b) {
        if (a < b) {
            return a;
        }
        return b;
    }

    this.scrollData = function (scroll) {
        var scrollBar = $(scroll);
        var scrollTopBottom = $(_scrollBottomDivIdentifier);
        var scrollData = $(_scrollDataDivIdentifier);
        var scrollHeader = $(_scrollHeaderDivIdentifier);
        var scrollLeft = scrollBar.scrollLeft();
        var scrollTop = scrollBar.scrollTop();
        scrollData.scrollLeft(scrollLeft);
        scrollHeader.scrollLeft(scrollLeft);
        scrollTopBottom.scrollTop(scrollTop);
    };

    this.setupMouseWheelScroll = function () {
        var grid = this;
        $(_fixedDataDivIdentifier).mousewheel(function (event, delta, deltaX, deltaY) {
            grid.mouseWheel(event, delta, deltaX, deltaY);
        });
        $(_scrollDataDivIdentifier).mousewheel(function (event, delta, deltaX, deltaY) {
            grid.mouseWheel(event, delta, deltaX, deltaY);
        });
        $(_fixedHeaderDivIdentifier).mousewheel(function (event, delta, deltaX, deltaY) {
            grid.mouseWheel(event, delta, deltaX, deltaY);
        });
        $(_scrollHeaderDivIdentifier).mousewheel(function (event, delta, deltaX, deltaY) {
            grid.mouseWheel(event, delta, deltaX, deltaY);
        });

        if (_isIpad) {
            $(_scrollDivIdentifier).css("z-index", 100);
        }
    };

    this.mouseWheel = function (event, delta, deltaX, deltaY) {
        var scrollBar = $(_scrollDivIdentifier);
        var scrollLeft = scrollBar.scrollLeft();
        var scrollTop = scrollBar.scrollTop();
        scrollBar.scrollLeft(max(scrollLeft - (deltaX * 45), 0));
        scrollBar.scrollTop(max(scrollTop - (deltaY * 45), 0));
        this.scrollData(scrollBar);

        event.stopPropagation();
        event.preventDefault();
    };
    /***************************************************************************************************************/
    /*                                                                                                             */
    /* Grid Draw                                                                                                   */
    /*                                                                                                             */
    /***************************************************************************************************************/

    /***************************************************************************************************************/
    /*                                                                                                             */
    /* Customize Grid Display (add/remove columns, change page size)                                               */
    /*                                                                                                             */
    /***************************************************************************************************************/
    this.setGridRowsPerPage = function (numberOfRows) {
        if (_gridState.pageSize !== numberOfRows) {
            // we don't support more than 100 items displayed at a time
            if (numberOfRows > 100)
                numberOfRows = 100;

            _gridState.pageSize = numberOfRows;

            this.trackUsage('pageSize', { pageSize: numberOfRows });
            //Each time we reset the number of rows per page then we go to page 1;
            _gridSettings.pageToDisplay = 1;
            saveGridStateToCookie();
            this.redrawGrid();
        }
    };

    this.setGridColumnVisibility = function (colIndex, value) {
        _dataColumns[colIndex].IsVisibleByDefault = value;
        if (value) {
            _gridState.visibleColumns.push(_dataColumns[colIndex].U);
        }
    };

    this.finishColumns = function () {
        $(_resetColumnsLinkIdentifier).hide();
        $(_saveColumnsLinkIdentifier).hide();
        $(_cancelColumnsLinkIdentifier).hide();
        $(_changeColumnsLinkIdentifier).show();
        $(_sizeGridDivIdentifier).show();
        $(_interactionCustomDivIdentifier).show();

        this.redrawGrid();
    };

    this.resetColumns = function () {
        var l, k;
        for (l = 0; l < _dataColumns.length; l++) {
            _dataColumns[l].IsVisibleByDefault = false;
        }

        for (k = 0; k < _defaultVisibleColumns.length; k++) {
            _dataColumns[_defaultVisibleColumns[k]].IsVisibleByDefault = true;
        }

        this.trackUsage('columnsReset', null);
        _gridState.visibleColumns = [];
        saveGridStateToCookie();
        this.finishColumns();
    };

    this.saveColumns = function () {
        _gridState.visibleColumns = [];
        this.toggleCheckboxes();
        this.trackUsage('columnsSave', { visibleColumns: _gridState.visibleColumns });
        saveGridStateToCookie();
        this.finishColumns();
    };

    this.toggleHeaders = function (link) {
        this.toggleCheckboxes();

        $(_interactionCustomDivIdentifier).hide();
        $(_sizeGridDivIdentifier).hide();
        $(link).hide();
        $(_resetColumnsLinkIdentifier).show();
        $(_saveColumnsLinkIdentifier).show();
        $(_cancelColumnsLinkIdentifier).show();
        this.trackUsage('columnsChange', { visibleColumns: _gridState.visibleColumns });
        this.showAllColumns();
        setRowHeights(_fixedHeaderTableIdentifier, _scrollHeaderTableIdentifier);
        layoutDivs(calculateFixedTopDiv());
        this.sizeTables();
    };

    this.showAllColumns = function () {
        if (_useServerSidePaging) {
            this.sizeGridAndShow();

            var holdThis = this;
            var colNum;
            var showAllColumnIds = [];

            for (colNum = 0; colNum < _dataColumns.length; colNum++) {
                if (_dataColumns[colNum].U === null || _dataColumns[colNum].U === -1) {
                    continue;
                }
                showAllColumnIds.push(_dataColumns[colNum].U);
            }

            var dataToSend = new Object();
            dataToSend.pageNumber = _gridSettings.pageToDisplay;
            dataToSend.pageSize = _gridState.pageSize;
            dataToSend.sortColumn = _gridState.columnToSort;
            dataToSend.sortDirection = _gridState.sortDirection;
            dataToSend.visibleColumns = showAllColumnIds;
            dataToSend.studentWatchListData = _studentWatchListData;
            dataToSend.studentIdList = _entityIds;
            dataToSend.selectedDemographicOption = _demographicData;
            dataToSend.selectedSchoolCategoryOption = _schoolCategoryData;
            dataToSend.selectedGradeOption = _gradeData;
            dataToSend.previousNextSessionPage = _previousNextSessionPage;
            dataToSend.listType = _pageListType;

            $.ajax(
                {
                    type: "POST",
                    contentType: "application/json",
                    url: _paginationCallbackUrl,
                    data: JSON.stringify(dataToSend)
                }).done(function (pageData) {
                    var dataTemp = jQuery[_gridDataIdentifier];
                    dataTemp.Rows = pageData.Rows;
                    dataTemp.TotalRows = pageData.TotalRows;
                    jQuery[_gridDataIdentifier] = dataTemp;
                    holdThis.loadGridData(dataTemp);
                    holdThis.showMoreData(dataTemp.Columns);
                });

            return;
        }
        var dataTemp = jQuery[_gridDataIdentifier];
        this.showMoreData(dataTemp.Columns);
    };

    this.loadGridData = function (data) {
        //Data preparation sorting, paging, etc...
        var dataCols = data.Columns;
        var dataRows = data.Rows;

        //Selecting the rows to display based on the paging.
        var dataRowsToDisplay = [];

        for (var i = 0; i < _gridState.pageSize; i++) {
            dataRowsToDisplay.push(dataRows[i]);
        }

        var dataTemplates = [];
        var dataClasses = [];
        var headerClasses = [];
        var overriddenStyles = [];
        var flatColumns = flattenColumns(dataCols);
        this.getColumnTemplatesAndClasses(dataRowsToDisplay[0], flatColumns, flatColumns.length != dataCols.length, dataTemplates, dataClasses, headerClasses, overriddenStyles);

        //Draw the Body
        $(_fixedDataTableIdentifier + '>tbody>tr,' + _scrollDataTableIdentifier + '>tbody>tr').remove();
        this.drawGridBody(dataRowsToDisplay, dataTemplates, dataClasses, overriddenStyles);

        $(_edfiGridDivIdentifier).find('.' + hiddenColumnClass).each(function () {
            $(this).removeClass(hiddenColumnClass);
        });

        setRowHeights(_fixedHeaderTableIdentifier, _scrollHeaderTableIdentifier);
        layoutDivs(calculateFixedTopDiv());
        this.sizeTables();

        $(_processingBackgroundDivIdentifier).hide();
        $(_processingDivIdentifier).hide();
    };

    this.showMoreData = function (dataCols) {
        //Shows all columns that are hidden.
        $(_edfiGridDivIdentifier).find('.' + hiddenColumnClass).each(function () {
            $(this).removeClass(hiddenColumnClass);
        });

        if (dataCols.length > 0 && dataCols[0].Children.length > 0) { //we have a double header
            //If its a double layer header then it expands the colspans as needed
            var i = 0;
            $(_fixedHeaderTableIdentifier + ' > tbody > tr.EdFiGrid-header-topper > td').each(function (index, item) {
                $(item).attr('colspan', dataCols[index].Children.length);
                i++;
            });
            $(_scrollHeaderTableIdentifier + ' > tbody > tr.EdFiGrid-header-topper > td').each(function (index, item) {
                $(item).attr('colspan', dataCols[index + i].Children.length);
            });
        }
    };

    this.toggleCheckboxes = function () {
        var i = 0;
        i = this.toggleHeaderCheckboxes(_fixedHeaderTableIdentifier, i);
        this.toggleHeaderCheckboxes(_scrollHeaderTableIdentifier, i);
    };

    this.toggleHeaderCheckboxes = function (headerIdentifier, i) {
        var holdThis = this;
        $(headerIdentifier + ' > tbody > tr:not(.EdFiGrid-table-fixed-data-row):last > td').each(function () {
            if ($(this).html().indexOf('EdFiGrid-column-visibility-checkbox') < 0) {
                //If it isn't a column that is tagged as a spacer.
                if ($(this).find("div[id=spacer]").length <= 0) {
                    var isChecked = "";
                    if (_dataColumns[i].IsVisibleByDefault) {
                        isChecked = "checked";
                    }

                    $(this).find("img.sortArrow").hide();
                    $(this).html("<label class='EdFiGrid-column-visibility-checkbox' for=cb" + i + ">" + $(this).html() + "</label><br/><input class='EdFiGrid-column-visibility-checkbox' type=checkbox " + isChecked + " id=cb" + i + ">");
                    $(this).css('vertical-align', 'bottom');

                    //Silent/Remove the click for sorting in this mode.
                    $(this).unbind('click');
                }
            } else {
                //For setting the show hide columns
                holdThis.setGridColumnVisibility(i, $(this).find("input.EdFiGrid-column-visibility-checkbox[type=checkbox]").is(':checked'));
                //Do nothing else because it will anyway be redrawn... (to take into account the selected columns.)
            }

            i++;
        });
        return i;
    };

    this.toggleMaximizeGrid = function () {
        this.maximizeGrid();
        this.redrawGrid();
    };

    this.maximizeGrid = function () {
        if (!_isGridMaximized) {
            $(".l-area-layout").toggleClass("l-area-layout-maximized");
            $("#footer").toggleClass("constrained");
            $("#footer").toggleClass("footer-maximized");
            $(".l-legend").hide();
            $(".EdFiGrid-maximize-grid").css("display", "none");
            $(".EdFiGrid-restore-grid").css("display", "");
//            $("#content").css('min-height', '300px');
//            resizeParentContainers();
//            $("#content").css('margin-bottom', $("#footer").height() * -1);
//            $("#Push").height($("#footer").height());
//            $("#header").hide();
//            $(".l-legend").hide();
//            $(_maximizeGridLinkIdentifier).hide();
//            $(_restoreGridLinkIdentifier).show();

            _isGridMaximized = true;
            $(_edfiGridDivIdentifier).trigger('afterMaximizeEvent', $(_edfiGridDivIdentifier));
            this.trackUsage('maximize', null);

        } else {
            $(".l-area-layout").toggleClass("l-area-layout-maximized");
            $("#footer").toggleClass("constrained");
            $("#footer").toggleClass("footer-maximized");
            $(".l-legend").show();
            $(".EdFiGrid-restore-grid").css("display", "none");
            $(".EdFiGrid-maximize-grid").css("display", "");

            if (_isIE || _isSafari) {
                $(_edfiGridOutermostDivIdentifier).css('width', 'inherit');
            }
//            $('#footer').width(windowMinWidth);
//            $("#content").css('margin-bottom', _holdPushHeight * -1);
//            $("#Push").height(_holdPushHeight);
//            $("#header").show();
//            $(".l-legend").show();
//            $(_maximizeGridLinkIdentifier).show();
//            $(_restoreGridLinkIdentifier).hide();
//            $("#StaffSlidingMenuButton").hide();
            
            _isGridMaximized = false;
            $(_edfiGridDivIdentifier).trigger('afterRestoreDownEvent', $(_edfiGridDivIdentifier));
            this.trackUsage('restoreDown', null);
        }

        if (window.sessionStorage) {
            window.sessionStorage.setItem("EdFiGridMaximized", _isGridMaximized);
        }
    };

    this.showStudentWatchListMenu = function () {
        $(_sizeGridDivIdentifier).hide();
        $(_changeColumnsLinkIdentifier).hide();
        $(_watchListCreateLinkIdentifier).hide();
        $(_studentWatchListRenameLinkIdentifier).show();
        $(_studentWatchListDeleteLinkIdentifier).show();
        $(_studentWatchListSearchLinkIdentifier).show();
        $(_studentWatchListCancelLinkIdentifier).show();
    };

    this.hideStudentWatchListMenu = function () {
        $(_sizeGridDivIdentifier).show();
        $(_changeColumnsLinkIdentifier).show();
        $(_watchListCreateLinkIdentifier).show();
        $(_studentWatchListRenameLinkIdentifier).hide();
        $(_studentWatchListDeleteLinkIdentifier).hide();
        $(_studentWatchListSearchLinkIdentifier).hide();
        $(_studentWatchListCancelLinkIdentifier).hide();
    };

    /// Added By Vinoth.N
    this.Submit2diffURL = function () {
        var data = jQuery[_gridDataIdentifier];
        var dataRows = data.Rows;
        var studentUSIlst = "";
        if (dataRows.length <= 0) {
            alert("No Student Available in List");
            return;
        }
        for (var iLoop = 0; iLoop < dataRows.length; iLoop++) {
            if (iLoop != 0)
                studentUSIlst = studentUSIlst + ", ";
            studentUSIlst = studentUSIlst + dataRows[iLoop][1].StudentUniqueID;
        }
        try {
            var form = document.createElement("FORM");
            form.method = "POST";
            form.enctype = "multipart/form-data";
            form.target = "_parent";
            form.style.display = "none";
            document.body.appendChild(form);
            //form.action = "http://demo.qforedu.com/local/programmgmt/index.php"; //"http://demo.qforedu.com/mapinterventions.php";
            form.action = $("#InterventionURL").val(); // Fix : EDFIDB-124
            var input = document.createElement("INPUT");
            input.type = "hidden";
            input.name = "hdnStudentUSI";
            input.value = studentUSIlst;
            form.appendChild(input);
            form.submit();
        } catch (e) { alert(e); alert(e.description); }
    }

    var menuDisplayed = false;
    var allowMenuAction = true;
    this.showInteractionMenu = function () {
        if (!menuDisplayed && allowMenuAction) {
            menuDisplayed = true;
            allowMenuAction = false;
            $(_interactionMenuDivIdentifier).show("slide", { direction: "up" }, 250, toggleMenuAction);
        }
    };

    this.hideInteractionMenu = function (element) {
        if ((element !== null) && ('#' + element.target.id !== _interactionMenuDivIdentifier) && ('#' + element.target.id !== _interactionSubmenuDivIdentifier)) {
            return;
        }

        if (menuDisplayed && allowMenuAction) {
            allowMenuAction = false;
            $(_interactionMenuDivIdentifier).hide("slide", { direction: "up" }, 250, toggleMenuAction);
            menuDisplayed = false;
        }
    };

    function toggleMenuAction() {
        allowMenuAction = true;
    }

    /***************************************************************************************************************/
    /*                                                                                                             */
    /* Customize Grid Display (add/remove columns, change page size)                                               */
    /*                                                                                                             */
    /***************************************************************************************************************/
};

/******************************************************************************
 * These objects will be used to house the export data that will be sent to
 * the server side.
 *****************************************************************************/
var ExportColumn = function(index, displayValue) {
    var self = this;

    self.ColumnIndex = index;
    self.ColumnDisplayValue = displayValue;
};

var ExportCell = function(index, displayValue) {
    var self = this;

    self.ColumnIndex = index;
    self.CellDisplayValue = displayValue;
};

var ExportRow = function() {
    var self = this;

    self.Cells = new Array();

    self.AddCell = function(cell) {
        self.Cells.push(cell);
    };
};