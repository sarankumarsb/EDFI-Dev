/// <reference path="../External/jquery.js" />
/// <reference path="../External/json2.js" />

var searchResultsFilter = "All";
var searchOption;
var searchResultsData;

//Sorting the Json Object array
jQuery.fn.sort = function () { return this.pushStack([].sort.apply(this, arguments), []); };

var searchResultsColumnKeyToSortBy;

function sortValueAsc(a, b) {

    var key = searchResultsColumnKeyToSortBy;

    //Logic for the nulls
    if (a[key] == null)
        return 1;
    if (b[key] == null)
        return -1;

    //If the item is an object then it must have a V (value) property to sort by.
    if (typeof (a[key]) == 'object') {
        if (a[key].V == b[key].V)
            return 0;
        return (a[key].V > b[key].V) ? 1 : -1;
    } else {
        if (a[key] == b[key])
            return 0;
        return (a[key] > b[key]) ? 1 : -1;
    }
};

function sortValueDesc(a, b) {
    return sortValueAsc(a, b) * -1;
};

function SetSearchResultsFilter(filterToUse) {
    $('#localSearchFilter').val(filterToUse);

    searchResultsFilter = filterToUse;
    setAllFilterImagesToGray();
    $('#filter' + filterToUse).addClass("btn");
    $('#filter' + filterToUse).removeClass("btn-deemphasized");

    searchAgainClick();
}

function setAllFilterImagesToGray() {
    $('#filterAll').addClass("btn-deemphasized");
    $('#filterSchools').addClass("btn-deemphasized");
    $('#filterTeachers').addClass("btn-deemphasized");
    $('#filterStudents').addClass("btn-deemphasized");
    $('#filterAll').removeClass("btn");
    $('#filterSchools').removeClass("btn");
    $('#filterTeachers').removeClass("btn");
    $('#filterStudents').removeClass("btn");
}

function searchAgainClick() {
    //Clear the content of the div.
    $('#searchResultsDiv').empty();
    var searchText = $('#localSearchInput').val();
    var filterToUse = $('#localSearchFilter').val();

    if (searchText != '') {
        window.location = searchResultsPage.replace("{textValue}", searchText).replace("{filterValue}", filterToUse);
    }
}

function search() {

    var filterToUse = $('#localSearchFilter').val();
    if (filterToUse == '')
        filterToUse = "All";
    searchResultsFilter = filterToUse;
    setAllFilterImagesToGray();
    $('#filter' + filterToUse).addClass("btn");
    $('#filter' + filterToUse).removeClass("btn-deemphasized");

    //Clear the content of the div.
    $('#searchResultsDiv').empty();
    var searchText = $('#localSearchInput').val();

    if (searchText != '')
        getSearchResults(searchText);
}

function getSearchResults(searchText) {

    $('#localSearchInput').addClass("loading");

    var matchContains = false;
    if (searchText.length > 2)
        matchContains = true;

    //Prepare the parameters we are going to submit.
    var submitParameters = {
        textToFind: searchText,
        rowCountToReturn: maxSearchCount,
        matchContains: matchContains,
        filter: searchResultsFilter
    };

    analyticsManager.trackSearch('Advanced Type', searchResultsFilter);
    analyticsManager.trackSearch('Advanced Terms', searchText);

    //Ajax call to get data...
    $.ajax({
        type: "POST",
        url: getExpandedResultsUrl,
        data: JSON.stringify(submitParameters),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            searchResultsData = {
                Students: msg.Students,
                Schools: msg.Schools,
                Teachers: msg.Teachers,
                Staff: msg.Staff,
                AbsoluteStudentsCount: msg.AbsoluteStudentsCount,
                AbsoluteSchoolsCount: msg.AbsoluteSchoolsCount,
                AbsoluteTeachersCount: msg.AbsoluteTeachersCount,
                AbsoluteStaffCount: msg.AbsoluteStaffCount
            };
            handleStudentAndStaffIds(searchText);
            drawSearchResults(searchResultsData, searchText);
            $('#localSearchInput').removeClass("loading");
        },
        error: function (result) {
            //alert("Search failed. Please try again. (AJAX call failed: " + result.status + " " + result.statusText+ " )"); 
        }
    });
}

