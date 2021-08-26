zk.$package('com.idera.sqldm.d3zk.chart');
com.idera.sqldm.d3zk.chart.BarChart = zk.$extends(com.idera.sqldm.d3zk.chart.RectangularChart, {

	_orient: "vertical",
	
	_innerPadding: 0,
	_outerPadding: 0,
	_series: { },
	
	_hasRendered: false,
	
	_animateBars: true,
	_animationDelay: 50,
	_animationLength: 500,
	
	_currentClientWidth: 0,
	_currentClientHeight: 0,
	
	_showValueLabels: true,
	_valueFormat: "",

	getOrient: function() { 
		return this._orient;
	},
	
	setOrient: function(value) { 
		if( value != this._orient ) { 
			this._orient = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	getHasRendered: function() { 
		return this._hasRendered;
	},
	
	setHasRendered: function(value) { 
		this._hasRendered = value;
	},
	
	getInnerPadding: function() { 
		return this._innerPadding;
	},

	setInnerPadding: function(value) { 
		if( value != this._innerPadding ) { 
			this._innerPadding = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	getOuterPadding: function() { 
		return this._outerPadding;
	},
	
	setOuterPadding: function(value) {
		if( value != this._outerPadding ) { 
			this._outerPadding = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},

	getSeries: function() { 
		return this._series;
	},
	
	setSeries: function(value) { 
		this._series = value;
		if( this._hasRendered ) this.rerenderChart();
	},
	

	
	getAnimateBars: function() { 
		return this._animateBars;
	},
	
	setAnimateBars: function(value) { 
		if( value != this._animateBars ) {
			this._animateBars = value;
		}
	},
	
	getAnimationDelay: function() {
		return this._animationDelay;
	},
	
	setAnimationDelay: function(value) {
		if( value != this._animationDelay ) {
			this._animationDelay = value;
		}
	},
	
	getAnimationLength: function() {
		return this._animationLength;
	},
	
	setAnimationLength: function(value) {
		if( value != this._animationLength ) {
			this._animationLength = value;
		}
	},
	
	getShowValueLabels: function() {
		return this._showValueLabels;
	},
	
	setShowValueLabels: function(value) {
		if( value != this._showValueLabels ) { 
			this._showValueLabels = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	getValueFormat: function() { 
		return this._valueFormat;
	},
	
	setValueFormat: function(value) { 
		if( value != this._valueFormat ) { 
			this._valueFormat = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},

	getZclass: function(){
		var zcls = this._zclass;
		return zcls ? zcls : "z-d3-barchart";
	},
	
	rerenderChart: function() {
		
		var local = this;
		
		var div = document.getElementById(this.uuid);
		if( div == null ) return;
		
		var svg = d3.select("#" + this.uuid).select("svg");
		if( svg ) svg.remove();
		
		var clientWidth = parseInt(div.clientWidth);
		var clientHeight = parseInt(div.clientHeight);
		
		var margin = this.computeMargins();		
		
		var width = parseInt(clientWidth - margin.left - margin.right);
		var height = parseInt(clientHeight - margin.bottom - margin.top);

		var svg = d3.select("#" + this.uuid).append("svg")
			.attr("width", clientWidth)
			.attr("height", clientHeight)
			.append("g")
			.attr("transform", "translate(" + margin.left + "," + margin.top + ")")
			.attr("width", clientWidth)
			.attr("height", clientHeight);

		var x,y; 
		
		if( this._orient == "horizontal" ) { 
			x = d3.scale.linear().range([0, width]);
			x.domain([0, d3.max(d3.map(this._series).values())]);
			
			y = d3.scale.ordinal().rangeRoundBands([0, height], this._innerPadding, this._outerPadding);	
			y.domain(d3.map(this._series).keys());
		}
		else { 
			x = d3.scale.ordinal().rangeRoundBands([0, width], this._innerPadding, this._outerPadding);
			x.domain(d3.map(this._series).keys());
			
			y = d3.scale.linear().range([height, 0]);
			y.domain([0, d3.max(d3.map(this._series).values())]);
		}

		if( this._showValueLabels ) { 

			var format = d3.format(this._valueFormat);

			if( this._orient == "vertical" ) {
			
				svg.selectAll(".sizeLabel")
					.data(d3.entries(this._series))
					.enter()
					.append("text")
					.transition()
					.delay((this._animateBars ? this._animationDelay + this._animationLength : 0))
					.attr("class","sizeLabel")  	
					.attr("x", function(d){return x(d.key);})
					.attr("y", function(d,i) { return y(d.value); })
					.attr("dy", -5)
					.attr("text-anchor", "beginning") 
					.text(function(d){ return format(d.value); });
				
			}
			else { 
			
				svg.selectAll(".sizeLabel")
					.data(d3.entries(this._series))
					.enter()
					.append("text")
					.attr("class","sizeLabel")  	
					.attr("x", function(d){return x(d.value);})
					.attr("y", function(d,i) { return y(d.key) + (y.rangeBand() / 2); })
					.attr("dx", 5) 
					.attr("dy", ".35em")
					.attr("text-anchor", "beginning") 
					.text(function(d){ return format(d.value); });

				var maxShift = -1;
				
				svg.selectAll("text").each(function(d, i) {
					
					var bbox = d3.select(this).node().getBBox();
					var rhs = bbox.x + bbox.width;
					
					if( rhs > width ) { 
						var overlap = rhs - width;
						if( overlap > maxShift ) maxShift = overlap;
					}

				});
				
				if( maxShift > 0 ) {
					
					width -= maxShift;
					
					x = d3.scale.linear().range([0, width]);
					x.domain([0, d3.max(d3.map(this._series).values())]);
				
					svg.selectAll("text").remove();
					
					svg.selectAll(".sizeLabel")
						.data(d3.entries(this._series))
						.enter()
						.append("text")
						.transition()
						.delay((this._animateBars ? this._animationDelay + this._animationLength : 0))						
						.attr("class","sizeLabel")  	
						.attr("x", function(d){return x(d.value);})
						.attr("y", function(d,i) { return y(d.key) + (y.rangeBand() / 2); })
						.attr("dx", 5) 
						.attr("dy", ".35em")
						.attr("text-anchor", "beginning") 
						.text(function(d){ return format(d.value); });

					
				}
						
			}
			
		}		
		
		var xAxis = d3.svg.axis().scale(x);
		
		if( this._xAxisTickFormat != null ) { 
			xAxis.tickFormat(d3.format(this._xAxisTickFormat));
		}
	
		var yAxis = d3.svg.axis().scale(y).orient("left");
		
		if( this._yAxisTickFormat != null ) { 
			yAxis.tickFormat(d3.format(this._yAxisTickFormat));
		}
		
		yAxis.innerTickSize(this._yAxisInnerTickSize);
		yAxis.outerTickSize(this._yAxisOuterTickSize);
		yAxis.tickPadding(this._yAxisTickPadding);		
		
		var yAxisScale = svg.append("g")
			.attr("class", "y axis")
			.call(yAxis);
		
		yAxisScale.selectAll("g").each(function(d, i) {

			var bbox = d3.select(this).node().getBBox();
			
			while( bbox.width > margin.left ) {
				
				var text = d3.select(this).select("text")[0][0]; 
				text.textContent = text.textContent.substring(0, text.textContent.length-1) + "...";
				if( text.textContent.length <= 4 ) break;
				
				bbox = d3.select(this).node().getBBox();
				if( bbox.width > margin.left ) text.textContent = text.textContent.substring(0, text.textContent.length-3);
			}
			
			if( local._yAxisLabelOrient == "left" ) { 
				d3.select(this).select("text").attr("transform", "translate(" + ((margin.left - bbox.width) * -1) + ",0)");
			}
			
		});

		if( this._yAxisTitle != null ) { 
			yAxisScale.append("text")
				.attr("transform", "rotate(-90)")
				.attr("y", 6)
				.attr("dy", ".71em")
				.style("text-anchor", "end")
				.text(this._yAxisTitle);
		}
		
		var xAxisDetail=svg.append("g")
			.attr("class", "x axis")
			.attr("transform", "translate(0," + height + ")")
			.call(xAxis);

		//Calculate length of x axis label tick
		var maxTickLength=0;

        xAxisDetail.selectAll(".tick text").each(function(d, i) {
            var bbox = d3.select(this).node().getBBox();
            if(bbox.width > maxTickLength){
                maxTickLength = bbox.width;
            }
        });

        var tickGap=10;
		if(maxTickLength > 0){
		    var numberOfXTicks=Math.floor(width/(maxTickLength+tickGap));
		    if(numberOfXTicks > 0 && numberOfXTicks <=10){
		        xAxis.ticks(numberOfXTicks); //+1  is done to add last tick
		        svg.selectAll("g.x.axis")
                        .call(xAxis);
		    }
		}
		var bars = svg.selectAll(".bar").data(d3.entries(this._series));
		
		var startingBarLength = 0;
		var easingFunction = "quad";

		if( this._orient == "vertical" ) {

			if( this._animateBars ) { 
			
				bars.enter().append("rect") 
					.attr("class", "bar")
					.attr("x", function(d) { return x(d.key); })
					.attr("width", x.rangeBand())
					.attr("y", height) 
					.attr("height", startingBarLength);

				bars.transition()
					.ease(easingFunction)
					.duration(this._animationLength)
					.delay(this._animationDelay)
					.attr("height", function(d) { return height - y(d.value); })
					.attr("y", function(d) { return y(d.value); });

			}
			else { 
				bars.enter().append("rect") 
					.attr("class", "bar")	
					.attr("x", function(d) { return x(d.key); })
					.attr("width", x.rangeBand())
					.attr("y", function(d) { return y(d.value); })
					.attr("height", function(d) { return y(d.value); });
			}

		}
		else { 

			if( this._animateBars ) { 

				bars.enter().append("rect")
					.attr("class", "bar")
					.attr("y", function(d) { return y(d.key); })
					.attr("height", y.rangeBand())
					.attr("x", 0)
					.attr("width", startingBarLength);
	
				bars.transition()
					.ease(easingFunction)
					.duration(this._animationLength)
					.delay(this._animationDelay)
					.attr("width", function(d) { return x(d.value); });

			}
			else { 
				bars.enter().append("rect")
					.attr("class", "bar")
					.attr("y", function(d) { return y(d.key); })
					.attr("height", y.rangeBand())
					.attr("x", 0)
					.attr("width", function(d) { return x(d.value); });
			}

		}
		
		var color = this._getColorPalette();
		
		if( color != null ) { 
			svg.selectAll(".bar").data(d3.entries(this._series)).attr("fill", function(d) { return color(d.value); });
		}
		
		this._hasRendered = true;
	}
	
	
});