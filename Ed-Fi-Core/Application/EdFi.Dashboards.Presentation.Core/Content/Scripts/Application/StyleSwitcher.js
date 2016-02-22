/// <reference path="../External/jquery.js" />
/// <reference path="../External/jStorage.js" />

function setActiveStyleSheet(title) {
    var i, a;
    for (i = 0; (a = document.getElementsByTagName("link")[i]); i++) {
        if (a.getAttribute("rel").indexOf("style") != -1 && a.getAttribute("title")) {
            a.disabled = true;
            if (a.getAttribute("title") == title) 
                a.disabled = false;
        }
    }
}

function getActiveStyleSheet() {
    var i, a;
    for (i = 0; (a = document.getElementsByTagName("link")[i]); i++) {
        if (a.getAttribute("rel").indexOf("style") != -1 && a.getAttribute("title") && !a.disabled) 
            return a.getAttribute("title");
    }
    return null;
}

var saveStyleSheet = false;
document.onready = function () {
    var styleSheetTitle = $.jStorage.get("style");
    if (styleSheetTitle)
        setActiveStyleSheet(styleSheetTitle);
    saveStyleSheet = true;
};

window.onunload = function () {
    if (saveStyleSheet) {
        var title = getActiveStyleSheet();
        $.jStorage.set("style", title); //persisted for a year.
    }
};
