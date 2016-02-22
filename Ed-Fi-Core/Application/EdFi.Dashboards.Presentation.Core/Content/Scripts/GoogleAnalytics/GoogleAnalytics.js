/// <reference path="../Application/Analytics.js" />


function GoogleAnalyticsTrackDrilldown(drilldownName, metricVariantId, additionalId) {
    if (typeof (_gaq) != 'undefined') {
        _gaq.push(['_trackEvent', 'DataView', pageArea.Area, additionalId]);
        _gaq.push(['_trackEvent', 'Drilldown', drilldownName, metricVariantId]);
    }
}

function GoogleAnalyticsTrackDataView(id) {
    if (typeof (_gaq) != 'undefined') {
        _gaq.push(['_trackEvent', 'DataView', pageArea.Area, id]);
    }
}

function GoogleAnalyticsTrackExport(exportType, metricVariantId) {
    if (typeof (_gaq) != 'undefined') {
        _gaq.push(['_trackEvent', 'Export', exportType, metricVariantId.toString()]);
    }
}

function GoogleAnalyticsTrackPrint() {
    if (typeof (_gaq) != 'undefined') {
        _gaq.push(['_trackEvent', 'Print', pageArea.Area, pageArea.Page]);
    }
}

function GoogleAnalyticsTrackCustomStudentList(action, gridIdentifier, additionalData) {
    if (typeof (_gaq) != 'undefined') {
        _gaq.push(['_trackEvent', 'CustomStudentList', action, gridIdentifier]);
    }
}

function GoogleAnalyticsTrackHistoricalChart(action, chartIdentifier, additionalData) {
    if (typeof (_gaq) != 'undefined') {
        _gaq.push(['_trackEvent', 'HistoricalChart', action, chartIdentifier.toString()]);
    }
}

function GoogleAnalyticsTrackEdFiGrid(action, gridIdentifier, additionalData) {
    if (typeof (_gaq) != 'undefined') {
        _gaq.push(['_trackEvent', 'EdFigrid', action, gridIdentifier]);
    }
}

function GoogleAnalyticsTrackSearch(action, terms) {
    if (typeof (_gaq) != 'undefined') {
        _gaq.push(['_trackEvent', 'Search', action, terms]);
    }
}

function GoogleAnalyticsTrackGoalPlanning(action, additionalData) {
    if (typeof (_gaq) != 'undefined') {
        _gaq.push(['_trackEvent', 'GoalPlanning', action]);
    }
}

function GoogleAnalyticsTrackPriorYear(action) {
    if (typeof (_gaq) != 'undefined') {
        _gaq.push(['_trackEvent', 'PriorYear', action]);
    }
}

var _gaq = _gaq || [];
function GoogleAnalyticsTrackUsage(analyticsId, userTracking, localEducationAgency, impersonation, trackingUrl) {
    _gaq.push(['_setAccount', analyticsId]);
    if (userTracking != null && userTracking != '')
        _gaq.push(['_setCustomVar', 1, 'UT', userTracking, 2]);
    if (localEducationAgency != null && localEducationAgency != '')
        _gaq.push(['_setCustomVar', 2, 'D', localEducationAgency, 2]);

    if (trackingUrl != null && trackingUrl != '') {
        if (typeof (additionalParameters) != 'undefined') {
            trackingUrl = trackingUrl + additionalParameters;
        }
        _gaq.push(['_trackPageview', trackingUrl]);
    } else {
        _gaq.push(['_trackPageview']);
    }
    _gaq.push(['_trackPageLoadTime']);

    (function() {
        var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
        ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
        var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
    })();

    if (typeof (analyticsManager) != 'undefined') {
        analyticsManager.addDrilldownTracking(GoogleAnalyticsTrackDrilldown);
        analyticsManager.addDataViewTracking(GoogleAnalyticsTrackDataView);
        analyticsManager.addExportTracking(GoogleAnalyticsTrackExport);
        analyticsManager.addPrintTracking(GoogleAnalyticsTrackPrint);
        analyticsManager.addCustomStudentListTracking(GoogleAnalyticsTrackCustomStudentList);
        analyticsManager.addEdFiGridTracking(GoogleAnalyticsTrackEdFiGrid);
        analyticsManager.addSearchTracking(GoogleAnalyticsTrackSearch);
        analyticsManager.addGoalPlanningTracking(GoogleAnalyticsTrackGoalPlanning);
        analyticsManager.addPriorYearTracking(GoogleAnalyticsTrackPriorYear);
        analyticsManager.addHistoricalChartTracking(GoogleAnalyticsTrackHistoricalChart);
    }
}