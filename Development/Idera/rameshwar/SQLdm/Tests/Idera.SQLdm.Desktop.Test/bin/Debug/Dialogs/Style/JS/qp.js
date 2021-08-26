if (typeof (QP) == "undefined" || !QP) {
    var QP = {};
};

var myInterval;
function callDrawLines() {

    myInterval = setInterval(function () {

        QP.drawLines();
    }, 100);
}
(function () {

    /*
     * Draws the lines linking nodes in query plan diagram. root - The document
     * element in which the diagram is contained.
     */
    QP.drawLines = function (root) {
        clearInterval(myInterval);

        if (root === null || root === undefined) {
            // Try and find it ourselves
            root = $(".qp-root").parent();
        } else {
            // Make sure the object passed is jQuery wrapped
            root = $(root);
        }
        internalDrawLines(root);
    };

    /* Internal implementaiton of drawLines. */
    function internalDrawLines(root) {
        var canvas = getCanvas(root);
        var canvasElm = canvas[0];


        // Check for browser compatability

        if (canvasElm.getContext !== null && canvasElm.getContext !== undefined) {
            // Chrome is usually too quick with document.ready
            window
                .setTimeout(
                    function () {

                        var context = canvasElm.getContext("2d");

                        // The first root node may be smaller than the
                        // full query plan if using overflow
                        var firstNode = $(".qp-tr", root);
                        var divQp = $('#divQueryPlan');
                        canvasElm.width = Math.max.apply(Math, $(
                            '.qp-tr').map(function () {
                                return $(this).outerWidth(true);
                            }).get());
                        canvasElm.height = divQp.outerHeight(true);
                        var offset = canvas.offset();

                        $(".qp-tr", root)
                            .each(
                                function () {

                                    var from = $(
                                        "> * > .qp-node",
                                        $(this));
                                    $(
                                        "> * > .qp-tr > * > .qp-node",
                                        $(this))
                                        .each(
                                            function () {
                                                var topOffset = Math.max
                                                    .apply(
                                                        Math,
                                                        $(
                                                            this,
                                                            "div[class^='qp-icon']")
                                                            .map(
                                                                function () {
                                                                    return $(
                                                                        this)
                                                                        .offset().top;
                                                                })
                                                            .get());
                                                topOffset += $(
                                                    $(
                                                        this,
                                                        "div[class^='qp-icon']")[0])
                                                    .outerHeight() / 2;
                                                drawArrowLine(
                                                    context,
                                                    offset,
                                                    from,
                                                    $(this),
                                                    topOffset);
                                            });
                                });
                        context.stroke();
                        var context = canvasElm.getContext("2d");

                    }, 1);
        }
    }

    /*
     * Locates or creates the canvas element to use to draw lines for a given
     * root element.
     */
    function getCanvas(root) {
        var returnValue = $("#lineCanvas", root);
        if (returnValue.length == 0) {
            root.prepend($("<canvas id='lineCanvas'></canvas>").css("position", "absolute")
                .css("top", 0).css("left", 0).css("font-family", "SegoeUI"));
            returnValue = $("#lineCanvas", root);
        }
        return returnValue;
    }

    /*
     * Draws a line between two nodes. context - The canvas context with which
     * to draw. offset - Canvas offset in the document. from - The document
     * jQuery object from which to draw the line. to - The document jQuery
     * object to which to draw the line.
     */
    function drawLine(context, offset, from, to) {
        var fromOffset = from.offset();
        fromOffset.top += from.outerHeight() / 2;
        fromOffset.left += from.outerWidth();

        var toOffset = to.offset();
        toOffset.top += to.outerHeight() / 2;

        var midOffsetLeft = fromOffset.left / 2 + toOffset.left / 2;

        context.moveTo(fromOffset.left - offset.left, fromOffset.top
            - offset.top);
        context
            .lineTo(midOffsetLeft - offset.left, fromOffset.top
                - offset.top);
        context.lineTo(midOffsetLeft - offset.left, toOffset.top - offset.top);
        context.lineTo(toOffset.left - offset.left, toOffset.top - offset.top);
    }

    function drawArrowLine(context, offset, from, to, topOffset) {
        var fromOffset = from.offset();

        // fromOffset.top +=25;//$( "div[Class^='qp-icon']" )[0].outerHeight() /
        // 2
        fromOffset.top += from.outerHeight() / 2;
        fromOffset.left += from.outerWidth();

        var toOffset = to.offset();
        toOffset.top += to.outerHeight() / 2;

        // for making straight lines
        if (Math.abs(fromOffset.top - toOffset.top) < 15) {
            toOffset.top = fromOffset.top;
            // fromOffset.top = topOffset;
        }
        var midOffsetLeft = fromOffset.left / 2 + toOffset.left / 2;
        context.moveTo(fromOffset.left - offset.left, fromOffset.top
            - offset.top);
        // context.lineTo(midOffsetLeft - offset.left, fromOffset.top -
        // offset.top);

        context.lineTo(fromOffset.left - offset.left + 2, fromOffset.top
            - offset.top - 1);
        context.arcTo(fromOffset.left - offset.left + 1, fromOffset.top
            - offset.top, fromOffset.left - offset.left + 2, fromOffset.top
            - offset.top + 1, 8);
        context.lineTo(fromOffset.left - offset.left, fromOffset.top
            - offset.top);

        context
            .lineTo(midOffsetLeft - offset.left, fromOffset.top
                - offset.top);
        context.lineTo(midOffsetLeft - offset.left, toOffset.top - offset.top);
        context.lineTo(toOffset.left - offset.left, toOffset.top - offset.top);

    }
})();