function drawSearchResults(data, searchText) {
    //Get the search option that we are going to apply for drawing.
    searchOption = getSearchOption();

    //alert(JSON.stringify(data));
    //Apply Templates to data...
    var templateName = 'noResultsTemplate';
    if (hasResults(data)) {
        var isNumericInput = !isNaN($.trim(searchText));
        if (isNumericInput)
            templateName ='grayTableHeaderTemplateNumeric';
        else
            templateName = 'grayTableHeaderTemplate';
    }
    var templateResults = $.tmpl(templateName, data);
    templateResults.appendTo('#searchResultsDiv');

    //After drawing then we stripe the tables.
    $('#dataResultsForSchools tr:even').addClass('alternating-row');
    $('#dataResultsForTeachers tr:even').addClass('alternating-row');
    $('#dataResultsForStudents tr:even').addClass('alternating-row');
    $('#dataResultsForStaff tr:even').addClass('alternating-row');

    createEntityListCookieWithExtraParametersForSearch($('#dataResultsForStudents'), 1, 'Students', 'Students', setSessionObjectUrl);
    createEntityListCookieWithExtraParametersForSearch($('#dataResultsForSchools'), 1, 'Schools', 'Schools', setSessionObjectUrl);
    createEntityListCookieWithExtraParametersForSearch($('#dataResultsForTeachers'), 1, 'Teachers', 'Teachers', setSessionObjectUrl);
}

function hasResults(data) {
    for (key in data)
        if ((key.indexOf("__type") == -1) && (data[key].length > 0))
            return true;

    return false;
}

function selectHeaderTemplate(headerObjectType) {
    var template = $.template("searchResults" + headerObjectType + "HeaderTemplate");
    return template;
}

function selectHeaderTemplateNumeric(headerObjectType) {
    var template = $.template("searchResults" + headerObjectType + "HeaderTemplateNumeric");
    return template;
}

function selectBaseRowTemplate() {
    var template = $.template("rowTemplate");
    return template;
}

function selectBaseNumericRowTemplate() {
    var template = $.template("rowTemplateNumeric");
    return template;
}

function selectRowTemplate(parentType) {
    var template = $.template("searchResults" + parentType + "RowTemplate");
    return template;
}

function selectRowTemplateNumeric(parentType) {
    var template = $.template("searchResults" + parentType + "RowTemplateNumeric");
    return template;
}

function highlightSearchText(line, mode) {

    //if we have nothing then return "" so it doesn't blow up.
    if (line == undefined)
        return "";

    var word = $('#localSearchInput').val().replace(",", "").replace(".", "");

    //If they are the same: don't highlight...
    if (word == line)
        return line;

    switch (mode) {
        case 'SchoolName':
            //return line;
            return highlightBasedOnSearchOption(word, line);
        case 'FirstName':
            switch (searchOption) {
                case 1:
                    return highlightStartsWith(word, line);
                case 2:
                    return highlightContains(word, line);
                case 3:
                case 4:
                    return highlightFirstNameOrLastSurnameStartsWith(word, line);
                default:
                    return highlightContains(word, line);
            }
        case 'MiddleName':
            return highlightMiddleNameStartsWith(word, line);
        case 'LastSurname':
            switch (searchOption) {
                case 1:
                    return highlightStartsWith(word, line);
                case 2:
                    return highlightContains(word, line);
                case 3:
                case 4:
                    return highlightFirstNameOrLastSurnameStartsWith(word, line);
                default:
                    return highlightContains(word, line);
            }
        case 'IdentificationCode':
            switch (searchOption) {
                case 1:
                    return highlightStartsWith(word, line);
                case 2:
                case 3:
                case 4:
                default:
                    return highlightContains(word, line);
            }
        default:
            return line;
    }

}

function highlightBasedOnSearchOption(wordToMatch, lineToReplace) {
    switch (searchOption) {
        case 1:
            return highlightStartsWith(wordToMatch, lineToReplace);
            break;
        case 2:
            return highlightContains(wordToMatch, lineToReplace);
            break;
        case 3:
            return highlightStartsWithFirstWordContainsTheNext(wordToMatch, lineToReplace);
            break;
        case 4:
            return highlightStartsWithFirstWordContainsSecondContainsThird(wordToMatch, lineToReplace);
            break;
        default:
            return highlightContains(wordToMatch, lineToReplace);
            break;
    }
}

function highlightStartsWith(wordToMatch, lineToReplace) {
    var regex = new RegExp('^(' + wordToMatch + ')', 'gi');
    return lineToReplace.replace(regex, "<em>$1</em>");
}

