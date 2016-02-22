/// <reference path="../External/jquery.js" />

function Main() {
//    alert("Yes I am Here");    
//    alert("a");
}

var warningTimeoutEnabled = false;
var forceLogoutEnabled = false;

var warningTimeoutKey = null;
var sessionRefreshUrl = null;
var sessionRefreshTime = null;
var forceLogoutTime = null;
var forceLogoutTimeoutKey = null;
var forceLogoutEndpoint = null;
var forceLogoutAction = null;

// Start session timeout alert

function setSessionExpirationAlert(warningEnabled, forceEnabled, keepAliveEndpoint, logoutEndpoint, warningMinutes, logoutMinutes, forceAction) {

    warningTimeoutEnabled = warningEnabled;
    sessionRefreshUrl = keepAliveEndpoint;
    sessionRefreshTime = warningMinutes;
    forceLogoutEnabled = forceEnabled;
    forceLogoutTime = logoutMinutes;
    forceLogoutEndpoint = logoutEndpoint;
    forceLogoutAction = forceAction;

    // when $.ajax() calls are made in other places in the system, push back the timeout
    $(document).ajaxSuccess(function () { _startAlertCountdown(); });

    _startAlertCountdown();
}

function _startAlertCountdown() {
    if (warningTimeoutKey !== null) clearTimeout(warningTimeoutKey);
    if (forceLogoutTimeoutKey !== null) clearTimeout(forceLogoutTimeoutKey);

    if(warningTimeoutEnabled) warningTimeoutKey = setTimeout(_sessionExpirationAlert, sessionRefreshTime * 60000);
    if(forceLogoutEnabled) forceLogoutTimeoutKey = setTimeout(_forceLogout, forceLogoutTime * 60000);
}

function _sessionExpirationAlert() {
    $("#dialog-confirm").dialog({
        resizable: false,
        modal: true,
        title: "Your session is about to expire. Click okay to continue browsing.",
        buttons: {
            "Okay": function () {
                $.ajax({
                    url: sessionRefreshUrl,
                    cache: false,
                    success: function () {
                        _startAlertCountdown(); // restart countdown
                    }
                });
                
                $(this).dialog("close");
            }
        }
    });
}

function _forceLogout() {
    switch (forceLogoutAction) {
        case "Redirect":
            window.location = forceLogoutEndpoint;
            break;
        case "CloseWindow":
            window.close();
            break;
        default:
            break;
    }
}

// end session timeout alert

// this code allows us to use console.log even if the debugging console is not open in IE 8 and below
var alertFallback = false;
if (typeof console === "undefined" || typeof console.log === "undefined") {
    console = {};
    if (alertFallback) {
        console.log = function (msg) {
            alert(msg);
        };
    } else {
        console.log = function () { };
    }
}

function highlightMetric() {
    var hash = window.location.hash;
    if (hash) {
        //The hash in this context is in the pattern of #m123 where 123 = the metricVariantId.
        var metricVariantIdFromHash = hash.replace("#m", "");

        var tr = $("#vMetric" + metricVariantIdFromHash);
        if (tr.hasClass("MetricHeaderTitle"))
            tr = tr.next(); 
        
        tr.effect("highlight", {}, 4000);
    }
}

function getUrlParameters() {
    if (window.location.href.indexOf('?') == -1)
        return {};

    var vars = {}, hash;
    var href = window.location.href;
    var poundIndex = href.indexOf('#');
    if (poundIndex != -1)
        href = href.substring(0, poundIndex);
    var hashes = href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars[hash[0]] = hash[1];
    }
    return vars;
}

function requestQuerystring(key) {

    var parameter = getUrlParameters()[key];
    if (parameter !== undefined)
        return parameter;

    return "";
}

function getValueFromKey(key, kvpairs) {

    for (var i = 0; i < kvpairs.length; i++) {
        var kvp = kvpairs[i].split('=');
        if (kvp[0] == key)
            return kvp[1];
    }

    return null;
}

// Begin dropdown navigation
// Updated to handle keyboard navigation scenarios, code modified from post @ http://www.themaninblue.com/writing/perspective/2004/10/19/
// TODO: This is working well with the navigateOnChange plugin, but could be cleaner

function NavigateToPage(url) {
    if (url == '') {
        alert("Please select a page to go to.");
    }
    else {
        window.location.href = url.replace("~/", "../../");
    }
}

(function ($) {
    $.fn.navigateOnChange = function () {
        var select = $(this)[0];

        select.changed = false;
        select.onfocus = selectFocussed;
        select.onchange = selectChanged;
        select.onkeydown = selectKeyed;
        select.onclick = selectClicked;

        return true;
    };
}(jQuery));

function selectChanged(theElement) {
    var theSelect;
    if (this !== window)
        theElement = this;

    if (theElement && theElement.value) {
        theSelect = theElement;
    } else {
        theSelect = this;
    }

    if (!theSelect.changed) {
        return false;
    }

    NavigateToPage($(theElement).val());

    return true;
}


function selectClicked() {
    this.changed = true;
}


function selectFocussed() {
    this.initValue = this.value;

    return true;
}


function selectKeyed(e) {
    var theEvent;
    var keyCodeTab = "9";
    var keyCodeEnter = "13";
    var keyCodeEsc = "27";

    if (e) {
        theEvent = e;
    } else {
        theEvent = event;
    }

    if ((theEvent.keyCode == keyCodeEnter || theEvent.keyCode == keyCodeTab) && this.value != this.initValue) {
        this.changed = true;
        selectChanged(this);
    } else if (theEvent.keyCode == keyCodeEsc) {
        this.value = this.initValue;
    } else {
        this.changed = false;
    }

    return true;
}

// end dropdown navigation

function searchGetHeightForScrollDiv(rowCount) {
    if (rowCount > 14)
        return 'height:400px;';
    return 'min-height:' + String(30 * rowCount) + 'px;';

}

// this method will format a form for printing by hiding certain form items
function formatForPrint(idsToHide) {
    // hide the header
    $('header').hide();
    // hide the footer
    $('footer').hide();

    for (var i = 0; i < idsToHide.length; i++) {
        $(idsToHide[i]).hide();
    }
}

// created contains method so a single line call can be made instead of the
// multi-line indexOf code below
String.prototype.contains = function(value) {
    var index = this.indexOf(value);

    if (index > -1) {
        return true;
    }

    return false;
}