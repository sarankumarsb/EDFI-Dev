/// <reference path="../External/jquery.js" />
/// <reference path="analytics.js" />

var priorYear = function () {
    "use strict";

    var priorYearJStorage = "priorYear",
        showPriorYearJStorage = "showPriorYear",
        hidePriorYearJStorage = "hidePriorYear",
        showingPriorYearText = "HIDE PRIOR YEAR",
        hidingPriorYearText = "SHOW PRIOR YEAR",
        priorYearDisplayOptionClassSelector = ".PriorYearDisplayOption",
        priorYearRowClassSelector = ".PriorYearRow",
        priorYearSetting;

    this.initialize = function () {
        if ($(priorYearRowClassSelector).size() === 0) {
            $(priorYearDisplayOptionClassSelector).text("");
            $(priorYearDisplayOptionClassSelector).hide();
            return;
        }

        $(priorYearDisplayOptionClassSelector).show();
        priorYearSetting = $.jStorage.get(priorYearJStorage);
        if (priorYearSetting === hidePriorYearJStorage) {
            this.hide();
        } else {
            this.show();
        }
    };

    this.toggle = function () {
        if (priorYearSetting === hidePriorYearJStorage) {
            analyticsManager.trackPriorYear('show');
            this.show();
        } else {
            analyticsManager.trackPriorYear('hide');
            this.hide();
        }
    };

    this.show = function () {
        $(priorYearDisplayOptionClassSelector).text(showingPriorYearText);
        $(priorYearRowClassSelector).show();
        priorYearSetting = showPriorYearJStorage;
        $.jStorage.set(priorYearJStorage, priorYearSetting);
    };

    this.hide = function () {
        $(priorYearDisplayOptionClassSelector).text(hidingPriorYearText);
        $(priorYearRowClassSelector).hide();
        priorYearSetting = hidePriorYearJStorage;
        $.jStorage.set(priorYearJStorage, priorYearSetting);
    };
};

var priorYearDisplay = new priorYear();

$(document).ready(function () {
    priorYearDisplay.initialize();
});