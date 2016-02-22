/// <reference path="../External/jquery.js" />


function test(txt) {
    alert(txt);
}

function log(msg) {
    console.log(msg);
}

function sortArrow(index, item) {
    if (index == item.columnToSort)
        if (item.sortDirection == "asc")
            return "<img class='sortArrow' src='" + $.global.siteBasePath + "/App_themes/Theme1/img/GrayHeader/AscArrowWhite.png' alt='Sort Ascending' />";
        else
            return "<img class='sortArrow' src='" + $.global.siteBasePath + "/App_themes/Theme1/img/GrayHeader/DescArrowWhite.png' alt='Sort Descending' />";

    return "";
}

function getFlattendedColumnIndex(columnsDataItem, i, j) {
    var kidCount = 0;
    for (var x = 0; x < i; x++) {
        kidCount += columnsDataItem[x].Children.length;
    }

    return kidCount + j;
}

function headerHasChildren(cellItemToTest) {
    if (cellItemToTest.Children.length > 0)
        return true;

    return false;
}

function getTemplateForSingleTabedHeader(i, j, cellDataItem) {
    //the first one is always going to be a left corner
    if (i == 0 && j == 0)
        return $.template("leftRoundedCornerImage");

    //Apply spacer...
    if (i % 2 == 1)
        return $.template("headerTabSpacerCellTemplate");

    //Could be a flag or any text... So lets check the props on the dataObject
    if (i == 0 && j == 1)
        return $.template("selectAllHeaderTabCellTemplate");

    return $.template("defaultHeaderTabCellTemplate");
}

function getTemplateForSingleTabedHeaderTooltip(i, cellDataItem) {
    //the first one is always going to be a left corner //Apply spacer...
    if (i == 0 || i == 1)
        return $.template("blank");

    //Could be a flag or any text... So lets check the props on the dataObject
    if (cellDataItem.Tooltip != null)
        return $.template("tooltipTemplate");
    return $.template("blank");
}

function getHeaderObjectType(dataObj) {
    for (prop in dataObj) {
        if (prop == "Src")
            return "ImageColumn";
        if (prop == "DisplayName")
            return "TextColumn";
    }

    return "Column"; //The base class...
}


function getTemplateForTabedHeader(index, cellDataItem) {
    if (index == 0)
        return $.template("emptyHeaderTabCellTemplate");

    if (index % 2 == 1)
        return $.template("headerTabSpacerCellTemplate");

    switch (index % 3) {
        case 2:
            return $.template("yellowHeaderTabCellTemplate");
        case 1:
            return $.template("orangeHeaderTabCellTemplate");
        case 0:
            return $.template("redHeaderTabCellTemplate");
    }

    return $.template("defaultHeaderTabCellTemplate");
}

function getTemplateForSingleRowHeader(index, count, cellDataItem) {
    //test(getHeaderObjectType(cellDataItem));

    if (index == 0)
        return $.template("leftRoundedCornerImage");

    if (index == count - 1)
        return $.template("rightRoundedCornerImage");
    
    if (index == 1)
        return $.template("selectAllHeaderTabCellTemplate");

    return $.template("defaultHeaderTabCellTemplate");
}

function getVisibleChildrenCount(children) {
    var visibleChildren = 0;
    for (var j = 0; j < children.length; j++)
        if (children[j].IsVisibleByDefault)
            visibleChildren++;

    return visibleChildren;
}

function getIndex() {
    return $.inArray(this.data, datatable01.Columns);
}

function getCssClassForTopHeader(index, headerItem) {
    var res = "";
    if (getVisibleChildrenCount(headerItem.Children) == 0)
        res += ' hiddenCol';

    if (index == 0)//Do nothing else
        res += ' mcHeadGrey02';

    if (index % 2 == 1)//Do nothing else
        res += ' mcHeadClearDividerBG';

//    switch (index % 3) {//Add background color
//        case 2:
//            return res + "mcHeadYellow02";
//        case 1:
//            return res + "mcHeadOrange02";
//        case 0:
//            return res + "mcHeadRed02";
//    }

    return res;
}

