.tagName {
	float: left;
	background-color: #bbb;
	border-radius: 4px;
	font-size: 10px;
	padding: 2px 4px;
	color:#3A78B2;
	margin: 2px;
	vertical-align: middle;
	white-space: nowrap;
    line-height: 18px;
    text-align: center;
}

.tags {
	display: block;
}

.tag {
	font-size: 13px;
	color:#3A78B2;
	margin: 2px;
	font-weight: bold;
}

.tag .z-label {
 float:left;
 line-height:2;
}


.heatmapView .z-hlayout-inner {
	width: 100%;
}

.z-macro {
	width:100%;
}

.serverPopup {
	position: relative;
    display: block;
    min-width: 200px;
    min-height: 50px;
    color: #eee;
    background: #666;
    border: 1px solid #000;
    -webkit-border-radius: 4px;
    -moz-border-radius: 4px;
    -ms-border-radius: 4px;
    -o-border-radius: 4px;
    border-radius: 4px;
    -webkit-box-shadow: 0 1px 0 #fff inset,0 2px 10px #000;
    -moz-box-shadow: 0 1px 0 #fff inset,0 2px 10px #000;
    box-shadow: 0 1px 0 #fff inset,0 2px 10px #000;
    background: -webkit-gradient(linear, 50% 0%, 50% 25, color-stop(0%, #666666), color-stop(100%, #000000));
    background: -webkit-linear-gradient(#666666 0%,#000000 25px);
    background: -moz-linear-gradient(#666666 0%,#000000 25px);
    background: -o-linear-gradient(#666666 0%,#000000 25px);
    background: linear-gradient(#666666 0%,#000000 25px);
    border-bottom: 1px solid #333;
    filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#666666', endColorstr='#000000');
    -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorstr='#666666', endColorstr='#000000')";
    line-height: 1.3em;
    font-size: 11px;
}

.white {
	border: 1px solid #555;
    border-top: 1px solid #444;
    font-size: 11px;
    -webkit-box-shadow: #333 0 5px 25px,#fff 0 1px 0 inset;
    -moz-box-shadow: #333 0 5px 25px,#fff 0 1px 0 inset;
    box-shadow: #333 0 5px 25px,#fff 0 1px 0 inset;
    color: black;
    text-shadow: none;
    background: #fff;
    filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#ffffff', endColorstr='#ffffff');
    -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorstr='#ffffff', endColorstr='#ffffff')";
}


.serverPopup .serverHeatMapHeader {
	overflow: hidden;
	*zoom:1;
	margin:0px;
    -moz-border-radius-topleft: 3px;
    -webkit-border-top-left-radius: 3px;
    border-top-left-radius: 3px;
    -moz-border-radius-topright: 3px;
    -webkit-border-top-right-radius: 3px;
    border-top-right-radius: 3px;
}

.serverPopup div.serverHeatMapHeader .state_ok{
 	border-top: 1px solid #8ef843;
    background: -webkit-gradient(linear, 50% 0%, 50% 100%, color-stop(0%, #59cd08), color-stop(50%, #449c06), color-stop(100%, #2f6b04));
    background: -webkit-linear-gradient(#59cd08 0%,#449c06 50%,#2f6b04 100%);
    background: -moz-linear-gradient(#59cd08 0%,#449c06 50%,#2f6b04 100%);
    background: -o-linear-gradient(#59cd08 0%,#449c06 50%,#2f6b04 100%);
    background: linear-gradient(#59cd08 0%,#449c06 50%,#2f6b04 100%);
    filter: progid:DXImageTransform.Microsoft.Gradient(GradientType=0, startColorstr='#59cd08', endColorstr='#2f6b04');
    -webkit-box-shadow: 0 -1px 1px #0e2101 inset;
    -moz-box-shadow: 0 -1px 1px #0e2101 inset;
    box-shadow: 0 -1px 1px #0e2101 inset;
    border-bottom: 1px solid #fff;
}

.serverPopup .z-vlayout-inner {
	position: relative;
}
serverHeatMapHeader .label {
	color: #000;
    font-weight: bold;
    font-size: 14px;
}

.serverHeatMapHeader .healthLabel,.serverHeatMapHeader .lastUpdatedLabel {
	color: #000;
    line-height: 1.2em;
    opacity: 0.75;
    font-size: 10px;
}

.serverHeatMapHeader .unseen {
    text-align: center;
    margin-top: 10px;
}

.heatmapContainer {
    border-top: 1px solid #444;
    border-left: 1px solid #444;
    border-right: 1px solid #ddd;
    border-bottom: 1px solid #ddd;
    -webkit-border-radius: 4px;
    -moz-border-radius: 4px;
    -ms-border-radius: 4px;
    -o-border-radius: 4px;
    border-radius: 4px;
    padding: 4px;
    display: block;
    text-shadow: none !important;
}

.heatmapContainer svg {
    display: block;
}

.heatmapContainer .box {
    cursor: pointer;
}

.heatmapContainer .box:hover {
    opacity: 0.75;
}

.heatmapContainer .box.nohilight:hover {
    opacity: 1;
}

.heatmapContainer .box.frozen:hover {
    opacity: 1;
}

.heatmapContainer h2 {
    font-size: 14px;
    line-height: 1.2em;
    margin-bottom: 0.5em;
}

.hoverBoxContainerOuter {
    position: absolute;
    z-index: 10;
}

.hoverBoxContainerOuter img.arrowIcon {
    position: absolute;
    z-index: 1000;
    top: -8px;
}

.hoverBoxContainerOuter img.arrowIconDown {
    position: absolute;
    z-index: 1000;
    bottom: -8px;
}


.hoverBoxContainerOuter .close {
    position: absolute;
    color: #fff;
    top: 8px;
    right: 8px;
    font-size: 10px;
    text-decoration: underline;
    cursor: pointer;
    z-index:11;
}

.hoverBoxContainerOuter .launchDm {
    position: absolute;
    color: #fff;
    top: 27px;
    right: 8px;
    font-size: 10px;
    text-decoration: underline;
    cursor: pointer;
    z-index:11;
}

.hoverBoxContainer {
    position: relative;
    display: block;
    min-width: 200px;
    min-height: 50px;
    color: #eee;
    padding: 15px;
    background: #666;
    border: 1px solid #000;
    -webkit-border-radius: 4px;
    -moz-border-radius: 4px;
    -ms-border-radius: 4px;
    -o-border-radius: 4px;
    border-radius: 4px;
    -webkit-box-shadow: 0 1px 0 #fff inset,0 2px 10px #000;
    -moz-box-shadow: 0 1px 0 #fff inset,0 2px 10px #000;
    box-shadow: 0 1px 0 #fff inset,0 2px 10px #000;
    background: -webkit-gradient(linear, 50% 0%, 50% 25, color-stop(0%, #666666), color-stop(100%, #000000));
    background: -webkit-linear-gradient(#666666 0%,#000000 25px);
    background: -moz-linear-gradient(#666666 0%,#000000 25px);
    background: -o-linear-gradient(#666666 0%,#000000 25px);
    background: linear-gradient(#666666 0%,#000000 25px);
    border-bottom: 1px solid #333;
    filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#666666', endColorstr='#000000');
    -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorstr='#666666', endColorstr='#000000')";
    line-height: 1.3em;
    font-size: 11px;
}

.hoverBoxContainer.white {
    border: 1px solid #555;
    border-top: 1px solid #444;
    font-size: 11px;
    -webkit-box-shadow: #333 0 5px 25px,#fff 0 1px 0 inset;
    -moz-box-shadow: #333 0 5px 25px,#fff 0 1px 0 inset;
    box-shadow: #333 0 5px 25px,#fff 0 1px 0 inset;
    color: black;
    text-shadow: none;
    background: #fff;
    filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#ffffff', endColorstr='#ffffff');
    -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorstr='#ffffff', endColorstr='#ffffff')";
}

.hoverBoxContainer hr {
    border: 0;
    border-top: 1px solid #ccc;
}

.serverHeatMapHeader.state_ok {
    border-top: 1px solid #8ef843;
    background: -webkit-gradient(linear, 50% 0%, 50% 100%, color-stop(0%, #59cd08), color-stop(50%, #449c06), color-stop(100%, #2f6b04));
    background: -webkit-linear-gradient(#59cd08 0%,#449c06 50%,#2f6b04 100%);
    background: -moz-linear-gradient(#59cd08 0%,#449c06 50%,#2f6b04 100%);
    background: -o-linear-gradient(#59cd08 0%,#449c06 50%,#2f6b04 100%);
    background: linear-gradient(#59cd08 0%,#449c06 50%,#2f6b04 100%);
    filter: progid:DXImageTransform.Microsoft.Gradient(GradientType=0, startColorstr='#59cd08', endColorstr='#2f6b04');
    -webkit-box-shadow: 0 -1px 1px #0e2101 inset;
    -moz-box-shadow: 0 -1px 1px #0e2101 inset;
    box-shadow: 0 -1px 1px #0e2101 inset;
    border-bottom: 1px solid #fff;
}

.serverHeatMapHeader.state_warning {
    border-top: 1px solid #feeaad;
    background: -webkit-gradient(linear, 50% 0%, 50% 100%, color-stop(0%, #fdd148), color-stop(50%, #fcc416), color-stop(100%, #dca703));
    background: -webkit-linear-gradient(#fdd148 0%,#fcc416 50%,#dca703 100%);
    background: -moz-linear-gradient(#fdd148 0%,#fcc416 50%,#dca703 100%);
    background: -o-linear-gradient(#fdd148 0%,#fcc416 50%,#dca703 100%);
    background: linear-gradient(#fdd148 0%,#fcc416 50%,#dca703 100%);
    filter: progid:DXImageTransform.Microsoft.Gradient(GradientType=0, startColorstr='#fdd148', endColorstr='#dca703');
    -webkit-box-shadow: 0 -1px 1px #916e02 inset;
    -moz-box-shadow: 0 -1px 1px #916e02 inset;
    box-shadow: 0 -1px 1px #916e02 inset;
    border-bottom: 1px solid #fff;
}

.serverHeatMapHeader.state_maintenancemode {
    border-top: 1px solid #ffc176;
    background: -webkit-gradient(linear, 50% 0%, 50% 100%, color-stop(0%, #ff8c00), color-stop(50%, #e67e00), color-stop(100%, #cc7000));
    background: -webkit-linear-gradient(#ff8c00 0%,#e67e00 50%,#cc7000 100%);
    background: -moz-linear-gradient(#ff8c00 0%,#e67e00 50%,#cc7000 100%);
    background: -o-linear-gradient(#ff8c00 0%,#e67e00 50%,#cc7000 100%);
    background: linear-gradient(#ff8c00 0%,#e67e00 50%,#cc7000 100%);
    filter: progid:DXImageTransform.Microsoft.Gradient(GradientType=0, startColorstr='#ff8c00', endColorstr='#cc7000');
    -webkit-box-shadow: 0 -1px 1px #420e03 inset;
    -moz-box-shadow: 0 -1px 1px #420e03 inset;
    box-shadow: 0 -1px 1px #420e03 inset;
    border-bottom: 1px solid #fff;
}

.serverHeatMapHeader.state_critical {
    border-top: 1px solid #f87e65;
    background: -webkit-gradient(linear, 50% 0%, 50% 100%, color-stop(0%, #ed310a), color-stop(50%, #bc2708), color-stop(100%, #8b1d06));
    background: -webkit-linear-gradient(#ed310a 0%,#bc2708 50%,#8b1d06 100%);
    background: -moz-linear-gradient(#ed310a 0%,#bc2708 50%,#8b1d06 100%);
    background: -o-linear-gradient(#ed310a 0%,#bc2708 50%,#8b1d06 100%);
    background: linear-gradient(#ed310a 0%,#bc2708 50%,#8b1d06 100%);
    filter: progid:DXImageTransform.Microsoft.Gradient(GradientType=0, startColorstr='#ed310a', endColorstr='#8b1d06');
    color: #fff;
    -webkit-box-shadow: 0 -1px 1px #420e03 inset;
    -moz-box-shadow: 0 -1px 1px #420e03 inset;
    box-shadow: 0 -1px 1px #420e03 inset;
    border-bottom: 1px solid #fff;
}


.serverHeatMapHeader {
    overflow: hidden;
    padding: 15px;
    margin: -15px;
    padding-bottom: 9px;
    padding-top: 9px;
    margin-bottom: 5px;
    -moz-border-radius-topleft: 3px;
    -webkit-border-top-left-radius: 3px;
    border-top-left-radius: 3px;
    -moz-border-radius-topright: 3px;
    -webkit-border-top-right-radius: 3px;
    border-top-right-radius: 3px;
}

a.button {
    white-space: nowrap;
    display: inline-block;
    text-align: center;
    font-size: 13px;
    text-decoration: underline;
    height: 20px;
    line-height: 25px;
    margin-right: 5px;
    cursor: pointer;
    color: #3a78b2;
    -webkit-user-select: none;
    -khtml-user-select: none;
    -moz-user-select: none;
    -o-user-select: none;
    user-select: none;
}