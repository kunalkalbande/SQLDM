if (typeof (QP) == "undefined" || !QP) {
    var QP = {};
};

var myInterval;
function callDrawLines() {
    myInterval = setInterval(function() {
	QP.drawLines();
    }, 1000);
}
(function() {
    /*
     * Draws the lines linking nodes in query plan diagram. root - The document
     * element in which the diagram is contained.
     */
    QP.drawLines = function(root) {
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
			    function() {
				var context = canvasElm.getContext("2d");

				// The first root node may be smaller than the
				// full query plan if using overflow
				var firstNode = $(".qp-tr", root);
				var divQp = $('.divQueryPlan');
				canvasElm.width = Math.max.apply(Math, $(
					'.qp-tr').map(function() {
				    return $(this).outerWidth(true);
				}).get());
				canvasElm.height = divQp.outerHeight(true);
				var offset = canvas.offset();

				$(".qp-tr", root)
					.each(
						function() {
						    var from = $(
							    "> * > .qp-node",
							    $(this));
						    $(
							    "> * > .qp-tr > * > .qp-node",
							    $(this))
							    .each(
								    function() {
									var topOffset = Math.max
										.apply(
											Math,
											$(
												this,
												"div[class^='qp-icon']")
												.map(
													function() {
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
			    }, 1);
	}
    }

    /*
     * Locates or creates the canvas element to use to draw lines for a given
     * root element.
     */
    function getCanvas(root) {
	var returnValue = $("canvas", root);
	if (returnValue.length == 0) {
	    root.prepend($("<canvas></canvas>").css("position", "absolute")
		    .css("top", 0).css("left", 0));
	    returnValue = $("canvas", root);
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
    $(".divQueryPlan").css("transform-origin", "0px 0% 0px");
    $(".divQueryPlan").css("transform", "scale(" + scale + ")");
}
function zoomout() {
    scale = scale - .1;
    $(".divQueryPlan").css("transform-origin", "0px 0% 0px");
    $(".divQueryPlan").css("transform", "scale(" + scale + ")");
}
function zoomtofit() {
    var divQp = $('.divQueryPlan');
    var width = divQp.outerWidth();
    var height = divQp.outerHeight();
    var minW = 820;
    var minH = 350;
    var zoomW = minW / width;
    var zoomH = minH / height;
    if (zoomW < zoomH) {
	scale = zoomW;
    } else {
	scale = zoomH;
    }
    divQp.css("transform-origin", "0px 0% 0px");
    divQp.css("transform", "scale(" + scale + ")");
}

function exportData() {

    var divQp = $('.divQueryPlan');

    // Temporary div to change transformations
    var newDiv = $(divQp).clone();
    $(newDiv).attr("id", "tempDiv");
    var tempCanvasCntxt = $(newDiv.find("canvas"))[0].getContext("2d");
    var orgCanvas = $(divQp).find("canvas");
    tempCanvasCntxt.drawImage($(orgCanvas)[0], 0, 0);

    $(newDiv).css({
	'overflow' : 'visible',
	'height' : 1295,
	'width' : 2528,
	'maxHeight' : 'auto',
	'maxWidth' : 'auto'
    });
    $(newDiv).css("transform-origin", "0px 0% 0px");
    $(newDiv).css("transform", "scale(1)");

    $(newDiv).appendTo("body");

    html2canvas($(newDiv), {
	onrendered : function(canvas) {

	    $(newDiv).remove();

	    var dataUrl = canvas.toDataURL("image/png");
	    zAu.send(new zk.Event(zk.Widget.$('$exportLink'), 'onClickSave',
		    dataUrl.substr(dataUrl.indexOf(',') + 1).toString(), {
			toServer : true
		    }));
	}
    });

}
function drillTime(startTime, endTime) {
   // alert("In drill Time JS file");
    var windowWidget = zk.Widget.$('$queryChartDiv');
    var startAndEnd = startTime + "&" + endTime;
    //alert(startAndEnd);
    zAu.send(new zk.Event(windowWidget, 'onDrillDate', startAndEnd));
}

function drillGraph(legID) {

  //  alert("In Drill Graph JS file");
    var windowWidget = zk.Widget.$('$queryChartDiv');
    //alert("legID"  + legID);
    // var startAndEnd = "2014-10-29 00:12:23.222" + "&" + "2014-11-15
    // 22:12:23.222";
    // alert(startAndEnd.startTime);
    zAu.send(new zk.Event(windowWidget, 'onDrillGraph', legID));

}

function setOffset() {

	var offsetValue = (new Date()).getTimezoneOffset()/60;
    var windowWidget = zk.Widget.$('$queryChartDiv');
    zAu.send(new zk.Event(windowWidget, 'onSetOffset', offsetValue));

}

function setHoursOffset() {

	var offsetValue = (new Date()).getTimezoneOffset()/60;
    var windowWidget = zk.Widget.$('$leftBarDiv');
    zAu.send(new zk.Event(windowWidget, 'onSetOffsetLeftBar', offsetValue));

}

function waitFilter(chartOption, groupId, groupName) {

	var windowWidget = zk.Widget.$('$queryWaitsTabbox');
	zAu.send(new zk.Event(windowWidget, 'onApplyFilter', {"":{'chartOption': chartOption, 'filterId': groupId, 'filterName': groupName}}));

}

function clickBar() {

	var windowWidget = zk.Widget.$('$queryWaitsTabbox');
	zAu.send(new zk.Event(windowWidget, 'onClickGraph', null));

}