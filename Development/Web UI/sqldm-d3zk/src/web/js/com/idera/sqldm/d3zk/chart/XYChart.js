zk.$package('com.idera.sqldm.d3zk.chart');
com.idera.sqldm.d3zk.chart.XYChart = zk.$extends(com.idera.sqldm.d3zk.chart.RectangularChart, {

	// Specified properties
	_animateChart: true,
	_animationDelay: 50,
	_animationEasingFunction: "linear",
	_colorPalette: "category10",
	_drawHorizontalGridLines: true,
	_horizontalGridLineCount: 10,
	_drawVerticalGridLines: true,
	_verticalGridLineCount: 10,
	_interpolationMode: "linear",
	_model: { },
	_showSeriesLabels: true,
	_truncateSeriesLabels: false,
	_xAxisScaleType: "time",
	_xAxisTickCount: 10,
	_yAxisScaleType: "linear",
	_yAxisTickCount: 10,
	_drawXAxis: true,
	_drawYAxis: true,

	// Computed values
	_chartHeight: 0,
	_chartWidth: 0,
	_clientHeight: 0,
	_clientWidth: 0,
	_color: null,
	_horizontalGridLines: null,
	_div: null,
	_margin: null,
	_series: null,
	_seriesGroup: null,
	_svg: null,
	_verticalGridLines: null,
	_x: null,
	_xAxis: null,
	_xAxisGroup: null,
	_y: null,
	_yAxis: null,
	_yAxisGroup: null,
	_xAxisLegendSpacing:25,
	_drillable:false,
	
	getDrillable:function(){
		return this._drillable;
	},
	
	setDrillable:function(value){
    	if(value != this.drillable){
    		this._drillable = value;
    		if(this._hasRendered) this.rerenderChart();
    	}
    },

    getXAxisLegendSpacing: function() {
            return this._xAxisLegendSpacing;
    },
    
    getDrawXAxis: function() {
    	return this._drawXAxis;
    },
    
    getDrawYAxis: function() {
    	return this._drawYAxis;
    },
    
    setDrawXAxis: function(value){
    	if(value != this._drawXAxis){
    		this._drawXAxis = value;
    		if(this._hasRendered) this.rerenderChart();
    	}
    },
    
    setDrawYAxis: function(value) {
    	if(value != this._drawYAxis){
    		this._drawYAxis = value;
    		if(this._hasRendered) this.rerenderChart();
    	}
    },

    setXAxisLegendSpacing: function(value) {
        if( value != this._xAxisLegendSpacing ) {
            this._xAxisLegendSpacing = value;
            if( this._hasRendered ) this.rerenderChart();
        }
    },
	
	getAnimateChart: function() { 
		return this._animateChart;
	},
	
	setAnimateChart: function(value) { 
		if( value != this._animateChart ) {
			this._animateChart = value;
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
	
	getAnimationEasingFunction: function() { 
		return this._animationEasingFunction;
	},
	
	setAnimationEasingFunction: function(value) {
		if( value != this._animationEasingFunction ) {
			this._animationEasingFunction = value;
		}
	},
	
	getDrawHorizontalGridLines: function() { 
		return this._drawHorizontalGridLines;
	},
	
	setDrawHorizontalGridLines: function(value) { 
		if( value != this._drawHorizontalGridLines ) {
			this._drawHorizontalGridLines = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},

	getHorizontalGridLineCount: function() { 
		return this._horizontalGridLineCount;
	},
	
	setHorizontalGridLineCount: function(value) { 
		if( value != this._horizontalGridLineCount ) {
			this._horizontalGridLineCount = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	getDrawVerticalGridLines: function() { 
		return this._drawVerticalGridLines;
	},
	
	setDrawVerticalGridLines: function(value) { 
		if( value != this._drawVerticalGridLines ) {
			this._drawVerticalGridLines = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	getVerticalGridLineCount: function() { 
		return this._verticalGridLineCount;
	},
	
	setVerticalGridLineCount: function(value) { 
		if( value != this._verticalGridLineCount ) {
			this._verticalGridLineCount = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	
	getInterpolationMode: function() { 
		return this._interpolationMode;
	},
	
	setInterpolationMode: function(value) { 
		if( value != this._interpolationMode ) {
			this._interpolationMode = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	getModel: function() { 
		return this._model;
	},
	
	setModel: function(value) { 
		this._model = value;
		if( this._hasRendered ) this.rerenderChart();
	},	
	
	getShowSeriesLabels: function() { 
		return this._showSeriesLabels;
	},
	
	setShowSeriesLabels: function(value) { 
		if( value != this._showSeriesLabels ) { 
			this._showSeriesLabels = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	getTruncateSeriesLabels: function() { 
		return this._truncateSeriesLabels;
	},
	
	setTruncateSeriesLabels: function(value) { 
		if( value != this._truncateSeriesLabels ) { 
			this._truncateSeriesLabels = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
		
	getXAxisScaleType: function() { 
		return this._xAxisScaleType;
	},
	
	setXAxisScaleType: function(value) {
		if( value != this._xAxisScaleType ) { 
			this._xAxisScaleType = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	getXAxisTickCount: function() {
		return this._xAxisTickCount;
	},
	
	setXAxisTickCount: function(value) {
		if( value != this._xAxisTickCount ) {
			this._xAxisTickCount = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},

	getYAxisScaleType: function() { 
		return this._yAxisScaleType;
	},
	
	setYAxisScaleType: function(value) {
		if( value != this._yAxisScaleType ) { 
			this._yAxisScaleType = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	getYAxisTickCount: function() {
		return this._yAxisTickCount;
	},
	
	setYAxisTickCount: function(value) {
		if( value != this._yAxisTickCount ) {
			this._yAxisTickCount = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	_createHorizontalGridLines: function() { 
		
		if( !this._drawHorizontalGridLines ) return;
		
		var yAxis = d3.svg.axis()
			.scale(this._y)
			.orient("left")
			.ticks(this._horizontalGridLineCount);
		
		yAxis.tickSize(-this._chartWidth, 0, 0);
		yAxis.tickFormat("");
		
		this._horizontalGridLines = this._svg.append("g")
			.attr("class", "horizontal gridLines")
			.call(yAxis);
			
	},
	
	_createSeriesFromModel: function() { 
		
		var data = d3.map(this._model);
		
		this._series = this._color.domain().map(function(seriesName) { 
			return {
				seriesName: seriesName,
				values: data.get(seriesName) != null ? data.get(seriesName) : []
			};
		});
		
	},
	
	_createSeriesGroup: function() { 
		
		this._seriesGroup = this._svg.selectAll(".series")
			.data(this._series)
			.enter()
			.append("g")
			.attr("class", "series")
			.attr("width", this._chartWidth)
			.attr("height", this._chartHeight);
		
		this._pathGroup = this._seriesGroup.append("g")
			.style("clip-path", "url(#" + this.uuid + "-pathClip)")
			.attr("clip-path", "url(#" + this.uuid + "-pathClip)");
		
		if( this._showSeriesLabels ) {
			this._appendSeriesLabels();
		}
		
	},
	
	_createSVGElement: function() {

		this._svgElement = d3.select("#" + this.uuid).append("svg")
			.attr("width", this._clientWidth)
			.attr("height", this._clientHeight);

		this._svgElement
			.append("defs")
			.append("clipPath")
			.attr("id", this.uuid + "-pathClip")
			.append("rect")
			.attr("x", 0)
			.attr("y", 0)
			.attr("width", this._chartWidth)
			.attr("height", this._chartHeight);		
		
		this._svg = this._svgElement 
			.append("g").attr("transform", "translate(" + this._margin.left + "," + this._margin.top + ")");
			//.attr("width", this._clientWidth)
			//.attr("height", this._clientHeight);

	},
	
	_createVerticalGridLines: function() { 
		
		if( !this._drawVerticalGridLines ) return;
		
		var xAxis = d3.svg.axis()
			.scale(this._x)
			.orient("bottom")
			.ticks(this._verticalGridLineCount);
		
		xAxis.tickSize(-this._chartHeight, 0, 0);
		xAxis.tickFormat("");
		
		this._verticalGridLines = this._svg.append("g")
			.attr("class", "vertical gridLines")
			.attr("transform", "translate(0," + this._chartHeight + ")")
			.call(xAxis);
		
	},
	
	_createXAxis: function() { 
		
		if(!this._drawXAxis) return;
		
		this._xAxis = d3.svg.axis()
			.scale(this._x)
			.orient("bottom")
			.ticks(this._xAxisTickCount);
		
		if( this._xAxisTickFormat && this._xAxisTickFormat != "" ) {
		    if(this._xAxisScaleType && this._xAxisScaleType == "time"){
		        this._xAxis.tickFormat(d3.time.format(this._xAxisTickFormat));
		    }else{
		        this._xAxis.tickFormat(this._xAxisTickFormat);
		    }

		}else{
			this._xAxis.tickFormat(d3.time.format('%m-%d %I:%M %p'));
		}
		
		this._xAxisGroup = this._svg.append("g")
			.attr("class", "x axis")
			.attr("transform", "translate(0," + this._chartHeight + ")")
			.call(this._xAxis)
			.selectAll("text")
		    .attr("transform", "rotate(90)")
		    .style("text-anchor", "start");;
			
	},
	
	_createXAxisScale: function() { 
		
		switch(this._xAxisScaleType) { 
			case "linear":
				this._x = d3.scale.linear().range([0, this._chartWidth]);
				break;
			case "pow":
				this._x = d3.scale.pow().range([0, this._chartWidth]);
				break;
			case "log":
				this._x = d3.scale.log().range([0, this._chartWidth]);
				break;
			case "time":
				this._x = d3.time.scale().range([0, this._chartWidth]);
				break;
			default:
				this._x = d3.scale.linear().range([0, this._chartWidth]);
				break;				
		}
		//x domain configurable changes
		if(this._xAxisCustomMinDomainValue && this._xAxisCustomMaxDomainValue){
			this._x.domain([this._xAxisMinDomainValue,this._xAxisMaxDomainValue]);
		}else if(this._xAxisCustomMinDomainValue){
			this._x.domain([this._xAxisMinDomainValue,d3.max(this._series, function(c) { return d3.max(c.values, function(v) { return v.category; }); })]);
		}else if(this._xAxisCustomMaxDomainValue){
			this._x.domain([d3.min(this._series, function(c) { return d3.min(c.values, function(v) { return v.category; }); }),this._xAxisMaxDomainValue]);
		}
		else{
		
			this._x.domain([
				d3.min(this._series, function(c) { return d3.min(c.values, function(v) { return v.category; }); }),
				d3.max(this._series, function(c) { return d3.max(c.values, function(v) { return v.category; }); })
			]);
		}

		/*this._x.domain([
			d3.min(this._series, function(c) { return d3.min(c.values, function(v) { return v.category; }); }),
			d3.max(this._series, function(c) { return d3.max(c.values, function(v) { return v.category; }); })
		]);*/
	},
	
	_createYAxis: function() { 
		
		if(!this._drawYAxis) return;
		
		this._yAxis = d3.svg.axis()
			.scale(this._y)
			.orient("left")
			.ticks(this._yAxisTickCount);
		
		if( this._yAxisTickFormat && this._yAxisTickFormat != "" ) { 
			this._yAxis.tickFormat(this._yAxisTickFormat);
		}

		this._yAxisGroup = this._svg.append("g")
			.attr("class", "y axis")
			.call(this._yAxis);
		
		var maxWidth = 1;

		this._yAxisGroup.selectAll("text").each(function(d, i) {
			var bbox = d3.select(this).node().getBBox();
			if( bbox.width > maxWidth ) maxWidth = bbox.width;
		});

		if( maxWidth >= 0 ) {
			this._transformChartSvgX(maxWidth);
		}
			
	},
	
	_createYAxisScale: function(height) { 
		
		switch(this._yAxisScaleType) {
			case "linear": 
				this._y = d3.scale.linear().range([this._chartHeight, 0]);
				break;
			case "pow":
				this._y = d3.scale.pow().range([this._chartHeight, 0]);
				break;
			case "log":
				this._y = d3.scale.log().range([this._chartHeight, 0]);
				break;
			case "time":
				this._y = d3.time.scale().range([this._chartHeight, 0]);
				break;
			default:
				this._y = d3.scale.linear().range([this._chartHeight, 0]);
				break;				
		}
		
		if(this._yAxisCustomMinDomainValue && this._yAxisCustomMaxDomainValue){
			this._y.domain([this._yAxisMinDomainValue,this._yAxisMaxDomainValue]);
		}else if(this._yAxisCustomMinDomainValue){
			this._y.domain([this._yAxisMinDomainValue,d3.max(this._series, function(c) { return d3.max(c.values, function(v) { return v.value; }); })]);
		}else if(this._yAxisCustomMaxDomainValue){
			this._y.domain([d3.min(this._series, function(c) { return d3.min(c.values, function(v) { return v.value; }); }),this._yAxisMaxDomainValue]);
		}
		else{

			this._y.domain([
				d3.min(this._series, function(c) { return d3.min(c.values, function(v) { return v.value; }); }),
				d3.max(this._series, function(c) { return d3.max(c.values, function(v) { return v.value; }); })
			]);
		}
	},
	
	_transformChartSvgX: function (width) {
		this._chartWidth -= width;
		this._svg.attr("transform", "translate(" + (this._margin.left + width) + "," + this._margin.top + ")");
		this._createXAxisScale();
	},
	
	_transformChartSvgY: function (height) {
		//this._chartHeight -= height;
		this._svg.attr("transform", "translate("+this._margin.left+"," + (this._margin.top - height) + ")");
		this._createYAxisScale();
	},
	
	_appendSeriesLabels: function() {
		var seriesHorizontalGap = 25;
		var verticalGap = this._xAxisLegendSpacing;
		
		var color = this._color;
		var maxLegendWidth = -1;
		var bbox;
		var svgBox = this._svg.node().getBBox();
		var bottomY = svgBox.y + svgBox.height + verticalGap;
		var index = 1;
		var legendCounts = this._seriesGroup[0].length;
		
		if (legendCounts == 0) {
			return;
		}

        var legendSpace = this._chartWidth/legendCounts;

        if(legendCounts == 1){
            legendSpace = legendSpace/2;
        }
//DE42828 - Fixed Y-axis cut off issue
		this._seriesGroup.append("text")
			.datum(function(d) { return { seriesName: d.seriesName, value: d.values[d.values.length - 1]}; })
			.attr("x", function(d,i) { return ((legendSpace/2)+i*legendSpace); })
			.attr("y", function(d) { return bottomY; })
			.attr("dx", "5")
			.attr("dy", ".35em")
			.attr("class", "seriesLabel")
			.text(function(d) { return d.seriesName; })
			.style("fill", function(d) { return color(d.seriesName); });


        var totalLegendWidth = 0;
        this._seriesGroup.selectAll("text").each(function(d, i) {
			bbox = d3.select(this).node().getBBox();
			totalLegendWidth=totalLegendWidth+bbox.width;
		});

//Re-adjust x position based on total Legend width

//This code is required when labels gets cut off along x-axis
        /*if(totalLegendWidth > this._chartWidth){
            legendSpace = (this._chartWidth - totalLegendWidth)/legendCounts;
            var allowedLegendWidth=totalLegendWidth/legendCounts;
            var runningX=0;
            var ind=0;
            this._seriesGroup.selectAll("text").each(function(d, i) {
                var d3This = d3.select(this);
                var oldX=d3.select(this).node().getBBox().x;
                var newX=legendSpace + runningX;
                if(ind==0)
                    newX=legendSpace/2;
                d3This.attr("transform", "translate("+(newX-oldX)+",0)");

                runningX=newX + d3.select(this).node().getBBox().width;
                ind=ind+1;
            });
		 //   this._seriesGroup.selectAll("text").call(this.wrap,legendSpace);
		}else{*/
		var wrapContent=false;
		var allowedLegendWidth=totalLegendWidth/legendCounts;
		if(totalLegendWidth > this._chartWidth){
		    wrapContent = true;
		    var minimumSpace=5;
		    legendSpace = minimumSpace;
		    allowedLegendWidth = (this._chartWidth/legendCounts)-minimumSpace;
		}else{
            legendSpace = (this._chartWidth - totalLegendWidth)/legendCounts;
        }

        if(legendCounts == 1){
        //    legendSpace = legendSpace/2;
        }
        var runningX=0;
        var ind=0;
        this._seriesGroup.selectAll("text").each(function(d, i) {
            var d3This = d3.select(this);
            var oldX=d3.select(this).node().getBBox().x;
            var newX=legendSpace + runningX;
            if(ind==0)
                newX=legendSpace/2;
            d3This.attr("transform", "translate("+(newX-oldX)+",0)");
             if(wrapContent && d3This.node().getComputedTextLength() > allowedLegendWidth){
               var proposedLabel = d3This.text();
                     var proposedLabelArray = proposedLabel.split('').reverse();
                     var word="";

                     var lineNumber = 0,
                     lineHeight = 1.1, // ems
                     x = d3This.attr("x"),
                     y = d3This.attr("y"),
                     dy = parseFloat(d3This.attr("dy")),
                     tspan = d3This.text(null).append("tspan").attr("x", x).attr("y", y).attr("dy", dy + "em");

                     while (proposedLabelArray.length){
                         var letter=proposedLabelArray.pop();
                         tspan.text(word+letter);
                         //word=word+letter;
                         if(tspan.node().getComputedTextLength() <=allowedLegendWidth){
                             word=word+letter;
                         }else{
                            tspan.text(word);
                            word=letter;
                            tspan = d3This.append("tspan").attr("x", (x+5)).attr("y", y).attr("dy", ++lineNumber * lineHeight + dy + "em").text(word);
                         }
                     }
            }
            runningX=newX + d3.select(this).node().getBBox().width;
            ind=ind+1;
        });
		//}

//Check if legends can be displayed in available bottom margin

	},
	
//	_appendSeriesLabels: function() {
//		
//		var color = this._color;
//		var maxLegendWidth = -1;
//		var vertical_gap = 5;
//		var bbox;
//		var leftX = this._svg.node().getBBox().x; // its already a negative value
//		var index = 1;
//		
//		this._seriesGroup.append("text")
//			.datum(function(d) { return { seriesName: d.seriesName, value: d.values[d.values.length - 1]}; })
//			.attr("x", function(d) { return 0; })
//			.attr("y", function(d) { return 0; })
//			.attr("dx", "5")
//			.attr("dy", ".35em")
//			.attr("class", "seriesLabel")
//			.text(function(d) { return d.seriesName; })
//			.style("fill", function(d) { return color(d.seriesName); });
//		
//		this._seriesGroup.selectAll("text").each(function(d, i) {
//			bbox = d3.select(this).node().getBBox();
//			if ( bbox.width > maxLegendWidth ) {
//				maxLegendWidth = bbox.width;
//			}
//		});
//		
//		this._transformChartSvgX(maxLegendWidth - leftX);
//		
//		this._seriesGroup.selectAll("text").each(function(d, i) {
//			var d3This = d3.select(this);
//			bbox = d3This.node().getBBox();
//			d3This.attr("transform", "translate("+(-1 * maxLegendWidth + leftX)+", "+(index * bbox.height+vertical_gap)+")");
//			++index;
//		});
//	},

    wrap:function(text, width)  {
      text.each(function() {
        var text = d3.select(this),
            words = text.text().split(/\s+/).reverse(),
            word,
            line = [],
            lineNumber = 0,
            lineHeight = 1.1, // ems
            x = text.attr("x"),
            y = text.attr("y"),
            dy = parseFloat(text.attr("dy")),
            tspan = text.text(null).append("tspan").attr("x", x).attr("y", y).attr("dy", dy + "em");
        while (word = words.pop()) {
          line.push(word);
          tspan.text(line.join(" "));
          if (tspan.node().getComputedTextLength() > width) {
            line.pop();
            tspan.text(line.join(" "));
            line = [word];
            tspan = text.append("tspan").attr("x", x).attr("y", y).attr("dy", ++lineNumber * lineHeight + dy + "em").text(word);
          }
        }
      });
      },


	_appendSeriesLabels_todelete: function() { 
		
		var color = this._color, x = this._x, y = this._y;
		
		this._seriesGroup.append("text")
			.datum(function(d) { return { seriesName: d.seriesName, value: d.values[d.values.length - 1]}; })
			.attr("x", function(d) { return x(d.value.category); })
			.attr("y", function(d) { return y(d.value.value); })
			.attr("dx", "5")
			.attr("dy", ".35em")
			.attr("class", "seriesLabel")
			.text(function(d) { return d.seriesName; })
			.style("fill", function(d) { return color(d.seriesName); });
		
		if( !this._truncateSeriesLabels ) { 
		
			var maxShift = -1;
			
			this._seriesGroup.selectAll("text").each(function(d, i) {
				
				var bbox = d3.select(this).node().getBBox();
				var rhs = bbox.x + bbox.width;
				
				if( rhs > this._chartWidth) { 
					var overlap = rhs - this._chartWidth;
					if( overlap > maxShift ) maxShift = overlap;
				}

			});
			
			if( maxShift > 0 ) {
				
				this._chartWidth -= maxShift;
				this._createXAxisScale();

				x = this._x;
				
				this._seriesGroup.selectAll("text").remove();
				
				this._seriesGroup.append("text")
					.datum(function(d) { return { seriesName: d.seriesName, value: d.values[d.values.length - 1]}; })
					.attr("x", function(d) { return x(d.value.category); })
					.attr("y", function(d) { return y(d.value.value); })
					.attr("dx", "5")
					.attr("dy", ".35em")
					.attr("class", "seriesLabel")
					.text(function(d) { return d.seriesName; })
					.style("fill", function(d) { return color(d.seriesName); });
			}
			
		}
		else { 

			var margin = this._margin;
			
			this._seriesGroup.selectAll("text").each(function(d, i) {
				
				var node = d3.select(this).node();
				var bbox = node.getBBox();
				
				while( bbox.width > margin.right ) {
					
					node.textContent = node.textContent.substring(0, node.textContent.length-1) + "...";
					if( node.textContent.length <= 4 ) break;
					
					bbox = d3.select(this).node().getBBox();
					if( bbox.width > margin.right ) node.textContent = node.textContent.substring(0, node.textContent.length-3);
				}
			
			});			
			
		}
		
	},
	
	_appendYAxisLabel: function() {
		
		if( this._yAxisTitle == null ) return;
		
		this._yAxisGroup.append("text")
			.attr("transform", "rotate(-90)")
			.attr("y", 6)
			.attr("dy", ".71em")
			.style("text-anchor", "end")
			.text(this._yAxisTitle);
	},
	
	_redrawXAxis: function() {
		
		if(!this._drawXAxis) return;

		this._svg.select(".x.axis").remove();
		
		this._xAxisGroup = this._svg.append("g")
			.attr("class", "x axis")
			.attr("transform", "translate(0," + this._chartHeight + ")")
			.call(this._xAxis);
		
		//Calculate length of x axis label tick
		var maxTickLength=0;

		this._xAxisGroup.selectAll(".tick text").each(function(d, i) {
            var bbox = d3.select(this).node().getBBox();
            if(bbox.width > maxTickLength){
                maxTickLength = bbox.width;
            }
        });

        var tickGap=10;
		if(maxTickLength > 0){
		    var numberOfXTicks=Math.floor(this._chartWidth/(maxTickLength+tickGap));
		    if(numberOfXTicks > 0 && numberOfXTicks <=this._xAxisTickCount){
		    	this._xAxis.ticks(numberOfXTicks); 
		        this._svg.selectAll("g.x.axis")
                        .call(this._xAxis);
		    }
		}
	},
	
	_redrawYAxis: function() {
		
		if(!this._drawYAxis) return;
		
		this._svg.select(".y.axis").remove();
		
		this._yAxisGroup = this._svg.append("g")
			.attr("class", "y axis")
			.call(this._yAxis);
		
		this._appendYAxisLabel();
	},
	
	_preRenderSetup: function() { 
		
		this._div = document.getElementById(this.uuid);
		if( this._div == null ) return;
		
		this._svg = d3.select("#" + this.uuid).select("svg");
		if( this._svg ) this._svg.remove();
		
		this._clientWidth = parseInt(this._div.clientWidth);
		this._clientHeight = parseInt(this._div.clientHeight);
		
		this._margin = this.computeMargins();
		
		this._chartWidth = parseInt(this._clientWidth - this._margin.left - this._margin.right);
		this._chartHeight = parseInt(this._clientHeight - this._margin.bottom - this._margin.top);
		
		this._createSVGElement();
		
		this._createColorPalette();
		
		this._createSeriesFromModel();
		
		this._createXAxisScale();
		this._createYAxisScale();

		this._createYAxis();

		this._createSeriesGroup();
		
		this._createXAxis();
		
	},
	
	_postRenderSetup: function() { 
		
		this._createHorizontalGridLines();
		this._createVerticalGridLines();
		this._redrawXAxis();
		this._redrawYAxis();
	
		this._hasRendered = true;
	},
	
	_getXAxisValues:function(){
		
		var svg = this._svg;
		var div = this._div;
		var x = this._x;
		var y = this._y;
		var chartHeight = this._chartHeight;
		var chartWidth = this._chartWidth;
		var x1,x2;
		var clientHeight = this.__clientHeight;
		var clientWidth = this._clientWidth;
		var xAxis = this._xAxis;
		var yAxis = this._yAxis;
		var origin,destination;
		//svg.on("mousedown",zoomselected);
		var brush = d3.svg.brush()
					    .x(x)
					    .on("brush", brushmove)
					    .on("brushend", brushend);
		
		var brushSVG = svg.append("g")
			.attr("id","drillSelectedArea")
			.attr("class", "brush")
			.style("stroke","#fff")
			.style("fill-opacity",".125")
			.style("shape-rendering","crispEdges")
			.call(brush)
			.selectAll('rect')
			.attr('height', chartHeight);
			
		
		svg.on('mousedown', function(){
		      var e= this;
			  origin =  d3.mouse(e);
			  brush_elm = svg.select(".brush").node();
			  new_click_event = new Event('mousedown');
			  new_click_event.pageX = d3.event.pageX;
			  new_click_event.clientX = d3.event.clientX;
			  new_click_event.pageY = d3.event.pageY;
			  new_click_event.clientY = d3.event.clientY;
			  brush_elm.dispatchEvent(new_click_event);
			});
		function brushmove() {
			  var extent = brush.extent();
			  x1=extent[0];
			  console.log("x1 : "+x1);
			  x2=extent[1];
			  console.log("x2 : "+x2);
		}
		function brushend() {
				
				//d3.select(".brush").call(brush.clear());
			    var windowWidget = zk.Widget.$('$historyPanel');
			    var startAndEnd = x1 + "&" + x2;
			    //alert(startAndEnd);
			    zAu.send(new zk.Event(windowWidget,'onDrillChange', startAndEnd));
			    brushSVG.remove();
	}
	
		/*function zoomselected(){
		      var e = this;
	          origin = d3.mouse(e);
	          rect = d3.select(e).append("rect").style("fill","#ddd").style("fill-opacity", "0.5");
		      origin[0] = Math.max(0, Math.min(chartWidth, origin[0]));
		      origin[1] = Math.max(0, Math.min(chartHeight, origin[1]));
		      d3.select(this).on("mousemove", function() {
		            var m = d3.mouse(e);
		            m[0] = Math.max(0, Math.min(chartWidth, m[0]));
		            m[1] = Math.max(0, Math.min(chartHeight, m[1]));
		            rect.attr("x", Math.min(origin[0], m[0]))
		                .attr("y", Math.min(origin[1], m[1]))
		                .attr("width", Math.abs(m[0] - origin[0]))
		                .attr("height", Math.abs(m[1] - origin[1]));
		          })
		          .on("mouseup", function() {
		             d3.select("body").classed("noselect", false);
		            var m = d3.mouse(e);
		            m[0] = Math.max(0, Math.min(chartWidth, m[0]));
		            m[1] = Math.max(0, Math.min(chartHeight, m[1]));
		            if (m[0] !== origin[0] && m[1] !== origin[1]) {
		            	x1=x.invert(origin[0]);
		            	console.log("x1 : "+x1);
		            	x2=x.invert(m[0]);
		            	console.log("x2 : "+x2);
		            }
		          }, true);
		      rect.remove();
		      d3.event.stopPropagation();
	    }*/
	},	
	
	
});
	