function highlightFirstNameOrLastSurnameStartsWith(wordToMatch, lineToReplace) {
    var wordsToSearch = wordToMatch.split(' ');
    var firstWord = wordsToSearch[0];
    var lastWord = wordsToSearch[wordsToSearch.length - 1];

    var regex1 = new RegExp('^(' + wordsToSearch[0] + ')(.*)', 'gi');
    var regex2 = new RegExp('^(' + wordsToSearch[1] + ')(.*)', 'gi');
    var m = regex1.exec(lineToReplace);
    if (m == null)
        m = regex2.exec(lineToReplace);

    //If m is still null then return text.
    if (m == null)
        return lineToReplace;

    var result = "<em>" + m[1] + "</em>";
    if (m.length > 1)
        result += m[2];

    return result;
}

function highlightMiddleNameStartsWith(wordToMatch, lineToReplace) {
    var wordsToSearch = wordToMatch.split(' ');
    if (wordsToSearch.length == 3)
        return highlightStartsWith(wordsToSearch[1], lineToReplace);

    return lineToReplace;
}

function highlightContains(wordToMatch, lineToReplace) {
    var regex = new RegExp('(' + wordToMatch + ')', 'gi');
    if (!isNaN(lineToReplace)) {
        lineToReplace = lineToReplace.toString();
    }
    return lineToReplace.replace(regex, "<em>$1</em>");
}

function highlightLastWordContains(wordToMatch, lineToReplace) {
    var wordsToSearch = wordToMatch.split(' ');

    highlightContains(wordsToSearch[wordsToSearch.length - 1], lineToReplace);

}

function highlightStartsWithFirstWordContainsTheNext(wordToMatch, lineToReplace) {
    var wordsToSearch = wordToMatch.split(' ');

    var regex1 = new RegExp('^(' + wordsToSearch[0] + ')(.*)(' + wordsToSearch[1] + ')(.*)', 'gi');
    var regex2 = new RegExp('^(' + wordsToSearch[1] + ')(.*)(' + wordsToSearch[0] + ')(.*)', 'gi');
    var m = regex1.exec(lineToReplace);
    if (m == null)
        m = regex2.exec(lineToReplace);

    //If m is still null then return text.
    if (m == null)
        return lineToReplace;

    var result = "<em>" + m[1] + "</em>";
    result += m[2];
    result += "<em>" + m[3] + "</em>";
    if (m.length > 3)
        result += m[4];

    return result;
}

function highlightStartsWithFirstWordContainsSecondContainsThird(wordToMatch, lineToReplace) {
    var wordsToSearch = wordToMatch.split(' ');
    var regex1 = new RegExp('^(' + wordsToSearch[0] + ')(.*)(' + wordsToSearch[1] + ')(.*)(' + wordsToSearch[2] + ')(.*)', 'gi');
    var regex2 = new RegExp('^(' + wordsToSearch[2] + ')(.*)(' + wordsToSearch[1] + ')(.*)(' + wordsToSearch[0] + ')(.*)', 'gi');

    var m = regex1.exec(lineToReplace);
    if (m == null)
        m = regex2.exec(lineToReplace);

    //If m is still null then return text.
    if (m == null)
        return lineToReplace;

    var result = "<em>" + m[1] + "</em>";
    result += m[2];
    result += "<em>" + m[3] + "</em>";
    result += m[4];
    result += "<em>" + m[5] + "</em>";
    if (m.length > 5)
        result += m[6];

    return result;
}

function getSearchOption() {

    var searchOption = 1; //We start with a "Starts With"
    var word = $('#localSearchInput').val().replace(",", "").replace(".", "");

    var wordsToSearch = word.split(' ');

    if (word.length > 2) {
        if (wordsToSearch.length == 1)
            searchOption = 2; //"Contains"
        else if (wordsToSearch.length == 2)
            searchOption = 3; //FirstWord:StartsWith, Second:Contains
        else if (wordsToSearch.length == 3) {
            if (wordsToSearch[2] == "")
                searchOption = 3; //FirstWord:StartsWith, Second:Contains
            else
                searchOption = 4; //FirstWord:StartsWith, Second:Contains, Third:Contains
        }
        else //Default or fallback is to use contains option
            searchOption = 2; //"Contains"
    }

    return searchOption;
}

