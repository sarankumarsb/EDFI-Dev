/// <reference path="../External/jquery.js" />
/// <reference path="../External/jquery-ui.min.js" />
/// <reference path="../External/jquery.flot.js" />
/// <reference path="../External/jquery.flot.selection.js" />
/// <reference path="../External/jquery.flot.threshold.js" />
/// <reference path="../External/jquery.flot.trends.js" />
/// <reference path="../External/excanvas.js" />
/// <reference path="analytics.js" />

function getMetricStateTextFromInt(state) {
    switch (state) {
        case 0:
            return "neutral";
        case 1:
            return "met";
        case 2:
            return "not-met-acceptable";
        case 3:
            return "not-met";
        case 4:
            return "na";
        case 5:
            return "none";
        case 6:
            return "met-exceeded";
        case 7:
            return "not-met-way-below";
        default:
            return "none";
    }
}

function isMetricStateGreen(state) {
    if (state == 1)
        return true;
    return false;
}

function isMetricStateYellow(state) {
    if (state == 2)
        return true;
    return false;
}

function isMetricStateRed(state) {
    if (state == 3)
        return true;
    return false;
}

function isMetricStateGrey(state) {
    if (state == 4 || state == 5)
        return true;
    return false;
}

function setAvailablePeriods(metricId, periods) {

    var periodsForDisplay = "";
    $.each(periods, function (index, value) {
        var cssClass = "LinkFromList";
        if (index == periods.length - 1)
            cssClass = "LinkFromListActive";

        periodsForDisplay += "<a href=\"javascript:getJsonDataForPeriod(" + metricId + "," + value.Id + ");\" class=\"" + cssClass + " display-period\" data-period=\"" + value.Id + "\">" + value.Text + "</a>";
    });

    return periodsForDisplay;
}

function setSchoolGoal(stripLines) {
    if (stripLines.length > 0)
        return stripLines[0].Tooltip;
    return "";
}

function setPeriodTitle(header, title) {
    $(header).html("&nbsp;-&nbsp;" + title.replace(/ /g, "&nbsp;"));
}

function clearPeriodTitle(header) {
    $(header).html("&nbsp;");
}

//Basic Init function
function initSlider(metricId) {
    var sliderBar = $('#sliderBar' + metricId);
    var sliderRangeSelectorLeft = $('#sliderRangeSelectorLeft' + metricId);
    var sliderRangeSelectorRight = $('#sliderRangeSelectorRight' + metricId);

    //While data is being fetched lets put together the rest.
    //The slider...
    var dragOpt = {
        axis: "x",
        containment: "parent",
        start: function () {
            jQuery["userIsMovingSlide" + metricId] = true;
            jQuery["userIsDraggingTheBar" + metricId] = true;
        },
        drag: function (event, ui) {
            var sliderBarOffset = sliderBar.offset();
            //Move the sliderRangeSelectors with the SliderBar.
            sliderPan(metricId, sliderBarOffset.left);
        },
        stop: function (event, ui) {
            jQuery["userIsMovingSlide" + metricId] = false;
            jQuery["userIsDraggingTheBar" + metricId] = false;
        }
    };

    sliderBar.draggable(dragOpt);
    //For the arrows
    $('#sliderLeftArrow' + metricId).mousedown(function () {
        jQuery["userIsMovingSlide" + metricId] = true;
        var sliderBarOffset = sliderBar.offset();

        var parent = sliderBar.parent();
        var parentOffset = parent.offset();

        var parentLeft = 0;
        if (parentOffset != null)
            parentLeft = parentOffset.left;

        var maxRight = parentLeft;

        //sliderPan(metricId,newLeft);
        jQuery["sliderBarLeftPos" + metricId] = sliderBarOffset.left;
        jQuery["timerForScrollBarArrows" + metricId] = setInterval("moveLeft(" + metricId + "," + maxRight + ");", 80);
    }).mouseup(function () {
        jQuery["userIsMovingSlide" + metricId] = false;
        clearInterval(jQuery["timerForScrollBarArrows" + metricId]);
    }).mouseout(function () {
        if (!jQuery["userIsDraggingTheRange" + metricId] && !jQuery["userIsDraggingTheBar" + metricId]) {
            jQuery["userIsMovingSlide" + metricId] = false;
            clearInterval(jQuery["timerForScrollBarArrows" + metricId]);
        }
    });


    $('#sliderRightArrow' + metricId).mousedown(function () {
        jQuery["userIsMovingSlide" + metricId] = true;
        var sliderBarOffset = sliderBar.offset();
        var sliderWidth = sliderBar.width();

        var parent = sliderBar.parent();
        var parentWidth = parent.width();
        var parentOffset = parent.offset();
        var parentLeft = parentOffset.left;

        var maxLeft = (parentLeft + parentWidth) - (sliderWidth + 1);

        //sliderPan(metricId,newLeft);
        jQuery["sliderBarLeftPos" + metricId] = sliderBarOffset.left;
        jQuery["timerForScrollBarArrows" + metricId] = setInterval("moveRight(" + metricId + "," + maxLeft + ");", 80);

    }).mouseup(function () {
        jQuery["userIsMovingSlide" + metricId] = false;
        clearInterval(jQuery["timerForScrollBarArrows" + metricId]);

    }).mouseout(function () {
        if (!jQuery["userIsDraggingTheRange" + metricId] && !jQuery["userIsDraggingTheBar" + metricId]) {
            jQuery["userIsMovingSlide" + metricId] = false;
            clearInterval(jQuery["timerForScrollBarArrows" + metricId]);
        }
    });

    //For the rangeSelectors...
    var dragOptRangeSelector = {
        axis: "x",
        containment: "parent",
        start: function () {
            jQuery["userIsMovingSlide" + metricId] = true;
            jQuery["userIsDraggingTheRange" + metricId] = true;
        },
        drag: function (event, ui) {
            jQuery["userIsDraggingTheRange" + metricId] = true;
            var sliderRangeSelectorLeftOffset = sliderRangeSelectorLeft.offset();
            var sliderRangeSelectorRightOffset = sliderRangeSelectorRight.offset();
            var distanceBetweenSelectors = (sliderRangeSelectorRightOffset.left - sliderRangeSelectorLeftOffset.left) + 9; //The 9 is the width of the image.

            //Move to the left then expand width to the distance.
            sliderBar.offset({ left: sliderRangeSelectorLeftOffset.left });
            sliderBar.width(distanceBetweenSelectors);

            //$('#msgsLabel').text("MouseMoving" + event.pageX + " Distance:"+ distanceBetweenSelectors);
            syncChartSelectionAndSliderBar(metricId, sliderRangeSelectorLeftOffset.left);
        },
        stop: function (event, ui) {
            jQuery["userIsMovingSlide" + metricId] = false;
            jQuery["userIsDraggingTheRange" + metricId] = false;
        }
    };

    sliderRangeSelectorLeft.draggable(dragOptRangeSelector);
    sliderRangeSelectorRight.draggable(dragOptRangeSelector);

    sliderReset(metricId);
}

