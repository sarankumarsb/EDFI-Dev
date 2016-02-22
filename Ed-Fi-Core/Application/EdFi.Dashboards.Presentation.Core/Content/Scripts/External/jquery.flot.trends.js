/// <reference path="jquery.js" />
/// <reference path="excanvas.js" />
/// <reference path="jquery.flot.js" />


/* Arrow Trends Plugin for flot.

Created by: Douglas Loyo, August 2011. for contact info/bugs/other: www.douglasloyo.com

*/

(function ($) {
    //Here are the options you can set on this plugin
    var options = {
        arrowTrends: {
            show: false,
            arrowFillStyle: '#827e7e',
            arrowBaseWidth: 10
        }
    };

    function init(plot) {
        //
        plot.hooks.draw.push(function (plot, ctx) {

            var actualOptions = plot.getOptions();

            if (!actualOptions.arrowTrends.show)
                return;

            //Setting the color of the arrow.
            if (actualOptions.arrowTrends.arrowFillStyle)
                ctx.fillStyle = actualOptions.arrowTrends.arrowFillStyle;
            else
                ctx.fillStyle = options.arrowTrends.arrowFillStyle;

            //setting the width of the arrow base to use.
            var arrowBaseWidth = 0;
            if (actualOptions.arrowTrends.arrowBaseWidth)
                arrowBaseWidth = actualOptions.arrowTrends.arrowBaseWidth;
            else
                arrowBaseWidth = options.arrowTrends.arrowBaseWidth;


            //Get a reference to a series object for ploting purposes.
            var seriesObj;
            //Flatten out the series that are bars for trend calculation.
            var flatBarData = [];
            $.each(plot.getData(), function (index, series) {
                //Only if it is a bars series we draw trends...
                if (!series.bars.show)
                    return;

                seriesObj = series;

                for (var i = 0; i < series.data.length; ++i)
                    if (series.data[i] != null)
                        flatBarData.push(series.data[i]);
            });

            //Order the points to be able to calculate trends.
            flatBarData.sort(xCoordinatePointComparer);

            var prevValue = null;
            for (var i = 0; i < flatBarData.length; ++i) {

                //Calculate x to be the center of the bar.
                var x = seriesObj.xaxis.p2c(flatBarData[i][0] + seriesObj.bars.barWidth / 2) + plot.getPlotOffset().left;
                var y = seriesObj.yaxis.p2c(flatBarData[i][1]) + plot.getPlotOffset().top - 20;
                var yValue = flatBarData[i][1];

                //If it is the first point we cant calculate the trend.
                if (prevValue == null) {
                    prevValue = yValue;
                    continue;
                }

                //Only draw if inside of viewport....
                if (x > plot.getPlotOffset().left && ((x + arrowBaseWidth) < plot.getCanvas().width)) {
                    var trendDirection = Compare10Percent(prevValue, yValue);

                    if (trendDirection == 1)
                        drawUpTriangle(ctx, x, y, arrowBaseWidth);

                    if (trendDirection == 0)
                        drawNoChangeTriangle(ctx, x, y, arrowBaseWidth);

                    if (trendDirection == -1)
                        drawDownTriangle(ctx, x, y, arrowBaseWidth);
                }

                prevValue = yValue;
            }
        });

    }

    function drawUpTriangle(ctx, fromX, fromY, baseWidth) {
        //This is so the triangle draws in the middle of the given (x,y) coordinate.
        var newY = fromY + (baseWidth / 2);
        var newX = fromX - (baseWidth / 2);

        ctx.beginPath();

        ctx.moveTo(newX, newY);
        ctx.lineTo(newX + baseWidth, newY);
        ctx.lineTo(newX + (baseWidth / 2), newY - baseWidth);
        ctx.lineTo(newX, newY);

        ctx.fill();
        ctx.closePath();
    }

    function drawDownTriangle(ctx, fromX, fromY, baseWidth) {
        //This is so the triangle draws in the middle of the given (x,y) coordinate.
        var newY = fromY - (baseWidth / 2);
        var newX = fromX - (baseWidth / 2);

        ctx.beginPath();

        ctx.moveTo(newX, newY);
        ctx.lineTo(newX + baseWidth, newY);
        ctx.lineTo(newX + (baseWidth / 2), newY + baseWidth);
        ctx.lineTo(newX, newY);

        ctx.fill();
        ctx.closePath();
    }

    function drawNoChangeTriangle(ctx, fromX, fromY, baseWidth) {
        //This is so the triangle draws in the middle of the given (x,y) coordinate.
        var newX = fromX - (baseWidth / 2);

        //Left arrow
        ctx.beginPath();

        ctx.moveTo(newX, fromY);
        ctx.lineTo(newX + (baseWidth / 2), fromY + (baseWidth / 2));
        ctx.lineTo(newX + (baseWidth / 2), fromY - (baseWidth / 2));
        ctx.lineTo(newX, fromY);

        ctx.fill();
        ctx.closePath();

        //Right arrow
        var newX2 = newX + baseWidth + 2;

        ctx.beginPath();

        ctx.moveTo(newX2, fromY);
        ctx.lineTo(newX2 - (baseWidth / 2), fromY + (baseWidth / 2));
        ctx.lineTo(newX2 - (baseWidth / 2), fromY - (baseWidth / 2));
        ctx.lineTo(newX2, fromY);

        ctx.fill();
        ctx.closePath();
    }

    //Custom comparer to be able to sort the array by the x coordinate.
    function xCoordinatePointComparer(a, b) {
        if (a[0] > b[0])
            return 1;
        if (a[0] == b[0])
            return 0;
        if (a[0] < b[0])
            return -1;
    }

    //Custom comparer to be able to sort the array by the x coordinate.
    function xCoordinatePointComparer(a, b) {
        if (a[0] > b[0])
            return 1;
        if (a[0] == b[0])
            return 0;
        if (a[0] < b[0])
            return -1;
    }

    // Compares 2 values and returns:
    //    1 if a is bigger than b
    //    0 if a is the same as b
    //   -1 if a is smaller than b
    function Compare(a, b) {
        if (a > b)
            return 1;
        if (a == b)
            return 0;
        if (a < b)
            return -1;
    }

    // Compares 2 values and returns:
    //    1 if a is bigger than b
    //    0 if a is the same as b
    //   -1 if a is smaller than b
    function Compare10Percent(a, b) {
        //If we dont have a previous value then we get out of here.
        //If any of the nubmers are not numbers then we get out of here.
        if (isNaN(a) || isNaN(b))
            return;

        //var currentValuePercentage = (((a * 100) / b) - 100);
        var currentValuePercentage = ((b - a) / a) * 100;

        //alert(a + "/" + b + "=" + currentValuePercentage);

        if (currentValuePercentage >= 10)
        //return -1;
            return 1;
        else if (currentValuePercentage <= -10)
        //return 1;
            return -1;
        else
            return 0;
    }

    $.plot.plugins.push({
        init: init,
        options: options,
        name: 'trends',
        version: '1.0'
    });
})(jQuery);