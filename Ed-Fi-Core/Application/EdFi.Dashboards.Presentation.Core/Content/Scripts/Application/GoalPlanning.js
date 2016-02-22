/// <reference path="../External/jquery.js" />
/// <reference path="../External/json2.js" />
/// <reference path="analytics.js" />

var goalPlanning = function (edOrgId, goalDataId, postUrl, publishUrl, postSchoolListUrl) {
    "use strict";
    var _educationOrganizationId = edOrgId;
    var _goalDataIdentifier = goalDataId;
    var _postUrl = postUrl;
    var _publishUrl = publishUrl;
    var _postSchoolListUrl = postSchoolListUrl;
    var _metricIds = [];
    var changedGoalClass = "changedGoal";
    var pendingGoalClass = "pendingGoal";
    var currentGoalAttr = "data-currentGoal";
    var metricIdAttr = "data-metricId";
    var educationOrganizationIdAttr = "educationOrganiziationId";
    var add = "add";
    var set = "set";
    var remove = "remove";

    this.initialize = function (changeMoreActionLinks) {
        if (changeMoreActionLinks) {
            $('a[data-metricaction|="moreActions"]').each(function () {
                var onClick = this.attributes["onclick"];
                onClick = onClick.value.replace(/SchoolMetricTable/i, "GoalPlanningSchoolMetricTable");
                var click = new Function(onClick);
                this.onclick = click;
            });
        }

        var holdThis = this;
        $('input.newGoal').change(holdThis.saveGoal);

        setChangedGoalStyle();

        $('input.newGoal').each(function () {
            var goal = $(this);
            _metricIds.push(Number(goal.attr(metricIdAttr)));
        });
    };

    function setChangedGoalStyle() {
        var i, currentGoal, currentGoalDifference, newGoal, goalData = jQuery[_goalDataIdentifier];
        for (i = 0; i < goalData.PublishedGoals.length; i++) {
            if (goalData.PublishedGoals[i].EducationOrganizationId === _educationOrganizationId) {
                currentGoal = $("#vGoal" + goalData.PublishedGoals[i].MetricId);
                if (currentGoal !== undefined) {
                    currentGoal.text(goalData.PublishedGoals[i].DisplayGoal);
                    currentGoal.addClass(pendingGoalClass);
                }
                currentGoalDifference = $("#vDifference" + goalData.PublishedGoals[i].MetricId);
                var statusState = "status-goal-met";
                if (currentGoalDifference !== undefined) {
                    currentGoalDifference.text(goalData.PublishedGoals[i].DisplayGoalDifference);
                    currentGoalDifference.removeClass("goal-not-met");
                    if (goalData.PublishedGoals[i].GoalDifference < 0) {
                        currentGoalDifference.addClass("goal-not-met");
                        statusState = "status-goal-not-met";
                    }
                }

                //Updating the status label class to make it change accordingly.
                var statusItem = $("#vStatus" + goalData.PublishedGoals[i].MetricId).find(".status-goal-not-met, .status-goal-met");
                statusItem.removeClass("status-goal-not-met");
                statusItem.removeClass("status-goal-met");
                statusItem.addClass(statusState);

                newGoal = $("#newGoal" + goalData.PublishedGoals[i].MetricId);
                if (newGoal !== undefined) {
                    newGoal.val(goalData.PublishedGoals[i].Goal);
                    newGoal.attr(currentGoalAttr, goalData.PublishedGoals[i].Goal);
                }
            }
        }

        $("input.newGoal").each(function () {
            var goal = $(this);
            if (goal.hasClass(changedGoalClass)) {
                goal.val(Number(goal.attr(currentGoalAttr)));
                goal.removeClass(changedGoalClass);
            }
        });

        for (i = 0; i < goalData.ProposedGoals.length; i++) {
            if (goalData.ProposedGoals[i].EducationOrganizationId === _educationOrganizationId) {
                newGoal = $("#newGoal" + goalData.ProposedGoals[i].MetricId);
                if (newGoal !== undefined) {
                    newGoal.val(goalData.ProposedGoals[i].Goal);
                    newGoal.addClass(changedGoalClass);
                }
            }
        }
    }

    this.applyGoalData = function (childGoalDataId, childMetricDataId) {
        var i, j, schoolId, publishedGoals, allTheSame, goalData = jQuery[childGoalDataId], metricData = jQuery[childMetricDataId], returnValue = false;

        for (i = 0; i < metricData.Rows.length; i++) {
            schoolId = metricData.Rows[i][1].CId;
            publishedGoals = [];
            for (j = 0; j < goalData.PublishedGoals.length; j++) {
                if (schoolId === goalData.PublishedGoals[j].EducationOrganizationId && jQuery.inArray(goalData.PublishedGoals[j].MetricId, metricData.Rows[i][7].MIds) > -1) {
                    publishedGoals.push(goalData.PublishedGoals[j]);
                }
            }
            if (publishedGoals.length === 0) {
                continue;
            }

            if (publishedGoals.length === 1) {
                if (metricData.Rows[i][5].V === publishedGoals[0].Goal) {
                    continue;
                }
                returnValue = true;
                applyPublishedGoal(metricData.Rows[i], publishedGoals[0].Goal, publishedGoals[0].DisplayGoal);
                continue;
            }

            // there is more than one school metric that could be used for this list
            allTheSame = true;
            for (j = 0; j < publishedGoals.length - 1; j++) {
                if (publishedGoals[j].Goal !== publishedGoals[j + 1].Goal) {
                    allTheSame = false;
                    break;
                }
            }

            if (allTheSame) {
                applyPublishedGoal(metricData.Rows[i], publishedGoals[0].Goal, publishedGoals[0].DisplayGoal);
            } else {
                metricData.Rows[i][5].DV = "See school";
                metricData.Rows[i][6].DV = "See school";
                metricData.Rows[i][6].STe = 1;
            }
            returnValue = true;
        }
        return returnValue;
    };

    function applyPublishedGoal(row, goal, displayGoal) {
        var newGoalDifference;
        row[5].DV = displayGoal;
        row[5].V = goal;
        row[7].G = goal;

        if (row[7].I === true) {
            newGoalDifference = row[4].V - goal;
        } else {
            newGoalDifference = goal - row[4].V;
        }
        row[6].V = newGoalDifference;

        newGoalDifference = newGoalDifference * 100;
        if (newGoalDifference > 0) {
            row[6].STe = 1;
        } else {
            row[6].STe = 3;
        }

        row[6].DV = newGoalDifference.toFixed(1) + " %";
    }

    this.initializeChildMetrics = function (childMetricTableIdentifier, childMetricDataId, schoolMetricDataId, childGoalDataId, childMetricGrid) {
        var childGoals = $(childMetricTableIdentifier + " input.childNewGoal");

        var holdThis = this;
        childGoals.change(function () { holdThis.saveChildGoal($(this), childMetricTableIdentifier, childMetricDataId, schoolMetricDataId, childGoalDataId, childMetricGrid); });

        setChangedChildGoalStyle(childMetricTableIdentifier, childGoalDataId);
    };

    function setChangedChildGoalStyle(childMetricTableIdentifier, childGoalDataId) {
        var i, newGoal, theSame, mixedSettings, metricIds, childEducationOrganizationId, data, goalData = jQuery[childGoalDataId];
        $(childMetricTableIdentifier + " input.childNewGoal").each(function () {
            newGoal = $(this);
            mixedSettings = $('#mixedSettings-' + newGoal.attr(metricIdAttr) + '-' + newGoal.attr(educationOrganizationIdAttr));
            metricIds = [];
            $.each(newGoal.attr(metricIdAttr).split("-"), function () { metricIds.push(Number(this)); });
            childEducationOrganizationId = Number(newGoal.attr(educationOrganizationIdAttr));
            data = [];
            if (metricIds.length > 1) {
                for (i = 0; i < goalData.PublishedGoals.length; i++) {
                    if (goalData.PublishedGoals[i].EducationOrganizationId == childEducationOrganizationId && jQuery.inArray(goalData.PublishedGoals[i].MetricId, metricIds) > -1) {
                        data.push(goalData.PublishedGoals[i]);
                    }
                }
                theSame = true;
                for (i = 0; i < data.length - 1; i++) {
                    if (data[i].Goal !== data[i + 1].Goal) {
                        theSame = false;
                        break;
                    }
                }
                if (!theSame) {
                    newGoal.hide();
                    mixedSettings.show();
                    return;
                }
                data = [];
            }

            for (i = 0; i < goalData.ProposedGoals.length; i++) {
                if (goalData.ProposedGoals[i].EducationOrganizationId == childEducationOrganizationId && jQuery.inArray(goalData.ProposedGoals[i].MetricId, metricIds) > -1) {
                    data.push(goalData.ProposedGoals[i]);
                }
            }

            if (data.length === 0) {
                newGoal.show();
                newGoal.val(Number(newGoal.attr(currentGoalAttr)));
                newGoal.removeClass(changedGoalClass);
                mixedSettings.hide();
                return;
            }

            if (data.length === 1 && metricIds.length === 1) {
                newGoal.show();
                newGoal.val(data[0].Goal);
                newGoal.addClass(changedGoalClass);
                mixedSettings.hide();
                return;
            }

            if (data.length === metricIds.length) {
                theSame = true;
                for (i = 0; i < data.length - 1; i++) {
                    if (data[i].Goal !== data[i + 1].Goal) {
                        theSame = false;
                        break;
                    }
                }

                if (theSame) {
                    newGoal.show();
                    newGoal.val(data[0].Goal);
                    newGoal.addClass(changedGoalClass);
                    mixedSettings.hide();
                    return;
                }
            }

            newGoal.hide();
            mixedSettings.show();
        });
    }

    this.saveGoal = function () {
        var goalData = jQuery[_goalDataIdentifier];
        var newGoal = $(this);
        var metricId = Number(newGoal.attr(metricIdAttr));
        var result = setGoalDataFromUI(newGoal, _educationOrganizationId, metricId, goalData.ProposedGoals);
        if (result !== null) {
            persistGoalData(new Array(result), _educationOrganizationId, _goalDataIdentifier);
        }
    };

    this.saveChildGoal = function (newGoal, childMetricTableIdentifier, childMetricDataId, schoolMetricDataId, childGoalDataId, childMetricGrid) {
        var schoolMetrics = jQuery[schoolMetricDataId];
        var goalData = jQuery[childGoalDataId];
        var results = setChildGoalData(newGoal, goalData.ProposedGoals);
        var holdThis = this;

        persistSchoolListGoalData(results, schoolMetrics, childGoalDataId, function () {
            analyticsManager.trackGoalPlanning('schoolListEdit', { metricIds: newGoal.attr(metricIdAttr) });
            var madeChanges = holdThis.applyGoalData(childGoalDataId, childMetricDataId);
            if (madeChanges) {
                // need to redraw grid
                childMetricGrid.redrawGrid();
            }
            setChangedChildGoalStyle(childMetricTableIdentifier, childGoalDataId);
        });
    };

    function setChildGoalData(newGoal, goalData) {
        var i, metricIds, metricId, childEducationOrganizationId, results = [], result;
        metricIds = newGoal.attr(metricIdAttr).split("-");
        childEducationOrganizationId = Number(newGoal.attr(educationOrganizationIdAttr));
        for (i = 0; i < metricIds.length; i++) {
            metricId = Number(metricIds[i]);
            result = setGoalDataFromUI(newGoal, childEducationOrganizationId, metricId, goalData);
            if (result !== null) {
                results.push(result);
            }
        }
        return results;
    }

    function setGoalDataFromUI(newGoal, educationOrganizationId, metricId, goalData) {
        var currentGoalValue = Number(newGoal.attr(currentGoalAttr));
        var newGoalValue = Number(newGoal.val());
        if (isNaN(newGoalValue)) {
            var currentGoal = newGoal.attr(currentGoalAttr);
            alert("ERROR: '" + newGoal.val() + "' is not a valid goal setting value.\n\nPlease enter a valid decimal value [Current Goal = " + currentGoal + "]");
            newGoal.val(currentGoal);
            return null;
        }
        var result = setGoalData(newGoalValue, currentGoalValue, educationOrganizationId, metricId, goalData);
        if (result !== null && result.Action !== remove) {
            newGoal.addClass(changedGoalClass);
        } else {
            newGoal.removeClass(changedGoalClass);
        }
        return result;
    }

    function setGoalData(newGoalValue, currentGoalValue, educationOrganizationId, metricId, goalData) {
        var i, goal;

        for (i = 0; i < goalData.length; i++) {
            if (goalData[i].EducationOrganizationId === educationOrganizationId && goalData[i].MetricId === metricId) {
                goal = goalData[i];
                break;
            }
        }
        if (goal === undefined) {
            if (newGoalValue !== currentGoalValue) {
                goal = {
                    EducationOrganizationId: Number(educationOrganizationId),
                    MetricId: metricId,
                    Goal: newGoalValue,
                    Action: add
                };
                return goal;
            }
            return null;
        }

        if (newGoalValue !== currentGoalValue) {
            goal.Goal = newGoalValue;
            goal.Action = set;
            return goal;
        }

        goal.Action = remove;
        return goal;
    }

    function persistGoalData(modifiedGoals, educationOrganizationId, modifiedGoalDataId) {
        if (modifiedGoals.length === 0) {
            return;
        }

        analyticsManager.trackGoalPlanning('goalEdit', null);

        var goalPlanningPostRequest = {
            GoalPlanningActions: modifiedGoals
        };
        var postData = { json: JSON.stringify(goalPlanningPostRequest) };

        $.ajax({
            type: 'POST',
            url: _postUrl,
            data: postData,
            success: function (returnValue) {
                jQuery[modifiedGoalDataId] = returnValue;
                setChangedGoalStyle();
            },
            traditional: true
        });
    }

    function persistSchoolListGoalData(modifiedGoals, schoolMetrics, modifiedGoalDataId, refreshUI) {
        if (modifiedGoals.length === 0) {
            return;
        }

        var goalPlanningPostRequest = {
            SchoolMetrics: schoolMetrics,
            GoalPlanningActions: modifiedGoals
        };
        var postData = { json: JSON.stringify(goalPlanningPostRequest) };

        $.ajax({
            type: 'POST',
            url: _postSchoolListUrl,
            data: postData,
            success: function (returnValue) {
                jQuery[modifiedGoalDataId] = returnValue;
                refreshUI();
            },
            traditional: true
        });
    }

    this.userConfirmedPublish = function (action) {
        return confirm("This action will replace existing goals for all schools.\n\nProceed with " + action + " all goals?");
    };

    this.replicateGoal = function (metricId, childMetricTableIdentifier, gridDataIdentifier, childSchoolMetricDataId, childGoalDataId, childMetricGrid) {
        if (!this.userConfirmedPublish("replicating")) {
            return;
        }

        var i, j, educationOrganizationId, childGoal, currentGoal, metricIds, results = [], result, goalToReplicate = $("input.newGoal[data-metricId='" + metricId + "']");
        var newGoal = goalToReplicate.val();
        var data = jQuery[gridDataIdentifier];
        var dataRows = data.Rows;

        var goalData = jQuery[childGoalDataId];
        for (i = 0; i < dataRows.length; i++) {
            educationOrganizationId = dataRows[i][1].CId;
            childGoal = $(childMetricTableIdentifier + " input.childNewGoal[educationorganiziationid='" + educationOrganizationId + "']");
            if (childGoal.length > 0) {
                childGoal.val(newGoal);
                var resultData = setChildGoalData(childGoal, goalData.ProposedGoals);
                if (resultData !== null && resultData.length > 0) {
                    results = results.concat(resultData);
                }
            } else {
                metricIds = dataRows[i][7].MIds;
                currentGoal = dataRows[i][7].G;
                for (j = 0; j < metricIds.length; j++) {
                    result = setGoalData(Number(newGoal), currentGoal, educationOrganizationId, metricIds[j], goalData.ProposedGoals);
                    if (result !== null) {
                        results.push(result);
                    }
                }
            }
        }

        analyticsManager.trackGoalPlanning('replicateGoal', { metricId: metricId });

        var holdThis = this;
        persistSchoolListGoalData(results, jQuery[childSchoolMetricDataId], childGoalDataId, function () {
            var madeChanges = holdThis.applyGoalData(childGoalDataId, gridDataIdentifier);
            if (madeChanges) {
                // need to redraw grid
                childMetricGrid.redrawGrid();
            }
            setChangedChildGoalStyle(childMetricTableIdentifier, childGoalDataId);
        });
    };

    this.publishAllGoals = function () {
        if (!this.userConfirmedPublish("publishing")) {
            return;
        }

        analyticsManager.trackGoalPlanning('publishAll', null);

        var postData = { PublishAllLocalEducationAgencyGoals: true };
        $.ajax({
            type: 'POST',
            url: _publishUrl,
            data: postData,
            success: function () {
                window.location.href = window.location.href.split('?')[0];
                location.reload(true);
            },
            traditional: true
        });
    };

    this.publishEdOrgGoals = function () {

        analyticsManager.trackGoalPlanning('publish', null);

        var postData = { PublishAllLocalEducationAgencyGoals: false };
        $.ajax({
            type: 'POST',
            url: _publishUrl,
            data: postData,
            success: function () {
                window.location.href = window.location.href.split('?')[0];
                location.reload(true);
            },
            traditional: true
        });
    };
};