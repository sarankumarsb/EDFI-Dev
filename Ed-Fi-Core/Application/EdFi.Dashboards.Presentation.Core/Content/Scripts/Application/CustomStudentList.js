/// <reference path="../External/jquery.js" />
/// <reference path="../External/json2.js" />
/// <reference path="analytics.js" />

var customStudentList = function (identifier, isCustomStudentList, linkParentIdentifier, checkboxParentIdentifier, selectAllCheckboxParentIdentifier, customStudentListId, localEducationAgencyId, schoolId, staffUSI, customStudentListUrl, sessionStudentListUrl, sessionUniqueId) {
    "use strict";
    var _identifier = identifier,
        _localEducationAgencyId = localEducationAgencyId,
        _schoolId = schoolId,
        _staffUSI = staffUSI,
        _customStudentListUrl = customStudentListUrl,
        _sessionStudentListUrl = sessionStudentListUrl,
        _sessionUniqueId = sessionUniqueId,
        _buttonDivIdentifier = "#" + identifier + "-CustomStudentList-ButtonDiv",
        _startLinkIdentifier = "#" + identifier + "-CustomStudentList-StartLink",
        _finishLinkIdentifier = "#" + identifier + "-CustomStudentList-FinishLink",
        _renameLinkIdentifier = "#" + identifier + "-CustomStudentList-RenameLink",
        _deleteLinkIdentifier = "#" + identifier + "-CustomStudentList-DeleteLink",
        _cancelLinkIdentifier = "#" + identifier + "-CustomStudentList-CancelLink",
        _cloneLinkIdentifier = "#" + identifier + "-CustomStudentList-CloneLink",
        _isCustomStudentList = isCustomStudentList,
        _linkParentIdentifier = linkParentIdentifier,
        _checkboxParentIdentifier = checkboxParentIdentifier,
        _selectAllCheckboxParentIdentifier = selectAllCheckboxParentIdentifier,
        _customStudentListStarted = false,
        _processingRequest = false,
        _ids = [],
        _selectedData = [],
        _gridDataIdentifier = _identifier + "-GridData",
        _addSelectedText = "ADD SELECTED STUDENTS TO WATCH LIST",
        _createCustomStudentListText = "CREATE OR ADD TO WATCH LIST",
        _editCustomStudentListText = "REMOVE SELECTED STUDENTS FROM WATCH LIST",
        _cloneCustomStudentListText = "ADD ALL STUDENTS TO WATCH LIST",
        _customStudentListId = customStudentListId,
        _removeSelectedText = "EDIT WATCH LIST",
        _nameAndImageCellItemTemplateId = "#" + identifier + "-CustomStudentList-NameAndImageCellItemTemplate",
        _createDialogId = "#" + identifier + "-CustomStudentList-CreateDialog",
        _createDialogStudentListTableId = "#" + identifier + "-CustomStudentList-Create-StudentListTable",
        _createdDialogListSelectionId = "#" + identifier + "-CustomStudentList-Create-ListSelectionListbox",
        _createDialogUseNewListRadioId = "#" + identifier + "-CustomStudentList-Create-UseNewListRadio",
        _createDialogNewListNameId = "#" + identifier + "-CustomStudentList-Create-NewListName",
        _createDialogUseOldListRadio = "#" + identifier + "-CustomStudentList-Create-UseOldListRadio",
        _createDialogUseOldListRadioLabel = "#" + identifier + "-CustomStudentList-Create-UseOldListRadioLabel",
        _createDialogWaitDivId = "#" + identifier + "-CustomStudentList-Create-Wait",
        _removeDialogWaitDivId = "#" + identifier + "-CustomStudentList-RemoveDialog-Wait",
        _renameDialogWaitDivId = "#" + identifier + "-CustomStudentList-Rename-Wait",
        _deleteDialogWaitDivId = "#" + identifier + "-CustomStudentList-DeleteDialog-Wait",
        _removeDialogId = "#" + identifier + "-CustomStudentList-RemoveDialog",
        _removeDialogStudentListTableId = "#" + identifier + "-CustomStudentList-Remove-StudentListTable",
        _deleteDialogId = "#" + identifier + "-CustomStudentList-DeleteDialog",
        _renameDialogId = "#" + identifier + "-CustomStudentList-RenameDialog",
        _renameDialogNewListNameId = "#" + identifier + "-CustomStudentList-Rename-Input",
        _selectedStudentUSIs;

    var startText = _isCustomStudentList ? _removeSelectedText : _createCustomStudentListText;
    var actionText = _isCustomStudentList ? _editCustomStudentListText : _addSelectedText;
    var actionClass = _isCustomStudentList ? "Remove" : "Finish";

    var linkHtml = "<div id='" + _identifier + "-CustomStudentList-ButtonDiv' class='CustomStudentList-ButtonDiv'>" +
                   "<a href='#' id='" + _identifier + "-CustomStudentList-StartLink'  class='CustomStudentList-button CustomStudentList-StartLink'>" + startText + "</a>" +
                   "<a href='#' id='" + _identifier + "-CustomStudentList-FinishLink' class='CustomStudentList-button CustomStudentList-" + actionClass + "Link' style='display:none;'>" + actionText + "</a>" +
                   "<a href='#' id='" + _identifier + "-CustomStudentList-CloneLink' class='CustomStudentList-button CustomStudentList-CloneLink' style='display:none;'>" + _cloneCustomStudentListText + "</a>" +
                   "<a href='#' id='" + _identifier + "-CustomStudentList-RenameLink' class='CustomStudentList-button CustomStudentList-RenameLink' style='display:none;'>RENAME WATCH LIST</a>" +
                   "<a href='#' id='" + _identifier + "-CustomStudentList-DeleteLink' class='CustomStudentList-button CustomStudentList-DeleteLink' style='display:none;'>DELETE WATCH LIST</a>" +
                   "<a href='#' id='" + _identifier + "-CustomStudentList-CancelLink' class='CustomStudentList-button CustomStudentList-CancelLink' style='display:none;'>CANCEL</a>" +
                   "</div>";

    this.initialize = function () {
        $(linkHtml).appendTo(_linkParentIdentifier);

        var holdThis = this;
        $(_startLinkIdentifier).click(function (e) { holdThis.startCustomStudentList(); (event.preventDefault) ? event.preventDefault() : event.returnValue = false; });
        $(_finishLinkIdentifier).click(function (e) { holdThis.finishCustomStudentList(); (event.preventDefault) ? event.preventDefault() : event.returnValue = false; });
        $(_renameLinkIdentifier).click(function (e) { holdThis.openRenameCustomStudentList(); (event.preventDefault) ? event.preventDefault() : event.returnValue = false; });
        $(_deleteLinkIdentifier).click(function (e) { holdThis.openDeleteCustomStudentList(); (event.preventDefault) ? event.preventDefault() : event.returnValue = false; });
        $(_cancelLinkIdentifier).click(function (e) { holdThis.cancelCustomStudentList(); (event.preventDefault) ? event.preventDefault() : event.returnValue = false; });
        $(_cloneLinkIdentifier).click(function (e) { holdThis.cloneCustomStudentList(); (event.preventDefault) ? event.preventDefault() : event.returnValue = false; });

        $(document).ready(function () {
            if (_isCustomStudentList === false) {
                $(_createDialogId).dialog({
                    autoOpen: false,
                    width: 550,
                    height: 327,
                    title: 'Add students to list',
                    dialogClass: 'bringToTop',
                    modal: true,
                    resizable: false,
                    close: function () { $(_createDialogStudentListTableId + " tr").remove(); },
                    buttons: [
                        {
                            text: 'Cancel',
                            click: function () {
                                $(this).dialog('close');
                            }
                        },
                        {
                            text: 'OK',
                            click: function () {
                                holdThis.createCustomStudentLists();
                            }
                        }
                    ]
                });
            }
            else {
                $(_removeDialogId).dialog({
                    autoOpen: false,
                    width: 305,
                    height: 357,
                    title: 'Remove students from list',
                    dialogClass: 'bringToTop',
                    modal: true,
                    resizable: false,
                    close: function () { $(_removeDialogStudentListTableId + " tr").remove(); },
                    buttons: [
                                    {
                                        text: 'Cancel',
                                        click: function () {
                                            $(this).dialog('close');
                                        }
                                    },
                                    {
                                        text: 'OK',
                                        click: function () {
                                            holdThis.removeFromCustomStudentLists();
                                        }
                                    }
                                ]
                });
                $(_renameDialogId).dialog({
                    autoOpen: false,
                    title: 'Rename watch list',
                    dialogClass: 'bringToTop',
                    modal: true,
                    resizable: false,
                    buttons: [
                        {
                            text: 'Cancel',
                            click: function () {
                                $(this).dialog('close');
                            }
                        },
                        {
                            text: 'OK',
                            click: function () {
                                holdThis.renameCustomStudentList();
                            }
                        }
                    ]
                });
                $(_deleteDialogId).dialog({
                    autoOpen: false,
                    title: 'Delete watch list',
                    dialogClass: 'bringToTop',
                    modal: true,
                    resizable: false,
                    buttons: [
                                      {
                                          text: 'Cancel',
                                          click: function () {
                                              $(this).dialog('close');
                                          }
                                      },
                                      {
                                          text: 'OK',
                                          click: function () {
                                              holdThis.deleteCustomStudentList();
                                          }
                                      }
                                  ]
                });
            }
        });
    };

    this.trackUsage = function (action, additionalData) {
        analyticsManager.trackCustomStudentList(action, _identifier, additionalData);
    };

    this.redrawCheckboxes = function () {
        var i, s, tr;
        var holdThis = this;
        $(_checkboxParentIdentifier + " input.CustomStudentList-checkbox").click(function () {
            holdThis.checkboxChanged($(this));
        });

        if (_selectAllCheckboxParentIdentifier !== "") {
            $(_selectAllCheckboxParentIdentifier + " input.CustomStudentList-checkbox").click(function (event) {
                event.stopPropagation();
                holdThis.selectAllCheckboxes($(this));
            });
        }

        if (_customStudentListStarted) {
            $(_checkboxParentIdentifier + ' .CustomStudentList-select').show();
            $(_checkboxParentIdentifier + ' input.CustomStudentList-checkbox').show();

            if (_selectAllCheckboxParentIdentifier !== "") {
                $(_selectAllCheckboxParentIdentifier + ' input.CustomStudentList-checkbox').show();
            }

            for (i = 0; i < _ids.length; i++) {
                s = $(_checkboxParentIdentifier + ' input.CustomStudentList-checkbox[dataId=' + _ids[i] + ']');
                s.prop('checked', true);
                tr = s.closest('tr').parent().closest('tr');
                $(tr).addClass('selectedRow');
            }
        }
    };

    this.startCustomStudentList = function () {
        $(_startLinkIdentifier).hide();
        $(_finishLinkIdentifier).show();
        if (_isCustomStudentList) {
            $(_renameLinkIdentifier).show();
            $(_deleteLinkIdentifier).show();
        } else {
            $(_cloneLinkIdentifier).show();
        }
        $(_cancelLinkIdentifier).show();
        $(_linkParentIdentifier).siblings().hide();
        $(_buttonDivIdentifier).siblings().hide();
        $(_checkboxParentIdentifier + ' .CustomStudentList-select').show();
        $(_checkboxParentIdentifier + ' input.CustomStudentList-checkbox').show();
        if (_selectAllCheckboxParentIdentifier !== "") {
            $(_selectAllCheckboxParentIdentifier + ' input.CustomStudentList-checkbox').show();
        }
        _customStudentListStarted = true;
        this.trackUsage('select', null);
    };

    this.cancelCustomStudentList = function () {
        $(_startLinkIdentifier).show();
        $(_finishLinkIdentifier).hide();
        $(_cloneLinkIdentifier).hide();
        if (_isCustomStudentList) {
            $(_renameLinkIdentifier).hide();
            $(_deleteLinkIdentifier).hide();
        }
        $(_cancelLinkIdentifier).hide();
        $(_linkParentIdentifier).siblings().show();
        $(_buttonDivIdentifier).siblings().show();
        $(_checkboxParentIdentifier + ' .CustomStudentList-select').hide();
        $(_checkboxParentIdentifier + ' input.CustomStudentList-checkbox').hide();
        if (_selectAllCheckboxParentIdentifier !== "") {
            $(_selectAllCheckboxParentIdentifier + ' input.CustomStudentList-checkbox').hide();
            $(_selectAllCheckboxParentIdentifier + ' input.CustomStudentList-checkbox').prop('checked', false);
        }
        $(_checkboxParentIdentifier + '>tbody>tr').removeClass('selectedRow');


        $(_checkboxParentIdentifier + ' input.CustomStudentList-checkbox').prop('checked', false);
        _ids = [];
        _selectedData = [];
        _customStudentListStarted = false;
    };

    this.finishCustomStudentList = function () {
        if (_ids.length > 0) {
            if (_isCustomStudentList) {
                this.openRemoveFromStudentListDialog(_selectedData, _customStudentListId);
            } else {
                this.openCreateStudentListDialog(_selectedData);
            }
        }
        this.cancelCustomStudentList();
    };

    this.cloneCustomStudentList = function () {
        var caller = this;
        $.ajax({
            type: 'POST',
            url: _sessionStudentListUrl,
            data: { sessionUniqueId: _sessionUniqueId },
            success: function (returnValue) {
                if (returnValue != null) {
                    caller._selectedData = returnValue.selectedData;
                    caller.openCreateStudentListDialog(caller._selectedData, returnValue.displayValue);
                }
            },
            traditional: true
        }).fail(function (xhr, status, error) {
            if ((xhr.responseText.indexOf("Login") > -1) || (xhr.responseText.indexOf("Log In") > -1)) {
                var url = window.location.href;
                _processingRequest = false;
                $(_createDialogWaitDivId).hide();
                NavigateToPage(url);
                return;
            }
        });

        this.cancelCustomStudentList();
    };

    this.checkboxChanged = function (checkbox) {
        var tr = checkbox.closest('tr').parent().closest('tr');
        var id = checkbox.attr('dataId');
        var dataId = Number(id);
        var pos = jQuery.inArray(dataId, _ids);
        if (checkbox.is(':checked')) {
            if (pos === -1) {
                _ids.push(dataId);
                $(tr).addClass('selectedRow');
                var data = jQuery[_gridDataIdentifier];
                var dataRows = data.Rows;
                var i;
                for (i = 0; i < dataRows.length; i++) {
                    if (dataRows[i][1].StudentUSI === dataId) {
                        _selectedData.push(dataRows[i][1]);
                        break;
                    }
                }
            }
        } else {
            $(tr).removeClass('selectedRow');
            if (pos !== -1) {
                _ids = jQuery.grep(_ids, function (value) {
                    return value !== dataId;
                });

                _selectedData = jQuery.grep(_selectedData, function (value) {
                    return value.StudentUSI !== dataId;
                });
            }
        }
    };

    this.selectAllCheckboxes = function (checkbox) {
        var checkboxes = $(_checkboxParentIdentifier + " input.CustomStudentList-checkbox");
        for (var i = 0; i < checkboxes.length; i++) {
            if (checkbox.is(':checked')) {
                $(checkboxes[i]).prop('checked', true);
            } else {
                $(checkboxes[i]).prop('checked', false);
            }

            this.checkboxChanged($(checkboxes[i]));
        }

    };

    this.openDeleteCustomStudentList = function () {
        $(_deleteDialogId).dialog('open');
    };

    this.openRenameCustomStudentList = function () {
        $(_renameDialogId).dialog('open');
    };

    this.openRemoveFromStudentListDialog = function (studentsParam) {
        _selectedStudentUSIs = studentsParam;

        //stick students into list
        $(_nameAndImageCellItemTemplateId).tmpl(_selectedStudentUSIs).appendTo(_removeDialogStudentListTableId + " tbody");

        //set alternating row colors
        $(_removeDialogStudentListTableId + '>tbody>tr:even').addClass('alternatingRow');

        // show dialog
        $(_removeDialogId).dialog('open');
    };

    this.openCreateStudentListDialog = function (studentsParam, displayLabel) {
        _selectedStudentUSIs = studentsParam;
        
        if (typeof displayLabel !== 'undefined') {
            $(_nameAndImageCellItemTemplateId).tmpl([{ DV: "Add All Students" }, { DV: displayLabel }]).appendTo(_createDialogStudentListTableId + " tbody");
        } else {
            $(_nameAndImageCellItemTemplateId).tmpl(_selectedStudentUSIs).appendTo(_createDialogStudentListTableId + " tbody");
        }

        //set alternating row colors
        $(_createDialogStudentListTableId + '>tbody>tr:even').addClass('alternatingRow');

        //clear lists
        $(_createdDialogListSelectionId + ' option').remove();

        // Avoid IE 304
        var cacheDate = new Date();
        var nocacheKey = cacheDate.getTime();
        var delim = _customStudentListUrl.indexOf("?") != -1 ? "&" : "?";

        _customStudentListUrl = _customStudentListUrl + delim + nocacheKey;

        //get custom list entries and stick into listbox
        $.get(_customStudentListUrl,
            function (data) {
                $(_createDialogId).dialog('open');
                if ($.isEmptyObject(data)) {
                    $(_createdDialogListSelectionId).hide();
                    $(_createDialogUseOldListRadio).hide();
                    $(_createDialogUseOldListRadioLabel).hide();
                } else {
                    $.each(data, function () {
                        var customStudentListName = this.Description;
                        var customStudentListId = this.CustomStudentListId;
                        if (customStudentListName) {
                            $(_createdDialogListSelectionId)
                                .append($("<option></option>")
                                        .attr("value", customStudentListId)
                                        .text(customStudentListName));
                        }
                    });
                }
            }, 'json').fail(function (xhr, status, error) {
                if ((xhr.responseText.indexOf("Login") > -1) || (xhr.responseText.indexOf("Log In") > -1)) {
                    var url = window.location.href;

                    NavigateToPage(url);
                    return;
                }
            });

        // show dialog
        $(_createDialogId).dialog('open');
        $(_createDialogNewListNameId).focus();
    };

    this.createCustomStudentLists = function () {
        if (_processingRequest)
            return;


        var listName = $(_createDialogNewListNameId).val();
        var selectedListId = null;
        if (useNewList == false) {
            if ($(_createdDialogListSelectionId).val() != null) {
                selectedListId = $(_createdDialogListSelectionId).val();
            } else {
                alert('Please select a valid list.');
                return;
            }
        } else { //new list
            listName = $.trim(listName);
            if (listName == '') {
                alert('Please provide a name for the list');
                return;
            } else {

            }
        }
        if (_selectedStudentUSIs.length === 0) {
            alert('Please select students to add to the list.');
            return;
        }

        _processingRequest = true;
        $(_createDialogWaitDivId).show();

        var studentIds = [];
        $.each(_selectedStudentUSIs, function () {
            studentIds.push(this.StudentUSI);
        });

        this.trackUsage('add', { newList: useNewList, studentCount: studentIds.length, customStudentListId: selectedListId });
        var customStudentListPostRequest = {
            LocalEducationAgencyId: _localEducationAgencyId,
            SchoolId: _schoolId,
            StaffUSI: _staffUSI,
            CustomStudentListId: selectedListId,
            Description: listName,
            studentUSIs: studentIds,
            Action: 'add'
        };

        var postData = { json: JSON.stringify(customStudentListPostRequest) };

        $.ajax({
            type: 'POST',
            url: _customStudentListUrl,
            data: postData,
            success: function (returnValue) {
                $(_createDialogId).dialog('close');
                _processingRequest = false;
                $(_createDialogWaitDivId).hide();
                window.location.href = returnValue;
            },
            traditional: true
        }).fail(function (xhr, status, error) {
            if ((xhr.responseText.indexOf("Login") > -1) || (xhr.responseText.indexOf("Log In") > -1)) {
                var url = window.location.href;
                _processingRequest = false;
                $(_createDialogWaitDivId).hide();
                NavigateToPage(url);
                return;
            }
        });
    };

    //this is to keep the correct controls active depending on the radio button selected
    var useNewList = true;
    $(function () {
        $(_createDialogUseNewListRadioId).change(function () {
            if ($(this).is(':checked')) {
                useNewList = true;
                $(_createDialogNewListNameId).removeAttr('disabled');
                $(_createdDialogListSelectionId).attr('disabled', true);
            } else {
                useNewList = false;
                $(_createDialogNewListNameId).attr('disabled', true);
                $(_createdDialogListSelectionId).removeAttr('disabled');
            }
        });
        $(_createDialogUseOldListRadio).change(function () {
            if ($(this).is(':checked')) {
                useNewList = false;
                $(_createdDialogListSelectionId).removeAttr('disabled');
                $(_createDialogNewListNameId).attr('disabled', true);
            } else {
                useNewList = true;
                $(_createdDialogListSelectionId).attr('disabled', true);
                $(_createDialogNewListNameId).removeAttr('disabled');
            }
        });
    });

    this.renameCustomStudentList = function () {
        if (_processingRequest)
            return;

        var listName = $(_renameDialogNewListNameId).val();
        listName = $.trim(listName);
        if (listName == '') {
            alert('Please provide a name for the list');
            return;
        }

        _processingRequest = true;
        $(_renameDialogWaitDivId).show();

        this.trackUsage('rename', { customStudentListId: _customStudentListId });
        var customStudentListRenameRequest = {
            LocalEducationAgencyId: _localEducationAgencyId,
            SchoolId: _schoolId,
            StaffUSI: _staffUSI,
            CustomStudentListId: _customStudentListId,
            Description: listName,
            Action: 'set'
        };

        var postData = { json: JSON.stringify(customStudentListRenameRequest) };

        $.ajax({
            type: 'POST',
            url: _customStudentListUrl,
            data: postData,
            success: function (returnValue) {
                $(_renameDialogId).dialog('close');
                _processingRequest = false;
                $(_renameDialogWaitDivId).hide();
                window.location.href = returnValue;
            },
            traditional: true
        }).fail(function (xhr, status, error) {
            if ((xhr.responseText.indexOf("Login") > -1) || (xhr.responseText.indexOf("Log In") > -1)) {
                var url = window.location.href;
                _processingRequest = false;
                $(_renameDialogWaitDivId).hide();

                NavigateToPage(url);
                return;
            }
        }); ;

    };

    this.deleteCustomStudentList = function () {
        if (_processingRequest)
            return;

        _processingRequest = true;
        $(_deleteDialogWaitDivId).show();

        this.trackUsage('delete', { customStudentListId: _customStudentListId });
        var customStudentListDeleteRequest = {
            LocalEducationAgencyId: _localEducationAgencyId,
            SchoolId: _schoolId,
            StaffUSI: _staffUSI,
            CustomStudentListId: _customStudentListId,
            Action: 'delete'
        };

        var postData = { json: JSON.stringify(customStudentListDeleteRequest) };

        $.ajax({
            type: 'POST',
            url: _customStudentListUrl,
            data: postData,
            success: function (returnValue) {
                $(_deleteDialogId).dialog('close');
                _processingRequest = false;
                $(_deleteDialogWaitDivId).hide();
                window.location.href = returnValue;
            },
            traditional: true
        }).fail(function (xhr, status, error) {
            if ((xhr.responseText.indexOf("Login") > -1) || (xhr.responseText.indexOf("Log In") > -1)) {
                var url = window.location.href;
                _processingRequest = false;
                $(_deleteDialogWaitDivId).hide();

                NavigateToPage(url);
                return;
            }
        }); ;

    };

    this.removeFromCustomStudentLists = function () {
        if (_processingRequest)
            return;

        _processingRequest = true;
        $(_removeDialogWaitDivId).show();

        var studentIds = [];
        $.each(_selectedStudentUSIs, function () {
            studentIds.push(this.StudentUSI);
        });
        this.trackUsage('remove', { studentCount: studentIds.length, customStudentListId: _customStudentListId });

        var customStudentListPostRequest = {
            LocalEducationAgencyId: _localEducationAgencyId,
            SchoolId: _schoolId,
            StaffUSI: _staffUSI,
            CustomStudentListId: _customStudentListId,
            studentUSIs: studentIds,
            Action: 'remove'
        };

        var postData = { json: JSON.stringify(customStudentListPostRequest) };

        $.ajax({
            type: 'POST',
            url: _customStudentListUrl,
            data: postData,
            success: function (returnValue) {
                $(_removeDialogId).dialog('close');
                _processingRequest = false;
                $(_removeDialogWaitDivId).hide();
                window.location.href = returnValue;
            },
            traditional: true
        }).fail(function (xhr, status, error) {
            if ((xhr.responseText.indexOf("Login") > -1) || (xhr.responseText.indexOf("Log In") > -1)) {
                var url = window.location.href;
                _processingRequest = false;
                $(_removeDialogWaitDivId).hide();

                NavigateToPage(url);
                return;
            }
        }); ;

    };
};