function moveLeft(metricId, maxLeft) {
    jQuery["sliderBarLeftPos" + metricId] -= 8;
    if (jQuery["sliderBarLeftPos" + metricId] < maxLeft) {
        jQuery["sliderBarLeftPos" + metricId] = maxLeft;
        clearTimeout(jQuery["timerForScrollBarArrows" + metricId]);
    }
    sliderPan(metricId, jQuery["sliderBarLeftPos" + metricId]);
}

function moveRight(metricId, maxRight) {
    jQuery["sliderBarLeftPos" + metricId] += 8;
    if (jQuery["sliderBarLeftPos" + metricId] > maxRight) {
        jQuery["sliderBarLeftPos" + metricId] = maxRight;
        clearTimeout(jQuery["timerForScrollBarArrows" + metricId]);
    }
    sliderPan(metricId, jQuery["sliderBarLeftPos" + metricId]);
}

function sliderPan(metricId, offsetLeft) {
    var sliderBar = $("#sliderBar" + metricId);
    var sliderRangeSelectorLeft = $('#sliderRangeSelectorLeft' + metricId);
    var sliderRangeSelectorRight = $('#sliderRangeSelectorRight' + metricId);

    sliderBar.offset({ left: offsetLeft });
    sliderRangeSelectorLeft.offset({ left: offsetLeft });
    sliderRangeSelectorRight.offset({ left: offsetLeft + sliderBar.width() - 9 }); //9 for the image width...

    syncChartSelectionAndSliderBar(metricId, offsetLeft);
}

function syncChartSelectionAndSliderBar(metricId, offsetLeft) {
    var sliderBar = $("#sliderBar" + metricId);

    var parent = sliderBar.parent();
    var parentOffset = parent.offset();

    //Calculate the percentage selected...
    var actualSliderPercentageRange = (sliderBar.width() * 100 / parent.width());
    var actualLeftPositionInTheSlider = (offsetLeft - parentOffset.left);
    var actualSliderPercentageLeftPoint = actualLeftPositionInTheSlider * 100 / parent.width();

    //Translate positions to points...
    var fromDataPointTranslation = (jQuery["chartMaxDataPoint" + metricId] * actualSliderPercentageLeftPoint / 100);
    var toDataPointTranslation = fromDataPointTranslation + (jQuery["chartMaxDataPoint" + metricId] * actualSliderPercentageRange / 100);
    var newSelection = { xaxis: { from: fromDataPointTranslation, to: toDataPointTranslation } };
    jQuery["chartOverview" + metricId].setSelection(newSelection);
}