function getCssClassForHeader(i, j, flattenedColumnIndex, headerItem, drawFixedColumn, headerClasses) {

    var resClass = getCssClassForRow(drawFixedColumn, flattenedColumnIndex);
    resClass += " ";
    resClass += headerClasses[flattenedColumnIndex];


    //All the first column group is gray.
    if (i == 0 && j == 0) {
        return resClass;
    }

    if (headerItem.Tooltip !== undefined && headerItem.Tooltip != null)
        resClass = resClass + ' headertooltip ';

    //All the first column group is gray.
    if (i == 0)
        return resClass + ' mcHeadGrayBG';

    //Calculate the dividers...
    if (i % 2 == 1)
        return resClass;

    switch (i % 3) {
        case 2:
            return resClass + ' mcHeadYellowBG';
        case 1:
            return resClass + ' mcHeadOrangeBG';
        case 0:
            return resClass + ' mcHeadRedBG';
    }

    return resClass + ' mcHeadGrayBG';
}

function getCssClassForFixedOrScrollHeader(drawFixedColumn) {
    if (drawFixedColumn) {
        return 'fixedHeaderCell';
    }
    return 'scrollHeaderCell';
}

function getCSSForSingleRowTh(index, count, headerItem, drawFixedColumn, headerClasses) {

    var resClass = getCssClassForRow(drawFixedColumn, index);
    resClass += " ";
    resClass += headerClasses[index];

    //The first column group is gray.
    if (index == 0 || index == (count - 1))
        return resClass;

    if (headerItem.Tooltip !== undefined && headerItem.Tooltip != null)
        resClass = resClass + ' headertooltip ';

    return resClass + ' mcHeadGrayBG ';
}

function getColumnClass(i) {
    return "column" + i;
}

function getIdForTooltip(i, j, identifier) {
    return identifier + "group" + i + "col" + j + "tooltip";
}

function alternateRow(rows) {
    var position = $.inArray(this.data, rows);
    var rowClass = "row" + position;
    if (position % 2)
        rowClass = rowClass + " alternatingRow";
    return rowClass;
}

function getCssClassForRow(drawFixedCells, i) {
    if (drawFixedCells) {
        return "fix" + i;
    }
    return "scroll" + i;
}

function drawCell(cellsToDraw, i) {
    return $.inArray(i, cellsToDraw) != -1;
}

function getMainLink(hrefToUse, href, links) {
    if (hrefToUse != null) {
        var link = getLink(links, hrefToUse);
        if (link !== undefined)
            return link;
    }
    return href;
}

function getLink(links, key) {
    for (var i = 0; i < links.length; i++)
        if (links[i] != null && links[i].Rel == key)
            return links[i].Href;
}

function evaluateYearsOfExperience(yearsOfExperience) {
    if (yearsOfExperience <= 5)
        return '<span class="StateTextRed">' + yearsOfExperience + '</span>';
    return yearsOfExperience;
}

function evaluateHighestLevelOfEducation(highestLevelOfEducation) {
    if (highestLevelOfEducation == "No Degree")
        return '<span class="StateTextRed">' + highestLevelOfEducation + '</span>';

    return highestLevelOfEducation;
}

function getMetricState(state) {
    switch (state) {
        case 1:
            return "StateTextGreen";
        case 2:
            return "StateTextYellow";
        case 3:
        case 7:
            return "StateTextRed";
        case 6:
            return "StateTextVeryGreen";
        default:
            return "StateTextNeutral";
    }
}

function getObjectiveCSS(state) {
    switch (state) {
        case 1:
            return "RoundStateGood";
        case 3:
        case 7:
            return "RoundStateLow";
        case 6:
            return "RoundStateVeryGood";
        default:
            return "RoundStateNone";
    }
}