var scale = 1.0;
function zoomin() {
    scale = scale + .1;
    zoom();
}
function zoom() {
    $('#divQueryPlan').css("transform", "scale(" + scale + ")");
}
function zoomout() {
    scale = scale - .1;
    if (scale <= 0.1) {
        scale = 0.1;
    }
    zoom();
}
function zoomtofit() {
    try {
        if ($('#divQueryPlan').height() > $('#divQueryPlan').width())
            scale = window.innerHeight / $('#divQueryPlan').height();
        else
            scale = window.innerWidth / $('#divQueryPlan').width();
    }
    catch (exception) {
        scale = 0.5;
    }
    $('html').scrollLeft(0);
    $('html').scrollTop(0);
    zoom();
}


function exportData() {

    var divQp = $('#divQueryPlan');

    // Temporary div to change transformations
    var newDiv = $(divQp).clone();
    $(newDiv).attr("id", "tempDiv");
    var tempCanvasCntxt = $(newDiv.find("canvas"))[0].getContext("2d");
    var orgCanvas = $(divQp).find("canvas");

    $(orgCanvas)[0].height = 1;

    tempCanvasCntxt.drawImage($(orgCanvas)[0], 0, 0);

    $(newDiv).css({
        'overflow': 'visible',
        'height': 1295,
        'width': 2528,
        'maxHeight': 'auto',
        'maxWidth': 'auto'
    });
    $(newDiv).css("transform-origin", "0px 0% 0px");
    $(newDiv).css("transform", "scale(1)");

    QP.drawLines($(newDiv));

    $(newDiv).appendTo("body");

    html2canvas($(newDiv), {
        onrendered: function (canvas) {

            $(newDiv).remove();

            QP.drawLines(divQp);

            var dataUrl = canvas.toDataURL("image/png");

            download(dataUrl, "IderaQueryExport.png", "image/png");
        }
    });

}

//////////////////////////////////////////////////////////
//The following handles the LassoZoom feature/////////////
/////////////////////////////////////////////////////////
function lassoZoom() {
    var divQp = document.getElementById('divQueryPlan');
    var canvas = document.getElementById('myDrawcanvas');
    var ctx = canvas.getContext('2d');
    canvas.width = document.documentElement.scrollWidth;
    canvas.height = document.documentElement.scrollHeight;
    var startX;
    var startY;
    var lassoing = false;
    divQp.addEventListener('mousedown', mouseDown, false);
    divQp.addEventListener('mouseup', mouseUp, false);
    divQp.addEventListener('mousemove', mouseMove, false);
    divQp.addEventListener('mouseenter', mouseEnter, false);
    divQp.addEventListener('mouseleave', mouseLeave, false);

    function mouseDown(e) {
        if (e.which === 1) {
            cleanUp();
            $(divQp).css("cursor", "crosshair");
            startX = e.pageX;
            startY = e.pageY;
            $(canvas).show();
            lassoing = true;
        }
    }

    function mouseUp(e) {
        if (e.which === 1) {
            var endX = e.pageX;
            var endY = e.pageY;
            var width = Math.abs(startX - endX);
            var height = Math.abs(startY - endY);

            if (!lassoing || Math.abs(startX - endX) < 20 || Math.abs(startY - endY) < 20) {
                cleanUp();
                return;
            }

            var scaleToHeight = (height / window.innerHeight) > (width / window.innerWidth);
            var lassoScale = scaleToHeight ? window.innerHeight / height : window.innerWidth / width;
            var centerY = (startY + endY) / 2;
            var centerX = (startX + endX) / 2;
            scale *= lassoScale;
            zoom();
            $('html').scrollLeft((centerX * lassoScale) - (window.innerWidth / 2));
            $('html').scrollTop((centerY * lassoScale) - (window.innerHeight / 2));

            cleanUp();
        }
    }

    function cleanUp() {
        startX = 0;
        startY = 0;
        lassoing = false;
        $(canvas).hide();
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        $(divQp).css("cursor", "pointer");
    }

    function mouseMove(e) {
        try {
            if (lassoing) {
                ctx.clearRect(0, 0, canvas.width, canvas.height);
                var width = ((e.pageX - this.offsetLeft) - startX) * (1 / scale);
                var height = ((e.pageY - this.offsetTop) - startY) * (1 / scale);
                var top = startY * (1 / scale);
                var left = startX * (1 / scale);
                ctx.globalAlpha = .5;
                var grd = ctx.createLinearGradient(left, top, width, height);
                grd.addColorStop(0, "#D9D9D9");
                ctx.fillStyle = grd;
                ctx.fillRect(left, top, width, height);
            }
        }
        catch (exception) {
            if (lassoing)
                cleanUp();
        }
    }
    function mouseLeave(e) {
        if (lassoing)
            cleanUp();
    }
    function mouseEnter(e) {
        if (lassoing)
            cleanUp();
    }
}   
