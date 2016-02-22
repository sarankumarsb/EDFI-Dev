/// <reference path="../External/jquery.js" />
/// <reference path="../External/json2.js" />

//Used for the new templated JSON Optimized Lists
function createEntityListCookieWithExtraParametersForTemplatedList(dataId, rowIndexForId, metricId, listType, webServicePath) {
    var data = jQuery[dataId];
    var dataRows = data.Rows;

    var entityIds = [];
    var paramNames = [];
    if (listType == "StudentList") {
        paramNames = ["studentUSI", "schoolId"];
        if (data.EntityIds.length > 0) {
            $.each(data.EntityIds, function (index, value) {
                entityIds.push([value, data.SchoolId]);
            }); 
        } else {
            $.each(dataRows, function (count, row) {
                if (row[rowIndexForId].CId != null)
                    entityIds.push([row[rowIndexForId].StudentUSI, row[rowIndexForId].CId]);
            });  
        }
    } else if (listType == "TeacherList") {
        paramNames = ["staffUSI", "schoolId"];
        $.each(dataRows, function (count, row) {
            if (row[rowIndexForId].Url != "") {
                entityIds.push([row[rowIndexForId].TId, row[rowIndexForId].CId]);
            }
        });
    } else if (listType == "SchoolList" || listType == "GoalPlanningSchoolList") {
        paramNames = ["schoolId"];
        $.each(dataRows, function (count, row) {
            entityIds.push([row[rowIndexForId].CId]);
        });
    } else {
        alert("Error: I don't know how to calculate entityIds for (" + listType + ") type of list");
    }
    //Get the Unique Id for this list context
    var persistenceUniqueId = getCookieUniqueKeyForPreviousNextControl(metricId);
    
    //Build Ajax Object    
    var jsonStudentListObject = {
        ListUrl: window.location.toString(),
        ListType: listType,
        ListPersistenceUniqueId: persistenceUniqueId,
        TableId: dataId,
        EntityIdArray: entityIds,
        ParameterNames: paramNames,
        MetricId: metricId,
        FromSearch: false
    };

    //We have our data now save to a persistent structure.
    var jsonStringRepresentation = JSON.stringify(jsonStudentListObject);
    //alert(jsonStringRepresentation);
    listControlSaveToPersistentMedium(jsonStringRepresentation, webServicePath);
}

function createEntityListCookieWithExtraParametersForSearch(table, rowIndexForStudentUSI, metricId, listType, webServicePath) {
    var tableId = $(table).attr('id');
    var data = eval('searchResultsData');
    
    var entityIds = [];
    var paramNames = [];

    if (listType == "Students") {
        paramNames = ["studentUSI", "schoolId"];
        $.each(data.Students, function (count, row) {
            entityIds.push([row.StudentUSI, row.SchoolId]);
        });
    }
    else if (listType == "Schools") {
        paramNames = ["schoolId"];
        $.each(data.Schools, function (count, row) {
            entityIds.push([row.SchoolId]);
        });
    }
    else if (listType == "Teachers") {
        paramNames = ["staffUSI", "schoolId"];
        
        $.each(data.Teachers, function (count, row) {
            entityIds.push([row.StaffUSI, row.SchoolId]);
            
        });
    }
    else {
        alert("Error: I don't know how to calculate entityIds for (" + listType + ") type of list");
    }

    //Get the Unique Id for this list context
    var persistenceUniqueId = getCookieUniqueKeyForPreviousNextControl(metricId);

    //Build Ajax Object    
    var jsonStudentListObject = {
        ListUrl: window.location.toString(),
        ListType: listType,
        ListPersistenceUniqueId: persistenceUniqueId,
        TableId: tableId,
        EntityIdArray: entityIds,
        ParameterNames: paramNames,
        MetricId: metricId,
        FromSearch: true
    };

    //We have our data now save to a persistent structure.
    var jsonStringRepresentation = JSON.stringify(jsonStudentListObject);
    //alert(jsonStringRepresentation);
    listControlSaveToPersistentMedium(jsonStringRepresentation, webServicePath);
}

function listControlSaveToPersistentMedium(json, webServicePath) {
    if (json === null || json === "")
        return;
    
    var jsonObject = {
        json: json
    };

    var jsonString = JSON.stringify(jsonObject);

    $.ajax({
        type: "POST",
        url: webServicePath,
        data: jsonString,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function(msg) {  },
        error: function(result) {
            //alert("AJAX call to set the grids sorted list to session failed: " + result.status + " " + result.statusText); 
        }
    }); 
}

//Functions for getting the UniqueId for the cookie.
function getCookieUniqueKeyForPreviousNextControl(metricId) {
    var page = window.location.pathname.substring(window.location.pathname.lastIndexOf('/') + 1).replace('.', '');
    
    if(metricId!=null)
        page += metricId;
    
    return page;
}