function sliderReset(metricId) {
    var sliderBar = $("#sliderBar" + metricId);
    var sliderRangeSelectorLeft = $('#sliderRangeSelectorLeft' + metricId);
    var sliderRangeSelectorRight = $('#sliderRangeSelectorRight' + metricId);

    var sliderBarOffset = sliderBar.offset();
    var sliderWidth = sliderBar.width();

    var parent = sliderBar.parent();
    var parentWidth = parent.width();
    var parentOffset = parent.offset();

    var parentLeft = 0;
    if (parentOffset != null)
        parentLeft = parentOffset.left;

    var maxLeft = (parentLeft + parentWidth) - (sliderWidth + 1);

    sliderBar.offset({ left: maxLeft });
    sliderRangeSelectorLeft.offset({ left: maxLeft });
    sliderRangeSelectorRight.offset({ left: maxLeft + sliderWidth - 9 }); //9 for the width of the image...
}

/***PLOT Function***/

function plot(metricId, chartData, chartOptions) {
    //DO NOT REMOVE! This hack is to make IE8 work. When calling plot dircetly without going through a $.ajax call we need to give some time for all the polyfills and shims to act.
    setTimeout(function () {
        plotCallback(metricId, chartData, chartOptions);
    }, 100);
}

function plotCallback(metricId, chartData, chartOptions) {

    var axisLabelHeight = 25; //I figured this value out with trial and error, finding a value that could display 1 or 2 lines of text, and still look right
    var yaxisLabelWidth = 45; //I figured this value out with trial and error, finding a value that would get the left side of the grid to align with the left-hand side of the scroll bar.
    var maxPointsToDisplay = 4;
    var chartPlaceHolder = $('#placeHolderForChart' + metricId);
    var chartOverviewPlaceHolder = $('#placeHolderForChartOverview' + metricId);
    var sliderPlaceHolder = $('#sliderContainer' + metricId);
    var widthOfGraphPoint; //BarGraphs have a width of 1, since they are graphed with a space between them.  Points have a width of zero, since they are just graphed, and the spot next to them is the next point used.

    var flotChartData = chartDataToFlotFormat(chartData);
    var availablePointsToDraw = getChartDataPointCount(flotChartData);

    //Set the formatter in a global spec...
    jQuery["yaxisLabelFormatterToUse" + metricId] = getYAxisFormatter(chartData);

    var maxYAxisValue = findMaxValueInData(chartData);
    if (chartData.Context)
        $('#chartContext' + metricId).text(chartData.Context);
    if (chartData.SubContext)
        $('#chartSubContext' + metricId).text(chartData.SubContext);

    var mainGraphSeries;
    if (chartData.DisplayType == 'LineGraph' || chartData.DisplayType == 'PointGraph') {
        mainGraphSeries = { lines: { show: chartData.DisplayType == 'LineGraph' }, points: { show: true} };
        widthOfGraphPoint = 0;
    } else {
        //With bar graphs, we want a little room at the top, so bump up the maxYAxisValue
        maxYAxisValue += (maxYAxisValue * .25);
        mainGraphSeries = { bars: { fill: 1, showDataPointAsLabel: true, dataPointAsLabelFormatter: jQuery["yaxisLabelFormatterToUse" + metricId] } };
        widthOfGraphPoint = 1;
    }

    var yAxisTicks;
    var yAxisTicksWithoutLines;
    var yAxisOverviewTicks;
    if (chartData.YAxisLabels && chartData.YAxisLabels.length > 0) {
        yAxisTicks = [];
        yAxisTicksWithoutLines = [];
        for (var i = 0; i < chartData.YAxisLabels.length; i++) {
            var label = chartData.YAxisLabels[i];
            if (label.Position != null) {
                yAxisTicksWithoutLines.push([label.Position, label.Text]);
                yAxisTicks.push([label.Position, '']);
            } else if (label.MinPosition != null && label.MaxPosition != null) {
                yAxisTicksWithoutLines.push([(label.MinPosition + label.MaxPosition) / 2, label.Text]);
                yAxisTicks.push([label.MinPosition, '']);
                yAxisTicks.push([label.MaxPosition, '']);
            }
        }
        yAxisOverviewTicks = [yAxisTicks[0], yAxisTicks[yAxisTicks.length - 1]];
    } else { //Fall back and let flot guess at axis labels.
        yAxisTicks = null;
        yAxisOverviewTicks = 1;
    }

    var plotOptions = {
        series: mainGraphSeries,
        grid: { hoverable: true, clickable: true },
        xaxis: {
            tickColor: "#FFFFFF",
            ticks: flotChartData.xAxisTickLabelsFormatedForFlot,
            labelHeight: axisLabelHeight
        },
        yaxes: [{ //The Y1 axis contains just the tick-lines that go across the grid.
            tickFormatter: jQuery["yaxisLabelFormatterToUse" + metricId],
            labelWidth: 0,
            labelHeight: axisLabelHeight,
            max: maxYAxisValue,
            min: 0,
            ticks: yAxisTicks,
            position: 'right',
            show: true
        },
            {//The Y2 axis contains all the text on the left side of the grid, with no grid lines
                tickFormatter: jQuery["yaxisLabelFormatterToUse" + metricId],
                labelWidth: yaxisLabelWidth,
                labelHeight: axisLabelHeight,
                max: maxYAxisValue,
                min: 0,
                tickLength: 0,
                ticks: yAxisTicksWithoutLines,
                show: true
            }],
        arrowTrends: {
            show: true
        }
    };

    //Extend replaces so we do it manually only to add these 2 props.
    if (chartOptions) {
        plotOptions.xaxis.min = chartOptions.xaxis.min;
        plotOptions.xaxis.max = chartOptions.xaxis.max;
    } else {
        plotOptions.xaxis.min = 0;
        plotOptions.xaxis.max = getChartMaxDataPoint(flotChartData, widthOfGraphPoint);
    }

    //Lets plot the Main Chart....	
    jQuery["chart" + metricId] = $.plot(
                chartPlaceHolder,
                [flotChartData.redBarData, flotChartData.yellowBarData, flotChartData.greenBarData, flotChartData.stripLineDataWithOptions, flotChartData.redDataPoints, flotChartData.greenDataPoints, flotChartData.greyDataPoints],
                plotOptions
            );

    //Set the count of the data for later access...
    jQuery["chartMaxDataPoint" + metricId] = getChartMaxDataPoint(flotChartData, widthOfGraphPoint);

    //alert(jQuery["chartMaxDataPoint" + metricId]);

    //***The overview chart...***
    //Only draw the overview chart if we have enough data...

    if (chartOptions == null && chartData.OverviewChartIsEnabled) { //If we have chart options this means that we are zooming in so we dont do anything.
        if (availablePointsToDraw > maxPointsToDisplay) {

            chartOverviewPlaceHolder.show();
            sliderPlaceHolder.show();

            var series;
            if (chartData.DisplayType == 'LineGraph' || chartData.DisplayType == 'PointGraph') {
                series = {};
            } else {
                series = { bars: { showDataPointAsLabel: false } };
            }

            var overviewplotOptions = {
                series: series,
                selection: {
                    mode: "x",
                    color: '#999999',
                    plotselecting:
                                function (e) {
                                    if (jQuery["userIsMovingSlide" + metricId] != true) {
                                        //$('#msgsLabel').text(e.x1 + ", " + e.x2);
                                        //Adjust the sliderBar
                                        var sliderBar = $("#sliderBar" + metricId);
                                        var parent = sliderBar.parent();
                                        var parentOffset = parent.offset();
                                        var absoluteLeft = e.x1 + parentOffset.left;
                                        var barWidth = e.x2 - e.x1 + 7;

                                        //Lets check if this coordinate is sane.
                                        var maxLeft = (parentOffset.left + parent.width()) - (barWidth + 1);

                                        if (absoluteLeft > maxLeft)
                                            absoluteLeft = maxLeft;


                                        var sliderRangeSelectorLeft = $('#sliderRangeSelectorLeft' + metricId);
                                        var sliderRangeSelectorRight = $('#sliderRangeSelectorRight' + metricId);

                                        //Move and Size the slider bar...
                                        sliderBar.offset({ left: absoluteLeft });
                                        sliderBar.width(barWidth);
                                        //Position the range selectors.
                                        sliderRangeSelectorLeft.offset({ left: absoluteLeft });
                                        sliderRangeSelectorRight.offset({ left: absoluteLeft + barWidth - 9 }); //9 for the image width...
                                    }
                                }
                },
                xaxis: {
                    tickColor: "#FFFFFF", ticks: 0
                },
                yaxis: {
                    tickFormatter: jQuery["yaxisLabelFormatterToUse" + metricId],
                    labelWidth: yaxisLabelWidth,
                    labelHeight: axisLabelHeight,
                    ticks: yAxisOverviewTicks
                }
            };

            if (getChartDataPointCount(flotChartData) > 0) {
                //Set the min and max to the first and last point, with a bit of margin on each side.
                overviewplotOptions.xaxis.min = getChartMinDataPoint(flotChartData);
                overviewplotOptions.xaxis.max = getChartMaxDataPoint(flotChartData, widthOfGraphPoint);
            }

            jQuery["chartOverview" + metricId] = $.plot(
                        chartOverviewPlaceHolder,
                        [flotChartData.redBarData, flotChartData.yellowBarData, flotChartData.greenBarData, flotChartData.stripLineDataWithOptions, flotChartData.redDataPoints, flotChartData.greenDataPoints, flotChartData.greyDataPoints],
                        overviewplotOptions
                    );
        } else {
            chartOverviewPlaceHolder.hide();
            sliderPlaceHolder.hide();
        }
    }

    //Add the tooltip handler and a little logic

    /*
    var previousPoint = null;
    chartPlaceHolder.unbind("plothover");
    chartPlaceHolder.bind("plothover", function (event, pos, item) {
        if (item) {
            if (previousPoint != item.dataIndex) {
                previousPoint = item.dataIndex;
                $("#chartTooltip" + metricId).hide();
                $('#tooltipIndicator' + metricId).hide();

                var sereiesIndex = item.seriesIndex;
                var realDataIndex = dataPointToIndex(item.datapoint[0]) - .5;
                var x = item.datapoint[0].toFixed(2);
                var y = item.datapoint[1].toFixed(2);
                var chartOffset = chartPlaceHolder.offset();

                var dataPoint = null;
                    if (sereiesIndex == 1) { //Serie number 1 is always the stripLine in this context
                        if (chartData.StripLines.length > 0) {
                            dataPoint = chartData.StripLines[0];
                        }
                    } else //Series0 and 2 are the green and red series.
                        dataPoint = chartData.SeriesCollection[0].Points[realDataIndex];
                    //alert(realDataIndex);
                showTooltip(chartOffset.left, item.pageX, item.pageY, dataPoint, metricId);
            }
        }
        else {
            $("#chartTooltip" + metricId).hide();
            $('#tooltipIndicator' + metricId).hide();
            previousPoint = null;
        }
    });
    */

    function showTooltip(x, y, contents) {

        var tooltip;

        if (chartData.DisplayType == 'LineGraph' || chartData.DisplayType == 'PointGraph') {
            tooltip = $('<div id="historicalLineChartTooltip" class="historical-chart-tooltip" ><div style="text-align: center; line-height: 8px;"><img style="top: 1px; position: relative;" src="' + $.global.siteBasePath + '/App_themes/Theme1/img/OverviewStatus/ToolTipArrow.png"></div><div class="tooltip-value">' + contents + '</div></div>');
            tooltip.css({
                position: 'absolute',
                display: 'none',
                top: y + 10
            });
        } else {
            tooltip = $('<div id="historicalLineChartTooltip" class="historical-chart-tooltip" ><div class="tooltip-value">' + contents + '</div></div>');
            tooltip.css({
                position: 'absolute',
                display: 'none'
            });
        }
        tooltip.appendTo("body");

        if (chartData.DisplayType == 'LineGraph' || chartData.DisplayType == 'PointGraph') {
            tooltip.css({ left: x - (tooltip.outerWidth(true) / 2) });
        } else {
            //The tooltip is currently left-justified over the bar.  This should probably use a better formula to figure out where to place the tooltip.
            tooltip.css({ top: y - tooltip.height() - 30, left: x });
        }

        tooltip.fadeIn(200);
    }

    chartPlaceHolder.unbind("plothover");
    chartPlaceHolder.bind("plothover", function (event, pos, item) {
        var dataPointValue;

        //If this is the stripline        
        if (item && item.seriesIndex == 1) {
            if (chartPlaceHolder.data("previousPoint") != item.seriesIndex + 100000) {
                chartPlaceHolder.data("previousPoint", item.seriesIndex + 100000);

                $("#historicalLineChartTooltip").remove();

                if (chartData.StripLines.length > 0) {
                    var dataPoint = chartData.StripLines[0];
                    dataPointValue = dataPoint.Value;
                    if (dataPoint.Tooltip) {
                        showTooltip('historicalLineChartTooltip', item.pageX, item.pageY, "<div class='tooltip-value'>" + dataPoint.Tooltip.replace("\n", "<br />") + "</div>", '');
                    }
                }
            }

            //Normal data points.
        } else if (item && item.seriesIndex != 1) {

            //This figure out the index based on the data, because this even can be raised from the red or the green series, which isn't the index from the series.
            var dataIndex = (item.datapoint[0] - 1) / 2;

            if (chartPlaceHolder.data("previousPoint") != dataIndex) {
                chartPlaceHolder.data("previousPoint", dataIndex);

                $("#historicalLineChartTooltip").remove();

                var periodToDisplay = chartData.SeriesCollection[0].Points[dataIndex];

                var tooltipMarkup = '';

                if (periodToDisplay.TooltipHeader)
                    tooltipMarkup += "<div class='title'>" + periodToDisplay.TooltipHeader + "</div>";

                if (periodToDisplay.Tooltip)
                    tooltipMarkup += "<div class='subtitle'>" + periodToDisplay.Tooltip + "</div>";

                dataPointValue = periodToDisplay.Value;
                var value = null;
                if (periodToDisplay.ValueAsText != null)
                    value = periodToDisplay.ValueAsText;

                if (value != null)
                    //TODO: This 17px is the same height as the StateGood class.  This really needs to be moved to the style sheet
                    tooltipMarkup += "<div style='text-align: center;'><span class='status-goal-" + getMetricStateTextFromInt(periodToDisplay.State) + "'>" + value + "</span></div.";

                showTooltip(item.pageX, item.pageY, tooltipMarkup);
            }
        }
        else {
            $("#historicalLineChartTooltip").remove();
            chartPlaceHolder.data("previousPoint", null);
        }

        //Show the tooltip on the left-hand side
        if (item && chartData.ShowMouseOverToolTipOnLeft) {
            if (chartPlaceHolder.data("previousTooltipPoint") != item.seriesIndex) {
                chartPlaceHolder.data("previousTooltipPoint", item.seriesIndex);

                $('#tooltipIndicator').remove();

                //var tooltipIndicatorValue = $('#tooltipIndicatorValue' + metricId);
                var dataPointValue = jQuery["yaxisLabelFormatterToUse" + metricId](dataPointValue);
                var chartOffset = chartPlaceHolder.offset();

                if (dataPointValue) {
                    var indicator = $('<table id="tooltipIndicator"><tr><td style="background-color:White; padding:5px; border:1px solid #225273; border-bottom:2px solid #225273; text-align:left; font-size:10px;">' + dataPointValue + '</td><td>---</td></tr></table>');
                    indicator.css({
                        position: 'absolute',
                        display: 'none'
                    });

                    indicator.appendTo("body");

                    //The tooltip is currently left-justified over the bar.  This should probably use a better formula to figure out where to place the tooltip.
                    indicator.css({ top: item.pageY - 17, left: chartOffset.left - 30 });

                    indicator.fadeIn(200);
                }
            }
        } else {
            $('#tooltipIndicator').remove();
            chartPlaceHolder.data("previousTooltipPoint", null);
        }
    });

    //Hook up the eventhandlers for the selections...
    //Add the handler for the selected action...
    chartOverviewPlaceHolder.unbind("plotselected");
    chartOverviewPlaceHolder.bind("plotselected", function (event, ranges) {
        var augmentedPlotOprions = {
            xaxis: { min: ranges.xaxis.from, max: ranges.xaxis.to }
        };

        //Just in case it is not hidden we hide it.
        $("#chartTooltip" + metricId).hide();

        //Redraw the chart.
        plot(metricId, chartData, augmentedPlotOprions);
    });

    //Analyse data and preset selection if needed.
    //If we have more points than what we want displayed then we zoom in and create a default selection.
    if (chartOptions == null && chartData.OverviewChartIsEnabled) {
        if (availablePointsToDraw > maxPointsToDisplay) {
            //Get the range of the last points.
            var from = (availablePointsToDraw - maxPointsToDisplay) * 2; //*2 because of the space i am saving in the chart
            var to = (availablePointsToDraw * 2) + widthOfGraphPoint; //*2 because of the space i am saving in the chart

            jQuery["chartOverview" + metricId].setSelection({ xaxis: { from: from, to: to } });

            //Set the scroll length to the selection length.
            var percentageToDisplay = maxPointsToDisplay * 100 / availablePointsToDraw;
            setSliderBarRangeSelectorWidth(metricId, percentageToDisplay);
        }
    }
} //End of PLOT function...

