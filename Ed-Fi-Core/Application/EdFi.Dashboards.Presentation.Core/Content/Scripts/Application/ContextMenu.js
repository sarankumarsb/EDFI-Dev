/// <reference path="../External/jquery.js" />
/// <reference path="../External/jquery-ui.min.js"/>
/// <reference path="Main.js" />

// safeToggle: fallback to regular toggle for IE7
(function ($) { $.fn.safeToggle = function (speed) { if ($.browser.msie && parseInt($.browser.version, 10) === 7) { this.toggle(); } else { this.slideToggle(speed); }};})(jQuery);

function openMoreMenu(event) {

	if (event.type === "focus") {
	    $('.contextMenu-DropDown:visible').hide("slide", { direction: "up" }, 100);
	    $('.contextMenu-ButtonSelected').removeClass('contextMenu-ButtonSelected');
    }

    if (event.type === "click"
        || (event.type === "keypress" && (event.which === 13/*enter*/ || event.which === 32/*space*/)) 
        ) {
        event.preventDefault(); // keep <a> tags with href="#" from causing the screen to scroll

        var moreMenuButton = $(this);
        var isSelected = moreMenuButton.hasClass('contextMenu-ButtonSelected');

        $('.contextMenu-DropDown:visible').hide("slide", { direction: "up" }, 100);
        $('.contextMenu-ButtonSelected').removeClass('contextMenu-ButtonSelected'); 

        if (!isSelected) {
            moreMenuButton.addClass('contextMenu-ButtonSelected');
            moreMenuButton.next(".contextMenu-DropDown").show("slide", { direction: "up" }, 100);
        }
    }
}

function closeMoreMenu() {
    $('.contextMenu-DropDown:visible').hide("slide", { direction: "up" }, 100);
    $('.contextMenu-ButtonSelected').removeClass('contextMenu-ButtonSelected');
}

function ToggleDrillDown(drillDownId) {
    $(drillDownId).safeToggle('fast');
}

function showDynamicContent(divId, htmlPath, moreImageId, tdid, parentMetricVariantId) {
    if (htmlPath != '') {//if there is a path to display then.
        if ($(divId).is(':hidden')) { //if its hidden then we load content
            //Lets Check if we already loaded data in this div.
            //If we have just slide it open.
            //If not then load the external html file in the div.
            var metricDrillDown = getMetricDrillDown(divId);
            var contentDivId = '#DrillDownDiv' + metricDrillDown;
            var contentHtml = $(contentDivId).html();
            var dynamicCloseLink = "#dynamicCloseLink-" + tdid;
            
            if (contentHtml.length < 30) {
                //Show a loading Message.
                $(divId).safeToggle('fast'); 
                var resolvedPath = htmlPath.replace("~/", $.global.siteBasePath + "/");
                $(contentDivId).load(resolvedPath, function (response, status, xhr) {
                    if (status == "error") {
                        NavigateToPage($.global.errorPath);
                    }
                    else if (status == "success") {
                        //if session expired and a drilldown was clicked then send to login.
                        if (response.indexOf("login-page") > -1) {
                            var url = buildUrl(tdid, "m" + parentMetricVariantId);

                            NavigateToPage(url);
                            return;
                        }

                        $(contentDivId).safeToggle('slow');
                        //Hide the loading Div...
                        $('#LoadingDiv' + metricDrillDown).safeToggle('slow');
                    } else {
                        alert(status);
                    }
                });
            }
            else {
                $(divId).safeToggle('slow');
            }

            //Finally do other things.

            //Set the focus of the calling more button.
            $(dynamicCloseLink).focus();


        } else {//If its not hidden only close.
            $(divId).safeToggle('slow');
        }
    }
}

function buildUrl(tdid, jump) {
    var parameters = getUrlParameters();
    parameters["tdId"] = tdid;
    parameters["jump"] = jump;

    var p = $.param(parameters, false);
    if (window.location.href.indexOf('?') != -1)
        return window.location.href.substring(0, window.location.href.indexOf('?') + 1) + p;
    return window.location.href + "?" + p;
}

function getMetricDrillDown(divId) {
    return divId.substr(divId.indexOf('Div') + 3);
}