function sortSearchResultsData(entity, field, numeric) {
    var direction = "Asc";
    //Define the sort Direction based on the class.
    if ($('#' + entity + "SortIcon" + field).hasClass('icon-chevron-up'))
        direction = "Desc";

    //Do some sorting...		
    for (var key in searchResultsData)
        if (key == entity) {
            //alert(entity+"|"+field+"|"+direction+"|"+key+"|"+ JSON.stringify(searchResultsData[key][0]["School"]));			

            searchResultsColumnKeyToSortBy = field;

            if (direction == "Asc")
                searchResultsData[key].sort(sortValueAsc);
            else
                searchResultsData[key].sort(sortValueDesc);

            redrawSearchResultRows(entity, searchResultsData[key], numeric);
            break;
        }


    //Change the direction of sort just in case they reclick it.
    //First reset all..
    $('i[id^="' + entity + 'SortIcon"]').removeClass("icon-chevron-down");
    $('i[id^="' + entity + 'SortIcon"]').removeClass("icon-chevron-up");

    if (direction == "Asc") {
        $('#' + entity + "SortIcon" + field).removeClass("icon-chevron-down");
        $('#' + entity + "SortIcon" + field).addClass("icon-chevron-up");        
    }
    else {
        $('#' + entity + "SortIcon" + field).removeClass("icon-chevron-up");
        $('#' + entity + "SortIcon" + field).addClass("icon-chevron-down");
    }

    //alert(entity + " " + field + " " + direction);
    logSortingActions(entity, field, direction);
}

function logSortingActions(entity, field, direction) {
    analyticsManager.trackSearch('Advanced Sort', entity + " " + field + " " + direction);
    
	var dataToSendToService = { sortCriteria: entity + " " + field + " " + direction };

	$.ajax({
		type: "POST",
		url: getLoggerWebServiceUrlWithMethodForSortingAction,
		data: JSON.stringify(dataToSendToService),
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function (jsonData) {
		},
		error: function (result) {
			//we might have an error because we are probably leaving the page while we submit the data.
			//even though it will throw an error. It always executes the command on the web service.
			//alert("AJAX call failed: " + result.status + " " + result.statusText + " obj:" + JSON.stringify(result));
		}
	});
}

function logNavigationActions(url, type) {
    analyticsManager.trackSearch('Advanced Select', type);

    var dataToSendToService = { url: url, controlNameWhoIsCalling: "through the advanced search" };

	$.ajax({
		type: "POST",
		url: getLoggerWebServiceUrlWithMethodForNavigationAction,
		data: JSON.stringify(dataToSendToService),
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function (jsonData) {
		    window.location = url;
		},
		error: function (result) {
		}
	});
}

function goToSelectedEntityLink(url, listContext, type) {
    if (listContext != null && listContext != '') {
        if (url.indexOf("?") != -1)
            url = url + "&listContext=" + listContext;
        else
            url = url + "?listContext=" + listContext;
    }
    
	logNavigationActions(url, type);

}

function redrawSearchResultRows(entity, data, numeric) {
    //Clean everything
    $('#dataResultsFor' + entity).empty();

    if (numeric === true) {
        $.tmpl('rowTemplateNumeric', data, { "objType": entity }).appendTo('#dataResultsFor' + entity);
    } else {
        $.tmpl('rowTemplate', data, { "objType": entity }).appendTo('#dataResultsFor' + entity);
    }

    //After drawing then we stripe the tables.
    $('#dataResultsFor' + entity + ' tr:even').addClass('alternating-row');

    if (entity != "Staff")//Today we do not traverse the staff list so lets not set cookies on it.
        createEntityListCookieWithExtraParametersForSearch($('#dataResultsFor' + entity), 1, entity, entity, setSessionObjectUrl);
}

function handleStudentAndStaffIds(searchText) {

    if (hasResults(searchResultsData)) {
        // Handle Student Identification 
        for (idx = 0; idx < searchResultsData.Students.length; idx++) {
            if (searchResultsData.Students[idx].IdentificationCode == null ||
                (searchResultsData.Students[idx].StudentUSI.toString() == searchText && searchResultsData.Students[idx].IdentificationCode != searchText)) {
                searchResultsData.Students[idx].IdentificationCode = searchResultsData.Students[idx].StudentUSI.toString();
                searchResultsData.Students[idx].IdentificationSystem = "USI";
            }
        }

        // Handle Staff Identification
        for (idx = 0; idx < searchResultsData.Staff.length; idx++) {
            if (searchResultsData.Staff[idx].IdentificationCode == null ||
                (searchResultsData.Staff[idx].StaffUSI.toString() == searchText && searchResultsData.Staff[idx].IdentificationCode != searchText)) {
                searchResultsData.Staff[idx].IdentificationCode = searchResultsData.Staff[idx].StaffUSI.toString();
                searchResultsData.Staff[idx].IdentificationSystem = "USI";
            }
        }
    }
}