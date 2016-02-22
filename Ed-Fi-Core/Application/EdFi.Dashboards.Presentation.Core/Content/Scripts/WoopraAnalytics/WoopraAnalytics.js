/// <reference path="../Application/Analytics.js" />

var woopraTrackingDomain = '';
var woopraVisitorProperties = null;
var woopraPageViewProperties = null;

function prepareWoopraData(basicData, extraUsageStudyData) {

    if (woopraPageViewProperties !== null) {
        $.extend(basicData, woopraPageViewProperties);
    }

    if (typeof (pageArea) != 'undefined') {
        $.extend(basicData, pageArea);

        if (typeof (pageArea.Area) == 'undefined' || typeof (pageArea.Page) == 'undefined') {
            basicData.UsageStudy = '';
        } else {
            basicData.UsageStudy = basicData.UsageStudy + pageArea.Area + '|' + pageArea.Page + '|' + extraUsageStudyData;
        }
    } else {
        basicData.UsageStudy = '';
    }
    return basicData;
}

function WoopraTrackDrilldown(drilldownName, metricVariantId, additionalId) {
    if (typeof(woopraTracker) == 'undefined') {
        return;
    }

    var basicEventData = {
        name: 'drilldown',
        userAction: drilldownName,
        metricVariantId: metricVariantId
    };

    var trackingData = prepareWoopraData(basicEventData, basicEventData.name + '|' + drilldownName + '|' + metricVariantId);
    woopraTracker.pushEvent(trackingData);
}

function WoopraTrackExport(exportType, metricVariantId) {
    if (typeof (woopraTracker) == 'undefined') {
        return;
    }

    var basicEventData = {
        name: 'export',
        userAction: exportType,
        metricVariantId: metricVariantId
    };

    var trackingData = prepareWoopraData(basicEventData, basicEventData.name + '|' + exportType + '|' + metricVariantId);
    woopraTracker.pushEvent(trackingData);
}

function WoopraTrackPrint() {
    if (typeof (woopraTracker) == 'undefined') {
        return;
    }

    var basicEventData = {
        name: 'print'
    };

    var trackingData = prepareWoopraData(basicEventData, basicEventData.name);
    woopraTracker.pushEvent(trackingData);
}

function WoopraTrackCustomStudentList(action, gridIdentifier, additionalData) {
    if (typeof (woopraTracker) == 'undefined') {
        return;
    }

    var basicEventData = {
        name: 'customstudentlist',
        userAction: action,
        gridid: gridIdentifier
    };

    if (additionalData !== null) {
        $.extend(basicEventData, additionalData);
    }

    var trackingData = prepareWoopraData(basicEventData, basicEventData.name + '|' + action + '|' + gridIdentifier);
    woopraTracker.pushEvent(trackingData);
}

function WoopraTrackEdFiGrid(action, gridIdentifier, additionalData) {
    if (typeof (woopraTracker) == 'undefined') {
        return;
    }

    var basicEventData = {
        name: 'edfigrid',
        userAction: action,
        gridid: gridIdentifier
    };

    if (additionalData !== null) {
        $.extend(basicEventData, additionalData);
    }

    var trackingData = prepareWoopraData(basicEventData, basicEventData.name + '|' + action + '|' + gridIdentifier);
    woopraTracker.pushEvent(trackingData);
}

function WoopraTrackHistoricalChart(action, historicalChartIdentifier, additionalData) {
    if (typeof (woopraTracker) == 'undefined') {
        return;
    }

    var basicEventData = {
        name: 'historicalchart',
        userAction: action,
        chartid: historicalChartIdentifier
    };

    if (additionalData !== null) {
        $.extend(basicEventData, additionalData);
    }

    var trackingData = prepareWoopraData(basicEventData, basicEventData.name + '|' + action + '|' + historicalChartIdentifier);
    woopraTracker.pushEvent(trackingData);
}


function WoopraTrackSearch(action, terms) {
    if (typeof (woopraTracker) == 'undefined') {
        return;
    }

    var basicEventData = {
        name: 'search',
        userAction: action,
        terms: terms
    };

    var trackingData = prepareWoopraData(basicEventData, basicEventData.name + '|' + action + '|' + terms);
    woopraTracker.pushEvent(trackingData);
}

function WoopraTrackGoalPlanning(action, additionalData) {
    if (typeof (woopraTracker) == 'undefined') {
        return;
    }

    var basicEventData = {
        name: 'goalplanning',
        userAction: action
    };

    if (additionalData !== null) {
        $.extend(basicEventData, additionalData);
    }

    var trackingData = prepareWoopraData(basicEventData, basicEventData.name + '|' + action);
    woopraTracker.pushEvent(trackingData);
}

function WoopraTrackPriorYear(action) {
    if (typeof (woopraTracker) == 'undefined') {
        return;
    }

    var basicEventData = {
        name: 'prioryear',
        userAction: action
    };

    var trackingData = prepareWoopraData(basicEventData, basicEventData.name + '|' + action);
    woopraTracker.pushEvent(trackingData);
}

function woopraReady(tracker) {
    tracker.setDomain(woopraTrackingDomain);
    tracker.setIdleTimeout(1800000);
    
    if (woopraVisitorProperties !== null && $.isArray(woopraVisitorProperties)) {
        for (var i = 0; i < woopraVisitorProperties.length; i++) {
            var vp = woopraVisitorProperties[i];
            tracker.addVisitorProperty(vp[0], vp[1]);
        }
    }

    var basicPageViewData = {
        type: 'pageview',
        title: document.title
    };
    
    var trackingData = prepareWoopraData(basicPageViewData, '');
    tracker.trackPageview(trackingData);
    return false;
}

function TrackWoopraUsage(domain, visitorProperties, pageViewProperties) {
    woopraTrackingDomain = domain;

    if (visitorProperties !== null) {
        woopraVisitorProperties = visitorProperties;
    }
    
    if (pageViewProperties !== null) {
        woopraPageViewProperties = pageViewProperties;
    }

    (function() {
        var wsc = document.createElement('script');
        wsc.src = document.location.protocol + '//static.woopra.com/js/woopra.js';
        wsc.type = 'text/javascript';
        wsc.async = true;
        var ssc = document.getElementsByTagName('script')[0];
        ssc.parentNode.insertBefore(wsc, ssc);
    })();
    
    if (typeof (analyticsManager) != 'undefined') {
        analyticsManager.addDrilldownTracking(WoopraTrackDrilldown);
        analyticsManager.addExportTracking(WoopraTrackExport);
        analyticsManager.addPrintTracking(WoopraTrackPrint);
        analyticsManager.addCustomStudentListTracking(WoopraTrackCustomStudentList);
        analyticsManager.addEdFiGridTracking(WoopraTrackEdFiGrid);
        analyticsManager.addSearchTracking(WoopraTrackSearch);
        analyticsManager.addGoalPlanningTracking(WoopraTrackGoalPlanning);
        analyticsManager.addPriorYearTracking(WoopraTrackPriorYear);
        analyticsManager.addHistoricalChartTracking(WoopraTrackHistoricalChart);
    }
}

