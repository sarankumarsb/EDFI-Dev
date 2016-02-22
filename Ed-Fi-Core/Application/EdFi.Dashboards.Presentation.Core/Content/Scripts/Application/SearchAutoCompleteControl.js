/// <reference path="../External/jquery.js" />
/// <reference path="../External/json2.js" />

//This jQuery was based on http://www.pengoworks.com/workshop/jquery/lib/jquery.autocomplete.js
//JQuery plugin definition...
(function ($) {
    $.fn.SearchAutoCompleteControl = function (options) {

        //The default settings....
        var pluginSettings = {
            delayToStartTheFetch: 400,
            rowCountToReturn: 6,
            stringLengthToUseContains: 2,
            urlForWebService: "Search/QuickSearch",
            urlForSearchResults: "Search/Results",
            cssClassForLoading: "Loading",
            noResultsText: "Your search did not return any results."
            //,onItemSelect: defaultItemSelected
            //,onNavigateToSearchResults: defaultNavigateToSearchResultsPage
            //onLogSearch : defaultLogUserQuickSearch
        };

        //Adding the supplied settings
        if (options) {
            $.extend(pluginSettings, options);
        }

        //Disable default browser autocomplete.
        this.attr("autocomplete", "off");

        //Global variables.
        var input = this;
        var searchResults = $('.l-search-results');
        var activeTdId = "";
        var hasFocus = false;
        var selectedItem = { text: "", id: -1, type: "", schoolId: -1, count: -1, link: "" };
        var timeout = null;
        var data = null;



        //*****Adding jquery events**************************************************************************
        //Adding the keyup event...
        input.keydown(function (e) {
            switch (e.keyCode) {
                case 38: // up
                    e.preventDefault();
                    moveSelect(1);
                    break;
                case 40: // down
                    e.preventDefault();
                    moveSelect(-1);
                    break;
                case 13: // return
                    //If we dont have anything in the input box use default behavior.
                    if (!input.val())
                        break;

                    e.preventDefault();
                    if (isItemSelected()) {
                        //Navigate to corresponding search.
                        hideResultsNow();
                    } else {
                        //Go to search results page.
                        //Add event Handler.
                        if (pluginSettings.onNavigateToSearchResults) setTimeout(function () { pluginSettings.onNavigateToSearchResults(input.val()); }, 1);
                        //window.location = pluginSettings.urlForSearchResults + "?t=" + input.val();
                    }
                    //else do nothing.
                    break;
                default:
                    activeTdId = "";
                    //					if (timeout) clearTimeout(timeout);
                    //					timeout = setTimeout(function() { onChange(); }, options.delay);
                    break;
            }
        });
        input.keyup(function (e) {
            switch (e.keyCode) {
                case 38: // up
                case 40: // down
                case 9:  // tab
                case 13: // return
                    break; //Do nothing, these events are handled on keydown.
                default:
                    //Add a delay if they are typing...
                    if (timeout) clearTimeout(timeout);
                    timeout = setTimeout(function () { drawAutoCompleteResults(input); }, pluginSettings.delayToStartTheFetch);

                    break;
            }
        });

        input.focus(function () {
            hasFocus = true;
            deactivateDefaultText();
            if (input.val())
                drawAutoCompleteResults(input);

        });

        input.blur(function () {
            hasFocus = false;
            if (timeout)
                clearTimeout(timeout);
            timeout = setTimeout(hideResultsNow, 200);

            activateDefaultText();
        });

        if (input.val() == '')
            activateDefaultText();

        function activateDefaultText() {
            if (input.val() == '') {
                input.val(input[0].title);
                input.addClass("defaultTextActive");
            }
        }

        function deactivateDefaultText() {
            if (input.val() == input[0].title) {
                input.val('');
                input.removeClass("defaultTextActive");
            }
        }

        function moveSelect(direction) {

            var lis = searchResults.find('li');
            if (!lis) return;

            //Get the index of th current one
            var selectedIndex = -1;
            for (var i = 0; i < lis.length; i++) {
                if (activeTdId == $(lis[i]).attr("id")) {
                    selectedIndex = i;
                    break;
                }
            }

            //Remove the class to reset all
            resetSelectedItem(lis);

            var indexToSelect = -1;
            if (direction == 1)//Go Up
            {
                var prevIndex = selectedIndex - 1;
                if (prevIndex < 0)
                    prevIndex = lis.length - 1;

                indexToSelect = prevIndex;
            }
            else//Go down
            {
                var nextIndex = selectedIndex + 1;
                if (nextIndex > lis.length - 1)
                    nextIndex = 0;

                indexToSelect = nextIndex;
            }

            setSelectedItem(lis, indexToSelect);
        }

        function resetSelectedItem(lis) {
            for (var i = 0; i < lis.length; i++)
                $(lis[i]).removeClass('selected');
        }

        function setSelectedItem(lis, index) {
            $(lis[index]).addClass('selected');
            activeTdId = $(lis[index]).attr("id");
        }

        function drawAutoCompleteResults(inputControl) {
            var inputContent = $.trim($(inputControl).val());
            if (inputContent.length > 0) {

                var isNumericInput = !isNaN(inputContent);

                searchResults.show();

                var inputOffset = $(inputControl).offset();
                searchResults.offset({ top: (inputOffset.top + 22), left: (inputOffset.left) });
                searchResults.offset({ top: (inputOffset.top + 22), left: (inputOffset.left) });

                getData(inputContent, isNumericInput);
            }
            else
                hideResultsNow();
        }

        function getData(text, isNumericInput) {

            if (text == null || text == '')
                return;

            input.addClass(pluginSettings.cssClassForLoading);

            var matchContains = false;
            if (text.length > pluginSettings.stringLengthToUseContains)
                matchContains = true;

            var dataToSendToService = {
                textToFind: text,
                rowCountToReturn: pluginSettings.rowCountToReturn,
                matchContains: matchContains
            };

            if (pluginSettings.onLogSearch != undefined) {
                pluginSettings.onLogSearch(text);
            }

            var jsonData = JSON.stringify(dataToSendToService);
            if (jsonData == null || jsonData == '') {
                if (pluginSettings.onLogSearchError != undefined) {
                    pluginSettings.onLogSearchError(text);
                }

                input.removeClass(pluginSettings.cssClassForLoading);
                return;
            }

            $.ajax({
                type: "POST",
                url: pluginSettings.urlForWebService,
                data: jsonData,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (jsonSearchResult) {

                    //Set the global data variable.
                    data = jsonSearchResult;

                    presentResults(text, isNumericInput);
                }
                , error: function (result) {
                    input.removeClass(pluginSettings.cssClassForLoading);
                    //alert("AJAX call failed: " + result.status + " " + result.statusText); 
                }
            });

        }

        function presentResults(text, isNumericInput) {
            searchResults.empty();

            var html = "";
            var hasResults = false;
            for (key in data) {
                if ((key.indexOf("__type") == -1) && (key.indexOf("SearchTerm") == -1) && (key.indexOf("StudentQuery") == -1) && (data[key].length > 0)) {
                    hasResults = true;
                    //Create Group Item.
                    html += "<li id=\"quickSearch" + key + "\" data-ItemType=\"Group\"><span class=\"results-group\">" + key + " (<a href=\"" + pluginSettings.urlForSearchResults + "?t=" + input.val() + "\">Show all results...</a>)</span>";
                    html += "<ol>";
                    var parent = data[key];
                    for (child in parent) {

                        var moreResultsForThisEntity = "";
                        if (parent[child].Count > 1)
                            moreResultsForThisEntity = '&nbsp;&nbsp;<b>(' + parent[child].Count + ') results.</b>';

                        html += "<li id=\"" + key.substring(0, 1) + parent[child].Id + "c" + parent[child].SchoolId + "\" data-Id=\"" + parent[child].Id + "\" data-ItemType=\"" + key + "\"><a href=\"" + parent[child].Link.Href + "\">" + highlightWords(parent[child].Text, text) + moreResultsForThisEntity;
                        
                        if (key == "Students" && parent[child].IdentificationCode != null && parent[child].IdentificationCode.toString().indexOf(text) > -1)
                            html += "<span class=\"EntityId\">" + highlightWords(parent[child].IdentificationCode.toString(), text) + "</span>";
                        else if (isNumericInput)
                            html += "<span class=\"EntityId\">" + highlightWords(parent[child].Id.toString(), text) + "</span>";
                        
                        html += "</a></li>";
                    }
                    //Close Children and Group li.
                    html += "</ol></li>";
                }
            }
            searchResults.append(html);

            input.removeClass(pluginSettings.cssClassForLoading);

            //If no results then add a message.
            if (!hasResults)
                hideResultsNow();
        }


        function highlightWords(line, word) {
            word = word.replace(",", "").replace(".", "");
            word = word.replace(' ', "|");
            var regex = new RegExp('(' + word + ')', 'gi');
            line = line.replace(regex, "<em>$1</em>");
            return line;
        }

        function hideResultsNow() {
            searchResults.hide();
            //searchResults.empty();
        }

        function isItemSelected() {

            var li = searchResults.find('li[class*="selected"]');

            if (!li)
                return false;
            if(li.length==0)
                return false;
            
            selectItem(li);
            return true;
        }

        function selectItem(li) {
            if (!li) {
                alert("Could not select an item.");
                return "";
            } else {
                var itemType = $(li).attr("data-ItemType");
                var itemId = $(li).attr("data-Id");

                if (itemType == "Group") {
                    var groupHref = $(li).find('a').attr('href');
                    selectedItem.link = groupHref;
                    selectedItem.type = itemType;
                } else {
                    var dbItem = findItemInData(itemType, itemId);

                    selectedItem.id = itemId;
                    selectedItem.text = dbItem.Text;
                    selectedItem.type = itemType;
                    selectedItem.schoolId = dbItem.SchoolId;
                    selectedItem.count = dbItem.Count;
                    selectedItem.link = dbItem.Link.Href;
                }


                //Execute event Handler.
                if (pluginSettings.onItemSelect) setTimeout(function () { pluginSettings.onItemSelect(selectedItem) }, 1);
            }
        }

        function findItemInData(objectType, idToFind) {
            if (data != null) {
                for (key in data) {
                    if ((key.indexOf("__type") == -1) && (data[key].length > 0)) {
                        if (key == objectType) {
                            var parent = data[key];
                            for (child in parent)
                                if (parent[child].Id == idToFind)
                                    return parent[child];
                        }
                    }
                }
            }
        }

        function defaultItemSelected(selectedObj) {
            $("body").append("<label style=\"position:absolute;top:10px;left:10px;\">" + selectedObj.text + "</label>");
        }

    };
})(jQuery);