function getChartMaxDataPoint(flotChartData, widthOfGraphPoint) {
    var redBarIndex = 0,
        yellowBarIndex = 0,
        greenBarIndex = 0,
        redPointIndex = 0,
        greenPointIndex = 0,
        greyPointIndex = 0;

    redBarIndex = flotChartData.redBarData.data.length && flotChartData.redBarData.data[flotChartData.redBarData.data.length - 1][0];
    yellowBarIndex = flotChartData.yellowBarData.data.length && flotChartData.yellowBarData.data[flotChartData.yellowBarData.data.length - 1][0];
    greenBarIndex = flotChartData.greenBarData.data.length && flotChartData.greenBarData.data[flotChartData.greenBarData.data.length - 1][0];
    redPointIndex = flotChartData.redDataPoints.data.length && flotChartData.redDataPoints.data[flotChartData.redDataPoints.data.length - 1][0];
    greenPointIndex = flotChartData.greenDataPoints.data.length && flotChartData.greenDataPoints.data[flotChartData.greenDataPoints.data.length - 1][0];
    greyPointIndex = flotChartData.greyDataPoints.data.length && flotChartData.greyDataPoints.data[flotChartData.greyDataPoints.data.length - 1][0];

    //Find biggest index.
    var maxIndex = redBarIndex;
    if (yellowBarIndex > maxIndex)
        maxIndex = yellowBarIndex;

    if (greenBarIndex > maxIndex)
        maxIndex = greenBarIndex;

    if (redPointIndex > maxIndex)
        maxIndex = redPointIndex;

    if (greenPointIndex > maxIndex)
        maxIndex = greenPointIndex;

    if (greyPointIndex > maxIndex)
        maxIndex = greyPointIndex;

    return (maxIndex + 1 + widthOfGraphPoint);
}

