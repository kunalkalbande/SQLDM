zk.$package('com.idera.sqldm.d3zk.chart');
com.idera.sqldm.d3zk.chart.LineChart = zk.$extends(com.idera.sqldm.d3zk.chart.XYChart, {
	
	_animationStepSize: 1,
	_enableMouseOverText: false,
	_customColorMap: null,
	
	getCustomColorMap: function() {
		return this._customColorMap;
	},
	
	setCustomColorMap: function(value) {
		if(value != null) {
			this._customColorMap = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	getEnableMouseOverText: function() {
		return this._enableMouseOverText;
	},
	
	setEnableMouseOverText: function(value) {
		if(value != this._enableMouseOverText) {
			this._enableMouseOverText = value;
		}
	},
	
	getAnimationStepSize: function() { 
		return this._animationStepSize;
	},
	
	setAnimationStepSize: function(value) { 
		if( value != this._animationStepSize ) { 
			this._animationStepSize = value;
		}
	},

	getZclass: function(){
		var zcls = this._zclass;
		return zcls ? zcls : "z-d3-linechart";
	},
	
	_getGenerator: function() { 
		
		var x = this._x, y = this._y;
		
		return d3.svg.line()
			.interpolate(this._interpolationMode)
			.x(function(d) { return x(d.category); })
			.y(function(d) { return y(d.value); });
	},
	
	_createPath: function() { 
		
		var color = this._color;
		var customColorMap = this._customColorMap;
		
		this._path = this._pathGroup.append("path")
			.attr("class", "line")
			.style("stroke", 
					function(d) { 
						if(customColorMap != null)
							return customColorMap[d.seriesName];
						return color(d.seriesName); })
			.style("fill", "none")
			.style("clip-path", "url(#" + this.uuid + "-pathClip)")
			.attr("width", this._chartWidth)
			.attr("height", this._chartHeight);
	},

	
	_createMouseOverText: function() {
		
		var svg = this._svg;
		var x = this._x;
		var y = this._y;
		var y2 = this._chartHeight;
		var series = this._series;
		var color = this._color;
		var customColorMap = this._customColorMap;
		
		var dateMap = {};
		var dateMapArray = new Array();
		
		for(i = 0 ;i < series.length; i++) {
			for(j = 0;j < series[i].values.length; j++) {
				
				var obj = {};
				obj.seriesName = series[i].seriesName;
				obj.value = series[i].values[j].value;
				
				if(!dateMap.hasOwnProperty(series[i].values[j].category)) {
					var array = new Array();
					dateMap[series[i].values[j].category] = array;
					//pushing data into array
					var o = {};
					o.category = series[i].values[j].category;
					o.value = array;
					dateMapArray.push(o);
				}
				dateMap[series[i].values[j].category]
				.push(obj);
			}
		}
		
		
		/*for(var key in dateMap) {
			if(dateMap.hasOwnProperty(key)) {
				var obj = {};
				obj.category = key;
				obj.value = dateMap[key];
				dateMapArray.push(obj);
			}
		}*/
		
		dateMapArray.sort(function(a,b){
			return a.category - b.category;
		});
		
		var tooltipdiv = d3.select("body")
			.append("div")
			.style("position", "absolute")
			.style("text-align", "center")
			.style("padding", "2px")
			.style("background", "#ffffff")
			.style("border-radius", "2px")
			.style("pointer-events", "none")
			.style("z-index", "3000")
			.style("font", "10px Montserrat")
			.style("opacity", 0);
		
		/*svg.append("path") // this is the black vertical line to follow mouse
		  .attr("class","mouseLine")
		  .style("clip-path", "url(#" + this.uuid + "-pathClip)")
		  .style("stroke","orange")
		  .style("stroke-width", "1px")
		  .style("opacity", "0")
		  .style("pointer-events", "none");*/
		
		var mouseMove = function() {
			var bisectCategory = d3.bisector(function(d) { return d.category; }).right;
			var xValue = x.invert(d3.mouse(this)[0]);
			var i = bisectCategory(dateMapArray, xValue);
			var value= dateMapArray[i].value;
			var category = dateMapArray[i].category;
			
			/*d3.select(".mouseLine")
				.style("z-index", "3000")
				.attr("x1",x(category))
				.attr("y1",0)
				.attr("x2",x(category))
				.attr("y2",y2);*/
			var data = ["M" + x(category) +","+ 0 +"L"+ x(category) + "," + y2];
			
			var line = svg.selectAll(".mouseLine");
				
			line.data(data)
				.enter()
				.append("svg:path")
				.attr("class","mouseLine")
				.style("clip-path", "url(#" + this.uuid + "-pathClip)")
				.style("stroke","orange")
				.style("stroke-width", "1px")
				.style("pointer-events", "none");
			
			line.attr("d",data[0]);	
			
			/*d3.select(".mouseLine")
				.attr("d","M" + x(category) +","+ 0 +"L"+ x(category) + "," + y2)
				.style("z-index", "3000");*/
			
			tooltipdiv.style("left", (d3.event.pageX) + "px")     
            		  .style("top", (d3.event.pageY - 28) + "px");
			
			var date = new Date(+category);
			var day = date.getDate();
		    var month = date.getMonth()+1;//month count starts from 0
		    var year = date.getFullYear();
		    var hour = date.getHours();
		    var minutes = date.getMinutes();
		 // adding 0 if number is less than two digit 
		    if(day < 10)
		    	day = "0"+day;
		    if(month < 10)
		    	month = "0"+month;
		    if(hour < 10)
		    	hour = "0" + hour;
		    if(minutes < 10)
		    	minutes = "0" + minutes;
		    
			var html = "Date: "+ year+"-"+ month+ "-" + day+ " " + hour +":"+ minutes + "<br/>";
			for(i = 0; i < value.length ; i++) {
				var textColor;
				if(customColorMap != null)
					textColor = customColorMap[value[i].seriesName];
				else
					textColor = color(value[i].seriesName);
				html += "<span "+ "style=\"color:" + textColor + "\">" +value[i].seriesName +" : " + value[i].value + "</span> <br/>";
			}
			tooltipdiv.html(html);
		};
		
		var mouseOut = function() {
			svg.select(".mouseLine")
				.style("opacity", "0");
			tooltipdiv.style("opacity", "0");
		};
		
		var mouseOver = function() {
			svg.select(".mouseLine")
				.style("opacity", "1");
			tooltipdiv.style("opacity", "0.6");
		};
		
		
		svg.append("svg:rect")
			.attr("width",this._chartWidth)
			.attr("height",this._chartHeight)
			.attr("pointer-events", "all")
			.attr("fill","none")
			.on("mouseout",mouseOut)
			.on("mouseover",mouseOver)
			.on("mousemove",mouseMove);
	},
	// renderPointMarker: function (seriesGroup, index) {
	// 	var x = this._x;
	// 	var y = this._y;
	// 	var color = this._color;

	// 	var marker = this._seriesGroup.append("circle")
	// 		.datum(seriesGroup)
	// 		.attr("cx", function(d) { return x(d.values[index - 1].category); })
	// 		.attr("cy", function(d) { return y(d.values[index - 1].value); })
	// 		.attr("r", "3")
	// 		.attr("class", "seriesMarker");
	// 		//.style("fill", function(d) { return color(seriesGroup.seriesName); });
	// 	var that = this;
	// 	marker.on('mouseover', function(d, i) {
	// 		var rect = that._seriesGroup.append("rect")
	// 			.datum(seriesGroup)
	// 			.attr("x", function(d) { return x(d.values[index - 1].category); })
	// 			.attr("y", function(d) { return y(d.values[index - 1].value); })
	// 			.attr("width", "100")
	// 			.attr("height", "50")
	// 			.style("fill", 'none')
	// 			.style("stroke", color(seriesGroup.seriesName));

	// 		var text = that._seriesGroup.append("text")
	// 			.datum(seriesGroup)
	// 			.attr("x", function(d) { return x(d.values[index - 1].category) + 5; })
	// 			.attr("y", function(d) { return y(d.values[index - 1].value) + 25; })
	// 			.attr("font-size", 15)
	// 			.text(function(d) { return seriesGroup.seriesName  + ": "+ d.values[index - 1].value; })
	// 			.style("fill", color(seriesGroup.seriesName));
	// 			//.style("stroke", color(seriesGroup.seriesName));

	// 		setTimeout(function () {
	// 			rect.remove();
	// 			text.remove();
	// 		}, 2000);
	// 	});
	// },
	
	rerenderChart: function() {
		
		this._preRenderSetup();
		if ( this._series && this._series.length != 0 ) {
		if( this._div == null ) return;
		
		var generator = this._getGenerator();
		
		this._createPath();
		
		if(this._enableMouseOverText) {
			this._createMouseOverText();
		}
		//drilldown function
		if(this._drillable){
			this._getXAxisValues();
		}		
		if( this._animateChart ) { 
			
			var seriesGroup = this._seriesGroup,
				i = 2, 
				n = this._series[0].values.length, 
				stepSize = this._animationStepSize, 
				easingFunc = this._animationEasingFunction;
			
			this._path.attr("d", function(d) { return generator(d.values.slice(0,1)); });
			
			var draw = function(i, n) { 
				
				seriesGroup.each(function(d) {
				
					var e = d3.select(this);
					
					e.select("path")
						.transition()
						.ease(easingFunc)
						.attr("d", function(d) { return generator(d.values.slice(0, i)); });
					
				});

			};

			d3.timer(function() {
				
				draw(i, n);
				
				if( (i += stepSize) > n ) {
					draw(n, n);
					return true;
				}
				
			}, this._animationDelay);

		}
		else { 
			this._path.attr("d", function(d) { return generator(d.values); });
			}
		}
		
		this._postRenderSetup();
	},
	
//	dataUpdated: function() { 
//		
//		this.$supers('dataUpdated', arguments);
//		
//		var generator = this._getGenerator();
//		var maxPoints = 0;
//		var y = this._y;
//		
//		for( var series=0; series < this._series.length; series++ ) { 
//			if( this._series[series].values.length > maxPoints ) { 
//				maxPoints = this._series[series].values.length;
//			}
//		}
//		
//		var tickSize = this._chartWidth / this._xAxisTickCount;
//		
//		var x = d3.scale.linear().domain([0, maxPoints]).range([0, this._chartWidth]);
//		
//		this._path
//			.data(this._series)
//			.attr("d", function(d) { return generator(d.values); })
//			.attr("transform", "translate(" + x(1) + ")")
//			.transition()
//			.duration(2000)
//			.attr("transform", "translate(" + x(0) + ")")
//			;
//
//		this._seriesGroup
//			.selectAll("text")
//			.data(this._series)
//			.datum(function(d) { return { seriesName: d.seriesName, value: d.values[d.values.length - 1]}; })			
//			.transition()
//			.duration(2000)
//			.attr("y", function(d) { return y(d.value.value); })
//			;
//		
//	}
	
});