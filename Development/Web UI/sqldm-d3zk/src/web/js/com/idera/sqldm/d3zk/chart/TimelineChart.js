zk.$package('com.idera.sqldm.d3zk.chart');
com.idera.sqldm.d3zk.chart.TimelineChart = zk
		.$extends(
				com.idera.sqldm.d3zk.chart.RectangularChart,
				{

					_orient : "horizontal",

					_innerPadding : 0,
					_outerPadding : 0,
					_series : {},

					_hasRendered : false,

					_animateBars : true,
					_animationDelay : 50,
					_animationLength : 500,

					_currentClientWidth : 0,
					_currentClientHeight : 0,

					_showValueLabels : true,
					_showXAxis : false,
					_startDate : null,
					_endDate : null,
					_valueFormat : "",
					_xAxisTitle : null,
					_mouseOverText : null,
					_legendLinks : null,
					_axisLinks : null,
					_colorCodes : null,
					_yAxisTickPadding : 0,
					_yAxis : null,
					_y : null,

					getXAxisTickPadding : function() {
						return this._xAxisTickPadding;
					},

					setXAxisTickPadding : function(value) {
						if (value != this._xAxisTickPadding) {
							this._xAxisTickPadding = value;
							if (this._hasRendered)
								this.rerenderChart();
						}
					},

					getAxisLinks : function() {
						return this._axisLinks;
					},

					setAxisLinks : function(value) {
						this._axisLinks = value;
						if (this._hasRendered)
							this.rerenderChart();
					},

					getLegendLinks : function() {
						return this._legendLinks;
					},

					setLegendLinks : function(value) {
						this._legendLinks = value;
						if (this._hasRendered)
							this.rerenderChart();
					},

					getColorCodes : function() {
						return this._colorCodes;
					},

					setColorCodes : function(value) {
						this._colorCodes = value;
					},

					getMouseOverText : function() {
						return this._mouseOverText;
					},

					setMouseOverText : function(value) {
						if (value != this._mouseOverText) {
							this._mouseOverText = value;
							if (this._hasRendered)
								this.rerenderChart();
						}
					},

					getXAxisTitle : function() {
						return this._xAxisTitle;
					},

					setXAxisTitle : function(value) {
						if (value != this._xAxisTitle) {
							this._xAxisTitle = value;
							if (this._hasRendered)
								this.rerenderChart();
						}
					},

					getOrient : function() {
						return this._orient;
					},

					setOrient : function(value) {
						if (value != this._orient) {
							this._orient = value;
							if (this._hasRendered)
								this.rerenderChart();
						}
					},

					getHasRendered : function() {
						return this._hasRendered;
					},

					setHasRendered : function(value) {
						this._hasRendered = value;
					},

					getInnerPadding : function() {
						return this._innerPadding;
					},

					setInnerPadding : function(value) {
						if (value != this._innerPadding) {
							this._innerPadding = value;
							if (this._hasRendered)
								this.rerenderChart();
						}
					},

					getOuterPadding : function() {
						return this._outerPadding;
					},

					setOuterPadding : function(value) {
						if (value != this._outerPadding) {
							this._outerPadding = value;
							if (this._hasRendered)
								this.rerenderChart();
						}
					},

					getSeries : function() {
						return this._series;
					},

					setSeries : function(value) {
						this._series = value;
						if (this._hasRendered)
							this.rerenderChart();
					},

					getAnimateBars : function() {
						return this._animateBars;
					},

					setAnimateBars : function(value) {
						if (value != this._animateBars) {
							this._animateBars = value;
						}
					},

					getAnimationDelay : function() {
						return this._animationDelay;
					},

					setAnimationDelay : function(value) {
						if (value != this._animationDelay) {
							this._animationDelay = value;
						}
					},

					getAnimationLength : function() {
						return this._animationLength;
					},

					setAnimationLength : function(value) {
						if (value != this._animationLength) {
							this._animationLength = value;
						}
					},

					getShowValueLabels : function() {
						return this._showValueLabels;
					},

					setShowValueLabels : function(value) {
						if (value != this._showValueLabels) {
							this._showValueLabels = value;
							if (this._hasRendered)
								this.rerenderChart();
						}
					},

					getShowXAxis : function() {
						return this._showXAxis;
					},

					setShowXAxis : function(value) {
						if (value != this._showXAxis) {
							this._showXAxis = value;
							if (this._hasRendered)
								this.rerenderChart();
						}
					},
					
					getStartDate : function() {
						return this._startDate;
					},
					
					setStartDate : function(value) {
						this._startDate = value;
					},

					getEndDate : function() {
						return this._endDate;
					},
					
					setEndDate : function(value) {
						this._endDate = value;
					},

					getValueFormat : function() {
						return this._valueFormat;
					},

					setValueFormat : function(value) {
						if (value != this._valueFormat) {
							this._valueFormat = value;
							if (this._hasRendered)
								this.rerenderChart();
						}
					},

					getZclass : function() {
						var zcls = this._zclass;
						return zcls ? zcls : "z-d3-barchart";
					},

					_createYAxis : function() {
						this._yAxis = d3.svg.axis().scale(this._y).orient(
								"left");

						if (this._yAxisTickFormat != null) {
							this._yAxis.tickFormat(d3
									.format(this._yAxisTickFormat));
						}

						this._yAxis.innerTickSize(this._yAxisInnerTickSize);
						this._yAxis.outerTickSize(this._yAxisOuterTickSize);
						this._yAxis.tickPadding(this._yAxisTickPadding);

					},

					rerenderChart : function() {

						var local = this;

						var div = document.getElementById(this.uuid);
						if (div == null)
							return;

						var svg = d3.select("#" + this.uuid).select("svg");
						if (svg)
							svg.remove();

						var clientWidth = parseInt(div.clientWidth);
						var clientHeight = parseInt(div.clientHeight);

						var margin = this.computeMargins();

						var width = parseInt(clientWidth - margin.left
								- margin.right);
						var height = parseInt(clientHeight - margin.bottom
								- margin.top);

						var svg = d3.select("#" + this.uuid).append("svg")
								.attr("width", clientWidth).attr("height",
										clientHeight).append("g").attr(
										"transform",
										"translate(" + margin.left + ","
												+ margin.top + ")").attr(
										"width", clientWidth).attr("height",
										clientHeight);

						var data = d3.map(this._series).get("data");

						var x, y;

						x = d3.scale.linear().range([ 0, width ]);
						this._y = d3.scale.ordinal().rangeRoundBands(
								[ 0, height ], this._innerPadding,
								this._outerPadding);

						x_new = d3.time.scale().range([ 0, width ]);
						var xAxis = d3.svg.axis().scale(x_new).orient("bottom")
								.tickFormat(d3.time.format("%m/%d/%y %H:%M"))
								.ticks(5);

						if (this._xAxisTickFormat != null) {
							xAxis.tickFormat(d3.format(this._xAxisTickFormat));
						}
						xAxis.tickPadding(this._xAxisTickPadding);

						var color = this._getColorPalette();

						color.domain(d3.keys(data[0]).filter(
								function(key) {
									return key !== "series"
											&& key != "categories"
											&& key != "total";
								}));

						data.forEach(function(d) {
							var y0 = 0;
							d.categories = color.domain().map(function(name) {
								return {
									name : name,
									y0 : y0,
									y1 : y0 += +d[name],
									value : d[name]
								};
							});
							d.total = d.categories[d.categories.length - 1].y1;
						});
						// data.sort(function(a, b) { return b.total - a.total;
						// });

						var state = null;
						var xAxisOverlap = false;
						var startingBarLength = 0;
						var easingFunction = "quad";

						this._y.domain(data.map(function(d) {
							return d.series;
						}));
						y = this._y;
						this._createYAxis();
						
						x.domain([ 0, d3.max(data, function(d) {
							return d.total;
						}) ]);
						
						x_new.domain([this._startDate, this._endDate]);
						
/*						x_new.domain([ d3.min(data, function(d) {
											return new Date(d.total);
										}), 
										d3.max(data, function(d) {
											return new Date(d.total);
										})]);
*/						
/*						x_new.domain(d3.extent(data, function(d) { 
							return new Date(d.total); 
						}));*/

						if (this._showXAxis)
							svg.append("g").attr("class", "x axis").attr(
									"transform", "translate(0," + height + ")")
									.call(xAxis);

						state = svg.selectAll(".series").data(data).enter()
								.append("g").attr("class", "g").attr(
										"transform",
										function(d) {
											return "translate(0," + y(d.series)
													+ ")";

										});

						state.append("text").attr("y", 8).attr("x", -20).attr(
								"dy", ".32em").style("text-anchor", "end")
								.text(function(d) {
									return d.series;
								});

						if (this._animateBars) {
							var rect = state
									.selectAll("rect")
									.data(function(d) {
										return d.categories;
									})
									.enter()
									.append("rect")
									.attr("class", "bar")
									.attr("height", this._y.rangeBand())
									.attr("x", height)
									.attr("width", startingBarLength)
									.style(
											"fill",
											function(d) {
												return local._colorCodes[d.name
														.substring(0, d.name
																.indexOf('_'))];
											});

							rect.transition().ease(easingFunction).duration(
									this._animationLength).delay(
									this._animationDelay).attr("width",
									function(d) {
										return x(d.y1) - x(d.y0);
									}).attr("x", function(d) {
								return x(d.y0);
							});

						} else {
							state.selectAll("rect").data(function(d) {
								return d.categories;
							}).enter().append("rect").attr("height",
									this._y.rangeBand()).attr("x", function(d) {
								return x(d.y0);
							}).attr("width", function(d) {
								return x(d.y1) - x(d.y0);
							}).style(
									"fill",
									function(d) {
										return local._colorCodes[d.name
												.substring(0, d.name
														.indexOf('_'))];
									});
						}

						if (this._mouseOverText && this._mouseOverText != "") {

							var div = d3.select("body").append("div") // declare
																		// the
																		// tooltip
																		// div
							.style("position", "absolute").style("text-align",
									"center").style("width", "60px").style(
									"padding", "2px").style("background",
									"white").style("border", "1px").style(
									"border-style", "solid").style(
									"border-color", "black").style(
									"border-radius", "8px").style(
									"pointer-events", "none").style("z-index",
									"10").style("font", "10px Montserrat")
									.style("opacity", 0);

							// Add mouse over
							state
									.selectAll("rect")
									.on(
											"mouseover",
											function(d, i, j) {
												div.transition().duration(200)
														.style("opacity", .9)
														.style("visibility",
																"visible");
												div
														.html(
																local._mouseOverText
																		.replace(
																				"%s",
																				(d.value))
																		+ "<br/>")
														.style(
																"left",
																(d3.event.pageX)
																		+ "px")
														.style(
																"top",
																(d3.event.pageY - 28)
																		+ "px");
											}).on(
											"mouseout",
											function() {
												div.transition().duration(500)
														.style("opacity", 0)
														.style("visibility",
																"hidden");
											});
						}

						this._hasRendered = true;
					},
				});