function getChartMinDataPoint(flotChartData) {
    var redIndex = 0,
        yellowIndex = 0,
        greenIndex = 0;

    redIndex = flotChartData.redBarData.data.length && flotChartData.redBarData.data[0][0];
    yellowIndex = flotChartData.yellowBarData.data.length && flotChartData.yellowBarData.data[0][0];
    greenIndex = flotChartData.greenBarData.data.length && flotChartData.greenBarData.data[0][0];

    //Find smallest index.
    var minIndex = redIndex;
    if (yellowIndex < minIndex)
        minIndex = yellowIndex;

    if (greenIndex < minIndex)
        minIndex = greenIndex;

    if (minIndex != 0)
        minIndex--;

    return minIndex;
}

function getChartDataPointCount(flotChartData) {
    return (flotChartData.redBarData.data.length + flotChartData.yellowBarData.data.length + flotChartData.greenBarData.data.length);
}

function findMaxValueInData(chartData) {
    var points = chartData.SeriesCollection[0].Points;

    var max = 0;
    for (var i = 0; i < points.length; i++) {
        var v;
        if (points[i].RatioLocation != null)
            v = points[i].RatioLocation;
        else
            v = points[i].Value;
        if (v > max)
            max = v;
    }

    if (chartData.StripLines.length > 0) {
        var stripLineValue = chartData.StripLines[0].Value;
        if (max < stripLineValue)
            max = stripLineValue;
    }

    if (chartData.YAxisLabels && chartData.YAxisLabels.length > 0) {
        for (i = 0; i < chartData.YAxisLabels.length; i++) {
            var label = chartData.YAxisLabels[i];
            if (label.Position > max)
                max = label.Position;
        }
    }

    if (chartData.YAxisMaxValue) {
        if (chartData.YAxisMaxValue > max)
            max = chartData.YAxisMaxValue;
    }

    return max;
}

