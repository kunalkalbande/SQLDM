.z-d3-barchart .axis line,
.z-d3-linechart .axis path,
.z-d3-linechart .axis line,
.z-d3-areachart .axis path,
.z-d3-areachart .axis line {
  fill: none;
  stroke: #333333;
  stroke-width: 1px;
  shape-rendering: crispEdges;
}

.z-d3-linechart .tick {
  fill: #333333;
}

.z-d3-linechart .line {
  fill: none;
  stroke-width: 2.5px;
  stroke-linecap: butt;
  shape-rendering: geometricPrecision;
}

.z-d3-areachart .gridLines line { 
  stroke: white;
  opacity: .25;
  stroke-dasharray: 0;
  stroke-width: 2px;
}

.z-d3-linechart .gridLines line { 
  stroke: gray;
  opacity: 1;
  stroke-dasharray: 1;
  stroke-width: 0.5px;
}

.z-d3-areachart .tick {
  fill: #333333;
}

.z-d3-areachart .area {
  stroke-width: 0;
}

.z-d3-linechart .seriesLabel,
.z-d3-barchart .seriesLabel,
.z-d3-areachart .seriesLabel { 
  font-family: Montserrat;
  font-weight: normal;
}

.z-d3-areachart text, .z-d3-linechart text, .z-d3-barchart text {
  font-size: 8pt;
  font-family: Montserrat;
  font-weight: normal;
}
