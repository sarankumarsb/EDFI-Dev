(function ($) {
    function init(plot) {

        function drawHatches(plot, canvascontext, topCoordinate, rightCoordinate, bottomCoordinate, leftCoordinate, offsetCoordinate) {

            //Only if we are drwaing something red then we do hatchets.
            if (canvascontext.strokeStyle == "#b30002") {
                canvascontext.save();

                var flotOptions = plot.getOptions();

                drawPattern(true, canvascontext, topCoordinate, rightCoordinate, bottomCoordinate, leftCoordinate, offsetCoordinate);

                canvascontext.restore();
            }
        }

        plot.hooks.barDrawn.push(drawHatches);
    }

    function drawPattern(forward, c, topCoordinate, rightCoordinate, bottomCoordinate, leftCoordinate, offsetCoordinate) {
        var img = document.getElementById('red');
        var pat = c.createPattern(img, 'repeat');
        c.fillStyle = pat;
        c.fillRect(leftCoordinate, topCoordinate, rightCoordinate - leftCoordinate, bottomCoordinate - topCoordinate);
    }

    var options = {
        series: {
            hatchStyle: 'None', //Available options are: None, WideDownwardDiagonal, WideUpwardDiagonal... More can be added...
            hatchColor: '#fff' //The color of the lines to be drawn.  
        }
    };

    $.plot.plugins.push({
        init: init,
        options: options,
        name: "disability",
        version: "0.1"
    });

})(jQuery);