function setSliderBarRangeSelectorWidth(metricId, percent) {
    var sliderBar = $("#sliderBar" + metricId);

    var parent = sliderBar.parent();
    var parentWidth = parent.width();

    var newBarWidth = (percent * parentWidth / 100) - 9; //The width of the image.
    sliderBar.width(newBarWidth);
}

function dataPointToIndex(point) {
    if (point == 0)
        return 0;

    //This is because when we draw we have spacesaver each other bar...
    return point / 2;
}

function chartDataToFlotFormat(chartData) {
    var stripLineColor = "#0000ff";

    //Creating the return object.
    var retModel = {};
    retModel.redDataPoints = { data: [], lines: { show: false }, points: { show: true }, color: "#b30002" };
    retModel.greenDataPoints = { data: [], lines: { show: false }, points: { show: true }, color: "#09662f" };
    retModel.greyDataPoints = { data: [], lines: { show: false }, points: { show: true }, color: "#555" };

    //For bar charts
    retModel.redBarData = { data: [], bars: { show: true }, color: "#b30002" };
    retModel.yellowBarData = { data: [], bars: { show: true }, color: "#957013" };
    retModel.greenBarData = { data: [], bars: { show: true }, color: "#0a662f" };

    var dataFormatedForFlot = [];
    var xAxisTickLabelsFormatedForFlot = [];

    var flotXslotIndex = 1;
    $.each(chartData.SeriesCollection[0].Points, function (index, point) {
        var v;
        if (point.RatioLocation != null)
            v = point.RatioLocation;
        else
            v = point.Value;

        dataFormatedForFlot.push([flotXslotIndex, v]);

        var label = point.Label;
        if (point.SubLabel) {
            label += "<br/>" + point.SubLabel;
        }

        if (chartData.DisplayType == 'LineGraph' || chartData.DisplayType == 'PointGraph') { //line graphs need their text directly below the point
            xAxisTickLabelsFormatedForFlot.push([flotXslotIndex, label]);
            if (isMetricStateGreen(point.State))
                retModel.greenDataPoints.data.push([flotXslotIndex, v]);
            if (isMetricStateRed(point.State))
                retModel.redDataPoints.data.push([flotXslotIndex, v]);
            if (isMetricStateGrey(point.State))
                retModel.greyDataPoints.data.push([flotXslotIndex, v]);

        } else { //Bar graphs need their text moved over just a hair
            xAxisTickLabelsFormatedForFlot.push([flotXslotIndex + .5, label]);

            if (isMetricStateGreen(point.State))
                retModel.greenBarData.data.push([flotXslotIndex, v]);
            if (isMetricStateYellow(point.State))
                retModel.yellowBarData.data.push([flotXslotIndex, v]);
            if (isMetricStateRed(point.State))
                retModel.redBarData.data.push([flotXslotIndex, v]);
        }
        //Add 2 so we have a white space between bars...
        flotXslotIndex += 2;
    });

    //Creating the Striplines...
    var stripLineData = [];

    if (chartData.StripLines.length > 0) {
        //We want the stripline to extend past the end of the graph, the +4 is to ensure that happens.
        for (var i = 0; i < (dataFormatedForFlot.length * 2) + 4; i++)
            stripLineData.push([i, chartData.StripLines[0].Value]);
    }

    retModel.xAxisTickLabelsFormatedForFlot = xAxisTickLabelsFormatedForFlot;

    retModel.stripLineDataWithOptions = {
        data: stripLineData,
        color: stripLineColor,
        lines: { show: true }
    };

    //We have striplines that have tooltips, and striplines that don't have tooltips.  If it doesn't have tooltips, then never show the points.
    if (chartData.StripLines.length > 0) {
        if (!chartData.StripLines[0].Tooltip) {
            retModel.stripLineDataWithOptions.points = { show: false };
            retModel.stripLineDataWithOptions.hoverable = false;
        }
    }

    return retModel;
}