//Note: This function can be overwritten depending on trend requirement.
//Base requirement defines: 
//    --If tend is good the arrow should always go up and be green.
//    --If tend is bad the arrow should always go down and be red.
function getMetricTrend(direction) {
    switch (direction) {
        case "UpGood":
        case "DownGood":
            return "<i class=\"icon-up-dir arrow-green\"><span class=\"hidden\">Getting better from prior period.</span></i>";
        case "UpBad":
        case "DownBad":
            return "<i class=\"icon-down-dir arrow-red\"><span class=\"hidden\">Getting worse from prior period.</span></i>";
        case "1":
        case "UpNoOpinion":
            return "<i class=\"icon-up-dir arrow-grey\"><span class=\"hidden\">Increase from prior period.</span></i>";
        case "-1":
        case "DownNoOpinion":
            return "<i class=\"icon-down-dir arrow-grey\"><span class=\"hidden\">Decrease from prior period.</span></i>";
        case "0":
        case "NoChangeNoOpinion":
            return "<i class=\"icon-no-change arrow-grey\"><span class=\"hidden\">No change from prior period.</span></i>";
        case "None":
        default:
            return "<div style='width:15px;'></div>";
    }
}

function getStudentFlag(flag) {
    if (flag) {
        return "<div style='width:11px;'><img src=\"" + $.global.siteBasePath + "/App_Themes/Theme1/img/FlagRed.gif\"></div>";
    }
    else {
        return "<div style='width:11px;'><img src=\"" + $.global.siteBasePath + "/App_Themes/Theme1/img/FlagSpaceSaver.gif\"></div>";
    }
}

function getMetricIndicator(indicator) {
    if (indicator == 1 || indicator == 2 || indicator == 3)
        return "<i class=\"icon-test-ccommodation\"><span class=\"hidden\">Test Accommodation</span></i>";
    else if (indicator == 4)
        return "<i class=\"icon-taks-m\"><span class=\"hidden\">TAKS-M</span></i>";
    else if (indicator == 5)
        return "<i class=\"icon-taks-alt\"><span class=\"hidden\">TAKS-Alt</span></i>";
    else if (indicator == 6)
        return "<i class=\"icon-taks-commended\"><span class=\"hidden\">TAKS-Commended</span></i>";
    else if (indicator == 7)
        return "<i class=\"icon-taks-m\"><span class=\"hidden\">STAAR-M</span></i>";
    else if (indicator == 8)
        return "<i class=\"icon-taks-alt\"><span class=\"hidden\">STAAR-Alt</span></i>";
    else if (indicator == 9)
        return "<i class=\"icon-staar-level-iii\"><span class=\"hidden\">STAAR Level 3</span></i>";
    else
        return "<div style='width:23px;'></div>";
}

function getDesignations(designationsArray) {
    var designations = "";
    for (d in designationsArray) {
        switch (designationsArray[d].Value) {
            case 1:
                designations += "<i class=\"icon-gifted-and-talented\"><span class=\"hidden\">Gifted and Talented</span></i>";
                break;
            case 2:
                designations += "<i class=\"icon-special-education\"><span class=\"hidden\">Special Education</span></i>";
                break;
            case 3:
                designations += "<i class=\"icon-esl\"><span class=\"hidden\">ESL, LEP or Bilingual</span></i>";
                break;
            case 5:
                designations += "<i class=\"icon-repeater\"><span class=\"hidden\">Repeater</span></i>";
                break;
            case 6:
                designations += "<i class=\"icon-late-enrollment\"><span class=\"hidden\">Late Enrollment</span></i>";
                break;
            case 7:
                designations += "<i class=\"icon-partial-transcript\"><span class=\"hidden\">Partial Transcript</span></i>";
                break;
            case 8:
                designations += "<i class=\"icon-test-ccommodation\"><span class=\"hidden\">Test Accommodation</span></i>";
                break;
            case 9:
                designations += "<i class=\"icon-504-designation\"><span class=\"hidden\">504 Designation</span></i>";
                break;
            default:
                break;
        }
    }

    if (designations.length == 0) {
        return "<div style='width:23px;'></div>";
    } else {
        return designations;
    }
}

function listArray(a) {
    var i, result = "";
    for (i = 0; i < a.length; i++) {
        if (result.length > 0) {
            result += "-";
        }
        result += a[i];
    }
    return result;
}