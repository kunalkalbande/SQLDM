zk.$package('com.idera.sqldm.d3zk.chart');
com.idera.sqldm.d3zk.chart.AreaChart = zk.$extends(com.idera.sqldm.d3zk.chart.XYChart, {

	_animationLength: 500,
	
	getZclass: function(){
		var zcls = this._zclass;
		return zcls ? zcls : "z-d3-areachart";
	},

	getAnimationLength: function() { 
		return this.animationLength;
	},
	
	setAnimationLength: function(value) { 
		if( this._animationLength != value ) { 
			this._animationLength = value;
		}
	},
	
//	_createSeriesFromModel: function() { 
//		
//		if( this._model.length == 0 ) { 
//			this._series = [];
//			return;
//		}
//		
//		var data = d3.map(this._model);
//		var stack = d3.layout.stack()
//		    .offset("zero")
//		    .values(function(d) { return d.values; })
//		    .x(function(d) { return d.category; })
//		    .y(function(d) { return d.value; });
//		
//		this._series = stack(this._color.domain().map(function(seriesName) { 
//			return {
//				seriesName: seriesName,
//				values: data.get(seriesName)
//			};
//		}));
//		
//	},
	
	_getZeroedGenerator: function() { 
		
		var x = this._x, y = this._y, chartHeight = this._chartHeight;
		
		return d3.svg.area()
	    	.x(function(d) { return x(d.category); })
	    	.y0(this._chartHeight)
	    	.y1(function(d) { return chartHeight; });
	},
	
	_getGenerator: function() { 
		
		var x = this._x, y = this._y;
		
		return d3.svg.area()
			.interpolate(this._interpolationMode)
	    	.x(function(d) { return x(d.category); })
	    	.y0(this._chartHeight)
	    	.y1(function(d) { return y(d.value); });
	},
	
	_createPath: function() { 
		
		var color = this._color;
		
		this._path = this._pathGroup.append("path")
			.attr("class", "area")
			.style("fill", function(d) { return color(d.seriesName); });
	},
	
	rerenderChart: function() {
		
		this._preRenderSetup();
		if( this._div == null ) return;
		
		var generator = this._getGenerator();
		this._createPath();
		
		if( this._animateChart ) { 
			
			var seriesGroup = this._seriesGroup,
				easingFunc = this._animationEasingFunction,
				delay = this._animationDelay,
				length = this._animationLength;
			
			var zeroedGenerator = this._getZeroedGenerator();
			this._path.attr("d", function(d) { return zeroedGenerator(d.values); });
			
			seriesGroup.each(function(d) {
				
				var e = d3.select(this);
				
				e.select("path")
					.transition()
					.ease(easingFunc)
					.delay(delay)
					.duration(length)
					.attr("d", function(d) { return generator(d.values); });
				
			});

		}
		else { 
			this._path.attr("d", function(d) { return generator(d.values); });
		}
		
		if(this._drillable){
			this._getXAxisValues();
		}		

		this._postRenderSetup();
	},
//	
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
//		var x = d3.scale.linear().domain([0, maxPoints-1]).range([0, this._chartWidth]);
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