//Axis formatters: we should have 2 today. 1 for normal integers and 1 for percentages.
function getYAxisFormatter(chartData) {
    if (chartData.SeriesCollection.length > 0)
        if (chartData.SeriesCollection[0].Points.length > 0)
            if (chartData.SeriesCollection[0].Points[0].ValueType == "System.Double")
                return yaxisLabelPercentageFormatter;
            else if (chartData.SeriesCollection[0].Points[0].ValueType == "System.Int32")
                return yaxisLabelIntegerFormatter;

    return yaxisLabelFormatter;
}

function yaxisLabelPercentageFormatter(val, axis, nbsp) {
    if (val <= 1 && (nbsp == undefined || nbsp == true))
        return (val * 100).toFixed(1) + "&nbsp;%";

    if (val <= 1)
        return (val * 100).toFixed(1) + " %";
    return "";
}

function yaxisLabelIntegerFormatter(val, axis) {
    if (val == val.toFixed(0))
        return val;
    return "";
}

function yaxisLabelFormatter(val, axis) {
    return (val).toFixed(1);
}

function showHistoricalChartComponent(metricId) {
    $('#placeHolderForChart' + metricId).css('height', '250px');
    $('#historical-chart-' + metricId + " .slider-container").show();
    $('#tableForAvailablePeriods' + metricId).show();
    $('#topSpace' + metricId).show();
    $('#bottomSpace' + metricId).show();
}

function hideHistoricalChartComponent(metricId) {
    $('#placeHolderForChart' + metricId).hide();
    $('#placeHolderForChartOverview' + metricId).hide();
    $('#historical-chart-' + metricId + " .slider-container").hide();
    $('#tableForAvailablePeriods' + metricId).hide();
    $('#topSpace' + metricId).hide();
    $('#bottomSpace' + metricId).hide();
}

function hasHistoricalChartasData(jsonData) {

    if (jsonData.ChartData && jsonData.ChartData.SeriesCollection)
        if (jsonData.ChartData.SeriesCollection.length > 0 && jsonData.ChartData.SeriesCollection[0].Points.length > 0)
            return true;

    return false;
}