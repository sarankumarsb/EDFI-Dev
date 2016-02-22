var analytics = function () {
    "use strict";
    var drilldownTracking = new Array();
    var dataViewTracking = new Array();
    var exportTracking = new Array();
    var printTracking = new Array();
    var customStudentListTracking = new Array();
    var edFiGridTracking = new Array();
    var searchTracking = new Array();
    var goalPlanningTracking = new Array();
    var priorYearTracking = new Array();
    var historicalChartTracking = new Array();

    this.addDrilldownTracking = function (drilldownTrackingFunction) {
        drilldownTracking.push(drilldownTrackingFunction);
    };

    this.addDataViewTracking = function (dataViewTrackingFunction) {
        dataViewTracking.push(dataViewTrackingFunction);
    };

    this.addExportTracking = function (exportTrackingFunction) {
        exportTracking.push(exportTrackingFunction);
    };

    this.addPrintTracking = function (printTrackingFunction) {
        printTracking.push(printTrackingFunction);
    };

    this.addCustomStudentListTracking = function (customStudentListTrackingFunction) {
        customStudentListTracking.push(customStudentListTrackingFunction);
    };

    this.addEdFiGridTracking = function (edFiGridTrackingFunction) {
        edFiGridTracking.push(edFiGridTrackingFunction);
    };

    this.addSearchTracking = function (searchTrackingFunction) {
        searchTracking.push(searchTrackingFunction);
    };

    this.addGoalPlanningTracking = function (goalPlanningTrackingFunction) {
        goalPlanningTracking.push(goalPlanningTrackingFunction);
    };

    this.addPriorYearTracking = function (priorYearTrackingFunction) {
        priorYearTracking.push(priorYearTrackingFunction);
    };

    this.addHistoricalChartTracking = function (historicalChartTrackingFunction) {
        historicalChartTracking.push(historicalChartTrackingFunction);
    };

    this.trackDrilldown = function (drilldownName, metricVariantId, additionalId) {
        for (var i = 0; i < drilldownTracking.length; i++) {
            drilldownTracking[i](drilldownName, metricVariantId, additionalId);
        }
    };

    this.trackDataView = function (id) {
        for (var i = 0; i < dataViewTracking.length; i++) {
            dataViewTracking[i](id);
        }
    };

    this.trackExport = function (exportType, metricVariantId) {
        for (var i = 0; i < exportTracking.length; i++) {
            exportTracking[i](exportType, metricVariantId);
        }
    };

    this.trackPrint = function () {
        for (var i = 0; i < printTracking.length; i++) {
            printTracking[i]();
        }
    };

    this.trackCustomStudentList = function (action, gridIdentifier, additionalData) {
        for (var i = 0; i < customStudentListTracking.length; i++) {
            customStudentListTracking[i](action, gridIdentifier, additionalData);
        }
    };

    this.trackEdFiGrid = function (action, gridIdentifier, additionalData) {
        for (var i = 0; i < edFiGridTracking.length; i++) {
            edFiGridTracking[i](action, gridIdentifier, additionalData);
        }
    };

    this.trackSearch = function (action, terms) {
        for (var i = 0; i < searchTracking.length; i++) {
            searchTracking[i](action, terms);
        }
    };

    this.trackGoalPlanning = function (action, additionalData) {
        for (var i = 0; i < goalPlanningTracking.length; i++) {
            goalPlanningTracking[i](action, additionalData);
        }
    };

    this.trackPriorYear = function (action) {
        for (var i = 0; i < priorYearTracking.length; i++) {
            priorYearTracking[i](action);
        }
    };

    this.trackHistoricalChart = function (action, historicalChartIdentifier, additionalData) {
        for (var i = 0; i < historicalChartTracking.length; i++) {
            historicalChartTracking[i](action, historicalChartIdentifier, additionalData);
        }
    };
};

var analyticsManager = new analytics();
$(window).on("beforeprint", analyticsManager.trackPrint);